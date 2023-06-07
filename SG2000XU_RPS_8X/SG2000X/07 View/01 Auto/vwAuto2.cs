using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SG2000X
{
    public partial class vwAuto2 : UserControl
    {
        private Timer m_tmUp;
        //20190618 ghk_dfserver
        public bool m_bLotEnd = false;
        public bool m_bReady = false;

        //201103 jhc : Orientation Check 1회 Skip 기능 추가 (ASE-Kr VOC)
        private Timer m_tmFlicker;
        //

        public vwAuto2()
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

            m_tmUp = new Timer();
            m_tmUp.Interval = 50;
            m_tmUp.Tick += _M_tmUp_Tick;

            //201103 jhc : Orientation Check 1회 Skip 기능 추가 (ASE-Kr VOC)
            if (GV.ORIENTATION_ONE_TIME_SKIP)
            {
                m_tmFlicker = new Timer();
                m_tmFlicker.Interval = 500;
                m_tmFlicker.Tick += M_tmFlicker_Tick;
                m_tmFlicker.Start();
            }
            //
        }

        private void _M_tmUp_Tick(object sender, EventArgs e)
        {
            lbl_LotName.Text = CData.LotInfo  .sLotName;
            lbl_RunMod .Text = CData.Dev.bDual.ToString();

            if (CData.IsMultiLOT())
            {
                lbl_MgzCnt.Text = CData.LotMgr.LoadMgzSN.ToString();            // Loading된 총 Magazine 수량
                lbl_StpCnt.Text = CData.LotInfo.iTInCnt.ToString();             // 투입된 총 Strip 수량
            }
            else
            {
                lbl_MgzCnt.Text = CData.LotInfo.iTotalMgz.ToString();
                lbl_StpCnt.Text = CData.LotInfo.iTotalStrip.ToString();
            }


            // 2021.04.26 lhs Start     
            if (CData.LotInfo.bLotEnd && CData.CurCompany == ECompany.JCET)
            {
                CData.bEraseLotName = true;
            }
            if (CData.bEraseLotName)
            {
                lbl_LotName.Text = "";
            }
            // 2021.04.26 lhs End

            // 200116-maeng
            // Lot open 상태에서 Elapse 표시
            if (CData.LotInfo.bLotOpen)
            {
                // 2021-11-17, jhLee, Multi-LOT인 경우 처리중인 LOT이 존재하는 경우에만 시간 증가
                if (CData.IsMultiLOT())
                {
                    // LOT 정보가 있는 경우에만 시간 표시
                    if (CData.LotMgr.GetCount() > 0)
                    {
                        lbl_LotTime.Text = (DateTime.Now - CData.LotInfo.dtOpen).ToString(@"hh\:mm\:ss");
                    }
                }
                else
                   lbl_LotTime.Text = (DateTime.Now - CData.LotInfo.dtOpen).ToString(@"hh\:mm\:ss");
            }

            if(CData.bLotEndShow)
            {
                // 2021-03-05, jhLee : Skyworks, Lot End 되었으나 Off-Loader의 Magazine들이 아직 Place를 하지 않았다면 회색으로 버튼 표시
                if (CData.CurCompany == ECompany.SkyWorks)
                {
                    if ( CheckOffLoaderMagazine() )         // 아직 Magazine이 감지된다면
                    {
                        // 버튼을 회색으로 표시하여 아직 새로운 LOT Open을 하지 못한다고 표시해준다.
                        // 이후 Magazine이 감지 되지 않을 때까지 반복해서 체크한다.
                        if (btn_Lot.BackColor != Color.Gray)
                        {
                            btn_Lot.Text = "LOT\r\nOPEN";
                            btn_Lot.BackColor = Color.Gray;
                            btn_Lot.FlatAppearance.MouseDownBackColor = Color.DarkGray;
                            btn_Lot.FlatAppearance.MouseOverBackColor = Color.DarkGray;
                        }
                    }
                    else   
                    {
                        // 더이상 Off Loader에서 Magazine이 감지되지 않는다면 Place 완료, Lot의 완전한 종료
                        CData.bLotEndShow = false;
                        btn_Lot.Text = "LOT\r\nOPEN";
                        btn_Lot.BackColor = Color.DarkOrange;
                        btn_Lot.FlatAppearance.MouseDownBackColor = Color.OrangeRed;
                        btn_Lot.FlatAppearance.MouseOverBackColor = Color.Orange;
                    }

                }
                else
                {
                    // Skywork外 Site
                    CData.bLotEndShow = false;
                    btn_Lot.Text = "LOT\r\nOPEN";
                    btn_Lot.BackColor = Color.DarkOrange;
                    btn_Lot.FlatAppearance.MouseDownBackColor = Color.OrangeRed;
                    btn_Lot.FlatAppearance.MouseOverBackColor = Color.Orange;
                    //190407 ksg :
                    //if(CData.LotInfo.iTInCnt == CData.LotInfo.iTOutCnt) CMsg.Show(eMsg.Notice, "Notice", "Lot End");
                    //else                                                CMsg.Show(eMsg.Error , "Error" , "Lot End Now, But Different In/Out Strip Count. So Check Strip Count");
                }                
            }
            //191030 ksg :
            if(CData.bSecLotOpen)
            {
                CData.bSecLotOpen = false;
                btn_Lot.Text = "LOT\r\nEND";
                btn_Lot.BackColor = Color.FromArgb(210, 56, 80);
                btn_Lot.FlatAppearance.MouseDownBackColor = Color.DarkRed;
                btn_Lot.FlatAppearance.MouseOverBackColor = Color.IndianRed;

            }

            if(CSQ_Main.It.m_bPause)
            {
                if (CData.CurCompany == ECompany.SkyWorks)  {   btn_PauseOn.BackColor = Color.Red;          }
                else                                        {   btn_PauseOn.BackColor = Color.Lime;         }
            }
            else
            {
                if (CData.CurCompany == ECompany.SkyWorks)  {   btn_PauseOn.BackColor = Color.DarkRed;      }
                else                                        {   btn_PauseOn.BackColor = Color.DarkGreen;    }
            }

            // 2020-11-17, jhLee,  Auto loading stop
            if (CSQ_Main.It.m_bAutoLoadingStop)                 // 설정된 상태
            {
                if (CData.CurCompany == ECompany.SkyWorks)  {   btn_AutoLDStop.BackColor = Color.Red;       }
                else                                        {   btn_AutoLDStop.BackColor = Color.Lime;      }
            }
            else
            {
                if (CData.CurCompany == ECompany.SkyWorks)  {   btn_AutoLDStop.BackColor = Color.DarkRed;   }
                else                                        {   btn_AutoLDStop.BackColor = Color.DarkGreen; }
            }
            // end of jhLee

            //if (CSQ_Main.It.m_iStat == eStatus.Auto_Running)
            if (CSQ_Main.It.m_iStat != EStatus.Stop)
            {
                #region Left Grinding Data
                int iWy = (int)EWay.L;
                //200414 ksg : 12 Step  기능 추가
                for (int i = 0; i < GV.StepMaxCnt_3 - 1; i++)
                {
                    ((Label)(Controls.Find("lblL_R" + (i + 1) + "Tar", true)[0])).Text = CData.GrData[iWy].aTar[i].ToString();
                    ((Label)(Controls.Find("lblL_R" + (i + 1) + "Cnt", true)[0])).Text = string.Format("{0} / {1}", CData.GrData[iWy].aInx[i].ToString("00"), CData.GrData[iWy].aCnt[i].ToString("00"));
                    ((Label)(Controls.Find("lblL_R" + (i + 1) + "Num", true)[0])).Text = CData.GrData[iWy].aReNum[i].ToString();    // 2020.10.10 JSKim
                    ((Label)(Controls.Find("lblL_R" + (i + 1) + "Re", true)[0])).Text = string.Format("{0} / {1}", CData.GrData[iWy].aReInx[i].ToString("00"), CData.GrData[iWy].aReCnt[i].ToString("00"));
                    ((Label)(Controls.Find("lblL_R" + (i + 1) + "Pt", true)[0])).Text = CData.GrData[iWy].a1Pt[i].ToString("0.000");
                }
                lblL_FiTar.Text = CData.GrData[iWy].aTar[GV.StepMaxCnt_3 - 1].ToString();
                lblL_FiCnt.Text = string.Format("{0} / {1}", CData.GrData[iWy].aInx[GV.StepMaxCnt_3 - 1].ToString("00"), CData.GrData[iWy].aCnt[GV.StepMaxCnt_3 - 1].ToString("00"));
                lblL_FiNum.Text = CData.GrData[iWy].aReNum[GV.StepMaxCnt_3 - 1].ToString();
                lblL_FiRe.Text = string.Format("{0} / {1}", CData.GrData[iWy].aReInx[GV.StepMaxCnt_3 - 1].ToString("00"), CData.GrData[iWy].aReCnt[GV.StepMaxCnt_3 - 1].ToString("00"));
                lblL_FiPt.Text = CData.GrData[iWy].a1Pt[GV.StepMaxCnt_3 - 1].ToString();

                // Barcode
                lbl_BcrL.Text = CData.Parts[(int)EPart.GRDL].sBcr;

                // Grind Time
                //190423 ksg :
                //lbl_GrdTime_L.Text = CData.GrdTime[(int)EWay.L].ToString();
                if(CData.L_GRD.m_CntTimeFlag)
                {
                    lbl_GrdTime_L.Text = (DateTime.Now - CData.GrdElp[(int)EWay.L].dtStr).ToString(@"hh\:mm\:ss");
                }
                
                #endregion

                #region Right Grinding Data
                iWy = (int)EWay.R;
                //200414 ksg : 12 Step  기능 추가
                for (int i = 0; i < GV.StepMaxCnt_3 - 1; i++)
                {
                    ((Label)(Controls.Find("lblR_R" + (i + 1) + "Tar", true)[0])).Text = CData.GrData[iWy].aTar[i].ToString();
                    ((Label)(Controls.Find("lblR_R" + (i + 1) + "Cnt", true)[0])).Text = string.Format("{0} / {1}", CData.GrData[iWy].aInx[i].ToString("00"), CData.GrData[iWy].aCnt[i].ToString("00"));
                    ((Label)(Controls.Find("lblR_R" + (i + 1) + "Num", true)[0])).Text = CData.GrData[iWy].aReNum[i].ToString();
                    ((Label)(Controls.Find("lblR_R" + (i + 1) + "Re", true)[0])).Text = string.Format("{0} / {1}", CData.GrData[iWy].aReInx[i].ToString("00"), CData.GrData[iWy].aReCnt[i].ToString("00"));
                    ((Label)(Controls.Find("lblR_R" + (i + 1) + "Pt", true)[0])).Text = CData.GrData[iWy].a1Pt[i].ToString("0.000");
                }

                lblR_FiTar.Text = CData.GrData[iWy].aTar[GV.StepMaxCnt_3 - 1].ToString();
                lblR_FiCnt.Text = string.Format("{0} / {1}", CData.GrData[iWy].aInx[GV.StepMaxCnt_3 - 1].ToString("00"), CData.GrData[iWy].aCnt[GV.StepMaxCnt_3 - 1].ToString("00"));
                lblR_FiNum.Text = CData.GrData[iWy].aReNum[GV.StepMaxCnt_3 - 1].ToString();
                lblR_FiRe.Text = string.Format("{0} / {1}", CData.GrData[iWy].aReInx[GV.StepMaxCnt_3 - 1].ToString("00"), CData.GrData[iWy].aReCnt[GV.StepMaxCnt_3 - 1].ToString("00"));
                lblR_FiPt.Text = CData.GrData[iWy].a1Pt[GV.StepMaxCnt_3 - 1].ToString();

                // Barcode
                lbl_BcrR.Text = CData.Parts[(int)EPart.GRDR].sBcr;

                // Grind Time
                //190423 ksg :
                //lbl_GrdTime_R.Text = CData.GrdTime[(int)EWay.R].ToString();
                if(CData.R_GRD.m_CntTimeFlag)
                {
                    lbl_GrdTime_R.Text = (DateTime.Now - CData.GrdElp[(int)EWay.R].dtStr).ToString(@"hh\:mm\:ss");
                }
                #endregion
            }

            // 2020.08.21 JSKim St
            string strformat = "0.";
            int nfontsize = CData.Opt.iProbeFontSize;

            if (CData.Opt.iProbeFloationPoint == 0)
            {
                strformat = "0.000";
            }
            else
            {
                for (int i = 0; i < CData.Opt.iProbeFloationPoint; i++)
                {
                    strformat += "0";
                }
            }

            if (nfontsize == 0) nfontsize = 10;
            // 2020.08.21 JSKim Ed

            if (CData.Dev.iCol > 0 && CData.Dev.iRow > 0)
            {
                int iCol = CData.Dev.iCol;
                int iRow = CData.Dev.iRow * CData.Dev.iWinCnt;

                if (CDataOption.Package == ePkg.Strip)
                {
                    if (CData.GrData[0].aMeaBf != null && CData.GrData[0].aMeaBf.GetLength(0) == iRow)
                    {

                        if (CData.GrData[0].aMeaBf.GetLength(0) == iRow && CData.L_GRD.m_abMeaBfErr.GetLength(0) == iRow &&
                            CData.GrData[0].aMeaBf.GetLength(1) == iCol && CData.L_GRD.m_abMeaBfErr.GetLength(1) == iCol)
                        {
                            for (int i = 0; i < iCol; i++)
                            {
                                for (int j = 0; j < iRow; j++)
                                {
                                    if (CData.GrData[0].aMeaBf != null)
                                    {
                                        if (CData.L_GRD.m_abMeaBfErr[j, i])
                                        {
                                            dgvL_Bf.Rows[j].Cells[i].Style.ForeColor = Color.Red;
                                        }
                                        else
                                        {
                                            dgvL_Bf.Rows[j].Cells[i].Style.ForeColor = Color.Black;
                                        }
                                        // 2020.08.21 JSKim St
                                        dgvL_Bf.Rows[j].Cells[i].Style.Font = new Font("Arial Unicode MS", nfontsize);
                                        if (CData.GrData[0].aMeaBf[j, i] == 0)
                                        {
                                            dgvL_Bf.Rows[j].Cells[i].Style.Format = "0";
                                        }
                                        else
                                        {
                                            dgvL_Bf.Rows[j].Cells[i].Style.Format = strformat;
                                        }
                                        // 2020.08.21 JSKim Ed
                                        dgvL_Bf.Rows[j].Cells[i].Value = CData.GrData[0].aMeaBf[j, i];
                                    }
                                    if (CData.GrData[0].aMeaAf != null)
                                    {
                                        if (CData.L_GRD.m_abMeaAfErr[j, i])
                                        {
                                            dgvL_Af.Rows[j].Cells[i].Style.ForeColor = Color.Red;
                                        }
                                        else
                                        {
                                            dgvL_Af.Rows[j].Cells[i].Style.ForeColor = Color.Black;
                                        }
                                        // 2020.08.21 JSKim St
                                        dgvL_Af.Rows[j].Cells[i].Style.Font = new Font("Arial Unicode MS", nfontsize);
                                        if (CData.GrData[0].aMeaAf[j, i] == 0)
                                        {
                                            dgvL_Af.Rows[j].Cells[i].Style.Format = "0";
                                        }
                                        else
                                        {
                                            dgvL_Af.Rows[j].Cells[i].Style.Format = strformat;
                                        }
                                        // 2020.08.21 JSKim Ed
                                        dgvL_Af.Rows[j].Cells[i].Value = CData.GrData[0].aMeaAf[j, i];
                                    }

                                    if (CData.GrData[1].aMeaBf != null)
                                    {
                                        if (CData.R_GRD.m_abMeaBfErr[j, i])
                                        {
                                            dgvR_Bf.Rows[j].Cells[i].Style.ForeColor = Color.Red;
                                        }
                                        else
                                        {
                                            dgvR_Bf.Rows[j].Cells[i].Style.ForeColor = Color.Black;
                                        }
                                        // 2020.08.21 JSKim St
                                        dgvR_Bf.Rows[j].Cells[i].Style.Font = new Font("Arial Unicode MS", nfontsize);
                                        if (CData.GrData[1].aMeaBf[j, i] == 0)
                                        {
                                            dgvR_Bf.Rows[j].Cells[i].Style.Format = "0";
                                        }
                                        else
                                        {
                                            dgvR_Bf.Rows[j].Cells[i].Style.Format = strformat;
                                        }
                                        // 2020.08.21 JSKim Ed
                                        dgvR_Bf.Rows[j].Cells[i].Value = CData.GrData[1].aMeaBf[j, i];
                                    }
                                    if (CData.GrData[1].aMeaAf != null)
                                    {
                                        if (CData.R_GRD.m_abMeaAfErr[j, i])
                                        {
                                            dgvR_Af.Rows[j].Cells[i].Style.ForeColor = Color.Red;
                                        }
                                        else
                                        {
                                            dgvR_Af.Rows[j].Cells[i].Style.ForeColor = Color.Black;
                                        }
                                        // 2020.08.21 JSKim St
                                        dgvR_Af.Rows[j].Cells[i].Style.Font = new Font("Arial Unicode MS", nfontsize);
                                        if (CData.GrData[1].aMeaAf[j, i] == 0)
                                        {
                                            dgvR_Af.Rows[j].Cells[i].Style.Format = "0";
                                        }
                                        else
                                        {
                                            dgvR_Af.Rows[j].Cells[i].Style.Format = strformat;
                                        }
                                        // 2020.08.21 JSKim Ed
                                        dgvR_Af.Rows[j].Cells[i].Value = CData.GrData[1].aMeaAf[j, i];
                                    }
                                }
                            }
                        }
                    }
                }
                else                
                {
                    iRow = CData.Dev.iRow;

                    if (CData.GrData[0].aUnit[0].aMeaBf.GetLength(0) == iRow && CData.GrData[0].aUnit[0].aErrBf.GetLength(0) == iRow &&
                        CData.GrData[0].aUnit[0].aMeaBf.GetLength(1) == iCol && CData.GrData[0].aUnit[0].aErrBf.GetLength(1) == iCol)
                    {
                        for (int i = 0; i < iCol; i++)
                        {
                            for (int iU = CData.Dev.iUnitCnt - 1; iU >= 0; iU--)
                            {
                                for (int j = 0; j < iRow; j++)
                                {
                                    // 현재 Row 인덱스 계산
                                    int iR = j + ((CData.Dev.iUnitCnt - (iU + 1)) * iRow);

                                    // Left before
                                    if (CData.GrData[0].aUnit[iU].aMeaBf != null)
                                    {
                                        if (CData.GrData[0].aUnit[iU].aErrBf[j, i])
                                        {
                                            dgvL_Bf.Rows[iR].Cells[i].Style.ForeColor = Color.Red;
                                        }
                                        else
                                        {
                                            dgvL_Bf.Rows[iR].Cells[i].Style.ForeColor = Color.Black;
                                        }
                                        // 2020.08.21 JSKim St
                                        dgvL_Bf.Rows[iR].Cells[i].Style.Font = new Font("Arial Unicode MS", nfontsize);
                                        if (CData.GrData[0].aUnit[iU].aMeaBf[j, i] == 0)
                                        {
                                            dgvL_Bf.Rows[iR].Cells[i].Style.Format = "0";
                                        }
                                        else
                                        {
                                            dgvL_Bf.Rows[iR].Cells[i].Style.Format = strformat;
                                        }
                                        // 2020.08.21 JSKim Ed
                                        dgvL_Bf.Rows[iR].Cells[i].Value = CData.GrData[0].aUnit[iU].aMeaBf[j, i];
                                    }
                                    // Left after
                                    if (CData.GrData[0].aUnit[iU].aMeaAf != null)
                                    {
                                        if (CData.GrData[0].aUnit[iU].aErrAf[j, i])
                                        {
                                            dgvL_Af.Rows[iR].Cells[i].Style.ForeColor = Color.Red;
                                        }
                                        else
                                        {
                                            dgvL_Af.Rows[iR].Cells[i].Style.ForeColor = Color.Black;
                                        }
                                        // 2020.08.21 JSKim St
                                        dgvL_Af.Rows[iR].Cells[i].Style.Font = new Font("Arial Unicode MS", nfontsize);
                                        if (CData.GrData[0].aUnit[iU].aMeaAf[j, i] == 0)
                                        {
                                            dgvL_Af.Rows[iR].Cells[i].Style.Format = "0";
                                        }
                                        else
                                        {
                                            dgvL_Af.Rows[iR].Cells[i].Style.Format = strformat;
                                        }
                                        // 2020.08.21 JSKim Ed
                                        dgvL_Af.Rows[iR].Cells[i].Value = CData.GrData[0].aUnit[iU].aMeaAf[j, i];
                                    }
                                    // Right before
                                    if (CData.GrData[1].aUnit[iU].aMeaBf != null)
                                    {
                                        if (CData.GrData[1].aUnit[iU].aErrBf[j, i])
                                        {
                                            dgvR_Bf.Rows[iR].Cells[i].Style.ForeColor = Color.Red;
                                        }
                                        else
                                        {
                                            dgvR_Bf.Rows[iR].Cells[i].Style.ForeColor = Color.Black;
                                        }
                                        // 2020.08.21 JSKim St
                                        dgvR_Bf.Rows[iR].Cells[i].Style.Font = new Font("Arial Unicode MS", nfontsize);
                                        if (CData.GrData[1].aUnit[iU].aMeaBf[j, i] == 0)
                                        {
                                            dgvR_Bf.Rows[iR].Cells[i].Style.Format = "0";
                                        }
                                        else
                                        {
                                            dgvR_Bf.Rows[iR].Cells[i].Style.Format = strformat;
                                        }
                                        // 2020.08.21 JSKim Ed
                                        dgvR_Bf.Rows[iR].Cells[i].Value = CData.GrData[1].aUnit[iU].aMeaBf[j, i];
                                    }
                                    // Right after
                                    if (CData.GrData[1].aUnit[iU].aMeaAf != null)
                                    {
                                        if (CData.GrData[1].aUnit[iU].aErrAf[j, i])
                                        {
                                            dgvR_Af.Rows[iR].Cells[i].Style.ForeColor = Color.Red;
                                        }
                                        else
                                        {
                                            dgvR_Af.Rows[iR].Cells[i].Style.ForeColor = Color.Black;
                                        }
                                        // 2020.08.21 JSKim St
                                        dgvR_Af.Rows[iR].Cells[i].Style.Font = new Font("Arial Unicode MS", nfontsize);
                                        if (CData.GrData[1].aUnit[iU].aMeaAf[j, i] == 0)
                                        {
                                            dgvR_Af.Rows[iR].Cells[i].Style.Format = "0";
                                        }
                                        else
                                        {
                                            dgvR_Af.Rows[iR].Cells[i].Style.Format = strformat;
                                        }
                                        // 2020.08.21 JSKim Ed
                                        dgvR_Af.Rows[iR].Cells[i].Value = CData.GrData[1].aUnit[iU].aMeaAf[j, i];
                                    }
                                }
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < GV.StepMaxCnt_3 - 1; i++)
            {
                if(!CData.Dev.aData[(int)EWay.L].aSteps[i].bUse) ((Panel)(this.Controls.Find("pnlL_R"+(i+1), true)[0])).Visible = false;
                else                                             ((Panel)(this.Controls.Find("pnlL_R"+(i+1), true)[0])).Visible = true ;

                if(CData.Dev.bDual == eDual.Dual)
                {
                    if(!CData.Dev.aData[(int)EWay.R].aSteps[i].bUse) ((Panel)(this.Controls.Find("pnlR_R"+(i+1), true)[0])).Visible = false;
                    else                                             ((Panel)(this.Controls.Find("pnlR_R"+(i+1), true)[0])).Visible = true ;
                }
                else
                {
                    if(!CData.Dev.aData[(int)EWay.L].aSteps[i].bUse) ((Panel)(this.Controls.Find("pnlR_R"+(i+1), true)[0])).Visible = false;
                    else                                             ((Panel)(this.Controls.Find("pnlR_R"+(i+1), true)[0])).Visible = true ;
                }
            }
            //200514 ksg : 화면 표시 수정
            if (!CData.Dev.aData[(int)EWay.L].aSteps[GV.StepMaxCnt_3 - 1].bUse) pnlL_Fi.Visible = false;
            else pnlL_Fi.Visible = true;
            if (CData.Dev.bDual == eDual.Dual)
            {
                if (!CData.Dev.aData[(int)EWay.R].aSteps[GV.StepMaxCnt_3 - 1].bUse) pnlR_Fi.Visible = false;
                else pnlR_Fi.Visible = true;
            }
            else
            {
                if (!CData.Dev.aData[(int)EWay.L].aSteps[GV.StepMaxCnt_3 - 1].bUse) pnlR_Fi.Visible = false;
                else pnlR_Fi.Visible = true;
            }

            //20190218 ghk_dynamicfunction
            lbl_Pcb1.Text = CData.Dynamic.dPcb[0].ToString();
            lbl_Pcb2.Text = CData.Dynamic.dPcb[1].ToString();
            lbl_Pcb3.Text = CData.Dynamic.dPcb[2].ToString();
            //20200328 jhc : 다이나믹 펑션 측정 포인트 최대 5개로 확대
            lbl_Pcb4.Text = CData.Dynamic.dPcb[3].ToString();
            lbl_Pcb5.Text = CData.Dynamic.dPcb[4].ToString();
            //

#if true //20200331 jhc : DF Max/Mean/TTV 표시 버그 수정
            double[] dTempPcb = new double[CData.Dev.iDynamicPosNum];
            for (int i = 0; i < CData.Dev.iDynamicPosNum; i++) dTempPcb[i] = CData.Dynamic.dPcb[i];
            double dTmpPcbMax = dTempPcb.Max();
            double dTmpPcbMean = Math.Round(dTempPcb.Average(), 4);
            double dTmpPcbTtv = Math.Round((dTempPcb.Max() - dTempPcb.Min()), 4);
            lbl_PcbMax.Text = dTmpPcbMax.ToString();
            lbl_PcbMean.Text = dTmpPcbMean.ToString();
            lbl_PcbTtv.Text = dTmpPcbTtv.ToString();
#else
            CData.Dynamic.dPcbMax = CData.Dynamic.dPcb.Max();
            lbl_PcbMax.Text = CData.Dynamic.dPcbMax.ToString();

            CData.Dynamic.dPcbMean = Math.Round(CData.Dynamic.dPcb.Average(), 4);
            lbl_PcbMean.Text = CData.Dynamic.dPcbMean.ToString();

            CData.Dynamic.dPcbTtv = Math.Round((CData.Dynamic.dPcb.Max() - CData.Dynamic.dPcb.Min()), 4);
            lbl_PcbTtv.Text = CData.Dynamic.dPcbTtv.ToString();
#endif
            //
            //20190625 ghk_dfserver
            if (CDataOption.CurEqu == eEquType.Nomal)
            {//바코드 카메라 인레일에 있을 경우
                // 201029 jym : 스카이웍스 제외 조건 추가
                if (CData.CurCompany != ECompany.SkyWorks)
                {
                    tp_Lot.SelectedIndex = (CSq_Inr.It.m_bBcrView) ? 1 : 0;
                }
            }
            else
            {//바코드 카메라 온로더 피커에 있을 경우
                if(CSq_OnP.It.m_bBcrView)
                {
                    // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                    //if(CData.CurCompany == ECompany.Qorvo || (CData.CurCompany == ECompany.Qorvo_DZ) || CData.CurCompany == ECompany.SST || CData.CurCompany == ECompany.USI) //191202 ksg :
                    if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                        CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC ||
                        CData.CurCompany == ECompany.SST || CData.CurCompany == ECompany.USI)
                    {
                        btn_BcrErr .Visible = false;
                        label27    .Visible = false;
                        txt_BcrPass.Visible = false;
                    }
                    //else if(CData.CurCompany == eCompany.JSCK)
                    else if(CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK || CData.CurCompany == ECompany.JCET) //200121 ksg :, 200625 lks
                    {
                        label27.Text = "BCR2";
                    }
                    tp_Lot.SelectedIndex = 1;
                }
                else
                {
                    tp_Lot.SelectedIndex = 0;
                }
            }
            //

            //201117 jhc : Orientation Error발생 시에만 버튼 표시, 검사 완료 후 다시 감춤
            _Set_OriSkipBtn_ResetBtn();
            //
        }

        public void Open()
        {
            _Set();
            _Draw();

            
            //20191205 ghk_autoview
            if(CData.CurCompany == ECompany.SkyWorks)
            {
                btn_PauseOn.BackColor = Color.Red;
                btn_PauseOn.FlatAppearance.MouseOverBackColor = Color.Red;
                btn_PauseOn.FlatAppearance.MouseDownBackColor = Color.DarkRed;

                btn_PauseOff.BackColor = Color.DarkGreen;
                btn_PauseOff.FlatAppearance.MouseOverBackColor = Color.Lime;
                btn_PauseOff.FlatAppearance.MouseDownBackColor = Color.DarkGreen;
            }

            //syc : ase kr loading stop 
            //if (CDataOption.UseAutoLoadingStop && CData.CurCompany != ECompany.SkyWorks)//210309 pjh : 조건 추가
            if (CDataOption.UseAutoLoadingStop)
            {
                btn_AutoLDStop.Visible = true;   
            }
            else
            {
                // 2020-11-18, jhLee : Skyworks 전용으로 UI를 적용한다. AutoLoadingStop 기능
                //                     Skyworks가 아닌경우 전용 기능을 감춘다.
                btn_AutoLDStop.Visible = false;         // AutoLoadingStop 버튼을 감춘다.

                // 기존의 Loading Stop On / oFF의 버튼은 위치와 크기를 변경한다.
                btn_PauseOn.Location = new Point(870, 515);
                btn_PauseOn.Size = new Size(142, 66);
                btn_PauseOff.Location = new Point(1028, 515);
                btn_PauseOff.Size = new Size(142, 66);
            }

            if (m_tmUp != null)
            {
                if (!m_tmUp.Enabled)
                { m_tmUp.Start(); }
            }

            //201103 jhc : Orientation Check 1회 Skip 기능 추가 (ASE-Kr VOC)
            _Set_OriSkipBtn_ResetBtn();
            //
        }

        public void Close()
        {
            if (m_tmUp != null)
            {
                if (m_tmUp.Enabled)
                { m_tmUp.Stop(); }
            }
        }

        public void Release()
        {
            //201103 jhc : Orientation Check 1회 Skip 기능 추가 (ASE-Kr VOC)
            if ((GV.ORIENTATION_ONE_TIME_SKIP) && (m_tmFlicker != null))
            {
                if (m_tmFlicker.Enabled)
                { m_tmFlicker.Stop(); }
                m_tmFlicker.Dispose();
                m_tmFlicker = null;
            }

            if (m_tmUp != null)
            {
                if (m_tmUp.Enabled)
                { m_tmUp.Stop(); }
                m_tmUp.Dispose();
                m_tmUp = null;
            }
        }

        public void _Set()
        {
            // Barcode Control Visible
            lbl_BcrL.Visible = !CData.Dev.bBcrSkip;
            lbl_BcrL_T.Visible = !CData.Dev.bBcrSkip;
            lbl_BcrR.Visible = !CData.Dev.bBcrSkip;
            lbl_BcrR_T.Visible = !CData.Dev.bBcrSkip;

            pl_Dynamic.Visible = !CData.Dev.bDynamicSkip;

            #region Grind Left Parameter
            int iWy = (int)EWay.L;
            string text;
            if (CData.Dev.aData[iWy].eGrdMod == eGrdMode.Target)
            {
                SetWriteLanguage(lblL_Mod.Name, CLang.It.GetLanguage("Target"));
                // 2021.11.26 lhs Start
                if (CDataOption.UseNewSckGrindProc)
                {
                    if (CData.Dev.aData[iWy].eBaseOnThick == EBaseOnThick.Mold)     { lblL_Mod.Text += " / Mold";   }
                    if (CData.Dev.aData[iWy].eBaseOnThick == EBaseOnThick.Total)    { lblL_Mod.Text += " / Total";  }
                }
                // 2021.11.26 lhs End

                text = CLang.It.GetLanguage("Target") + "\r\n" + CLang.It.GetLanguage("Thickness");
                SetWriteLanguage(lblL_Tot_T.Name, text);
            }
            else
            {
                SetWriteLanguage(lblL_Mod.Name, CLang.It.GetLanguage("TopDown"));
                text = CLang.It.GetLanguage("Total") + "\r\n" + CLang.It.GetLanguage("Thickness");
                SetWriteLanguage(lblL_Tot_T.Name, text);
            }
            //200414 ksg : 12 Step  기능 추가
            for (int i = 0; i < GV.StepMaxCnt_3 - 1; i++)
            {
                ((Panel)(Controls.Find("pnlL_R" + (i + 1), true)[0])).Enabled = CData.Dev.aData[iWy].aSteps[i].bUse; ;
                ((Label)(Controls.Find("lblL_R" + (i + 1) + "Tot", true)[0])).Text = CData.Dev.aData[iWy].aSteps[i].dTotalDep.ToString();
                ((Label)(Controls.Find("lblL_R" + (i + 1) + "Cyl", true)[0])).Text = CData.Dev.aData[iWy].aSteps[i].dCycleDep.ToString();
                ((Label)(Controls.Find("lblL_R" + (i + 1) + "Tbl", true)[0])).Text = CData.Dev.aData[iWy].aSteps[i].dTblSpd.ToString();
                ((Label)(Controls.Find("lblL_R" + (i + 1) + "Spl", true)[0])).Text = CData.Dev.aData[iWy].aSteps[i].iSplSpd.ToString();
            }
            pnlL_Fi.Enabled = CData.Dev.aData[iWy].aSteps[GV.StepMaxCnt_3 - 1].bUse;
            lblL_FiTot.Text = CData.Dev.aData[iWy].aSteps[GV.StepMaxCnt_3 - 1].dTotalDep.ToString();
            lblL_FiCyl.Text = CData.Dev.aData[iWy].aSteps[GV.StepMaxCnt_3 - 1].dCycleDep.ToString();
            lblL_FiTbl.Text = CData.Dev.aData[iWy].aSteps[GV.StepMaxCnt_3 - 1].dTblSpd.ToString();
            lblL_FiSpl.Text = CData.Dev.aData[iWy].aSteps[GV.StepMaxCnt_3 - 1].iSplSpd.ToString();
            #endregion

            #region Grind Right Parameter
            iWy = (int)EWay.R;
            if (CData.Dev.aData[iWy].eGrdMod == eGrdMode.Target)
            {
                SetWriteLanguage(lblR_Mod.Name, CLang.It.GetLanguage("Target"));
                // 2021.11.26 lhs Start
                if (CDataOption.UseNewSckGrindProc)
                {
                    if (CData.Dev.aData[iWy].eBaseOnThick == EBaseOnThick.Mold)     { lblR_Mod.Text += " / Mold";   }
                    if (CData.Dev.aData[iWy].eBaseOnThick == EBaseOnThick.Total)    { lblR_Mod.Text += " / Total";  }
                }
                // 2021.11.26 lhs End

                text = CLang.It.GetLanguage("Target") + "\r\n" + CLang.It.GetLanguage("Thickness");
                SetWriteLanguage(lblR_Tot_T.Name, text);
            }
            else
            {
                SetWriteLanguage(lblR_Mod.Name, CLang.It.GetLanguage("TopDown"));
                text = CLang.It.GetLanguage("Total") + "\r\n" + CLang.It.GetLanguage("Thickness");
                SetWriteLanguage(lblR_Tot_T.Name, text);
            }
            //200414 ksg : 12 Step  기능 추가
            for (int i = 0; i < GV.StepMaxCnt_3 - 1; i++)
            {
                ((Panel)(Controls.Find("pnlR_R" + (i + 1), true)[0])).Enabled = CData.Dev.aData[iWy].aSteps[i].bUse; ;
                ((Label)(Controls.Find("lblR_R" + (i + 1) + "Tot", true)[0])).Text = CData.Dev.aData[iWy].aSteps[i].dTotalDep.ToString();
                ((Label)(Controls.Find("lblR_R" + (i + 1) + "Cyl", true)[0])).Text = CData.Dev.aData[iWy].aSteps[i].dCycleDep.ToString();
                ((Label)(Controls.Find("lblR_R" + (i + 1) + "Tbl", true)[0])).Text = CData.Dev.aData[iWy].aSteps[i].dTblSpd.ToString();
                ((Label)(Controls.Find("lblR_R" + (i + 1) + "Spl", true)[0])).Text = CData.Dev.aData[iWy].aSteps[i].iSplSpd.ToString();
            }
            pnlR_Fi.Enabled = CData.Dev.aData[iWy].aSteps[GV.StepMaxCnt_3 - 1].bUse;
            lblR_FiTot.Text = CData.Dev.aData[iWy].aSteps[GV.StepMaxCnt_3 - 1].dTotalDep.ToString();
            lblR_FiCyl.Text = CData.Dev.aData[iWy].aSteps[GV.StepMaxCnt_3 - 1].dCycleDep.ToString();
            lblR_FiTbl.Text = CData.Dev.aData[iWy].aSteps[GV.StepMaxCnt_3 - 1].dTblSpd.ToString();
            lblR_FiSpl.Text = CData.Dev.aData[iWy].aSteps[GV.StepMaxCnt_3 - 1].iSplSpd.ToString();
            #endregion
        }

        public void _Get()
        {

        }

        public void _Draw()
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

            // 200317 mjy : 조건 추가
            if (CDataOption.Package == ePkg.Unit)
            {
                iCnt = CData.Dev.iUnitCnt;
            }

            dgvL_Bf.Rows.Clear();
            dgvL_Bf.Columns.Clear();
            dgvL_Af.Rows.Clear();
            dgvL_Af.Columns.Clear();

            dgvR_Bf.Rows.Clear();
            dgvR_Bf.Columns.Clear();
            dgvR_Af.Rows.Clear();
            dgvR_Af.Columns.Clear();

            for (int i = 0; i < iCol; i++)
            {
                dgvL_Bf.Columns.Add(i.ToString(), i.ToString());
                dgvL_Af.Columns.Add(i.ToString(), i.ToString());
                dgvR_Bf.Columns.Add(i.ToString(), i.ToString());
                dgvR_Af.Columns.Add(i.ToString(), i.ToString());
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
                    dgvL_Bf.Rows.Add();
                    dgvL_Bf.Rows[(k * iRow) + j].DefaultCellStyle.BackColor = tColor;
                    dgvL_Af.Rows.Add();
                    dgvL_Af.Rows[(k * iRow) + j].DefaultCellStyle.BackColor = tColor;

                    dgvR_Bf.Rows.Add();
                    dgvR_Bf.Rows[(k * iRow) + j].DefaultCellStyle.BackColor = tColor;
                    dgvR_Af.Rows.Add();
                    dgvR_Af.Rows[(k * iRow) + j].DefaultCellStyle.BackColor = tColor;
                }
            }

            for (int i = 0; i < iCol; i++)
            {
                if (CDataOption.Package == ePkg.Strip)
                {
                    for (int j = 0; j < iRow * iCnt; j++)
                    {
                        //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                        if ((CDataOption.Use18PointMeasure == true) && (0 < CData.Dev.i18PStripCount)) //Leading Strip Count 0일 경우 Leading Strip 용으로 설정된 Contact Point 표시 안 함
                        {
                            iBkColor = 0; //초기화
                            if (CData.Dev.aData[0].aPosBf[j, i].bUse)    { iBkColor |= 0x01; }
                            if (CData.Dev.aData[0].aPosBf[j, i].bUse18P) { iBkColor |= 0x02; }
                            if (0 < iBkColor && iBkColor <= 3)
                            {//Leading Strip 측정 포인트 또는 Main Strip 측정 포인트인 경우에만 색상 변경
                                dgvL_Bf.Rows[j].Cells[i].Style.BackColor = GV.CELL_BK_COLOR[iBkColor];
                            }
                            iBkColor = 0; //초기화
                            if (CData.Dev.aData[0].aPosAf[j, i].bUse)    { iBkColor |= 0x01; }
                            if (CData.Dev.aData[0].aPosAf[j, i].bUse18P) { iBkColor |= 0x02; }
                            if (0 < iBkColor && iBkColor <= 3)
                            {//Leading Strip 측정 포인트 또는 Main Strip 측정 포인트인 경우에만 색상 변경
                                dgvL_Af.Rows[j].Cells[i].Style.BackColor = GV.CELL_BK_COLOR[iBkColor];
                            }

                            iBkColor = 0; //초기화
                            if (CData.Dev.aData[1].aPosBf[j, i].bUse)    { iBkColor |= 0x01; }
                            if (CData.Dev.aData[1].aPosBf[j, i].bUse18P) { iBkColor |= 0x02; }
                            if (0 < iBkColor && iBkColor <= 3)
                            {//Leading Strip 측정 포인트 또는 Main Strip 측정 포인트인 경우에만 색상 변경
                                dgvR_Bf.Rows[j].Cells[i].Style.BackColor = GV.CELL_BK_COLOR[iBkColor];
                            }
                            iBkColor = 0; //초기화
                            if (CData.Dev.aData[1].aPosAf[j, i].bUse)    { iBkColor |= 0x01; }
                            if (CData.Dev.aData[1].aPosAf[j, i].bUse18P) { iBkColor |= 0x02; }
                            if (0 < iBkColor && iBkColor <= 3)
                            {//Leading Strip 측정 포인트 또는 Main Strip 측정 포인트인 경우에만 색상 변경
                                dgvR_Af.Rows[j].Cells[i].Style.BackColor = GV.CELL_BK_COLOR[iBkColor];
                            }
                        }
                        else
                        {
                            if (CData.Dev.aData[0].aPosBf[j, i].bUse)
                            { dgvL_Bf.Rows[j].Cells[i].Style.BackColor = Color.Lime; }
                            if (CData.Dev.aData[0].aPosAf[j, i].bUse)
                            { dgvL_Af.Rows[j].Cells[i].Style.BackColor = Color.Lime; }

                            if (CData.Dev.aData[1].aPosBf[j, i].bUse)
                            { dgvR_Bf.Rows[j].Cells[i].Style.BackColor = Color.Lime; }
                            if (CData.Dev.aData[1].aPosAf[j, i].bUse)
                            { dgvR_Af.Rows[j].Cells[i].Style.BackColor = Color.Lime; }
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

                            if (CData.Dev.aData[0].aPosBf[j, i].bUse)
                            { dgvL_Bf.Rows[iR].Cells[i].Style.BackColor = Color.Lime; }
                            if (CData.Dev.aData[0].aPosAf[j, i].bUse)
                            { dgvL_Af.Rows[iR].Cells[i].Style.BackColor = Color.Lime; }

                            if (CData.Dev.aData[1].aPosBf[j, i].bUse)
                            { dgvR_Bf.Rows[iR].Cells[i].Style.BackColor = Color.Lime; }
                            if (CData.Dev.aData[1].aPosAf[j, i].bUse)
                            { dgvR_Af.Rows[iR].Cells[i].Style.BackColor = Color.Lime; }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Auto view에 조작 로그 저장 함수
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

        private void btn_Lot_Click(object sender, EventArgs e)
        {
            _SetLog("Click");

            // 2021-06-08, jhLee : Multi-LOT 사용이라면 LOT 정보 관리 창을 보여준다.
            if (CData.IsMultiLOT())
            {
                CData.dlgLotInput.ShowForm();
                return;
            }
            // == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == ==

            if (!((CSQ_Main.It.m_iStat == EStatus.Idle) || (CSQ_Main.It.m_iStat == EStatus.Stop))) return;

            timer_Dfserver2.Enabled = false;
            m_bReady = false;
            if (btn_Lot.Text.Contains("OPEN"))
            {
                //20190618 ghk_dfserver
                //if ((CData.CurCompany == eCompany.AseKr || CData.CurCompany == eCompany.AseK26) && !CData.Dev.bDfServerSkip)
                if (CDataOption.eDfserver == eDfserver.Use && !CData.Dev.bDfServerSkip)
                {
                    CDf.It.SendReady();
                    m_bReady = true;
                    timer_Dfserver2.Enabled = true;
                }
                else
                {
                    // LOT Open
                    using (frmLot mForm = new frmLot())
                    {
                        if (mForm.ShowDialog() == DialogResult.OK)
                        {
                            //190107 ksg : Lot Open시 장비 Check
                            if (!CheckStatus())
                            {
                                CMsg.Show(eMsg.Error, "ERROR", "Magazin Or Strip Remove Please");
                                return;
                            }
                            //211202 pjh : DF Net Drive Connect Check
                            if (!CData.m_bNetCheck && CData.Opt.bDFNetUse)
                            {
                                CMsg.Show(eMsg.Error, "ERROR", "Check Network Connection Status");
                                return;
                            }
                            //

                            LotInfoClear(); // vwAuto2
                            _Set();

                            btn_Lot.Text = "LOT\r\nEND";
                            btn_Lot.BackColor = Color.FromArgb(210, 56, 80);
                            btn_Lot.FlatAppearance.MouseDownBackColor = Color.DarkRed;
                            btn_Lot.FlatAppearance.MouseOverBackColor = Color.IndianRed;
                            //추가
                            CData.IdleS = DateTime.Now; //190108 ksg : 추가 및 초기화
                            CData.JamS  = DateTime.Now; //ksg : 초기화
                            CData.RunS  = DateTime.Now; //ksg : 추가 및 초기화
                            /*
                            frmView2.swError.Reset(); //190110 ksg : 수정
                            frmView2.swIdle.Reset(); //190110 ksg : 수정
                            frmView2.swRun.Reset(); //190110 ksg : 수정
                            */
                            CData.SwErr .Reset(); //190110 ksg : 수정
                            CData.SwIdle.Reset(); //190110 ksg : 수정
                            CData.SwRun .Reset(); //190110 ksg : 수정
                                                   //190221 ksg : Lot Open시 화면 데이터 클리어
                            CData.L_GRD.m_aInspBf  = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //메뉴얼
                            CData.L_GRD.m_aInspAf  = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //메뉴얼
                            CData.GrData[0].aMeaBf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //Auto
                            CData.GrData[0].aMeaAf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //Auto

                            CData.R_GRD.m_aInspBf  = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //메뉴얼
                            CData.R_GRD.m_aInspAf  = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //메뉴얼
                            CData.GrData[1].aMeaBf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //Auto
                            CData.GrData[1].aMeaAf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //Auto

                            // 200316 mjy : Unit일 시 측정 배열 초기화
                            if (CDataOption.Package == ePkg.Unit)
                            {
                                for (int iU = 0; iU < CData.Dev.iUnitCnt; iU++)
                                {
                                    CData.GrData[0].aUnit[iU].aMeaBf = new double[CData.Dev.iRow, CData.Dev.iCol];
                                    CData.GrData[0].aUnit[iU].aMeaAf = new double[CData.Dev.iRow, CData.Dev.iCol];
                                    CData.GrData[0].aUnit[iU].aErrBf = new bool[CData.Dev.iRow, CData.Dev.iCol];
                                    CData.GrData[0].aUnit[iU].aErrAf = new bool[CData.Dev.iRow, CData.Dev.iCol];
                                    CData.GrData[1].aUnit[iU].aMeaBf = new double[CData.Dev.iRow, CData.Dev.iCol];
                                    CData.GrData[1].aUnit[iU].aMeaAf = new double[CData.Dev.iRow, CData.Dev.iCol];
                                    CData.GrData[1].aUnit[iU].aErrBf = new bool[CData.Dev.iRow, CData.Dev.iCol];
                                    CData.GrData[1].aUnit[iU].aErrAf = new bool[CData.Dev.iRow, CData.Dev.iCol];
                                }
                            }
                            // 2021.04.26 lhs Start 
                            // JCET VOC : LotEnd시 LotName 화면에서 지우기
                            if (CData.CurCompany == ECompany.JCET)
                            {
                                CData.bEraseLotName = false;
                            }
                            // 2021.04.26 lhs End
                        }
                    }
                }
            }
            else
            {
                if (CMsg.Show(eMsg.Warning, "Warning", "Do you want LOT end?") == DialogResult.Cancel) { return; }

                //20191206 ghk_lotend_placemgz
                if (CDataOption.LotEndPlcMgz == eLotEndPlaceMgz.Use)
                {//기능 사용 시
                    if (!CSQ_Main.It.CheckDoor())
                    {//도어 열려 있을 경우 도어 닫아달라는 메시지 생성
                        CMsg.Show(eMsg.Notice, "Warning", "Door Close Please");
                        return;
                    }
                }
                //

                //20190618 ghk_dfserver
                //if ((CData.CurCompany == eCompany.AseKr || CData.CurCompany == eCompany.AseK26) && !CData.Dev.bDfServerSkip)
                if (CDataOption.eDfserver == eDfserver.Use && !CData.Dev.bDfServerSkip)
                {
                    timer_Dfserver2.Enabled = true;
                    m_bLotEnd = true;
                    CDf.It.SendLotEnd();
                }
                else
                {
                    //20190624 josh
                    //jsck secsgem
                    //svid 999103 lot ended time
                    if (CData.GemForm != null) CData.GemForm.OnLotEnded(CData.LotInfo.sLotName, CData.LotInfo.iTInCnt.ToString(), CData.LotInfo.iTOutCnt.ToString());
                    //ksg 추가
                    CData.LotInfo.bLotOpen = false;
                    LotInfoClear(); // vwAuto2
                    //190718 ksg : 메뉴얼 Lot end시에도 Lot Log 파일 남게 수정
                    CData.SpcInfo.sEndTime = DateTime.Now.ToString("HH:mm:ss");
                    // 2020.12.10 JSKim St
                    if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                    {
                        CErr.SaveErrLog_LotInfo();
                    }
                    // 2020.12.10 JSKim Ed
                    CSpc.It.SaveLotInfo();
                    CData.SpcInfo.sLotName = "";
                    CData.SpcInfo.sDevice  = "";
                    // LOT End
                    btn_Lot.Text = "LOT\r\nOPEN";
                    btn_Lot.BackColor = Color.DarkOrange;
                    btn_Lot.FlatAppearance.MouseDownBackColor = Color.OrangeRed;
                    btn_Lot.FlatAppearance.MouseOverBackColor = Color.Orange;

                    //20191206 ghk_lotend_placemgz
                    if(CDataOption.LotEndPlcMgz == eLotEndPlaceMgz.Use)
                    {//해당 기능 사용 시
                        CSQ_Man.It.Seq = ESeq.ONLOFL_Place;
                        CSQ_Man.It.bBtnShow = false;
                    }
                    //

                    if(CIO.It.Get_Y(eY.GRDL_TbFlow)) CData.L_GRD.ActWater(false); //200414 ksg : Lot End시 Table Water Off
                    if(CIO.It.Get_Y(eY.GRDR_TbFlow)) CData.R_GRD.ActWater(false); //200414 ksg : Lot End시 Table Water Off

                    //20200618 lks
                    if (CData.Opt.bQcUse)
                    {
                        //if (CTcpIp.It.IsConnect() && CTcpIp.It.bIsConnect)
                        //{
                        //  CTcpIp.It.SendLotEnd();
                        //}
                        if (CGxSocket.It.IsConnected())
                        {
                            CGxSocket.It.SendMessage("LotEnd");

                            _SetLog("[SEND](EQ->QC) LotEnd");    // 2022.01.17 SungTae : [추가] QC-Vision 관련 Log 추가
                        }
                    }
                }
                //
            }
        }

        private void btn_Run_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CSQ_Main.It.m_bBtStart = true;
        }

        private void btn_Stop_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CSQ_Main.It.m_bBtStop = true;            
        }

        private void btn_Reset_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CSQ_Main.It.m_bBtReset = true;            
        }

        public void LotInfoClear()  // vwAuto2
        {
            _SetLog("Click");

            int iMax;
            if(CData.CurCompany == ECompany.SkyWorks) iMax = (int)EPart.OFR;
            else                                      iMax = (int)EPart.OFL;
            //for(int i = 0; i <= (int)EPart.OFL; i++)
            for(int i = 0; i <= iMax; i++)
            {
                CData.Parts[i].iStat    = ESeq.Idle;
                CData.Parts[i].sLotName = "";
                CData.Parts[i].iMGZ_No  = 0 ;
                CData.Parts[i].sMGZ_ID  = "";
                CData.Parts[i].iSlot_No = 0 ;
                for(int j = 0; j < CData.Dev.iMgzCnt; j++) CData.Parts[i].iSlot_info[j] = 0;
                //CData.Parts[i].dMeaBf = 0;
                //CData.Parts[i].dMeaAf = 0;
                CData.Parts[i].sBcr = "";

                //20190604 ghk_onpbcr
                CData.Parts[i].bBcr = false;
                CData.Parts[i].bOri = false;
                //

                // 2020.12.12 JSKim St - 음 결국은 다 ExistStrip... 지워 말어??
                //20191121 ghk_display_strip
                if (CDataOption.DisplayStrip == eDisplayStrip.NotUse)
                {//View 화면 점멸 기능 사용 안할 경우
                    CData.Parts[i].bExistStrip = false;
                }
                else
                {//View 화면 정멸 기능 사용 할 경우
                    if (i != (int)EPart.DRY)
                    {//현재 파트가 Dry 파트가 아닐 경우 시퀀스 자재 상태 초기화
                        CData.Parts[i].bExistStrip = false;
                    }
                }
                //

                CData.Parts[i].bExistStrip = false;
                // 2020.12.12 JSKim Ed
                CData.bPreLotEndMsgShow    = false; //koo : Qorvo Lot Rework

                CData.Parts[i].b18PMeasure = false; //200712 jhc : 18 포인트 측정 (ASE-KR VOC) :: Lot Open 후 첫 n장의 스트립인지 여부 (Before Measure 시작 기준)
            }

            //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)
            //201012 jhc : (KEYENCE BCR)/(RFID) 옵션으로 구분 적용
            if ((CDataOption.RFID == ERfidType.Keyence) && (CDataOption.OnlRfid == eOnlRFID.Use || CDataOption.OflRfid == eOflRFID.Use))
            //if ((CData.CurCompany == ECompany.SPIL) && (CDataOption.OnlRfid == eOnlRFID.Use || CDataOption.OflRfid == eOflRFID.Use))
            {
                //OnLoader 바코드 리더 버퍼 클리어
                if ((CDataOption.OnlRfid == eOnlRFID.Use) && (!CData.Opt.bOnlRfidSkip))
                {
                    CKeyenceBcr.It.SendCommand(EKeyenceBcrPos.OnLd, eKeyenceBcrCmd.LOFF);
                    CKeyenceBcr.It.SendCommand(EKeyenceBcrPos.OnLd, eKeyenceBcrCmd.BCLR);
                }
                //OffLoader 바코드 리더 버퍼 클리어
                if ((CDataOption.OflRfid == eOflRFID.Use) && (!CData.Opt.bOflRfidSkip))
                {
                    CKeyenceBcr.It.SendCommand(EKeyenceBcrPos.OffLd, eKeyenceBcrCmd.LOFF);
                    CKeyenceBcr.It.SendCommand(EKeyenceBcrPos.OffLd, eKeyenceBcrCmd.BCLR);
                }
            }

            CData.LotInfo.i18PMeasuredCount = 0; //200730 jhc : 18 포인트 측정 (ASE-KR VOC)
            CData.bWhlDrsLimitAlarmSkip = false; //200730 jhc : Wheel/Dresser Limit Alaram 보류용 (ASE-Kr VOC) - Lot 초기화 시 Wheel/Dresser Limit Alarm 보류 해제
        }


        // 2021-03-05, jhLee : Off loader의 Top/Bottom Magazine을 모두 Place 하였는지 여부를 확인
        public bool CheckOffLoaderMagazine()
        {
            // Skyworks일 경우 Top/Bottom magazine의 존재여부를 모두 검사한다.
            if (CData.CurCompany == ECompany.SkyWorks)
            {
                // Top 혹은 Bottom에 Magazine이 감지되는지 여부 조회
                return (CSq_OfL.It.Get_BtmMgzChk() || CSq_OfL.It.Get_TopMgzChk());
            }

            // 그 외의 Site는 기존대로 Bottom magazine만 조사한다.
            return CSq_OfL.It.Get_BtmMgzChk();
        }

        public bool CheckStatus()
        {
            _SetLog("Click");

            bool Ret = true;

            bool ChkLoadMgz     = CSq_OnL.It.Chk_ClampMgz ();
            bool ChkOffLoadMgz  = CheckOffLoaderMagazine();  //old CSq_OfL.It.Get_BtmMgzChk();  // 2021-03-05, jhLee : Magazine 존재여부 체크 함수
            bool ChkInrailStrip = CIO.It.Get_X(eX.INR_StripBtmDetect);
            bool ChkOnLPicker   = CIO.It.Get_X(eX.ONP_Vacuum        );
            bool ChkLeftTable   = CIO.It.Get_X(eX.GRDL_TbVaccum     );
            bool ChkRightTable  = CIO.It.Get_X(eX.GRDR_TbVaccum     );
            // 210928 syc : 2004U 
            bool ChkOffLPicker = true;
            if (CDataOption.Use2004U) { ChkOffLPicker = CIO.It.Get_X(eX.OFFP_Carrier_Clamp_Close); }
            else { ChkOffLPicker = CIO.It.Get_X(eX.OFFP_Vacuum); }

            if(ChkLoadMgz    ) Ret = false;
            if(ChkOffLoadMgz ) Ret = false;
            if(ChkInrailStrip) Ret = false;
            if(ChkOnLPicker  ) Ret = false;
            if(ChkLeftTable  ) Ret = false;
            if(ChkRightTable ) Ret = false;
            if(ChkOffLPicker ) Ret = false;
                        
            return Ret;
        }

        private void btn_PauseOn_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            if (CSQ_Main.It.bSetUpStripStatus)
            {
                if (CMsg.Show(eMsg.Warning, "Warning", "Set up strip Run now. Do you want to change status to Load Stop?") == DialogResult.OK)
                {
                    if (CSQ_Main.It.m_iStat != EStatus.Auto_Running) return;
                    CSQ_Main.It.m_bPause = true;                // 즉시 투입 중지 
                    CSQ_Main.It.m_bAutoLoadingStop = false;     // 2020-11-17, jhLee :  자동 투입 중지를 비활성화 시킨다.
                    CSQ_Main.It.bSetUpStripStatus = false;
                }
                else return;
            }
            else
            {
                if (CSQ_Main.It.m_iStat != EStatus.Auto_Running) return;
                CSQ_Main.It.m_bPause = true;                // 즉시 투입 중지 
                CSQ_Main.It.m_bAutoLoadingStop = false;     // 2020-11-17, jhLee :  자동 투입 중지를 비활성화 시킨다.
            }
        }

        private void btn_PauseOff_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            if (CSQ_Main.It.bSetUpStripStatus)
            {
                if (CMsg.Show(eMsg.Warning, "Warning", "Set up strip Run now. Do you want to stop set up strip function?") == DialogResult.OK)
                {
                    CSQ_Main.It.m_bPause = false;
                    CData.bWhlDrsLimitAlarmSkip = false; //200730 jhc : Wheel/Dresser Limit Alaram 보류용 (ASE-Kr VOC) - LOADING STOP OFF 시 Wheel/Dresser Limit Alarm 보류 해제

                    CSQ_Main.It.m_bAutoLoadingStop = false;     // 자동 투입 중지를 비활성화 시킨다.
                }
                else return;
            }
            else
            {
                CSQ_Main.It.m_bPause = false;
                CData.bWhlDrsLimitAlarmSkip = false; //200730 jhc : Wheel/Dresser Limit Alaram 보류용 (ASE-Kr VOC) - LOADING STOP OFF 시 Wheel/Dresser Limit Alarm 보류 해제

                CSQ_Main.It.m_bAutoLoadingStop = false;     // 자동 투입 중지를 비활성화 시킨다.
            }
        }

        //koo : Qorvo 언어변환. --------------------------------------------------------------------------------
        //SetConvertLanguage(lblS_GrdL_PumpA.Name,CData.Opt.iSelLan,"Overload");
        private void SetWriteLanguage(string controlName,string text)
        {
            Control[] ctrls = this.Controls.Find(controlName, true);
            ctrls[0].GetType().GetProperty("Text").SetValue(ctrls[0], text, null);
        }

        public void SetGrindParameter()
        {


        }

        private void timer_Dfserver2_Tick(object sender, EventArgs e)
        {//20190618 ghk_dfserver
            if (m_bReady)
            {
                if (CDf.It.ReciveAck((int)ECMD.scReady))
                {
                    m_bReady = false;
                    timer_Dfserver2.Enabled = false;
                    // LOT Open
                    using (frmLot mForm = new frmLot())
                    {
                        if (mForm.ShowDialog() == DialogResult.OK)
                        {
                            //190107 ksg : Lot Open시 장비 Check
                            if (!CheckStatus())
                            {
                                CMsg.Show(eMsg.Error, "ERROR", "Magazin Or Strip Remove Please");
                                return;
                            }

                            LotInfoClear(); // vwAuto2
                            _Set();

                            btn_Lot.Text = "LOT\r\nEND";
                            btn_Lot.BackColor = Color.FromArgb(210, 56, 80);
                            btn_Lot.FlatAppearance.MouseDownBackColor = Color.DarkRed;
                            btn_Lot.FlatAppearance.MouseOverBackColor = Color.IndianRed;
                            //추가
                            CData.IdleS = DateTime.Now; //190108 ksg : 추가 및 초기화
                            CData.JamS = DateTime.Now; //ksg : 초기화
                            CData.RunS = DateTime.Now; //ksg : 추가 및 초기화

                            CData.SwErr.Reset(); //190110 ksg : 수정
                            CData.SwIdle.Reset(); //190110 ksg : 수정
                            CData.SwRun.Reset(); //190110 ksg : 수정
                                                 //190221 ksg : Lot Open시 화면 데이터 클리어
                            CData.L_GRD.m_aInspBf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //메뉴얼
                            CData.L_GRD.m_aInspAf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //메뉴얼
                            CData.GrData[0].aMeaBf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //Auto
                            CData.GrData[0].aMeaAf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //Auto

                            CData.R_GRD.m_aInspBf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //메뉴얼
                            CData.R_GRD.m_aInspAf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //메뉴얼
                            CData.GrData[1].aMeaBf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //Auto
                            CData.GrData[1].aMeaAf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //Auto

                            // 200316 mjy : Unit일 시 측정 배열 초기화
                            if (CDataOption.Package == ePkg.Unit)
                            {
                                for (int iU = 0; iU < CData.Dev.iUnitCnt; iU++)
                                {
                                    CData.GrData[0].aUnit[iU].aMeaBf = new double[CData.Dev.iRow, CData.Dev.iCol];
                                    CData.GrData[0].aUnit[iU].aMeaAf = new double[CData.Dev.iRow, CData.Dev.iCol];
                                    CData.GrData[1].aUnit[iU].aMeaBf = new double[CData.Dev.iRow, CData.Dev.iCol];
                                    CData.GrData[1].aUnit[iU].aMeaAf = new double[CData.Dev.iRow, CData.Dev.iCol];
                                }
                            }
                        }
                    }
                }
            }
            if (m_bLotEnd)
            {
                if (CDf.It.ReciveAck((int)ECMD.scLotEnd))
                {
                    //20190626 ghk_dfserver
                    tp_Lot.SelectedIndex = 0;
                    //

                    CData.LotInfo.bLotOpen = false;
                    LotInfoClear(); // vwAuto2
                    // LOT End
                    btn_Lot.Text = "LOT\r\nOPEN";
                    btn_Lot.BackColor = Color.DarkOrange;
                    btn_Lot.FlatAppearance.MouseDownBackColor = Color.OrangeRed;
                    btn_Lot.FlatAppearance.MouseOverBackColor = Color.Orange;

                    m_bLotEnd = false;
                    timer_Dfserver2.Enabled = false;
                }
            }
        }

        private void btn_BcrKeyIn_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            string sPass = "";
            sPass = txt_BcrPass.Text;

            if (txt_Bcr.Text == "")
            {
                lbl_BcrMsg.Text = "Need Input Bcr Id";
                return;
            }

            if(CData.CurCompany == ECompany.ASE_K12)
            {
                if (CheckBcrPass(sPass))
                {
                    CData.Bcr.sBcr = txt_Bcr.Text;

                    if (CDataOption.CurEqu == eEquType.Nomal)
                    {//바코드 인레일에 있을 경우
                        CSq_Inr.It.m_bBcrKeyIn = true;
                    }
                    else
                    {//바코드 온로더피커에 잇을 경우
                        CSq_OnP.It.m_bBcrKeyIn = true;
                    }

                    lbl_BcrMsg.Text = "";

                    return;
                }
                else
                {
                    lbl_BcrMsg.Text = "Wrong Passwaord";
                    return;
                }
            }
            //else if(CData.CurCompany == eCompany.JSCK)
            else if(CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK || CData.CurCompany == ECompany.JCET) //200121 ksg : , 200625 lks
            {
                if (txt_BcrPass.Text == "")
                {
                    lbl_BcrMsg.Text = "Need Input Bcr Id";
                    return;
                }

                if(txt_Bcr.Text == txt_BcrPass.Text)
                {
                    CData.Bcr.sBcr = txt_Bcr.Text;

                    if (CDataOption.CurEqu == eEquType.Nomal)
                    {//바코드 인레일에 있을 경우
                        CSq_Inr.It.m_bBcrKeyIn = true;
                    }
                    else
                    {//바코드 온로더피커에 잇을 경우
                        CSq_OnP.It.m_bBcrKeyIn = true;
                    }

                    lbl_BcrMsg.Text = "";
                }
                else
                {
                    lbl_BcrMsg.Text = "Different input Bcr";
                    return;
                }
            }
            else
            {
                CData.Bcr.sBcr = txt_Bcr.Text;
                //20191204 ghk_bcrocr
                CData.Bcr.sOcr = txt_Bcr.Text;

                if (CDataOption.CurEqu == eEquType.Nomal)
                {//바코드 인레일에 있을 경우
                    CSq_Inr.It.m_bBcrKeyIn = true;
                }
                else
                {//바코드 온로더피커에 잇을 경우
                    CSq_OnP.It.m_bBcrKeyIn = true;
                }

                lbl_BcrMsg.Text = "";
            }
            //191107 ksg : 
            txt_Bcr.Text = "";
        }

        private bool CheckBcrPass(string sPass)
        {
            _SetLog("Click");

            bool bRet = false;
            CIni m_Ini;
            string sPath   = GV.PATH_CONFIG + "PW.cfg";
            string sSec    = "";
            string sKey    = "";
            string sEng    = "";
            string sManger = "";

            m_Ini = new CIni(sPath);
            sSec = "PW";
            sKey = "TECHNICIAN";
            sEng = m_Ini.Read(sSec, sKey);
            sKey = "ENGINEER";
            sManger = m_Ini.Read(sSec, sKey);

            //if (CurLevel == eARL.Engineer && txtPw.Text == "test")
            if (sPass == sEng)
            { bRet = true; }
            else if (sPass == sManger)
            { bRet = true; }
            else
            { bRet = false; }
            
            return bRet;
        }

        private void btn_BcrErr_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            if (CDataOption.CurEqu == eEquType.Nomal)
            {//바코드 인레일에 있을 경우
                CSq_Inr.It.m_bBcrErr = true;
            }
            else
            {//바코드 온로더 피커에 있을 경우
                CSq_OnP.It.m_bBcrErr = true;
            }           
        }

        //201103 jhc : Orientation Check 1회 Skip 기능 추가 (ASE-Kr VOC)
        private void btn_OriSkipOneTime_Click(object sender, EventArgs e)
        {
            if (CData.Dev.bOriOneTimeSkipUse)
            { CData.bOriOneTimeSkip = !CData.bOriOneTimeSkip; } //1회의 한하여 Orientation 검사 Skip (Toggle)

            _Set_OriSkipBtn_Color();
        }

        private void _Set_OriSkipBtn_ResetBtn()
        {
            if (GV.ORIENTATION_ONE_TIME_SKIP && CData.Dev.bOriOneTimeSkipUse && CData.bOriOneTimeSkipBtnView) //201117 jhc : Orientation Error발생 시에만 버튼 표시, 검사 완료 후 다시 감춤
            {
                if (btn_OriSkipOneTime.Visible == false)
                {
                    btn_Reset.Location = new System.Drawing.Point(1000,791);
                    btn_Reset.Size = new System.Drawing.Size(170,94);
                    btn_OriSkipOneTime.Visible = true;
                }
            }
            else
            {
                if (btn_OriSkipOneTime.Visible == true)
                {
                    btn_OriSkipOneTime.Visible = false;
                    btn_Reset.Location = new System.Drawing.Point(870,791);
                    btn_Reset.Size = new System.Drawing.Size(300,94);
                }
            }
        }

        private void _Set_OriSkipBtn_Color()
        {
            if (CData.Dev.bOriOneTimeSkipUse && CData.bOriOneTimeSkip)
            {
                btn_OriSkipOneTime.BackColor = System.Drawing.Color.Violet;
                btn_OriSkipOneTime.ForeColor = System.Drawing.Color.Blue;
            }
            else
            {
                btn_OriSkipOneTime.BackColor = System.Drawing.Color.BlueViolet;
                btn_OriSkipOneTime.ForeColor = System.Drawing.Color.White;
            }
        }

        private void M_tmFlicker_Tick(object sender, EventArgs e)
        {
            if (CData.Dev.bOriOneTimeSkipUse && CData.bOriOneTimeSkip)
            {
                if (btn_OriSkipOneTime.ForeColor == System.Drawing.Color.White)
                { btn_OriSkipOneTime.ForeColor = System.Drawing.Color.Blue; }
                else
                { btn_OriSkipOneTime.ForeColor = System.Drawing.Color.White; }
            }
            else
            {
                btn_OriSkipOneTime.BackColor = System.Drawing.Color.BlueViolet;
                btn_OriSkipOneTime.ForeColor = System.Drawing.Color.White;
            }
        }


        // 2020-11-17, jhLee, Auto Loading Stop 설정
        private void btn_AutoLDStop_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            if (!CSQ_Main.It.m_bAutoLoadingStop && !CSQ_Main.It.m_bPause)       // 현재 Loading Stop이 설정되지 않았다면
            {
                CSQ_Main.It.m_iLoadingCount = 0;           // 자동 투입중지 모드를 설정 한 뒤 투입된 제품 수량 초기화
                CSQ_Main.It.m_bAutoLoadingStop = true;     // 자동으로 투입 중지를 활성화 시킨다.
            }
        }
        //

        // 화면이 보여질때마다 체크해야하는 내용 지정
        private void vwAuto2_VisibleChanged(object sender, EventArgs e)
        {
            // Multi-LOT일 경우 LOT Input 버튼으로 바꾼다.
            if (CData.IsMultiLOT())
            {
                if (btn_Lot.Text != "LOT Input")
                {
                    btn_Lot.Text = "LOT Input";
                }
            }
            else if (btn_Lot.Text == "LOT Input")
            {
                btn_Lot.Text = "LOT\r\nOPEN";
            }

        }
        //
    }
}
