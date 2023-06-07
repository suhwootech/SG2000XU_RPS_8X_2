using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace SG2000X
{
    public partial class vwOpt_07Sys : UserControl
    {
        private int m_iAx1 = 0;
        private int m_iAx2 = 0;
        private int m_iAx3 = 0;
        private bool m_bAx3 = true;

        string m_sStep = "";

        /// <summary>
        /// 시스템 탭내에서 파트 탭 번호
        /// </summary>
        private int m_iSysPart = 0;

        /// <summary>
        /// 화면에 정보 업데이트 타이머
        /// </summary>
        private Timer m_tmUpdt;
        public vwOpt_07Sys()
        {
            if      ((int)ELang.English == CData.Opt.iSelLan) { System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US"); }
            else if ((int)ELang.Korea   == CData.Opt.iSelLan) { System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ko-KR"); }
            else if ((int)ELang.China   == CData.Opt.iSelLan) { System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-Hans"); }
            InitializeComponent();

            m_tmUpdt = new Timer();
            m_tmUpdt.Interval = 50;
            m_tmUpdt.Tick += _M_tmUpdt_Tick;

            pnlS_OnP_Clean.Visible = (CDataOption.eOnpClean == eOnpClean.Use) ? true : false;

            if (CDataOption.Dryer == eDryer.Knife) { tpDry_R.Text = "Y Axis"; }   // 2022.03.04 lhs

            // 2021.02.27 SungTae Start : 고객사(ASE-KR) 요청으로 Device에서 관리 위해 변경 
            if (CData.CurCompany == ECompany.ASE_KR     ||
                CData.CurCompany == ECompany.SCK        ||  // 2021.07.14 lhs  SCK, JSCK 추가
                CData.CurCompany == ECompany.JSCK       ||
                CData.CurCompany == ECompany.SkyWorks   ||  // 210126 pjh : Skyworks 추가
                CDataOption.UseSprayBtmCleaner          )   // 2022.03.08 lhs Spray Nozzle Bottom Cleaner 추가 
            {
                // Btm Clean
                lblS_OffP_Z_BtClnSt.Visible         = true;     // 레벨 표시
                txtS_OffP_Z_BtClnSt.Visible         = true;     // 텍스트 표시     
                btnS_Get_OffP_Z_BtClnSt.Visible     = false;    // Get 버튼 표시 안함 // Device에서 설정
                btnS_Mv_OffP_Z_BtClnSt.Visible      = false;    // Move 버튼 표시 안함 // Device에서 설정

                lblS_OffP_Z_BtClnSt.ForeColor       = System.Drawing.Color.OrangeRed;
                txtS_OffP_Z_BtClnSt.BackColor       = System.Drawing.Color.Silver;
                txtS_OffP_Z_BtClnSt.ReadOnly        = true;

                // Strip Clean
                lblS_OffP_Z_StrpClnSt.Visible       = true;     // 레벨 표시
                txtS_OffP_Z_StrpClnSt.Visible       = true;     // 텍스트 표시     
                btnS_Get_OffP_Z_StrpClnSt.Visible   = false;    // Get 버튼 표시 안함  // Device에서 설정
                btnS_Mv_OffP_Z_StrpClnSt.Visible    = false;    // Move 버튼 표시 안함 // Device에서 설정

                lblS_OffP_Z_StrpClnSt.ForeColor     = System.Drawing.Color.OrangeRed;
                txtS_OffP_Z_StrpClnSt.BackColor     = System.Drawing.Color.Silver;
                txtS_OffP_Z_StrpClnSt.ReadOnly      = true;

            }
            else // 그외 Site : Btm Clean만 있음, Btm Clean Option에서 설정 (기존 로직)
            {
                // Btm Clean
                lblS_OffP_Z_BtClnSt.ForeColor       = System.Drawing.Color.Black;
                txtS_OffP_Z_BtClnSt.BackColor       = System.Drawing.Color.White;
                txtS_OffP_Z_BtClnSt.ReadOnly        = false;

                // Strip Clean 표시 안함. 레벨 ~ 버튼 등 모두
                lblS_OffP_Z_StrpClnSt.Visible       = false;
                txtS_OffP_Z_StrpClnSt.Visible       = false;
                btnS_Get_OffP_Z_StrpClnSt.Visible   = false;
                btnS_Mv_OffP_Z_StrpClnSt.Visible    = false;
            }
            // 2021.02.27 SungTae End

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

            //--------------
            // OffPicker
            //--------------
            // OffPicker에 IV 설치 여부
            bool bOffP_IOIV = CDataOption.UseDryWtNozzle || CDataOption.UseOffP_IOIV;

            pnlS_OffP_IV2.Visible       = bOffP_IOIV;     // IV2(화상판별센서) 버튼 및 상태 
            // 현재 선택된 IV2 Program 표시
            //bool bProg1 = CIO.It.Get_Y(eY.OFFP_IV2_ProgBit0);  // lhs 수정 필요
            //comboS_OffP_IV2_Program.SelectedIndex = Convert.ToInt32(bProg1);
            int nProgNum = CSq_OfP.It.Func_GetIVProgram();  // 0~3
            comboS_OffP_IV2_Program.SelectedIndex = nProgNum;


            lblS_OffP_X_Picture1.Visible        = bOffP_IOIV;    // 촬영위치1 X
            txtS_OffP_X_Picture1.Visible        = bOffP_IOIV;
            btnS_OffP_X_Get_Picture1.Visible    = bOffP_IOIV;
            btnS_OffP_X_Move_Picture1.Visible   = bOffP_IOIV;

            lblS_OffP_X_Picture2.Visible        = bOffP_IOIV;    // 촬영위치2 X
            btnS_OffP_X_Get_Picture2.Visible    = bOffP_IOIV;
            txtS_OffP_X_Picture2.Visible        = bOffP_IOIV;
            btnS_OffP_X_Move_Picture2.Visible   = bOffP_IOIV;

            lblS_OffP_Z_Picture.Visible         = bOffP_IOIV;     // 촬영위치 Z
            txtS_OffP_Z_Picture.Visible         = bOffP_IOIV;
            btnS_OffP_Z_Get_Picture.Visible     = bOffP_IOIV;
            btnS_OffP_Z_Move_Picture.Visible    = bOffP_IOIV;

            //--------------
            // Dryer
            //--------------
            bool bSpringClamp = CDataOption.eDryClamp == EDryClamp.None; // 기존 Spring Clamp 방식

            // 좌측 중앙에 위치
            lblS_Dry_Lv1.Visible                = bSpringClamp;
            lblS_Dry_Lv2.Visible                = bSpringClamp;

            // Cylinder Clamp 판넬
            pnlS_Dry_Clamp.Visible = CDataOption.eDryClamp == EDryClamp.Cylinder;

            // Dryer Z Axis 탭
            lblS_Dry_Z_Level.Visible            = bSpringClamp;
            txtS_Dry_Z_Level.Visible            = bSpringClamp;
            btnS_Dry_Z_Get_Level.Visible        = bSpringClamp;
            btnS_Dry_Z_Move_Level.Visible       = bSpringClamp;

            lblS_Dry_Z_TipClamped.Visible       = bOffP_IOIV;
            txtS_Dry_Z_TipClamped.Visible       = bOffP_IOIV;       // Dryer Tip이 클램프 되는 위치. (dDRY_Z_Up - dOffP_Z_PlaceDn)
            btnS_Dry_Z_Move_TipClamped.Visible  = bOffP_IOIV;

            // 2021.03.30 lhs Start
            if (CDataOption.UseDryWtNozzle)
            {
                ckbS_Dry_BtmAir.Text = "Dry Water Nozzle";
                lblS_Dry_BtmStd.Text = "Dry Water Nozzle Standby";
                lblS_Dry_BtmTar.Text = "Dry Water Nozzle  Target";
                btnS_Dry_Std.Text = "Dry Water Nozzle Standby";
                btnS_Dry_Tar.Text = "Dry Water Nozzle  Target";
            }
            else
            {
                ckbS_Dry_BtmAir.Text = "Bottom Air Blow";
                lblS_Dry_BtmStd.Text = "Bottom Air Blow Standby";
                lblS_Dry_BtmTar.Text = "Bottom Air Blow Target";
                btnS_Dry_Std.Text = "BTM AIR STANDBY";
                btnS_Dry_Tar.Text = "BTM AIR TARGET";
            }
            // 2021.03.30 lhs End

            // 2022.01.18 SungTae Start : [추가] (ASE-KR VOC) Strip 유무 체크 시작 위치를 별도로 설정할 있도록 고객사 요청 사항
            if (CData.CurCompany == ECompany.ASE_KR && CData.CurCompanyName != "INARI")  // 2022.01.18 lhs : Inari 제외
            {
                lblS_Dry_Z_ChkStrip.Visible = true;
                txtS_Dry_Z_ChkStrip.Visible = true;
                btnS_Dry_Z_ChkStrip_Get.Visible = true;
                btnS_Dry_Z_ChkStrip_Move.Visible = true;
            }
            else
            {
                lblS_Dry_Z_ChkStrip.Visible = false;
                txtS_Dry_Z_ChkStrip.Visible = false;
                btnS_Dry_Z_ChkStrip_Get.Visible = false;
                btnS_Dry_Z_ChkStrip_Move.Visible = false;
            }
            // 2022.01.18 SungTae End

            if (CDataOption.Use2004U)
            {
                panel9.Visible = false; // inrail align 패널 비활성화

                lblS_GrdL_CVac.Visible = true; //GRDL Carrier Vac 
                lblS_GrdR_CVac.Visible = true; //GRDR Carrier Vac

                pnlS_OffP_Clamp.Visible = true; // offp Clamp 패널

                lblS_Dry_Lv1.Text = "Cover Detect"; // Safty 센서 이름 변경
                lblS_Dry_Lv1.Visible = false;       // Dry Safty2 센서 사용 안함

                panel20.Visible = false; // 온피커 로드셀 사용 안함
                panel21.Visible = false; // 오프피커 로드셀 사용 안함
            }

            // 2022.03.04 lhs Start : SprayBtmCleaner 사용시 표시
            bool bSprayCln = CDataOption.UseSprayBtmCleaner;
            ckbS_Dry_BtmClnNzlMove.Visible = bSprayCln;
            ckbS_Dry_BtmClnAirBlow.Visible = bSprayCln;
            lblS_Dry_BtmClnNzlForward.Visible = bSprayCln;
            lblS_Dry_BtmClnNzlBackward.Visible = bSprayCln;
            // 2022.03.04 lhs End

            // 2022.03.15 lhs Start : Dryer의 Level Safety Sensor에 Air Blow 설치시
            ckbS_Dry_LevelAirBlow.Visible = CDataOption.UseDryerLevelAirBlow;
            
            if(CData.CurCompany == ECompany.ASE_KR || CData.CurCompany == ECompany.ASE_K12)
            {
                //Left Table Grinding Start 
                label10.Visible              = true;
                txtS_GrdL_Y_TblGrdSt.Visible = true;
                button48.Visible             = true;
                button36.Visible             = true;
                //Left Table Grinding End
                label12.Visible              = true;
                txtS_GrdL_Y_TblGrdEd.Visible = true;
                button56.Visible             = true;
                button46.Visible             = true;
                //Right Table Grinding Start 
                label22.Visible              = true;
                txtS_GrdR_Y_TblGrdSt.Visible = true;
                button86.Visible             = true;
                button115.Visible            = true;
                //Right Table Grinding End 
                label15.Visible              = true;
                txtS_GrdR_Y_TblGrdEd.Visible = true;
                button77.Visible             = true;
                button114.Visible            = true;
            }
            else
            {
                //Left Table Grinding Start 
                label10.Visible              = false;
                txtS_GrdL_Y_TblGrdSt.Visible = false;
                button48.Visible             = false;
                button36.Visible             = false;
                //Left Table Grinding End      
                label12.Visible              = false;
                txtS_GrdL_Y_TblGrdEd.Visible = false;
                button56.Visible             = false;
                button46.Visible             = false;
                //Right Table Grinding Start   
                label22.Visible              = false;
                txtS_GrdR_Y_TblGrdSt.Visible = false;
                button86.Visible             = false;
                button115.Visible            = false;
                //Right Table Grinding End     
                label15.Visible              = false;
                txtS_GrdR_Y_TblGrdEd.Visible = false;
                button77.Visible             = false;
                button114.Visible            = false;
            }
        }

        #region Basic method
        /// <summary>
        /// View가 화면에 그려질때 실행해야 하는 함수
        /// </summary>
        public void Open()
        {
            if (CSQ_Man.It.bBtnShow && (CSQ_Main.It.m_iStat == EStatus.Idle || CSQ_Main.It.m_iStat == EStatus.Stop))
            { _EnableBtn(true); }
            else
            { _EnableBtn(false); }

            // 탭 정보 초기화
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

            Set();

            // 타이머 멈춤 상태면 타이머 다시 시작
            if (!m_tmUpdt.Enabled)
            { m_tmUpdt.Start(); }
        }

        /// <summary>
        /// View가 화면에서 지워질때 실행해야 하는 함수
        /// </summary>
        public void Close()
        {
            // 타이머 실행 중이면 타이머 멈춤
            if (m_tmUpdt.Enabled)
            { m_tmUpdt.Stop(); }
        }

        public void Set()
        {
            // On Loader X
            // 대기 포지션
            txtS_OnL_X_Wait.Text = CData.SPos.dONL_X_Wait.ToString();
            txtS_OnL_X_Align.Text = CData.SPos.dONL_X_DefAlign.ToString();

            // On Loader Y
            // Pick 포지션
            txtS_OnL_Y_Pick.Text = CData.SPos.dONL_Y_Pick.ToString();
            // Place 포지션
            txtS_OnL_Y_Place.Text = CData.SPos.dONL_Y_Place.ToString();

            // On Loader Z
            txtS_OnL_Z_Wait.Text = CData.SPos.dONL_Z_Wait.ToString();
            txtS_OnL_Z_PickGo.Text = CData.SPos.dONL_Z_PickGo.ToString();
            txtS_OnL_Z_Clamp.Text = CData.SPos.dONL_Z_Clamp.ToString();
            txtS_OnL_Z_PickUp.Text = CData.SPos.dONL_Z_PickUp.ToString();
            txtS_OnL_Z_PlaceGo.Text = CData.SPos.dONL_Z_PlaceGo.ToString();
            txtS_OnL_Z_Unclamp.Text = CData.SPos.dONL_Z_UnClamp.ToString();
            txtS_OnL_Z_PlaceDn.Text = CData.SPos.dONL_Z_PlaceDn.ToString();

            // Inrail X
            txtS_Inr_X_Wait.Text = CData.SPos.dINR_X_Wait.ToString();
            txtS_Inr_X_Check.Text = CData.SPos.dINR_X_ChkUnit.ToString();      // 2021-06-01, jhLee : SPIL 2003U, Sensor Check 위치


            // InRail Y
            txtS_Inr_Y_Wait.Text = CData.SPos.dINR_Y_Wait.ToString();
            txtS_Inr_Y_Align.Text = CData.SPos.dINR_Y_Align.ToString();

            // On Loader Picker X
            txtS_OnP_X_PlaceL.Text = CData.SPos.dONP_X_PlaceL.ToString();
            txtS_OnP_X_PlaceR.Text = CData.SPos.dONP_X_PlaceR.ToString();
            txtS_OnP_X_Conv.Text = CData.SPos.dONP_X_Con.ToString();
            txtS_OnP_X_WaitPickL.Text = CData.SPos.dONP_X_WaitPickL.ToString(); //20200406 jhc : OnPicker L-Table Pickup 대기 위치

            // On Loader Picker Z
            txtS_OnP_Z_Wait.Text = CData.SPos.dONP_Z_Wait.ToString();

            // On Loader Picker Y
            txtS_OnP_Y_Wait.Text = CData.SPos.dONP_Y_Wait.ToString();
            //

            // Grind Left X
            txtS_GrdL_X_Wait.Text = CData.SPos.dGRD_X_Wait[0].ToString();
            txtS_GrdL_X_Zero.Text = CData.SPos.dGRD_X_Zero[0].ToString();

            // Grind Left Y
            txtS_GrdL_Y_Wait.Text = CData.SPos.dGRD_Y_Wait[0].ToString();
            txtS_GrdL_Y_ClnSt.Text = CData.SPos.dGRD_Y_ClnStart[0].ToString();
            txtS_GrdL_Y_ClnEd.Text = CData.SPos.dGRD_Y_ClnEnd[0].ToString();
            // 2021.04.13 SungTae Start : Table Grinding 시 조작 실수 방지 위해 추가
            txtS_GrdL_Y_TblGrdSt.Text = CData.SPos.dGRD_Y_TblGrdSt[0].ToString();
            txtS_GrdL_Y_TblGrdEd.Text = CData.SPos.dGRD_Y_TblGrdEd[0].ToString();
            // 2021.04.13 SungTae End

            //syc : Ase Kr Dressing Start, End 위치 다름
            //200731 : myk Dressing Position 자동화
            if (CData.CurCompany == ECompany.ASE_KR)
            {
                txtS_GrdL_Y_DrsSt.Text = (CData.MPos[0].dY_WHL_DRS - GV.dDressEndPos).ToString();
                txtS_GrdL_Y_DrsEd.Text = (CData.MPos[0].dY_WHL_DRS - GV.dDressStartPos).ToString();
            }
            else
            {
                txtS_GrdL_Y_DrsSt.Text = (CData.MPos[0].dY_WHL_DRS - GV.dDressStartPos).ToString();
                txtS_GrdL_Y_DrsEd.Text = (CData.MPos[0].dY_WHL_DRS - GV.dDressEndPos).ToString();
            }

            txtS_GrdL_Y_TblInsp.Text = CData.SPos.dGRD_Y_TblInsp[0].ToString();
            txtS_GrdL_Y_DrsCh.Text = CData.SPos.dGRD_Y_DrsChange[0].ToString();

            // Grind Left Z
            txtS_GrdL_Z_Wait.Text = CData.SPos.dGRD_Z_Wait[0].ToString();
            txtS_GrdL_Z_Able.Text = CData.SPos.dGRD_Z_Able[0].ToString();

            // Grind Right X
            txtS_GrdR_X_Wait.Text = CData.SPos.dGRD_X_Wait[1].ToString();
            txtS_GrdR_X_Zero.Text = CData.SPos.dGRD_X_Zero[1].ToString();

            // Grind Right Y
            txtS_GrdR_Y_Wait.Text = CData.SPos.dGRD_Y_Wait[1].ToString();
            txtS_GrdR_Y_ClnSt.Text = CData.SPos.dGRD_Y_ClnStart[1].ToString();
            txtS_GrdR_Y_ClnEd.Text = CData.SPos.dGRD_Y_ClnEnd[1].ToString();
            // 2021.04.13 SungTae Start : Table Grinding 시 조작 실수 방지 위해 추가
            txtS_GrdR_Y_TblGrdSt.Text = CData.SPos.dGRD_Y_TblGrdSt[1].ToString();
            txtS_GrdR_Y_TblGrdEd.Text = CData.SPos.dGRD_Y_TblGrdEd[1].ToString();
            // 2021.04.13 SungTae End

            //syc : Ase Kr Dressing Start, End 위치 다름
            //200731 myk : Dressing Position 자동화
            if (CData.CurCompany == ECompany.ASE_KR)
            {
                txtS_GrdR_Y_DrsSt.Text = (CData.MPos[1].dY_WHL_DRS - GV.dDressEndPos).ToString();
                txtS_GrdR_Y_DrsEd.Text = (CData.MPos[1].dY_WHL_DRS - GV.dDressStartPos).ToString();
            }
            else
            {
                txtS_GrdR_Y_DrsSt.Text = (CData.MPos[1].dY_WHL_DRS - GV.dDressStartPos).ToString();
                txtS_GrdR_Y_DrsEd.Text = (CData.MPos[1].dY_WHL_DRS - GV.dDressEndPos).ToString();
            }


            txtS_GrdR_Y_TblInsp.Text = CData.SPos.dGRD_Y_TblInsp[1].ToString();
            txtS_GrdR_Y_DrsCh.Text = CData.SPos.dGRD_Y_DrsChange[1].ToString();

            // Grind Right Z
            txtS_GrdR_Z_Wait.Text = CData.SPos.dGRD_Z_Wait[1].ToString();
            txtS_GrdR_Z_Able.Text = CData.SPos.dGRD_Z_Able[1].ToString();

            // Off Loader Picker X
            txtS_OffP_X_Wait.Text = CData.SPos.dOFP_X_Wait.ToString();
            txtS_OffP_X_PickL.Text = CData.SPos.dOFP_X_PickL.ToString();
            txtS_OffP_X_PickR.Text = CData.SPos.dOFP_X_PickR.ToString();
            txtS_OffP_X_Place.Text = CData.SPos.dOFP_X_Place.ToString();
            txtS_OffP_X_Conv.Text = CData.SPos.dOFP_X_Conv.ToString(); //190321 ksg :

            txtS_OffP_X_Picture1.Text = CData.SPos.dOFP_X_Picture1.ToString(); // 2021.03.30 lhs
            txtS_OffP_X_Picture2.Text = CData.SPos.dOFP_X_Picture2.ToString(); // 2021.03.30 lhs

            // Off Loader Picker Z
            txtS_OffP_Z_Wait.Text = CData.SPos.dOFP_Z_Wait.ToString();
            // 2021.02.27 SungTae Start : 고객사(ASE-KR) 요청으로 Device에서 관리 위해 변경
            if (CData.CurCompany == ECompany.ASE_KR     ||
                CData.CurCompany == ECompany.SCK        ||  // 2021.07.14 lhs  SCK, JSCK 추가
                CData.CurCompany == ECompany.JSCK       ||
                CData.CurCompany == ECompany.SkyWorks   ||  // 220216 pjh : Skyworks 추가
                CDataOption.UseSprayBtmCleaner          )   // 2022.03.08 lhs Spray Nozzle Bottom Cleaner 추가 
            {
                txtS_OffP_Z_BtClnSt.Text    = CData.Dev.dOffP_Z_ClnStart.ToString();
                txtS_OffP_Z_StrpClnSt.Text  = CData.Dev.dOffP_Z_StripClnStart.ToString();    //20200424 jhc : Off-Picker Z축 Strip Clean 높이 별도 설정
            }
            else
            {
                txtS_OffP_Z_BtClnSt.Text    = CData.SPos.dOFP_Z_ClnStart.ToString();
                // 표시안함 //txtS_OffP_Z_StrpClnSt.Text  = CData.SPos.dOFP_Z_StripClnStart.ToString();    //20200424 jhc : Off-Picker Z축 Strip Clean 높이 별도 설정
            }
            // 2021.02.27 SungTae End

            txtS_OffP_Z_Picture.Text = CData.SPos.dOFP_Z_Picture.ToString();    // 2021.03.30 lhs


            // Dry X
            txtS_Dry_X_Wait.Text = CData.SPos.dDRY_X_Wait.ToString();
            txtS_Dry_X_Out.Text = CData.SPos.dDRY_X_Out.ToString();

            // Dry Z
            txtS_Dry_Z_Wait.Text = CData.SPos.dDRY_Z_Wait.ToString();
            txtS_Dry_Z_Up.Text = CData.SPos.dDRY_Z_Up.ToString();
            txtS_Dry_Z_Level.Text = CData.SPos.dDRY_Z_Check.ToString();
            CData.SPos.dDRY_Z_TipClamped = CData.SPos.dDRY_Z_Up - CData.Dev.dOffP_Z_PlaceDn; // 2021.04.22 lhs
            txtS_Dry_Z_TipClamped.Text = CData.SPos.dDRY_Z_TipClamped.ToString();                 // 2021.04.22 lhs
            txtS_Dry_Z_ChkStrip.Text = CData.SPos.dDRY_Z_ChkStrip.ToString();     // 2022.01.17 SungTae : [추가] (ASE-KR VOC) Strip 유무 체크 시작 위치를 별도로 설정할 있도록 고객사 요청 사항

            // Dry R
            txtS_Dry_R_Wait.Text = CData.SPos.dDRY_R_Wait.ToString();

            // Off Loader X
            txtS_OffL_X_Wait.Text = CData.SPos.dOFL_X_Wait.ToString();
            txtS_OffL_X_Align.Text = CData.SPos.dOFL_X_DefAlign.ToString();

            // Off Loader Y
            txtS_OffL_Y_Pick.Text = CData.SPos.dOFL_Y_Pick.ToString();
            txtS_OffL_Y_Place.Text = CData.SPos.dOFL_Y_Place.ToString();

            // Off Loader Z
            txtS_OffL_Z_Wait.Text = CData.SPos.dOFL_Z_Wait.ToString();
            txtS_OffL_Z_TcPickGo.Text = CData.SPos.dOFL_Z_TPickGo.ToString();
            txtS_OffL_Z_TcClamp.Text = CData.SPos.dOFL_Z_TClamp.ToString();
            txtS_OffL_Z_TcPickUp.Text = CData.SPos.dOFL_Z_TPickUp.ToString();
            txtS_OffL_Z_TcPlaceGo.Text = CData.SPos.dOFL_Z_TPlaceGo.ToString();
            txtS_OffL_Z_TcUnclamp.Text = CData.SPos.dOFL_Z_TUnClamp.ToString();
            txtS_OffL_Z_TcPlaceDn.Text = CData.SPos.dOFL_Z_TPlaceDn.ToString();
            txtS_OffL_Z_BtPickGo.Text = CData.SPos.dOFL_Z_BPickGo.ToString();
            txtS_OffL_Z_BtClamp.Text = CData.SPos.dOFL_Z_BClamp.ToString();
            txtS_OffL_Z_BtPickUp.Text = CData.SPos.dOFL_Z_BPickUp.ToString();
            txtS_OffL_Z_BtPlaceGo.Text = CData.SPos.dOFL_Z_BPlaceGo.ToString();
            txtS_OffL_Z_BtUnclamp.Text = CData.SPos.dOFL_Z_BUnClamp.ToString();
            txtS_OffL_Z_BtPlaceDn.Text = CData.SPos.dOFL_Z_BPlaceDn.ToString();

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
            // On Loader X
            // 대기 포지션
            double.TryParse(txtS_OnL_X_Wait.Text, out CData.SPos.dONL_X_Wait);
            double.TryParse(txtS_OnL_X_Align.Text, out CData.SPos.dONL_X_DefAlign);

            // On Loader Y
            // Pick 포지션
            double.TryParse(txtS_OnL_Y_Pick.Text, out CData.SPos.dONL_Y_Pick);
            // Place 포지션
            double.TryParse(txtS_OnL_Y_Place.Text, out CData.SPos.dONL_Y_Place);

            // On Loader Z
            double.TryParse(txtS_OnL_Z_Wait.Text, out CData.SPos.dONL_Z_Wait);
            double.TryParse(txtS_OnL_Z_PickGo.Text, out CData.SPos.dONL_Z_PickGo);
            double.TryParse(txtS_OnL_Z_Clamp.Text, out CData.SPos.dONL_Z_Clamp);
            double.TryParse(txtS_OnL_Z_PickUp.Text, out CData.SPos.dONL_Z_PickUp);
            double.TryParse(txtS_OnL_Z_PlaceGo.Text, out CData.SPos.dONL_Z_PlaceGo);
            double.TryParse(txtS_OnL_Z_Unclamp.Text, out CData.SPos.dONL_Z_UnClamp);
            double.TryParse(txtS_OnL_Z_PlaceDn.Text, out CData.SPos.dONL_Z_PlaceDn);

            // Inrail X
            double.TryParse(txtS_Inr_X_Wait.Text, out CData.SPos.dINR_X_Wait);
            double.TryParse(txtS_Inr_X_Check.Text, out CData.SPos.dINR_X_ChkUnit);  // 2021-06-01, jhLee : SPIL 2003U, Sensor Check 위치


            // InRail Y
            double.TryParse(txtS_Inr_Y_Wait.Text, out CData.SPos.dINR_Y_Wait);
            double.TryParse(txtS_Inr_Y_Align.Text, out CData.SPos.dINR_Y_Align);

            // On Loader Picker X
            double.TryParse(txtS_OnP_X_PlaceL.Text, out CData.SPos.dONP_X_PlaceL);
            double.TryParse(txtS_OnP_X_PlaceR.Text, out CData.SPos.dONP_X_PlaceR);
            double.TryParse(txtS_OnP_X_Conv.Text, out CData.SPos.dONP_X_Con);
            double.TryParse(txtS_OnP_X_WaitPickL.Text, out CData.SPos.dONP_X_WaitPickL); //20200406 jhc : OnPicker L-Table Pickup 대기 위치

            // On Loader Picker Z
            double.TryParse(txtS_OnP_Z_Wait.Text, out CData.SPos.dONP_Z_Wait);

            // On Loader Picker Y
            double.TryParse(txtS_OnP_Y_Wait.Text, out CData.SPos.dONP_Y_Wait);
            //

            // Grind Left X
            double.TryParse(txtS_GrdL_X_Wait.Text, out CData.SPos.dGRD_X_Wait[0]);
            double.TryParse(txtS_GrdL_X_Zero.Text, out CData.SPos.dGRD_X_Zero[0]);

            // Grind Left Y
            double.TryParse(txtS_GrdL_Y_Wait.Text, out CData.SPos.dGRD_Y_Wait[0]);
            double.TryParse(txtS_GrdL_Y_ClnSt.Text, out CData.SPos.dGRD_Y_ClnStart[0]);
            double.TryParse(txtS_GrdL_Y_ClnEd.Text, out CData.SPos.dGRD_Y_ClnEnd[0]);
            double.TryParse(txtS_GrdL_Y_DrsSt.Text, out CData.SPos.dGRD_Y_DrsStart[0]);
            double.TryParse(txtS_GrdL_Y_DrsEd.Text, out CData.SPos.dGRD_Y_DrsEnd[0]);
            double.TryParse(txtS_GrdL_Y_TblInsp.Text, out CData.SPos.dGRD_Y_TblInsp[0]);
            double.TryParse(txtS_GrdL_Y_DrsCh.Text, out CData.SPos.dGRD_Y_DrsChange[0]);
            // 2021.04.13 SungTae Start : Table Grinding 시 조작 실수 방지 위해 추가
            double.TryParse(txtS_GrdL_Y_TblGrdSt.Text, out CData.SPos.dGRD_Y_TblGrdSt[0]);
            double.TryParse(txtS_GrdL_Y_TblGrdEd.Text, out CData.SPos.dGRD_Y_TblGrdEd[0]);
            // 2021.04.13 SungTae End

            // Grind Left Z
            double.TryParse(txtS_GrdL_Z_Wait.Text, out CData.SPos.dGRD_Z_Wait[0]);
            double.TryParse(txtS_GrdL_Z_Able.Text, out CData.SPos.dGRD_Z_Able[0]);

            // Grind Right X
            double.TryParse(txtS_GrdR_X_Wait.Text, out CData.SPos.dGRD_X_Wait[1]);
            double.TryParse(txtS_GrdR_X_Zero.Text, out CData.SPos.dGRD_X_Zero[1]);

            // Grind Right Y
            double.TryParse(txtS_GrdR_Y_Wait.Text, out CData.SPos.dGRD_Y_Wait[1]);
            double.TryParse(txtS_GrdR_Y_ClnSt.Text, out CData.SPos.dGRD_Y_ClnStart[1]);
            double.TryParse(txtS_GrdR_Y_ClnEd.Text, out CData.SPos.dGRD_Y_ClnEnd[1]);
            double.TryParse(txtS_GrdR_Y_DrsSt.Text, out CData.SPos.dGRD_Y_DrsStart[1]);
            double.TryParse(txtS_GrdR_Y_DrsEd.Text, out CData.SPos.dGRD_Y_DrsEnd[1]);
            double.TryParse(txtS_GrdR_Y_TblInsp.Text, out CData.SPos.dGRD_Y_TblInsp[1]);
            double.TryParse(txtS_GrdR_Y_DrsCh.Text, out CData.SPos.dGRD_Y_DrsChange[1]);
            // 2021.04.13 SungTae Start : Table Grinding 시 조작 실수 방지 위해 추가
            double.TryParse(txtS_GrdR_Y_TblGrdSt.Text, out CData.SPos.dGRD_Y_TblGrdSt[1]);
            double.TryParse(txtS_GrdR_Y_TblGrdEd.Text, out CData.SPos.dGRD_Y_TblGrdEd[1]);
            // 2021.04.13 SungTae End

            // Grind Right Z
            double.TryParse(txtS_GrdR_Z_Wait.Text, out CData.SPos.dGRD_Z_Wait[1]);
            double.TryParse(txtS_GrdR_Z_Able.Text, out CData.SPos.dGRD_Z_Able[1]);

            // Off Loader Picker X
            double.TryParse(txtS_OffP_X_Wait.Text, out CData.SPos.dOFP_X_Wait);
            double.TryParse(txtS_OffP_X_PickL.Text, out CData.SPos.dOFP_X_PickL);
            double.TryParse(txtS_OffP_X_PickR.Text, out CData.SPos.dOFP_X_PickR);
            double.TryParse(txtS_OffP_X_Place.Text, out CData.SPos.dOFP_X_Place);
            double.TryParse(txtS_OffP_X_Conv.Text, out CData.SPos.dOFP_X_Conv); //190321 ksg :

            double.TryParse(txtS_OffP_X_Picture1.Text, out CData.SPos.dOFP_X_Picture1); // 2021.03.30 lhs
            double.TryParse(txtS_OffP_X_Picture2.Text, out CData.SPos.dOFP_X_Picture2); // 2021.03.30 lhs

            // Off Loader Picker Z
            double.TryParse(txtS_OffP_Z_Wait.Text, out CData.SPos.dOFP_Z_Wait);
            // 2021.02.27 SungTae Start : 고객사(ASE-KR) 요청으로 Device에서 관리 위해 변경
            if (CData.CurCompany == ECompany.ASE_KR     ||
                CData.CurCompany == ECompany.SCK        ||  // 2021.07.14 lhs  SCK, JSCK 추가
                CData.CurCompany == ECompany.JSCK       ||
                CData.CurCompany == ECompany.SkyWorks   ||  // 220216 pjh : Skyworks 추가
                CDataOption.UseSprayBtmCleaner          )   // 2022.03.08 lhs Spray Nozzle Bottom Cleaner 추가 
            {
                double.TryParse(txtS_OffP_Z_BtClnSt.Text,   out CData.Dev.dOffP_Z_ClnStart);
                double.TryParse(txtS_OffP_Z_StrpClnSt.Text, out CData.Dev.dOffP_Z_StripClnStart);   //20200424 jhc : Off-Picker Z축 Strip Clean 높이 별도 설정
            }
            else
            {
                double.TryParse(txtS_OffP_Z_BtClnSt.Text,   out CData.SPos.dOFP_Z_ClnStart);
                // 표시안함 //double.TryParse(txtS_OffP_Z_StrpClnSt.Text, out CData.SPos.dOFP_Z_StripClnStart);  // lhs 삭제 예정 //20200424 jhc : Off-Picker Z축 Strip Clean 높이 별도 설정
            }
            // 2021.02.27 SungTae End

            double.TryParse(txtS_OffP_Z_Picture.Text, out CData.SPos.dOFP_Z_Picture);    // 2021.03.30 lhs

            // Dry X
            double.TryParse(txtS_Dry_X_Wait.Text, out CData.SPos.dDRY_X_Wait);
            double.TryParse(txtS_Dry_X_Out.Text, out CData.SPos.dDRY_X_Out);

            // Dry Z
            double.TryParse(txtS_Dry_Z_Wait.Text,   out CData.SPos.dDRY_Z_Wait);
            double.TryParse(txtS_Dry_Z_Up.Text,     out CData.SPos.dDRY_Z_Up);
            double.TryParse(txtS_Dry_Z_Level.Text,  out CData.SPos.dDRY_Z_Check);
            CData.SPos.dDRY_Z_TipClamped = CData.SPos.dDRY_Z_Up - CData.Dev.dOffP_Z_PlaceDn; // 2021.04.22 lhs
            double.TryParse(txtS_Dry_Z_ChkStrip.Text, out CData.SPos.dDRY_Z_ChkStrip);    // 2022.01.17 SungTae : [추가] (ASE-KR VOC) Strip 유무 체크 시작 위치를 별도로 설정할 있도록 고객사 요청 사항

            // Dry R
            double.TryParse(txtS_Dry_R_Wait.Text, out CData.SPos.dDRY_R_Wait);

            // Off Loader X
            double.TryParse(txtS_OffL_X_Wait.Text, out CData.SPos.dOFL_X_Wait);
            double.TryParse(txtS_OffL_X_Align.Text, out CData.SPos.dOFL_X_DefAlign);

            // Off Loader Y
            double.TryParse(txtS_OffL_Y_Pick.Text, out CData.SPos.dOFL_Y_Pick);
            double.TryParse(txtS_OffL_Y_Place.Text, out CData.SPos.dOFL_Y_Place);

            // Off Loader Z
            double.TryParse(txtS_OffL_Z_Wait.Text, out CData.SPos.dOFL_Z_Wait);
            double.TryParse(txtS_OffL_Z_TcPickGo.Text, out CData.SPos.dOFL_Z_TPickGo);
            double.TryParse(txtS_OffL_Z_TcClamp.Text, out CData.SPos.dOFL_Z_TClamp);
            double.TryParse(txtS_OffL_Z_TcPickUp.Text, out CData.SPos.dOFL_Z_TPickUp);
            double.TryParse(txtS_OffL_Z_TcPlaceGo.Text, out CData.SPos.dOFL_Z_TPlaceGo);
            double.TryParse(txtS_OffL_Z_TcUnclamp.Text, out CData.SPos.dOFL_Z_TUnClamp);
            double.TryParse(txtS_OffL_Z_TcPlaceDn.Text, out CData.SPos.dOFL_Z_TPlaceDn);
            double.TryParse(txtS_OffL_Z_BtPickGo.Text, out CData.SPos.dOFL_Z_BPickGo);
            double.TryParse(txtS_OffL_Z_BtClamp.Text, out CData.SPos.dOFL_Z_BClamp);
            double.TryParse(txtS_OffL_Z_BtPickUp.Text, out CData.SPos.dOFL_Z_BPickUp);
            double.TryParse(txtS_OffL_Z_BtPlaceGo.Text, out CData.SPos.dOFL_Z_BPlaceGo);
            double.TryParse(txtS_OffL_Z_BtUnclamp.Text, out CData.SPos.dOFL_Z_BUnClamp);
            double.TryParse(txtS_OffL_Z_BtPlaceDn.Text, out CData.SPos.dOFL_Z_BPlaceDn);
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

        private void _M_tmUpdt_Tick(object sender, EventArgs e)
        {
            // 설비가 자동 동작중에는 내용을 변경할 수 없게 한다. 
            // 2021-06-01, jhLee : Error 상태일 때로 위치 확인이 가능하도록 변경
            if (CSQ_Man.It.bBtnShow && (CSQ_Main.It.m_iStat == EStatus.Idle || CSQ_Main.It.m_iStat == EStatus.Stop || (CSQ_Main.It.m_iStat == EStatus.Error)))
            {
                CSQ_Man.It.bBtnShow = true;
                _EnableBtn(true);
            }

            #region Motion
            for (int i = 0; i < 3; i++)
            {
                int iAx;
                if (CDataOption.CurEqu == eEquType.Nomal)
                {
                    iAx = m_iAx1 + i;
                }
                else
                {
                    iAx = m_iAx1;
                    if      (i == 0) { iAx = m_iAx1; }
                    else if (i == 1) { iAx = m_iAx2; }
                    else             { iAx = m_iAx3; }
                }

                string sId = (i + 1).ToString();
                Panel mPnl = (Panel)Controls["pnlS_Ax" + sId];
                //
                //int iAx = m_iAx1 + i;
                //string sId = (i + 1).ToString();
                //Panel mPnl = (Panel)tp2_Sys.Controls["pnlS_Ax" + sId];
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

                    mPnl.Controls["lblS_Srv" + sId].BackColor = GV.CR_X[CMot.It.Chk_SrvI(iAx)];
                    mPnl.Controls["lblS_Al" + sId].BackColor = GV.CR_Y[CMot.It.Chk_AlrI(iAx)];
                    mPnl.Controls["lblS_HD" + sId].BackColor = GV.CR_X[CMot.It.Chk_HDI(iAx)];
                    mPnl.Controls["lblS_N" + sId].BackColor = GV.CR_X[CMot.It.Chk_NOTI(iAx)];
                    mPnl.Controls["lblS_H" + sId].BackColor = GV.CR_X[CMot.It.Chk_HomeI(iAx)];
                    mPnl.Controls["lblS_P" + sId].BackColor = GV.CR_X[CMot.It.Chk_POTI(iAx)];

                    mPnl.Controls["lblS_Cmd" + sId].Text = CMot.It.Get_CP(iAx).ToString();
                    mPnl.Controls["lblS_Enc" + sId].Text = CMot.It.Get_FP(iAx).ToString();
                }
            }
            #endregion

            #region IO
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
                    lblS_OnP_Rot0.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.ONP_Rotate0]];
                    lblS_OnP_Rot90.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.ONP_Rotate90]];
                    // Utility
                    lblS_OnP_Vac.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.ONP_Vacuum]];
                    // Load Cell
                    lblS_OnP_LodCell.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.ONP_LoadCell]];
                    //20190628 ghk_onpclean
                    lblS_OnP_CleanL.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.INR_OnpCleaner_L]];
                    lblS_OnP_CleanR.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.INR_OnpCleaner_R]];
                    //
                    break;

                case 3:
                    // Common
                    lblS_GrdL_FDor.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRD_DoorFront]];
                    lblS_GrdL_RDor.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRD_DoorRear]];
                    lblS_GrdL_FCovr.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRD_CoverFront]];
                    lblS_GrdL_RCovr.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRD_CoverRear]];
                    // Table
                    lblS_GrdL_TVac.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDL_TbVaccum]];
                    lblS_GrdL_TWtr.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDL_TbFlow]]; //201209 jhc : DEBUG
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
                        //lblS_GrdL_PumpA.Text = "Overload";
                        SetWriteLanguage(lblS_GrdL_PumpA.Name, CLang.It.GetLanguage("Overload"));
                    }
                    else if (CIO.It.aInput[(int)eX.PUMPL_FlowLow] == 1)
                    {
                        lblS_GrdL_PumpA.BackColor = GV.CR_Y[1];
                        //lblS_GrdL_PumpA.Text = "Flow Low";
                        SetWriteLanguage(lblS_GrdL_PumpA.Name, CLang.It.GetLanguage("Flow Low"));
                    }
                    else if (CIO.It.aInput[(int)eX.PUMPL_TempHigh] == 1)
                    {
                        lblS_GrdL_PumpA.BackColor = GV.CR_Y[1];
                        //lblS_GrdL_PumpA.Text = "Temp High";
                        SetWriteLanguage(lblS_GrdL_PumpA.Name, CLang.It.GetLanguage("Temp High"));
                    }
                    else
                    {
                        lblS_GrdL_PumpA.BackColor = GV.CR_X[1];
                        //lblS_GrdL_PumpA.Text = "No Alarm";
                        SetWriteLanguage(lblS_GrdL_PumpA.Name, CLang.It.GetLanguage("No Alarm"));
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
                    lblS_GrdL_GrdTmp.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDL_SplWaterTemp]];
                    lblS_GrdL_BtmFl.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDL_SplBtmWater]];
                    // Spindle
                    lblS_GrdL_PcwFl.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDL_SplPCW]];
                    lblS_GrdL_PcwTmp.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDL_SplPCWTemp]];

                    //20191022 ghk_spindle_type
                    //if (CSpl.It.bUse232)
                    //if (CDataOption.SplType == eSpindleType.Rs232)
                    //{
                    //    lblS_GrdL_Rpm.Text = CData.Spls[0].iRpm.ToString("0.0") + " rpm";   // Right 와 format 맞춤
                    //    lblS_GrdL_Load.Text = CData.Spls[0].dLoad.ToString("0.0") + " %";
                    //}
                    //else
                    //{
                    //    lblS_GrdL_Rpm.Text = CSpl.It.GetFrpm(true).ToString("0.0") + " rpm";
                    //    lblS_GrdL_Load.Text = CData.Spls[0].dLoad.ToString("0.0") + " %";
                    //}
                    // 2023.03.15 Max
                    lblS_GrdL_Rpm.Text = CData.Spls[0].iRpm.ToString("0.0") + " rpm";   // Right 와 format 맞춤
                    lblS_GrdL_Load.Text = CData.Spls[0].dLoad.ToString("0.0") + " %";
                    break;

                case 4:
                    // Common
                    lblS_GrdR_FDor.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRD_DoorFront]];
                    lblS_GrdR_RDor.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRD_DoorRear]];
                    lblS_GrdR_FCovr.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRD_CoverFront]];
                    lblS_GrdR_RCovr.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRD_CoverRear]];
                    // Table
                    lblS_GrdR_TVac.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDR_TbVaccum]];
                    lblS_GrdR_TWtr.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.GRDR_TbFlow]]; //201209 jhc : DEBUG
                    lblS_GrdR_PumpR.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.PUMPR_Run]];

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
                        lblS_GrdR_PumpA.Text = "Overload";
                        SetWriteLanguage(lblS_GrdR_PumpA.Name, CLang.It.GetLanguage("Overload"));
                    }
                    else if (CIO.It.aInput[(int)eX.PUMPR_FlowLow] == 1)
                    {
                        lblS_GrdR_PumpA.BackColor = GV.CR_Y[1];
                        //lblS_GrdR_PumpA.Text = "Flow Low";
                        SetWriteLanguage(lblS_GrdR_PumpA.Name, CLang.It.GetLanguage("Flow Low"));
                    }
                    else if (CIO.It.aInput[(int)eX.PUMPR_TempHigh] == 1)
                    {
                        lblS_GrdR_PumpA.BackColor = GV.CR_Y[1];
                        lblS_GrdR_PumpA.Text = "Temp High";
                        SetWriteLanguage(lblS_GrdR_PumpA.Name, CLang.It.GetLanguage("Temp High"));
                    }
                    else
                    {
                        lblS_GrdR_PumpA.BackColor = GV.CR_X[1];
                        lblS_GrdR_PumpA.Text = "No Alarm";
                        SetWriteLanguage(lblS_GrdR_PumpA.Name, CLang.It.GetLanguage("No Alarm"));
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
                    lblS_GrdR_Rpm.Text = CData.Spls[1].iRpm.ToString() + " rpm";
                    lblS_GrdR_Load.Text = CData.Spls[1].dLoad.ToString("0.0") + " %";
                    break;

                case 5:
                    lblS_OffP_Rt0.BackColor         = GV.CR_X[CIO.It.aInput[(int)eX.OFFP_Rotate0]];
                    lblS_OffP_Rt90.BackColor        = GV.CR_X[CIO.It.aInput[(int)eX.OFFP_Rotate90]];
                    lblS_OffP_Vac.BackColor         = GV.CR_X[CIO.It.aInput[(int)eX.OFFP_Vacuum]];
                    lblS_OffP_Lcell.BackColor       = GV.CR_X[CIO.It.aInput[(int)eX.OFFP_LoadCell]];
                    // 2021.04.19 lhs Start
                    lblS_OffP_IV2_OK.BackColor      = GV.CR_X[CIO.It.aInput[(int)eX.OFFP_IV2_AllOK]];
                    lblS_OffP_IV2_Busy.BackColor    = GV.CR_X[CIO.It.aInput[(int)eX.OFFP_IV2_Busy]];
                    lblS_OffP_IV2_Error.BackColor   = GV.CR_X[CIO.It.aInput[(int)eX.OFFP_IV2_Error]];
                    //int nIdx = 0;
                    //if (CIO.It.aInput[(int)eX.OFFP_IV2_Error] == 0) nIdx = 1;
                    //lblS_OffP_IV2_Error.BackColor   = GV.CR_X[nIdx];
                    // 2021.04.19 lhs End

                    //211005 syc : 2004U
                    lblS_OffP_ClampClose.BackColor      = GV.CR_X[CIO.It.aInput[(int)eX.OFFP_Carrier_Clamp_Close]];
                    lblS_OffP_ClampOpen.BackColor       = GV.CR_X[CIO.It.aInput[(int)eX.OFFP_Carrier_Clamp_Open]];
                    lblS_OffP_CarrierDetect.BackColor   = GV.CR_X[CIO.It.aInput[(int)eX.OFFP_Picker_Carrier_Detect]];
                    //
                    break;

                case 6:
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
                        lblS_Dry_BtmClnNzlForward.BackColor  = GV.CR_X[CIO.It.aInput[(int)eX.DRY_ClnBtmNzlForward]];
                        lblS_Dry_BtmClnNzlBackward.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.DRY_ClnBtmNzlBackward]];
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
                    lblS_OffL_BtFull.BackColor = GV.CR_X[CIO.It.aInput[(int)eX.OFFL_TopMGZDetectFull]];
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
            #endregion
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

        #endregion

        /// <summary>
        /// 축 이동버튼 클릭 이벤트 (조그 제외한 스탭 이동 시)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnS_MvStep_Click(object sender, EventArgs e)
        {
            CData.bLastClick = true; //190509 ksg :
            //190430 ksg : Auto & Manual 때 동작 안함
            if (CSQ_Main.It.m_iStat == EStatus.Auto_Running || CSQ_Main.It.m_iStat == EStatus.Manual) return;

            Button mBtn = sender as Button;
            int iRet = 0;
            int iAx = int.Parse(mBtn.Parent.Tag.ToString()) + m_iAx1;
            if (CDataOption.CurEqu == eEquType.Pikcer)
            {
                int iTag = int.Parse(mBtn.Parent.Tag.ToString());
                if      (iTag == 0) { iAx = m_iAx1; }
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
                double dStp = 0.0;
                if (m_sStep == "Custom")
                { double.TryParse(txtS_Cus.Text, out dStp); }
                else
                { double.TryParse(m_sStep, out dStp); }

                //190214 ksg :
                if (iAx == 14 || iAx == 4)
                {
                    if (mBtn.Tag.ToString() == "N") { dStp *= 1; }
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

                    CMsg.Show(eMsg.Notice, "Notice", "Step move ok");
                }
            }
        }

        /// <summary>
        /// 스탑 버튼 클릭 이벤트
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
                int iTag = int.Parse(mBtn.Parent.Tag.ToString());
                if      (iTag == 0) { iAx = m_iAx1; }
                else if (iTag == 1) { iAx = m_iAx2; }
                else                { iAx = m_iAx3; }
                //
            }

            iRet = CMot.It.Stop(iAx);
            if (iRet != 0)
            {
                //CErr.Show()
            }
        }

        /// <summary>
        /// 축 이동버튼 마우스 다운 이벤트 (조그 이동 시)
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
                int iTag = int.Parse(mBtn.Parent.Tag.ToString());
                if      (iTag == 0) { iAx = m_iAx1; }
                else if (iTag == 1) { iAx = m_iAx2; }
                else                { iAx = m_iAx3; }
                //
            }

            string sTxt = mBtn.Tag.ToString();

            if (m_sStep == "Jog")
            {
                //if (sTxt == "N")
                //{ bPot = false; }
                //190214 ksg :
                if (iAx == 14 || iAx == 4)
                {
                    if (sTxt == "N")    bPot = true;
                    else                bPot = false;
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
        /// 축 이동버튼 마우스 업 이벤트 (조그 이동 시)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnS_Mv_MouseUp(object sender, MouseEventArgs e)
        {
            Button mBtn = sender as Button;

            int iAx = int.Parse(mBtn.Parent.Tag.ToString()) + m_iAx1;

            if (CDataOption.CurEqu == eEquType.Pikcer)
            {
                int iTag = int.Parse(mBtn.Parent.Tag.ToString());
                if      (iTag == 0) { iAx = m_iAx1; }
                else if (iTag == 1) { iAx = m_iAx2; }
                else                { iAx = m_iAx3; }
                //
            }

            string sTxt = mBtn.Tag.ToString();

            CMot.It.EStop(iAx);
        }

        /// <summary>
        /// 포지션 Get 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnS_Part_Get_Click(object sender, EventArgs e)
        {
            Button mBtn = sender as Button;

            int iAx = int.Parse(mBtn.Parent.Tag.ToString()) + m_iAx1;
            if (CDataOption.CurEqu == eEquType.Pikcer)  //200707 pjh : eEquType.Nomal > eEquType.Pikcer으로 수정
            {
                int iTag = int.Parse(mBtn.Parent.Tag.ToString());
                if      (iTag == 0) { iAx = m_iAx1; }
                else if (iTag == 1) { iAx = m_iAx2; }
                else                { iAx = m_iAx3; }
                //
            }

            string sTxt = mBtn.Tag.ToString();

            mBtn.Parent.Controls[sTxt].Text = CMot.It.Get_FP(iAx).ToString();
        }

        /// <summary>
        /// 포지션 Move 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnS_Part_Mv_Click(object sender, EventArgs e)
        {
            //190430 ksg : Auto & Manual 때 동작 안함
            if (CSQ_Main.It.m_iStat == EStatus.Auto_Running || CSQ_Main.It.m_iStat == EStatus.Manual) return;

            Button mBtn = sender as Button;

            int iRt = 0;
            int iAx = int.Parse(mBtn.Parent.Tag.ToString()) + m_iAx1;

            if (CDataOption.CurEqu == eEquType.Pikcer)  // 2022.03.04 lhs : 오류 수정 Normal -> Picker
            {
                int iTag = int.Parse(mBtn.Parent.Tag.ToString());
                if      (iTag == 0) { iAx = m_iAx1; }
                else if (iTag == 1) { iAx = m_iAx2; }
                else                { iAx = m_iAx3; }
                //
            }

            string sTxt = mBtn.Tag.ToString();
            double dPos = 0;

            //190615 pjh:
            //Dry Stick Check

            //if (iAx == 17)
            if (iAx == (int)EAx.DryZone_Z)  // 2022.07.07 lhs : 17 -> DryZone_Z
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

            if (!double.TryParse(mBtn.Parent.Controls[sTxt].Text, out dPos))
            {
                // 포지션 더블형 변환 에러
                // Error
            }
            else
            {
                //190322 ksg :
                //if (!CheckStatus(iAx)) return;
                if (!CheckStatus(iAx, dPos)) return;  //syc : Picker X 이동시 확인 구문 추가 
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
        /// Set Position 탭내에 파트 나뉘는 탭의 인덱스 체인지
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
                    /*
                    btnS_MvN1.Text = "LEFT";
                    btnS_MvP1.Text = "RIGHT";
                    btnS_MvN2.Text = "FRONT";
                    btnS_MvP2.Text = "REAR";
                    btnS_MvN3.Text = "UP";
                    btnS_MvP3.Text = "DOWN";
                    */
                    SetWriteLanguage(btnS_MvN1.Name, CLang.It.GetLanguage("LEFT"));
                    SetWriteLanguage(btnS_MvP1.Name, CLang.It.GetLanguage("RIGHT"));
                    SetWriteLanguage(btnS_MvN2.Name, CLang.It.GetLanguage("FRONT"));
                    SetWriteLanguage(btnS_MvP2.Name, CLang.It.GetLanguage("REAR"));
                    SetWriteLanguage(btnS_MvN3.Name, CLang.It.GetLanguage("UP"));
                    SetWriteLanguage(btnS_MvP3.Name, CLang.It.GetLanguage("DOWN"));


                    ckbS_OnL_Lock.Checked = CIO.It.Get_Y(eY.ONL_DoorLock);
                    ckbS_OnL_TpRun.Checked = CIO.It.Get_Y(eY.ONL_TopBeltRun);
                    ckbS_OnL_BtCCW.Checked = CIO.It.Get_Y(eY.ONL_BtmBeltCCW);
                    ckbS_OnL_BtRun.Checked = CIO.It.Get_Y(eY.ONL_BtmBeltRun);
                    ckbS_OnL_Forw.Checked = CIO.It.Get_Y(eY.ONL_PusherForward);
                    ckbS_OnL_ClmpOn.Checked = CIO.It.Get_Y(eY.ONL_ClampOn);
                    ckbS_OnL_ClmpOff.Checked = CIO.It.Get_Y(eY.ONL_ClampOff);

                    tabOnL.SelectTab(0);
                    break;

                case 1:
                    m_iAx1 = (int)EAx.Inrail_X;
                    m_iAx2 = (int)EAx.Inrail_Y;
                    m_bAx3 = false;
                    /*
                    btnS_MvN1.Text = "LEFT";
                    btnS_MvP1.Text = "RIGHT";
                    btnS_MvN2.Text = "FRONT";
                    btnS_MvP2.Text = "REAR";
                    */
                    SetWriteLanguage(btnS_MvN1.Name, CLang.It.GetLanguage("LEFT"));
                    SetWriteLanguage(btnS_MvP1.Name, CLang.It.GetLanguage("RIGHT"));
                    SetWriteLanguage(btnS_MvN2.Name, CLang.It.GetLanguage("FRONT"));
                    SetWriteLanguage(btnS_MvP2.Name, CLang.It.GetLanguage("REAR"));


                    CkbS_Inr_Forw.Checked = CIO.It.Get_Y(eY.INR_GuideForward);
                    CkbS_Inr_Clamp.Checked = CIO.It.Get_Y(eY.INR_GripperClampOn);
                    CkbS_Inr_Up.Checked = CIO.It.Get_Y(eY.INR_LiftUp);

                    break;

                case 2:
                    m_iAx1 = (int)EAx.OnLoaderPicker_X;
                    m_iAx2 = (int)EAx.OnLoaderPicker_Z;

                    if (CDataOption.CurEqu == eEquType.Pikcer)
                    {
                        m_iAx3 = (int)EAx.OnLoaderPicker_Y;
                        m_bAx3 = true;
                    }
                    else
                    { m_bAx3 = false; }
                    /*
                    btnS_MvN1.Text = "LEFT";
                    btnS_MvP1.Text = "RIGHT";
                    btnS_MvN2.Text = "UP";
                    btnS_MvP2.Text = "DOWN";
                    */
                    SetWriteLanguage(btnS_MvN1.Name, CLang.It.GetLanguage("LEFT"));
                    SetWriteLanguage(btnS_MvP1.Name, CLang.It.GetLanguage("RIGHT"));
                    SetWriteLanguage(btnS_MvN2.Name, CLang.It.GetLanguage("UP"));
                    SetWriteLanguage(btnS_MvP2.Name, CLang.It.GetLanguage("DOWN"));
                    if (m_bAx3)
                    {
                        SetWriteLanguage(btnS_MvN3.Name, CLang.It.GetLanguage("FRONT"));
                        SetWriteLanguage(btnS_MvP3.Name, CLang.It.GetLanguage("REAR"));
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
                    ckbS_OnP_Ejt.Checked = CIO.It.Get_Y(eY.ONP_Ejector);
                    ckbS_OnP_Drn.Checked = CIO.It.Get_Y(eY.ONP_Drain);

                    break;

                case 3:
                    m_iAx1 = (int)EAx.LeftGrindZone_X;
                    m_iAx2 = (int)EAx.LeftGrindZone_Y;
                    m_iAx3 = (int)EAx.LeftGrindZone_Z;
                    m_bAx3 = true;
                    /*
                    btnS_MvN1.Text = "LEFT";
                    btnS_MvP1.Text = "RIGHT";
                    btnS_MvN2.Text = "FRONT";
                    btnS_MvP2.Text = "REAR";
                    btnS_MvN3.Text = "UP";
                    btnS_MvP3.Text = "DOWN";
                    */
                    SetWriteLanguage(btnS_MvN1.Name, CLang.It.GetLanguage("LEFT"));
                    SetWriteLanguage(btnS_MvP1.Name, CLang.It.GetLanguage("RIGHT"));
                    SetWriteLanguage(btnS_MvN2.Name, CLang.It.GetLanguage("FRONT"));
                    SetWriteLanguage(btnS_MvP2.Name, CLang.It.GetLanguage("REAR"));
                    SetWriteLanguage(btnS_MvN3.Name, CLang.It.GetLanguage("UP"));
                    SetWriteLanguage(btnS_MvP3.Name, CLang.It.GetLanguage("DOWN"));

                    ckbS_GrdL_Lock.Checked = CIO.It.Get_Y(eY.GRD_DoorLock);
                    ckbS_GrdL_FWtr.Checked = CIO.It.Get_Y(eY.GRD_FrontWater);
                    ckbS_GrdL_TWtr.Checked = CIO.It.Get_Y(eY.GRDL_TbFlow);
                    ckbS_GrdL_TEjt.Checked = CIO.It.Get_Y(eY.GRDL_TbEjector);
                    ckbS_GrdL_TVac.Checked = CIO.It.Get_Y(eY.GRDL_TbVacuum);
                    ckbS_GrdL_PumpR.Checked = CIO.It.Get_Y(eY.PUMPL_Run);
                    ckbS_GrdL_TcSpng.Checked = CIO.It.Get_Y(eY.GRDL_TopClnFlow);
                    ckbS_GrdL_TcAir.Checked = CIO.It.Get_Y(eY.GRDL_TopClnAir);
                    ckbS_GrdL_TcDn.Checked = CIO.It.Get_Y(eY.GRDL_TopClnDn);
                    ckbS_GrdL_TcKnif.Checked = CIO.It.Get_Y(eY.GRDL_TopWaterKnife);
                    ckbS_GrdL_PrbAir.Checked = CIO.It.Get_Y(eY.GRDL_ProbeAir);
                    ckbS_GrdL_PrbDn.Checked = CIO.It.Get_Y(eY.GRDL_ProbeDn);
                    ckbS_GrdL_GrdWtr.Checked = CIO.It.Get_Y(eY.GRDL_SplWater);
                    ckbS_GrdL_BtmWtr.Checked = CIO.It.Get_Y(eY.GRDL_SplBtmWater);
                    ckbS_GrdL_Pcw.Checked = CIO.It.Get_Y(eY.GRDL_SplPCW);
                    ckbS_GrdL_Cda.Checked = CIO.It.Get_Y(eY.GRDL_SplCDA);

                    break;

                case 4:
                    m_iAx1 = (int)EAx.RightGrindZone_X;
                    m_iAx2 = (int)EAx.RightGrindZone_Y;
                    m_iAx3 = (int)EAx.RightGrindZone_Z;
                    m_bAx3 = true;
                    /*
                    btnS_MvN1.Text = "LEFT";
                    btnS_MvP1.Text = "RIGHT";
                    btnS_MvN2.Text = "FRONT";
                    btnS_MvP2.Text = "REAR";
                    btnS_MvN3.Text = "UP";
                    btnS_MvP3.Text = "DOWN";
                    */
                    SetWriteLanguage(btnS_MvN1.Name, CLang.It.GetLanguage("LEFT"));
                    SetWriteLanguage(btnS_MvP1.Name, CLang.It.GetLanguage("RIGHT"));
                    SetWriteLanguage(btnS_MvN2.Name, CLang.It.GetLanguage("FRONT"));
                    SetWriteLanguage(btnS_MvP2.Name, CLang.It.GetLanguage("REAR"));
                    SetWriteLanguage(btnS_MvN3.Name, CLang.It.GetLanguage("UP"));
                    SetWriteLanguage(btnS_MvP3.Name, CLang.It.GetLanguage("DOWN"));

                    ckbS_GrdR_Lock.Checked = CIO.It.Get_Y(eY.GRD_DoorLock);
                    ckbS_GrdR_FWtr.Checked = CIO.It.Get_Y(eY.GRD_FrontWater);
                    ckbS_GrdR_TWtr.Checked = CIO.It.Get_Y(eY.GRDR_TbFlow);
                    ckbS_GrdR_TEjt.Checked = CIO.It.Get_Y(eY.GRDR_TbEjector);
                    ckbS_GrdR_TVac.Checked = CIO.It.Get_Y(eY.GRDR_TbVacuum);
                    // 2020.10.28 JSKim St
                    //ckbS_GrdR_PumpR.Checked = CIO.It.Get_Y(eY.PUMPL_Run);
                    ckbS_GrdR_PumpR.Checked = CIO.It.Get_Y(eY.PUMPR_Run);
                    // 2020.10.28 JSKim Ed
                    ckbS_GrdR_TcSpng.Checked = CIO.It.Get_Y(eY.GRDR_TopClnFlow);
                    ckbS_GrdR_TcAir.Checked = CIO.It.Get_Y(eY.GRDR_TopClnAir);
                    ckbS_GrdR_TcDn.Checked = CIO.It.Get_Y(eY.GRDR_TopClnDn);
                    ckbS_GrdR_TcKnif.Checked = CIO.It.Get_Y(eY.GRDR_TopWaterKnife);
                    ckbS_GrdR_PrbAir.Checked = CIO.It.Get_Y(eY.GRDR_ProbeAir);
                    ckbS_GrdR_PrbDn.Checked = CIO.It.Get_Y(eY.GRDR_ProbeDn);
                    ckbS_GrdR_GrdWtr.Checked = CIO.It.Get_Y(eY.GRDR_SplWater);
                    ckbS_GrdR_BtmWtr.Checked = CIO.It.Get_Y(eY.GRDR_SplBtmWater);
                    ckbS_GrdR_Pcw.Checked = CIO.It.Get_Y(eY.GRDR_SplPCW);
                    ckbS_GrdR_Cda.Checked = CIO.It.Get_Y(eY.GRDR_SplCDA);

                    break;

                case 5:
                    m_iAx1 = (int)EAx.OffLoaderPicker_X;
                    m_iAx2 = (int)EAx.OffLoaderPicker_Z;
                    m_bAx3 = false;
                    /*
                    btnS_MvN1.Text = "LEFT";
                    btnS_MvP1.Text = "RIGHT";
                    btnS_MvN2.Text = "UP";
                    btnS_MvP2.Text = "DOWN";
                    */
                    SetWriteLanguage(btnS_MvN1.Name, CLang.It.GetLanguage("LEFT"));
                    SetWriteLanguage(btnS_MvP1.Name, CLang.It.GetLanguage("RIGHT"));
                    SetWriteLanguage(btnS_MvN2.Name, CLang.It.GetLanguage("UP"));
                    SetWriteLanguage(btnS_MvP2.Name, CLang.It.GetLanguage("DOWN"));

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
                    ckbS_OffP_Ejt.Checked = CIO.It.Get_Y(eY.OFFP_Ejector);
                    ckbS_OffP_Drn.Checked = CIO.It.Get_Y(eY.OFFP_Drain);

                    break;

                case 6:
                    m_iAx1 = (int)EAx.DryZone_X;
                    m_iAx2 = (int)EAx.DryZone_Z;
                    m_iAx3 = (int)EAx.DryZone_Air;
                    m_bAx3 = true;
                    /*
                    btnS_MvN1.Text = "LEFT";
                    btnS_MvP1.Text = "RIGHT";
                    btnS_MvN2.Text = "DOWN";
                    btnS_MvP2.Text = "UP";
                    btnS_MvN3.Text = "CW";
                    btnS_MvP3.Text = "CCW";
                    */
                    SetWriteLanguage(btnS_MvN1.Name, CLang.It.GetLanguage("LEFT"));
                    SetWriteLanguage(btnS_MvP1.Name, CLang.It.GetLanguage("RIGHT"));
                    SetWriteLanguage(btnS_MvN2.Name, CLang.It.GetLanguage("DOWN"));
                    SetWriteLanguage(btnS_MvP2.Name, CLang.It.GetLanguage("UP"));
                    SetWriteLanguage(btnS_MvN3.Name, CLang.It.GetLanguage("CW"));
                    SetWriteLanguage(btnS_MvP3.Name, CLang.It.GetLanguage("CCW"));

                    ckbS_Dry_OutAir.Checked     = CIO.It.Get_Y(eY.DRY_OutRailAir);
                    ckbS_Dry_TcAir.Checked      = CIO.It.Get_Y(eY.DRY_TopAir);
                    ckbS_Dry_BtmAir.Checked     = CIO.It.Get_Y(eY.DRY_BtmAir);
                    ckbS_Dry_Wtr.Checked        = CIO.It.Get_Y(eY.DRY_ClnBtmFlow);
                    ckbS_Dry_ClampOpen.Checked  = CIO.It.Get_Y(eY.DRY_ClampOpenOnOff);

                    break;

                case 7:
                    m_iAx1 = (int)EAx.OffLoader_X;
                    m_iAx2 = (int)EAx.OffLoader_Y;
                    m_iAx3 = (int)EAx.OffLoader_Z;
                    m_bAx3 = true;
                    /*
                    btnS_MvN1.Text = "RIGHT";
                    btnS_MvP1.Text = "LEFT";
                    btnS_MvN2.Text = "FRONT";
                    btnS_MvP2.Text = "REAR";
                    btnS_MvN3.Text = "UP";
                    btnS_MvP3.Text = "DOWN";
                    */
                    SetWriteLanguage(btnS_MvN1.Name, CLang.It.GetLanguage("RIGHT"));
                    SetWriteLanguage(btnS_MvP1.Name, CLang.It.GetLanguage("LEFT"));
                    SetWriteLanguage(btnS_MvN2.Name, CLang.It.GetLanguage("FRONT"));
                    SetWriteLanguage(btnS_MvP2.Name, CLang.It.GetLanguage("REAR"));
                    SetWriteLanguage(btnS_MvN3.Name, CLang.It.GetLanguage("UP"));
                    SetWriteLanguage(btnS_MvP3.Name, CLang.It.GetLanguage("DOWN"));

                    ckbS_OffL_Lock.Checked = CIO.It.Get_Y(eY.OFFL_DoorLock);
                    ckbS_OffL_TpRun.Checked = CIO.It.Get_Y(eY.OFFL_TopBeltRun);
                    ckbS_OffL_MiCCW.Checked = CIO.It.Get_Y(eY.OFFL_MidBeltCCW);
                    ckbS_OffL_MiRun.Checked = CIO.It.Get_Y(eY.OFFL_MidBeltRun);
                    ckbS_OffL_BtRun.Checked = CIO.It.Get_Y(eY.OFFL_BtmBeltRun);
                    ckbS_OffL_TpClmpOn.Checked = CIO.It.Get_Y(eY.OFFL_TopClampOn);
                    ckbS_OffL_TpClmpOff.Checked = CIO.It.Get_Y(eY.OFFL_TopClampOff);
                    ckbS_OffL_TpCylUp.Checked = CIO.It.Get_Y(eY.OFFL_TopMGZUp);
                    ckbS_OffL_TpCylDn.Checked = CIO.It.Get_Y(eY.OFFL_TopMGZDn);
                    ckbS_OffL_BtClmpOn.Checked = CIO.It.Get_Y(eY.OFFL_BtmClampOn);
                    ckbS_OffL_BtClmpOff.Checked = CIO.It.Get_Y(eY.OFFL_BtmClampOff);
                    ckbS_OffL_BtCylUp.Checked = CIO.It.Get_Y(eY.OFFL_BtmMGZUp);
                    ckbS_OffL_BtCylDn.Checked = CIO.It.Get_Y(eY.OFFL_BtmMGZDn);

                    break;
            }
        }

        /// <summary>
        /// 라디오 버튼 인덱스 체인지 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdbS_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton mRdb = sender as RadioButton;
            m_sStep = mRdb.Tag.ToString();
        }

        private void ckbS_Output_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void ckbS_Output_Click(object sender, EventArgs e)
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
                }
                //

                CIO.It.Set_Y(eDO, mCkb.Checked);
            }
        }

        private void button116_Click(object sender, EventArgs e)
        {//20200113 myk_ONP_Rotate_InterLock
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
        {//20200113 myk_ONP_Rotate_InterLock
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
        {//20200113 myk_OFP_Rotate_InterLock
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
        {//20200113 myk_OFP_Rotate_InterLock
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

        public bool CheckStatus(int iAxis, double dPos = 0)
        {
            bool IsOk = true;
            bool   bX1     , bX2;
            int    iAxis1  , iAxis2  , iAxis3  , iAxis4  ;
            double dPos1, dPos2, dPos3, dPos4, dPos5, dPos6, dPos7, dPos8, dPos9;
            double dCurPos1, dCurPos2, dCurPos3, dCurPos4;
            switch (iAxis)
            {
                case (int)EAx.Inrail_X:
                    {
                        iAxis1 = (int)EAx.OnLoaderPicker_X;
                        iAxis2 = (int)EAx.OnLoaderPicker_Z;
                        dPos1 = CData.Dev.dOnP_X_Wait;
                        dPos2 = CData.Dev.dOnP_Z_Pick - CData.Dev.dOnP_Z_Slow;
                        dCurPos1 = CMot.It.Get_FP(iAxis1);
                        dCurPos2 = CMot.It.Get_FP(iAxis2);

                        //if ((dPos1 == dCurPos1) && (dCurPos2 >= (dPos2 - 10)))
                        if (CMot.It.Get_Mv(iAxis1, dPos1) && (dCurPos2 >= (dPos2 - 10))) //201103 pjh : 조건 변경
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Check Onloader Picker");
                            IsOk = false;
                        }
                        break;
                    }

                case (int)EAx.OnLoaderPicker_X:
                    {
                        iAxis1 = (int)EAx.OnLoaderPicker_Z;
                        iAxis2 = (int)EAx.OffLoaderPicker_X;
                        iAxis3 = (int)EAx.OnLoaderPicker_X; //syc : Picker X 이동시 확인 구문 추가

                        dPos1 = CData.SPos.dONP_Z_Wait;
                        dPos2 = CData.Dev.dOffP_X_ClnStart;

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

                case (int)EAx.OnLoaderPicker_Z:
                    {
                        iAxis1 = (int)EAx.OnLoaderPicker_X;
                        iAxis2 = (int)EAx.LeftGrindZone_Y;
                        iAxis3 = (int)EAx.RightGrindZone_Y;
                        iAxis4 = (int)EAx.Inrail_X;

                        dPos1 = CData.SPos.dONP_X_PlaceL;
                        dPos2 = CData.SPos.dONP_X_PlaceR;
                        dPos3 = CData.Dev.dOnP_X_Wait;
                        dPos4 = CData.SPos.dGRD_Y_Wait[0];
                        dPos5 = CData.SPos.dGRD_Y_Wait[1];
                        dPos6 = CData.SPos.dINR_X_Wait;
                        dPos7 = CData.SPos.dONP_X_WaitPickL; //20200406 jhc : OnPicker L-Table Pickup 대기 위치 
                        //dPos8 = CData.SPos.dINR_X_ChkUnit;      // 2021-06-01, jhLee : SPIL 2003U, Sensor Check 위치

                        dCurPos1 = CMot.It.Get_FP(iAxis1);
                        dCurPos2 = CMot.It.Get_FP(iAxis2);
                        dCurPos3 = CMot.It.Get_FP(iAxis3);
                        dCurPos4 = CMot.It.Get_FP(iAxis4);

                        //if (dCurPos1 == dPos1)
                        if (CMot.It.Get_Mv(iAxis1, dPos1)) //201103 pjh : 조건 변경
                        {
                            //if (dCurPos2 != dPos4)
                            if (!CMot.It.Get_Mv(iAxis2, dPos4)) //201103 pjh : 조건 변경
                            {
                                CMsg.Show(eMsg.Warning, "Warning", "Check Onloader Picker X Axis Pos");
                                IsOk = false;
                            }
                        }
                        //else if (dCurPos1 == dPos2)
                        else if (CMot.It.Get_Mv(iAxis1, dPos2)) //201103 pjh : 조건 변경
                        {
                            //if (dCurPos3 != dPos5)
                            if (!CMot.It.Get_Mv(iAxis3, dPos5)) //201103 pjh : 조건 변경
                            {
                                CMsg.Show(eMsg.Warning, "Warning", "Check Onloader Picker X Axis Pos");
                                IsOk = false;
                            }
                        }
                        //else if (dCurPos1 == dPos3)
                        else if (CMot.It.Get_Mv(iAxis1, dPos3)) //201103 pjh : 조건 변경
                        {
                            //if (dCurPos4 != dPos6)
                            if (!CMot.It.Get_Mv(iAxis4, dPos6)) //201103 pjh : 조건 변경
                            {
                                CMsg.Show(eMsg.Warning, "Warning", "Check Onloader Picker X Axis Pos");
                                IsOk = false;
                            }
                        }
                        //20200406 jhc : OnPicker L-Table Pickup 대기 위치
                        //else if (dCurPos1 == dPos7)
                        else if (CMot.It.Get_Mv(iAxis1, dPos7)) //201103 pjh : 조건 변경
                        {
                            //if (dCurPos2 != dPos4)
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

                case (int)EAx.LeftGrindZone_X:
                    {
                        iAxis1 = (int)EAx.LeftGrindZone_Z;
                        dPos1 = CData.SPos.dGRD_Z_Able[0];
                        dCurPos1 = CMot.It.Get_FP(iAxis1);
                        bX1 = CIO.It.Get_Y(eY.GRDL_ProbeDn);

                        if ((dCurPos1 > dPos1) || bX1)// && (dCurPos2 >= (dPos2-10)))
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Check Left Table Z Axis");
                            IsOk = false;
                        }
                        break;
                    }

                case (int)EAx.LeftGrindZone_Y:
                    {
                        iAxis1 = (int)EAx.OnLoaderPicker_Z;
                        iAxis2 = (int)EAx.OffLoaderPicker_Z;
                        iAxis3 = (int)EAx.LeftGrindZone_Z;
                        dPos1 = CData.SPos.dONP_Z_Wait;
                        dPos2 = CData.SPos.dOFP_Z_Wait;
                        dPos3 = CData.SPos.dGRD_Z_Able[0];
                        dCurPos1 = CMot.It.Get_FP(iAxis1);
                        dCurPos2 = CMot.It.Get_FP(iAxis2);
                        dCurPos3 = CMot.It.Get_FP(iAxis3);
                        bX1 = CIO.It.Get_Y(eY.GRDL_ProbeDn);

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

                case (int)EAx.RightGrindZone_X:
                    {
                        iAxis1 = (int)EAx.RightGrindZone_Z;
                        dPos1 = CData.SPos.dGRD_Z_Able[1];
                        dCurPos1 = CMot.It.Get_FP(iAxis1);
                        bX1 = CIO.It.Get_Y(eY.GRDR_ProbeDn);

                        if ((dCurPos1 > dPos1) || bX1)// && (dCurPos2 >= (dPos2-10)))
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Check Left Table Z Axis");
                            IsOk = false;
                        }
                        break;
                    }

                case (int)EAx.RightGrindZone_Y:
                    {
                        iAxis1 = (int)EAx.OnLoaderPicker_Z;
                        iAxis2 = (int)EAx.OffLoaderPicker_Z;
                        iAxis3 = (int)EAx.RightGrindZone_Z;
                        dPos1 = CData.SPos.dONP_Z_Wait;
                        dPos2 = CData.SPos.dOFP_Z_Wait;
                        dPos3 = CData.SPos.dGRD_Z_Able[1];
                        dCurPos1 = CMot.It.Get_FP(iAxis1);
                        dCurPos2 = CMot.It.Get_FP(iAxis2);
                        dCurPos3 = CMot.It.Get_FP(iAxis3);
                        bX1 = CIO.It.Get_Y(eY.GRDR_ProbeDn);

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
                            CMsg.Show(eMsg.Warning, "Warning", "Check Right Table Z Axis");
                            IsOk = false;
                        }
                        if (bX1)
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Check Right Table Probe");
                            IsOk = false;
                        }
                        break;
                    }

                case (int)EAx.OffLoaderPicker_X:
                    {
                        iAxis1 = (int)EAx.OffLoaderPicker_Z;
                        iAxis2 = (int)EAx.OnLoaderPicker_X;
                        iAxis3 = (int)EAx.OffLoaderPicker_X; //syc : Picker X 이동시 확인 구문 추가 

                        dPos1 = CData.SPos.dOFP_Z_Wait;
                        dPos2 = CData.Dev.dOnP_X_Wait;

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
                        
                        if (dCurPos3 - dPos < 0 && dCurPos2 > dPos2 + 5) //syc : Picker X 이동시 확인 구문 추가 //Onloder picker X축 '-'방향으로 이동시 Offloader X축 위치 확인 안함
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Check Onloader Picker X Axis Pos");
                            IsOk = false;
                        }
                        break;
                    }

                case (int)EAx.OffLoaderPicker_Z:
                    {
                        iAxis1 = (int)EAx.OffLoaderPicker_X;
                        iAxis2 = (int)EAx.LeftGrindZone_Y;
                        iAxis3 = (int)EAx.RightGrindZone_Y;

                        dPos1 = CData.SPos.dOFP_X_PickL;
                        dPos2 = CData.SPos.dOFP_X_PickR;
                        dPos3 = CData.SPos.dOFP_X_Place;
                        dPos4 = CData.Dev.dOffP_X_ClnStart;
                        dPos5 = CData.Dev.dOffP_X_ClnEnd;
                        dPos6 = CData.SPos.dGRD_Y_Wait[0];
                        dPos7 = CData.SPos.dGRD_Y_Wait[1];
                        dPos8 = CData.Dev.dOffP_X_ClnStart_Brush;
                        dPos9 = CData.Dev.dOffP_X_ClnEnd_Brush;

                        dCurPos1 = CMot.It.Get_FP(iAxis1);
                        dCurPos2 = CMot.It.Get_FP(iAxis2);
                        dCurPos3 = CMot.It.Get_FP(iAxis3);

                        if (CMot.It.Get_Mv(iAxis1, dPos1)) //201103 pjh : 조건 변경
                        {
                            if (!CMot.It.Get_Mv(iAxis2, dPos6)) //201103 pjh : 조건 변경
                            {
                                CMsg.Show(eMsg.Warning, "Warning", "Check Offloader Picker X Axis Pos");
                                IsOk = false;
                            }
                        }
                        else if (CMot.It.Get_Mv(iAxis1, dPos2)) //201103 pjh : 조건 변경
                        {
                            if (!CMot.It.Get_Mv(iAxis3, dPos7)) //201103 pjh : 조건 변경
                            {
                                CMsg.Show(eMsg.Warning, "Warning", "Check Offloader Picker X Axis Pos");
                                IsOk = false;
                            }
                        }
                        else if (!(CMot.It.Get_Mv(iAxis1, dPos3) || CMot.It.Get_Mv(iAxis1, dPos4) || CMot.It.Get_Mv(iAxis1, dPos5) || //201103 pjh : 조건 변경 
                                   CMot.It.Get_Mv(iAxis1, dPos8) || CMot.It.Get_Mv(iAxis1, dPos9)))  // 2022.07.28 lhs dPos8, dPos9 추가
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Check Offloader Picker X Axis Pos");
                            IsOk = false;
                        }
                        else
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Check Offloader Picker X Axis Pos");
                            IsOk = false;
                        }
                        break;
                    }

                case (int)EAx.DryZone_X:
                    {
                        iAxis1 = (int)EAx.OffLoaderPicker_Z;
                        iAxis2 = (int)EAx.DryZone_Z;
                        iAxis3 = (int)EAx.OffLoader_Y;
                        iAxis4 = (int)EAx.OffLoader_Z;
                        dPos1 = CData.SPos.dOFP_Z_Wait;
                        dPos2 = CData.SPos.dDRY_Z_Up;
                        dPos3 = CData.Dev.dOffL_Y_Wait;
                        dPos4 = CData.Dev.dOffL_Z_BRcv_Dn;
                        dPos5 = CData.Dev.dOffL_Z_BRcv_Up;
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
                        //if ((dCurPos2 != dPos2))
                        if (!CMot.It.Get_Mv(iAxis2, dPos2)) //201103 pjh : 조건 변경
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Check Dry Z Axis Pos");
                            IsOk = false;
                        }
                        //if ((dCurPos3 != dPos3))
                        if (!CMot.It.Get_Mv(iAxis3, dPos3)) //201103 pjh : 조건 변경
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

                        if (CDataOption.eDryClamp == EDryClamp.Cylinder) // 2022.07.12 lhs
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
        //190501 ksg : 추가
        private void DryStickMove(object sender, EventArgs e)
        {
            if (CSQ_Main.It.m_iStat == EStatus.Auto_Running || CSQ_Main.It.m_iStat == EStatus.Manual) return;

            Button mBtn = sender as Button;
            int Tag = int.Parse(mBtn.Tag.ToString());

            if (Tag == 0)       // Standby
            {
                CIO.It.Set_Y((int)eY.DRY_BtmTargetPos, false);
                CIO.It.Set_Y((int)eY.DRY_BtmStandbyPos, true);
            }
            else if (Tag == 1)  // Target
            {
                // 2021.04.16 lhs Start
                if (CDataOption.UseDryWtNozzle || CDataOption.eDryClamp == EDryClamp.Cylinder)  // 2022.07.05 lhs : Cylinder Clamp 추가
                {
                    if (!CIO.It.Get_X(eX.DRY_BtmStandbyPos) && !CIO.It.Get_X(eX.DRY_BtmTargetPos))
                    {
                        CMsg.Show(eMsg.Error, "Error", "Check Water Nozzle Stick Sensor");
                        return;
                    }
                    if (CMot.It.Get_FP((int)EAx.DryZone_Z) > CData.SPos.dDRY_Z_Check) // 반드시 Z축이 내려져 있어야 충돌이 안됨.
                    {
                        CMsg.Show(eMsg.Error, "Error", "Check Dry Z Axis Position");
                        return;
                    }
                    else
                    {
                        CIO.It.Set_Y((int)eY.DRY_BtmStandbyPos, false);
                        CIO.It.Set_Y((int)eY.DRY_BtmTargetPos,  true);
                    }
                } // 2021.04.16 lhs End
                else
                {
                    if (!CIO.It.Get_X(eX.DRY_BtmStandbyPos) && !CIO.It.Get_X(eX.DRY_BtmTargetPos))
                    {
                        CMsg.Show(eMsg.Error, "Error", "Check Bottom Air Blow Stick Sensor");
                        return;
                    }
                    if (!CIO.It.Get_X(eX.DRY_LevelSafety1) || !CIO.It.Get_X(eX.DRY_LevelSafety2))
                    {
                        CMsg.Show(eMsg.Error, "Error", "Check Dry Z Axis Position");
                        return;
                    }
                    else
                    {
                        CIO.It.Set_Y((int)eY.DRY_BtmStandbyPos, false);
                        CIO.It.Set_Y((int)eY.DRY_BtmTargetPos,  true);
                    }
                }                
            }
        }

        //koo : Qorvo 언어변환. --------------------------------------------------------------------------------
        //SetConvertLanguage(lblS_GrdL_PumpA.Name,CData.Opt.iSelLan,"Overload");
        private void SetWriteLanguage(string controlName, string text)
        {
            Control[] ctrls = this.Controls.Find(controlName, true);
            ctrls[0].GetType().GetProperty("Text").SetValue(ctrls[0], text, null);
        }

        //200630 jhc : 온/오프 로더 Move(Jog포함) 전 자재 감지 체크
        private bool _CheckStrip(int iAx)
        {
            bool result = false;

            if (iAx == (int)EAx.OnLoader_Y || iAx == (int)EAx.OnLoader_Z)
            {
                if (!CIO.It.Get_X(eX.INR_StripInDetect))
                {
                    CMsg.Show(eMsg.Error, "Error", "INRAIL Strip In Detect");
                    result = true;
                }
            }
            else if (iAx == (int)EAx.OffLoader_Y || iAx == (int)EAx.OffLoader_Z)
            {
                if (!CIO.It.Get_X(eX.DRY_StripOutDetect))
                {
                    CMsg.Show(eMsg.Error, "Error", "Dry Out Detect Strip");
                    result = true;
                }
            }

            return result;
        }

        private void _EnableBtn(bool bVal)
        {
            this.Enabled = bVal;
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

            // 트리거는 어떻게 내보내지? (IV2의 외부트리거)
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

            int nIdx = comboS_OffP_IV2_Program.SelectedIndex;
            CSq_OfP.It.Func_SetIVProgram(nIdx); // 0~3 : 4개 가능

            //bool bProg1 = Convert.ToBoolean(nIdx);
            // IV2 Program P000 / P001 선택
            //CIO.It.Set_Y(eY.OFFP_IV2_ProgBit0, bProg1);


        }

     
    }
}
