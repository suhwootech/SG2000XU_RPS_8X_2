using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace SG2000X
{
    public partial class vwDev_03Set : UserControl
    {
        private int m_iAx1 = 0;
        private int m_iAx2 = 0;
        private int m_iAx3 = 0;
        private bool m_bAx3 = true;

        private string m_sStep = "";

        /// <summary>
        /// 셋 포지션 탭내에서 파트 탭 번호
        /// </summary>
        private int m_iSysPart = 0;

        /// <summary>
        /// 화면에 정보 업데이트 타이머
        /// </summary>
        private Timer m_tmUpdt;

        public vwDev_03Set()
        {
            if      ((int)ELang.English == CData.Opt.iSelLan)   {   System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");     }
            else if ((int)ELang.Korea   == CData.Opt.iSelLan)   {   System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ko-KR");     }
            else if ((int)ELang.China   == CData.Opt.iSelLan)   {   System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-Hans");   }
            InitializeComponent();

            //20190611 ghk_onpclean
            if (CDataOption.eOnpClean == eOnpClean.Use)
            {
                lbS_Onp_X_Clean.Visible = true;
                txtS_OnP_X_Clean.Visible = true;
                btnS_OnpGet_X_Clean.Visible = true;
                btnS_OnpMv_X_Clean.Visible = true;

                lbS_Onp_Z_Clean.Visible = true;
                txtS_OnP_Z_Clean.Visible = true;
                btnS_OnpGet_Z_Clean.Visible = true;
                btnS_OnpMv_Z_Clean.Visible = true;
                pnlS_OnP_Clean.Visible = true;
            }
            //20191010 ghk_manual_bcr
            if (CDataOption.ManualBcr == eManualBcr.NotUse)
            {
                //X
                lblS_OnP_X_Bcr_TbL.Visible = false;
                lblS_OnP_X_Ori_TbL.Visible = false;
                lblS_OnP_X_Bcr_TbR.Visible = false;
                lblS_OnP_X_Ori_TbR.Visible = false;

                txtS_OnP_X_Bcr_TbL.Visible = false;
                txtS_OnP_X_Bcr_TbR.Visible = false;
                txtS_OnP_X_Ori_TbL.Visible = false;
                txtS_OnP_X_Ori_TbR.Visible = false;

                btnS_Get_OnP_X_Bcr_TbL.Visible = false;
                btnS_Get_OnP_X_Ori_TbL.Visible = false;
                btnS_Get_OnP_X_Bcr_TbR.Visible = false;
                btnS_Get_OnP_X_Ori_TbR.Visible = false;

                btnS_Mov_OnP_X_Bcr_TbL.Visible = false;
                btnS_Mov_OnP_X_Ori_TbL.Visible = false;
                btnS_Mov_OnP_X_Bcr_TbR.Visible = false;
                btnS_Mov_OnP_X_Ori_TbR.Visible = false;

                //Z
                lblS_OnP_Z_Bcr_TbL.Visible = false;
                lblS_OnP_Z_Ori_TbL.Visible = false;
                lblS_OnP_Z_Bcr_TbR.Visible = false;
                lblS_OnP_Z_Ori_TbR.Visible = false;

                txtS_OnP_Z_Bcr_TbL.Visible = false;
                txtS_OnP_Z_Bcr_TbR.Visible = false;
                txtS_OnP_Z_Ori_TbL.Visible = false;
                txtS_OnP_Z_Ori_TbR.Visible = false;

                btnS_Get_OnP_Z_Bcr_TbL.Visible = false;
                btnS_Get_OnP_Z_Ori_TbL.Visible = false;
                btnS_Get_OnP_Z_Bcr_TbR.Visible = false;
                btnS_Get_OnP_Z_Ori_TbR.Visible = false;

                btnS_Mov_OnP_Z_Bcr_TbL.Visible = false;
                btnS_Mov_OnP_Z_Ori_TbL.Visible = false;
                btnS_Mov_OnP_Z_Bcr_TbR.Visible = false;
                btnS_Mov_OnP_Z_Ori_TbR.Visible = false;

                //Y
                lblS_OnP_Y_Bcr_TbL.Visible = false;
                lblS_OnP_Y_Ori_TbL.Visible = false;
                lblS_OnP_Y_Bcr_TbR.Visible = false;
                lblS_OnP_Y_Ori_TbR.Visible = false;

                txtS_OnP_Y_Bcr_TbL.Visible = false;
                txtS_OnP_Y_Bcr_TbR.Visible = false;
                txtS_OnP_Y_Ori_TbL.Visible = false;
                txtS_OnP_Y_Ori_TbR.Visible = false;

                btnS_Get_OnP_Y_Bcr_TbL.Visible = false;
                btnS_Get_OnP_Y_Ori_TbL.Visible = false;
                btnS_Get_OnP_Y_Bcr_TbR.Visible = false;
                btnS_Get_OnP_Y_Ori_TbR.Visible = false;

                btnS_Mov_OnP_Y_Bcr_TbL.Visible = false;
                btnS_Mov_OnP_Y_Ori_TbL.Visible = false;
                btnS_Mov_OnP_Y_Bcr_TbR.Visible = false;
                btnS_Mov_OnP_Y_Ori_TbR.Visible = false;
            }

            if (CDataOption.Dryer == eDryer.Knife)
            {
                tpDry_R.Text = "Y Axis";

                pnlS_Dry_Str.Visible = false;
                pnlS_Dry_Unit.Visible = true;
            }

			//201230 jhc : 2D 코드 판독 실패 Alarm 발생 전 제3의 위치(OCR 위치)로 카메라 이동 기능
			if (CDataOption.Use2DErrorPosition && (CDataOption.CurEqu == eEquType.Pikcer))
            {
                lbl_OnP_X_BcrErr.Visible = true;
                txtS_OnP_X_BcrErr.Visible = true;
                btnS_Get_OnP_X_BcrErr.Visible = true;
                btnS_Mov_OnP_X_BcrErr.Visible = true;

                lbl_OnP_Y_BcrErr.Visible = true;
                txtS_OnP_Y_BcrErr.Visible = true;
                btnS_Get_OnP_Y_BcrErr.Visible = true;
                btnS_Mov_OnP_Y_BcrErr.Visible = true;

                lbl_OnP_Z_BcrErr.Visible = true;
                txtS_OnP_Z_BcrErr.Visible = true;
                btnS_Get_OnP_Z_BcrErr.Visible = true;
                btnS_Mov_OnP_Z_BcrErr.Visible = true;
            }
            else
            {
                lbl_OnP_X_BcrErr.Visible = false;
                txtS_OnP_X_BcrErr.Visible = false;
                btnS_Get_OnP_X_BcrErr.Visible = false;
                btnS_Mov_OnP_X_BcrErr.Visible = false;

                lbl_OnP_Y_BcrErr.Visible = false;
                txtS_OnP_Y_BcrErr.Visible = false;
                btnS_Get_OnP_Y_BcrErr.Visible = false;
                btnS_Mov_OnP_Y_BcrErr.Visible = false;

                lbl_OnP_Z_BcrErr.Visible = false;
                txtS_OnP_Z_BcrErr.Visible = false;
                btnS_Get_OnP_Z_BcrErr.Visible = false;
                btnS_Mov_OnP_Z_BcrErr.Visible = false;

                //syc : new cleaner
                if (CDataOption.UseNewClenaer)
                {
                    lblS_GrdL_TcDn.Visible = false;
                    ckbS_GrdL_TcDn.Visible = false;
                    lblS_GrdR_TcDn.Visible = false;
                    ckbS_GrdR_TcDn.Visible = false;

                    ckbS_GrdL_TcSpng.Text = "Cleaner Water";
                    ckbS_GrdL_TcKnif.Text = "Point Water";

                    ckbS_GrdR_TcSpng.Text = "Cleaner Water";
                    ckbS_GrdR_TcKnif.Text = "Point Water";
                }
            }
            //

            //--------------
            // OffPicker
            //--------------
            // OffPicker에 IV 설치 여부
            bool bOffP_IOIV = CDataOption.UseDryWtNozzle || CDataOption.UseOffP_IOIV;

            pnlS_OffP_IV2.Visible = bOffP_IOIV;     // IV2(화상판별센서) 버튼 및 상태 

            int nProgNum = CSq_OfP.It.Func_GetIVProgram();
            comboS_OffP_IV2_Program.SelectedIndex = nProgNum;

            bool bProg1 = CIO.It.Get_Y(eY.OFFP_IV2_ProgBit0);  // 현재 선택된 IV2 Program 표시
            comboS_OffP_IV2_Program.SelectedIndex = Convert.ToInt32(bProg1);

			lblS_OffP_X_Picture1.Visible        = bOffP_IOIV;    // 촬영위치1 X
			txtS_OffP_X_Picture1.Visible        = bOffP_IOIV;
			btnS_OffP_X_Move_Picture1.Visible   = bOffP_IOIV;

			lblS_OffP_X_Picture2.Visible        = bOffP_IOIV;    // 촬영위치2 X
			txtS_OffP_X_Picture2.Visible        = bOffP_IOIV;
			btnS_OffP_X_Move_Picture2.Visible   = bOffP_IOIV;

			lblS_OffP_Z_Picture.Visible         = bOffP_IOIV;     // 촬영위치 Z
			txtS_OffP_Z_Picture.Visible         = bOffP_IOIV;
			btnS_OffP_Z_Move_Picture.Visible    = bOffP_IOIV;

            bool bSpringClamp = CDataOption.eDryClamp == EDryClamp.None; // 기존 Spring Clamp 방식
            // OffP Z축 Dryer와 같이 Down 
            txtS_OffP_Z_DryGap.Visible  = bSpringClamp;
            label100.Visible            = bSpringClamp;
            label99.Visible             = bSpringClamp;

            //--------------
            // Dryer
            //--------------
            // 좌측 중앙에 위치
            lblS_Dry_Lv1.Visible    = bSpringClamp;
            lblS_Dry_Lv2.Visible    = bSpringClamp;

            // Cylinder Clamp 판넬
            pnlS_Dry_Clamp.Visible = CDataOption.eDryClamp == EDryClamp.Cylinder;

            // Dryer Z Axis 탭
            lblS_Dry_Z_Level.Visible        = bSpringClamp;
            txtS_Dry_Z_Level.Visible        = bSpringClamp;
            btnS_Dry_Z_Move_Level.Visible   = bSpringClamp;

            lblS_Dry_Z_TipClamped.Visible       = bOffP_IOIV;
            txtS_Dry_Z_TipClamped.Visible       = bOffP_IOIV;
            btnS_Dry_Z_Move_TipClamped.Visible  = bOffP_IOIV;

            // Dryer Rotate Tap
            lblS_Dry_R_Lv1.Visible          = bSpringClamp;
            txtS_Dry_R_Lv1.Visible          = bSpringClamp;
            btnS_Dry_R_Get_Lv1.Visible      = bSpringClamp;
            btnS_Dry_R_Move_Lv1.Visible     = bSpringClamp;

            lblS_Dry_R_Lv2.Visible          = bSpringClamp;
            txtS_Dry_R_Lv2.Visible          = bSpringClamp;
            btnS_Dry_R_Get_Lv2.Visible      = bSpringClamp;
            btnS_Dry_R_Move_Lv2.Visible     = bSpringClamp;

            // 2021.03.30 lhs Start
            if (CDataOption.UseDryWtNozzle)
            {
                ckbS_Dry_BtmAir.Text    = "Dry Water Nozzle";
                lblS_Dry_BtmStd.Text    = "Dry Water Nozzle Standby";
                lblS_Dry_BtmTar.Text    = "Dry Water Nozzle  Target";
                btnS_Dry_Std.Text       = "Dry Water Nozzle Standby";
                btnS_Dry_Tar.Text       = "Dry Water Nozzle  Target";                
            }
            else
            {
                ckbS_Dry_BtmAir.Text    = "Bottom Air Blow";
                lblS_Dry_BtmStd.Text    = "Bottom Air Blow Standby";
                lblS_Dry_BtmTar.Text    = "Bottom Air Blow Target";
                btnS_Dry_Std.Text       = "BTM AIR STANDBY";
                btnS_Dry_Tar.Text       = "BTM AIR TARGET";
            }
            // 2021.03.30 lhs End

            //210831 syc : 2004U IV2
            if (CDataOption.Use2004U)
            {
                panel9.Visible = false; //inrail align 패널 비활성화

                lblS_GrdL_CVac.Visible = true; //GRDL Carrier Vac 
                lblS_GrdR_CVac.Visible = true; //GRDR Carrier Vac 

                pnlS_OffP_Clamp.Visible = true; // offp Clamp 패널

                lblS_Dry_Lv1.Text = "Cover Detect"; // Safty 센서 이름 변경
                lblS_Dry_Lv1.Visible = false;       // Dry Safty2 센서 사용 안함

                panel20.Visible = false; // 온피커 로드셀 사용안함
                panel21.Visible = false; // 오프피커 로드셀 사용안함

                // ONP 라벨 이름 좀더 쉽게 변경 테스트
                label30.Text = "Inrail Pick Position [mm]";
                label37.Text = "Table Pick && Place Position [mm]";
                // OFP 라벨 이름 좀더 쉽게 변경 테스트
                label93.Text = "Table Pick && Place Position [mm]";
                label92.Text = "Dry Zone Place Position [mm]";
            }


            // 2022.03.04 lhs Start : SprayBtmCleaner 사용시 표시
            bool bSprayCln = CDataOption.UseSprayBtmCleaner;
            ckbS_Dry_BtmClnNzlMove.Visible      = bSprayCln;
            ckbS_Dry_BtmClnAirBlow.Visible      = bSprayCln;
            lblS_Dry_BtmClnNzlForward.Visible   = bSprayCln;
            lblS_Dry_BtmClnNzlBackward.Visible  = bSprayCln;
            // 2022.03.04 lhs End

            // 2022.03.15 lhs Start : Dryer의 Level Safety Sensor에 Air Blow 설치시
            ckbS_Dry_LevelAirBlow.Visible = CDataOption.UseDryerLevelAirBlow;

            /// <summary>
            /// ONP로 Table 에서 자재 Pick시
            /// Pick Position 일정값 조절하는 기능
            /// 2004U Unit 타입 "Place" 포지션 하나로 Pick, Place 시 Unit 피커 패드에 붙음
            /// </summary>
            //211026 syc : Onp Pick Up Offset
            if (CDataOption.UseOnpPickUpOffset)
            {
                lbS_Onp_Z_PickOffset .Visible = true;
                txtS_Onp_Z_PickOffset.Visible = true;
            }

            m_tmUpdt = new Timer();
            m_tmUpdt.Interval = 50;
            m_tmUpdt.Tick += _M_tmUpdt_Tick;
        }

        #region Private method
        private void _M_tmUpdt_Tick(object sender, EventArgs e)
        {
            if (CSQ_Man.It.bBtnShow && (CSQ_Main.It.m_iStat == EStatus.Idle || CSQ_Main.It.m_iStat == EStatus.Stop))
            {
                CSQ_Man.It.bBtnShow = true;
                _EnableBtn(true);
            }

            if (CData.WMX)
            {
                #region Motion
                for (int i = 0; i < 3; i++)
                {
                    int iAx;
                    if(CDataOption.CurEqu == eEquType.Nomal)
                    {
                        iAx = m_iAx1 + i;
                    }
                    else
                    {
                        // 20190604 ghk_onpbcr
                        iAx = m_iAx1;
                        if      (i == 0) { iAx = m_iAx1; }
                        else if (i == 1) { iAx = m_iAx2; }
                        else             { iAx = m_iAx3; }
                    }
                   
                    string sId = (i + 1).ToString();
                    Panel mPnl = (Panel)this.Controls["pnlS_Ax" + sId];
                    //
                    if (i == 2 && m_bAx3 == false)
                    {
                        mPnl.Visible = false;
                    }
                    else
                    {
                        mPnl.Visible = true;

                        mPnl.Controls["lblS_Ax" + sId].Text = ((EAx)iAx).ToString();
                        if (CMot.It.Chk_OP(iAx) == EAxOp.Idle)
                        { mPnl.Controls["lblS_Ax" + sId].BackColor = GV.CR_X[0]; }
                        else
                        { mPnl.Controls["lblS_Ax" + sId].BackColor = GV.CR_X[1]; }

                        mPnl.Controls["lblS_Srv" + sId].BackColor = GV.CR_X[CMot.It.Chk_SrvI (iAx)];
                        mPnl.Controls["lblS_Al"  + sId].BackColor = GV.CR_Y[CMot.It.Chk_AlrI (iAx)];
                        mPnl.Controls["lblS_HD"  + sId].BackColor = GV.CR_X[CMot.It.Chk_HDI  (iAx)];
                        mPnl.Controls["lblS_N"   + sId].BackColor = GV.CR_X[CMot.It.Chk_NOTI (iAx)];
                        mPnl.Controls["lblS_H"   + sId].BackColor = GV.CR_X[CMot.It.Chk_HomeI(iAx)];
                        mPnl.Controls["lblS_P"   + sId].BackColor = GV.CR_X[CMot.It.Chk_POTI (iAx)];

                        mPnl.Controls["lblS_Cmd" + sId].Text = CMot.It.Get_CP(iAx).ToString();
                        mPnl.Controls["lblS_Enc" + sId].Text = CMot.It.Get_FP(iAx).ToString();
                    }
                }
                #endregion

                switch (m_iSysPart)
                {
                    case 0:    // On Loader
                               // Door
                        lblS_OnL_FDoor.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.ONL_DoorFront]];
                        lblS_OnL_LDoor.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.ONL_DoorLeft]];
                        lblS_OnL_RDoor.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.ONL_DoorRear]];
                        lblS_OnL_LCurt.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.ONL_LightCurtain]];
                        // Top Conveyor
                        lblS_OnL_TpDetc.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.ONL_TopMGZDetect]];
                        lblS_OnL_TpFull.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.ONL_TopMGZDetectFull]];
                        // Bottom Conveyor
                        lblS_OnL_BtDetc.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.ONL_BtmMGZDetect]];
                        // Pubsher
                        lblS_OnL_Back.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.ONL_PusherBackward]];
                        lblS_OnL_Forw.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.ONL_PusherForward]];
                        lblS_OnL_Over.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.ONL_PusherOverLoad]];
                        // MGZ Clamp
                        lblS_OnL_Detc1.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.ONL_ClampMGZDetect1]];
                        lblS_OnL_Detc2.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.ONL_ClampMGZDetect2]];
                        lblS_OnL_ClmpOn.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.ONL_ClampOn]];
                        lblS_OnL_ClmpOff.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.ONL_ClampOff]];
                        break;

                    case 1:
                        // Detect
                        lblS_Inr_InDect.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.INR_StripInDetect]];
                        lblS_Inr_BtmDetc.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.INR_StripBtmDetect]];
                        // Align Guide
                        lblS_Inr_Forw.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.INR_GuideForward]];
                        lblS_Inr_Back.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.INR_GuideBackward]];
                        // Gripper
                        lblS_Inr_ClmpOn.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.INR_GripperClampOn]];
                        lblS_Inr_ClmpOff.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.INR_GripperClampOff]];
                        lblS_Inr_GripDetc.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.INR_GripperDetect]];
                        // Lift
                        lblS_Inr_LiftUp.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.INR_LiftUp]];
                        lblS_Inr_LiftDn.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.INR_LiftDn]];
                        break;

                    case 2:
                        // Rotate
                        lblS_OnP_Rot0.BackColor  = GV.CR_X[CIO.It.aInput[(int)eX.ONP_Rotate0]];
                        lblS_OnP_Rot90.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.ONP_Rotate90]];
                        // Utility
                        lblS_OnP_Vac.BackColor   = GV.CR_X[CIO.It.aInput[(int)eX.ONP_Vacuum]];
                        // Load Cell
                        lblS_OnP_LodCell.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.ONP_LoadCell]];
                        //20190628 ghk_onpclean
                        lblS_OnP_CleanL.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.INR_OnpCleaner_L]];
                        lblS_OnP_CleanR.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.INR_OnpCleaner_R]];
                        //
                        break;

                    case 3:
                        // Common
                        lblS_GrdL_FDor.BackColor  = GV.CR_X[CIO.It.aInput[(int)eX.GRD_DoorFront]];
                        lblS_GrdL_RDor.BackColor  = GV.CR_X[CIO.It.aInput[(int)eX.GRD_DoorRear]];
                        lblS_GrdL_FCovr.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRD_CoverFront]];
                        lblS_GrdL_RCovr.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRD_CoverRear]];
                        // Table
                        lblS_GrdL_TVac.BackColor  = GV.CR_X[CIO.It.aInput[(int)eX.GRDL_TbVaccum]];
                        lblS_GrdL_TWtr.BackColor  = GV.CR_X[CIO.It.aInput[(int)eX.GRDL_TbFlow]]; //201209 jhc : DEBUG
                        lblS_GrdL_PumpR.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.PUMPL_Run]];
                        //211005 syc : 2004U
                        if (CDataOption.Use2004U) { lblS_GrdL_CVac.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDL_Carrier_Vacuum_4U]]; }
                        //

                        // 2020.10.22 JSKim St
                        if (CData.CurCompany == ECompany.JCET && CData.Opt.bDualPumpUse == true)
                        {
                            lblS_GrdL_AddPumpR.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.ADD_PUMPL_Run]];
                        }
                        // 2020.10.22 JSKim Ed

                        if (CIO.It.aInput[(int)eX.PUMPL_OverLoad] == 1)
                        {
                            lblS_GrdL_PumpA.BackColor = GV.CR_Y[1];
                            //koo : Qorvo 언어변환.lblS_GrdL_PumpA.Text = "Overload";
                            SetWriteLanguage(lblS_GrdL_PumpA.Name,CLang.It.GetLanguage("Overload"));

                        }
                        else if (CIO.It.aInput[(int)eX.PUMPL_FlowLow] == 1)
                        {
                            lblS_GrdL_PumpA.BackColor = GV.CR_Y[1];
                            //lblS_GrdL_PumpA.Text = "Flow Low";
                            SetWriteLanguage(lblS_GrdL_PumpA.Name,CLang.It.GetLanguage("Flow Low"));

                        }
                        else if (CIO.It.aInput[(int)eX.PUMPL_TempHigh] == 1)
                        {
                            lblS_GrdL_PumpA.BackColor = GV.CR_Y[1];
                            //lblS_GrdL_PumpA.Text = "Temp High";
                            SetWriteLanguage(lblS_GrdL_PumpA.Name,CLang.It.GetLanguage("Temp High"));
                        }
                        else
                        {
                            lblS_GrdL_PumpA.BackColor = GV.CR_X[1];
                            //lblS_GrdL_PumpA.Text = "No Alarm";
                            SetWriteLanguage(lblS_GrdL_PumpA.Name,CLang.It.GetLanguage("No Alarm"));
                        }

                        // 2020.10.22 JSKim St
                        if (CData.CurCompany == ECompany.JCET && CData.Opt.bDualPumpUse == true)
                        {
                            if (CIO.It.aInput[(int)eX.ADD_PUMPL_OverLoad] == 1)
                            {
                                lblS_GrdL_AddPumpA.BackColor = GV.CR_Y[1];
                                //lblS_GrdL_PumpA.Text = "Overload";
                                SetWriteLanguage(lblS_GrdL_AddPumpA.Name, CLang.It.GetLanguage("Overload"));

                            }
                            else if (CIO.It.aInput[(int)eX.ADD_PUMPL_FlowLow] == 1)
                            {
                                lblS_GrdL_AddPumpA.BackColor = GV.CR_Y[1];
                                //lblS_GrdL_PumpA.Text = "Flow Low";
                                SetWriteLanguage(lblS_GrdL_AddPumpA.Name, CLang.It.GetLanguage("Flow Low"));

                            }
                            else if (CIO.It.aInput[(int)eX.ADD_PUMPL_TempHigh] == 1)
                            {
                                lblS_GrdL_AddPumpA.BackColor = GV.CR_Y[1];
                                //lblS_GrdL_PumpA.Text = "Temp High";
                                SetWriteLanguage(lblS_GrdL_AddPumpA.Name, CLang.It.GetLanguage("Temp High"));
                            }
                            else
                            {
                                lblS_GrdL_AddPumpA.BackColor = GV.CR_X[1];
                                //lblS_GrdL_PumpA.Text = "No Alarm";
                                SetWriteLanguage(lblS_GrdL_AddPumpA.Name, CLang.It.GetLanguage("No Alarm"));
                            }
                        }
                        // 2020.10.22 JSKim Ed

                        // Top Cleaner
                        lblS_GrdL_TcDn.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDL_TopClnDn]];
                        lblS_GrdL_TcFl.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDL_TopClnFlow]];
                        // Grind
                        lblS_GrdL_GrdFl.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDL_SplWater]];
                        lblS_GrdL_GrdFl.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDL_SplWaterTemp]];
                        lblS_GrdL_BtmFl.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDL_SplBtmWater]];
                        // Spindle
                        lblS_GrdL_PcwFl.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDL_SplPCW]];
                        lblS_GrdL_PcwTmp.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDL_SplPCWTemp]];

                        //20191022 ghk_spindle_type
                        //if (CSpl.It.bUse232)
                        //if (CDataOption.SplType == eSpindleType.Rs232)
                        //{
                        //    lblS_GrdL_Rpm.Text = CData.Spls[0].iRpm.ToString() + " rpm";
                        //    lblS_GrdL_Load.Text = CData.Spls[0].dLoad.ToString() + " %";
                        //}
                        //else
                        //{
                        //    lblS_GrdL_Rpm.Text = CSpl.It.GetFrpm(true).ToString() + " rpm";
                        //    lblS_GrdL_Load.Text = CData.Spls[0].dLoad.ToString("0.0") + " %";
                        //}                        
                        // 2023.03.15 Max
                        lblS_GrdL_Rpm.Text = CData.Spls[0].iRpm.ToString() + " rpm";
                        lblS_GrdL_Load.Text = CData.Spls[0].dLoad.ToString() + " %";
                        break;

                    case 4:
                        // Common
                        lblS_GrdR_FDor.BackColor  = GV.CR_X[CIO.It.aInput [(int)eX.GRD_DoorFront]];
                        lblS_GrdR_RDor.BackColor  = GV.CR_X[CIO.It.aInput [(int)eX.GRD_DoorRear ]];
                        lblS_GrdR_FCovr.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRD_CoverFront]];
                        lblS_GrdR_RCovr.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRD_CoverRear ]];
                        // Table
                        lblS_GrdR_TVac.BackColor  = GV.CR_X[CIO.It.aInput[(int)eX.GRDR_TbVaccum]];
                        lblS_GrdR_TWtr.BackColor  = GV.CR_X[CIO.It.aInput[(int)eX.GRDR_TbFlow]]; //201209 jhc : DEBUG
                        lblS_GrdR_PumpR.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.PUMPR_Run    ]]; //190129 ksg : tnwjd

                        //211005 syc : 2004U
                        if (CDataOption.Use2004U) { lblS_GrdR_CVac.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDR_Carrier_Vacuum_4U]]; }
                        //

                        // 2020.10.22 JSKim St
                        if (CData.CurCompany == ECompany.JCET && CData.Opt.bDualPumpUse == true)
                        {
                            lblS_GrdR_AddPumpR.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.ADD_PUMPR_Run]];
                        }
                        // 2020.10.22 JSKim Ed

                        if (CIO.It.aInput[(int)eX.PUMPR_OverLoad] == 1)
                        {
                            lblS_GrdR_PumpA.BackColor = GV.CR_Y[1];
                            //lblS_GrdR_PumpA.Text = "Overload";
                            SetWriteLanguage(lblS_GrdR_PumpA.Name,CLang.It.GetLanguage("Overload"));
                        }
                        else if (CIO.It.aInput[(int)eX.PUMPR_FlowLow] == 1)
                        {
                            lblS_GrdR_PumpA.BackColor = GV.CR_Y[1];
                            //lblS_GrdR_PumpA.Text = "Flow Low";
                            SetWriteLanguage(lblS_GrdR_PumpA.Name,CLang.It.GetLanguage("Flow Low"));
                        }
                        else if (CIO.It.aInput[(int)eX.PUMPR_TempHigh] == 1)
                        {
                            lblS_GrdR_PumpA.BackColor = GV.CR_Y[1];
                            //lblS_GrdR_PumpA.Text = "Temp High";
                            SetWriteLanguage(lblS_GrdR_PumpA.Name,CLang.It.GetLanguage("Temp High"));
                        }
                        else
                        {
                            lblS_GrdR_PumpA.BackColor = GV.CR_X[1];
                            //lblS_GrdR_PumpA.Text = "No Alarm";
                            SetWriteLanguage(lblS_GrdR_PumpA.Name,CLang.It.GetLanguage("No Alarm"));
                        }

                        // 2020.10.22 JSKim St
                        if (CData.CurCompany == ECompany.JCET && CData.Opt.bDualPumpUse == true)
                        {
                            if (CIO.It.aInput[(int)eX.ADD_PUMPR_OverLoad] == 1)
                            {
                                lblS_GrdR_AddPumpA.BackColor = GV.CR_Y[1];
                                //lblS_GrdL_PumpA.Text = "Overload";
                                SetWriteLanguage(lblS_GrdR_AddPumpA.Name, CLang.It.GetLanguage("Overload"));

                            }
                            else if (CIO.It.aInput[(int)eX.ADD_PUMPR_FlowLow] == 1)
                            {
                                lblS_GrdR_AddPumpA.BackColor = GV.CR_Y[1];
                                //lblS_GrdL_PumpA.Text = "Flow Low";
                                SetWriteLanguage(lblS_GrdR_AddPumpA.Name, CLang.It.GetLanguage("Flow Low"));

                            }
                            else if (CIO.It.aInput[(int)eX.ADD_PUMPR_TempHigh] == 1)
                            {
                                lblS_GrdR_AddPumpA.BackColor = GV.CR_Y[1];
                                //lblS_GrdL_PumpA.Text = "Temp High";
                                SetWriteLanguage(lblS_GrdR_AddPumpA.Name, CLang.It.GetLanguage("Temp High"));
                            }
                            else
                            {
                                lblS_GrdR_AddPumpA.BackColor = GV.CR_X[1];
                                //lblS_GrdL_PumpA.Text = "No Alarm";
                                SetWriteLanguage(lblS_GrdR_AddPumpA.Name, CLang.It.GetLanguage("No Alarm"));
                            }
                        }
                        // 2020.10.22 JSKim Ed

                        // Top Cleaner
                        lblS_GrdR_TcDn.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDR_TopClnDn]];
                        lblS_GrdR_TcFl.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDR_TopClnFlow]];
                        // Grind
                        lblS_GrdR_GrdFl.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDR_SplWater]];
                        lblS_GrdR_GrdTmp.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDL_SplWaterTemp]];
                        lblS_GrdR_BtmFl.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDR_SplBtmWater]];
                        // Spindle
                        lblS_GrdR_PcwFl.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDR_SplPCW]];
                        lblS_GrdR_PcwTmp.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDL_SplPCWTemp]];

                        //20191022 ghk_spindle_type
                        //if (CSpl.It.bUse232)
                        //if (CDataOption.SplType == eSpindleType.Rs232)
                        //{
                        //    lblS_GrdR_Rpm.Text = CData.Spls[1].iRpm.ToString() + " rpm";
                        //    lblS_GrdR_Load.Text = CData.Spls[1].dLoad.ToString("0.0") + " %";
                        //}
                        //else
                        //{
                        //    lblS_GrdR_Rpm.Text = CSpl.It.GetFrpm(false).ToString() + " rpm";
                        //    lblS_GrdR_Load.Text = CData.Spls[1].dLoad.ToString("0.0") + " %"; 
                        //}
                        // 2023.03.15 Max
                        lblS_GrdR_Rpm.Text = CData.Spls[0].iRpm.ToString() + " rpm";
                        lblS_GrdR_Load.Text = CData.Spls[0].dLoad.ToString() + " %";
                        break;

                    case 5:
                        lblS_OffP_Rt0.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFP_Rotate0]];
                        lblS_OffP_Rt90.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFP_Rotate90]];
                        lblS_OffP_Vac.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFP_Vacuum]];
                        lblS_OffP_Lcell.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFP_LoadCell]];
                        // 2021.04.19 lhs Start
                        lblS_OffP_IV2_OK.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFP_IV2_AllOK]];
                        lblS_OffP_IV2_Busy.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFP_IV2_Busy]];
                        lblS_OffP_IV2_Error.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFP_IV2_Error]];
                        //int nIdx = 0;
                        //if (CIO.It.aInput[(int)eX.OFFP_IV2_Error] == 0) nIdx = 1;
                        //lblS_OffP_IV2_Error.BackColor = GV.CR_X[nIdx];
                        // 2021.04.19 lhs End
                        //211005 syc : 2004U
                        lblS_OffP_ClampClose.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFP_Carrier_Clamp_Close]];                        
                        lblS_OffP_ClampOpen .BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFP_Carrier_Clamp_Open]];
                        lblS_OffP_CarrierDetect.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFP_Picker_Carrier_Detect]];
                        //
                        break;

                    case 6:     // Dryer
                        lblS_Dry_POver.BackColor    = GV.CR_X[CIO.It.aInput[(int)eX.DRY_PusherOverload]];
                        lblS_Dry_ODetc.BackColor    = GV.CR_X[CIO.It.aInput[(int)eX.DRY_StripOutDetect]];
                        lblS_Dry_Lv1.BackColor      = GV.CR_X[CIO.It.aInput[(int)eX.DRY_LevelSafety1]];
                        lblS_Dry_Lv2.BackColor      = GV.CR_X[CIO.It.aInput[(int)eX.DRY_LevelSafety2]];
                        lblS_Dry_BtmStd.BackColor   = GV.CR_X[CIO.It.aInput[(int)eX.DRY_BtmStandbyPos]];
                        lblS_Dry_BtmTar.BackColor   = GV.CR_X[CIO.It.aInput[(int)eX.DRY_BtmTargetPos]];
                        lblS_Dry_Fl.BackColor       = GV.CR_X[CIO.It.aInput[(int)eX.DRY_ClnBtmFlow]];
                        // 2022.03.04 lhs Start : SprayBtmCleaner 사용시 표시
                        if (CDataOption.UseSprayBtmCleaner)
                        {
                            lblS_Dry_BtmClnNzlForward.BackColor     = GV.CR_X[CIO.It.aInput[(int)eX.DRY_ClnBtmNzlForward]];
                            lblS_Dry_BtmClnNzlBackward.BackColor    = GV.CR_X[CIO.It.aInput[(int)eX.DRY_ClnBtmNzlBackward]];
                        }
                        // 2022.03.04 lhs End
                        // 2022.07.04 lhs Start : Cylinder Clamp 
                        lblS_Dry_FrontClamp.BackColor   = GV.CR_X[CIO.It.aInput[(int)eX.DRY_Front_Clamp]];
                        lblS_Dry_FrontUnClamp.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.DRY_Front_Unclamp]];
                        lblS_Dry_RearClamp.BackColor    = GV.CR_X[CIO.It.aInput[(int)eX.DRY_Rear_Clamp]];
                        lblS_Dry_RearUnClamp.BackColor  = GV.CR_X[CIO.It.aInput[(int)eX.DRY_Rear_Unclamp]];
                        // 2022.07.04 lhs End

                        break;

                    case 7:    // Off Loader
                               // Door
                        lblS_OffL_FDoor.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFL_DoorFront]];
                        lblS_OffL_RiDoor.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFL_DoorRight]];
                        lblS_OffL_RDoor.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFL_DoorRear]];
                        lblS_OffL_LCurt.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFL_LightCurtain]];
                        // Top Conveyor
                        lblS_OffL_TpDetc.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFL_TopMGZDetect]];
                        lblS_OffL_TpFull.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFL_TopMGZDetectFull]];
                        // Middle Conveyor
                        lblS_OffL_MiDetc.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFL_BtmMGZDetect]];
                        // Bottom Conveyor
                        lblS_OffL_BtDetc.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFL_BtmMGZDetect]];
                        lblS_OffL_BtFull.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFL_BtmMGZDetectFull]];
                        // MGZ Clamp
                        lblS_OffL_TcDetc1.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFL_TopClampMGZDetect1]];
                        lblS_OffL_TcDetc2.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFL_TopClampMGZDetect2]];
                        lblS_OffL_TcClmpOn.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFL_TopClampOn]];
                        lblS_OffL_TcClmpOff.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFL_TopClampOff]];
                        lblS_OffL_TcCylUp.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFL_TopMGZUp]];
                        lblS_OffL_TcCylDn.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFL_TopMGZDn]];
                        // MGZ Clamp
                        lblS_OffL_BtDetc1.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFL_BtmClampMGZDetect1]];
                        lblS_OffL_BtDetc2.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFL_BtmClampMGZDetect2]];
                        lblS_OffL_BtClmpOn.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFL_BtmClampOn]];
                        lblS_OffL_BtClmpOff.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFL_BtmClampOff]];
                        lblS_OffL_BtCylUp.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFL_BtmMGZUp]];
                        lblS_OffL_BtCylDn.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFL_BtmMGZDn]];
                        break;
                }
            }
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

        //190617 ksg : Picker UI Hide
        private void _DisplayBtn()
        {
            label413      .Visible = false; //Picker X BCR
            txtS_OnP_X_Bcr.Visible = false;
            button151     .Visible = false;
            button150     .Visible = false;

            label412      .Visible = false; //Picker X Ori
            txtS_OnP_X_Ori.Visible = false;
            button149     .Visible = false;
            button148     .Visible = false;

            label411      .Visible = false; //Picker Z Ori
            txtS_OnP_Z_Bcr.Visible = false;
            button147     .Visible = false;
            button146     .Visible = false;

            label410      .Visible = false; //Picker Z Ori
            txtS_OnP_Z_Ori.Visible = false;
            button145     .Visible = false;
            button144     .Visible = false;

            label408      .Visible = false; //Picker Y Ori
            txtS_OnP_Y_Bcr.Visible = false;
            button141     .Visible = false;
            button139     .Visible = false;

            label409      .Visible = false; //Picker Y Ori
            txtS_OnP_Y_Ori.Visible = false;
            button143     .Visible = false;
            button142     .Visible = false;
        }

        private void _EnableBtn(bool bVal)
        {
            this.Enabled = bVal;
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
            if (CSQ_Man.It.bBtnShow && (CSQ_Main.It.m_iStat == EStatus.Idle || CSQ_Main.It.m_iStat == EStatus.Stop))
            { _EnableBtn(true); }
            else
            { _EnableBtn(false); }

            Set();

            _setMgzBcrUi(); //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)

            //if (CData.CurCompany != ECompany.Qorvo    && CData.CurCompany != ECompany.Qorvo_DZ && CData.CurCompany != ECompany.SkyWorks &&
            if (CData.CurCompany != ECompany.Qorvo    && CData.CurCompany != ECompany.Qorvo_DZ &&
                CData.CurCompany != ECompany.Qorvo_RT && CData.CurCompany != ECompany.Qorvo_NC &&       // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                CData.CurCompany != ECompany.SkyWorks &&
                CData.CurCompany != ECompany.ASE_K12  && CData.CurCompany != ECompany.ASE_KR   && 
                CData.CurCompany != ECompany.SST      && CData.CurCompany != ECompany.USI      &&
                CData.CurCompany != ECompany.SCK      && CData.CurCompany != ECompany.JSCK     &&
                CData.CurCompany != ECompany.JCET) //191202 ksg : -> 200121 ksg : SCK 추가 //200410 pjh : D/F 사용 업체 추가
            {
                label192.Visible = false;
                label384.Visible = false;
                label388.Visible = false;
                label1.Visible   = false; //20200328 jhc : 다이나믹 펑션 측정 포인트 최대 5개로 확대
                label38.Visible  = false; //20200328 jhc : 다이나믹 펑션 측정 포인트 최대 5개로 확대
                txtS_Inr_X_DynamicPos1.Visible = false;
                txtS_Inr_X_DynamicPos2.Visible = false;
                txtS_Inr_X_DynamicPos3.Visible = false;
                txtS_Inr_X_DynamicPos4.Visible = false; //20200328 jhc : 다이나믹 펑션 측정 포인트 최대 5개로 확대
                txtS_Inr_X_DynamicPos5.Visible = false; //20200328 jhc : 다이나믹 펑션 측정 포인트 최대 5개로 확대
                button126.Visible = false;
                button128.Visible = false;
                button130.Visible = false;
                button166.Visible = false; //20200328 jhc : 다이나믹 펑션 측정 포인트 최대 5개로 확대
                button168.Visible = false; //20200328 jhc : 다이나믹 펑션 측정 포인트 최대 5개로 확대
                button125.Visible = false;
                button127.Visible = false;
                button129.Visible = false;
                button165.Visible = false; //20200328 jhc : 다이나믹 펑션 측정 포인트 최대 5개로 확대
                button167.Visible = false; //20200328 jhc : 다이나믹 펑션 측정 포인트 최대 5개로 확대
            }

            // 2021.02.27 SungTae Start : 
            if(CData.CurCompany == ECompany.ASE_KR  ||
               CData.CurCompany == ECompany.SCK     ||  // 2021.07.14 lhs  SCK, JSCK 추가
               CData.CurCompany == ECompany.JSCK    ||
               CData.CurCompany == ECompany.SkyWorks||  // 220126 pjh : Skyworks 추가
               CDataOption.UseSprayBtmCleaner       )   // 2022.03.08 lhs Spray Nozzle Bottom Cleaner 추가 
            {
                // OffP Z축 Btm Clean
                btnS_Get_OffP_Z_BtClnSt.Visible     = true;
                btnS_Mv_OffP_Z_BtClnSt.Visible      = true;

                lblS_OffP_Z_BtClnSt.ForeColor       = System.Drawing.Color.Black;
                txtS_OffP_Z_BtClnSt.BackColor       = System.Drawing.Color.White;
                txtS_OffP_Z_BtClnSt.ReadOnly        = false;

                // OffP Z축 Strip Clean
                lblS_OffP_Z_StrpClnSt.Visible       = true;     // 2022.03.07 lhs
                txtS_OffP_Z_StrpClnSt.Visible       = true;     // 2022.03.07 lhs
                btnS_Get_OffP_Z_StrpClnSt.Visible   = true;
                btnS_Mv_OffP_Z_StrpClnSt.Visible    = true;
                
                lblS_OffP_Z_StrpClnSt.ForeColor     = System.Drawing.Color.Black;
                txtS_OffP_Z_StrpClnSt.BackColor     = System.Drawing.Color.White;
                txtS_OffP_Z_StrpClnSt.ReadOnly      = false;

            }
            else  // 그외 Site : Btm Clean만 있음, Btm Clean Option에서 설정 (기존 로직)
            {
                // OffP Z축 Btm Clean
                btnS_Get_OffP_Z_BtClnSt.Visible     = false;    // Get 버튼 표시 안함
                btnS_Mv_OffP_Z_BtClnSt.Visible      = false;    // Move 버튼 표시 안함

                lblS_OffP_Z_BtClnSt.ForeColor       = System.Drawing.Color.OrangeRed;
                txtS_OffP_Z_BtClnSt.BackColor       = System.Drawing.Color.Silver;
                txtS_OffP_Z_BtClnSt.ReadOnly        = true;

                // OffP Z축 Strip Clean 표시 안함. 레벨 ~ 버튼 등 모두
                lblS_OffP_Z_StrpClnSt.Visible       = false;    // 2022.03.07 lhs
                txtS_OffP_Z_StrpClnSt.Visible       = false;    // 2022.03.07 lhs
                btnS_Get_OffP_Z_StrpClnSt.Visible   = false;
                btnS_Mv_OffP_Z_StrpClnSt.Visible    = false;

                //lblS_OffP_Z_StrpClnSt.ForeColor     = System.Drawing.Color.OrangeRed; // 2022.03.08 lhs
                //txtS_OffP_Z_StrpClnSt.BackColor     = System.Drawing.Color.Silver;    // 2022.03.08 lhs
                //txtS_OffP_Z_StrpClnSt.ReadOnly      = true;                           // 2022.03.08 lhs
            }
            // 2021.02.27 SungTae End

            // 2022.03.08 lhs Start : Spray Nozzle형 Btm Cleaner일 경우 X축은 Start Position만 사용, 
            if(CDataOption.UseSprayBtmCleaner)
            {
                lblS_OffP_X_BtClnEd.Visible         = false;    // X축 End Position은 안 보이게..
                txtS_OffP_X_BtClnEd.Visible         = false;
                btnS_Get_OffP_X_BtClnEd.Visible     = false;
                btnS_Move_OffP_X_BtClnEd.Visible    = false;
            }

            // 2022.07.28 lhs Start
            bool bUseBrush = CDataOption.UseBrushBtmCleaner;
            // Brush X Start
            lblS_OffP_X_BtClnSt_Brush.Visible       = bUseBrush;    
            txtS_OffP_X_BtClnSt_Brush.Visible       = bUseBrush;
            btnS_Get_OffP_X_BtClnSt_Brush.Visible   = bUseBrush;
            btnS_Move_OffP_X_BtClnSt_Brush.Visible  = bUseBrush;
            // Brush X End
            lblS_OffP_X_BtClnEd_Brush.Visible       = bUseBrush;
            txtS_OffP_X_BtClnEd_Brush.Visible       = bUseBrush;
            btnS_Get_OffP_X_BtClnEd_Brush.Visible   = bUseBrush;
            btnS_Move_OffP_X_BtClnEd_Brush.Visible  = bUseBrush;
            // Brush Z
            lblS_OffP_Z_StrpClnSt_Brush.Visible     = bUseBrush;
            txtS_OffP_Z_StrpClnSt_Brush.Visible     = bUseBrush;
            btnS_Get_OffP_Z_StrpClnSt_Brush.Visible = bUseBrush;
            btnS_Mv_OffP_Z_StrpClnSt_Brush.Visible  = bUseBrush;
            // 2022.07.28 lhs End

            // 2021.03.18 SungTae Start
            if (CData.CurCompany == ECompany.ASE_KR)
            {
                lblS_OffP_X_BtClnCenter.Visible = true;
                txtS_OffP_X_BtClnCenter.Visible = true;
                btnS_OffP_X_BtClnCenter.Visible = true;
            }
            else
            {
                lblS_OffP_X_BtClnCenter.Visible = false;
                txtS_OffP_X_BtClnCenter.Visible = false;
                btnS_OffP_X_BtClnCenter.Visible = false;
            }

            //20191029 ghk_dfserver_notuse_df
            if (CDataOption.eDfserver == eDfserver.Use && CDataOption.MeasureDf == eDfServerType.NotMeasureDf)
            {//DF서버 사용시 DF측정 안할 경우
                label192.Visible = false;
                label384.Visible = false;
                label388.Visible = false;
                label1.Visible   = false; //20200328 jhc : 다이나믹 펑션 측정 포인트 최대 5개로 확대
                label38.Visible  = false; //20200328 jhc : 다이나믹 펑션 측정 포인트 최대 5개로 확대
                txtS_Inr_X_DynamicPos1.Visible = false;
                txtS_Inr_X_DynamicPos2.Visible = false;
                txtS_Inr_X_DynamicPos3.Visible = false;
                txtS_Inr_X_DynamicPos4.Visible = false; //20200328 jhc : 다이나믹 펑션 측정 포인트 최대 5개로 확대
                txtS_Inr_X_DynamicPos5.Visible = false; //20200328 jhc : 다이나믹 펑션 측정 포인트 최대 5개로 확대
                button126.Visible = false;
                button128.Visible = false;
                button130.Visible = false;
                button166.Visible = false; //20200328 jhc : 다이나믹 펑션 측정 포인트 최대 5개로 확대
                button168.Visible = false; //20200328 jhc : 다이나믹 펑션 측정 포인트 최대 5개로 확대
                button125.Visible = false;
                button127.Visible = false;
                button129.Visible = false;
                button165.Visible = false; //20200328 jhc : 다이나믹 펑션 측정 포인트 최대 5개로 확대
                button167.Visible = false; //20200328 jhc : 다이나믹 펑션 측정 포인트 최대 5개로 확대
            }

            if(CDataOption.CurEqu == eEquType.Pikcer)
            {//device inrail X Pos Bar / ori 안씀
                label34       .Visible = false;
                label380      .Visible = false;
                label35       .Visible = false;
                txtS_Inr_X_Bcr.Visible = false;
                txtS_Inr_X_Ori.Visible = false;
                txtS_Inr_X_Vis.Visible = false;
                button25      .Visible = false;
                button123     .Visible = false;
                button27      .Visible = false;
                button24      .Visible = false;
                button124     .Visible = false;
                button26      .Visible = false;
            }

            // 2022.01.18 SungTae Start : [추가] (ASE-KR VOC) Strip 유무 체크 시작 위치를 별도로 설정할 있도록 고객사 요청 사항
            if (CData.CurCompany == ECompany.ASE_KR && 
                CData.CurCompanyName != "INARI")  // 2022.01.18 lhs : Inari 제외
            {
                label101        .Visible = true;
                txtS_Dry_Z_ChkStrip.Visible = true;
                button7         .Visible = true;
            }
            else
            {
                label101        .Visible = false;
                txtS_Dry_Z_ChkStrip.Visible = false;
                button7         .Visible = false;
            }
            // 2022.01.18 SungTae End

            //190617 ksg :
            if (CDataOption.CurEqu == eEquType.Nomal)
            {
                _DisplayBtn();
            }

            tabSetPart.SelectTab(0);
            rdbS_1000.Checked = true;
            m_sStep = "1";
            txtS_Cus.Text = "0";

            m_iAx1 = (int)EAx.OnLoader_X;
            m_iAx2 = (int)EAx.OnLoader_Y;
            m_iAx3 = (int)EAx.OnLoader_Z;
            m_bAx3 = true;

            SetWriteLanguage(btnS_MvN1.Name, CLang.It.GetLanguage("LEFT"));
            SetWriteLanguage(btnS_MvP1.Name, CLang.It.GetLanguage("RIGHT"));
            SetWriteLanguage(btnS_MvN2.Name, CLang.It.GetLanguage("FRONT"));
            SetWriteLanguage(btnS_MvP2.Name, CLang.It.GetLanguage("REAR"));
            SetWriteLanguage(btnS_MvN3.Name, CLang.It.GetLanguage("UP"));
            SetWriteLanguage(btnS_MvP3.Name, CLang.It.GetLanguage("DOWN"));

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
        /// 데이터를 화면에 출력
        /// </summary>
        public void Set()
        {
            // On Loader X
            // 대기 포지션
            txtS_OnL_X_Wait.Text = CData.SPos.dONL_X_Wait.ToString();
            txtS_OnL_X_Align.Text =CData.Dev.dOnL_X_Algn.ToString();

            // On Loader Y
            // 대기 위치
            txtS_OnL_Y_Wait.Text = CData.Dev.dOnL_Y_Wait.ToString();
            // Pick 포지션
            txtS_OnL_Y_Pick.Text = CData.SPos.dONL_Y_Pick.ToString();
            // Place 포지션
            txtS_OnL_Y_Place.Text = CData.SPos.dONL_Y_Place.ToString();
            // Magazine Barcode 포지션
            txtS_OnL_Y_MgzBcr.Text = CData.Dev.dOnL_Y_MgzBcr.ToString(); //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)
            //

            // On Loader Z
            txtS_OnL_Z_Wait   .Text = CData.SPos.dONL_Z_Wait    .ToString();
            txtS_OnL_Z_PickGo .Text = CData.SPos.dONL_Z_PickGo  .ToString();
            txtS_OnL_Z_Clamp  .Text = CData.SPos.dONL_Z_Clamp   .ToString();
            txtS_OnL_Z_PickUp .Text = CData.SPos.dONL_Z_PickUp  .ToString();
            txtS_OnL_Z_PlaceGo.Text = CData.SPos.dONL_Z_PlaceGo .ToString();
            txtS_OnL_Z_Unclamp.Text = CData.SPos.dONL_Z_UnClamp .ToString();
            txtS_OnL_Z_PlaceDn.Text = CData.SPos.dONL_Z_PlaceDn .ToString();
            txtS_OnL_Z_EntryUp.Text = CData.Dev .dOnL_Z_Entry_Up.ToString();
            txtS_OnL_Z_EntryDn.Text = CData.Dev .dOnL_Z_Entry_Dn.ToString();
            // Magazine Barcode 포지션
            txtS_OnL_Z_MgzBcr.Text  = CData.Dev .dOnL_Z_MgzBcr  .ToString(); //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)
            //

            // Inrail X
            txtS_Inr_X_Wait .Text = CData.SPos.dINR_X_Wait .ToString();
            txtS_Inr_X_Pick .Text = CData.Dev.dInr_X_Pick  .ToString();
            txtS_Inr_X_Bcr  .Text = CData.Dev.dInr_X_Bcr   .ToString();
            txtS_Inr_X_Ori  .Text = CData.Dev.dInr_X_Ori   .ToString();//190211 ksg :
            txtS_Inr_X_Vis  .Text = CData.Dev.dInr_X_Vision.ToString();
            txtS_Inr_X_Align.Text = CData.Dev.dInr_X_Align .ToString();
            //20190220 ghk_dynamicfunction
            txtS_Inr_X_DynamicPos1.Text = CData.Dev.dInr_X_DynamicPos1.ToString();
            txtS_Inr_X_DynamicPos2.Text = CData.Dev.dInr_X_DynamicPos2.ToString();
            txtS_Inr_X_DynamicPos3.Text = CData.Dev.dInr_X_DynamicPos3.ToString();
            //20200328 jhc : 다이나믹 펑션 측정 포인트 최대 5개로 확대
            txtS_Inr_X_DynamicPos4.Text = CData.Dev.dInr_X_DynamicPos4.ToString();
            txtS_Inr_X_DynamicPos5.Text = CData.Dev.dInr_X_DynamicPos5.ToString();
            //

            // InRail Y
            txtS_Inr_Y_Wait .Text = CData.SPos.dINR_Y_Wait .ToString();
            txtS_Inr_Y_Align.Text = CData.Dev .dInr_Y_Align.ToString();

            // On Loader Picker X
            txtS_OnP_X_Wait  .Text = CData.Dev .dOnP_X_Wait  .ToString();
            txtS_OnP_X_PlaceL.Text = CData.SPos.dONP_X_PlaceL.ToString();
            txtS_OnP_X_PlaceR.Text = CData.SPos.dONP_X_PlaceR.ToString();
            txtS_OnP_X_Conv  .Text = CData.SPos.dONP_X_Con   .ToString(); //190321 ksg :
            txtS_OnP_X_WaitPickL.Text = CData.SPos.dONP_X_WaitPickL.ToString(); //20200406 jhc : OnPicker L-Table Pickup 대기 위치
            //20190611 ghk_onpclean
            txtS_OnP_X_Clean.Text = CData.Dev.dOnp_X_Clean.ToString();
            //
            //20190604 ghk_onpbcr
            txtS_OnP_X_Bcr    .Text = CData.Dev.dOnp_X_Bcr    .ToString();
            txtS_OnP_X_Ori    .Text = CData.Dev.dOnp_X_Ori    .ToString();
            txtS_OnP_X_BcrSecon.Text = CData.Dev.dOnp_X_BcrSecon.ToString(); //190821 ksg :
            //201230 jhc : 2D 코드 판독 실패 Alarm 발생 전 제3의 위치(OCR 위치)로 카메라 이동 기능
            txtS_OnP_X_BcrErr.Text = CData.Dev.dOnp_X_BcrErr.ToString();
            //
            //20191010 ghk_manual_bcr
            txtS_OnP_X_Bcr_TbL.Text = CData.Dev.dOnp_X_Bcr_TbL.ToString();
            txtS_OnP_X_Ori_TbL.Text = CData.Dev.dOnp_X_Ori_TbL.ToString();
            txtS_OnP_X_Bcr_TbR.Text = CData.Dev.dOnp_X_Bcr_TbR.ToString();
            txtS_OnP_X_Ori_TbR.Text = CData.Dev.dOnp_X_Ori_TbR.ToString();
            //

            // On Loader Picker Z
            txtS_OnP_Z_Wait.Text = CData.SPos.dONP_Z_Wait .ToString();
            txtS_OnP_Z_Pick .Text = CData.Dev.dOnP_Z_Pick .ToString();
            txtS_OnP_Z_Place.Text = CData.Dev.dOnP_Z_Place.ToString();
            txtS_OnP_Z_Slow .Text = CData.Dev.dOnP_Z_Slow .ToString();
            //20190611 ghk_onpclean
            txtS_OnP_Z_Clean.Text = CData.Dev.dOnp_Z_Clean.ToString();
            //
            //20190604 ghk_onpbcr
            txtS_OnP_Z_Bcr.Text = CData.Dev.dOnp_Z_Bcr.ToString();
            txtS_OnP_Z_Ori.Text = CData.Dev.dOnp_Z_Ori.ToString();
            //201230 jhc : 2D 코드 판독 실패 Alarm 발생 전 제3의 위치(OCR 위치)로 카메라 이동 기능
            txtS_OnP_Z_BcrErr.Text = CData.Dev.dOnp_Z_BcrErr.ToString();
            //
            //20191010 ghk_manual_bcr
            txtS_OnP_Z_Bcr_TbL.Text = CData.Dev.dOnp_Z_Bcr_TbL.ToString();
            txtS_OnP_Z_Ori_TbL.Text = CData.Dev.dOnp_Z_Ori_TbL.ToString();
            txtS_OnP_Z_Bcr_TbR.Text = CData.Dev.dOnp_Z_Bcr_TbR.ToString();
            txtS_OnP_Z_Ori_TbR.Text = CData.Dev.dOnp_Z_Ori_TbR.ToString();
            //
            //211022 syc : Onp Pick Up Offset
            txtS_Onp_Z_PickOffset.Text = CData.Dev.dOnp_Z_PickOffset.ToString();
            //

            //20190604 ghk_onpbcr
            // On Loader Picker Y
            txtS_OnP_Y_Wait.Text = CData.SPos.dONP_Y_Wait    .ToString();
            txtS_OnP_Y_Bcr     .Text = CData.Dev .dOnp_Y_Bcr     .ToString();
            txtS_OnP_Y_Ori     .Text = CData.Dev .dOnp_Y_Ori     .ToString();
            txtS_OnP_Y_BcrSecon.Text = CData.Dev .dOnp_Y_BcrSecon.ToString();//190821 ksg :
            //201230 jhc : 2D 코드 판독 실패 Alarm 발생 전 제3의 위치(OCR 위치)로 카메라 이동 기능
            txtS_OnP_Y_BcrErr.Text = CData.Dev.dOnp_Y_BcrErr.ToString();
            //
            //20191010 ghk_manual_bcr
            txtS_OnP_Y_Bcr_TbL.Text = CData.Dev.dOnp_Y_Bcr_TbL.ToString();
            txtS_OnP_Y_Ori_TbL.Text = CData.Dev.dOnp_Y_Ori_TbL.ToString();
            txtS_OnP_Y_Bcr_TbR.Text = CData.Dev.dOnp_Y_Bcr_TbR.ToString();
            txtS_OnP_Y_Ori_TbR.Text = CData.Dev.dOnp_Y_Ori_TbR.ToString();
            //

            // Grind Left X
            txtS_GrdL_X_Wait.Text = CData.SPos.dGRD_X_Wait[0].ToString();
            txtS_GrdL_X_Zero.Text = CData.SPos.dGRD_X_Zero[0].ToString();

            // Grind Left Y
            txtS_GrdL_Y_Wait   .Text = CData.SPos.dGRD_Y_Wait    [0].ToString();
            txtS_GrdL_Y_ClnSt  .Text = CData.SPos.dGRD_Y_ClnStart[0].ToString();
            txtS_GrdL_Y_ClnEd  .Text = CData.SPos.dGRD_Y_ClnEnd  [0].ToString();
            txtS_GrdL_Y_DrsSt  .Text = CData.SPos.dGRD_Y_DrsStart[0].ToString();
            txtS_GrdL_Y_DrsEd  .Text = CData.SPos.dGRD_Y_DrsEnd  [0].ToString();
            txtS_GrdL_Y_GrdSt  .Text = CData.Dev .aGrd_Y_Start   [0].ToString();
            txtS_GrdL_Y_GrdEd  .Text = CData.Dev .aGrd_Y_End     [0].ToString();
            txtS_GrdL_Y_TblInsp.Text = CData.SPos.dGRD_Y_TblInsp [0].ToString();
            txtS_GrdL_Y_WhlInsp.Text = (CData.MPos[0].dY_WHL_TOOL_SETTER 
                                                            - (GV.WHEEL_DEF_OUTER / 2.0) 
                                                            + (GV.WHEEL_DEF_TIP_W / 2.0)).ToString();
            txtS_GrdL_Y_DrsInsp  .Text = CData.MPos[0].dY_PRB_DRS       .ToString();
            txtS_GrdL_Y_PrbTblCen.Text = CData.MPos[0].dY_PRB_TBL_CENTER.ToString();
            txtS_GrdL_Y_WhlTblCen.Text = CData.MPos[0].dY_WHL_TBL_CENTER.ToString();
            txtS_GrdL_Y_DrsCh.Text = CData.SPos.dGRD_Y_DrsChange[0].ToString();
            //20191011 ghk_manual_bcr
            txtS_GrdL_Y_Ori.Text = CData.Dev.aGrd_Y_Ori[0].ToString();
            //

            // Grind Left Z
            txtS_GrdL_Z_Wait.Text = CData.SPos.dGRD_Z_Wait[0].ToString();
            txtS_GrdL_Z_Able.Text = CData.SPos.dGRD_Z_Able[0].ToString();

            // Grind Right X
            txtS_GrdR_X_Wait.Text = CData.SPos.dGRD_X_Wait[1].ToString();
            txtS_GrdR_X_Zero.Text = CData.SPos.dGRD_X_Zero[1].ToString();

            // Grind Right Y
            txtS_GrdR_Y_Wait   .Text = CData.SPos.dGRD_Y_Wait    [1].ToString();
            txtS_GrdR_Y_ClnSt  .Text = CData.SPos.dGRD_Y_ClnStart[1].ToString();
            txtS_GrdR_Y_ClnEd  .Text = CData.SPos.dGRD_Y_ClnEnd  [1].ToString();
            txtS_GrdR_Y_DrsSt  .Text = CData.SPos.dGRD_Y_DrsStart[1].ToString();
            txtS_GrdR_Y_DrsEd  .Text = CData.SPos.dGRD_Y_DrsEnd  [1].ToString();
            txtS_GrdR_Y_GrdSt  .Text = CData.Dev .aGrd_Y_Start   [1].ToString();
            txtS_GrdR_Y_GrdEd  .Text = CData.Dev .aGrd_Y_End     [1].ToString();
            txtS_GrdR_Y_TblInsp.Text = CData.SPos.dGRD_Y_TblInsp [1].ToString();
            txtS_GrdR_Y_WhlInsp.Text = (CData.MPos[1].dY_WHL_TOOL_SETTER
                                                            - (GV.WHEEL_DEF_OUTER / 2.0)
                                                            + (GV.WHEEL_DEF_TIP_W / 2.0)).ToString();
            txtS_GrdR_Y_DrsInsp  .Text = CData.MPos[1].dY_PRB_DRS       .ToString();
            txtS_GrdR_Y_PrbTblCen.Text = CData.MPos[1].dY_PRB_TBL_CENTER.ToString();
            txtS_GrdR_Y_WhlTblCen.Text = CData.MPos[1].dY_WHL_TBL_CENTER.ToString();
            txtS_GrdR_Y_DrsCh.Text = CData.SPos.dGRD_Y_DrsChange[1].ToString();
            //20191011 ghk_manual_bcr
            txtS_GrdR_Y_Ori.Text = CData.Dev.aGrd_Y_Ori[1].ToString();
            //

            // Grind Right Z
            txtS_GrdR_Z_Wait.Text = CData.SPos.dGRD_Z_Wait[1].ToString();
            txtS_GrdR_Z_Able.Text = CData.SPos.dGRD_Z_Able[1].ToString();

			// Off Loader Picker X
			txtS_OffP_X_Wait.Text           = CData.SPos.dOFP_X_Wait.ToString();
			txtS_OffP_X_PickL.Text          = CData.SPos.dOFP_X_PickL.ToString();
			txtS_OffP_X_PickR.Text          = CData.SPos.dOFP_X_PickR.ToString();
			txtS_OffP_X_Place.Text          = CData.SPos.dOFP_X_Place.ToString();
			txtS_OffP_X_BtClnSt.Text        = CData.Dev.dOffP_X_ClnStart.ToString();
			txtS_OffP_X_BtClnEd.Text        = CData.Dev.dOffP_X_ClnEnd.ToString();
            txtS_OffP_X_BtClnSt_Brush.Text  = CData.Dev.dOffP_X_ClnStart_Brush.ToString();  // 2022.07.27 lhs : Brush X 시작 위치
            txtS_OffP_X_BtClnEd_Brush.Text  = CData.Dev.dOffP_X_ClnEnd_Brush.ToString();    // 2022.07.27 lhs : Brush X 끝 위치
            txtS_OffP_X_Conv.Text           = CData.SPos.dOFP_X_Conv.ToString();            // 190321 ksg :
			txtS_OffP_X_Picture1.Text       = CData.SPos.dOFP_X_Picture1.ToString();        // 2021.03.30 lhs
			txtS_OffP_X_Picture2.Text       = CData.SPos.dOFP_X_Picture2.ToString();        // 2021.03.30 lhs

			// 2021.03.18 SungTae Start : 고객사(ASE-KR) 요청으로 Center Position 추가
			if (CData.CurCompany == ECompany.ASE_KR)
            {
                CData.Dev.dOffP_X_ClnCenter     = CData.Dev.dOffP_X_ClnStart - (CData.Dev.dOffP_X_ClnStart - CData.Dev.dOffP_X_ClnEnd) / 2;
                txtS_OffP_X_BtClnCenter.Text    = CData.Dev.dOffP_X_ClnCenter.ToString();
            }

            // Off Loader Picker Z
            txtS_OffP_Z_Wait   .Text = CData.SPos.dOFP_Z_Wait    .ToString();
            txtS_OffP_Z_Pick   .Text = CData.Dev .dOffP_Z_Pick   .ToString();
            txtS_OffP_Z_Place  .Text = CData.Dev .dOffP_Z_Place  .ToString();
            txtS_OffP_Z_Slow   .Text = CData.Dev .dOffP_Z_Slow   .ToString();
            txtS_OffP_Z_DryGap .Text = CData.Dev .dOffP_Z_PlaceDn.ToString();
            // 2021.02.27 SungTae Start : 고객사(ASE-KR) 요청으로 Device에서 관리 위해 변경
            if (CData.CurCompany == ECompany.ASE_KR     ||
                CData.CurCompany == ECompany.SCK        ||  // 2021.07.14 lhs  SCK, JSCK 추가
                CData.CurCompany == ECompany.JSCK       ||
                CData.CurCompany == ECompany.SkyWorks   ||  // 220216 pjh : Skyworks 추가
                CDataOption.UseSprayBtmCleaner          )   // 2022.03.08 lhs Spray Nozzle Bottom Cleaner 추가 
            {
                txtS_OffP_Z_BtClnSt.Text    = CData.Dev.dOffP_Z_ClnStart.ToString();
                txtS_OffP_Z_StrpClnSt.Text  = CData.Dev.dOffP_Z_StripClnStart.ToString(); //20200424 jhc : Off-Picker Z축 Strip Clean 높이 별도 설정
            }
            else
            {
                txtS_OffP_Z_BtClnSt.Text    = CData.SPos.dOFP_Z_ClnStart.ToString();
                // 표시안함 //txtS_OffP_Z_StrpClnSt.Text  = CData.SPos.dOFP_Z_StripClnStart.ToString(); // lhs 삭제 예정  //20200424 jhc : Off-Picker Z축 Strip Clean 높이 별도 설정
            }
            // 2021.02.27 SungTae End

            txtS_OffP_Z_StrpClnSt_Brush.Text = CData.Dev.dOffP_Z_StripClnStart_Brush.ToString();   // 2022.07.28 lhs : Brush Z 위치    

            txtS_OffP_Z_Picture.Text = CData.SPos.dOFP_Z_Picture.ToString();    // 2021.03.30 lhs : // IV Z 위치

            // Dry X
            txtS_Dry_X_Wait.Text = CData.SPos.dDRY_X_Wait.ToString();
            txtS_Dry_X_Out .Text = CData.SPos.dDRY_X_Out .ToString();

            // Dry Z
            txtS_Dry_Z_Wait .Text = CData.SPos.dDRY_Z_Wait .ToString();
            txtS_Dry_Z_Up   .Text = CData.SPos.dDRY_Z_Up   .ToString();
            txtS_Dry_Z_Level.Text = CData.SPos.dDRY_Z_Check.ToString();
            CData.SPos.dDRY_Z_TipClamped = CData.SPos.dDRY_Z_Up - CData.Dev.dOffP_Z_PlaceDn; // 2021.04.22 lhs
            txtS_Dry_Z_TipClamped.Text = CData.SPos.dDRY_Z_TipClamped.ToString();                 // 2021.04.22 lhs
            txtS_Dry_Z_ChkStrip.Text = CData.SPos.dDRY_Z_ChkStrip.ToString();     // 2022.01.17 SungTae : [추가] (ASE-KR VOC) Strip 유무 체크 시작 위치를 별도로 설정할 있도록 고객사 요청 사항

            // Dry R
            txtS_Dry_R_Wait.Text = CData.SPos.dDRY_R_Wait  .ToString();
            txtS_Dry_R_Lv1 .Text = CData.Dev .dDry_R_Check1.ToString();
            txtS_Dry_R_Lv2 .Text = CData.Dev .dDry_R_Check2.ToString();
            // 200312-jym : Unit 방식 포지션 추가
            txtS_Dry_R_Carr.Text = CData.Dev.dDry_Car_Check.ToString();
            txtS_Dry_R_Unit.Text = CData.Dev.dDry_Unit_Check.ToString();
            txtS_Dry_R_Start.Text = CData.Dev.dDry_Start.ToString();
            txtS_Dry_R_End.Text = CData.Dev.dDry_End.ToString();

            // Off Loader X
            txtS_OffL_X_Wait .Text = CData.SPos.dOFL_X_Wait .ToString();
            txtS_OffL_X_Align.Text = CData.Dev .dOffL_X_Algn.ToString();

            // Off Loader Y
            txtS_OffL_Y_Wait .Text = CData.Dev .dOffL_Y_Wait.ToString();
            txtS_OffL_Y_Pick .Text = CData.SPos.dOFL_Y_Pick .ToString();
            txtS_OffL_Y_Place.Text = CData.SPos.dOFL_Y_Place.ToString();
            // Magazine Barcode 포지션
            txtS_OffL_Y_MgzBcr.Text = CData.Dev.dOffL_Y_MgzBcr.ToString(); //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)
            //

            // Off Loader Z
            txtS_OffL_Z_Wait     .Text = CData.SPos.dOFL_Z_Wait    .ToString();
            txtS_OffL_Z_TpRcvUp  .Text = CData.Dev .dOffL_Z_TRcv_Up.ToString();
            txtS_OffL_Z_TpRcvDn  .Text = CData.Dev .dOffL_Z_TRcv_Dn.ToString();
            txtS_OffL_Z_BtRcvUp  .Text = CData.Dev .dOffL_Z_BRcv_Up.ToString();
            txtS_OffL_Z_BtRcvDn  .Text = CData.Dev .dOffL_Z_BRcv_Dn.ToString();
            txtS_OffL_Z_TcPickGo .Text = CData.SPos.dOFL_Z_TPickGo .ToString();
            txtS_OffL_Z_TcClamp  .Text = CData.SPos.dOFL_Z_TClamp  .ToString();
            txtS_OffL_Z_TcPickUp .Text = CData.SPos.dOFL_Z_TPickUp .ToString();
            txtS_OffL_Z_TcPlaceGo.Text = CData.SPos.dOFL_Z_TPlaceGo.ToString();
            txtS_OffL_Z_TcUnclamp.Text = CData.SPos.dOFL_Z_TUnClamp.ToString();
            txtS_OffL_Z_TcPlaceDn.Text = CData.SPos.dOFL_Z_TPlaceDn.ToString();
            txtS_OffL_Z_BtPickGo .Text = CData.SPos.dOFL_Z_BPickGo .ToString();
            txtS_OffL_Z_BtClamp  .Text = CData.SPos.dOFL_Z_BClamp  .ToString();
            txtS_OffL_Z_BtPickUp .Text = CData.SPos.dOFL_Z_BPickUp .ToString();
            txtS_OffL_Z_BtPlaceGo.Text = CData.SPos.dOFL_Z_BPlaceGo.ToString();
            txtS_OffL_Z_BtUnclamp.Text = CData.SPos.dOFL_Z_BUnClamp.ToString();
            txtS_OffL_Z_BtPlaceDn.Text = CData.SPos.dOFL_Z_BPlaceDn.ToString();
            // Magazine Barcode 포지션
            txtS_OffL_Z_MgzBcr.Text = CData.Dev.dOffL_Z_MgzBcr.ToString(); //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)
                                                                           //
            // 2020.10.22 JSKim St
            if (CData.CurCompany == ECompany.JCET && CData.Opt.bDualPumpUse == true)
            {
                lblS_GrdL_PumpR.Text = "Pump 1 Run";
                lblS_GrdL_PumpA.Text = "Pump 1 Alarm";
                lblS_GrdL_AddPumpR.Text = "Pump 2 Run";
                lblS_GrdL_AddPumpA.Text = "Pump 2 Alarm";
                lblS_GrdR_PumpR.Text = "Pump 1 Run";
                lblS_GrdR_PumpA.Text = "Pump 1 Alarm";
                lblS_GrdR_AddPumpR.Text = "Pump 2 Run";
                lblS_GrdR_AddPumpA.Text = "Pump 2 Alarm";

                lblS_GrdL_AddPumpR.Visible = true;
                lblS_GrdL_AddPumpA.Visible = true;
                lblS_GrdR_AddPumpR.Visible = true;
                lblS_GrdR_AddPumpA.Visible = true;
            }
            else
            {
                lblS_GrdL_PumpR.Text = "Pump Run";
                lblS_GrdL_PumpA.Text = "Pump Alarm";
                lblS_GrdR_PumpR.Text = "Pump Run";
                lblS_GrdR_PumpA.Text = "Pump Alarm";

                lblS_GrdL_AddPumpR.Visible = false;
                lblS_GrdL_AddPumpA.Visible = false;
                lblS_GrdR_AddPumpR.Visible = false;
                lblS_GrdR_AddPumpA.Visible = false;
            }
            // 2020.10.22 JSKim Ed
        }

        public void Get()
        {
            double dVal = 0;
            double dPos = 0;

            // On Loader X
            // 현재 스트립 높이 - 기준 스트립 높이
            dVal = CData.Dev.dLnSide - GV.STRIP_DEF_LONG;
            // 기준 스트립 얼라인 포지션 - 기준과 현재의 스트립 차이 값
            dVal = CData.SPos.dONL_X_DefAlign - dVal;
            CData.Dev.dOnL_X_Algn = dVal;

            // On Loader Y
            double.TryParse(txtS_OnL_Y_Wait.Text, out dPos);
            CData.Dev.dOnL_Y_Wait = dPos;
            //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)
            double.TryParse(txtS_OnL_Y_MgzBcr.Text, out dPos);
            CData.Dev.dOnL_Y_MgzBcr = dPos;
            //

            //On Loader Z
            double.TryParse(txtS_OnL_Z_EntryDn.Text, out dPos);
            CData.Dev.dOnL_Z_Entry_Dn = dPos;
            // 스트립 배출 다운 포지션 + (각 메거진 피치 사이즈 * (매거진 내에 피치 갯수 - 1))
            dVal = dPos + (CData.Dev.dMgzPitch * (CData.Dev.iMgzCnt - 1));
            CData.Dev.dOnL_Z_Entry_Up = dVal;
            //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)
            double.TryParse(txtS_OnL_Z_MgzBcr.Text, out dPos);
            CData.Dev.dOnL_Z_MgzBcr = dPos;
            //

            // Inrail X
            double.TryParse(txtS_Inr_X_Pick.Text, out dPos);
            CData.Dev.dInr_X_Pick = dPos;
            double.TryParse(txtS_Inr_X_Bcr.Text, out dPos);
            CData.Dev.dInr_X_Bcr = dPos;
            double.TryParse(txtS_Inr_X_Ori.Text, out dPos); //190211 ksg :
            CData.Dev.dInr_X_Ori = dPos;
            double.TryParse(txtS_Inr_X_Vis.Text, out dPos);
            CData.Dev.dInr_X_Vision = dPos;
            double.TryParse(txtS_Inr_X_Align.Text, out dPos);
            CData.Dev.dInr_X_Align = dPos;
            //20190220 ghk_dynamicfunction
            double.TryParse(txtS_Inr_X_DynamicPos1.Text, out dPos);
            CData.Dev.dInr_X_DynamicPos1 = dPos;
            double.TryParse(txtS_Inr_X_DynamicPos2.Text, out dPos);
            CData.Dev.dInr_X_DynamicPos2 = dPos;
            double.TryParse(txtS_Inr_X_DynamicPos3.Text, out dPos);
            CData.Dev.dInr_X_DynamicPos3 = dPos;
            //20200328 jhc : 다이나믹 펑션 측정 포인트 최대 5개로 확대
            double.TryParse(txtS_Inr_X_DynamicPos4.Text, out dPos);
            CData.Dev.dInr_X_DynamicPos4 = dPos;
            double.TryParse(txtS_Inr_X_DynamicPos5.Text, out dPos);
            CData.Dev.dInr_X_DynamicPos5 = dPos;
            //

            // Inrail Y
            double.TryParse(txtS_Inr_Y_Align.Text, out dPos);
            CData.Dev.dInr_Y_Align = dPos;
            //

            // On Loader Picker X
            double.TryParse(txtS_OnP_X_Wait.Text, out dPos);
            CData.Dev.dOnP_X_Wait = dPos;
            //20190611 ghk_onpclean
            double.TryParse(txtS_OnP_X_Clean.Text, out dPos);
            CData.Dev.dOnp_X_Clean = dPos;
            //
            //20190604 ghk_onpbcr
            double.TryParse(txtS_OnP_X_Bcr.Text    , out dPos);
            CData.Dev.dOnp_X_Bcr = dPos;
            double.TryParse(txtS_OnP_X_Ori.Text    , out dPos);
            CData.Dev.dOnp_X_Ori = dPos;
            double.TryParse(txtS_OnP_X_BcrSecon.Text, out dPos); //190821 ksg:
            CData.Dev.dOnp_X_BcrSecon = dPos;
            //201230 jhc : 2D 코드 판독 실패 Alarm 발생 전 제3의 위치(OCR 위치)로 카메라 이동 기능
            double.TryParse(txtS_OnP_X_BcrErr.Text, out dPos);
            CData.Dev.dOnp_X_BcrErr = dPos;
            //
            //20191010 ghk_manual_bcr
            double.TryParse(txtS_OnP_X_Bcr_TbL.Text, out dPos);
            CData.Dev.dOnp_X_Bcr_TbL = dPos;
            double.TryParse(txtS_OnP_X_Ori_TbL.Text, out dPos);
            CData.Dev.dOnp_X_Ori_TbL = dPos;
            double.TryParse(txtS_OnP_X_Bcr_TbR.Text, out dPos);
            CData.Dev.dOnp_X_Bcr_TbR = dPos;
            double.TryParse(txtS_OnP_X_Ori_TbR.Text, out dPos);
            CData.Dev.dOnp_X_Ori_TbR = dPos;
            //

            // On Loader Picker Z
            double.TryParse(txtS_OnP_Z_Pick.Text, out dPos);
            CData.Dev.dOnP_Z_Pick = dPos;
            double.TryParse(txtS_OnP_Z_Place.Text, out dPos);
            CData.Dev.dOnP_Z_Place = dPos;
            double.TryParse(txtS_OnP_Z_Slow.Text, out dPos);
            CData.Dev.dOnP_Z_Slow = dPos;
            //20190611 ghk_onpclean
            double.TryParse(txtS_OnP_Z_Clean.Text, out dPos);
            CData.Dev.dOnp_Z_Clean = dPos;
            //
            //20190604 ghk_onpbcr
            double.TryParse(txtS_OnP_Z_Bcr.Text, out dPos);
            CData.Dev.dOnp_Z_Bcr = dPos;
            double.TryParse(txtS_OnP_Z_Ori.Text, out dPos);
            CData.Dev.dOnp_Z_Ori = dPos;
            //201230 jhc : 2D 코드 판독 실패 Alarm 발생 전 제3의 위치(OCR 위치)로 카메라 이동 기능
            double.TryParse(txtS_OnP_Z_BcrErr.Text, out dPos);
            CData.Dev.dOnp_Z_BcrErr = dPos;
            //
            //20191010 ghk_manual_bcr
            double.TryParse(txtS_OnP_Z_Bcr_TbL.Text, out dPos);
            CData.Dev.dOnp_Z_Bcr_TbL = dPos;
            double.TryParse(txtS_OnP_Z_Ori_TbL.Text, out dPos);
            CData.Dev.dOnp_Z_Ori_TbL = dPos;
            double.TryParse(txtS_OnP_Z_Bcr_TbR.Text, out dPos);
            CData.Dev.dOnp_Z_Bcr_TbR = dPos;
            double.TryParse(txtS_OnP_Z_Ori_TbR.Text, out dPos);
            CData.Dev.dOnp_Z_Ori_TbR = dPos;
            //
            //211022 syc : Onp Pick Up Offset
            double.TryParse(txtS_Onp_Z_PickOffset.Text, out dPos);
            CData.Dev.dOnp_Z_PickOffset = dPos;
            //

            //20190604 ghk_onpbcr
            // On Loader Picker Y
            double.TryParse(txtS_OnP_Y_Bcr.Text    , out dPos);
            CData.Dev.dOnp_Y_Bcr = dPos;
            double.TryParse(txtS_OnP_Y_Ori.Text    , out dPos);
            CData.Dev.dOnp_Y_Ori = dPos;
            double.TryParse(txtS_OnP_Y_BcrSecon.Text, out dPos); //190821 ksg :
            CData.Dev.dOnp_Y_BcrSecon = dPos;
            //201230 jhc : 2D 코드 판독 실패 Alarm 발생 전 제3의 위치(OCR 위치)로 카메라 이동 기능
            double.TryParse(txtS_OnP_Y_BcrErr.Text, out dPos);
            CData.Dev.dOnp_Y_BcrErr = dPos;
            //
            //20191010 ghk_manual_bcr
            double.TryParse(txtS_OnP_Y_Bcr_TbL.Text, out dPos);
            CData.Dev.dOnp_Y_Bcr_TbL = dPos;
            double.TryParse(txtS_OnP_Y_Ori_TbL.Text, out dPos);
            CData.Dev.dOnp_Y_Ori_TbL = dPos;
            double.TryParse(txtS_OnP_Y_Bcr_TbR.Text, out dPos);
            CData.Dev.dOnp_Y_Bcr_TbR = dPos;
            double.TryParse(txtS_OnP_Y_Ori_TbR.Text, out dPos);
            CData.Dev.dOnp_Y_Ori_TbR = dPos;
            //

            // Left Grind Y
            double.TryParse(txtS_GrdL_Y_GrdSt.Text, out dPos);      CData.Dev.aGrd_Y_Start[0]   = dPos;
            double.TryParse(txtS_GrdL_Y_GrdEd.Text, out dPos);      CData.Dev.aGrd_Y_End[0]     = dPos;
            //20191011 ghk_manual_bcr
            double.TryParse(txtS_GrdL_Y_Ori.Text, out dPos);        CData.Dev.aGrd_Y_Ori[0]     = dPos;
            //

            // Right Grind Y
            double.TryParse(txtS_GrdR_Y_GrdSt.Text, out dPos);      CData.Dev.aGrd_Y_Start[1]   = dPos;
            double.TryParse(txtS_GrdR_Y_GrdEd.Text, out dPos);      CData.Dev.aGrd_Y_End[1]     = dPos;
            //20191011 ghk_manual_bcr
            double.TryParse(txtS_GrdR_Y_Ori.Text,   out dPos);      CData.Dev.aGrd_Y_Ori[1]     = dPos;
            //

            // Off Loader Picker X
            double.TryParse(txtS_OffP_X_BtClnSt.Text,       out dPos);  CData.Dev.dOffP_X_ClnStart       = dPos;
            double.TryParse(txtS_OffP_X_BtClnEd.Text,       out dPos);  CData.Dev.dOffP_X_ClnEnd         = dPos;
            // Brush X Start/End Pos
            double.TryParse(txtS_OffP_X_BtClnSt_Brush.Text, out dPos);  CData.Dev.dOffP_X_ClnStart_Brush = dPos;   // 2022.07.28 lhs
            double.TryParse(txtS_OffP_X_BtClnEd_Brush.Text, out dPos);  CData.Dev.dOffP_X_ClnEnd_Brush   = dPos;   // 2022.07.28 lhs

            // 2021.03.18 SungTae Start : 고객사(ASE-KR) 요청으로 Center Position 추가
            if (CData.CurCompany == ECompany.ASE_KR)
            {
                double.TryParse(txtS_OffP_X_BtClnCenter.Text, out dPos);    CData.Dev.dOffP_X_ClnCenter = dPos;
            }
            // 2021.03.18 SungTae End

            // Off Loader Picker Z
            double.TryParse(txtS_OffP_Z_Pick.Text,   out dPos);     CData.Dev.dOffP_Z_Pick      = dPos;
            double.TryParse(txtS_OffP_Z_Place.Text,  out dPos);     CData.Dev.dOffP_Z_Place     = dPos;
            double.TryParse(txtS_OffP_Z_Slow.Text,   out dPos);     CData.Dev.dOffP_Z_Slow      = dPos;
            double.TryParse(txtS_OffP_Z_DryGap.Text, out dPos);     CData.Dev.dOffP_Z_PlaceDn   = dPos;
            // 2021.02.27 SungTae Start : 고객사(ASE-KR) 요청으로 Device에서 관리 위해 추가
            //if(CData.CurCompany == ECompany.ASE_KR)
            if (CData.CurCompany == ECompany.ASE_KR     ||
                CData.CurCompany == ECompany.SCK        ||  // 2021.07.14 lhs  SCK, JSCK 추가
                CData.CurCompany == ECompany.JSCK       ||
                CData.CurCompany == ECompany.SkyWorks   ||  //220216 pjh : Skyworks 추가
                CDataOption.UseSprayBtmCleaner          )   // 2022.03.08 lhs Spray Nozzle Bottom Cleaner 추가 
            {
                double.TryParse(txtS_OffP_Z_BtClnSt.Text,   out dPos);  CData.Dev.dOffP_Z_ClnStart      = dPos;
                double.TryParse(txtS_OffP_Z_StrpClnSt.Text, out dPos);  CData.Dev.dOffP_Z_StripClnStart = dPos;
            }
            // 2021.02.27 SungTae End
            
            // Brush Z Pos
            double.TryParse(txtS_OffP_Z_StrpClnSt_Brush.Text, out dPos); CData.Dev.dOffP_Z_StripClnStart_Brush = dPos;  // 2022.07.28 lhs 

            // Dry R
            double.TryParse(txtS_Dry_R_Lv1.Text,    out dPos);      CData.Dev.dDry_R_Check1     = dPos;
            double.TryParse(txtS_Dry_R_Lv2.Text,    out dPos);      CData.Dev.dDry_R_Check2     = dPos;
            // 200312-jym : Unit방식 포지션 추가
            double.TryParse(txtS_Dry_R_Carr.Text,   out dPos);      CData.Dev.dDry_Car_Check    = dPos;
            double.TryParse(txtS_Dry_R_Unit.Text,   out dPos);      CData.Dev.dDry_Unit_Check   = dPos;
            double.TryParse(txtS_Dry_R_Start.Text,  out dPos);      CData.Dev.dDry_Start        = dPos;
            double.TryParse(txtS_Dry_R_End.Text,    out dPos);      CData.Dev.dDry_End          = dPos;

            // Off Loader X
            // 현재 스트립 높이 - 기준 스트립 높이
            dVal = CData.Dev.dLnSide - GV.STRIP_DEF_LONG;
            // 기준 스트립 얼라인 포지션 - 기준과 현재의 스트립 차이 값
            dVal = CData.SPos.dOFL_X_DefAlign - dVal;
            CData.Dev.dOffL_X_Algn = Math.Round(dVal, 5);

            // Off Loader Y
            double.TryParse(txtS_OffL_Y_Wait.Text, out dPos);
            CData.Dev.dOffL_Y_Wait = dPos;
            //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)
            double.TryParse(txtS_OffL_Y_MgzBcr.Text, out dPos);
            CData.Dev.dOffL_Y_MgzBcr = dPos;
            //

            // Off Loader Z
            double.TryParse(txtS_OffL_Z_TpRcvDn.Text, out dPos);
            CData.Dev.dOffL_Z_TRcv_Dn = dPos;
            //dVal = dPos + (CData.Dev.dMgzPitch * CData.Dev.iMgzCnt);
            dVal = dPos + (CData.Dev.dMgzPitch * (CData.Dev.iMgzCnt - 1)); //190416 ksg : 수치 계산 오류
            CData.Dev.dOffL_Z_TRcv_Up = dVal;

            double.TryParse(txtS_OffL_Z_BtRcvDn.Text, out dPos);
            CData.Dev.dOffL_Z_BRcv_Dn = dPos;
            //dVal = dPos + (CData.Dev.dMgzPitch * CData.Dev.iMgzCnt);
            dVal = dPos + (CData.Dev.dMgzPitch * (CData.Dev.iMgzCnt - 1)); //190416 ksg : 수치 계산 오류
            CData.Dev.dOffL_Z_BRcv_Up = dVal;

            //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)
            double.TryParse(txtS_OffL_Z_MgzBcr.Text, out dPos);
            CData.Dev.dOffL_Z_MgzBcr = dPos;
            //
        }
        #endregion

        #region Set position page

        /// <summary>
        /// Set Position - 축 이동버튼 클릭 이벤트 (조그 제외한 스탭 이동 시)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnS_MvStep_Click(object sender, EventArgs e)
        {
            CData.bLastClick = true; //190509 ksg :
            //190430 ksg : Auto & Manual 때 동작 안함
            if(CSQ_Main.It.m_iStat == EStatus.Auto_Running || CSQ_Main.It.m_iStat == EStatus.Manual) return;

            Button mBtn = sender as Button;
            int iRet = 0;
            int iAx = int.Parse(mBtn.Parent.Tag.ToString()) + m_iAx1;
            if(CDataOption.CurEqu == eEquType.Pikcer)
            { 
                //20190604 ghk_onpbcr
                int iTag = int.Parse(mBtn.Parent.Tag.ToString());
                     if (iTag == 0) { iAx = m_iAx1; }
                else if (iTag == 1) { iAx = m_iAx2; }
                else                { iAx = m_iAx3; }
                //
            }

            if (m_sStep == "Jog")
            {
                return;
            }
            else
            {
                // 200824 jym : 프로브 체크
                if (iAx == (int)EAx.LeftGrindZone_Y && (CIO.It.Get_Y(eY.GRDL_ProbeDn) || !CIO.It.Get_X(eX.GRDL_ProbeAMP)))
                {
                    CMsg.Show(eMsg.Error, "Error", "Check left probe status.");
                    return;
                }
                // 200824 jym : 프로브 체크
                if (iAx == (int)EAx.RightGrindZone_Y && (CIO.It.Get_Y(eY.GRDR_ProbeDn) || !CIO.It.Get_X(eX.GRDR_ProbeAMP)))
                {
                    CMsg.Show(eMsg.Error, "Error", "Check right probe status.");
                    return;
                }

                double dStp = 0.0;
                if (m_sStep == "Custom")
                { double.TryParse(txtS_Cus.Text, out dStp); }
                else if (m_sStep == "Pitch")
                { dStp = CData.Dev.dMgzPitch; }
                else
                { double.TryParse(m_sStep, out dStp); }
                //190214 ksg :
                if(iAx == 14 || iAx == 4)
                {
                    if(mBtn.Tag.ToString() == "N") { dStp *= 1; }
                    else { dStp *= -1; }

                }
                else
                {
                    if (mBtn.Tag.ToString() == "N")
                    { dStp *= -1; }
                }
                //if (mBtn.Tag.ToString() == "N")
                //{ dStp *= -1; }

                //190615 pjh:
                //Dry Stick Check

                if (iAx == 17)
                {
                    if (CDataOption.Dryer == eDryer.Rotate)
                    {
                        if (CIO.It.Get_X(eX.DRY_BtmTargetPos))
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Please Check Dry Bottom Stick");
                            return;
                        }
                    }
                }

                //200630 jhc : 온/오프 로더 Move(Jog포함) 전 자재 감지 체크
                if (_CheckStrip(iAx))
                { return; }
                //

                //
                iRet = CMot.It.Mv_I(iAx, dStp);
                if (iRet != 0)
                {
                    CMot.It.EStop(iAx);
                    CMsg.Show(eMsg.Error, "Error", "Step move error");
                }
                else
                {
                    GU.Delay(100);
                    while (true)
                    {
                        if (CMot.It.Get_Mv(iAx))
                        {
                            break;
                        }

                        // Time out error

                        Application.DoEvents();
                    }
                }
            }
        }

        /// <summary>
        /// Set Position - 스탑 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnS_Stop_Click(object sender, EventArgs e)
        {
            CData.bLastClick = true; //190509 ksg :
            Button mBtn = sender as Button;
            int iRet = 0;
            int iAx = int.Parse(mBtn.Parent.Tag.ToString()) + m_iAx1;
            if (CDataOption.CurEqu == eEquType.Pikcer)
            {
                //20190604 ghk_onpbcr
                int iTag = int.Parse(mBtn.Parent.Tag.ToString());
                if      (iTag == 0) { iAx = m_iAx1; }
                else if (iTag == 1) { iAx = m_iAx2; }
                else                { iAx = m_iAx3; }
            }
            iRet = CMot.It.Stop(iAx);
            if (iRet != 0)
            {
                //CErr.Show()
            }
        }

        /// <summary>
        /// Set Position - 축 이동버튼 마우스 다운 이벤트 (조그 이동 시)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnS_Mv_MouseDown(object sender, MouseEventArgs e)
        {
            Button mBtn = sender as Button;

            bool bPot = true;
            int iAx = int.Parse(mBtn.Parent.Tag.ToString()) + m_iAx1;
            if (CDataOption.CurEqu == eEquType.Pikcer)
            {
                //20190604 ghk_onpbcr
                int iTag = int.Parse(mBtn.Parent.Tag.ToString());
                if      (iTag == 0) { iAx = m_iAx1; }
                else if (iTag == 1) { iAx = m_iAx2; }
                else                { iAx = m_iAx3; }
            }
            string sTxt = mBtn.Tag.ToString();

            if (m_sStep == "Jog")
            {
                // 200824 jym : 프로브 체크
                if (iAx == (int)EAx.LeftGrindZone_Y && (CIO.It.Get_Y(eY.GRDL_ProbeDn) || !CIO.It.Get_X(eX.GRDL_ProbeAMP)))
                {
                    CMsg.Show(eMsg.Error, "Error", "Check left probe status.");
                    return;
                }
                // 200824 jym : 프로브 체크
                if (iAx == (int)EAx.RightGrindZone_Y && (CIO.It.Get_Y(eY.GRDR_ProbeDn) || !CIO.It.Get_X(eX.GRDR_ProbeAMP)))
                {
                    CMsg.Show(eMsg.Error, "Error", "Check right probe status.");
                    return;
                }

                //190214 ksg :
                if (iAx == 14 || iAx == 4)
                {
                    if (sTxt == "N") bPot = true;
                    else bPot = false;
                }
                else if (sTxt == "N")
                {
                    bPot = false;
                }

                //190615 pjh:
                //Dry Stick Check
                if (iAx == 17)
                {
                    if (CDataOption.Dryer == eDryer.Rotate)
                    {
                        if (CIO.It.Get_X(eX.DRY_BtmTargetPos))
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Please Check Dry Bottom Stick");
                            return;
                        }
                    }
                }

                //200630 jhc : 온/오프 로더 Move(Jog포함) 전 자재 감지 체크
                if (_CheckStrip(iAx))
                { return; }
                //

                CMot.It.Mv_J(iAx, bPot);
            }
            else
            { return; }
        }

        /// <summary>
        /// Set Position - 축 이동버튼 마우스 업 이벤트 (조그 이동 시)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnS_Mv_MouseUp(object sender, MouseEventArgs e)
        {
            Button mBtn = sender as Button;

            int iAx = int.Parse(mBtn.Parent.Tag.ToString()) + m_iAx1;
            if (CDataOption.CurEqu == eEquType.Pikcer)
            {
                //20190604 ghk_onpbcr
                int iTag = int.Parse(mBtn.Parent.Tag.ToString());
                if      (iTag == 0) { iAx = m_iAx1; }
                else if (iTag == 1) { iAx = m_iAx2; }
                else                { iAx = m_iAx3; }
            }
            string sTxt = mBtn.Tag.ToString();

            CMot.It.EStop(iAx);
        }

        /// <summary>
        /// Set Position - 라디오 버튼 인덱스 체인지 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdbS_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton mRdb = sender as RadioButton;
            m_sStep = mRdb.Tag.ToString();
        }

        /// <summary>
        /// Set Position - 포지션 Get 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnS_Part_Get_Click(object sender, EventArgs e)
        {
            Button mBtn = sender as Button;

            int iAx = int.Parse(mBtn.Parent.Tag.ToString()) + m_iAx1;
            if (CDataOption.CurEqu == eEquType.Pikcer)
            {
                //20190604 ghk_onpbcr
                int iTag = int.Parse(mBtn.Parent.Tag.ToString());
                if      (iTag == 0) { iAx = m_iAx1; }
                else if (iTag == 1) { iAx = m_iAx2; }
                else                { iAx = m_iAx3; }
            }
            string sTxt = mBtn.Tag.ToString();

            mBtn.Parent.Controls[sTxt].Text = CMot.It.Get_FP(iAx).ToString();
        }

        /// <summary>
        /// Set Position - 포지션 Move 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnS_Part_Mv_Click(object sender, EventArgs e)
        {
            //190430 ksg : Auto & Manual 때 동작 안함
            if(CSQ_Main.It.m_iStat == EStatus.Auto_Running || CSQ_Main.It.m_iStat == EStatus.Manual) return;

            Button mBtn = sender as Button;

            int iRt = 0;
            int iAx = int.Parse(mBtn.Parent.Tag.ToString()) + m_iAx1;
            if(CDataOption.CurEqu == eEquType.Pikcer)
            { 
                //20190604 ghk_onpbcr
                int iTag = int.Parse(mBtn.Parent.Tag.ToString());
                     if (iTag == 0) { iAx = m_iAx1; }
                else if (iTag == 1) { iAx = m_iAx2; }
                else                { iAx = m_iAx3; }
                //
            }
            string sTxt = mBtn.Tag.ToString();
            double dPos = 0;

            //190615 pjh:
            //Dry Stick Check

            if (iAx == (int)EAx.DryZone_Z/*17*/)
            {
                if (CDataOption.Dryer == eDryer.Rotate)
                {
                    if (CIO.It.Get_X(eX.DRY_BtmTargetPos) || !CIO.It.Get_X(eX.DRY_BtmStandbyPos)) // 2022.07.25 lhs : Standby 추가
                    {
                        CMsg.Show(eMsg.Warning, "Warning", "Please Check Dry Bottom Stick");
                        return;
                    }
                }
            }

            //200630 jhc : 온/오프 로더 Move(Jog포함) 전 자재 감지 체크
            if (_CheckStrip(iAx))
            { return; }
            //

            if (!double.TryParse(mBtn.Parent.Controls[sTxt].Text, out dPos))
            {
                // 포지션 더블형 변환 에러
                // Error
            }
            else
            {
                //190322 ksg :
                //syc : Picker X 이동시 확인 구문 추가 
                //if(!CheckStatus(iAx)) return;
                if (!CheckStatus(iAx, dPos)) return;

                if (CMsg.Show(eMsg.Warning, "Warning", "Can you move position -> " + dPos + "mm") == DialogResult.OK)
                {
                                    iRt = CMot.It.Mv_N(iAx, dPos);
                    if (iRt != 0)
                    {
                        // Error
                    }
                    else
                    {
                        while (true)
                        {
                            if (CMot.It.Get_Mv(iAx, dPos))
                            { break; }
                            Application.DoEvents();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Set Position - Output CheckBox Checked Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckbS_Out_Click(object sender, EventArgs e)
        {
            //190430 ksg : Auto & Manual 때 동작 안함
            if (CSQ_Main.It.m_iStat == EStatus.Auto_Running || CSQ_Main.It.m_iStat == EStatus.Manual) return;

            CheckBox mCkb = sender as CheckBox;
            eY eDO;
            //200224 ksg :
            if (mCkb.Tag.ToString() == "PUMPL_Run" || mCkb.Tag.ToString() == "PUMPR_Run")
            {
                CData.SwPump.Stop();
            }

            if (Enum.TryParse(mCkb.Tag.ToString(), out eDO))
            {
                //201208 jhc : Manual 동작 상호 배제 (Vacuum/Eject vs. Table Water)
                if (mCkb.Checked)
                {
                    if (eDO == eY.GRDL_TbEjector)
                    {
                        CData.L_GRD.ActWater(false);  //L-Table Water OFF
                        CData.L_GRD.ActVacuum(false); //L-Table Vacuum OFF
                    }
                    else if (eDO == eY.GRDL_TbVacuum)
                    {
                        CData.L_GRD.ActWater(false); //L-Table Water OFF
                        CData.L_GRD.ActEject(false); //L-Table Eject OFF
                    }
                    else if (eDO == eY.GRDL_TbFlow)
                    {
                        CData.L_GRD.ActEject(false);  //L-Table Eject OFF
                        CData.L_GRD.ActVacuum(false); //L-Table Vacuum OFF
                    }
                    else if (eDO == eY.GRDR_TbEjector)
                    {
                        CData.R_GRD.ActWater(false);  //R-Table Water OFF
                        CData.R_GRD.ActVacuum(false); //R-Table Vacuum OFF
                    }
                    else if (eDO == eY.GRDR_TbVacuum)
                    {
                        CData.R_GRD.ActWater(false); //R-Table Water OFF
                        CData.R_GRD.ActEject(false); //R-Table Eject OFF
                    }
                    else if (eDO == eY.GRDR_TbFlow)
                    {
                        CData.R_GRD.ActEject(false);  //R-Table Eject OFF
                        CData.R_GRD.ActVacuum(false); //R-Table Vacuum OFF
                    }

                    //DRY_ClnBtmFlow
                }
                //

                CIO.It.Set_Y(eDO, mCkb.Checked);
            }
        }

        /// <summary>
        /// Set Position - 탭내에 파트 나뉘는 탭의 인덱스 체인지
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabSetPart_SelectedIndexChanged(object sender, EventArgs e)
        {
            CData.bLastClick = true; //190509 ksg :
            m_iSysPart = int.Parse(tabSetPart.SelectedTab.Tag.ToString());

            switch (m_iSysPart)
            {
                case 0:
                    m_iAx1 = (int)EAx.OnLoader_X;
                    m_iAx2 = (int)EAx.OnLoader_Y;
                    m_iAx3 = (int)EAx.OnLoader_Z;
                    m_bAx3 = true;

                    SetWriteLanguage(btnS_MvN1.Name,CLang.It.GetLanguage("LEFT"));
                    SetWriteLanguage(btnS_MvP1.Name,CLang.It.GetLanguage("RIGHT"));
                    SetWriteLanguage(btnS_MvN2.Name,CLang.It.GetLanguage("FRONT"));
                    SetWriteLanguage(btnS_MvP2.Name,CLang.It.GetLanguage("REAR"));
                    SetWriteLanguage(btnS_MvN3.Name,CLang.It.GetLanguage("UP"));
                    SetWriteLanguage(btnS_MvP3.Name,CLang.It.GetLanguage("DOWN"));

                    ckbS_OnL_Lock   .Checked = CIO.It.Get_Y(eY.ONL_DoorLock     );
                    ckbS_OnL_TpRun  .Checked = CIO.It.Get_Y(eY.ONL_TopBeltRun   );
                    ckbS_OnL_BtCCW  .Checked = CIO.It.Get_Y(eY.ONL_BtmBeltCCW   );
                    ckbS_OnL_BtRun  .Checked = CIO.It.Get_Y(eY.ONL_BtmBeltRun   );
                    ckbS_OnL_Forw   .Checked = CIO.It.Get_Y(eY.ONL_PusherForward);
                    ckbS_OnL_ClmpOn .Checked = CIO.It.Get_Y(eY.ONL_ClampOn      );
                    ckbS_OnL_ClmpOff.Checked = CIO.It.Get_Y(eY.ONL_ClampOff     );

                    tabOnL.SelectTab(0);
                    break;

                case 1:
                    m_iAx1 = (int)EAx.Inrail_X;
                    m_iAx2 = (int)EAx.Inrail_Y;
                    m_bAx3 = false;
               
                    SetWriteLanguage(btnS_MvN1.Name,CLang.It.GetLanguage("LEFT"));
                    SetWriteLanguage(btnS_MvP1.Name,CLang.It.GetLanguage("RIGHT"));
                    SetWriteLanguage(btnS_MvN2.Name,CLang.It.GetLanguage("FRONT"));
                    SetWriteLanguage(btnS_MvP2.Name,CLang.It.GetLanguage("REAR"));

                    CkbS_Inr_Forw .Checked = CIO.It.Get_Y(eY.INR_GuideForward  );
                    CkbS_Inr_Clamp.Checked = CIO.It.Get_Y(eY.INR_GripperClampOn);
                    CkbS_Inr_Up   .Checked = CIO.It.Get_Y(eY.INR_LiftUp        );

                    break;

                case 2:
                    m_iAx1 = (int)EAx.OnLoaderPicker_X;
                    m_iAx2 = (int)EAx.OnLoaderPicker_Z;
                    // 20190604 ghk_onpbcr
                    if (CDataOption.CurEqu == eEquType.Pikcer)
                    {
                        m_iAx3 = (int)EAx.OnLoaderPicker_Y;
                        m_bAx3 = true;
                    }
                    else
                    { m_bAx3 = false; }

                    SetWriteLanguage(btnS_MvN1.Name,CLang.It.GetLanguage("LEFT"));
                    SetWriteLanguage(btnS_MvP1.Name,CLang.It.GetLanguage("RIGHT"));
                    SetWriteLanguage(btnS_MvN2.Name,CLang.It.GetLanguage("UP"));
                    SetWriteLanguage(btnS_MvP2.Name,CLang.It.GetLanguage("DOWN"));
                    if(m_bAx3)
                    {
                        SetWriteLanguage(btnS_MvN3.Name,CLang.It.GetLanguage("FRONT"));
                        SetWriteLanguage(btnS_MvP3.Name,CLang.It.GetLanguage("REAR"));
                    }

                    if (CDataOption.IsRevPicker)
                    {
                        ckbS_OnP_Vac1.Checked = !CIO.It.Get_Y(eY.ONP_Vacuum1);
                        ckbS_OnP_Vac2.Checked = !CIO.It.Get_Y(eY.ONP_Vacuum2);
                    }
                    else
                    {
                        ckbS_OnP_Vac1.Checked = CIO.It.Get_Y(eY.ONP_Vacuum1);
                        ckbS_OnP_Vac2.Checked = CIO.It.Get_Y(eY.ONP_Vacuum2);
                    }
                    ckbS_OnP_Ejt .Checked = CIO.It.Get_Y(eY.ONP_Ejector);
                    ckbS_OnP_Drn .Checked = CIO.It.Get_Y(eY.ONP_Drain  );

                    break;

                case 3:
                    m_iAx1 = (int)EAx.LeftGrindZone_X;
                    m_iAx2 = (int)EAx.LeftGrindZone_Y;
                    m_iAx3 = (int)EAx.LeftGrindZone_Z;
                    m_bAx3 = true;

                    SetWriteLanguage(btnS_MvN1.Name,CLang.It.GetLanguage("LEFT"));
                    SetWriteLanguage(btnS_MvP1.Name,CLang.It.GetLanguage("RIGHT"));
                    SetWriteLanguage(btnS_MvN2.Name,CLang.It.GetLanguage("FRONT"));
                    SetWriteLanguage(btnS_MvP2.Name,CLang.It.GetLanguage("REAR"));
                    SetWriteLanguage(btnS_MvN3.Name,CLang.It.GetLanguage("UP"));
                    SetWriteLanguage(btnS_MvP3.Name,CLang.It.GetLanguage("DOWN"));


                    ckbS_GrdL_Lock  .Checked = CIO.It.Get_Y(eY.GRD_DoorLock      );
                    ckbS_GrdL_FWtr  .Checked = CIO.It.Get_Y(eY.GRD_FrontWater    );
                    ckbS_GrdL_TWtr  .Checked = CIO.It.Get_Y(eY.GRDL_TbFlow       );
                    ckbS_GrdL_TEjt  .Checked = CIO.It.Get_Y(eY.GRDL_TbEjector    );
                    ckbS_GrdL_TVac  .Checked = CIO.It.Get_Y(eY.GRDL_TbVacuum     );
                    ckbS_GrdL_PumpR .Checked = CIO.It.Get_Y(eY.PUMPL_Run         );
                    ckbS_GrdL_TcSpng.Checked = CIO.It.Get_Y(eY.GRDL_TopClnFlow   );
                    ckbS_GrdL_TcAir .Checked = CIO.It.Get_Y(eY.GRDL_TopClnAir    );
                    ckbS_GrdL_TcDn  .Checked = CIO.It.Get_Y(eY.GRDL_TopClnDn     );
                    ckbS_GrdL_TcKnif.Checked = CIO.It.Get_Y(eY.GRDL_TopWaterKnife);
                    ckbS_GrdL_PrbAir.Checked = CIO.It.Get_Y(eY.GRDL_ProbeAir     );
                    ckbS_GrdL_PrbDn .Checked = CIO.It.Get_Y(eY.GRDL_ProbeDn      );
                    ckbS_GrdL_GrdWtr.Checked = CIO.It.Get_Y(eY.GRDL_SplWater     );
                    ckbS_GrdL_BtmWtr.Checked = CIO.It.Get_Y(eY.GRDL_SplBtmWater  );
                    ckbS_GrdL_Pcw   .Checked = CIO.It.Get_Y(eY.GRDL_SplPCW       );
                    ckbS_GrdL_Cda   .Checked = CIO.It.Get_Y(eY.GRDL_SplCDA       );

                    break;

                case 4:
                    m_iAx1 = (int)EAx.RightGrindZone_X;
                    m_iAx2 = (int)EAx.RightGrindZone_Y;
                    m_iAx3 = (int)EAx.RightGrindZone_Z;
                    m_bAx3 = true;

                    SetWriteLanguage(btnS_MvN1.Name,CLang.It.GetLanguage("LEFT"));
                    SetWriteLanguage(btnS_MvP1.Name,CLang.It.GetLanguage("RIGHT"));
                    SetWriteLanguage(btnS_MvN2.Name,CLang.It.GetLanguage("FRONT"));
                    SetWriteLanguage(btnS_MvP2.Name,CLang.It.GetLanguage("REAR"));
                    SetWriteLanguage(btnS_MvN3.Name,CLang.It.GetLanguage("UP"));
                    SetWriteLanguage(btnS_MvP3.Name,CLang.It.GetLanguage("DOWN"));


                    ckbS_GrdR_Lock  .Checked = CIO.It.Get_Y(eY.GRD_DoorLock      );
                    ckbS_GrdR_FWtr  .Checked = CIO.It.Get_Y(eY.GRD_FrontWater    );
                    ckbS_GrdR_TWtr  .Checked = CIO.It.Get_Y(eY.GRDR_TbFlow       );
                    ckbS_GrdR_TEjt  .Checked = CIO.It.Get_Y(eY.GRDR_TbEjector    );
                    ckbS_GrdR_TVac  .Checked = CIO.It.Get_Y(eY.GRDR_TbVacuum     );
                    ckbS_GrdR_PumpR .Checked = CIO.It.Get_Y(eY.PUMPR_Run         );
                    ckbS_GrdR_TcSpng.Checked = CIO.It.Get_Y(eY.GRDR_TopClnFlow   );
                    ckbS_GrdR_TcAir .Checked = CIO.It.Get_Y(eY.GRDR_TopClnAir    );
                    ckbS_GrdR_TcDn  .Checked = CIO.It.Get_Y(eY.GRDR_TopClnDn     );
                    ckbS_GrdR_TcKnif.Checked = CIO.It.Get_Y(eY.GRDR_TopWaterKnife);
                    ckbS_GrdR_PrbAir.Checked = CIO.It.Get_Y(eY.GRDR_ProbeAir     );
                    ckbS_GrdR_PrbDn .Checked = CIO.It.Get_Y(eY.GRDR_ProbeDn      );
                    ckbS_GrdR_GrdWtr.Checked = CIO.It.Get_Y(eY.GRDR_SplWater     );
                    ckbS_GrdR_BtmWtr.Checked = CIO.It.Get_Y(eY.GRDR_SplBtmWater  );
                    ckbS_GrdR_Pcw   .Checked = CIO.It.Get_Y(eY.GRDR_SplPCW       );
                    ckbS_GrdR_Cda   .Checked = CIO.It.Get_Y(eY.GRDR_SplCDA       );

                    break;

                case 5:
                    m_iAx1 = (int)EAx.OffLoaderPicker_X;
                    m_iAx2 = (int)EAx.OffLoaderPicker_Z;
                    m_bAx3 = false;

                    SetWriteLanguage(btnS_MvN1.Name,CLang.It.GetLanguage("LEFT"));
                    SetWriteLanguage(btnS_MvP1.Name,CLang.It.GetLanguage("RIGHT"));
                    SetWriteLanguage(btnS_MvN2.Name,CLang.It.GetLanguage("UP"));
                    SetWriteLanguage(btnS_MvP2.Name,CLang.It.GetLanguage("DOWN"));

                    if (CDataOption.IsRevPicker)
                    {
                        ckbS_OffP_Vac1.Checked = !CIO.It.Get_Y(eY.OFFP_Vacuum1);
                        ckbS_OffP_Vac2.Checked = !CIO.It.Get_Y(eY.OFFP_Vacuum2);
                    }
                    else
                    {
                        ckbS_OffP_Vac1.Checked = CIO.It.Get_Y(eY.OFFP_Vacuum1);
                        ckbS_OffP_Vac2.Checked = CIO.It.Get_Y(eY.OFFP_Vacuum2);
                    }
                    ckbS_OffP_Ejt .Checked = CIO.It.Get_Y(eY.OFFP_Ejector);
                    ckbS_OffP_Drn .Checked = CIO.It.Get_Y(eY.OFFP_Drain  );

                    break;

                case 6:
                    m_iAx1 = (int)EAx.DryZone_X;
                    m_iAx2 = (int)EAx.DryZone_Z;
                    m_iAx3 = (int)EAx.DryZone_Air;
                    m_bAx3 = true;

                    SetWriteLanguage(btnS_MvN1.Name,CLang.It.GetLanguage("LEFT"));
                    SetWriteLanguage(btnS_MvP1.Name,CLang.It.GetLanguage("RIGHT"));
                    SetWriteLanguage(btnS_MvN2.Name,CLang.It.GetLanguage("DOWN"));
                    SetWriteLanguage(btnS_MvP2.Name,CLang.It.GetLanguage("UP"));
                    SetWriteLanguage(btnS_MvN3.Name,CLang.It.GetLanguage("CW"));
                    SetWriteLanguage(btnS_MvP3.Name,CLang.It.GetLanguage("CCW"));

                    ckbS_Dry_OutAir.Checked = CIO.It.Get_Y(eY.DRY_OutRailAir);
                    ckbS_Dry_TcAir .Checked = CIO.It.Get_Y(eY.DRY_TopAir    );
                    ckbS_Dry_BtmAir.Checked = CIO.It.Get_Y(eY.DRY_BtmAir    );
                    ckbS_Dry_Wtr   .Checked = CIO.It.Get_Y(eY.DRY_ClnBtmFlow);

                    ckbS_Dry_ClampOpen.Checked = CIO.It.Get_Y(eY.DRY_ClampOpenOnOff);

                    break;

                case 7:
                    m_iAx1 = (int)EAx.OffLoader_X;
                    m_iAx2 = (int)EAx.OffLoader_Y;
                    m_iAx3 = (int)EAx.OffLoader_Z;
                    m_bAx3 = true;

                    SetWriteLanguage(btnS_MvN1.Name,CLang.It.GetLanguage("RIGHT"));
                    SetWriteLanguage(btnS_MvP1.Name,CLang.It.GetLanguage("LEFT"));
                    SetWriteLanguage(btnS_MvN2.Name,CLang.It.GetLanguage("FRONT"));
                    SetWriteLanguage(btnS_MvP2.Name,CLang.It.GetLanguage("REAR"));
                    SetWriteLanguage(btnS_MvN3.Name,CLang.It.GetLanguage("UP"));
                    SetWriteLanguage(btnS_MvP3.Name,CLang.It.GetLanguage("DOWN"));

                    ckbS_OffL_Lock     .Checked = CIO.It.Get_Y(eY.OFFL_DoorLock   );
                    ckbS_OffL_TpRun    .Checked = CIO.It.Get_Y(eY.OFFL_TopBeltRun );
                    ckbS_OffL_MiCCW    .Checked = CIO.It.Get_Y(eY.OFFL_MidBeltCCW );
                    ckbS_OffL_MiRun    .Checked = CIO.It.Get_Y(eY.OFFL_MidBeltRun );
                    ckbS_OffL_BtRun    .Checked = CIO.It.Get_Y(eY.OFFL_BtmBeltRun );
                    ckbS_OffL_TpClmpOn .Checked = CIO.It.Get_Y(eY.OFFL_TopClampOn );
                    ckbS_OffL_TpClmpOff.Checked = CIO.It.Get_Y(eY.OFFL_TopClampOff);
                    ckbS_OffL_TpCylUp  .Checked = CIO.It.Get_Y(eY.OFFL_TopMGZUp   );
                    ckbS_OffL_TpCylDn  .Checked = CIO.It.Get_Y(eY.OFFL_TopMGZDn   );
                    ckbS_OffL_BtClmpOn .Checked = CIO.It.Get_Y(eY.OFFL_BtmClampOn );
                    ckbS_OffL_BtClmpOff.Checked = CIO.It.Get_Y(eY.OFFL_BtmClampOff);
                    ckbS_OffL_BtCylUp  .Checked = CIO.It.Get_Y(eY.OFFL_BtmMGZUp   );
                    ckbS_OffL_BtCylDn  .Checked = CIO.It.Get_Y(eY.OFFL_BtmMGZDn   );

                    break;
            }
        }
        #endregion

        private void button5_Click(object sender, EventArgs e)
        {
            //20200113 myk_ONP_Rotate_InterLock
            if (CMot.It.Get_FP((int)EAx.OnLoaderPicker_Z) > CData.SPos.dONP_Z_Wait)
            {
                CMsg.Show(eMsg.Error, "Error", "Check the Onloader Picker Z Axis Position");
                return;
            }
            else if (CMot.It.Chk_HDI((int)EAx.OnLoaderPicker_Z) != 1)
            {
                CMsg.Show(eMsg.Error, "Error", "First Onloader Picker Z Axis Home Please");
                return;
            }
            else
            {
                CIO.It.Set_Y(eY.ONP_Rotate90, false);
                CIO.It.Set_Y(eY.ONP_Rotate0, true);
            }
        }

        private void button134_Click(object sender, EventArgs e)
        {
            //20200113 myk_ONP_Rotate_InterLock
            if (CMot.It.Get_FP((int)EAx.OnLoaderPicker_Z) > CData.SPos.dONP_Z_Wait)
            {
                CMsg.Show(eMsg.Error, "Error", "Check the Onloader Picker Z Axis Position");
                return;
            }
            else if (CMot.It.Chk_HDI((int)EAx.OnLoaderPicker_Z) != 1)
            {
                CMsg.Show(eMsg.Error, "Error", "First Onloader Picker Z Axis Home Please");
                return;
            }
            else
            {
                CIO.It.Set_Y(eY.ONP_Rotate0, false);
                CIO.It.Set_Y(eY.ONP_Rotate90, true);
            }
        }

        private void btnS_OffP_Rt0_Click(object sender, EventArgs e)
        {
            //20200113 myk_OFP_Rotate_InterLock
            if (CMot.It.Get_FP((int)EAx.OffLoaderPicker_Z) > CData.SPos.dOFP_Z_Wait)
            {
                CMsg.Show(eMsg.Error, "Error", "Check the Offloader Picker Z Axis Position");
                return;
            }
            else if (CMot.It.Chk_HDI((int)EAx.OffLoaderPicker_Z) != 1)
            {
                CMsg.Show(eMsg.Error, "Error", "First Offloader Picker Z Axis Home Please");
                return;
            }
            else
            {
                CIO.It.Set_Y(eY.OFFP_Rotate90, false);
                CIO.It.Set_Y(eY.OFFP_Rotate0, true);
            }
        }

        private void btnS_OffP_Rt90_Click(object sender, EventArgs e)
        {
            //20200113 myk_OFP_Rotate_InterLock
            if (CMot.It.Get_FP((int)EAx.OffLoaderPicker_Z) > CData.SPos.dOFP_Z_Wait)
            {
                CMsg.Show(eMsg.Error, "Error", "Check the Offloader Picker Z Axis Position");
                return;
            }
            else if (CMot.It.Chk_HDI((int)EAx.OffLoaderPicker_Z) != 1)
            {
                CMsg.Show(eMsg.Error, "Error", "First Offloader Picker Z Axis Home Please");
                return;
            }
            else
            {
                CIO.It.Set_Y(eY.OFFP_Rotate0, false);
                CIO.It.Set_Y(eY.OFFP_Rotate90, true);
            }
        }

        //190322 ksg :
        public bool CheckStatus(int iAxis, double dPos = 0)
        {
            bool IsOk = true;
            bool   bX1     , bX2;
            int    iAxis1  , iAxis2  , iAxis3  , iAxis4  ;
            double dPos1   , dPos2   , dPos3   , dPos4   , dPos5, dPos6, dPos7, dPos8, dPos9;
            double dCurPos1, dCurPos2, dCurPos3, dCurPos4;
            switch(iAxis)
            {
                case (int)EAx.Inrail_X :
                    {
                        iAxis1 = (int)EAx.OnLoaderPicker_X;
                        iAxis2 = (int)EAx.OnLoaderPicker_Z;
                        dPos1  = CData.Dev.dOnP_X_Wait;
                        dPos2  = CData.Dev.dOnP_Z_Pick - CData.Dev.dOnP_Z_Slow;
                        dCurPos1 = CMot.It.Get_FP(iAxis1);
                        dCurPos2 = CMot.It.Get_FP(iAxis2);

                        //if((dPos1 == dCurPos1) && (dCurPos2 >= (dPos2-10)))
                        if (CMot.It.Get_Mv(iAxis1, dPos1) && (dCurPos2 >= (dPos2 - 10))) //201103 pjh : 조건 변경
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Check Onloader Picker");
                            IsOk = false;
                        }
                        break;
                    }
                    
                case (int)EAx.OnLoaderPicker_X :
                    {
                        iAxis1 = (int)EAx.OnLoaderPicker_Z;
                        iAxis2 = (int)EAx.OffLoaderPicker_X;
                        iAxis3 = (int)EAx.OnLoaderPicker_X; //syc : Picker X 이동시 확인 구문 추가 

                        dPos1  = CData.SPos.dONP_Z_Wait;
                        dPos2  = CData.Dev.dOffP_X_ClnStart;

                        dCurPos1 = CMot.It.Get_FP(iAxis1);
                        dCurPos2 = CMot.It.Get_FP(iAxis2);
                        dCurPos3 = CMot.It.Get_FP(iAxis3); //syc : Picker X 이동시 확인 구문 추가

                        // 2022.06.07 lhs Start : (SCK+) Z축이 0이 아니면 X축 제한
                        if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                        {
                            if (dCurPos1 >= 0.1) 
                            {
                                CMsg.Show(eMsg.Warning, "Warning", "Check Onloader Picker Z Axis. The z-axis is non-zero.");
                                IsOk = false;
                            }
                        }
                        else // 기존
                        {
                            if (dCurPos1 > (dPos1 + 10))
                            {
                                CMsg.Show(eMsg.Warning, "Warning", "Check Onloader Picker Z Axis");
                                IsOk = false;
                            }
                        }

                        if (dCurPos3 - dPos < 0 && dCurPos2 > dPos2) //syc : Picker X 이동시 확인 구문 추가 //Onloder picker X축 '-'방향으로 이동시 Offloader X축 위치 확인 안함
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Check Offloader Picker X Axis Pos");
                            IsOk = false;
                        }
                        break;
                    }

                case (int)EAx.OnLoaderPicker_Z :
                    {
                        iAxis1 = (int)EAx.OnLoaderPicker_X;
                        iAxis2 = (int)EAx.LeftGrindZone_Y ;
                        iAxis3 = (int)EAx.RightGrindZone_Y;
                        iAxis4 = (int)EAx.Inrail_X        ;

                        dPos1    = CData.SPos.dONP_X_PlaceL ;
                        dPos2    = CData.SPos.dONP_X_PlaceR ;
                        dPos3    = CData.Dev .dOnP_X_Wait   ;
                        dPos4    = CData.SPos.dGRD_Y_Wait[0]; 
                        dPos5    = CData.SPos.dGRD_Y_Wait[1]; 
                        dPos6    = CData.SPos.dINR_X_Wait   ; 
                        dPos7    = CData.SPos.dONP_X_WaitPickL; //20200406 jhc : OnPicker L-Table Pickup 대기 위치

                        dCurPos1 = CMot.It.Get_FP(iAxis1)  ;
                        dCurPos2 = CMot.It.Get_FP(iAxis2)  ;
                        dCurPos3 = CMot.It.Get_FP(iAxis3)  ;
                        dCurPos4 = CMot.It.Get_FP(iAxis4)  ;

                        //if(dCurPos1 == dPos1)
                        if (CMot.It.Get_Mv(iAxis1, dPos1)) //201103 pjh : 조건 변경
                        {
                            //if(dCurPos2 != dPos4) 
                            if (!CMot.It.Get_Mv(iAxis2, dPos4)) //201103 pjh : 조건 변경
                            {
                                CMsg.Show(eMsg.Warning, "Warning", "Check Onloader Picker X Axis Pos");
                                IsOk = false;
                            }
                        }
                        //else if(dCurPos1 == dPos2)
                        else if (CMot.It.Get_Mv(iAxis1, dPos2)) //201103 pjh : 조건 변경
                        {
                            //if(dCurPos3 != dPos5) 
                            if (!CMot.It.Get_Mv(iAxis3, dPos5)) //201103 pjh : 조건 변경
                            {
                                CMsg.Show(eMsg.Warning, "Warning", "Check Onloader Picker X Axis Pos");
                                IsOk = false;
                            }
                        }
                        //else if(dCurPos1 == dPos3)
                        else if (CMot.It.Get_Mv(iAxis1, dPos3)) //201103 pjh : 조건 변경
                        {
                            //if(dCurPos4 != dPos6)
                            if (!CMot.It.Get_Mv(iAxis4, dPos6)) //201103 pjh : 조건 변경
                            {
                                CMsg.Show(eMsg.Warning, "Warning", "Check Onloader Picker X Axis Pos");
                                IsOk = false;
                            }
                        }
                        //20200406 jhc : OnPicker L-Table Pickup 대기 위치
                        //else if(dCurPos1 == dPos7)
                        if (CMot.It.Get_Mv(iAxis1, dPos7)) //201103 pjh : 조건 변경
                        {
                            //if(dCurPos2 != dPos4) 
                            if (!CMot.It.Get_Mv(iAxis2, dPos4)) //201103 pjh : 조건 변경
                            {
                                CMsg.Show(eMsg.Warning, "Warning", "Check Onloader Picker X Axis Pos");
                                IsOk = false;
                            }
                        }
                        //
                        else 
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Check Onloader Picker X Axis Pos");
                            IsOk = false;
                        }
                        break;
                    }

                case (int)EAx.LeftGrindZone_X :
                    {
                        iAxis1   = (int)EAx.LeftGrindZone_Z;
                        dPos1    = CData.SPos.dGRD_Z_Able[0];
                        dCurPos1 = CMot.It.Get_FP(iAxis1);
                        bX1      = CIO.It.Get_Y(eY.GRDL_ProbeDn);

                        if ((dCurPos1 > dPos1) || bX1)// && (dCurPos2 >= (dPos2-10)))
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Check Left Table Z Axis");
                            IsOk = false;
                        }
                        break;
                    }

                case (int)EAx.LeftGrindZone_Y :
                    {
                        iAxis1   = (int)EAx.OnLoaderPicker_Z ;
                        iAxis2   = (int)EAx.OffLoaderPicker_Z;
                        iAxis3   = (int)EAx.LeftGrindZone_Z  ;
                        dPos1    = CData.SPos.dONP_Z_Wait   ;
                        dPos2    = CData.SPos.dOFP_Z_Wait   ;
                        dPos3    = CData.SPos.dGRD_Z_Able[0];
                        dCurPos1 = CMot.It.Get_FP(iAxis1);
                        dCurPos2 = CMot.It.Get_FP(iAxis2);
                        dCurPos3 = CMot.It.Get_FP(iAxis3);
                        bX1      = CIO.It.Get_Y(eY.GRDL_ProbeDn);

                        if (dCurPos1 > (dPos1 + 10))
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Check OnLoader Picker Z Axis");
                            IsOk = false;
                        }
                        if (dCurPos2 > (dPos2 + 10))
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Check OffLoader Picker Z Axis");
                            IsOk = false;
                        }
                        if (dCurPos3 > dPos3)
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Check Left Table Z Axis");
                            IsOk = false;
                        }
                        if (bX1)
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Check Left Table Probe");
                            IsOk = false;
                        }
                        break;
                    }
                
                case (int)EAx.RightGrindZone_X :
                    {
                        iAxis1   = (int)EAx.RightGrindZone_Z;
                        dPos1    = CData.SPos.dGRD_Z_Able[1];
                        dCurPos1 = CMot.It.Get_FP(iAxis1);
                        bX1      = CIO.It.Get_Y(eY.GRDR_ProbeDn);
                    
                        if((dCurPos1 > dPos1) || bX1)// && (dCurPos2 >= (dPos2-10)))
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Check Left Table Z Axis");
                            IsOk = false;
                        }
                        break;
                    }

                case (int)EAx.RightGrindZone_Y :
                    {
                        iAxis1   = (int)EAx.OnLoaderPicker_Z ;
                        iAxis2   = (int)EAx.OffLoaderPicker_Z;
                        iAxis3   = (int)EAx.RightGrindZone_Z ;
                        dPos1    = CData.SPos.dONP_Z_Wait   ;
                        dPos2    = CData.SPos.dOFP_Z_Wait   ;
                        dPos3    = CData.SPos.dGRD_Z_Able[1];
                        dCurPos1 = CMot.It.Get_FP(iAxis1);
                        dCurPos2 = CMot.It.Get_FP(iAxis2);
                        dCurPos3 = CMot.It.Get_FP(iAxis3);
                        bX1      = CIO.It.Get_Y(eY.GRDR_ProbeDn);
                    
                        if(dCurPos1 > (dPos1 + 10))
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Check OnLoader Picker Z Axis");
                            IsOk = false;
                        }
                        if(dCurPos2 > (dPos2 + 10))
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Check OffLoader Picker Z Axis");
                            IsOk = false;
                        }
                        if(dCurPos3 > dPos3)
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Check Right Table Z Axis");
                            IsOk = false;
                        }
                        if(bX1)
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Check Right Table Probe");
                            IsOk = false;
                        }
                        break;
                    }

                case (int)EAx.OffLoaderPicker_X :
                    {
                        iAxis1 = (int)EAx.OffLoaderPicker_Z;
                        iAxis2 = (int)EAx.OnLoaderPicker_X ;
                        iAxis3 = (int)EAx.OffLoaderPicker_X; //syc : Picker X 이동시 확인 구문 추가 

                        dPos1  = CData.SPos.dOFP_Z_Wait;
                        dPos2  = CData.Dev .dOnP_X_Wait;

                        dCurPos1 = CMot.It.Get_FP(iAxis1);
                        dCurPos2 = CMot.It.Get_FP(iAxis2);
                        dCurPos3 = CMot.It.Get_FP(iAxis3); //syc : Picker X 이동시 확인 구문 추가

                        // 2022.06.07 lhs Start : (SCK+) Z축이 0이 아니면 X축 제한
                        if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                        {
                            if (dCurPos1 >= 0.1)
                            {
                                CMsg.Show(eMsg.Warning, "Warning", "Check Offloader Picker Z Axis. The z-axis is non-zero.");
                                IsOk = false;
                            }
                        }
                        else // 기존
                        {
                            if (dCurPos1 > (dPos1 + 10))
                            {
                                CMsg.Show(eMsg.Warning, "Warning", "Check Offloader Picker Z Axis");
                                IsOk = false;
                            }
                        }
                        //if(dCurPos2 != dPos2) //190716 ksg :조건 변경
                        //if (dCurPos2 > (dPos2 + 5)) //201103 pjh : 조건 변경
                        //{
                        //    CMsg.Show(eMsg.Warning, "Warning", "Check Onloader Picker X Axis Pos");
                        //    IsOk = false;
                        //}
                        if (dCurPos3 - dPos < 0 && dCurPos2 > dPos2 + 5) //syc : Picker X 이동시 확인 구문 추가 //Onloder picker X축 '-'방향으로 이동시 Offloader X축 위치 확인 안함
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Check Onloader Picker X Axis Pos");
                            IsOk = false;
                        }
                        break;
                    }

                case (int)EAx.OffLoaderPicker_Z :
                    {
                        iAxis1 = (int)EAx.OffLoaderPicker_X;
                        iAxis2 = (int)EAx.LeftGrindZone_Y ;
                        iAxis3 = (int)EAx.RightGrindZone_Y;

                        dPos1   = CData.SPos.dOFP_X_PickL    ;
                        dPos2   = CData.SPos.dOFP_X_PickR    ;
                        dPos3   = CData.SPos.dOFP_X_Place    ;
                        dPos4   = CData.Dev .dOffP_X_ClnStart; 
                        dPos5   = CData.Dev .dOffP_X_ClnEnd  ; 
                        dPos6   = CData.SPos.dGRD_Y_Wait[0]  ; 
                        dPos7   = CData.SPos.dGRD_Y_Wait[1]  ;
                        dPos8   = CData.Dev.dOffP_X_ClnStart_Brush;
                        dPos9   = CData.Dev.dOffP_X_ClnEnd_Brush;

                        dCurPos1 = CMot.It.Get_FP(iAxis1)  ;
                        dCurPos2 = CMot.It.Get_FP(iAxis2)  ;
                        dCurPos3 = CMot.It.Get_FP(iAxis3)  ;

                        //if(dCurPos1 == dPos1)
                        if (CMot.It.Get_Mv(iAxis1, dPos1)) //201103 pjh : 조건 변경
                        {
                            //if(dCurPos2 != dPos6) 
                            if (!CMot.It.Get_Mv(iAxis2, dPos6)) //201103 pjh : 조건 변경
                            {
                                CMsg.Show(eMsg.Warning, "Warning", "Check Offloader Picker X Axis Pos");
                                IsOk = false;
                            }
                        }
                        //else if(dCurPos1 == dPos2)
                        else if (CMot.It.Get_Mv(iAxis1, dPos2)) //201103 pjh : 조건 변경
                        {
                            //if(dCurPos3 != dPos7) 
                            if (!CMot.It.Get_Mv(iAxis3, dPos7)) //201103 pjh : 조건 변경
                            {
                                CMsg.Show(eMsg.Warning, "Warning", "Check Offloader Picker X Axis Pos");
                                IsOk = false;
                            }
                        }
                        //else if(!(dCurPos1 == dPos3 || dCurPos1 == dPos4 || dCurPos1 == dPos5))
                        else if (!(CMot.It.Get_Mv(iAxis1, dPos3) || CMot.It.Get_Mv(iAxis1, dPos4) || CMot.It.Get_Mv(iAxis1, dPos5) ||  //201103 pjh : 조건 변경
                                   CMot.It.Get_Mv(iAxis1, dPos8) || CMot.It.Get_Mv(iAxis1, dPos9)))     // 2022.07.27 lhs : dPos8, dPos9 추가
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Check Offloader Picker X Axis Pos");
                            IsOk = false;
                        }
                        else 
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Check Offloader Picker X Axis Pos");  // lhs 로직 개선 필요
                            IsOk = false;
                        }
                        break;
                    }

                case (int)EAx.DryZone_X :
                    {
                        iAxis1 = (int)EAx.OffLoaderPicker_Z;
                        iAxis2 = (int)EAx.DryZone_Z        ;
                        iAxis3 = (int)EAx.OffLoader_Y      ;
                        iAxis4 = (int)EAx.OffLoader_Z      ;
                        dPos1  = CData.SPos.dOFP_Z_Wait    ;
                        dPos2  = CData.SPos.dDRY_Z_Up      ;
                        dPos3  = CData.Dev .dOffL_Y_Wait   ;
                        dPos4  = CData.Dev .dOffL_Z_BRcv_Dn;
                        dPos5  = CData.Dev .dOffL_Z_BRcv_Up;
                        dCurPos1 = CMot.It.Get_FP(iAxis1);
                        dCurPos2 = CMot.It.Get_FP(iAxis2);
                        dCurPos3 = CMot.It.Get_FP(iAxis3);
                        dCurPos4 = CMot.It.Get_FP(iAxis4);
                        //20200513 jhc : 매거진 배출 위치 변경 추가
                        if (!CData.Opt.bQcUse && (CData.Opt.eEmitMgz == EMgzWay.Top))
                        {
                            bX1 = CIO.It.Get_X(eX.OFFL_TopClampMGZDetect1);
                            bX2 = CIO.It.Get_X(eX.OFFL_TopClampMGZDetect2);
                        }
                        else
                        {
                            bX1 = CIO.It.Get_X(eX.OFFL_BtmClampMGZDetect1);
                            bX2 = CIO.It.Get_X(eX.OFFL_BtmClampMGZDetect2);
                        }
                        //

                        if (dCurPos1 > (dPos1 + 10))
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Check Offloader Picker Z Axis");
                            IsOk = false;
                        }
                        //if((dCurPos2 != dPos2))
                        if (!CMot.It.Get_Mv(iAxis2, dPos2)) //201103 pjh : 조건변경
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Check Dry Z Axis Pos");
                            IsOk = false;
                        }
                        //if((dCurPos3 != dPos3))
                        if (!CMot.It.Get_Mv(iAxis3, dPos3)) //201103 pjh : 조건변경
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Check OffLoader Y Axis Pos");
                            IsOk = false;
                        }
                        if ((dCurPos4 < dPos4) || (dCurPos4 > dPos5))
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Check OffLoader Z Axis Pos");
                            IsOk = false;
                        }
                        if (!bX1 || !bX2)
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Check OffLoader Magazin");
                            IsOk = false;
                        }
                        break;
                    }

                case (int)EAx.DryZone_Z:
                    {
                        iAxis1 = (int)EAx.DryZone_Air;
                        dPos1 = CData.SPos.dDRY_R_Wait;
                        dCurPos1 = CMot.It.Get_FP(iAxis1);

                        //if (dCurPos1 != dPos1)
                        if (!CMot.It.Get_Mv(iAxis1, dPos1)) //201103 pjh : 조건 변경
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Check Dry R Axis");
                            IsOk = false;
                        }

                        if(CDataOption.eDryClamp == EDryClamp.Cylinder) // 2022.07.12 lhs
                        {
                            if (!CSq_Dry.It.Func_DryClampOpen(false))
                            {
                                CMsg.Show(eMsg.Warning, "Warning", "Check Dry Clamp");
                                IsOk = false;
                            }
                        }

                        break;
                    }

                case (int)EAx.DryZone_Air:
                    {
                        iAxis1 = (int)EAx.DryZone_Z;
                        dPos1 = CData.SPos.dDRY_Z_Check;
                        dCurPos1 = CMot.It.Get_FP(iAxis1);

                        if (dCurPos1 > dPos1)
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Check Dry Z Axis");
                            IsOk = false;
                        }
                        break;
                    }
            }
            return IsOk;
        }

        //190501 ksg :
        private void DryStickMove(object sender, EventArgs e)
        {
            if (CSQ_Main.It.m_iStat == EStatus.Auto_Running || CSQ_Main.It.m_iStat == EStatus.Manual) return;

            Button mBtn = sender as Button;
            int Tag = int.Parse(mBtn.Tag.ToString());

            if (Tag == 0)
            {
                CIO.It.Set_Y((int)eY.DRY_BtmTargetPos,false);
                CIO.It.Set_Y((int)eY.DRY_BtmStandbyPos,true);
            }
            else if (Tag == 1)
            {// 200103_pjh_Dry_Stick_InterLock
                if(!CIO.It.Get_X(eX.DRY_BtmStandbyPos) && !CIO.It.Get_X(eX.DRY_BtmTargetPos))
                {
                    if (CDataOption.UseDryWtNozzle) { CMsg.Show(eMsg.Error, "Error", "Check Water Nozzle Stick Sensor");    }   // 2021.04.09 lhs Start
                    else                            { CMsg.Show(eMsg.Error, "Error", "Check Bottom Air Blow Stick Sensor"); }
                    return;
                }

                // 2021.04.16 lhs Start
                if (CDataOption.UseDryWtNozzle || CDataOption.eDryClamp == EDryClamp.Cylinder)  // 2022.07.05 lhs : Cylinder Clamp 추가
                {
                    if (CMot.It.Get_FP((int)EAx.DryZone_Z) > CData.SPos.dDRY_Z_Check) // 반드시 Z축이 내려져 있어야 충돌이 안됨.
                    {
                        CMsg.Show(eMsg.Error, "Error", "Check Dry Z Axis Position");
                        return;
                    }
                    else
                    {
                        CIO.It.Set_Y((int)eY.DRY_BtmStandbyPos, false);
                        CIO.It.Set_Y((int)eY.DRY_BtmTargetPos, true);
                    }
                }// 2021.04.16 lhs End
                else
                {
                    if (!CIO.It.Get_X(eX.DRY_LevelSafety1) || !CIO.It.Get_X(eX.DRY_LevelSafety2))
                    {
                        CMsg.Show(eMsg.Error, "Error", "Check Dry Z Axis Position");
                        return;
                    }
                    else
                    {
                        CIO.It.Set_Y((int)eY.DRY_BtmStandbyPos, false);
                        CIO.It.Set_Y((int)eY.DRY_BtmTargetPos, true);
                    }
                }
                
            }
        }

        private void btnS_OnP_CleanL_Click(object sender, EventArgs e)
        {
            CIO.It.Set_Y((int)eY.INR_OnpCleaner, true);
        }

        private void btnS_OnP_CleanR_Click(object sender, EventArgs e)
        {
            CIO.It.Set_Y((int)eY.INR_OnpCleaner, false);
        }

        //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)
        private void _setMgzBcrUi()
        {
            //201012 jhc : (KEYENCE BCR)/(RFID) 옵션으로 구분 적용
            if (CDataOption.OnlRfid == eOnlRFID.Use && CDataOption.RFID == ERfidType.Keyence)
            //if (CDataOption.OnlRfid == eOnlRFID.Use && CData.CurCompany == ECompany.SPIL)
            {
                lblS_OnL_Y_MgzBcr.Visible    = true;
                txtS_OnL_Y_MgzBcr.Visible    = true;
                btn_Get_OnL_Y_MgzBcr.Visible = true;
                btn_Mov_OnL_Y_MgzBcr.Visible = true;

                lblS_OnL_Z_MgzBcr.Visible    = true;
                txtS_OnL_Z_MgzBcr.Visible    = true;
                btn_Get_OnL_Z_MgzBcr.Visible = true;
                btn_Mov_OnL_Z_MgzBcr.Visible = true;
            }
            else
            {
                lblS_OnL_Y_MgzBcr.Visible    = false;
                txtS_OnL_Y_MgzBcr.Visible    = false;
                btn_Get_OnL_Y_MgzBcr.Visible = false;
                btn_Mov_OnL_Y_MgzBcr.Visible = false;

                lblS_OnL_Z_MgzBcr.Visible    = false;
                txtS_OnL_Z_MgzBcr.Visible    = false;
                btn_Get_OnL_Z_MgzBcr.Visible = false;
                btn_Mov_OnL_Z_MgzBcr.Visible = false;
            }

            //201012 jhc : (KEYENCE BCR)/(RFID) 옵션으로 구분 적용
            if (CDataOption.OflRfid == eOflRFID.Use && CDataOption.RFID == ERfidType.Keyence)
            //if (CDataOption.OflRfid == eOflRFID.Use && CData.CurCompany == ECompany.SPIL)
            {
                lblS_OffL_Y_MgzBcr.Visible    = true;
                txtS_OffL_Y_MgzBcr.Visible    = true;
                btn_Get_OffL_Y_MgzBcr.Visible = true;
                btn_Mov_OffL_Y_MgzBcr.Visible = true;

                lblS_OffL_Z_MgzBcr.Visible    = true;
                txtS_OffL_Z_MgzBcr.Visible    = true;
                btn_Get_OffL_Z_MgzBcr.Visible = true;
                btn_Mov_OffL_Z_MgzBcr.Visible = true;
            }
            else
            {
                lblS_OffL_Y_MgzBcr.Visible    = false;
                txtS_OffL_Y_MgzBcr.Visible    = false;
                btn_Get_OffL_Y_MgzBcr.Visible = false;
                btn_Mov_OffL_Y_MgzBcr.Visible = false;

                lblS_OffL_Z_MgzBcr.Visible    = false;
                txtS_OffL_Z_MgzBcr.Visible    = false;
                btn_Get_OffL_Z_MgzBcr.Visible = false;
                btn_Mov_OffL_Z_MgzBcr.Visible = false;
            }
        }
        //

        //200630 jhc : 온/오프 로더 Move(Jog포함) 전 자재 감지 체크
        private bool _CheckStrip(int iAx)
        {
            bool result = false;

            if (iAx == (int)EAx.OnLoader_Y || iAx == (int)EAx.OnLoader_Z)
            {
                if (!CIO.It.Get_X(eX.INR_StripInDetect))
                {
                    //CErr.Show(eErr.INRAIL_DECTED_STRIPIN_SENSOR);
                    CMsg.Show(eMsg.Error, "Error", "INRAIL Strip In Detect");
                    result = true;
                }
            }
            else if (iAx == (int)EAx.OffLoader_Y || iAx == (int)EAx.OffLoader_Z)
            {
                if(!CIO.It.Get_X(eX.DRY_StripOutDetect))
                {
                    //CErr.Show(eErr.DRY_DETECTED_STRIP);
                    CMsg.Show(eMsg.Error, "Error", "Dry Out Detect Strip");
                    result = true;
                }
            }

            return result;
        }

        private void btnS_OffP_IV2_Trigger_Click(object sender, EventArgs e)
        {
            if (CIO.It.Get_X(eX.OFFP_IV2_Error)) // 에러 확인
            {
                _SetLog("Error : IV2 is Error");
            }
            if (CIO.It.Get_X(eX.OFFP_IV2_Busy))  // Busy 확인
            {
                _SetLog("IV2 is busy");
            }

            // IV2의 외부 트리거
            // 사양은 ON : Min 100us, OFF : Min 1.2ms
            CIO.It.Set_Y(eY.OFFP_IV2_Trigger, true);
            _SetLog("IV2 Trigger Out = ON");

            System.Threading.Thread.Sleep(100);

            CIO.It.Set_Y(eY.OFFP_IV2_Trigger, false);
            _SetLog("IV2 Trigger Out = OFF");
        }

        private void comboS_OffP_IV2_Program_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboS_OffP_IV2_Program.SelectedIndex < 0)
                return;

            // IV2 Program P000 ~ P003 선택
            int nIdx = comboS_OffP_IV2_Program.SelectedIndex;
            CSq_OfP.It.Func_SetIVProgram(nIdx);

        }

	}
}
