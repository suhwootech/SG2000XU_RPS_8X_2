using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace SG2000X
{
    public partial class vwOpt_01Gen : UserControl
    {
        public vwOpt_01Gen()
        {
            if      ((int)ELang.English == CData.Opt.iSelLan)   {   System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");     }
            else if ((int)ELang.Korea   == CData.Opt.iSelLan)   {   System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ko-KR");     }
            else if ((int)ELang.China   == CData.Opt.iSelLan)   {   System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-Hans");   }

            InitializeComponent();

            if (CData.CurCompany == ECompany.Qorvo      || CData.CurCompany == ECompany.Qorvo_DZ ||
                CData.CurCompany == ECompany.Qorvo_RT   || CData.CurCompany == ECompany.Qorvo_NC)      // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
            {
                lbl_WarmT_T.Visible = false;
                txt_WarmT.Visible = false;
                lbl_WmTimeHelp.Visible = false;
                lbl_IdleT_U.Visible = false;
                // 200820 jym : Network drive 설정 
                //210817 pjh : Net Drive 설정 License로 관리
                if (CDataOption.UseNetDrive && !CDataOption.UseDFNet)
                {
                    pnl_Net.Visible = true;
                    ckb_DFNetUse.Visible = false;
                }
                else if (!CDataOption.UseNetDrive && CDataOption.UseDFNet)
                {
                    pnl_Net.Visible = true;
                    ckb_NetUse.Visible = false;
                    ckb_DFNetUse.Visible = true;
                }
                else if (CDataOption.UseNetDrive && CDataOption.UseDFNet)
                {
                    pnl_Net.Visible = true;
                    ckb_DFNetUse.Visible = true;
                }
                else
                {
                    pnl_Net.Visible = false;
                    ckb_DFNetUse.Visible = false;
                }
                //

                // 2021.05.27 SungTae Start : [추가] Qorvo향 관련 미사용 처리
                lb_WorkTime_T.Visible = false;
                lb_CalPeriod_T.Visible = false;
                lb_RenewalTime_T.Visible = false;

                txt_WorkTime.Visible = false;
                cmb_CalPeriod.Visible = false;
                cmb_RenewalTime.Visible = false;

                lb_WorkTime_U.Visible = false;
                lb_RenewalTime_U.Visible = false;
                // 2021.05.27 SungTae End
            }
            else
            {
                lbl_WarmT_T.Visible     = true;
                txt_WarmT.Visible       = true;
                lbl_IdleT_U.Visible     = true;
                lbl_WmTimeHelp.Visible  = true;

                // 2022.06.09 lhs Start : Warm Up 간격 표시
                bool bVisiblePd = (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK);
                lbl_WarmPeriod_T.Visible    = bVisiblePd;
                txt_WarmPeriod.Visible      = bVisiblePd;
                lbl_WarmPeriod_U.Visible    = bVisiblePd;
                lbl_WarmPeriod_L.Visible    = bVisiblePd;

                ckb_WarmUpWithStrip.Visible = bVisiblePd;    // 2022.06.10 lhs : (SCK+)자재가 있어도 워밍업
                // 2022.06.09 lhs End

                // 200820 jym : Network drive 설정 
                //210817 pjh : Net Drive 설정 License로 관리
                if (CDataOption.UseNetDrive && !CDataOption.UseDFNet)
				{
					pnl_Net.Visible = true;
                    ckb_DFNetUse.Visible = false;
                }
                else if (!CDataOption.UseNetDrive && CDataOption.UseDFNet)
                {
                    pnl_Net.Visible      = true;
                    ckb_NetUse.Visible   = false;
                    ckb_DFNetUse.Visible = true;
                }
                else if (CDataOption.UseNetDrive && CDataOption.UseDFNet)
                {
                    pnl_Net.Visible = true;
                    ckb_DFNetUse.Visible = true;
                }
                else
                {
                    pnl_Net.Visible = false;
                    ckb_DFNetUse.Visible = false;
                }
                //

                // 2021.05.27 SungTae Start : [추가] Qorvo향 관련 미사용 처리
                lb_WorkTime_T.Visible    = true;
                lb_CalPeriod_T.Visible   = true;
                lb_RenewalTime_T.Visible = true;

                txt_WorkTime.Visible     = true;
                cmb_CalPeriod.Visible    = true;
                cmb_RenewalTime.Visible  = true;

                lb_WorkTime_U.Visible    = true;
                lb_RenewalTime_U.Visible = true;
                // 2021.05.27 SungTae End
            }

            // 2021-07-01, jhLee, QC-Vision을 Skyworks 와 Qorvo에서 사용하게됨에따라 라이센스에서 QC 사용 유무로 세부 메뉴 시각화를 결정한다.
            if (CDataOption.UseQC)
            {
                ckb_QcUse.Visible = true;
                ckb_QcByPass.Visible = true;
                ckb_QcDoor.Visible = true;
            }
            else
            {
                ckb_QcUse.Visible = false;
                ckb_QcByPass.Visible = false;
                ckb_QcDoor.Visible = false;
            }

            if (CData.CurCompany == ECompany.SkyWorks)
            {
                ckb_DoorSkip.Enabled = false;
                ckb_DiChiler.Visible = true;
                // 201023 jym : Auto warm up parameter add
                bool bVal = (CDataOption.AutoWarmUp == eAutoWarmUp.Use) ? true : false;
                lbl_AWT_T.Visible = bVal;
                lbl_AWT_U.Visible = bVal;
                lbl_AWT_L.Visible = bVal;
                txt_AWT.Visible   = bVal;

                lbl_AWP_T.Visible = bVal;
                lbl_AWP_U.Visible = bVal;
                lbl_AWP_L.Visible = bVal;
                txt_AWP.Visible   = bVal;
                // 201029 jym : 추가
                ckb_AtwSkip.Visible = bVal;
            }
            else
            {
                ckb_DryMeaSkip.Visible = false;
                ckb_DoorSkip.Enabled   = true;
                ckb_DiChiler.Visible   = false;
                ckb_AtwSkip.Visible    = (CDataOption.AutoWarmUp == eAutoWarmUp.Use) ? true : false; //201130 불필요 항목 감추기
            }

            if (CData.CurCompany != ECompany.ASE_KR)
            {
                ckb_WarmUpSkip.Visible = false;
            }

            if (CData.CurCompany == ECompany.SCK  ||
                CData.CurCompany == ECompany.JSCK ||
                CData.CurCompany == ECompany.JCET) //200121 ksg : , 200625 lks
            {
                lbl_WmTimeHelp.Visible = false;
                lbl_MgzMaxHelp.Visible = false;
            }

            pnl_Secsgem.Visible = (CDataOption.SecsUse == eSecsGem.Use) ? true : false;

            // 200901 jym : 조건 변경
            ckb_LeakSkip.Visible = (CDataOption.ChkGrdLeak == eChkGrdLeak.Use) ? true : false;

            // 2022.06.07 SungTae Start : [수정] ASE-KH도 SPIL과 동일하게 하기 위해 사이트 옵션 추가
            // 2020-10-19, jhLee, SPIL인 경우 RCMD Start 명령에 대한 옵션 표시
            //if (CData.CurCompany == ECompany.SPIL)
            if (CData.CurCompany == ECompany.SPIL || CData.CurCompany == ECompany.ASE_K12)
            // 2022.06.07 SungTae End
            {
                lbl_RCMDTimeout1.Visible = true;
                lbl_RCMDTimeout2.Visible = true;
                lbl_RCMDTimeout3.Visible = true;
                txt_RCMDStartTimeout.Visible = true;
            }
            else
            {
                lbl_RCMDTimeout1.Visible = false;
                lbl_RCMDTimeout2.Visible = false;
                lbl_RCMDTimeout3.Visible = false;
                txt_RCMDStartTimeout.Visible = false;
            }

            // 2020-11-18, jhLee : Skyworks Only
            // 201120 jym : 개발 완료 시 까지 사용안함
            //if (CData.CurCompany != ECompany.SkyWorks)
            //syc : ase kr loading stop - Loader 에서 Gen으로 이동

            if (CDataOption.UseAutoLoadingStop) //syc 추가
            {
                pnl_AutoLDStop.Visible = true;
                //if(CData.CurCompany == ECompany.SkyWorks)
                //{//210309 pjh : Skyworks에서는 대기 위치 선택 안함
                //    label12.Visible = false;
                //    cb_LDStopType.Visible = false;
                //    lbl_Title4.Text = "Set Up Strip";//210309 pjh : Set Up Strip Text 변경
                //    lbl_AutoLDStopCap3.Text = "Min : 1  ~   Max : 5";
                //    CData.Opt.iLoadingStopType = 0;
                //}
            }
            else if(CDataOption.UseSetUpStrip)
            {
                pnl_AutoLDStop.Visible = true;
                label12.Visible = false;
                cb_LDStopType.Visible = false;
                lbl_Title4.Text = "Set Up Strip";//210309 pjh : Set Up Strip Text 변경
                lbl_AutoLDStopCap3.Text = "Min : 1  ~   Max : 5";
                CData.Opt.iLoadingStopType = (int)eTypeLDStop.Inrail/*0*/;    // 2021.10.25 SungTae : [수정] Code 확인 용이하도록 변경 (0 -> (int)eTypeLDStop.Inrail)
            }

            // 2021-05-17, jhLee, 연속LOT 관련 설정 패널을 화면에 보여주는 판단, 라이센스에서 연속LOT을 사용한다고 설정되어야 표시
            pnl_MultiLot.Visible = CDataOption.UseMultiLOT;
            //syc : Qorvo 수정 - MTBF, MTBA
            if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC)
            {
                lb_WorkTime_T.Visible = false;
                txt_WorkTime.Visible = false;
                lb_WorkTime_U.Visible = false;

                lb_CalPeriod_T.Visible = false;
                cmb_CalPeriod.Visible = false;

                lb_RenewalTime_T.Visible = false;
                cmb_RenewalTime.Visible = false;
                lb_RenewalTime_U.Visible = false;

                txt_MtbaSetTime.ReadOnly = true;

                //label172.Visible = false;
                //txt_MtbaSetTime.Visible = false;
                //label174.Visible = false;
            }
        }

        #region Basic method
        /// <summary>
        /// View가 화면에 그려질때 실행해야 하는 함수
        /// </summary>
        public void Open()
        {
            Set();

            _SetDoorSkipEnable(); //201223 jhc : ASE-KH는 MASTER Level만 Door Skip 옵션 변경 가능 (ASE-KH VOC)
        }

        /// <summary>
        /// View가 화면에서 지워질때 실행해야 하는 함수
        /// </summary>
        public void Close()
        {
        }

        public void Set()
        {
            txt_WarmT.Text      = CData.Opt.iWmUT.ToString();
            txt_WarmVel.Text    = CData.Opt.iWmUS.ToString();
            txt_WarmPeriod.Text = CData.Opt.iWmUI.ToString(); //syc : Warm up 유휴 시간

            ckb_WarmUpWithStrip.Checked = CData.Opt.bWarmUpWithStrip;  // 2022.06.10 lhs : (SCK+)자재가 있어도 워밍업

            txt_LotMax.Text = CData.Opt.iLotCnt.ToString();
            //200401 jym : 추가
            ckb_LotTblClean.Checked = CData.Opt.bLotTblClean;
            ckb_LotDrs.Checked = CData.Opt.bLotDrs;

            txt_LvChange.Text = CData.Opt.iChangeLevel.ToString();
            
            ckb_DoorSkip.Checked = CData.Opt.bDoorSkip;

            ckb_DryGrindSkip.Checked = CData.Opt.bDryAuto;
            ckb_DryMeaSkip.Checked = CData.Opt.bDryAutoMeaStripSkip;
            ckb_DryMeaSkip.Enabled = CData.Opt.bDryAuto;
            ckb_LeakSkip.Checked = CData.Opt.bLeakSkip; //190109 ksg :  
            ckb_WarmUpSkip.Checked = CData.Opt.bWarmUpSkip; //190218 ksg :
            ckb_QcUse.Checked = CData.Opt.bQcUse; //190403 ksg : Qc            
            ckb_QcByPass.Checked = CData.Opt.bQcByPass; //190406 ksg : QC
            // 201116 jym START : QC 도어연동 기능 추가
            ckb_QcDoor.Checked = CData.Opt.bQcDoor;
            ckb_QcByPass.Enabled = CData.Opt.bQcUse;
            ckb_QcDoor.Enabled = CData.Opt.bQcUse;
            // 201116 jym END
            ckb_SecsUse.Checked = CData.Opt.bSecsUse;
            if (CData.GemForm != null)
            {
                if (ckb_SecsUse.Checked == false)
                {
                    CData.GemForm.Stop_GEM();
                }
                else
                {
                    CData.GemForm.Start_GEM();
                }
            }
            txt_RCMDStartTimeout.Text = CData.Opt.iRCMDStartTimeout.ToString();     // 2020-10-19, jhLee : RCMD START Timeout 지정

            ckb_DiChiler.Checked = CData.Opt.bDiChillerSkip;

            //MTBA MTBF
            txt_WorkTime.Text = CData.Opt.dDayWorking.ToString();
            cmb_CalPeriod.SelectedIndex = CData.Opt.iPeriod;
            cmb_RenewalTime.SelectedItem = CData.Opt.iRenewal.ToString("00");
            txt_MtbaSetTime.Text = CData.Opt.iSetTime.ToString();
            cmb_Lang.SelectedIndex = CData.Opt.iSelLan; //190516 ksg :

            // 200820 jym : Network drive 설정 
            ckb_NetUse.Checked = CData.Opt.bNetUse;
            txt_NetIP.Text = CData.Opt.sNetIP;
            txt_NetPath.Text = CData.Opt.sNetPath;
            txt_NetID.Text = CData.Opt.sNetID;
            txt_NetPw.Text = CData.Opt.sNetPw;
            //
            //210811 pjh : D/F Server Data Save 기능 Net Drive 추가
            ckb_DFNetUse.Checked = CData.Opt.bDFNetUse;
            //


            // 2020.08.21 JSKim St
            if (CData.Opt.iProbeFontSize == 0)
            {
                cb_ProbeFontSize.SelectedIndex = 2;
            }
            else
            {
                cb_ProbeFontSize.SelectedItem = CData.Opt.iProbeFontSize.ToString();
            }
            if (CData.Opt.iProbeFloationPoint == 0)
            {
                cb_ProbeFloationPoint.SelectedIndex = 0;
            }
            else
            {
                cb_ProbeFloationPoint.SelectedItem = CData.Opt.iProbeFloationPoint.ToString();
            }
            // 2020.08.21 JSKim Ed

            // 2021.04.09 SungTae Start : Wheel History Auto Delete Period
            //201005 pjh : Log Delete Period
            txt_DelPeriod.Text = CData.Opt.iDelPeriod.ToString();
            txt_DelPeriodHistory.Text = CData.Opt.iDelPeriodHistory.ToString();
            // 2021.04.09 SungTae End

            // 201023 jym : 추가
            txt_AWT.Text = CData.Opt.iAWT.ToString();
            txt_AWP.Text = CData.Opt.iAWP.ToString();
            // 201029 jym : Add
            ckb_AtwSkip.Checked = CData.Opt.bAwSkip;
            // 201029 jym End

            //syc : ase kr loading stop,Loading Stop on type 
            txt_AutoLDStopCount.Text = CData.Opt.iAutoLoadingStopCount.ToString();  // 2020-11-17, jhLee : Loading stop 투입 수량
            
            if (CDataOption.UseAutoLoadingStop) { cb_LDStopType.SelectedIndex = CData.Opt.iLoadingStopType; }
            else                                { cb_LDStopType.SelectedItem = 0; } //0 - Loader(nomal) 1 - table

            // 2021-05-17, jhLee, Multi-LOT 기능 지원
            ckb_UseMultiLot.Checked = CData.Opt.bMultiLOTUse;                   // 연속LOT 사용 여부 (라이센스에서 사용으로 설정되어야 화면 표시)
            txt_EmptySlotCount.Text = CData.Opt.nLOTEndEmptySlot.ToString();    // LOT 종료를 표시하는 연속 빈 Slot의 수량
        }

        public void Get()
        {
            int.TryParse(txt_WarmVel.Text,      out CData.Opt.iWmUS);
            int.TryParse(txt_WarmT.Text,        out CData.Opt.iWmUT);
            int.TryParse(txt_WarmPeriod.Text,   out CData.Opt.iWmUI); //syc : Warm up 유휴 시간
            CData.Opt.bWarmUpWithStrip      = ckb_WarmUpWithStrip.Checked;  // 2022.06.10 lhs : (SCK+)자재가 있어도 워밍업

            int.TryParse(txt_LotMax.Text,   out CData.Opt.iLotCnt);
            //200401 jym : 추가
            CData.Opt.bLotTblClean          = ckb_LotTblClean.Checked;
            CData.Opt.bLotDrs               = ckb_LotDrs.Checked;
            
            int.TryParse(txt_LvChange.Text, out CData.Opt.iChangeLevel);

            CData.Opt.bDoorSkip             = ckb_DoorSkip.Checked;
            CData.Opt.bDryAuto              = ckb_DryGrindSkip.Checked;
            CData.Opt.bDryAutoMeaStripSkip  = ckb_DryMeaSkip.Checked;
            CData.Opt.bLeakSkip             = ckb_LeakSkip.Checked;
            CData.Opt.bWarmUpSkip           = ckb_WarmUpSkip.Checked;
            CData.Opt.bQcUse                = ckb_QcUse.Checked;
            CData.Opt.bQcByPass             = ckb_QcByPass.Checked;
            CData.Opt.bQcDoor               = ckb_QcDoor.Checked;       // 201116 jym : QC 도어연동 기능 추가
            CData.Opt.bSecsUse              = ckb_SecsUse.Checked;

            int.TryParse(txt_RCMDStartTimeout.Text, out CData.Opt.iRCMDStartTimeout);     // 2020-10-19, jhLee : RCMD START Timeout 지정

            // 2021-07-01, jhLee : SPIL-QLE VOC : SECS/GEM 사용시 Start 응답 Timeout은 0 값으로 설정되지 못하게 한다. 기본값 6초로 지정
            if (CData.Opt.iRCMDStartTimeout <= 0)
            {
                CData.Opt.iRCMDStartTimeout = 6000;         // 기본값으로 지정한다.
                txt_RCMDStartTimeout.Text = CData.Opt.iRCMDStartTimeout.ToString();
            }

            CData.Opt.bDiChillerSkip        = ckb_DiChiler.Checked;

            //MTBA MTBF
            double.TryParse(txt_WorkTime.Text, out CData.Opt.dDayWorking);

            CData.Opt.iPeriod               = cmb_CalPeriod.SelectedIndex;
            CData.Opt.iRenewal              = Convert.ToInt32(cmb_RenewalTime.SelectedItem);
            
            int.TryParse(txt_MtbaSetTime.Text, out CData.Opt.iSetTime);
            
            CData.Opt.iSelLan               = cmb_Lang.SelectedIndex; //190516 ksg :

            if(CDataOption.UseNetDrive) { CData.Opt.bNetUse = ckb_NetUse.Checked; }
            else                        { CData.Opt.bNetUse = false; }

            CData.Opt.sNetIP                = txt_NetIP.Text;
            CData.Opt.sNetPath              = txt_NetPath.Text;
            CData.Opt.sNetID                = txt_NetID.Text;
            CData.Opt.sNetPw                = txt_NetPw.Text;
            
            CData.Opt.iProbeFontSize        = Convert.ToInt32(cb_ProbeFontSize.SelectedItem);
            CData.Opt.iProbeFloationPoint   = Convert.ToInt32(cb_ProbeFloationPoint.SelectedItem);
            
            // 2021.04.09 SungTae Start : Wheel History Auto Delete Period
            //201005 pjh : Log Delete Period
            CData.Opt.iDelPeriod            = int.Parse(txt_DelPeriod.Text);
            CData.Opt.iDelPeriodHistory     = int.Parse(txt_DelPeriodHistory.Text);
            // 2021.04.09 SungTae End

            // 201023 jym : 추가
            int.TryParse(txt_AWT.Text, out CData.Opt.iAWT);
            int.TryParse(txt_AWP.Text, out CData.Opt.iAWP);

            // 201029 jym : Add
            CData.Opt.bAwSkip               = ckb_AtwSkip.Checked;
            // 201029 jym End

            //syc : ase kr loading stop - Loader에서 General 쪽으로 이동
            int.TryParse(txt_AutoLDStopCount.Text, out CData.Opt.iAutoLoadingStopCount);    // 2020-11-17, jhLee : Loading stop 투입 수량

            CData.Opt.iAutoLoadingStopCount = (CData.Opt.iAutoLoadingStopCount <= 1) ? 1 : CData.Opt.iAutoLoadingStopCount;     // 최소/최대 범위 체크
            
            if(CData.CurCompany == ECompany.ASE_KR)
            {
                CData.Opt.iAutoLoadingStopCount = (CData.Opt.iAutoLoadingStopCount > 10) ? 10 : CData.Opt.iAutoLoadingStopCount;
            }
            else if(CData.CurCompany == ECompany.SkyWorks)
            {
                CData.Opt.iAutoLoadingStopCount = (CData.Opt.iAutoLoadingStopCount > 5) ? 5 : CData.Opt.iAutoLoadingStopCount; //210309 pjh : Set Up Strip 설정 가능 Count
            }

            // 2021.10.25 SungTae : [수정] Code 확인 용이하도록 변경 (0 -> (int)eTypeLDStop.Inrail)
            if (CDataOption.UseAutoLoadingStop) { CData.Opt.iLoadingStopType = cb_LDStopType.SelectedIndex;}
            else                                { CData.Opt.iLoadingStopType = (int)eTypeLDStop.Inrail/*0*/; }


            // 2021-05-17, jhLee, Multi-LOT 기능 지원
            CData.Opt.bMultiLOTUse = ckb_UseMultiLot.Checked;   // 연속LOT 사용 여부 (라이센스에서 사용으로 설정되어야 화면 표시)

            // LOT 종료를 표시하는 연속 빈 Slot의 수량
            if (int.TryParse(txt_EmptySlotCount.Text, out CData.Opt.nLOTEndEmptySlot) == false)
            {
                CData.Opt.nLOTEndEmptySlot = 3;             // 입력된 값이 잘못되었다면 기본값으로 3을 설정한다.
            }
            CData.LotMgr.MaxEmptySlot = CData.Opt.nLOTEndEmptySlot;     // 실제 사용하는 Class의 Max count도 Update한다

            //210811 pjh : D/F Server Data Save 기능 Net Drive 추가
            //if(CData.CurCompany == ECompany.JCET)
            if(CDataOption.UseDFNet)    { CData.Opt.bDFNetUse = ckb_DFNetUse.Checked; }
            else                        { CData.Opt.bDFNetUse = false; }
            //
            //211217 pjh : SECS/GEM과 D/F Server 기능이 동시 활성화 됐을 때 SECS/GEM만 활성화
            if (CDataOption.SecsUse == eSecsGem.Use && CDataOption.UseDFNet)
            {
                if (CData.Opt.bSecsUse && CData.Opt.bDFNetUse)
                {
                    CData.Opt.bDFNetUse = false;
                    return;
                }
            }
            //
        }

        /// <summary>
        /// 해당 View가 Dispose(종료)될 때 실행 됨 
        /// View의 Dispose함수 실행하면 자동으로 실행됨
        /// </summary>
        private void _Release()
        {
        }

        #endregion
        
        #region Private method
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
        
        //201223 jhc : VOC from ASE-KH 
        /// <summary>
        /// ASE-KH VOC : MASTER Level만 Door Skip 옵션 변경 가능하도록 요청 (2020.12.16.)
        /// </summary>
        private void _SetDoorSkipEnable()
        {
            if (CData.CurCompany == ECompany.ASE_K12)
            {
                ckb_DoorSkip.Enabled = (CData.Lev == ELv.Master); //ASE-KH는 MASTER Level에서만 Door Skip 옵션 변경 가능
            }
            else
            {
                ckb_DoorSkip.Enabled = (CData.CurCompany != ECompany.SkyWorks); //Skyworks는 비활성, 나머지 Company는 활성화
            }
        }
        //..

        #endregion

        private void ckb5_DryGrindSkip_CheckedChanged(object sender, EventArgs e)
        {
            if (ckb_DryGrindSkip.Checked)
            {
                ckb_DryMeaSkip.Enabled = true;
            }
            else
            {
                ckb_DryMeaSkip.Checked = true;
                ckb_DryMeaSkip.Enabled = false;
            }
        }

        private void ckb_QcUse_CheckedChanged(object sender, EventArgs e)
        {
            ckb_QcByPass.Enabled = ckb_QcUse.Checked;
            ckb_QcDoor.Enabled = ckb_QcUse.Checked;
        }
    }
}
