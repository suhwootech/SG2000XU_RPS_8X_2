using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace SG2000X
{
    public partial class vwDev_02Prm_1Str : UserControl
    {
        public delegate void DualEvt(bool bVal);
        public event DualEvt GoDual;

        public vwDev_02Prm_1Str()
        {
            if      ((int)ELang.English == CData.Opt.iSelLan)   {   System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");     }
            else if ((int)ELang.Korea   == CData.Opt.iSelLan)   {   System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ko-KR");     }
            else if ((int)ELang.China   == CData.Opt.iSelLan)   {   System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-Hans");   }

            InitializeComponent();

            if (CData.CurCompany == ECompany.ASE_KR   ||
                CData.CurCompany == ECompany.Qorvo    || CData.CurCompany == ECompany.Qorvo_DZ ||
                CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC)        // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
            {
                pnl_SpindleLoad.Visible = true;
                if (GV.DEV_WHL_SYNC)
                { pnl_Whl.Visible = true; }
            }
            else
            {
                pnl_SpindleLoad.Visible = false;
                pnl_Whl.Visible = false;
            }

            // 2022.08.30 lhs Start : Spindle Current High/Low Limit (SCK+ VOC) 
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            {
                //pnl_SpindleCurrent.Visible = true; //임시로 안보에게
            }
            // 2022.08.30 lhs End

            // 200630 jym : 추가
            pnl_Unit.Visible = (CDataOption.Package == ePkg.Strip) ? false : true;
        }

        #region Private method
        private void _WhlList()
        {
            string sPath = GV.PATH_WHEEL;

            sPath += "Left\\";
            cmb_WhlL.Items.Clear();
            if (Directory.Exists(sPath))
            {
                DirectoryInfo mFile = new DirectoryInfo(sPath);
                foreach (DirectoryInfo mInfo in mFile.GetDirectories())
                {
                    cmb_WhlL.Items.Add(mInfo.Name);
                }
            }
            else
            { Directory.CreateDirectory(sPath); }

            sPath = GV.PATH_WHEEL + "Right\\";
            cmb_WhlR.Items.Clear();
            if (Directory.Exists(sPath))
            {
                DirectoryInfo mFile = new DirectoryInfo(sPath);
                foreach (DirectoryInfo mInfo in mFile.GetDirectories())
                {
                    cmb_WhlR.Items.Add(mInfo.Name);
                }
            }
            else
            { Directory.CreateDirectory(sPath); }
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

            CLog.Save_Log(eLog.None, eLog.OPL, string.Format("{0}.cs    {1}()    {2}    Lv:{3}", sCls, sMth, sMsg, CData.Lev));
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
            if (CData.CurCompany == ECompany.ASE_KR && GV.DEV_WHL_SYNC)
            { _WhlList(); }

            Set();

            if (CData.CurCompany != ECompany.Qorvo    &&
                CData.CurCompany != ECompany.Qorvo_DZ &&
                CData.CurCompany != ECompany.Qorvo_RT &&       // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                CData.CurCompany != ECompany.Qorvo_NC &&       // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                CData.CurCompany != ECompany.SkyWorks && 
                CData.CurCompany != ECompany.ASE_K12  &&
                CData.CurCompany != ECompany.ASE_KR   &&
                CData.CurCompany != ECompany.SST      &&
                CData.CurCompany != ECompany.USI      &&
                CData.CurCompany != ECompany.SCK      &&
                CData.CurCompany != ECompany.JSCK     &&
                CData.CurCompany != ECompany.JCET)
            {
                pnl_DFOption.Visible = false;
                CData.Dev.bDynamicSkip = true;
                //20190618 ghk_dfserver
                CData.Dev.bDfServerSkip = true;
            }

            //20191029 ghk_dfserver_notuse_df
            if(CDataOption.eDfserver == eDfserver.Use && CDataOption.MeasureDf == eDfServerType.NotMeasureDf)
            {//DF서버 사용시 DF측정 안할 경우
                groupBox3.Visible       = false;
                label386.Visible        = false;
                ckb_DynamicSkip.Visible = false;

                lbl_PCBThick.Visible        = false;
                lblP_SrLMold.Visible    = false;
                txtP_SrLPCB.Visible     = false;
                txtP_SrLMold.Visible    = false;
                label288.Visible        = false;
                lblP_SrLMold_mm.Visible = false;
                label217.Visible        = false;
                lblP_SrRMold.Visible        = false;
                txtP_SrRPCB.Visible     = false;
                txtP_SrRMold.Visible    = false;
                label291.Visible        = false;
                lblP_SrRMold_mm.Visible        = false;

                label385.Visible        = false;
                label402.Visible        = false;
                label405.Visible        = false;
                label2.Visible          = false; //20200402 jhc : DF InRail Base 측정 값 허용 범위(+/-)
                txt_PcbRange.Visible    = false;
                txt_PcbMeanRange.Visible = false;
                txt_DFRnr.Visible        = false;
                txt_InRailYAlign.Visible = false;
                txt_BaseRange.Visible   = false; //20200402 jhc : DF InRail Base 측정 값 허용 범위(+/-)
                label383.Visible        = false;
                label382.Visible        = false;
                label403.Visible        = false;
                label404.Visible        = false;
                label3.Visible          = false; //20200402 jhc : DF InRail Base 측정 값 허용 범위(+/-)

                CData.Dev.bDynamicSkip  = false;
            }

            if (CDataOption.eDfserver == eDfserver.NotUse)
            {
                ckb_DfServerSkip.Visible = false;
            }

            // 2021.07.12 lhs Start : Grinding 자재 Side 선택 UI
            if (CDataOption.UseNewSckGrindProc)
            {
                SckProcItemVisible(true);
                chbMeasureMode.Visible = true;
            }
            else
            {
                SckProcItemVisible(false);
                rdb_PCB_BtmSide.Location    = rdb_PCB_BtmSingle.Location;

                if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                {
                    chbMeasureMode.Visible  = true;
                    rdb_PCB_TopSide.Text    = "TOP Side (SSM)";
                    rdb_PCB_BtmSide.Text    = "Bottom Side (DSM)";
                }
                else if (CData.CurCompany == ECompany.Qorvo_NC)
                {
                    chbMeasureMode.Visible = true;
                    rdb_PCB_TopSide.Text = "TOP Side";
                    rdb_PCB_BtmSide.Text = "Bottom Side";
                }
                else
                {
                    chbMeasureMode.Visible  = false;
                    rdb_PCB_TopSide.Text    = "TOP Side";
                    rdb_PCB_BtmSide.Text    = "Bottom Side";
                }
                lblP_SrSideComment.Text = "*Bottom side: Can't use Dynamic Function ";
            }
            // 2021.07.12 lhs End
            //220111 pjh : ASE KH 요청으로 명칭 변경
            if(CData.CurCompany == ECompany.ASE_K12)
            {
                ckb_DynamicSkip.Text = "In-Rail Probe Sensor Module Skip";
                lblP_SrSideComment.Text = "Bottom side can't use In-Rail Praobe Sensor Module";
            }
            //

            //220111 pjh : ASE KH 요청으로 Bottom Grinding 선택 시 명칭 변경
            if (CData.CurCompany == ECompany.ASE_K12)
            {
                if (rdb_PCB_BtmSide.Checked)
                {
                    lbl_PCBThick.Text = "GL1 total thickness";
                }
                else
                {
                    lbl_PCBThick.Text = "PCB thickness";
                }
            }
            //
        }

        private void SckProcItemVisible(bool bVisible)
        {
            rdb_PCB_TopDouble.Visible = bVisible;
            rdb_PCB_BtmSingle.Visible = bVisible;

            if (bVisible)
            {
                rdb_PCB_TopSide.Text    = "TOP Side (Single)";
                rdb_PCB_TopDouble.Text  = "TOP Side (Double)";
                rdb_PCB_BtmSide.Text    = "BTM Side (Double)";
                rdb_PCB_BtmSingle.Text  = "BTM Side (Single)";
                lblP_SrSideComment.Text = "*TOP/BTM Double side: Can't use Dynamic Function ";
            }
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
            lblP_SrNm .Text = CData.Dev.sName;
            lblP_SrLst.Text = CData.Dev.dtLast.ToString();
            txtP_SrW  .Text = CData.Dev.dShSide.ToString();
            txtP_SrH  .Text = CData.Dev.dLnSide.ToString();
            if (CData.Dev.bDual == eDual.Normal)
            {
                rdbP_SrNormal.Checked = true;
                lblP_SrLTitle.Text    = "Strip Information";
                pnlP_SrR     .Visible = false;
            }
            else
            {
                rdbP_SrDual  .Checked = true;
                lblP_SrLTitle.Text    = "Left Strip Information";
                pnlP_SrR     .Visible = true;
            }
            chbMeasureMode.Checked = CData.Dev.bMeasureMode; //20200825 lks     // 2020.09.11 JSKim Add
            txtP_SrLDrs  .Text = CData.Dev.aData[(int)EWay.L].dDrsPrid.ToString();
            cmbLDrs_Way.SelectedIndex = CData.Dev.aData[(int)EWay.L].iDrs_YEnd_Dir;
            cmbRDrs_Way.SelectedIndex = CData.Dev.aData[(int)EWay.R].iDrs_YEnd_Dir;

            txtP_SrRDrs  .Text = CData.Dev.aData[(int)EWay.R].dDrsPrid.ToString();
            txtP_SrLTotal.Text = CData.Dev.aData[0].dTotalTh.ToString();
            txtP_SrLPCB  .Text = CData.Dev.aData[0].dPcbTh.ToString();
            txtP_SrLMold .Text = CData.Dev.aData[0].dMoldTh.ToString();
            txtP_SrRTotal.Text = CData.Dev.aData[1].dTotalTh.ToString();
            txtP_SrRPCB  .Text = CData.Dev.aData[1].dPcbTh.ToString();
            txtP_SrRMold .Text = CData.Dev.aData[1].dMoldTh.ToString();

            //200310 ksg : Spindle 부하
            txtP_LSpdAuto .Text = CData.Dev.aData[(int)EWay.L].dSpdAuto .ToString();
            txtP_LSpdError.Text = CData.Dev.aData[(int)EWay.L].dSpdError.ToString();
            txtP_RSpdAuto .Text = CData.Dev.aData[(int)EWay.R].dSpdAuto .ToString();
            txtP_RSpdError.Text = CData.Dev.aData[(int)EWay.R].dSpdError.ToString();

            //// 2022.08.30 lhs Start : Spindle Current High/Low Limit 설정
            //txtP_SpdCurrLeftHL.Text     = CData.Dev.aData[(int)EWay.L].nSpdCurrHL.ToString();
            //txtP_SpdCurrLeftLL.Text     = CData.Dev.aData[(int)EWay.L].nSpdCurrLL.ToString();
            //txtP_SpdCurrRightHL.Text    = CData.Dev.aData[(int)EWay.R].nSpdCurrHL.ToString();
            //txtP_SpdCurrRightLL.Text    = CData.Dev.aData[(int)EWay.R].nSpdCurrLL.ToString();
            //// 2022.08.30 lhs End


            //20190218 ghk_dynamicfunction
            ckb_DynamicSkip.Checked = CData.Dev.bDynamicSkip;

            //20190618 ghk_dfserver
            ckb_DfServerSkip.Checked = CData.Dev.bDfServerSkip;

            // 2021.07.12 lhs Start : Side 선택 추가
            rdb_PCB_TopSide.Checked     = false; // 초기화
            rdb_PCB_BtmSide.Checked     = false;
            rdb_PCB_TopDouble.Checked   = false;
            rdb_PCB_BtmSingle.Checked   = false;

            if (CDataOption.UseNewSckGrindProc)
            {
                if      (CData.Dev.eMoldSide == ESide.Top)  // Top Single
                { 
                    rdb_PCB_TopSide.Checked   = true;
                    lblP_SrLMold.Text   = "TOP Mold thickness";
                    lblP_SrRMold.Text   = "TOP Mold thickness";
                } 
                else if (CData.Dev.eMoldSide == ESide.Btm)   // Btm Double
                { 
                    rdb_PCB_BtmSide.Checked = true;
                    lblP_SrLMold.Text   = "BTM Mold thickness";
                    lblP_SrRMold.Text   = "BTM Mold thickness";
                }
                else if (CData.Dev.eMoldSide == ESide.TopD) // Top Double
                { 
                    rdb_PCB_TopDouble.Checked   = true;
                    lblP_SrLMold.Text   = "TOP Mold thickness";
                    lblP_SrRMold.Text   = "TOP Mold thickness";
                } 
                else if (CData.Dev.eMoldSide == ESide.BtmS) // Btm Single
                { 
                    rdb_PCB_BtmSingle.Checked = true;
                    lblP_SrLMold.Text   = "BTM Mold thickness";
                    lblP_SrRMold.Text   = "BTM Mold thickness";
                }
            }
            else  // 기존
            {
                if      (CData.Dev.eMoldSide == ESide.Top)  { rdb_PCB_TopSide.Checked = true; }
                else if (CData.Dev.eMoldSide == ESide.Btm)  { rdb_PCB_BtmSide.Checked = true; } // Btm Double
                else                                        { rdb_PCB_TopSide.Checked = true; } // default
                lblP_SrLMold.Text = "Mold thickness";
                lblP_SrRMold.Text = "Mold thickness";
            }
            // 2021.07.12 lhs End

            if      (CData.Dynamic.iHeightType == 0)    { rdbP_PcbMax.Checked = true;   }
            else if (CData.Dynamic.iHeightType == 1)    { rdbP_PcbMean.Checked = true;  }

            txt_PcbRange    .Text = CData.Dynamic.dPcbRange    .ToString();
            txt_PcbMeanRange.Text = CData.Dynamic.dPcbMeanRange.ToString();
            txt_DFRnr       .Text = CData.Dynamic.dPcbRnr      .ToString();
            txt_InRailYAlign.Text = CData.Dynamic.dYAlign      .ToString(); //190516 ksg :
            txt_BaseRange   .Text = CData.Dynamic.dBaseRange   .ToString(); //20200402 jhc : DF InRail Base 측정 값 허용 범위(+/-)

            if (CDataOption.Package == ePkg.Unit)
            {
                cmb_Unit.SelectedIndex = (int)((CData.Dev.iUnitCnt == 2) ? EUnit.Unit2 : EUnit.Unit4);                
            }

            if (CData.CurCompany == ECompany.ASE_KR && GV.DEV_WHL_SYNC)
            {
                cmb_WhlL.SelectedItem = CData.Dev.aData[0].sWhl;
                cmb_WhlR.SelectedItem = CData.Dev.aData[1].sWhl;
            }
        }

        public void Get()
        {
            double.TryParse(txtP_SrW.Text, out CData.Dev.dShSide);
            double.TryParse(txtP_SrH.Text, out CData.Dev.dLnSide);

            if (rdbP_SrNormal.Checked)  { CData.Dev.bDual = eDual.Normal; }
            else                        { CData.Dev.bDual = eDual.Dual; }
            
            CData.Dev.bMeasureMode = chbMeasureMode.Checked;    //20200825 lks  // 2020.09.11 JSKim Add
            
            double.TryParse(txtP_SrLDrs  .Text, out CData.Dev.aData[(int)EWay.L].dDrsPrid);
            double.TryParse(txtP_SrRDrs  .Text, out CData.Dev.aData[(int)EWay.R].dDrsPrid);
            CData.Dev.aData[(int)EWay.L].iDrs_YEnd_Dir = cmbLDrs_Way.SelectedIndex;
            CData.Dev.aData[(int)EWay.R].iDrs_YEnd_Dir = cmbRDrs_Way.SelectedIndex;            

            double.TryParse(txtP_SrLTotal.Text,             out CData.Dev.aData[0].dTotalTh);   // Left
            double.TryParse(txtP_SrLPCB  .Text,             out CData.Dev.aData[0].dPcbTh  );
            double.TryParse(txtP_SrLMold .Text,             out CData.Dev.aData[0].dMoldTh );

            double.TryParse(txtP_SrRTotal.Text,             out CData.Dev.aData[1].dTotalTh);   // Right
            double.TryParse(txtP_SrRPCB  .Text,             out CData.Dev.aData[1].dPcbTh  );
            double.TryParse(txtP_SrRMold .Text,             out CData.Dev.aData[1].dMoldTh );

            // 2021.07.12 lhs Start : NewSckGrindProc 추가 및 기존과 구분
            if (CDataOption.UseNewSckGrindProc)
            {
                if      (rdb_PCB_TopSide.Checked)   { CData.Dev.eMoldSide = ESide.Top;  }    // Top Single
                else if (rdb_PCB_BtmSide.Checked)   { CData.Dev.eMoldSide = ESide.Btm;  }    // Btm Double
                else if (rdb_PCB_TopDouble.Checked) { CData.Dev.eMoldSide = ESide.TopD; }    // Top Double
                else if (rdb_PCB_BtmSingle.Checked) { CData.Dev.eMoldSide = ESide.BtmS; }    // Btm Single
            }
            else
            {
                if      (rdb_PCB_TopSide.Checked)   { CData.Dev.eMoldSide = ESide.Top;  }    // Top Single
                else if (rdb_PCB_BtmSide.Checked)   { CData.Dev.eMoldSide = ESide.Btm;  }    // Btm Double
            }
            // 2021.07.12 lhs End 

            //20190218 ghk_dynamicfunction
            CData.Dev.bDynamicSkip = ckb_DynamicSkip.Checked;

            //20190618 ghk_dfserver
            CData.Dev.bDfServerSkip = ckb_DfServerSkip.Checked;
            //
            //200310 ksg : Spindle 부하
            double.TryParse(txtP_LSpdAuto .Text, out CData.Dev.aData[(int)EWay.L].dSpdAuto );
            double.TryParse(txtP_LSpdError.Text, out CData.Dev.aData[(int)EWay.L].dSpdError);
            double.TryParse(txtP_RSpdAuto .Text, out CData.Dev.aData[(int)EWay.R].dSpdAuto );
            double.TryParse(txtP_RSpdError.Text, out CData.Dev.aData[(int)EWay.R].dSpdError);

            //// 2022.08.30 lhs Start : Spindle Current High/Low Limit 설정
            //int.TryParse(txtP_SpdCurrLeftHL.Text,   out CData.Dev.aData[(int)EWay.L].nSpdCurrHL);
            //int.TryParse(txtP_SpdCurrLeftLL.Text,   out CData.Dev.aData[(int)EWay.L].nSpdCurrLL);
            //int.TryParse(txtP_SpdCurrRightHL.Text,  out CData.Dev.aData[(int)EWay.R].nSpdCurrHL);
            //int.TryParse(txtP_SpdCurrRightLL.Text,  out CData.Dev.aData[(int)EWay.R].nSpdCurrLL);
            //// 2022.08.30 lhs End

            if (rdbP_PcbMax.Checked)   { CData.Dynamic.iHeightType = 0; }
            else if (rdbP_PcbMean.Checked)  { CData.Dynamic.iHeightType = 1; }

            double.TryParse(txt_PcbRange.Text,      out CData.Dynamic.dPcbRange);
            double.TryParse(txt_PcbMeanRange.Text,  out CData.Dynamic.dPcbMeanRange);
            double.TryParse(txt_DFRnr.Text,         out CData.Dynamic.dPcbRnr);     //190511 ksg :
            double.TryParse(txt_InRailYAlign.Text,  out CData.Dynamic.dYAlign);     //190516 ksg :
            double.TryParse(txt_BaseRange.Text,     out CData.Dynamic.dBaseRange);  //20200402 jhc : DF InRail Base 측정 값 허용 범위(+/-)

            if (CDataOption.Package == ePkg.Unit)
            {
                CData.Dev.iUnitCnt = (cmb_Unit.SelectedIndex == (int)EUnit.Unit2) ? 2 : 4;
            }

            if (CData.CurCompany == ECompany.ASE_KR && GV.DEV_WHL_SYNC)
            {
                CData.Dev.aData[0].sWhl = (cmb_WhlL.SelectedItem == null) ? "" : cmb_WhlL.SelectedItem.ToString();
                CData.Dev.aData[1].sWhl = (cmb_WhlR.SelectedItem == null) ? "" : cmb_WhlR.SelectedItem.ToString();
            }
        }
        #endregion

        private void rdbP_SrDual_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbP_SrDual.Checked)
            {
                pnlP_SrR.Visible = true;
                GoDual(true);
            }
            else
            {
                pnlP_SrR.Visible = false;
                GoDual(false);
            }
        }

        private void ckb_DynamicSkip_CheckedChanged(object sender, EventArgs e)
		{
			if (ckb_DynamicSkip.Checked)
			{
				//20191029 ghk_dfserver_notuse_df
				if (CDataOption.MeasureDf == eDfServerType.MeasureDf)
				{
					ckb_DfServerSkip.Enabled = false;
				}
				ckb_DfServerSkip.Checked = true;
			}
			else
			{
				ckb_DfServerSkip.Enabled = true;

                // 2021.07.26 lhs Start
				if (CDataOption.UseNewSckGrindProc)
				{
                    if (rdb_PCB_TopDouble.Checked)  ckb_DynamicSkip.Checked = true; // Double
                    if (rdb_PCB_BtmSide.Checked)    ckb_DynamicSkip.Checked = true; // Double
                }
                else
                // 2021.07.26 lhs End
                {
                    // 사용일 경우는 PCB Top 일때
                    //20200825 lks  // 2020.09.11 JSKim St
                    //rdb_PCB_TopSide.Checked = true;
                    //rdb_PCB_BtmSide.Checked = false;
                    if (rdb_PCB_BtmSide.Checked == true)
					{
						rdb_PCB_TopSide.Checked = true;
						rdb_PCB_BtmSide.Checked = false;
					}
					// 2020.09.11 JSKim Ed
				}
			}
		}

		// 2020.09.11 JSKim St
		private void rdbP_SrNormal_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbP_SrDual.Checked)
            {
                chbMeasureMode.Checked = false;
            }
        }
        // 2020.09.11 JSKim Ed

        // 2021.07.26 lhs Start : Side 선택에 따른 UI 변경
        private void rdb_PCB_Side_Click(object sender, EventArgs e)
        {
            RadioButton rdoBtn = sender as RadioButton;
            string sTag = rdoBtn.Tag.ToString();

            if (CDataOption.UseNewSckGrindProc)
            {
                //RadioButton rdoBtn = sender as RadioButton;
                //string sTag = rdoBtn.Tag.ToString();

                SckProcItemVisible(true);

                if      (sTag == "TopSingle")   { lblP_SrLMold.Text = lblP_SrRMold.Text = "TOP Mold thickness";   }
                else if (sTag == "BtmSingle")   { lblP_SrLMold.Text = lblP_SrRMold.Text = "BTM Mold thickness";   }
                else if (sTag == "TopDouble")   { lblP_SrLMold.Text = lblP_SrRMold.Text = "TOP Mold thickness"; ckb_DynamicSkip.Checked = true; } // DF 사용안함
                else if (sTag == "BtmDouble")   { lblP_SrLMold.Text = lblP_SrRMold.Text = "BTM Mold thickness"; ckb_DynamicSkip.Checked = true; }// DF 사용안함
                else                            { lblP_SrLMold.Text = lblP_SrRMold.Text = "mode selection error !!!";   }
            }
            else
            {
                SckProcItemVisible(false);
                //lblP_SrLMold.Text = "Mold thickness";   // 기본 Text
            }
            //220111 pjh : ASE KH 요청으로 Bottom Grinding 선택 시 명칭 변경
            if(CData.CurCompany == ECompany.ASE_K12)
            {
                if(sTag == "BtmDouble")
                {
                    lbl_PCBThick.Text = "GL1 total thickness";
                }
                else
                {
                    lbl_PCBThick.Text = "PCB thickness";
                }
            }
            //
        }

    }
}
