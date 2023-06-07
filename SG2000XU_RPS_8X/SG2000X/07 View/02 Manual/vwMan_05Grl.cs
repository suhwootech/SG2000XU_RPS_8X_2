using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace SG2000X
{
    public partial class vwMan_05Grl : UserControl
    {
        private int m_iWy = (int)EWay.L;
        private int m_iCol = 0;
        private int m_iRow = 0;
        private int m_iCnt = 0;
        private double m_dStripTick; //190226 ksg
        private int m_iStepMaxCnt = 0;      // 2020.09.08 SungTae : 3 Step Mode 추가

        /// <summary>
        /// 화면에 정보 업데이트 타이머
        /// </summary>
        private Timer m_tmUpdt;

        public vwMan_05Grl()
        {
            InitializeComponent();

            m_tmUpdt = new Timer();
            m_tmUpdt.Interval = 50;
            m_tmUpdt.Tick += _M_tmUpdt_Tick;

            //191025 ksg :
            //if (CData.CurCompany == eCompany.Qorvo)
            //if (CData.CurCompany == eCompany.Qorvo || CData.CurCompany == eCompany.SST) //191202 ksg :
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
                lblGrL_TcDn.Visible = false;
                ckbGrL_TcDn.Visible = false;
            }

            // 2020.09.08 SungTae : 3 Step Mode 추가
            if (CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)  { m_iStepMaxCnt = GV.StepMaxCnt;    }
            else                                                    { m_iStepMaxCnt = GV.StepMaxCnt_3;  }

            if (CDataOption.UseNewClenaer) // syc : new cleaner
            { ckbGrL_TcWKn.Text = "TOP" + "\n" +"CLEANER" + "\n" + "POINT WATER" + "\n" + "OFF"; }

        }

        private void ViewUpdate(int iStepCnt)
        {
            //if (iStepCnt == 12)
            if (CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)      // 2020.10.09 SungTae : Modify
            {
                pnlGrL_R4.Visible = true;
                pnlGrL_R5.Visible = true;
                pnlGrL_R6.Visible = true;
                pnlGrL_R7.Visible = true;
                pnlGrL_R8.Visible = true;
                pnlGrL_R9.Visible = true;
                pnlGrL_R10.Visible = true;
                pnlGrL_R11.Visible = true;
                pnlGrL_R12.Visible = true;
            }
            else
            {
                pnlGrL_R4.Visible = false;
                pnlGrL_R5.Visible = false;
                pnlGrL_R6.Visible = false;
                pnlGrL_R7.Visible = false;
                pnlGrL_R8.Visible = false;
                pnlGrL_R9.Visible = false;
                pnlGrL_R10.Visible = false;
                pnlGrL_R11.Visible = false;
                pnlGrL_R12.Visible = false;
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
                // Grind Left X
                if (CMot.It.Chk_Alr((int)EAx.LeftGrindZone_X))              { lbl_XStat.BackColor = Color.Red;          }
                if (CMot.It.Chk_OP((int)EAx.LeftGrindZone_X) == EAxOp.Idle) { lbl_XStat.BackColor = Color.LightGray;    }
                else                                                        { lbl_XStat.BackColor = Color.Lime;         }
                if (CData.CurCompany == ECompany.JCET)  {   lbl_XPos.Text = CMot.It.Get_FP((int)EAx.LeftGrindZone_X).ToString("0.000"); }
                else                                    {   lbl_XPos.Text = CMot.It.Get_FP((int)EAx.LeftGrindZone_X).ToString();        }

                // Grind Left Y
                if (CMot.It.Chk_Alr((int)EAx.LeftGrindZone_Y))              { lbl_YStat.BackColor = Color.Red;          }
                if (CMot.It.Chk_OP((int)EAx.LeftGrindZone_Y) == EAxOp.Idle) { lbl_YStat.BackColor = Color.LightGray;    }
                else                                                        { lbl_YStat.BackColor = Color.Lime;         }
                if (CData.CurCompany == ECompany.JCET)  {   lbl_YPos.Text = CMot.It.Get_FP((int)EAx.LeftGrindZone_Y).ToString("0.000"); }
                else                                    {   lbl_YPos.Text = CMot.It.Get_FP((int)EAx.LeftGrindZone_Y).ToString();        }
                
                // Grind Left Z
                if (CMot.It.Chk_Alr((int)EAx.LeftGrindZone_Z))              { lbl_ZStat.BackColor = Color.Red;          }
                if (CMot.It.Chk_OP((int)EAx.LeftGrindZone_Z) == EAxOp.Idle) { lbl_ZStat.BackColor = Color.LightGray;    }
                else                                                        { lbl_ZStat.BackColor = Color.Lime;         }
                if (CData.CurCompany == ECompany.JCET)  {   lbl_ZPos.Text = CMot.It.Get_FP((int)EAx.LeftGrindZone_Z).ToString("0.000"); }
                else                                    {   lbl_ZPos.Text = CMot.It.Get_FP((int)EAx.LeftGrindZone_Z).ToString();        }

                //210929 syc : 2004U
                if (CDataOption.Use2004U)
                {
                    int iCheckTbVac = 0;

                    if (CIO.It.aInput[(int)eX.GRDL_Carrier_Vacuum_4U] == 1 && 
                        CIO.It.aInput[(int)eX.GRDL_Unit_Vacuum_4U]    == 1 )
                    {
                        iCheckTbVac = 1;
                    }
                    lblGrL_TblVac.BackColor = GV.CR_X[iCheckTbVac];
                }
                else { lblGrL_TblVac.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDL_TbVaccum]]; }
                //syc end
                


                lblGrL_TblFlw   .BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDL_TbFlow      ]];
                lblGrL_TcDn     .BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDL_TopClnDn    ]];
                lblGrL_TcFlw    .BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDL_TopClnFlow  ]];
                lblGrL_PrbAMP   .BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDL_ProbeAMP    ]];
                lblGrL_SplZig   .BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDL_WheelZig    ]];
                lblGrL_SplPcw   .BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDL_SplPCW      ]];
                lblGrL_SplPcwTmp.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDL_SplPCWTemp  ]];
                lblGrL_GrdFlw   .BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDL_SplWater    ]];
                lblGrL_GrdTmp   .BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDL_SplWaterTemp]];
                lblGrL_BtmFlw   .BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDL_SplBtmWater ]];
                // 190711-maeng Tool Setter 감지
                lblGrL_TSetter.BackColor = GV.CR_X[CIO.It.Get_TS(EWay.L)];

                //210903 syc : 2004U
                if (CDataOption.Use2004U)   {   ckbGrL_Vac.Checked = CIO.It.Get_Y(eY.GRDL_Unit_Vacuum_4U) && CIO.It.Get_Y(eY.GRDL_Carrier_Vacuum_4U);   }
                else                        {   ckbGrL_Vac.Checked = CIO.It.Get_Y(eY.GRDL_TbVacuum);                                                    }

                ckbGrL_Wtr.Checked      = CIO.It.Get_Y(eY.GRDL_TbFlow);
                ckbGrL_Ejt.Checked      = CIO.It.Get_Y(eY.GRDL_TbEjector);
                ckbGrL_TcDn.Checked     = CIO.It.Get_Y(eY.GRDL_TopClnDn);
                ckbGrL_TcWKn.Checked    = CIO.It.Get_Y(eY.GRDL_TopWaterKnife);
                ckbGrL_TcAKn.Checked    = CIO.It.Get_Y(eY.GRDL_TopClnAir);
                ckbGrL_TcW.Checked      = CIO.It.Get_Y(eY.GRDL_TopClnFlow);
            }

            //190319 ksg -> 190325 mjy 수정
            lbl_BMax.Text = CData.PbResultVal[0].dBMax.ToString("F3");
            lbl_BMin.Text = CData.PbResultVal[0].dBMin.ToString("F3");
            lbl_BAvg.Text = CData.PbResultVal[0].dBAvg.ToString("F3");
            lbl_AMax.Text = CData.PbResultVal[0].dAMax.ToString("F3");
            lbl_AMin.Text = CData.PbResultVal[0].dAMin.ToString("F3");
            lbl_AAvg.Text = CData.PbResultVal[0].dAAvg.ToString("F3");

            //20191010 ghk_manual_bcr
            lblGrL_Bcr.Text = CData.Parts[m_iWy].sBcr;

            if (CData.Dev.iCol > 0 && CData.Dev.iRow > 0)
            {
                int iCol = CData.Dev.iCol;
                int iRow = CData.Dev.iRow * CData.Dev.iWinCnt;

                if (CDataOption.Package == ePkg.Strip)
                {
                    if (CData.GrData[m_iWy].aMeaBf.GetLength(0) == iRow && CData.L_GRD.m_abMeaBfErr.GetLength(0) == iRow &&
                        CData.GrData[m_iWy].aMeaBf.GetLength(1) == iCol && CData.L_GRD.m_abMeaBfErr.GetLength(1) == iCol)
                    {
                        for (int i = 0; i < iCol; i++)
                        {
                            for (int j = 0; j < iRow; j++)
                            {
                                if (CData.GrData[m_iWy].aMeaBf != null)
                                {
                                    dgvGrL_Bf.Rows[j].Cells[i].Style.ForeColor  = (CData.L_GRD.m_abMeaBfErr[j, i]) ? Color.Red : Color.Black;
                                    dgvGrL_Bf.Rows[j].Cells[i].Value            = CData.GrData[m_iWy].aMeaBf[j, i];// L_GRD.m_aInspBf[j, i];
                                }

                                if (CData.GrData[m_iWy].aMeaAf != null)
                                {
                                    dgvGrL_Af.Rows[j].Cells[i].Style.ForeColor  = (CData.L_GRD.m_abMeaAfErr[j, i]) ? Color.Red : Color.Black;
                                    dgvGrL_Af.Rows[j].Cells[i].Value            = CData.GrData[m_iWy].aMeaAf[j, i];
                                }
                            } //for Row
                        } //for Column
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
                                        dgvGrL_Bf.Rows[iR].Cells[i].Style.ForeColor = (CData.GrData[m_iWy].aUnit[iU].aErrBf[j, i]) ? Color.Red : Color.Black;
                                        dgvGrL_Bf.Rows[iR].Cells[i].Value           = CData.GrData[m_iWy].aUnit[iU].aMeaBf[j, i];
                                    }

                                    if (CData.GrData[m_iWy].aUnit[iU].aMeaAf != null)
                                    {
                                        dgvGrL_Af.Rows[iR].Cells[i].Style.ForeColor = (CData.GrData[m_iWy].aUnit[iU].aErrAf[j, i]) ? Color.Red : Color.Black;
                                        dgvGrL_Af.Rows[iR].Cells[i].Value           = CData.GrData[m_iWy].aUnit[iU].aMeaAf[j, i];
                                    }
                                } //for Row
                            } //for Unit
                        } //for Column
                    }
                }
            }

            if (CSQ_Man.It.Seq == ESeq.GRL_Grinding)
            {
                // 2021.12.11 lhs Start : CData.L_GRD.m_iIndex 일때만 데이터를 표시되는 문제 수정
                if (CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)  { m_iStepMaxCnt = GV.StepMaxCnt; }
                else                                                    { m_iStepMaxCnt = GV.StepMaxCnt_3; }

                for (int i = 0; i < m_iStepMaxCnt - 1; i++)
                {
                    if (CData.Dev.aData[m_iWy].aSteps[i].bUse)   // m_iWy = (int)EWay.L;
                    {
                        if (i == CData.L_GRD.m_iIndex)  { ((Label)(Controls.Find("lblGrL_R" + (i + 1), true)[0])).BackColor = Color.Lime;       }
                        else                            { ((Label)(Controls.Find("lblGrL_R" + (i + 1), true)[0])).BackColor = Color.Gainsboro;  }

                        ((Label)(Controls.Find("lblGrL_R" + (i + 1) + "Tar",   true)[0])).Text = CData.GrData[m_iWy].aTar[i].ToString();
                        ((Label)(Controls.Find("lblGrL_R" + (i + 1) + "Cnt",   true)[0])).Text = string.Format("{0} / {1}", CData.GrData[m_iWy].aInx[i].ToString("00"), CData.GrData[m_iWy].aCnt[i].ToString("00")); //200824 jhc : //CData.L_GRD.m_aGrd[Index]);
                        ((Label)(Controls.Find("lblGrL_R" + (i + 1) + "One",   true)[0])).Text = CData.GrData[m_iWy].a1Pt[i].ToString("F3");
                        ((Label)(Controls.Find("lblGrL_R" + (i + 1) + "ReCnt", true)[0])).Text = string.Format("{0} / {1}", CData.GrData[m_iWy].aReInx[i].ToString("00"), CData.GrData[m_iWy].aReCnt[i].ToString("00")); //200824 jhc : //CData.L_GRD.m_aReGrd[i].ToString("00"));
                        ((Label)(Controls.Find("lblL_R"   + (i + 1) + "Num",   true)[0])).Text = CData.GrData[m_iWy].aReNum[i].ToString();
                    }
                }

                if (CData.Dev.aData[m_iWy].aSteps[m_iStepMaxCnt - 1].bUse)    // Fine 사용시
                {
                    if (CData.L_GRD.m_iIndex == m_iStepMaxCnt - 1)  { lblGrL_Fi.BackColor = Color.Lime;      }
                    else                                            { lblGrL_Fi.BackColor = Color.Gainsboro; }

                    lblGrL_FiTar.Text   = CData.GrData[m_iWy].aTar[m_iStepMaxCnt - 1].ToString();
                    lblGrL_FiCnt.Text   = string.Format("{0} / {1}", CData.GrData[m_iWy].aInx[m_iStepMaxCnt - 1].ToString("00"),   CData.GrData[m_iWy].aCnt[m_iStepMaxCnt - 1].ToString("00")); //200824 jhc : //CData.L_GRD.m_aGrd[i]);
                    lblGrL_FiOne.Text   = CData.GrData[m_iWy].a1Pt[m_iStepMaxCnt - 1].ToString("F3");
                    lblGrL_FiReCnt.Text = string.Format("{0} / {1}", CData.GrData[m_iWy].aReInx[m_iStepMaxCnt - 1].ToString("00"), CData.GrData[m_iWy].aReCnt[m_iStepMaxCnt - 1].ToString("00")); //200824 jhc : //CData.L_GRD.m_aReGrd[i].ToString("00")
                    lblL_FiNum.Text     = CData.GrData[m_iWy].aReNum[m_iStepMaxCnt - 1].ToString();
                }
            }

            //--------- aSteps[i].bUse 만 표시
            for (int i = 0; i < m_iStepMaxCnt - 1; i++)
            {
                if(CData.Dev.aData[(int)EWay.L].aSteps[i].bUse) ((Panel)(Controls.Find("pnlGrL_R" +(i+1), true)[0])).Visible = true ;
                else                                            ((Panel)(Controls.Find("pnlGrL_R" +(i+1), true)[0])).Visible = false;
            }

            if (CData.Dev.aData[(int)EWay.L].aSteps[m_iStepMaxCnt - 1].bUse) pnlGrL_Fi.Visible = true;
            else                                                             pnlGrL_Fi.Visible = false;
            //---------

            lbl_GrdTime.Text = CData.GrdElp[(int)EWay.L].tsEls.ToString(@"hh\:mm\:ss");
            lbl_DrsTime.Text = CData.DrsElp[(int)EWay.L].tsEls.ToString(@"hh\:mm\:ss");

            // Dressing Count 갱신
            lbl4_UDrs1Cnt.Text = CData.DrData[0].aInx[0] + " / " + CData.DrData[0].aCnt[0];
            lbl4_UDrs2Cnt.Text = CData.DrData[0].aInx[1] + " / " + CData.DrData[0].aCnt[1];

            if (CDataOption.Package == ePkg.Unit)
            {
                lbl_Unit1.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDL_Unit_1_Vacuum]];
                lbl_Unit2.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDL_Unit_2_Vacuum]];
                lbl_Unit3.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDL_Unit_3_Vacuum]];
                lbl_Unit4.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDL_Unit_4_Vacuum]];
            }

            // 2020.11.23 JSKim St
            // Dressing Left Parameter
            if (tabGrL_StripInsp.SelectedIndex == 3)
            {
                //double dDrsAirCut = CData.Whls[(int)EWay.L].dDair;
                //211013 pjh : Device Wheel 사용 시 표시되는 air cut 변경
                double dDrsAirCut = 0;
                if (CDataOption.UseDeviceWheel)
                {
                    dDrsAirCut = CDev.It.a_tWhl[(int)EWay.L].dDair;

                    if (CDataOption.IsDrsAirCutReplace && CData.DrData[0].bDrsR)
                    {
                        dDrsAirCut = CDev.It.a_tWhl[(int)EWay.L].dDairRep;
                    }
                }
                else
                {
                    dDrsAirCut = CData.Whls[(int)EWay.L].dDair;
                    if (CDataOption.IsDrsAirCutReplace && CData.DrData[0].bDrsR)
                    {
                        dDrsAirCut = CData.Whls[(int)EWay.L].dDairRep;
                    }
                }

                lbl4_DrsAir.Text = dDrsAirCut.ToString();

                //if (CData.Whls[0].aNewP != null && CData.Whls[0].aUsedP != null)
                if ((CData.Whls[0].aNewP != null && CData.Whls[0].aUsedP != null) || (CDataOption.UseDeviceWheel && CDev.It.a_tWhl[0].aNewP != null && CDev.It.a_tWhl[0].aUsedP != null))
                {
                    //211013 pjh : Device Wheel 사용 시 표시되는 air cut 변경
                    if (CDataOption.UseDeviceWheel)
                    {
                        if (CData.DrData[0].bDrsR) { CData.DrData[0].aParm = CDev.It.a_tWhl[0].aNewP; }
                        else { CData.DrData[0].aParm = CDev.It.a_tWhl[0].aUsedP; }
                    }
                    else
                    {
                        if (CData.DrData[0].bDrsR) { CData.DrData[0].aParm = CData.Whls[0].aNewP; }
                        else { CData.DrData[0].aParm = CData.Whls[0].aUsedP; }
                    }
                    lbl4_UDrs1Tot.Text = CData.DrData[0].aParm[0].dTotalDep.ToString();
                    lbl4_UDrs1Cyl.Text = CData.DrData[0].aParm[0].dCycleDep.ToString();
                    lbl4_UDrs1Tbl.Text = CData.DrData[0].aParm[0].dTblSpd.ToString();
                    lbl4_UDrs1Spl.Text = CData.DrData[0].aParm[0].iSplSpd.ToString();
                    lbl4_UDrs2Tot.Text = CData.DrData[0].aParm[1].dTotalDep.ToString();
                    lbl4_UDrs2Cyl.Text = CData.DrData[0].aParm[1].dCycleDep.ToString();
                    lbl4_UDrs2Tbl.Text = CData.DrData[0].aParm[1].dTblSpd.ToString();
                    lbl4_UDrs2Spl.Text = CData.DrData[0].aParm[1].iSplSpd.ToString();
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
            btn_H         .Enabled = bVal;
            btn_Wait      .Enabled = bVal;
            btn_Grd       .Enabled = bVal;
            btn_Drs       .Enabled = bVal;
            btn_WaterKnife.Enabled = bVal;
            btn_TbClean   .Enabled = bVal;
            ckbGrL_Vac    .Enabled = bVal;
            ckbGrL_Ejt    .Enabled = bVal;
            ckbGrL_Wtr    .Enabled = bVal;
            ckbGrL_TcDn   .Enabled = bVal;
            ckbGrL_TcWKn  .Enabled = bVal;
            ckbGrL_TcAKn  .Enabled = bVal;
            ckbGrL_TcW    .Enabled = bVal;
            btnGrL_StInsp .Enabled = bVal;
            btnGrL_WhInsp .Enabled = bVal;
            btnGrL_DrInsp .Enabled = bVal;
        }

        /// <summary>
        /// Manual left grind view에 조작 로그 저장 함수
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

            dgvGrL_Bf.Rows.Clear();
            dgvGrL_Bf.Columns.Clear();
            dgvGrL_Af.Rows.Clear();
            dgvGrL_Af.Columns.Clear();

            for (int i = 0; i < iCol; i++)
            {
                dgvGrL_Bf.Columns.Add(i.ToString(), i.ToString());
                dgvGrL_Af.Columns.Add(i.ToString(), i.ToString());
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
                    dgvGrL_Bf.Rows.Add();
                    dgvGrL_Bf.Rows[(k * iRow) + j].DefaultCellStyle.BackColor = tColor;
                    dgvGrL_Af.Rows.Add();
                    dgvGrL_Af.Rows[(k * iRow) + j].DefaultCellStyle.BackColor = tColor;
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
                            if (CData.Dev.aData[0].aPosBf[j, i].bUse)    { iBkColor |= 0x01; }
                            if (CData.Dev.aData[0].aPosBf[j, i].bUse18P) { iBkColor |= 0x02; }
                            if (0 < iBkColor && iBkColor <= 3)
                            {//Leading Strip 측정 포인트 또는 Main Strip 측정 포인트인 경우에만 색상 변경
                                dgvGrL_Bf.Rows[j].Cells[i].Style.BackColor = GV.CELL_BK_COLOR[iBkColor];
                            }
                            iBkColor = 0; //초기화
                            if (CData.Dev.aData[0].aPosAf[j, i].bUse)    { iBkColor |= 0x01; }
                            if (CData.Dev.aData[0].aPosAf[j, i].bUse18P) { iBkColor |= 0x02; }
                            if (0 < iBkColor && iBkColor <= 3)
                            {//Leading Strip 측정 포인트 또는 Main Strip 측정 포인트인 경우에만 색상 변경
                                dgvGrL_Af.Rows[j].Cells[i].Style.BackColor = GV.CELL_BK_COLOR[iBkColor];
                            }
                        }
                        else
                        {
                            if (CData.Dev.aData[0].aPosBf[j, i].bUse)
                            { dgvGrL_Bf.Rows[j].Cells[i].Style.BackColor = Color.Lime; }
                            if (CData.Dev.aData[0].aPosAf[j, i].bUse)
                            { dgvGrL_Af.Rows[j].Cells[i].Style.BackColor = Color.Lime; }
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
                            { dgvGrL_Bf.Rows[iR].Cells[i].Style.BackColor = Color.Lime; }
                            if (CData.Dev.aData[m_iWy].aPosAf[j, i].bUse)
                            { dgvGrL_Af.Rows[iR].Cells[i].Style.BackColor = Color.Lime; }
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

            //210903 syc : 2004U
            if (CDataOption.Use2004U)
            {
                ckbGrL_Vac.Checked = CIO.It.Get_Y(eY.GRDL_Unit_Vacuum_4U) && CIO.It.Get_Y(eY.GRDL_Carrier_Vacuum_4U);
            }
            else
            {
                ckbGrL_Vac.Checked = CIO.It.Get_Y(eY.GRDL_TbVacuum);
            }
            ckbGrL_Wtr  .Checked = CIO.It.Get_Y(eY.GRDL_TbFlow       );
            ckbGrL_Ejt  .Checked = CIO.It.Get_Y(eY.GRDL_TbEjector    );
            ckbGrL_TcDn .Checked = CIO.It.Get_Y(eY.GRDL_TopClnDn     );
            ckbGrL_TcWKn.Checked = CIO.It.Get_Y(eY.GRDL_TopWaterKnife);
            ckbGrL_TcAKn.Checked = CIO.It.Get_Y(eY.GRDL_TopClnAir    );
            ckbGrL_TcW  .Checked = CIO.It.Get_Y(eY.GRDL_TopClnFlow   );

            if (CData.Dev.aData[(int)EWay.L].eGrdMod == eGrdMode.Target)
            {
                SetWriteLanguage(lblGrL_Mod.Name, CLang.It.GetLanguage("Target"));
				// 2021.11.26 lhs Start
				if (CDataOption.UseNewSckGrindProc)
				{
					if (CData.Dev.aData[(int)EWay.L].eBaseOnThick == EBaseOnThick.Mold)     { lblGrL_Mod.Text += " / Mold";     }
					if (CData.Dev.aData[(int)EWay.L].eBaseOnThick == EBaseOnThick.Total)    { lblGrL_Mod.Text += " / Total";    }
				}
				// 2021.11.26 lhs End
			}
			else
            {
                SetWriteLanguage(lblGrL_Mod.Name, CLang.It.GetLanguage("TopDown"));
            }

			// 2020.09.08 SungTae : 3 Step Mode
			if (CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)  { m_iStepMaxCnt = GV.StepMaxCnt;    }
            else                                                    { m_iStepMaxCnt = GV.StepMaxCnt_3;  }

            //200414 ksg : 12 Step  기능 추가
            //for (int i = 0; i < GV.StepMaxCnt - 1; i++)
            for (int i = 0; i < m_iStepMaxCnt - 1; i++)     // 2020.09.08 SungTae : Modify
            {
                ((Panel)(Controls.Find("pnlGrL_R" +(i+1), true)[0])).Visible = CData.Dev.aData[(int)EWay.L].aSteps[i].bUse;
                if (CData.Dev.aData[(int)EWay.L].eGrdMod == eGrdMode.Target)
                {
                    m_dStripTick = CData.Dev.aData[(int)EWay.L].dTotalTh - iSum;
                    if(i == 0) iSum  = m_dStripTick - CData.Dev.aData[(int)EWay.L].aSteps[i].dTotalDep;
                    else       iSum += m_dStripTick - CData.Dev.aData[(int)EWay.L].aSteps[i].dTotalDep;
                    ((Label)(Controls.Find("lblGrL_R" +(i+1) + "ToTh", true)[0])).Text = m_dStripTick.ToString();
                }
                else
                {
                   ((Label)(Controls.Find("lblGrL_R" +(i+1) + "ToTh", true)[0])).Text = CData.Dev.aData[(int)EWay.L].aSteps[i].dTotalDep.ToString();
                }
                ((Label)(Controls.Find("lblGrL_R" +(i+1) + "CyTh", true)[0])).Text = CData.Dev.aData[(int)EWay.L].aSteps[i].dCycleDep.ToString();
                ((Label)(Controls.Find("lblGrL_R" +(i+1) + "Tbl" , true)[0])).Text = CData.Dev.aData[(int)EWay.L].aSteps[i].dTblSpd  .ToString();
                ((Label)(Controls.Find("lblGrL_R" +(i+1) + "Spl" , true)[0])).Text = CData.Dev.aData[(int)EWay.L].aSteps[i].iSplSpd  .ToString();
            }
            // Fine
            //pnlGrL_Fi.Enabled = CData.Dev.aData[(int)EWay.L].aSteps[GV.StepMaxCnt - 1].bUse;
            pnlGrL_Fi.Enabled = CData.Dev.aData[(int)EWay.L].aSteps[m_iStepMaxCnt - 1].bUse;

            //lblGrL_FiToTh.Text = CData.Dev.aData[(int)EWay.L].aSteps[3].dTotalDep.ToString(); //190226 ksg :
            //if (CData.Dev.aData[(int)EWay.L].eGrdMod == eGrdMode.Normal && CData.Dev.aData[(int)EWay.L].aSteps[GV.StepMaxCnt - 1].bUse)
            if (CData.Dev.aData[(int)EWay.L].eGrdMod == eGrdMode.Target && CData.Dev.aData[(int)EWay.L].aSteps[m_iStepMaxCnt - 1].bUse)     // 2020.09.08 SungTae : Modify
            {
                m_dStripTick = CData.Dev.aData[(int)EWay.L].dTotalTh - iSum;
                //iSum += m_dStripTick - CData.Dev.aData[(int)EWay.L].aSteps[GV.StepMaxCnt - 1].dTotalDep;
                iSum += m_dStripTick - CData.Dev.aData[(int)EWay.L].aSteps[m_iStepMaxCnt - 1].dTotalDep;     // 2020.09.08 SungTae : Modify
                lblGrL_FiToTh.Text = m_dStripTick.ToString();
            }
            else
            {
                //lblGrL_FiToTh.Text = CData.Dev.aData[(int)EWay.L].aSteps[GV.StepMaxCnt - 1].dTotalDep.ToString();
                lblGrL_FiToTh.Text = CData.Dev.aData[(int)EWay.L].aSteps[m_iStepMaxCnt - 1].dTotalDep.ToString();     // 2020.09.08 SungTae : Modify
            }

            // 2020.09.08 SungTae : Modify
            //lblGrL_FiCyTh.Text = CData.Dev.aData[(int)EWay.L].aSteps[GV.StepMaxCnt - 1].dCycleDep.ToString();
            //lblGrL_FiTbl .Text = CData.Dev.aData[(int)EWay.L].aSteps[GV.StepMaxCnt - 1].dTblSpd.ToString();
            //lblGrL_FiSpl .Text = CData.Dev.aData[(int)EWay.L].aSteps[GV.StepMaxCnt - 1].iSplSpd.ToString();
            lblGrL_FiCyTh.Text = CData.Dev.aData[(int)EWay.L].aSteps[m_iStepMaxCnt - 1].dCycleDep.ToString();
            lblGrL_FiTbl .Text = CData.Dev.aData[(int)EWay.L].aSteps[m_iStepMaxCnt - 1].dTblSpd.ToString();
            lblGrL_FiSpl .Text = CData.Dev.aData[(int)EWay.L].aSteps[m_iStepMaxCnt - 1].iSplSpd.ToString();

            // Dressing Left Parameter
            // 2020.11.23 JSKim St - 값이 바로 갱신이 안된다는 말이 있어 Timer에서 처리
            //lbl4_DrsAir.Text = CData.Whls[(int)EWay.L].dDair.ToString();
            //if (CDataOption.IsDrsAirCutReplace && CData.DrData[0].bDrsR)
            //{
            //    lbl4_DrsAir.Text = CData.Whls[(int)EWay.L].dDairRep.ToString();
            //}

            //// 프로그램 시작시 현재 사용중인 파라메터 파일 비었을시 에러 방지 
            ////if (CData.DrData[0].aParm != null)
            //if (CData.Whls[0].aNewP != null && CData.Whls[0].aUsedP != null) //190430 ksg : 조건 바꿈
            //{
            //    if (CData.DrData[0].bDrsR) { CData.DrData[0].aParm = CData.Whls[0].aNewP; }
            //    else { CData.DrData[0].aParm = CData.Whls[0].aUsedP; }

            //    lbl4_UDrs1Tot.Text = CData.DrData[0].aParm[0].dTotalDep.ToString();
            //    lbl4_UDrs1Cyl.Text = CData.DrData[0].aParm[0].dCycleDep.ToString();
            //    lbl4_UDrs1Tbl.Text = CData.DrData[0].aParm[0].dTblSpd.ToString();
            //    lbl4_UDrs1Spl.Text = CData.DrData[0].aParm[0].iSplSpd.ToString();
            //    lbl4_UDrs2Tot.Text = CData.DrData[0].aParm[1].dTotalDep.ToString();
            //    lbl4_UDrs2Cyl.Text = CData.DrData[0].aParm[1].dCycleDep.ToString();
            //    lbl4_UDrs2Tbl.Text = CData.DrData[0].aParm[1].dTblSpd.ToString();
            //    lbl4_UDrs2Spl.Text = CData.DrData[0].aParm[1].iSplSpd.ToString();
            //}
            // 2020.11.23 JSKim Ed

            //20191010 ghk_manual_bcr
            if (CDataOption.ManualBcr == eManualBcr.NotUse)
            {
                lblGrL_Bcr.Visible = false;
                btnGrL_Bcr.Visible = false;
            }
            //

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
                  (ESeq == ESeq.INR_Home) || (ESeq == ESeq.ONP_Home    ) || (ESeq == ESeq.GRL_Home     ) || (ESeq == ESeq.GRR_Home) ||
                  (ESeq == ESeq.OFP_Home) || (ESeq == ESeq.DRY_Home    ) || (ESeq == ESeq.OFL_Home)))
            {
                if (!CSQ_Main.It.ChkAllHomeDone())
                {
                    CSQ_Man.It.bBtnShow = true; //190627 ksg :
                    CMsg.Show(eMsg.Warning, "Warning", "First All Home Please");
                    return;
                }
            }

            //20191128 ghk_check_warmup
            if (ESeq == ESeq.GRL_Grinding      || ESeq == ESeq.GRR_Grinding      ||
                ESeq == ESeq.GRL_Strip_Measure || ESeq == ESeq.GRR_Strip_Measure ||
                ESeq == ESeq.GRL_Wheel_Measure || ESeq == ESeq.GRR_Wheel_Measure ||
                ESeq == ESeq.GRL_Manual_GrdBcr || ESeq == ESeq.GRR_Manual_GrdBcr)
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
                if (ESeq == ESeq.GRL_Grinding || ESeq == ESeq.GRL_Manual_GrdBcr || ESeq == ESeq.GRR_Grinding || ESeq == ESeq.GRR_Manual_GrdBcr)
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
            if (ESeq == ESeq.GRL_Dressing      || ESeq == ESeq.GRL_Dresser_Measure ||
                ESeq == ESeq.GRL_Wheel_Measure || ESeq == ESeq.GRL_Table_Grinding)
            {
                //210902 syc : 2004U
                if (CDataOption.Use2004U)
                {
                    //if (CIO.It.Get_Y(eY.GRDL_TbVacuum))
                    if (CIO.It.Get_Y(eY.GRDL_Unit_Vacuum_4U) || CIO.It.Get_Y(eY.GRDL_Carrier_Vacuum_4U) || CData.Parts[(int)EPart.GRDL].bExistStrip == true)  // 2020.12.28 lhs
                    {
                        CSQ_Man.It.bBtnShow = true; //190627 ksg :
                        CMsg.Show(eMsg.Error, "ERROR", "One more check strip on each Table. Please");
                        return;
                    }
                }
                else
                {
                    //if (CIO.It.Get_Y(eY.GRDL_TbVacuum))
                    if (CIO.It.Get_Y(eY.GRDL_TbVacuum) || CData.Parts[(int)EPart.GRDL].bExistStrip == true)  // 2020.12.28 lhs
                    {
                        CSQ_Man.It.bBtnShow = true; //190627 ksg :
                        CMsg.Show(eMsg.Error, "ERROR", "One more check strip on each Table. Please");
                        return;
                    }
                }
            }

            // 200730 myk : Manual Grinding 동작 중 Barcode 입력 기능 추가
            if ((ESeq == ESeq.GRL_Grinding || ESeq == ESeq.GRL_Manual_GrdBcr) && CDataOption.IsGrdBcr)
            {
                using (frmTxt mForm = new frmTxt("Input Strip ID"))
                {
                    if (mForm.ShowDialog() == DialogResult.OK)
                    {
                        CData.Parts[(int)EPart.GRDL].sBcr = mForm.Val;
                    }
                }
            }

            if ((ESeq == ESeq.GRL_Grinding && CData.Opt.aTblSkip[(int)EWay.L]) || (ESeq == ESeq.GRL_Manual_GrdBcr && CData.Opt.aTblSkip[(int)EWay.L]))
            {
                CMsg.Show(eMsg.Error, "Error", "Left Table Skip Now!");
                return;
            }

            if (ESeq == ESeq.GRR_Dressing         || ESeq == ESeq.GRR_Grinding        || ESeq == ESeq.GRR_WaterKnife    ||
                ESeq == ESeq.GRR_Table_Measure    || ESeq == ESeq.GRR_Strip_Measure   || ESeq == ESeq.GRR_Water_Knife   ||
                ESeq == ESeq.GRR_Strip_Measureone || ESeq == ESeq.GRR_Dresser_Measure || ESeq == ESeq.GRR_Wheel_Measure ||
                ESeq == ESeq.GRR_Table_Grinding   || ESeq == ESeq.GRR_Manual_GrdBcr   || ESeq == ESeq.GRR_Manual_Bcr    ||
                ESeq == ESeq.GRL_Dressing         || ESeq == ESeq.GRL_Grinding        || ESeq == ESeq.GRL_WaterKnife    ||
                ESeq == ESeq.GRL_Table_Measure    || ESeq == ESeq.GRL_Strip_Measure   || ESeq == ESeq.GRL_Water_Knife   ||
                ESeq == ESeq.GRL_Strip_Measureone || ESeq == ESeq.GRL_Dresser_Measure || ESeq == ESeq.GRL_Wheel_Measure ||
                ESeq == ESeq.GRL_Table_Grinding   || ESeq == ESeq.GRL_Manual_GrdBcr   || ESeq == ESeq.GRL_Manual_Bcr)
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

            if (ESeq == ESeq.GRL_Dressing)
            { tabGrL_StripInsp.SelectTab(3); }



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
                    if (ESeq == ESeq.GRL_Manual_GrdBcr || ESeq == ESeq.GRL_Strip_Measure)
                    {
                        if (CMsg.Show(eMsg.Query, "Strip Measure Type", "Is This Test Strip(18 Points) Measure?") == DialogResult.OK) //Query 팝업
                        { CData.L_GRD.m_bManual18PMeasure = true; }
                        else
                        { CData.L_GRD.m_bManual18PMeasure = false; }
                    }
                    else
                    {
                        CData.L_GRD.m_bManual18PMeasure = false;
                    }
                }
                else
                {
                    CData.L_GRD.m_bManual18PMeasure = false;
                }
                //2. 설정된 측정 포인트 수 체크
                if (ESeq == ESeq.GRL_Manual_GrdBcr || ESeq == ESeq.GRL_Strip_Measure)
                {
                    string strResult = CData.L_GRD.CheckMeasurePointCount(true); //스트립 측정 포인트 수 체크 : 매뉴얼 동작에서는 반드시 CData.L_GRD.m_bManual18PMeasure 값 설정 후 호출해야 정확한 측정 포인트 수가 파악됨
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
            }

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
        private void ckb_CheckedChanged(object sender, EventArgs e)
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

                case 7:    // IONAZER
                    if (mCkb.Checked)
                        sTxt = CLang.It.GetLanguage("IONIZER") + "\r\n" + CLang.It.GetLanguage("ON");
                    else
                        sTxt = CLang.It.GetLanguage("IONIZER") + "\r\n" + CLang.It.GetLanguage("OFF");

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

            eX eIPRun    = eX.PUMPL_Run;
            // 2020.10.22 JSKim St
            eX eIAddPRun = eX.ADD_PUMPL_Run;
            // 2020.10.22 JSKim Ed
            eY eOPRun    = eY.PUMPL_Run;

            switch (iType)
            {
                case 0:    // Vacuum
                    if (mCkb.Checked)
                    {
                        //201208 jhc : Manual 동작 상호 배제 (Vacuum/Eject vs. Table Water)
                        CData.L_GRD.ActWater(false); //Table Water OFF
                        CData.L_GRD.ActEject(false); //Table Eject OFF
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
                                    CErr.Show(eErr.PUMP_LEFT_RUN_TIMEOUT); return;
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
                                        CErr.Show(eErr.ADD_PUMP_LEFT_RUN_TIMEOUT_ERROR); return;
                                    }
                                    Application.DoEvents();
                                }
                            }
                            // 2020.11.07 lhs End
                        }
                        //210902 syc : 2004U
                        if (CDataOption.Use2004U)
                        {                            
                            CIO.It.Set_Y(eY.GRDL_Unit_Vacuum_4U, true);
                            CIO.It.Set_Y(eY.GRDL_Carrier_Vacuum_4U, true);
                        }
                        else
                        {
                            CIO.It.Set_Y(eY.GRDL_TbVacuum, true);
                        }

                        if (CDataOption.Package == ePkg.Unit)
                        {
                            GU.Delay(3000);

                            string sUnit = "";
                            CIO.It.Set_Y(eY.GRDL_CarrierVacuum, true);
                            GU.Delay(GV.UNIT_VAC_DELAY);
                            if (!CIO.It.Get_X(eX.GRDL_TbVaccum))
                            {
                                CErr.Show(eErr.LEFT_GRIND_UNIT_ALL_VACUUM_NOT_ON_ERROR);
                                return;
                            }

                            if (CData.Dev.iUnitCnt == 4)
                            {
                                // Unit #1
                                CIO.It.Set_Y(eY.GRDL_Unit_1_Vacuum, true);
                                GU.Delay(GV.UNIT_VAC_DELAY);
                                if (CIO.It.Get_X(eX.GRDL_Unit_1_Vacuum))
                                {
                                    CData.GrData[(int)EWay.L].aUnitEx[0] = true;
                                    sUnit += "Unit #1  ";
                                }
                                else
                                {
                                    CData.GrData[(int)EWay.L].aUnitEx[0] = false;
                                    CIO.It.Set_Y(eY.GRDL_Unit_1_Vacuum, false);
                                }

                                // Unit #2
                                CIO.It.Set_Y(eY.GRDL_Unit_2_Vacuum, true);
                                GU.Delay(GV.UNIT_VAC_DELAY);
                                if (CIO.It.Get_X(eX.GRDL_Unit_2_Vacuum))
                                {
                                    CData.GrData[(int)EWay.L].aUnitEx[1] = true;
                                    sUnit += "Unit #2  ";
                                }
                                else
                                {
                                    CData.GrData[(int)EWay.L].aUnitEx[1] = false;
                                    CIO.It.Set_Y(eY.GRDL_Unit_2_Vacuum, false);
                                }

                                // Unit #3
                                CIO.It.Set_Y(eY.GRDL_Unit_3_Vacuum, true);
                                GU.Delay(GV.UNIT_VAC_DELAY);
                                if (CIO.It.Get_X(eX.GRDL_Unit_3_Vacuum))
                                {
                                    CData.GrData[(int)EWay.L].aUnitEx[2] = true;
                                    sUnit += "Unit #3  ";
                                }
                                else
                                {
                                    CData.GrData[(int)EWay.L].aUnitEx[2] = false;
                                    CIO.It.Set_Y(eY.GRDL_Unit_3_Vacuum, false);
                                }

                                // Unit #4
                                CIO.It.Set_Y(eY.GRDL_Unit_4_Vacuum, true);
                                GU.Delay(GV.UNIT_VAC_DELAY);
                                if (CIO.It.Get_X(eX.GRDL_Unit_4_Vacuum))
                                {
                                    CData.GrData[(int)EWay.L].aUnitEx[3] = true;
                                    sUnit += "Unit #4  ";
                                }
                                else
                                {
                                    CData.GrData[(int)EWay.L].aUnitEx[3] = false;
                                    CIO.It.Set_Y(eY.GRDL_Unit_4_Vacuum, false);
                                }
                            }
                            else
                            {
                                // Unit #1
                                CIO.It.Set_Y(eY.GRDL_Unit_1_Vacuum, true);
                                CIO.It.Set_Y(eY.GRDL_Unit_2_Vacuum, true);
                                GU.Delay(GV.UNIT_VAC_DELAY);
                                if (CIO.It.Get_X(eX.GRDL_Unit_1_Vacuum) && CIO.It.Get_X(eX.GRDL_Unit_2_Vacuum))
                                {
                                    CData.GrData[(int)EWay.L].aUnitEx[0] = true;
                                    sUnit += "Unit #1  ";
                                }
                                else
                                {
                                    CData.GrData[(int)EWay.L].aUnitEx[0] = false;
                                    CIO.It.Set_Y(eY.GRDL_Unit_1_Vacuum, false);
                                    CIO.It.Set_Y(eY.GRDL_Unit_2_Vacuum, false);
                                }

                                // Unit #2
                                CIO.It.Set_Y(eY.GRDL_Unit_3_Vacuum, true);
                                CIO.It.Set_Y(eY.GRDL_Unit_4_Vacuum, true);
                                GU.Delay(GV.UNIT_VAC_DELAY);
                                if (CIO.It.Get_X(eX.GRDL_Unit_3_Vacuum) && CIO.It.Get_X(eX.GRDL_Unit_4_Vacuum))
                                {
                                    CData.GrData[(int)EWay.L].aUnitEx[1] = true;
                                    sUnit += "Unit #2  ";
                                }
                                else
                                {
                                    CData.GrData[(int)EWay.L].aUnitEx[1] = false;
                                    CIO.It.Set_Y(eY.GRDL_Unit_3_Vacuum, false);
                                    CIO.It.Set_Y(eY.GRDL_Unit_4_Vacuum, false);
                                }
                            }

                            CMsg.Show(eMsg.Notice, "Notice", "Vacuum success\r\n" + sUnit);
                        }
                    }
                    else
                    {
                        if (CDataOption.Use2004U)
                        {
                            CIO.It.Set_Y(eY.GRDL_Unit_Vacuum_4U, false);
                            CIO.It.Set_Y(eY.GRDL_Carrier_Vacuum_4U, false);
                        }
                        CIO.It.Set_Y(eY.GRDL_TbVacuum, false);
                        CIO.It.Set_Y(eY.GRDL_CarrierVacuum, false);
                        CIO.It.Set_Y(eY.GRDL_Unit_1_Vacuum, false);
                        CIO.It.Set_Y(eY.GRDL_Unit_2_Vacuum, false);
                        CIO.It.Set_Y(eY.GRDL_Unit_3_Vacuum, false);
                        CIO.It.Set_Y(eY.GRDL_Unit_4_Vacuum, false);
                    }

                    break;

                case 1:    // Water
                    if (mCkb.Checked)
                    {
#if true //201208 jhc : Manual 동작 상호 배제 (Vacuum/Eject vs. Table Water)
                        CData.L_GRD.ActEject(false);  //Table Eject OFF
                        CData.L_GRD.ActVacuum(false); //Table Vacuum OFF
#else
                        if (CIO.It.Get_Y(eY.GRDL_TbVacuum))
                        { CIO.It.Set_Y(eY.GRDL_TbVacuum, false); }
#endif
                    }
                    if (CDataOption.Package == ePkg.Unit)
                    {
                        CIO.It.Set_Y(eY.GRDL_CarrierVacuum, mCkb.Checked);
                        CIO.It.Set_Y(eY.GRDL_Unit_1_Vacuum, mCkb.Checked);
                        CIO.It.Set_Y(eY.GRDL_Unit_2_Vacuum, mCkb.Checked);
                        CIO.It.Set_Y(eY.GRDL_Unit_3_Vacuum, mCkb.Checked);
                        CIO.It.Set_Y(eY.GRDL_Unit_4_Vacuum, mCkb.Checked);
                    }

                    CIO.It.Set_Y(eY.GRDL_TbFlow, mCkb.Checked);

                    break;

                case 2:    // Eject
                    if (mCkb.Checked)
                    {
#if true //201208 jhc : Manual 동작 상호 배제 (Vacuum/Eject vs. Table Water)
                        CData.L_GRD.ActWater(false);  //Table Water OFF
                        CData.L_GRD.ActVacuum(false); //Table Vacuum OFF
#else
                        if (CIO.It.Get_Y(eY.GRDL_TbVacuum))
                        { CIO.It.Set_Y(eY.GRDL_TbVacuum, false); }
#endif
                    }
                    if (CDataOption.Package == ePkg.Unit)
                    {
                        CIO.It.Set_Y(eY.GRDL_CarrierVacuum, mCkb.Checked);
                        CIO.It.Set_Y(eY.GRDL_Unit_1_Vacuum, mCkb.Checked);
                        CIO.It.Set_Y(eY.GRDL_Unit_2_Vacuum, mCkb.Checked);
                        CIO.It.Set_Y(eY.GRDL_Unit_3_Vacuum, mCkb.Checked);
                        CIO.It.Set_Y(eY.GRDL_Unit_4_Vacuum, mCkb.Checked);
                    }

                    CIO.It.Set_Y(eY.GRDL_TbEjector, mCkb.Checked);

                    break;

                case 3:    // Top Cleaner Down
                    CIO.It.Set_Y(eY.GRDL_TopClnDn, mCkb.Checked);
                    break;

                case 4:    // Top Cleaner Water Knife
                    CIO.It.Set_Y(eY.GRDL_TopWaterKnife, mCkb.Checked);
                    break;

                case 5:    // Top Cleaner Air Knife
                    CIO.It.Set_Y(eY.GRDL_TopClnAir, mCkb.Checked);
                    break;

                case 6:    // Top Cleaner Water
                    CIO.It.Set_Y(eY.GRDL_TopClnFlow, mCkb.Checked);
                    break;

                case 7:    // IONAZER
                    CIO.It.Set_Y(eY.IOZT_Power, mCkb.Checked);
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
