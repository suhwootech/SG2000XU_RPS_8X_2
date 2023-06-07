using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace SG2000X
{
    public partial class vwDev_02Prm_2Stp : UserControl
    {
        private EWay m_eWy;
        private int m_iWy;

        private int m_iStepMaxCnt;      // 2020.09.08 SungTae : 3 Step Mode 변경으로 추가

        public vwDev_02Prm_2Stp(EWay eWy)
        {
            if ((int)ELang.English == CData.Opt.iSelLan)    {   System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");     }
            else if ((int)ELang.Korea == CData.Opt.iSelLan) {   System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ko-KR");     }
            else if ((int)ELang.China == CData.Opt.iSelLan) {   System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-Hans");   }
            
            InitializeComponent();

            m_eWy = eWy;
            m_iWy = (int)eWy;

            // 2020.09.08 SungTae
            if (CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)  { m_iStepMaxCnt = GV.StepMaxCnt;    }
            else                                                    { m_iStepMaxCnt = GV.StepMaxCnt_3;  }
                        
            //200214 ksg :
            if (CData.CurCompany == ECompany.SkyWorks)
            {
                txtP_StLR1Spl.ExMax = 4000;
                txtP_StLR2Spl.ExMax = 4000;
                txtP_StLR3Spl.ExMax = 4000;
                txtP_StLFiSpl.ExMax = 4000;

                label379.Text = "Max : 4000";
            }

            //200424 jym : ASE KR 테이블 최소 속도 5로 변경
            if (CData.CurCompany == ECompany.ASE_KR)
            {
                for (int i = 0; i < m_iStepMaxCnt - 1; i++)     // 2020.09.08 SungTae : Modify
                {
                    ((Extended.ExTextBox)(Controls.Find("txtP_StLR" + (i+1)+"Tbl", true)[0])).ExMin = 5;
                }
                txtP_StLFiTbl.ExMin = 5;
                lbl_TblLimit.Text = "Max:200\r\nMin:5";
            }

            // 200327 mjy : 리그라인딩 관련 기능
            lbl_ReSkip    .Visible = CDataOption.IsReSkip ;
            ckb_ReSkip1   .Visible = CDataOption.IsReSkip ;
            ckb_ReSkip2   .Visible = CDataOption.IsReSkip ;
            ckb_ReSkip3   .Visible = CDataOption.IsReSkip ;
            ckb_ReSkip4   .Visible = CDataOption.IsReSkip ; //200414 ksg : 12 Step  기능 추가
            ckb_ReSkip5   .Visible = CDataOption.IsReSkip ; //200414 ksg : 12 Step  기능 추가
            ckb_ReSkip6   .Visible = CDataOption.IsReSkip ; //200414 ksg : 12 Step  기능 추가
            ckb_ReSkip7   .Visible = CDataOption.IsReSkip ; //200414 ksg : 12 Step  기능 추가
            ckb_ReSkip8   .Visible = CDataOption.IsReSkip ; //200414 ksg : 12 Step  기능 추가
            ckb_ReSkip9   .Visible = CDataOption.IsReSkip ; //200414 ksg : 12 Step  기능 추가
            ckb_ReSkip10  .Visible = CDataOption.IsReSkip ; //200414 ksg : 12 Step  기능 추가
            ckb_ReSkip11  .Visible = CDataOption.IsReSkip ; //200414 ksg : 12 Step  기능 추가
            ckb_ReSkip12  .Visible = CDataOption.IsReSkip ; //200414 ksg : 12 Step  기능 추가
            lbl_ReJudLimit.Visible = CDataOption.IsReJudge;
            lbl_ReJud     .Visible = CDataOption.IsReJudge;
            txt_ReJud1    .Visible = CDataOption.IsReJudge;
            txt_ReJud2    .Visible = CDataOption.IsReJudge;
            txt_ReJud3    .Visible = CDataOption.IsReJudge;
            txt_ReJud4    .Visible = CDataOption.IsReJudge;
            txt_ReJud5    .Visible = CDataOption.IsReJudge;
            txt_ReJud6    .Visible = CDataOption.IsReJudge;
            txt_ReJud7    .Visible = CDataOption.IsReJudge;
            txt_ReJud8    .Visible = CDataOption.IsReJudge;
            txt_ReJud9    .Visible = CDataOption.IsReJudge;
            txt_ReJud10   .Visible = CDataOption.IsReJudge;
            txt_ReJud11   .Visible = CDataOption.IsReJudge;
            txt_ReJud12   .Visible = CDataOption.IsReJudge;

            //201125 jhc : Grinding Step별 Correct 기능 제공/감춤 용
            lbl_GrdCorrectRange.Visible = CDataOption.UseGrindingStepCorrect;
            lbl_GrdCorrect.Visible      = CDataOption.UseGrindingStepCorrect;
            for (int i = 1; i <= CDataOption.StepCnt; i++)
            {
                ((Extended.ExTextBox)(Controls.Find("txtP_StLR" + i + "Correct", true)[0])).Visible = CDataOption.UseGrindingStepCorrect;
            }
            //
        }

        #region Private method
        /// <summary>
        /// 해당 View가 Dispose(종료)될 때 실행 됨 
        /// View의 Dispose함수 실행하면 자동으로 실행됨
        /// </summary>
        private void _Release()
        {
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
            Set();

            if (CData.Dev.bDual == eDual.Normal)
            {
                lblP_StLFi_T.Text = "Left Fine Step Setting";
                pnlP_StLFiR.Visible = true;
            }
            else
            {
                lblP_StLFi_T.Text = "Fine Step Setting";
                pnlP_StLFiR.Visible = false;
            }

            // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
            //if(CData.CurCompany == ECompany.Qorvo || (CData.CurCompany == ECompany.Qorvo_DZ) || CData.CurCompany == ECompany.SST) //191202 ksg :
            if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC ||
                CData.CurCompany == ECompany.SST)
            {//파라메터 셋팅에 빨강색 가이드 Label
                label377.Visible = false;
                //lbl_TblLimit.Visible = false;
                label379.Visible = false;
            }

            // 2020.09.08 SungTae : 3 Step Mode
            ViewUpdate(CDataOption.StepCnt);

            //201023 JSKim : Over Grinding Correction - Grinding Count Correction 기능
            lbl_OverGrdCorrection.Visible = CData.Opt.bOverGrdCountCorrectionUse;
            if (CData.Opt.bOverGrdCountCorrectionUse)
            {
                _Update_Ckb_OverGrdCorrectX_By_CkbP_StLRXUse( 1 );
            }
            else
            {
                for (int i = 1; i <= CDataOption.StepCnt; i++)
                {
                    ((CheckBox)(Controls.Find("ckb_OverGrdCorrect" + i, true)[0])).Visible = false;
                }
            }

            // 2021.07.27 lhs Start : Mold 기준 및 Total 기준으로 그라인딩(SCK VOC)
            lblP_StLBaseOnThick.Visible = CDataOption.UseNewSckGrindProc;
            cmbP_StLBaseOnThick.Visible = CDataOption.UseNewSckGrindProc;     
            // 2021.07.27 lhs End

            // 2022.07.25 SungTae Start : [수정] (ASE-KR 개발건) 설정된 Step 수량을 제외한 나머지 Step UI 비활성화
            if(CData.CurCompany == ECompany.ASE_KR)
            {
                lbl_LFixStepCnt.Visible     = true;
                txtP_StLFixStepCnt.Visible  = true;
                btn_LFixStepCnt.Visible     = true;
            }
            else
            {
                lbl_LFixStepCnt.Visible     = false;
                txtP_StLFixStepCnt.Visible  = false;
                btn_LFixStepCnt.Visible     = false;
            }
            // 2022.07.25 SungTae End
        }

        // 2020.09.08 SungTae : 3 Step Mode 추가
        /// <summary>
        /// View가 화면에서 지워질때 실행해야 하는 함수
        /// </summary>
        public void ViewUpdate(int iStepCnt)
        {
            // 2022.07.25 SungTae Start : [수정] (ASE-KR 개발건) 설정된 Step 수량을 제외한 나머지 Step UI 비활성화
            if (iStepCnt/*CDataOption.StepCnt*/ == (int)EStepCnt.DEFAULT_STEP)
            {
                if (CData.CurCompany == ECompany.ASE_KR)
                {
                    for (int i = 0; i < iStepCnt; i++)
                    {
                        ((CheckBox)(Controls.Find("ckbP_StLR" + (i + 1) + "Use", true)[0])).Visible = false;

                        // 별도로 입력한 Step Count 보다 작으면 Display, 아니면 비활성화
                        if (i < CData.Dev.aData[m_iWy].nFixStepCnt)
                        {
                            CData.Dev.aData[m_iWy].aSteps[i].bUse = true;

                            ((CheckBox  )(Controls.Find("ckbP_StLR" + (i + 1) + "Use", true)[0])).Checked = CData.Dev.aData[m_iWy].aSteps[i].bUse;
                            ((Label     )(Controls.Find("lblL_Step" + (i + 1) + "_T" , true)[0])).Visible = true;
                            ((Panel     )(Controls.Find("pnlL_U"    + (i + 1)        , true)[0])).Visible = true;
                        }
                        else
                        {
                            CData.Dev.aData[m_iWy].aSteps[i].bUse = false;

                            ((CheckBox  )(Controls.Find("ckbP_StLR" + (i + 1) + "Use", true)[0])).Checked = CData.Dev.aData[m_iWy].aSteps[i].bUse;
                            ((Label     )(Controls.Find("lblL_Step" + (i + 1) + "_T" , true)[0])).Visible = false;
                            ((Panel     )(Controls.Find("pnlL_U"    + (i + 1)        , true)[0])).Visible = false;
                        }
                    }
                }
                else
                {
                    // 2022.07.25 Start : 추후 삭제 예정
                    //pnlL_U4.Visible = true;
                    //pnlL_U5.Visible = true;
                    //pnlL_U6.Visible = true;
                    //pnlL_U7.Visible = true;
                    //pnlL_U8.Visible = true;
                    //pnlL_U9.Visible = true;
                    //pnlL_U10.Visible = true;
                    //pnlL_U11.Visible = true;
                    //pnlL_U12.Visible = true;

                    //lblL_Step4_T.Visible = true;
                    //lblL_Step5_T.Visible = true;
                    //lblL_Step6_T.Visible = true;
                    //lblL_Step7_T.Visible = true;
                    //lblL_Step8_T.Visible = true;
                    //lblL_Step9_T.Visible = true;
                    //lblL_Step10_T.Visible = true;
                    //lblL_Step11_T.Visible = true;
                    //lblL_Step12_T.Visible = true;

                    //ckbP_StLR4Use.Visible = true;
                    //ckbP_StLR5Use.Visible = true;
                    //ckbP_StLR6Use.Visible = true;
                    //ckbP_StLR7Use.Visible = true;
                    //ckbP_StLR8Use.Visible = true;
                    //ckbP_StLR9Use.Visible = true;
                    //ckbP_StLR10Use.Visible = true;
                    //ckbP_StLR11Use.Visible = true;
                    //ckbP_StLR12Use.Visible = true;
                    // 2022.07.25 End

                    for (int i = 0; i < iStepCnt; i++)
                    {
                        ((CheckBox  )(Controls.Find("ckbP_StLR" + (i + 1) + "Use", true)[0])).Visible = true;
                        ((Label     )(Controls.Find("lblL_Step" + (i + 1) + "_T" , true)[0])).Visible = true;
                        ((Panel     )(Controls.Find("pnlL_U"    + (i + 1)        , true)[0])).Visible = true;
                    }
                }
            }
            else        // 3 Step Mode : Only Qorvo
            {
                // 2022.07.25 추후 삭제 예정
                //pnlL_U4.Visible = false;
                //pnlL_U5.Visible = false;
                //pnlL_U6.Visible = false;
                //pnlL_U7.Visible = false;
                //pnlL_U8.Visible = false;
                //pnlL_U9.Visible = false;
                //pnlL_U10.Visible = false;
                //pnlL_U11.Visible = false;
                //pnlL_U12.Visible = false;

                //lblL_Step4_T.Visible = false;
                //lblL_Step5_T.Visible = false;
                //lblL_Step6_T.Visible = false;
                //lblL_Step7_T.Visible = false;
                //lblL_Step8_T.Visible = false;
                //lblL_Step9_T.Visible = false;
                //lblL_Step10_T.Visible = false;
                //lblL_Step11_T.Visible = false;
                //lblL_Step12_T.Visible = false;

                //ckbP_StLR4Use.Visible = false;
                //ckbP_StLR5Use.Visible = false;
                //ckbP_StLR6Use.Visible = false;
                //ckbP_StLR7Use.Visible = false;
                //ckbP_StLR8Use.Visible = false;
                //ckbP_StLR9Use.Visible = false;
                //ckbP_StLR10Use.Visible = false;
                //ckbP_StLR11Use.Visible = false;
                //ckbP_StLR12Use.Visible = false;
                // 2022.07.25 End

                for (int i = 0; i < (int)EStepCnt.DEFAULT_STEP; i++)
                {
                    if (i < iStepCnt)
                    {
                        ((CheckBox  )(Controls.Find("ckbP_StLR" + (i + 1) + "Use", true)[0])).Visible = true;
                        ((Label     )(Controls.Find("lblL_Step" + (i + 1) + "_T" , true)[0])).Visible = true;
                        ((Panel     )(Controls.Find("pnlL_U"    + (i + 1)        , true)[0])).Visible = true;
                    }
                    else
                    {
                        ((CheckBox  )(Controls.Find("ckbP_StLR" + (i + 1) + "Use", true)[0])).Visible = false;
                        ((Label     )(Controls.Find("lblL_Step" + (i + 1) + "_T" , true)[0])).Visible = false;
                        ((Panel     )(Controls.Find("pnlL_U"    + (i + 1)        , true)[0])).Visible = false;
                    }
                }
            }
            // 2022.07.25 SungTae End
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
            cmbP_StLMod    .SelectedIndex       = (int)CData.Dev.aData[m_iWy].eGrdMod;
            cmbP_StartLMode.SelectedIndex       = (int)CData.Dev.aData[m_iWy].eStartMode;   //190502 ksg :
            cmbP_StLBaseOnThick.SelectedIndex   = (int)CData.Dev.aData[m_iWy].eBaseOnThick; // 2021.07.27 lhs (SCK VOC)

            if (cmbP_StLMod.SelectedIndex == 1) // TopDown
            {
                cmbP_StartLMode.Visible = true;
                lbl_StartLMode .Visible = true;
                // 2021.07.27 lhs Start : Mold 및 Total 기준으로 그라인딩, 0=Mold, 1=Total (SCK VOC)
                cmbP_StLBaseOnThick.SelectedIndex = (int)EBaseOnThick.Total;
                cmbP_StLBaseOnThick.Enabled = false;
                // 2021.07.27 lhs End
            }
            else                                // Target
            {
                cmbP_StartLMode.Visible = false;
                lbl_StartLMode .Visible = false;
                cmbP_StLBaseOnThick.Enabled = true; // 2021.07.27 lhs  (SCK VOC)
            }

            txtP_StLAir.Text = CData.Dev.aData[m_iWy].dAir.ToString();


            //----------------------
            // 2022.09.26 lhs Start : Rough 01에만 적용되는 옵션 표시
            //bool bR1Option = (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK);  // 표시할 Site (SCK)
            gpR1Option.Visible  = CDataOption.UseDevR1Option;    // GroupBox 표시

            if (CDataOption.UseDevR1Option)  // R1 Option 적용시
            {
                ckbAppCylDepOnFirst.Checked = CData.Dev.aData[m_iWy].bAppCylDepOnFirst;    // 첫번째 Cycle Depth 적용     
                ckbAppLargeCylDep.Checked   = CData.Dev.aData[m_iWy].bAppLargeCylDep;      // 대량의 Depth Grinding 
                if (CData.Dev.aData[m_iWy].bAppLargeCylDep)
                {
                    ckbAppCylDepOnFirst.Enabled = false;
                    txtP_StLAir.Enabled         = false;
                    txtP_StLR1Cyl.ExMax         = 0.150D;
                    txtP_StLR1Tbl.ExMax         = 10D;
                }
                else
                {
                    ckbAppCylDepOnFirst.Enabled = true;
                    txtP_StLAir.Enabled         = true;
                    txtP_StLR1Cyl.ExMax         = 0.015D;  // 기존
                    txtP_StLR1Tbl.ExMax         = 200D;
                }
            }
            // 2022.09.26 lhs End 
            //----------------------

            // 2022.07.25 SungTae Start : [수정] (ASE-KR 개발건) 설정된 Step 수량을 제외한 나머지 Step UI 비활성화
            if (CData.CurCompany == ECompany.ASE_KR)
            {
                CData.Dev.aData[m_iWy].nFixStepCnt = GetFixedStepCount();

                txtP_StLFixStepCnt.Text = CData.Dev.aData[m_iWy].nFixStepCnt.ToString();
            }
            // 2022.07.25 SungTae End

            // 2020.09.08 SungTae
            if (CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)  { m_iStepMaxCnt = GV.StepMaxCnt;    }
            else                                                    { m_iStepMaxCnt = GV.StepMaxCnt_3;  }

            for (int i = 0; i < m_iStepMaxCnt - 1; i++)     // 2020.09.08 SungTae : Modify
            {
                ((CheckBox)(Controls.Find("ckbP_StLR" +(i+1)+"Use"  , true)[0])).Checked       =      CData.Dev.aData[m_iWy].aSteps[i].bUse                ;
                ((ComboBox)(Controls.Find("cmbP_StLR" +(i+1)+"Mod"  , true)[0])).SelectedIndex = (int)CData.Dev.aData[m_iWy].aSteps[i].eMode               ;
                ((TextBox )(Controls.Find("txtP_StLR" +(i+1)+"Total", true)[0])).Text          =      CData.Dev.aData[m_iWy].aSteps[i].dTotalDep.ToString();
                ((TextBox )(Controls.Find("txtP_StLR" +(i+1)+"Cyl"  , true)[0])).Text          =      CData.Dev.aData[m_iWy].aSteps[i].dCycleDep.ToString();
                ((TextBox )(Controls.Find("txtP_StLR" +(i+1)+"Tbl"  , true)[0])).Text          =      CData.Dev.aData[m_iWy].aSteps[i].dTblSpd  .ToString();
                ((TextBox )(Controls.Find("txtP_StLR" +(i+1)+"Spl"  , true)[0])).Text          =      CData.Dev.aData[m_iWy].aSteps[i].iSplSpd  .ToString();
                ((ComboBox)(Controls.Find("cmbP_StLR" +(i+1)+"Dir"  , true)[0])).SelectedIndex = (int)CData.Dev.aData[m_iWy].aSteps[i].eDir                ;
                ((CheckBox)(Controls.Find("ckb_ReSkip"+(i+1)        , true)[0])).Checked       =      CData.Dev.aData[m_iWy].aSteps[i].bReSkip             ;
                ((TextBox )(Controls.Find("txt_ReJud" +(i+1)        , true)[0])).Text          =      CData.Dev.aData[m_iWy].aSteps[i].dReJud   .ToString();
                //201023 JSKim : Over Grinding Correction - Grinding Count Correction 기능
                ((CheckBox)(Controls.Find("ckb_OverGrdCorrect"+(i+1), true)[0])).Checked       =      CData.Dev.aData[m_iWy].aSteps[i].bOverGrdCorrectionUse;
                //201125 jhc : Grinding Step별 Correct 기능
                ((TextBox)(Controls.Find("txtP_StLR" +(i+1)+"Correct", true)[0])).Text         =      CData.Dev.aData[m_iWy].aSteps[i].dCorrectDepth.ToString();
                //
            }

            ckbP_StLFiUse  .Checked       =      CData.Dev.aData[m_iWy].aSteps[m_iStepMaxCnt - 1].bUse;
            cmbP_StLFiMod  .SelectedIndex = (int)CData.Dev.aData[m_iWy].aSteps[m_iStepMaxCnt - 1].eMode;
            txtP_StLFiTotal.Text          =      CData.Dev.aData[m_iWy].aSteps[m_iStepMaxCnt - 1].dTotalDep.ToString();
            txtP_StLFiCyl  .Text          =      CData.Dev.aData[m_iWy].aSteps[m_iStepMaxCnt - 1].dCycleDep.ToString();
            txtP_StLFiTbl  .Text          =      CData.Dev.aData[m_iWy].aSteps[m_iStepMaxCnt - 1].dTblSpd.ToString();
            txtP_StLFiSpl  .Text          =      CData.Dev.aData[m_iWy].aSteps[m_iStepMaxCnt - 1].iSplSpd.ToString();
            cmbP_StLFiDir  .SelectedIndex = (int)CData.Dev.aData[m_iWy].aSteps[m_iStepMaxCnt - 1].eDir;

            cmbP_StLFiStart.SelectedIndex = (int)CData.Dev.aData[m_iWy].eFine;
            if (CData.Dev.aData[m_iWy].eFine == eFineMode.WheelInsp)    { txtP_StLFiVal.Enabled = false;    }
            else                                                        { txtP_StLFiVal.Enabled = true;     }
            txtP_StLFiVal.Text = CData.Dev.aData[m_iWy].dCpen.ToString();

            if (m_eWy == EWay.L && CData.Dev.bDual == eDual.Normal)
            {
                cmbP_StLFiStart_R.SelectedIndex = (int)CData.Dev.aData[1].eFine;
                
                if (CData.Dev.aData[1].eFine == eFineMode.WheelInsp)    { txtP_StLFiVal_R.Enabled = false;  }
                else                                                    { txtP_StLFiVal_R.Enabled = true;   }
                
                txtP_StLFiVal_R.Text = CData.Dev.aData[1].dCpen.ToString();
            }
        }

        public void Get()
        {
            if (m_eWy == EWay.L || (m_eWy == EWay.R && CData.Dev.bDual == eDual.Dual))
            {
                // Left or Dual right
                CData.Dev.aData[m_iWy].eGrdMod      = (eGrdMode    )cmbP_StLMod.SelectedIndex;
                CDev.It.aNowGrd[m_iWy]              = CData.Dev.aData[m_iWy].eGrdMod.ToString();        //211001 pjh :
                CData.Dev.aData[m_iWy].eStartMode   = (eTDStartMode)cmbP_StartLMode.SelectedIndex;      //190502 ksg :
                CData.Dev.aData[m_iWy].eBaseOnThick = (EBaseOnThick)cmbP_StLBaseOnThick.SelectedIndex;  // 2021.07.27 lhs (SCK VOC)
                double.TryParse(txtP_StLAir.Text,   out CData.Dev.aData[m_iWy].dAir);

                // 2022.09.23 lhs Start : Rough1, 첫번째 Griding시에 Z축 StartPos에 cycle depth 적용 
                if (CDataOption.UseDevR1Option)
                {
                    CData.Dev.aData[m_iWy].bAppCylDepOnFirst = ckbAppCylDepOnFirst.Checked;
                    CData.Dev.aData[m_iWy].bAppLargeCylDep   = ckbAppLargeCylDep.Checked;
                    if (CData.Dev.aData[m_iWy].bAppLargeCylDep)
                    {
                        ckbAppCylDepOnFirst.Checked = true;
                        ckbAppCylDepOnFirst.Enabled = false;
                        CData.Dev.aData[m_iWy].bAppCylDepOnFirst = true;

                        CData.Dev.aData[m_iWy].dAir = 0.0;
                        txtP_StLR1Cyl.ExMax = 0.150D;
                        txtP_StLR1Tbl.ExMax = 10D;
                    }
                    else
                    {
                        txtP_StLR1Cyl.ExMax = 0.015D;  // 기존
                        txtP_StLR1Tbl.ExMax = 200D;
                    }

                    double dR1_Cyl;
                    double dR1_TblSpd;
                    double.TryParse(txtP_StLR1Cyl.Text, out dR1_Cyl);
                    double.TryParse(txtP_StLR1Tbl.Text, out dR1_TblSpd);

                    if (dR1_Cyl    > txtP_StLR1Cyl.ExMax) txtP_StLR1Cyl.Text = txtP_StLR1Cyl.ExMax.ToString();
                    if (dR1_TblSpd > txtP_StLR1Tbl.ExMax) txtP_StLR1Tbl.Text = txtP_StLR1Tbl.ExMax.ToString();
                }
                // 2022.09.23 lhs End 

                // 2022.07.25 SungTae Start : [수정] (ASE-KR 개발건) 설정된 Step 수량을 제외한 나머지 Step UI 비활성화
                if (CData.CurCompany == ECompany.ASE_KR)
                {
                    int.TryParse(txtP_StLFixStepCnt.Text, out CData.Dev.aData[m_iWy].nFixStepCnt);
                }
                // 2022.07.25 SungTae End

                // 2020.09.08 SungTae
                if (CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)  { m_iStepMaxCnt = GV.StepMaxCnt;    }
                else                                                    { m_iStepMaxCnt = GV.StepMaxCnt_3;  }

                for (int i = 0; i < m_iStepMaxCnt - 1; i++)     // 2020.09.08 SungTae : Modify
                {
                    CData.Dev.aData[m_iWy].aSteps[i].bUse      =                  ((CheckBox)(this.Controls.Find("ckbP_StLR" +(i+1)+"Use"  , true)[0])).Checked       ;
                    CData.Dev.aData[m_iWy].aSteps[i].eMode     = (eStepMode)      ((ComboBox)(this.Controls.Find("cmbP_StLR" +(i+1)+"Mod"  , true)[0])).SelectedIndex ;

                    double.TryParse(((TextBox )(this.Controls.Find("txtP_StLR" +(i+1)+"Total", true)[0])).Text, out CData.Dev.aData[m_iWy].aSteps[i].dTotalDep);
                    double.TryParse(((TextBox )(this.Controls.Find("txtP_StLR" +(i+1)+"Cyl"  , true)[0])).Text, out CData.Dev.aData[m_iWy].aSteps[i].dCycleDep);
                    double.TryParse(((TextBox )(this.Controls.Find("txtP_StLR" +(i+1)+"Tbl"  , true)[0])).Text, out CData.Dev.aData[m_iWy].aSteps[i].dTblSpd);
                    int.TryParse(   ((TextBox )(this.Controls.Find("txtP_StLR" +(i+1)+"Spl"  , true)[0])).Text, out CData.Dev.aData[m_iWy].aSteps[i].iSplSpd);
                    CData.Dev.aData[m_iWy].aSteps[i].eDir      = (eStartDir)((ComboBox)(this.Controls.Find("cmbP_StLR" +(i+1)+"Dir"  , true)[0])).SelectedIndex;
                    CData.Dev.aData[m_iWy].aSteps[i].bReSkip   =            ((CheckBox)(this.Controls.Find("ckb_ReSkip"+(i+1)        , true)[0])).Checked;
                    double.TryParse(((TextBox )(this.Controls.Find("txt_ReJud" +(i+1)        , true)[0])).Text, out CData.Dev.aData[m_iWy].aSteps[i].dReJud);
                    //201023 JSKim : Over Grinding Correction - Grinding Count Correction 기능
                    CData.Dev.aData[m_iWy].aSteps[i].bOverGrdCorrectionUse = ((CheckBox)(this.Controls.Find("ckb_OverGrdCorrect"+(i+1), true)[0])).Checked;
                    //201125 jhc : Grinding Step별 Correct 기능
                    double.TryParse(((TextBox)(this.Controls.Find("txtP_StLR" +(i+1)+"Correct", true)[0])).Text, out CData.Dev.aData[m_iWy].aSteps[i].dCorrectDepth);
                    CData.Dev.aData[m_iWy].aSteps[i].dCorrectDepth = GU.Truncate(CData.Dev.aData[m_iWy].aSteps[i].dCorrectDepth,4);
                    //
                }

                // Fine
                CData.Dev.aData[m_iWy].aSteps[m_iStepMaxCnt - 1].bUse      =            ckbP_StLFiUse  .Checked       ;
                CData.Dev.aData[m_iWy].aSteps[m_iStepMaxCnt - 1].eMode     = (eStepMode)cmbP_StLFiMod  .SelectedIndex ;
                double.TryParse(txtP_StLFiTotal.Text, out CData.Dev.aData[m_iWy].aSteps[m_iStepMaxCnt - 1].dTotalDep);
                double.TryParse(txtP_StLFiCyl  .Text, out CData.Dev.aData[m_iWy].aSteps[m_iStepMaxCnt - 1].dCycleDep);
                double.TryParse(txtP_StLFiTbl  .Text, out CData.Dev.aData[m_iWy].aSteps[m_iStepMaxCnt - 1].dTblSpd);
                int.TryParse(   txtP_StLFiSpl  .Text, out CData.Dev.aData[m_iWy].aSteps[m_iStepMaxCnt - 1].iSplSpd);
                CData.Dev.aData[m_iWy].aSteps[m_iStepMaxCnt - 1].eDir      = (eStartDir)cmbP_StLFiDir  .SelectedIndex ;
                
                CData.Dev.aData[m_iWy].eFine = (eFineMode)cmbP_StLFiStart.SelectedIndex;
                double.TryParse(txtP_StLFiVal.Text, out CData.Dev.aData[m_iWy].dCpen);

            } // end : if (m_eWy == EWay.L || (m_eWy == EWay.R && CData.Dev.bDual == eDual.Dual))
            else
            {
                // Normal mode
                // Left parameter -> Right parameter copy
                CData.Dev.aData[1].eGrdMod          = CData.Dev.aData[0].eGrdMod;
                CDev.It.aNowGrd[1]                  = CData.Dev.aData[1].eGrdMod.ToString(); //211001 pjh :
                CData.Dev.aData[1].eStartMode       = CData.Dev.aData[0].eStartMode;
                CData.Dev.aData[1].eBaseOnThick     = CData.Dev.aData[0].eBaseOnThick;   // 2021.07.27 lhs (SCK VOC)
                CData.Dev.aData[1].dAir             = CData.Dev.aData[0].dAir;
                // 2022.09.23 lhs Start : Rough1, 첫번째 Griding시에 Z축 StartPos에 cycle depth 적용 
                if (CDataOption.UseDevR1Option)
                {
                    CData.Dev.aData[1].bAppCylDepOnFirst = CData.Dev.aData[0].bAppCylDepOnFirst;
                    CData.Dev.aData[1].bAppLargeCylDep   = CData.Dev.aData[0].bAppLargeCylDep;

                    if (CData.Dev.aData[1].bAppLargeCylDep)
                    {
                        //ckbAppCylDepOnFirst.Checked = true;
                        //ckbAppCylDepOnFirst.Enabled = false;
                        //txtP_StLR1Cyl.ExMax = 0.150D;
                        //txtP_StLR1Tbl.ExMax = 10D;
                    }
                    else
                    {
                        //txtP_StLR1Cyl.ExMax = 0.015D;  // 기존
                        //txtP_StLR1Tbl.ExMax = 200D;
                    }
                }
                // 2022.09.23 lhs End 

                // 2022.07.25 SungTae Start : [수정] (ASE-KR 개발건) 설정된 Step 수량을 제외한 나머지 Step UI 비활성화
                if (CData.CurCompany == ECompany.ASE_KR)
                {
                    CData.Dev.aData[1].nFixStepCnt = CData.Dev.aData[0].nFixStepCnt;
                }
                // 2022.07.25 SungTae End

                //for (int i = 0; i < GV.StepMaxCnt; i++)
                for (int i = 0; i < m_iStepMaxCnt; i++)     // 2020.09.08 SungTae : Modify
                {
                    CData.Dev.aData[1].aSteps[i].bUse      = CData.Dev.aData[0].aSteps[i].bUse     ;
                    CData.Dev.aData[1].aSteps[i].eMode     = CData.Dev.aData[0].aSteps[i].eMode    ;
                    CData.Dev.aData[1].aSteps[i].dTotalDep = CData.Dev.aData[0].aSteps[i].dTotalDep;
                    CData.Dev.aData[1].aSteps[i].dCycleDep = CData.Dev.aData[0].aSteps[i].dCycleDep;
                    CData.Dev.aData[1].aSteps[i].dTblSpd   = CData.Dev.aData[0].aSteps[i].dTblSpd  ;
                    CData.Dev.aData[1].aSteps[i].iSplSpd   = CData.Dev.aData[0].aSteps[i].iSplSpd  ;
                    CData.Dev.aData[1].aSteps[i].eDir      = CData.Dev.aData[0].aSteps[i].eDir     ;
                    CData.Dev.aData[1].aSteps[i].bReSkip   = CData.Dev.aData[0].aSteps[i].bReSkip  ;
                    CData.Dev.aData[1].aSteps[i].dReJud    = CData.Dev.aData[0].aSteps[i].dReJud   ;
                    //201023 JSKim : Over Grinding Correction - Grinding Count Correction 기능
                    CData.Dev.aData[1].aSteps[i].bOverGrdCorrectionUse = CData.Dev.aData[0].aSteps[i].bOverGrdCorrectionUse;
                    //201125 jhc : Grinding Step별 Correct 기능
                    CData.Dev.aData[1].aSteps[i].dCorrectDepth = CData.Dev.aData[0].aSteps[i].dCorrectDepth;
                    //
                }
            }

            if (m_eWy == EWay.L && CData.Dev.bDual == eDual.Normal)
            {
                CData.Dev.aData[1].eFine = (eFineMode)cmbP_StLFiStart_R.SelectedIndex;
                double.TryParse(txtP_StLFiVal_R.Text, out CData.Dev.aData[1].dCpen);
            }
        }
        #endregion

        private void cmbP_StLMod_SelectedIndexChanged(object sender, EventArgs e)
        {
            string text;
            if      (cmbP_StLMod.SelectedIndex == 0)    //Target
            {
                //lbl_TotalDepth.Text = "Target\r\n[mm]";
                text = CLang.It.GetLanguage("Target") + "\r\n[mm]";
                SetWriteLanguage(lbl_TotalDepth.Name, text);
                cmbP_StartLMode.Visible = false; //190502 ksg :
                lbl_StartLMode.Visible = false; //190502 ksg :

                // 2021.07.27 lhs Start
                cmbP_StLBaseOnThick.SelectedIndex = 0; // Mold
                cmbP_StLBaseOnThick.Enabled = true;
                // 2021.07.27 lhs End
            }
            else if (cmbP_StLMod.SelectedIndex == 1)    //TopDown
            {
                //lbl_TotalDepth.Text = "Total Depth\r\n[mm]";
                text = CLang.It.GetLanguage("Total") + " " + CLang.It.GetLanguage("Depth") + "\r\n[mm]";
                SetWriteLanguage(lbl_TotalDepth.Name, text);

                cmbP_StartLMode.Visible = true; //190502 ksg :
                lbl_StartLMode.Visible = true; //190502 ksg :

                // 2021.07.27 lhs Start
                cmbP_StLBaseOnThick.SelectedIndex = 1; // Total
                cmbP_StLBaseOnThick.Enabled = false;
                // 2021.07.27 lhs End
            }

            txtP_StLR1Total.Text = "0";
            txtP_StLR2Total.Text = "0";
            txtP_StLR3Total.Text = "0";
            txtP_StLFiTotal.Text = "0";
        }

        //201023 JSKim : Over Grinding Correction - Grinding Count Correction 기능
        private void ckb_OverGrdCorrectX_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox mChkBox = sender as CheckBox;

            bool bStartStep = true; //UI에서 건드린 스텝이 (기존 첫 스텝 또는 새로 첫 스텝이 되는 경우)에 대한 플래그
            int nSelectStepNo = Convert.ToInt32(mChkBox.Tag);

            if (((CheckBox)(Controls.Find("ckbP_StLR" + nSelectStepNo + "Use", true)[0])).Checked == false)
            {
                mChkBox.Checked = false;
                return;
            }

            //(해당 스텝 사용) + (해당 스텝에서 Over Grinding 보정 선택) ==> But, 해당 스텝이 그라인딩할 첫 스텝인 경우 Over Grinding 보정할 여지가 없으므로 체크 안 되도록 함
            for (int i = 1; i < nSelectStepNo; i++)
            {
                if (((CheckBox)(Controls.Find("ckbP_StLR" + i + "Use", true)[0])).Checked == true)
                {
                    bStartStep = false; //현재 스텝보다 이전 스텝에서 그라인딩 진행 스텝이 있음!
                    break;
                }
            }

            if (bStartStep)
            {
                mChkBox.Checked = false; //첫 스텝인 경우 보정 적용을 할 수 없음!!! (선택했든 해제했든 무조건 해제로 재설정)
            }
        }

        private void ckbP_StLRXUse_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox mChkBox = sender as CheckBox;

            int nSelectStepNo = Convert.ToInt32(mChkBox.Tag);

            _Update_Ckb_OverGrdCorrectX_By_CkbP_StLRXUse( nSelectStepNo );
        }

        private void _Update_Ckb_OverGrdCorrectX_By_CkbP_StLRXUse(int stepNum)
        {
            bool bStartStep = true; //UI에서 건드린 스텝이 (기존 첫 스텝 또는 새로 첫 스텝이 되는 경우)에 대한 플래그
            int nFirstUseStepNo = 0;
            int i = 0;

            for (i = 1; i < stepNum; i++)
            {
                if (((CheckBox)(Controls.Find("ckbP_StLR" + i + "Use", true)[0])).Checked == true)
                {
                    bStartStep = false; //현재 스텝보다 이전 스텝에서 그라인딩 진행 스텝이 있음!
                    break;
                }
            }

            //현재 건드린 스텝 다음 단계의 스텝들에 대한 Over Grinding Correction 옵션 표시/감춤 재설정 
            for (i = stepNum; i <= CDataOption.StepCnt; i++)
            {
                if (((CheckBox)(Controls.Find("ckbP_StLR" + i + "Use", true)[0])).Checked == true)
                {
                    if (nFirstUseStepNo == 0) nFirstUseStepNo = i; //사용 설정한 경우 현재 스텝 번호

                    if (CData.Opt.bOverGrdCountCorrectionUse)
                    {
                        ((CheckBox)(Controls.Find("ckb_OverGrdCorrect" + i, true)[0])).Visible = true; //현재 스텝 사용하면 Over Grinding Correction 옵션 표시
                    }
                }
                else
                {
                    //해당 스텝 비사용 => Over Grinding Correction 불가 => Over Grinding Correction 옵션 체크 해제, 옵션 감춤
                    ((CheckBox)(Controls.Find("ckb_OverGrdCorrect" + i, true)[0])).Checked = false;
                    ((CheckBox)(Controls.Find("ckb_OverGrdCorrect" + i, true)[0])).Visible = false;
                }
            }

            ///UI에서 건드린 스텝이 (기존 첫 스텝 또는 새로 첫 스텝이 되는 경우)에 대한 처리
            if (bStartStep)
            {
                for (i = 1; i <= CDataOption.StepCnt; i++)
                {
                    if (nFirstUseStepNo == 0 || nFirstUseStepNo == i)
                    {
                        //(nLastUseStepNo == 0) : 건드린 스텝 이 후의 선택된 스텝이 없고, 사용하던 첫 스텝마저 선택 해제한 경우
                        //(nLastUseStepNo == i) : 현재 설정 검토 스텝이 첫 스텝인 경우
                        ((CheckBox)(Controls.Find("ckb_OverGrdCorrect" + i, true)[0])).Checked = false;
                        ((CheckBox)(Controls.Find("ckb_OverGrdCorrect" + i, true)[0])).Visible = false;
                    }
                    else if (i < nFirstUseStepNo)
                    {
                        //현재 설정 검토 스텝이 사용되는 첫 스텝 이 전의 스텝인 경우
                        ((CheckBox)(Controls.Find("ckb_OverGrdCorrect" + i, true)[0])).Checked = false; //더블 체크
                        ((CheckBox)(Controls.Find("ckb_OverGrdCorrect" + i, true)[0])).Visible = false;
                    }
                    else if (((CheckBox)(Controls.Find("ckbP_StLR" + i + "Use", true)[0])).Checked == true)
                    {
                        //현재 설정 검토 스텝이 사용되는 첫 스텝 이 후의 체크된 스텝인 경우
                        if (CData.Opt.bOverGrdCountCorrectionUse)
                        {
                            ((CheckBox)(Controls.Find("ckb_OverGrdCorrect" + i, true)[0])).Visible = true;
                        }
                    }
                }
            }
        }

        // 2022.07.25 SungTae Start : [수정] (ASE-KR 개발건) 설정된 Step 수량을 제외한 나머지 Step UI 비활성화
        private void btn_LFixStepCnt_Click(object sender, EventArgs e)
        {
            int.TryParse(txtP_StLFixStepCnt.Text, out CData.Dev.aData[m_iWy].nFixStepCnt);
            
            ViewUpdate(CDataOption.StepCnt);
        }

        private int GetFixedStepCount()
        {
            int nCnt = 0;

            for (int i = 0; i < CDataOption.StepCnt; i++)
            {
                if(CData.Dev.aData[m_iWy].aSteps[i].bUse)
                {
                    nCnt++;
                }
            }

            return nCnt;
        }
		// 2022.07.25 SungTae End

		private void ckbAppLargeCylDep_Click(object sender, EventArgs e)    //2022.09.26 lhs
		{
            CheckBox mCkb = sender as CheckBox;

            if (mCkb.Checked)
            {
                //CData.Dev.aData[m_iWy].bAppLargeCylDep = true; Save 클릭시 적용

                ckbAppCylDepOnFirst.Checked = true;   // 맨처음 Cycle Depth 적용 : O
                ckbAppCylDepOnFirst.Enabled = false;

                txtP_StLAir.Text            = "0.0";  // Air Cut : X
                txtP_StLAir.Enabled         = false;

                // One 포인트 측정 안하고 리턴하여 리그라인딩 안함 (Cyl_Grd()에서)

                txtP_StLR1Cyl.ExMax = 0.150D;
                txtP_StLR1Tbl.ExMax = 10D;

                CMsg.Show(eMsg.Notice, "Notice", $"Apply Cycle Depth On First Count : O\r\n" +
                                                 $"Air Cut : X \r\n" +
                                                 $"ReGrinding : X \r\n" +
                                                 $"R1 Cycle Depth Max = {txtP_StLR1Cyl.ExMax} mm\r\n" +
                                                 $"R1 Table Speed Max = {txtP_StLR1Tbl.ExMax} mm/s\r\n");
            }
            else
            {
                ckbAppCylDepOnFirst.Checked = CData.Dev.aData[m_iWy].bAppCylDepOnFirst;    // 첫번째 Cycle Depth 적용     
                ckbAppCylDepOnFirst.Enabled = true;

                txtP_StLAir.Text    = CData.Dev.aData[m_iWy].dAir.ToString();  // Air Cut
                txtP_StLAir.Enabled = true;

                txtP_StLR1Cyl.ExMax = 0.015D;
                txtP_StLR1Tbl.ExMax = 200D;
            }

            double dR1_Cyl;
            double dR1_TblSpd;
            double.TryParse(txtP_StLR1Cyl.Text, out dR1_Cyl);
            double.TryParse(txtP_StLR1Tbl.Text, out dR1_TblSpd);

            if (dR1_Cyl    > txtP_StLR1Cyl.ExMax) txtP_StLR1Cyl.Text = txtP_StLR1Cyl.ExMax.ToString();
            if (dR1_TblSpd > txtP_StLR1Tbl.ExMax) txtP_StLR1Tbl.Text = txtP_StLR1Tbl.ExMax.ToString();
        }
	}
}
