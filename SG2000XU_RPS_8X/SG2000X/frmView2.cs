using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;

using Brushes = System.Windows.Media.Brushes;

//20190417 josh    secsgem
using EZGemPlusCS;
using SECSGEM;

namespace SG2000X
{
    public partial class frmView2 : Form
    {
        public static int MAX_DISP_LOT = 3;        // 화면에 표시되는 최대 LOT 수량 제한

        private int m_iMgz;

        private Timer m_tmData;
        private Timer m_tmFlick;
        private Timer m_tmFlick_60;

        //20191121 ghk_display_strip
        /// <summary>
        /// 스트립 표시 점멸 플래그
        /// </summary>
        private bool m_bFlick;
        /// <summary>
        /// 점멸 간격 시간
        /// </summary>
        private DateTime m_dtFlick;
        /// <summary>
        /// ONL, OFL 라벨 표시 색상
        /// </summary>
        private Color m_cFlickCol;

        DataGridViewCellStyle m_mStExis = new DataGridViewCellStyle() { BackColor = Color.Lime };
        DataGridViewCellStyle m_mStNull = new DataGridViewCellStyle() { BackColor = Color.White };

        // LOT 정보 표시용 Label
        private Label[] m_LotLabel  = new Label[MAX_DISP_LOT];      // LOT 이름
        private Label[] m_LotInput  = new Label[MAX_DISP_LOT];      // Strip Input 수량
        private Label[] m_LotOutput = new Label[MAX_DISP_LOT];      // Strip Output 수량

        public frmView2()
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

            // 2021-05-07, jhLee : LOT 정보를 표시해주는 Label 관리
            m_LotLabel[0] = lblWorkLot1;
            m_LotLabel[1] = lblWorkLot2;
            m_LotLabel[2] = lblWorkLot3;
            
            m_LotInput[0] = lblLotIn1;          // Strip Input 수량
            m_LotInput[1] = lblLotIn2;
            m_LotInput[2] = lblLotIn3;
            
            m_LotOutput[0] = lblLotOut1;        // Strip 배출 수량
            m_LotOutput[1] = lblLotOut2;
            m_LotOutput[2] = lblLotOut3;
            

            slgSpL_Rpm.To = 5000;
            slgSpL_Rpm.Base.LabelsVisibility = Visibility.Hidden;
            slgSpL_Rpm.Base.Foreground = Brushes.White;
            slgSpL_Rpm.GaugeBackground = Brushes.White;

            slgSpR_Rpm.To = 5000;
            slgSpR_Rpm.Base.LabelsVisibility = Visibility.Hidden;
            slgSpR_Rpm.Base.Foreground = Brushes.White;
            slgSpR_Rpm.GaugeBackground = Brushes.White;

            // 화면 시작 위치 지정
            this.Location = new System.Drawing.Point(1921, 0);

            m_tmData = new Timer();
            m_tmData.Interval = 50;
            m_tmData.Tick += _M_tmData_Tick;
            m_tmData.Start();

            m_tmFlick = new Timer();
            m_tmFlick.Interval = 1000;
            m_tmFlick.Tick += M_tmFlick_Tick;
            m_tmFlick.Start();

            m_tmFlick_60 = new Timer();
            m_tmFlick_60.Interval = 60 * 1000;
            m_tmFlick_60.Tick += M_tmFlick_60_Tick;
            m_tmFlick_60.Start();

            m_iMgz = 0;

            // 선택 표시 해제
            // maeng-190104
            dgv_Onl.ClearSelection();
            dgv_Ofl.ClearSelection();
            dgv_Oft.ClearSelection(); //190407 ksg : Qc

            CData.SwIdle = new Stopwatch(); //190110 ksg : 수정
            CData.SwErr  = new Stopwatch(); //190110 ksg : 수정
            CData.SwRun  = new Stopwatch(); //190110 ksg : 수정

            // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
            //if ((CData.CurCompany != ECompany.Qorvo ) && (CData.CurCompany != ECompany.Qorvo_DZ) && (CData.CurCompany != ECompany.SkyWorks) && 
            if ((CData.CurCompany != ECompany.Qorvo)    && (CData.CurCompany != ECompany.Qorvo_DZ)  &&
                (CData.CurCompany != ECompany.Qorvo_RT) && (CData.CurCompany != ECompany.Qorvo_NC)  &&
                (CData.CurCompany != ECompany.SkyWorks) &&
                (CData.CurCompany != ECompany.ASE_K12)  && (CData.CurCompany != ECompany.ASE_KR)    &&
                (CData.CurCompany != ECompany.SST)      && (CData.CurCompany != ECompany.USI)) //191202 ksg :
            {
                lbl_DynamicSkip.Visible = false;
                lbl_DfServerSkip.Visible = false;
            }

            //190309 ksg : 
            //if (CData.CurCompany != ECompany.SkyWorks) lbl_OcrSkip.Visible = false;
            
            //190407 ksg : Qc
            if (CData.CurCompany != ECompany.SkyWorks)
            {
                lbl_OcrSkip.Visible = false;
                dgv_Oft.Visible = false;
                lbl_OftMgz.Visible = false;
            }
            
            //190227 ksg :
            lbl_Leftskip.Visible = false;
            lbl_Rightskip.Visible = false;

            //190418 ksg :
            //if(CData.CurCompany != eCompany.JSCK)
            if (CDataOption.OflRfid == eOflRFID.NotUse)
            {
                lbl_BtmRfid.Visible = false;
            }
            else
            {
                lbl_BtmRfid.Visible = true;
            }

            //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)
            if (CDataOption.RFID == ERfidType.Keyence) //201012 jhc : (KEYENCE BCR)/(RFID) 옵션으로 구분 적용 //if (CData.CurCompany == ECompany.SPIL)
            {
                if (CDataOption.OnlRfid == eOnlRFID.Use)
                {
                    lbl_OnlRfid.Text = "OnLoader MGZ Barcode";
                    lbl_OnlRfid.Visible = true;
                }
                else
                {
                    lbl_OnlRfid.Visible = false;
                }

                if (CDataOption.OflRfid == eOflRFID.Use)
                {
                    lbl_BtmRfid.Text = "OffLoader MGZ Barcode";
                    lbl_BtmRfid.Visible = true;
                }
                else
                {
                    lbl_BtmRfid.Visible = false;
                }
            }
            //

            if (CData.CurCompany != ECompany.ASE_KR && CData.CurCompany != ECompany.ASE_K12 && CData.CurCompany != ECompany.USI)
            {
                lbl_DfServerSkip.Visible = false;
                lbl_DynamicSkip.Width = 99;
            }

            //210930 syc : 2004U
            if (CData.CurCompany == ECompany.SPIL && !CDataOption.Use2004U)
            {
                pnl_InrU4.Visible = true;
                pnl_InrU3.Visible = true;
                pnl_InrU2.Visible = true;
                pnl_InrU1.Visible = true;

                pnl_OnpU1.Visible = true;
                pnl_OnpU2.Visible = true;
                pnl_OnpU3.Visible = true;
                pnl_OnpU4.Visible = true;

                pnl_OfpU1.Visible = true;
                pnl_OfpU2.Visible = true;
                pnl_OfpU3.Visible = true;
                pnl_OfpU4.Visible = true;

                pnl_DryU1.Visible = true;
                pnl_DryU2.Visible = true;
                pnl_DryU3.Visible = true;
                pnl_DryU4.Visible = true;

                pnl_GrlU1.Visible = true;
                pnl_GrlU2.Visible = true;
                pnl_GrlU3.Visible = true;
                pnl_GrlU4.Visible = true;

                pnl_GrrU1.Visible = true;
                pnl_GrrU4.Visible = true;
                pnl_GrrU3.Visible = true;
                pnl_GrrU2.Visible = true;
            }

            //210825 pjh : JCET View화면 Flow Input Display추가
            if(CData.CurCompany == ECompany.JCET)
            {
                pnl_Err.Size = new System.Drawing.Size(380, 150);
                txt_Err.Size = new System.Drawing.Size(360, 108);
            }
            else
            {
                pnl_Err.Location = new System.Drawing.Point(0, 790);
                pnl_Err.Size = new System.Drawing.Size(380, 234);
                txt_Err.Size = new System.Drawing.Size(360, 190);

                panelFlow.Visible = false;
            }
            //

            // 191106-maeng 현재 프로그램 버젼 표시
            lbl_Ver.Text = "RPS Delta Ver " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            //200414 jym : 장비 이름 표시
            lbl_Mc.Text = (CDataOption.Package == ePkg.Strip) ? "SG 2000X" : "SG 2000U";

            //211022 syc : 2004U
            if (CDataOption.Use2004U == true)
            {
                lbl_Mc.Text = "SG 2000U";
            }

            // 2021-05-03, jhLee, Multi-LOT 기능을 사용하지 않는다면 해당 TabPage를 표시하지 않도록 조치
            if (CDataOption.UseMultiLOT == false)
            {
                // 현재 진행중인 LOT 표시관련 컨트롤을 화면에 보여주지 않도록 한다.
                lblWorkLot.Visible = false;

                lblLotNo1.Visible = false;
                lblLotNo2.Visible = false;
                lblLotNo3.Visible = false;
                
                lblWorkLot1.Visible = false;
                lblWorkLot2.Visible = false;
                lblWorkLot3.Visible = false;
            }

            tmrUpdateDataSpl1.Enabled = true;
            tmrUpdateDataSpl2.Enabled = true;
        }

        private void M_tmFlick_Tick(object sender, EventArgs e)
        {
            //ksg : 라인에서 자재색은 Lime으로 통일 요구, Data랑 실 자재랑 매칭 안될때 깜박임 추가 할 것.
            /*
            if (pnlInr_Strip.BackColor == Color.FromArgb(60, 60, 60))
            { pnlInr_Strip.BackColor = Color.Lime; }
            else
            { pnlInr_Strip.BackColor = Color.FromArgb(60, 60, 60); }
            */
            lbl_LTSetter.BackColor = GV.CR_X[CIO.It.Get_TS(EWay.L)]; //190724 ksg :
            lbl_RTSetter.BackColor = GV.CR_X[CIO.It.Get_TS(EWay.R)]; //190724 ksg :

            button1.Visible = CData.Opt.bSecsUse;

            // 2021-05-07, jhLee : Mult-LOT 기능을 사용하면  화면에 각 LOT의 ID를 보여주도록 한다.
            if (CDataOption.UseMultiLOT)
            {
                UpdateLotInfoDisply();
            }
        }

        private void M_tmFlick_60_Tick(object sender, EventArgs e)
        {
            if ((CData.GemForm != null) && (CData.Opt.bSecsUse))
            {// Measure Wheel Before Data
                CData.GemForm.Measure_Wheel_Dress_Data_Set(0, 0, 0);// Left/Right ,0-Wheel(1-Dress), 0-Before Data(1-Aftere)
                // Measure Wheel After Data
                CData.GemForm.Measure_Wheel_Dress_Data_Set(0, 0, 1);// Left/Right ,0-Wheel(1-Dress), 0-Before Data(1-Aftere)
                // Measure Wheel Before Data
                CData.GemForm.Measure_Wheel_Dress_Data_Set(1, 0, 0);// Left/Right ,0-Wheel(1-Dress), 0-Before Data(1-Aftere)
                // Measure Wheel After Data
                CData.GemForm.Measure_Wheel_Dress_Data_Set(1, 0, 1);// Left/Right ,0-Wheel(1-Dress), 0-Before Data(1-Aftere)
                // Measure Dress Before Data
                CData.GemForm.Measure_Wheel_Dress_Data_Set(0, 1, 0);// Left/Right ,0-Wheel(1-Dress), 0-Before Data(1-Aftere)
                // Measure Dress After Data
                CData.GemForm.Measure_Wheel_Dress_Data_Set(0, 1, 1);// Left/Right ,0-Wheel(1-Dress), 0-Before Data(1-Aftere)
                // Measure Dress Before Data
                CData.GemForm.Measure_Wheel_Dress_Data_Set(1, 1, 0);// Left/Right ,0-Wheel(1-Dress), 0-Before Data(1-Aftere)
                // Measure Dress After Data
                CData.GemForm.Measure_Wheel_Dress_Data_Set(1, 1, 1);// Left/Right ,0-Wheel(1-Dress), 0-Before Data(1-Aftere)
            }
        }


        public void Release()
        {
            if (m_tmData != null)
            {
                if (m_tmData.Enabled)
                { m_tmData.Stop(); }
                m_tmData.Dispose();
                m_tmData = null;
            }

            if (m_tmFlick != null)
            {
                if (m_tmFlick.Enabled)
                { m_tmFlick.Stop(); }
                m_tmFlick.Dispose();
                m_tmFlick = null;
            }
        }

        private void _M_tmData_Tick(object sender, EventArgs e)
        {
            lbl_Now.Text = DateTime.Now.ToString();

            // 200102-maeng
            button1.Visible = CData.Opt.bSecsUse;
            btn_PauseOn.Visible = CData.Opt.bSecsUse;

            TimeSpan tSpan = DateTime.Now.Subtract(CData.ProgS);
            lblTi_Prog.Text = tSpan.ToString(@"hh\:mm\:ss");

            if (CSQ_Main.It.m_iStat == EStatus.Idle || CSQ_Main.It.m_iStat == EStatus.Stop)
            {
                //190110 ksg : 수정
                CData.SwIdle.Start();
                CData.SwErr.Stop();
                CData.SwRun.Stop();
                lblTi_Idle.Text = CData.SwIdle.Elapsed.ToString(@"hh\:mm\:ss");
            }
            else if (CSQ_Main.It.m_iStat == EStatus.Error)
            {
                //190110 ksg : 수정
                CData.SwIdle.Stop();
                CData.SwErr.Start();
                CData.SwRun.Stop();
                lblTi_Jam.Text = CData.SwErr.Elapsed.ToString(@"hh\:mm\:ss");
            }
            else if (CSQ_Main.It.m_iStat == EStatus.Auto_Running)
            {
                //190110 ksg : 수정
                CData.SwIdle.Stop();
                CData.SwErr.Stop();
                CData.SwRun.Start();
                lblTi_Run.Text = CData.SwRun.Elapsed.ToString(@"hh\:mm\:ss");
            }

            // 2021-05-10, jhLee : Multi-LOT 기능 이용시 각 LOT별로 Strip의 색을 달리표시해준다.
            if (CData.IsMultiLOT())
            {
                if (CData.Parts[(int)EPart.INR].bExistStrip)    pnlInr_Strip.BackColor = CData.Parts[(int)EPart.INR].LotColor;
                if (CData.Parts[(int)EPart.ONP].bExistStrip)    pnlOnp_Strip.BackColor = CData.Parts[(int)EPart.ONP].LotColor;
                if (CData.Parts[(int)EPart.GRDL].bExistStrip)   pnlGrl_Strip.BackColor = CData.Parts[(int)EPart.GRDL].LotColor;
                if (CData.Parts[(int)EPart.GRDR].bExistStrip)   pnlGrr_Strip.BackColor = CData.Parts[(int)EPart.GRDR].LotColor;
                if (CData.Parts[(int)EPart.OFP].bExistStrip)    pnlOfp_Strip.BackColor = CData.Parts[(int)EPart.OFP].LotColor;
                if (CData.Parts[(int)EPart.DRY].bExistStrip)    pnlDry_Strip.BackColor = CData.Parts[(int)EPart.DRY].LotColor;
            }

            //20191121 ghk_display_strip
            if (CDataOption.DisplayStrip == eDisplayStrip.NotUse)
            {//점멸 표시 기능 사용 안할 때
                pnlInr_Strip.Visible = CData.Parts[(int)EPart.INR].bExistStrip;
                pnlOnp_Strip.Visible = CData.Parts[(int)EPart.ONP].bExistStrip;
                pnlGrl_Strip.Visible = CData.Parts[(int)EPart.GRDL].bExistStrip;
                pnlGrr_Strip.Visible = CData.Parts[(int)EPart.GRDR].bExistStrip;
                pnlOfp_Strip.Visible = CData.Parts[(int)EPart.OFP].bExistStrip;
                pnlDry_Strip.Visible = CData.Parts[(int)EPart.DRY].bExistStrip;
            }
            else
            {//점멸 표시 기능 사용 할때
                if (CData.IsMultiLOT())
                {
                    m_cFlickCol = m_bFlick ? Color.Lime : Color.Gainsboro;

                    if (DateTime.Now > m_dtFlick)
                    {
                        m_bFlick = !m_bFlick;
                        m_dtFlick = DateTime.Now.Add(new TimeSpan(0, 0, 0, 0, 500));
                    }

                    //ONL
                    if (CData.Parts[(int)EPart.ONL].bNotMatch)
                    {//점멸
                        lbl_OnlMgz.BackColor = m_bFlick ? CData.Parts[(int)EPart.ONL].LotColor : Color.Gainsboro; // m_cFlickCol;
                    }
                    else
                    {//정상 표시
                        if (CData.Parts[(int)EPart.ONL].bExistStrip)
                        { lbl_OnlMgz.BackColor = CData.Parts[(int)EPart.ONL].LotColor; } // Color.Lime; }
                        else
                        { lbl_OnlMgz.BackColor = Color.Gainsboro; }
                    }

                    //INR
                    if (CData.Parts[(int)EPart.INR].bNotMatch)
                    {//점멸
                        pnlInr_Strip.Visible = true;
                        pnlInr_Strip.BackColor = m_bFlick ? CData.Parts[(int)EPart.INR].LotColor : Color.Gainsboro; // m_cFlickCol;
                    }
                    else
                    {//정상 표시
                        pnlInr_Strip.BackColor = CData.Parts[(int)EPart.INR].LotColor;  // Color.Lime;
                        pnlInr_Strip.Visible = CData.Parts[(int)EPart.INR].bExistStrip;
                    }

                    //ONP
                    if (CData.Parts[(int)EPart.ONP].bNotMatch)
                    {//점멸
                        pnlOnp_Strip.Visible = true;
                        pnlOnp_Strip.BackColor = m_bFlick ? CData.Parts[(int)EPart.ONP].LotColor : Color.Gainsboro; // m_cFlickCol;
                    }
                    else
                    {//정상 표시
                        pnlOnp_Strip.BackColor = CData.Parts[(int)EPart.ONP].LotColor;  // Color.Lime;
                        pnlOnp_Strip.Visible = CData.Parts[(int)EPart.ONP].bExistStrip;
                    }

                    //GRDL
                    if (!CData.Opt.aTblSkip[(int)EWay.L])
                    {//스킵이 아닐 경우
                        if (CData.Parts[(int)EPart.GRDL].bNotMatch)
                        {//점멸
                            pnlGrl_Strip.Visible = true;
                            pnlGrl_Strip.BackColor = m_bFlick ? CData.Parts[(int)EPart.GRDL].LotColor : Color.Gainsboro; // m_cFlickCol;
                        }
                        else
                        {//정상 표시
                            pnlGrl_Strip.BackColor = CData.Parts[(int)EPart.GRDL].LotColor;  // Color.Lime;
                            pnlGrl_Strip.Visible = CData.Parts[(int)EPart.GRDL].bExistStrip;
                        }
                    }
                    else
                    {//스킵 일 경우 판넬 안보이게
                        pnlGrl_Strip.Visible = false;
                    }

                    //GRDR
                    if (!CData.Opt.aTblSkip[(int)EWay.R])
                    {//스킵이 아닐 경우
                        if (CData.Parts[(int)EPart.GRDR].bNotMatch)
                        {//점멸
                            pnlGrr_Strip.Visible = true;
                            pnlGrr_Strip.BackColor = m_bFlick ? CData.Parts[(int)EPart.GRDR].LotColor : Color.Gainsboro; // m_cFlickCol;
                        }
                        else
                        {//정상 표시
                            pnlGrr_Strip.BackColor = CData.Parts[(int)EPart.GRDR].LotColor;  // Color.Lime;
                            pnlGrr_Strip.Visible = CData.Parts[(int)EPart.GRDR].bExistStrip;
                        }
                    }
                    else
                    {//스킵 일 경우 판넬 안보이게
                        pnlGrr_Strip.Visible = false;
                    }

                    //OFP
                    if (CData.Parts[(int)EPart.OFP].bNotMatch)
                    {//점멸
                        pnlOfp_Strip.Visible = true;
                        pnlOfp_Strip.BackColor = m_bFlick ? CData.Parts[(int)EPart.OFP].LotColor : Color.Gainsboro; // m_cFlickCol;
                    }
                    else
                    {//정상 표시
                        pnlOfp_Strip.BackColor = CData.Parts[(int)EPart.OFP].LotColor;  // Color.Lime;
                        pnlOfp_Strip.Visible = CData.Parts[(int)EPart.OFP].bExistStrip;
                    }

                    //DRY
                    if (CData.Parts[(int)EPart.DRY].bNotMatch)
                    {//점멸
                        pnlDry_Strip.Visible = true;
                        pnlDry_Strip.BackColor = m_bFlick ? CData.Parts[(int)EPart.DRY].LotColor : Color.Gainsboro; // m_cFlickCol;
                    }
                    else
                    {//정상 표시
                        pnlDry_Strip.BackColor = CData.Parts[(int)EPart.DRY].LotColor;  // Color.Lime;
                        pnlDry_Strip.Visible = CData.Parts[(int)EPart.DRY].bExistStrip;
                    }

                    //OFL_TOP
                    if (CData.Parts[(int)EPart.OFR].bNotMatch)
                    {//점멸
                        lbl_OftMgz.BackColor = m_bFlick ? CData.Parts[(int)EPart.OFR].LotColor : Color.Gainsboro; // m_cFlickCol;
                    }
                    else
                    {//정상 표시
                        if (CData.Parts[(int)EPart.OFR].bExistStrip)
                        { lbl_OftMgz.BackColor = CData.Parts[(int)EPart.OFR].LotColor; }  // Color.Lime; }
                        else
                        { lbl_OftMgz.BackColor = Color.Gainsboro; }
                    }

                    //OFL_BTM
                    if (CData.Parts[(int)EPart.OFL].bNotMatch)
                    {//점멸
                        lbl_OflMgz.BackColor = m_bFlick ? CData.Parts[(int)EPart.OFL].LotColor : Color.Gainsboro; // m_cFlickCol;
                    }
                    else
                    {//정상 표시
                        if (CData.Parts[(int)EPart.OFL].bExistStrip)
                        { lbl_OflMgz.BackColor = CData.Parts[(int)EPart.OFR].LotColor; }  // Color.Lime; }
                        else
                        { lbl_OflMgz.BackColor = Color.Gainsboro; }
                    }
                }
                else // Not use Multi-LOT
                {
                    m_cFlickCol = m_bFlick ? Color.Lime : Color.Gainsboro;

                    if (DateTime.Now > m_dtFlick)
                    {
                        m_bFlick = !m_bFlick;
                        m_dtFlick = DateTime.Now.Add(new TimeSpan(0, 0, 0, 0, 500));
                    }

                    //ONL
                    if (CData.Parts[(int)EPart.ONL].bNotMatch)
                    {//점멸
                        lbl_OnlMgz.BackColor = m_cFlickCol;
                    }
                    else
                    {//정상 표시
                        if (CData.Parts[(int)EPart.ONL].bExistStrip)
                        { lbl_OnlMgz.BackColor = Color.Lime; }
                        else
                        { lbl_OnlMgz.BackColor = Color.Gainsboro; }
                    }

                    //INR
                    if (CData.Parts[(int)EPart.INR].bNotMatch)
                    {//점멸
                        pnlInr_Strip.Visible = true;
                        pnlInr_Strip.BackColor = m_cFlickCol;
                    }
                    else
                    {//정상 표시
                        pnlInr_Strip.BackColor = Color.Lime;
                        pnlInr_Strip.Visible = CData.Parts[(int)EPart.INR].bExistStrip;
                    }

                    //ONP
                    if (CData.Parts[(int)EPart.ONP].bNotMatch)
                    {//점멸
                        pnlOnp_Strip.Visible = true;
                        pnlOnp_Strip.BackColor = m_cFlickCol;
                    }
                    else
                    {//정상 표시
                        pnlOnp_Strip.BackColor = Color.Lime;
                        pnlOnp_Strip.Visible = CData.Parts[(int)EPart.ONP].bExistStrip;
                    }

                    //GRDL
                    if (!CData.Opt.aTblSkip[(int)EWay.L])
                    {//스킵이 아닐 경우
                        if (CData.Parts[(int)EPart.GRDL].bNotMatch)
                        {//점멸
                            pnlGrl_Strip.Visible = true;
                            pnlGrl_Strip.BackColor = m_cFlickCol;
                        }
                        else
                        {//정상 표시
                            pnlGrl_Strip.BackColor = Color.Lime;
                            pnlGrl_Strip.Visible = CData.Parts[(int)EPart.GRDL].bExistStrip;
                        }
                    }
                    else
                    {//스킵 일 경우 판넬 안보이게
                        pnlGrl_Strip.Visible = false;
                    }

                    //GRDR
                    if (!CData.Opt.aTblSkip[(int)EWay.R])
                    {//스킵이 아닐 경우
                        if (CData.Parts[(int)EPart.GRDR].bNotMatch)
                        {//점멸
                            pnlGrr_Strip.Visible = true;
                            pnlGrr_Strip.BackColor = m_cFlickCol;
                        }
                        else
                        {//정상 표시
                            pnlGrr_Strip.BackColor = Color.Lime;
                            pnlGrr_Strip.Visible = CData.Parts[(int)EPart.GRDR].bExistStrip;
                        }
                    }
                    else
                    {//스킵 일 경우 판넬 안보이게
                        pnlGrr_Strip.Visible = false;
                    }

                    //OFP
                    if (CData.Parts[(int)EPart.OFP].bNotMatch)
                    {//점멸
                        pnlOfp_Strip.Visible = true;
                        pnlOfp_Strip.BackColor = m_cFlickCol;
                    }
                    else
                    {//정상 표시
                        pnlOfp_Strip.BackColor = Color.Lime;
                        pnlOfp_Strip.Visible = CData.Parts[(int)EPart.OFP].bExistStrip;
                    }

                    //DRY
                    if (CData.Parts[(int)EPart.DRY].bNotMatch)
                    {//점멸
                        pnlDry_Strip.Visible = true;
                        pnlDry_Strip.BackColor = m_cFlickCol;
                    }
                    else
                    {//정상 표시
                        pnlDry_Strip.BackColor = Color.Lime;
                        pnlDry_Strip.Visible = CData.Parts[(int)EPart.DRY].bExistStrip;
                    }

                    //OFL_TOP
                    if (CData.Parts[(int)EPart.OFR].bNotMatch)
                    {//점멸
                        lbl_OftMgz.BackColor = m_cFlickCol;
                    }
                    else
                    {//정상 표시
                        if (CData.Parts[(int)EPart.OFR].bExistStrip)
                        { lbl_OftMgz.BackColor = Color.Lime; }
                        else
                        { lbl_OftMgz.BackColor = Color.Gainsboro; }
                    }

                    //OFL_BTM
                    if (CData.Parts[(int)EPart.OFL].bNotMatch)
                    {//점멸
                        lbl_OflMgz.BackColor = m_cFlickCol;
                    }
                    else
                    {//정상 표시
                        if (CData.Parts[(int)EPart.OFL].bExistStrip)
                        { lbl_OflMgz.BackColor = Color.Lime; }
                        else
                        { lbl_OflMgz.BackColor = Color.Gainsboro; }
                    }
                }
            }

            // 200715 jym : 추가
            if (CDataOption.Package == ePkg.Unit)
            {
                if (CData.Dev.iUnitCnt == 4)
                {
                    for (int iU = 0; iU < CData.Dev.iUnitCnt; iU++)
                    {
                        pnlInr_Strip.Controls["pnl_InrU" + (iU + 1)].BackColor = CData.Parts[(int)EPart.INR].aUnitEx[iU] ? Color.Black : Color.White;
                        pnlOnp_Strip.Controls["pnl_OnpU" + (iU + 1)].BackColor = CData.Parts[(int)EPart.ONP].aUnitEx[iU] ? Color.Black : Color.White;
                        pnlGrl_Strip.Controls["pnl_GrlU" + (iU + 1)].BackColor = CData.Parts[(int)EPart.GRDL].aUnitEx[iU] ? Color.Black : Color.White;
                        pnlGrr_Strip.Controls["pnl_GrrU" + (iU + 1)].BackColor = CData.Parts[(int)EPart.GRDR].aUnitEx[iU] ? Color.Black : Color.White;
                        pnlOfp_Strip.Controls["pnl_OfpU" + (iU + 1)].BackColor = CData.Parts[(int)EPart.OFP].aUnitEx[iU] ? Color.Black : Color.White;
                        pnlDry_Strip.Controls["pnl_DryU" + (iU + 1)].BackColor = CData.Parts[(int)EPart.DRY].aUnitEx[iU] ? Color.Black : Color.White;
                    }
                }
                else
                {
                    pnlInr_Strip.Controls["pnl_InrU1"].BackColor = CData.Parts[(int)EPart.INR].aUnitEx[0] ? Color.Black : Color.White;
                    pnlOnp_Strip.Controls["pnl_OnpU1"].BackColor = CData.Parts[(int)EPart.ONP].aUnitEx[0] ? Color.Black : Color.White;
                    pnlGrl_Strip.Controls["pnl_GrlU1"].BackColor = CData.Parts[(int)EPart.GRDL].aUnitEx[0] ? Color.Black : Color.White;
                    pnlGrr_Strip.Controls["pnl_GrrU1"].BackColor = CData.Parts[(int)EPart.GRDR].aUnitEx[0] ? Color.Black : Color.White;
                    pnlOfp_Strip.Controls["pnl_OfpU1"].BackColor = CData.Parts[(int)EPart.OFP].aUnitEx[0] ? Color.Black : Color.White;
                    pnlDry_Strip.Controls["pnl_DryU1"].BackColor = CData.Parts[(int)EPart.DRY].aUnitEx[0] ? Color.Black : Color.White;
                    pnlInr_Strip.Controls["pnl_InrU2"].Visible = false;
                    pnlOnp_Strip.Controls["pnl_OnpU2"].Visible = false;
                    pnlGrl_Strip.Controls["pnl_GrlU2"].Visible = false;
                    pnlGrr_Strip.Controls["pnl_GrrU2"].Visible = false;
                    pnlOfp_Strip.Controls["pnl_OfpU2"].Visible = false;
                    pnlDry_Strip.Controls["pnl_DryU2"].Visible = false;
                    pnlInr_Strip.Controls["pnl_InrU3"].BackColor = CData.Parts[(int)EPart.INR].aUnitEx[1] ? Color.Black : Color.White;
                    pnlOnp_Strip.Controls["pnl_OnpU3"].BackColor = CData.Parts[(int)EPart.ONP].aUnitEx[1] ? Color.Black : Color.White;
                    pnlGrl_Strip.Controls["pnl_GrlU3"].BackColor = CData.Parts[(int)EPart.GRDL].aUnitEx[1] ? Color.Black : Color.White;
                    pnlGrr_Strip.Controls["pnl_GrrU3"].BackColor = CData.Parts[(int)EPart.GRDR].aUnitEx[1] ? Color.Black : Color.White;
                    pnlOfp_Strip.Controls["pnl_OfpU3"].BackColor = CData.Parts[(int)EPart.OFP].aUnitEx[1] ? Color.Black : Color.White;
                    pnlDry_Strip.Controls["pnl_DryU3"].BackColor = CData.Parts[(int)EPart.DRY].aUnitEx[1] ? Color.Black : Color.White;
                    pnlInr_Strip.Controls["pnl_InrU4"].Visible = false;
                    pnlOnp_Strip.Controls["pnl_OnpU4"].Visible = false;
                    pnlGrl_Strip.Controls["pnl_GrlU4"].Visible = false;
                    pnlGrr_Strip.Controls["pnl_GrrU4"].Visible = false;
                    pnlOfp_Strip.Controls["pnl_OfpU4"].Visible = false;
                    pnlDry_Strip.Controls["pnl_DryU4"].Visible = false;
                }
            }

            lbl_OnlMgz.Text = string.Format("{0}/{1}", CData.Parts[(int)EPart.ONL].iMGZ_No, CData.LotInfo.iTotalMgz);
            lbl_OflMgz.Text = string.Format("{0}/{1}", CData.Parts[(int)EPart.OFL].iMGZ_No, CData.LotInfo.iTotalMgz);
            lbl_OftMgz.Text = string.Format("{0}/{1}", CData.Parts[(int)EPart.OFR].iMGZ_No, CData.LotInfo.iTotalMgz); //190407 ksg : Qc

            lbl_InCnt.Text  = CData.LotInfo.iTInCnt.ToString();
            lbl_OutCnt.Text = CData.LotInfo.iTOutCnt.ToString();

            //if(m_iMgz != CData.Dev.iMgzCnt)
            if (CData.Dev.iMgzCnt != 0 && m_iMgz != CData.Dev.iMgzCnt) //190320 ksg :
            {
                int iHei = 0;
                m_iMgz = CData.Dev.iMgzCnt;
                dgv_Onl.Rows.Clear();
                dgv_Ofl.Rows.Clear();
                dgv_Oft.Rows.Clear(); //190407 ksg : Qc
                iHei = (dgv_Onl.Height - 5) / m_iMgz;
                dgv_Onl.RowTemplate.Height = iHei;
                dgv_Onl.RowTemplate.Resizable = DataGridViewTriState.False;
                dgv_Ofl.RowTemplate.Height = iHei;
                dgv_Ofl.RowTemplate.Resizable = DataGridViewTriState.False;
                //190407 ksg : Qc
                dgv_Oft.RowTemplate.Height = iHei;
                dgv_Oft.RowTemplate.Resizable = DataGridViewTriState.False;

                for (int i = 0; i < m_iMgz; i++)
                {
                    dgv_Onl.Rows.Add();
                    dgv_Ofl.Rows.Add();
                    dgv_Oft.Rows.Add(); //190407 ksg : Qc
                }
            }

            if (CData.Parts[(int)EPart.ONL].iSlot_info != null && CData.Parts[(int)EPart.ONL].iSlot_info.Length == m_iMgz)
            {
                for (int i = 0; i < m_iMgz; i++)
                {
                    if (CData.Parts[(int)EPart.ONL].iSlot_info[i] == (int)eSlotInfo.Empty)
                    { dgv_Onl.Rows[m_iMgz - 1 - i].Cells[0].Style = m_mStNull; }
                    else
                    { dgv_Onl.Rows[m_iMgz - 1 - i].Cells[0].Style = m_mStExis; }

                    if (CData.Parts[(int)EPart.OFL].iSlot_info[i] == (int)eSlotInfo.Empty)
                    { dgv_Ofl.Rows[m_iMgz - 1 - i].Cells[0].Style = m_mStNull; }
                    else
                    { dgv_Ofl.Rows[m_iMgz - 1 - i].Cells[0].Style = m_mStExis; }

                    //190407 ksg : Qc
                    if (CData.Parts[(int)EPart.OFR].iSlot_info[i] == (int)eSlotInfo.Empty)
                    { dgv_Oft.Rows[m_iMgz - 1 - i].Cells[0].Style = m_mStNull; }
                    else
                    { dgv_Oft.Rows[m_iMgz - 1 - i].Cells[0].Style = m_mStExis; }
                }
            }

            //190211 ksg :
            string text;
            if (CData.Dev.bBcrSkip)
            {
                lbl_BcrSkip.BackColor = Color.LightGray;
                text = CLang.It.GetLanguage("Barcode") + "\r\n" + CLang.It.GetLanguage("Skip");
                SetWriteLanguage(lbl_BcrSkip.Name, text);
            }
            else
            {
                lbl_BcrSkip.BackColor = Color.Lime;
                text = CLang.It.GetLanguage("Barcode") + "\r\n" + CLang.It.GetLanguage("Use");
                SetWriteLanguage(lbl_BcrSkip.Name, text);
            }

            if (CData.Dev.bOriSkip)
            {
                lbl_OriSkip.BackColor = Color.LightGray;
                text = CLang.It.GetLanguage("Orientation") + "\r\n" + CLang.It.GetLanguage("Skip");
                SetWriteLanguage(lbl_OriSkip.Name, text);
            }
            else
            {
                lbl_OriSkip.BackColor = Color.Lime;
                text = CLang.It.GetLanguage("Orientation") + "\r\n" + CLang.It.GetLanguage("Use");
                SetWriteLanguage(lbl_OriSkip.Name, text);
            }

            //20190218 ghk_dynamicfunction
            if (CData.Dev.bDynamicSkip)
            {
                lbl_DynamicSkip.BackColor = Color.LightGray;
                text = CLang.It.GetLanguage("Dynamic") + "\r\n" + CLang.It.GetLanguage("Skip");
                SetWriteLanguage(lbl_DynamicSkip.Name, text);
            }
            else
            {
                lbl_DynamicSkip.BackColor = Color.Lime;
                lbl_DynamicSkip.Text = "DF\r\nUse";
                text = CLang.It.GetLanguage("Dynamic") + "\r\n" + CLang.It.GetLanguage("Use");
                SetWriteLanguage(lbl_DynamicSkip.Name, text);
            }

            //20190624 ghk_dfserver
            if (CData.Dev.bDfServerSkip)
            {
                lbl_DfServerSkip.BackColor = Color.LightGray;
                lbl_DfServerSkip.Text = "Server\r\nSkip";
            }
            else
            {
                lbl_DfServerSkip.BackColor = Color.Lime;
                lbl_DfServerSkip.Text = "Server\r\nUse";
            }
            //

            //910309 ksg :
            if (CData.Dev.bOcrSkip)
            {
                lbl_OcrSkip.BackColor = Color.LightGray;
                text = CLang.It.GetLanguage("OCR") + "\r\n" + CLang.It.GetLanguage("Skip");
                SetWriteLanguage(lbl_OcrSkip.Name, text);
            }
            else
            {
                lbl_OcrSkip.BackColor = Color.Lime;
                text = CLang.It.GetLanguage("OCR") + "\r\n" + CLang.It.GetLanguage("Use");
                SetWriteLanguage(lbl_OcrSkip.Name, text);
            }

            //210825 pjh : JCET는 SECS/GEM 사용 시에도 Water IO 보게끔 수정
            if (CData.CurCompany == ECompany.JCET)
            {
                if (!CDataOption.IsWhlCleaner)
                {
                    lbl_L_WCW.ForeColor = Color.Gray;
                    lbl_R_WCW.ForeColor = Color.Gray;
                }
                if (CIO.It.Get_Y(eY.GRDL_WhlCleaner)) { lbl_L_WCW.BackColor = Color.Lime; }
                else { lbl_L_WCW.BackColor = Color.LightGray; }
                if (CIO.It.Get_Y(eY.GRDR_WhlCleaner)) { lbl_R_WCW.BackColor = Color.Lime; }
                else { lbl_R_WCW.BackColor = Color.LightGray; }

                if (CIO.It.Get_X(eX.GRDL_SplBtmWater)) { lbl_L_SBW.BackColor = Color.Lime; }
                else { lbl_L_SBW.BackColor = Color.LightGray; }
                if (CIO.It.Get_X(eX.GRDR_SplBtmWater)) { lbl_R_SBW.BackColor = Color.Lime; }
                else { lbl_R_SBW.BackColor = Color.LightGray; }

                if (CIO.It.Get_X(eX.GRDL_SplWater)) { lbl_L_SW.BackColor = Color.Lime; }
                else { lbl_L_SW.BackColor = Color.LightGray; }
                if (CIO.It.Get_X(eX.GRDR_SplWater)) { lbl_R_SW.BackColor = Color.Lime; }
                else { lbl_R_SW.BackColor = Color.LightGray; }
            }
            //

            // 190711-maeng Group명, Dresser 이름 추가
            // Device file 정보 표시
            lbl_Grp.Text = CDev.It.m_sGrp;
            lbl_Dev.Text = CData.Dev.sName;
            // Wheel file 정보 표시
            lbl_WhlL.Text = CData.Whls[0].sWhlName;
            lbl_WhlR.Text = CData.Whls[1].sWhlName;
            // Dresser 정보 표시
            //211122 pjh : Dresser 별도 관리 기능 사용 시 Dresser Name 불러오는 위치 변경
            if (CDataOption.UseSeperateDresser)
            {
                lbl_DrsL.Text = CData.Drs[0].sDrsName;
                lbl_DrsR.Text = CData.Drs[1].sDrsName;
            }
            else
            {
                lbl_DrsL.Text = CData.Whls[0].sDrsName;
                lbl_DrsR.Text = CData.Whls[1].sDrsName;
            }
            // Wheel left thickness
            lblL_WhBf.Text  = (CData.Whls[0].dWhlBf + 0.0005).ToString("0.000");
            lblL_WhAf.Text  = (CData.Whls[0].dWhlAf + 0.0005).ToString("0.000");
            lblL_WhLs.Text  = (CData.GetWLoss(0) + 0.0005).ToString("0.000");
            lblL_Wh1st.Text = (CData.WhlMea[0, 0] + 0.0005).ToString("0.000");
            lblL_Wh2nd.Text = (CData.WhlMea[0, 1] + 0.0005).ToString("0.000");
            lblL_Wh3rd.Text = (CData.WhlMea[0, 2] + 0.0005).ToString("0.000");
            // Wheel right thickness
            lblR_WhBf.Text  = (CData.Whls[1].dWhlBf + 0.0005).ToString("0.000");
            lblR_WhAf.Text  = (CData.Whls[1].dWhlAf + 0.0005).ToString("0.000");
            lblR_WhLs.Text  = (CData.GetWLoss(1) + 0.0005).ToString("0.000");
            lblR_Wh1st.Text = (CData.WhlMea[1, 0] + 0.0005).ToString("0.000");
            lblR_Wh2nd.Text = (CData.WhlMea[1, 1] + 0.0005).ToString("0.000");
            lblR_Wh3rd.Text = (CData.WhlMea[1, 2] + 0.0005).ToString("0.000");
            // Dresser left thickness
            lblL_DrBf.Text  = (CData.Whls[0].dDrsBf + 0.0005).ToString("0.000");
            lblL_DrAf.Text  = (CData.Whls[0].dDrsAf + 0.0005).ToString("0.000");
            lblL_DrLs.Text  = (CData.GetDLoss(0) + 0.0005).ToString("0.000");
            // Dresser right thickness                                
            lblR_DrBf.Text  = (CData.Whls[1].dDrsBf + 0.0005).ToString("0.000");
            lblR_DrAf.Text  = (CData.Whls[1].dDrsAf + 0.0005).ToString("0.000");
            lblR_DrLs.Text  = (CData.GetDLoss(1) + 0.0005).ToString("0.000");

            if (!CData.VIRTUAL && CData.WMX)
            {
                //slgSpL_Rpm.Value = CSpl.It.Get_Frpm(EWay.L);
                // 2023.03.15 Max
                slgSpL_Rpm.Value = CSpl_485.It.GetFrpm(EWay.L);

                lblGrL_Load.Text    = CData.Spls[0].dLoad.ToString();
                //slgSpR_Rpm.Value    = CSpl.It.Get_Frpm(EWay.R);
                // 2023.03.15 Max
                slgSpR_Rpm.Value = CSpl_485.It.GetFrpm(EWay.R);

                lblGrR_Load.Text    = CData.Spls[1].dLoad.ToString();
                lblGrL_TblVel.Text  = CMot.It.Get_Vel((int)EAx.LeftGrindZone_Y).ToString("0.0");
                lblGrL_Zpos.Text    = CMot.It.Get_FP((int)EAx.LeftGrindZone_Z).ToString("F3");
                lblGrR_TblVel.Text  = CMot.It.Get_Vel((int)EAx.RightGrindZone_Y).ToString("0.0");
                lblGrR_Zpos.Text    = CMot.It.Get_FP((int)EAx.RightGrindZone_Z).ToString("F3");

                if (CData.GrData[0].bSplLoadFlag)
                {
                    if (CData.Spls[0].dLoad > CData.GrData[0].dSplMaxLoad)  { CData.GrData[0].dSplMaxLoad = CData.Spls[0].dLoad; }
                }

                if (CData.GrData[1].bSplLoadFlag)
                {
                    if (CData.Spls[1].dLoad > CData.GrData[1].dSplMaxLoad)  { CData.GrData[1].dSplMaxLoad = CData.Spls[1].dLoad; }
                }

                lblGrL_LMax.Text = CData.GrData[0].dSplMaxLoad.ToString();
                lblGrR_LMax.Text = CData.GrData[1].dSplMaxLoad.ToString();

                //201225 jhc : 고급 Grind Condition 설정/체크 기능 (Spindle Current, Table Vacuum Low Limit 설정)
                if (CData.GrData[0].bCheckGrindCondition || CData.GrData[0].bCheckDressCondition)
                {
                    //Grind 중 Table Vacuum 최소값 업데이트
                    if (CData.GrData[0].bCheckGrindCondition && (CData.GrData[0].dTableVacuumMin < CData.Parts[(int)EPart.GRDL].dChuck_Vacuum))
                    {
                        CData.GrData[0].dTableVacuumMin = CData.Parts[(int)EPart.GRDL].dChuck_Vacuum; //최저값 이력용 : Vacuum은 (-)값이므로 큰 값이 위험
                    }

                    //Grind/Dress 중 Spindle 전류 최대값 업데이트
                    if (CData.Spls[0].nCurrent_Amp > CData.GrData[0].nSpindleCurrentMax) { CData.GrData[0].nSpindleCurrentMax = CData.Spls[0].nCurrent_Amp; }
                    
                    //Grind/Dress 중 Spindle 전류 최소값 업데이트
                    if (CData.Spls[0].nCurrent_Amp < CData.GrData[0].nSpindleCurrentMin) { CData.GrData[0].nSpindleCurrentMin = CData.Spls[0].nCurrent_Amp; }
                }

                if (CData.GrData[1].bCheckGrindCondition || CData.GrData[1].bCheckDressCondition)
                {
                    //Grind 중 Table Vacuum 최소값 업데이트
                    if (CData.GrData[1].bCheckGrindCondition && (CData.GrData[1].dTableVacuumMin < CData.Parts[(int)EPart.GRDR].dChuck_Vacuum))
                    {
                        CData.GrData[1].dTableVacuumMin = CData.Parts[(int)EPart.GRDR].dChuck_Vacuum; //최저값 이력용 : Vacuum은 (-)값이므로 큰 값이 위험
                    }
                    
                    //Grind/Dress 중 Spindle 전류 최대값 업데이트
                    if (CData.Spls[1].nCurrent_Amp > CData.GrData[1].nSpindleCurrentMax) { CData.GrData[1].nSpindleCurrentMax = CData.Spls[1].nCurrent_Amp; }
                    
                    //Grind/Dress 중 Spindle 전류 최소값 업데이트
                    if (CData.Spls[1].nCurrent_Amp < CData.GrData[1].nSpindleCurrentMin) { CData.GrData[1].nSpindleCurrentMin = CData.Spls[1].nCurrent_Amp; }
                } 
                //..
            }

            //190227 ksg :
            if (CData.Opt.aTblSkip[(int)EWay.L])
            {
                lbl_Leftskip.Visible = true;
                lbl_Leftskip.Text = "S\n\rK\n\rI\n\rP";
            }
            else
            {
                lbl_Leftskip.Visible = false;
            }

            if (CData.Opt.aTblSkip[(int)EWay.R])
            {
                lbl_Rightskip.Visible = true;
                lbl_Rightskip.Text = "S\n\rK\n\rI\n\rP";
            }
            else
            {
                lbl_Rightskip.Visible = false;
            }

            if (CSQ_Main.It.m_iStat == EStatus.Error)
            {
                if (CData.ErrList[CData.iErrNo].sNo != null)
                {
                    txt_Err.Text = CData.ErrList[CData.iErrNo].sNo.ToString() + "\t" + CData.ErrList[CData.iErrNo].sName.ToString();
                }
            }
            else
            {
                txt_Err.Text = "";
            }

            //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)
            if (CDataOption.RFID == ERfidType.Keyence) //201012 jhc : (KEYENCE BCR)/(RFID) 옵션으로 구분 적용 //if (CData.CurCompany == ECompany.SPIL)
            {
                if (CDataOption.OnlRfid == eOnlRFID.Use)
                { lbl_OnlRfid.Text = string.Format("OnL MGZ Barcode :  {0}", CData.Parts[(int)EPart.ONL].sMGZ_ID); }
                
                if (CDataOption.OflRfid == eOflRFID.Use)
                { lbl_BtmRfid.Text = string.Format("OffL MGZ Barcode :  {0}", CData.Parts[(int)EPart.OFL].sMGZ_ID); }
            }
            //

            //190418 ksg :
            if ((CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK || CData.CurCompany == ECompany.JCET) //200121 ksg : , 200625 lks
                || CDataOption.UseMultiLOT)     // 2021-05-03, jhLee : Mult-LOT 기능을 사용하면  화면에 각 Strip의 ID를 보여주도록 한다.
            {
                // 190719-maeng : 190718 사내 Pre-buyoff에서 나온 요청사항 'RFID : '로 표시 요청
                lbl_BtmRfid.Text = string.Format("RFID : {0}", CData.Parts[(int)EPart.OFL].sMGZ_ID);

                //lbl_BcrInr.Text = CData.Parts[(int)EPart.INR].sBcr;
                //lbl_BcrOnp.Text = CData.Parts[(int)EPart.ONP].sBcr;
                //lbl_BcrGrl.Text = CData.Parts[(int)EPart.GRDL].sBcr;
                //lbl_BcrGrr.Text = CData.Parts[(int)EPart.GRDR].sBcr;
                //lbl_BcrOfp.Text = CData.Parts[(int)EPart.OFP].sBcr;
                //lbl_BcrDry.Text = CData.Parts[(int)EPart.DRY].sBcr;
            }

            lbl_LDrsId.Text     = CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Dress_Name[0];
            lbl_RDrsId.Text     = CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Dress_Name[1];
            lbl_LWhlId.Text     = CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[0];
            lbl_RWhlId.Text     = CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[1];
            lbl_LWhl_Num.Text   = CData.LotInfo.sLTool_Serial_Num;
            lbl_RWhl_Num.Text   = CData.LotInfo.sRTool_Serial_Num;

            lblComm_Status.Text      = GEM.strA_Comm[CData.GemForm.m_iCommState];
            lblComm_Status.BackColor = GEM.clrA_WRC [CData.GemForm.m_iCommState];

            lblCont_Status.Text      = GEM.strA_Cont[CData.GemForm.m_iCtrlState  ];
            lblCont_Status.BackColor = GEM.clrA_RYC [CData.GemForm.m_iCtrlState  ];

            lblConn_Status.Text      = GEM.strA_Conn[CData.GemForm.m_iConnState];
            lblConn_Status.BackColor = GEM.clrA_RC  [CData.GemForm.m_iConnState];

            lblPS_Status  .Text      = GEM.strA_PS  [CData.GemForm.m_iPS  ];
            lblPS_Status  .BackColor = GEM.clrA_YGCR[CData.GemForm.m_iPS  ];

            label58       .Text      = CData.LotInfo.sGMgzId;

            lbl_LStatus.Text = CLang.It.GetSeq(CData.L_GRD.SEQ);
            lbl_RStatus.Text = CLang.It.GetSeq(CData.R_GRD.SEQ);

            //200409 jym : 그라인딩 시간 화면 표시
            lbl_GrdTimeL.Text = CData.GrdElp[(int)EWay.L].tsEls.ToString(@"hh\:mm\:ss");
            lbl_GrdTimeR.Text = CData.GrdElp[(int)EWay.R].tsEls.ToString(@"hh\:mm\:ss");

            //200406 jym : 드레싱 시간 화면 표시
            lbl_DrsTimeL.Text = CData.DrsElp[(int)EWay.L].tsEls.ToString(@"hh\:mm\:ss");
            lbl_DrsTimeR.Text = CData.DrsElp[(int)EWay.R].tsEls.ToString(@"hh\:mm\:ss");

            // 2020.10.21 JSKim St
            if (CData.Dev.bMeasureMode)
            {
                lbl_LMeasuring.Location = lbl_LStatus.Location;
                lbl_RMeasuring.Location = lbl_RStatus.Location;

                lbl_LMeasuring.Visible = CData.Parts[(int)EPart.GRDL].bExistStrip;
                lbl_RMeasuring.Visible = CData.Parts[(int)EPart.GRDR].bExistStrip;
            }
            else
            {
                lbl_LMeasuring.Visible = false;
                lbl_RMeasuring.Visible = false;
            }
            // 2020.10.21 JSKim Ed
        }

        private void pib_CI_MouseDown(object sender, MouseEventArgs e)
        {
            pnl_Info.Visible = true;
        }

        private void pib_CI_MouseLeave(object sender, EventArgs e)
        {
            pnl_Info.Visible = false;
        }

        private void cms_Mgz_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (CSQ_Main.It.m_iStat == EStatus.Auto_Running) { return; }

            // 2020-10-21, jhLee : Skyworks VOC, 작업자 레벨에 따라 Magazine의 Strip 존재여부 편집 기능을 사용 권한을 지정한다.
            if ((int)CData.Lev < CData.Opt.iLvOpStripExistEdit)
            {
                CMsg.Show(eMsg.Warning, "Warning", "Unable to perform due to low privilege level.");
                return;
            }

            DataGridView mDgv = cms_Mgz.SourceControl as DataGridView;
            int iWy = int.Parse(mDgv.Tag.ToString());

            foreach (DataGridViewCell mCell in mDgv.SelectedCells)
            {
                if (e.ClickedItem.Text == "Empty")
                {
                    if (iWy == 0)
                    { CData.Parts[(int)EPart.ONL].iSlot_info[(m_iMgz-1) - mCell.RowIndex] = (int)eSlotInfo.Empty; }
                    else if(iWy == 1)
                    { CData.Parts[(int)EPart.OFL].iSlot_info[(m_iMgz-1) - mCell.RowIndex] = (int)eSlotInfo.Empty; }
                    else
                    { CData.Parts[(int)EPart.OFR].iSlot_info[(m_iMgz-1) - mCell.RowIndex] = (int)eSlotInfo.Empty; }
                }
                else if (e.ClickedItem.Text == "Exist")
                {
                    if (iWy == 0)
                    { CData.Parts[(int)EPart.ONL].iSlot_info[(m_iMgz-1) - mCell.RowIndex] = (int)eSlotInfo.Exist; }
                    else if(iWy == 1)
                    { CData.Parts[(int)EPart.OFL].iSlot_info[(m_iMgz-1) - mCell.RowIndex] = (int)eSlotInfo.Exist; }
                    else
                    { CData.Parts[(int)EPart.OFR].iSlot_info[(m_iMgz-1) - mCell.RowIndex] = (int)eSlotInfo.Exist; }
                }
            }

            mDgv.ClearSelection();
        }

        private void pnl_Strip_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (CSQ_Main.It.m_iStat == EStatus.Auto_Running) { return; }

            string sLotName = "";
            Panel mPnl = sender as Panel;
            int iTag = int.Parse(mPnl.Tag.ToString());
            string sPart = "";
            int iPart = 0;

            switch(iTag)
            {
                case 0:
                    sPart = "Inrail";
                    iPart = (int)EPart.INR;
                    break;

                case 1:
                    sPart = "On Loader Picker";
                    iPart = (int)EPart.ONP;
                    break;

                case 2:
                    sPart = "Grind Left Table";
                    iPart = (int)EPart.GRDL;
                    break;

                case 3:
                    sPart = "Grind Right Table";
                    iPart = (int)EPart.GRDR;
                    break;

                case 4:
                    sPart = "Off Loader Picker";
                    iPart = (int)EPart.OFP;
                    break;

                case 5:
                    sPart = "Dry Zone";
                    iPart = (int)EPart.DRY;
                    break;
            }

            // 2020.10.28 JSKim St
            System.Drawing.Point ptCenter = new System.Drawing.Point();

            ptCenter.X = Convert.ToInt32((this.Location.X + (this.Location.X + this.Size.Width)) / 2);
            ptCenter.Y = Convert.ToInt32((this.Location.Y + (this.Location.Y + this.Size.Height)) / 2);
            // 2020.10.28 JSKim Ed             

            //210729 pjh : Strip Status 제거 Level 설정 옵션 추가
            if((int)CData.Lev < CData.Opt.iLvOpStripExistEdit)
            {
                CMsg.Show(eMsg.Warning, "Warning", "Upper select Level");
                return;
            }
            //

            // 2020.10.28 JSKim St
            //if (CMsg.Show(eMsg.Warning, "Warning", sPart + " Strip Remove?") == DialogResult.OK)
            if (CMsg.Show(eMsg.Warning, "Warning", sPart + " Strip Remove?", ptCenter, true) == DialogResult.OK)
            // 2020.10.28 JSKim Ed
            {
                sLotName = CData.Parts[iPart].sLotName;         // 삭제하려하는 Strip의 LOT Name

                CData.Parts[iPart].bExistStrip  = false;
                CData.Parts[iPart].iStripStat   = 0;

                switch (iPart)
                {
                    case 1: 
                        {
                            //Inrail에서 자재 사라짐
                            CData.Parts[iPart].iStat = ESeq.INR_Ready;

                            if(CData.Parts[(int)EPart.ONP].iStat == ESeq.ONP_Pick) CData.Parts[(int)EPart.ONP].iStat = ESeq.ONP_Wait;

                            // 2022.05.18 SungTae Start : [추가] Multi-LOT 관련
                            // In-rail과 On-Picker에서 Strip 제거 시 제거한 Strip을 대부분 재투입 하기 때문에 투입한 Strip 수량을 다시 빼자.
                            if (CData.IsMultiLOT())
                            {
                                int nIdx = CData.LotMgr.GetListIndexNum(sLotName);
                            
                                if(nIdx >= 0)   CData.LotMgr.ListLOTInfo[nIdx].iTInCnt--;
                            }

                            CData.LotInfo.iTInCnt--;
                            // 2022.05.18 SungTae End

                            //190618 ksg :
                            if (CDataOption.CurEqu == eEquType.Pikcer)
                            {
                                //210929 syc : 2004U IV2 조건 추가
                                bool bcheck = false;
                                if (CDataOption.Use2004U)
                                {
                                    bcheck = (CData.Parts[(int)EPart.ONP].iStat == ESeq.ONP_CheckBcr) ||
                                             (CData.Parts[(int)EPart.ONP].iStat == ESeq.ONP_CheckOri) ||
                                             (CData.Parts[(int)EPart.ONP].iStat == ESeq.ONP_CheckBcrOri) ||
                                             (CData.Parts[(int)EPart.ONP].iStat == ESeq.ONP_IV2);
                                }
                                else
                                {
                                    bcheck = (CData.Parts[(int)EPart.ONP].iStat == ESeq.ONP_CheckBcr) ||
                                             (CData.Parts[(int)EPart.ONP].iStat == ESeq.ONP_CheckOri) ||
                                             (CData.Parts[(int)EPart.ONP].iStat == ESeq.ONP_CheckBcrOri);
                                }

                                if (bcheck)
                                {
                                    CData.Parts[(int)EPart.ONP].iStat = CSq_OnP.It.m_iPreState;
                                }
                                //syc end

                            }
                            //CData.GemForm.OnCarrierUnloaded(CData.LotInfo.sLotName, CData.Parts[(int)EPart.INR].sBcr);
                            break;    
                        }

                    case 2:
                        {
                            //ONP에서 자재 사라짐
                            CData.Parts[iPart].iStat = ESeq.ONP_Wait;
                            //CData.GemForm.OnCarrierUnloaded(CData.LotInfo.sLotName, CData.Parts[(int)EPart.ONP].sBcr);

                            // 2022.05.18 SungTae Start : [추가] Multi-LOT 관련
                            // In-rail과 On-Picker에서 Strip 제거 시 제거한 Strip을 대부분 재투입 하기 때문에 투입한 Strip 수량을 다시 빼자.
                            if (CData.IsMultiLOT())
                            {
                                int nIdx = CData.LotMgr.GetListIndexNum(sLotName);

                                if (nIdx >= 0) CData.LotMgr.ListLOTInfo[nIdx].iTInCnt--;
                            }

                            CData.LotInfo.iTInCnt--;
                            // 2022.05.18 SungTae End

                            break;  
                        }

                    case 3:
                        {
                            //Left Table에서 자재 사라짐
                            CData.Parts[iPart].iStat = ESeq.GRL_Wait;

                            if(CData.Dev.bDual == eDual.Dual)
                            {
                                 if(CData.Parts[(int)EPart.ONP].iStat == ESeq.ONP_PickTbL) 
                                 {
                                    CData.Parts[(int)EPart.ONP].iStat = ESeq.ONP_Wait;
                                 }
#if true //20200415 jhc : Picker Idle Time 개선
                                if (CData.Parts[(int)EPart.OFP].iStat == ESeq.OFP_PickTbL)
                                {
                                    CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_Wait;
                                }
#endif
                            }

                            //190416 ksg :
                            if (CData.Dev.bDual == eDual.Normal)
                            {
                                if (CData.Parts[(int)EPart.OFP].iStat == ESeq.OFP_PickTbL)
                                {
                                    CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_Wait;
                                }
                            }

                            CData.Parts[(int)EPart.GRDL].bChkGrd = false; //200624 pjh : Grinding 중 Error Check 변수
                            //CData.GemForm.OnCarrierUnloaded(CData.LotInfo.sLotName, CData.Parts[(int)EPart.GRDL].sBcr);
                            break;
                        }

                    case 4:
                        {
                            //Right Table에서 자재 사라짐
                            CData.Parts[iPart].iStat = ESeq.GRR_Wait;

                            if (CData.Parts[(int)EPart.OFP].iStat == ESeq.OFP_PickTbR)
                            {
                                CData.Parts[(int)EPart.OFP].iStat = ESeq.OFP_Wait;
                            }

                            CData.Parts[(int)EPart.GRDR].bChkGrd = false; //200624 pjh : Grinding 중 Error Check 변수
                            //CData.GemForm.OnCarrierUnloaded(CData.LotInfo.sLotName, CData.Parts[(int)EPart.GRDR].sBcr);
                            break;  
                        }

                    case 5:
                        {
                            //OffLoad Picker에서 자재 사라짐
                            CData.Parts[iPart].iStat = ESeq.OFP_Wait;
                            //CData.GemForm.OnCarrierUnloaded(CData.LotInfo.sLotName, CData.Parts[(int)EPart.OFP].sBcr);
                            break;
                        }

                    case 6:
                        {
                            CData.Parts[iPart].iStat = ESeq.DRY_Wait;
						
						    // 2021-01-??, YYY for AbortTransfer command send to QC
                            if (CData.Opt.bQcUse)
                            {
                                if (CMsg.Show(eMsg.Warning, "Warning", sPart + " Strip is Moved to QC vision?", ptCenter, true) == DialogResult.OK)
                                {
                                    CGxSocket.It.SendMessage("EQSendEnd");
                                }
                                else
                                {
                                    CGxSocket.It.SendMessage("EQAbortTransfer");
                                }
                            }
						    //CData.GemForm.OnCarrierUnloaded(CData.LotInfo.sLotName, CData.Parts[(int)EPart.DRY].sBcr);
                            break;
                        }
                }

                // 2021-06-08, jhLee, Multi-LOT, 자재 정보를 삭제한 경우 LOT을 종료할 지 여부를 판가름 한다.
                if (CData.IsMultiLOT())
                {
                    CData.LotMgr.RemoveStrip(sLotName);        // 지정 LOT의 자재를 삭제하였다.
                }
            }
        }

        /// <summary>
        /// 매거진 자재 관리 datagridview 좌클릭 이벤트 처리
        /// maeng-190104
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgv_MouseUp(object sender, MouseEventArgs e)
        {
            DataGridView mDgv = sender as DataGridView;

            int iWy = int.Parse(mDgv.Tag.ToString());

            if (e.Button == MouseButtons.Left)
            {
                //190116 ksg : 팝업 창 위치 수정 
                //cms_Mgz.Show(mDgv, mDgv.PointToScreen(e.Location));
                if(iWy == 0) cms_Mgz.Show(mDgv, e.X + 50                     , e.Y);
                else         cms_Mgz.Show(mDgv, e.X - (mDgv.Size.Width + 100), e.Y);
                //cms_Mgz.Show(mDgv, e.X, e.Y);
            }
        }

        //koo : Qorvo 언어변환. --------------------------------------------------------------------------------
        //SetConvertLanguage(lblS_GrdL_PumpA.Name,CData.Opt.iSelLan,"Overload");
        private void SetWriteLanguage(string controlName,string text)
        {
            Control[] ctrls = this.Controls.Find(controlName, true);
            ctrls[0].GetType().GetProperty("Text").SetValue(ctrls[0], text, null);
        }

        private void pib_CI_Click(object sender, EventArgs e)
        {//20190417 josh    secsgem
            if(!CData.Opt.bSecsUse) return; // 191125 ksg :

            if (!GEM.bExistForm)
            {
                CData.GemForm.StartPosition = FormStartPosition.Manual;
                CData.GemForm.Location = new System.Drawing.Point(0, 0);
                CData.GemForm.ShowDialog();
            }
            else
            {
                CData.GemForm.Show();
            }
        }

        private void btn_LDrsId_Click(object sender, EventArgs e)
        {

            Button mBtn = sender as Button;
            BeginInvoke(new Action(() => mBtn.Enabled = false));
            uint nMatLoc = 1;            

            using (frmTxt mForm = new frmTxt("Input Left Dresser ID"))
            {
                // 2021.06.07 lhs Start : 입력 폼에 Text 입력
                if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                {
                    mForm.SetValText(CData.Whls[0].sDrsName);
                }
                // 2021.06.07 lhs End

                if (mForm.ShowDialog() == DialogResult.OK)
                {
                    if (mForm.Val != "")
                    {
                        //20191023 josh
                        //jsck secsgem
                        //ceid 999601 Material Verify Request
                        CData.GemForm.OnMatVerifyRequest(mForm.Val, nMatLoc);
                    }
                    else
                    {
                        CMsg.Show(eMsg.Warning, "Warning", "Not Input Dresser Id");
                    }
                }
            }

            BeginInvoke(new Action(() => mBtn.Enabled = true));
        }

        private void btn_RDrsId_Click(object sender, EventArgs e)
        {
            Button mBtn = sender as Button;
            BeginInvoke(new Action(() => mBtn.Enabled = false));
            uint nMatLoc = 2;

            using (frmTxt mForm = new frmTxt("Input Right Dresser ID"))
            {
                // 2021.06.07 lhs Start : 입력 폼에 Text 입력
                if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                {
                    mForm.SetValText(CData.Whls[1].sDrsName);
                }
                // 2021.06.07 lhs End

                if (mForm.ShowDialog() == DialogResult.OK)
                {
                    if (mForm.Val != "")
                    {
                        //20191023 josh
                        //jsck secsgem
                        //ceid 999601 Material Verify Request
                        CData.GemForm.OnMatVerifyRequest(mForm.Val, nMatLoc);
                    }
                    else
                    {
                        CMsg.Show(eMsg.Warning, "Warning", "Not Input Dresser Id");
                    }
                }
            }

            BeginInvoke(new Action(() => mBtn.Enabled = true));
        }

        private void btn_LWhlId_Click(object sender, EventArgs e)
        {
            Button mBtn = sender as Button;
            BeginInvoke(new Action(() => mBtn.Enabled = false));

            uint nMatLoc = 1;
            using (frmTxt mForm = new frmTxt("Input Left Wheel ID"))
            {
                // 2021.06.07 lhs Start : 입력 폼에 Text 입력
                if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                {
                    mForm.SetValText(CData.Whls[0].sWhlName);
                }
                // 2021.06.07 lhs End

                if (mForm.ShowDialog() == DialogResult.OK)
                {
                    if (mForm.Val != "")
                    {
                        //20191023 josh
                        //jsck secsgem
                        //ceid 999701 Tool Verify Request                        
                        CData.GemForm.OnToolVerifyRequest(mForm.Val, nMatLoc);
                        CData.GemForm.ToolSerial_Number_Set(nMatLoc);
                    }
                    else
                    {
                        CMsg.Show(eMsg.Warning, "Warning", "Not Input Dresser Id");
                    }
                }
            }

            BeginInvoke(new Action(() => mBtn.Enabled = true));
        }

        private void btn_RWhlId_Click(object sender, EventArgs e)
        {
            Button mBtn = sender as Button;
            BeginInvoke(new Action(() => mBtn.Enabled = false));

            uint nMatLoc = 2;

            using (frmTxt mForm = new frmTxt("Input Right Wheel ID"))
            {
                // 2021.06.07 lhs Start : 입력 폼에 Text 입력
                if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                {
                    mForm.SetValText(CData.Whls[1].sWhlName);
                }
                // 2021.06.07 lhs End

                if (mForm.ShowDialog() == DialogResult.OK)
                {
                    if (mForm.Val != "")
                    {
                        //20191023 josh
                        //jsck secsgem
                        //ceid 999701 Tool Verify Request                        
                        CData.GemForm.OnToolVerifyRequest(mForm.Val, nMatLoc);
                        CData.GemForm.ToolSerial_Number_Set(nMatLoc);
                    }
                    else
                    {
                        CMsg.Show(eMsg.Warning, "Warning", "Not Input Wheel Id");
                    }
                }
            }

            BeginInvoke(new Action(() => mBtn.Enabled = true));
        }
        private void btn_PauseOn_Click(object sender, EventArgs e)
        {
            CData.GemForm.frmGEM_ShowWindow();
            /*
            if (CData.SecsUse == eSecsGem.NotUse) return;            
            CData.GemForm.StartPosition = FormStartPosition.Manual;
            CData.GemForm.Location = new System.Drawing.Point(0, 0);
            CData.GemForm.Show();
            */
            //CData.GemForm.ShowDialog();
        }

        private void btn_TerminalMsg_Call(object sender, EventArgs e)
        {
            //            CData.GemForm.frmGEM_Display();

            // 2021.10.13 SungTae Start : [수정] Multi-LOT 관련
            //if (CData.IsMultiLOT())
            //{
            //    if (CData.GemForm.sgfrmMsg2.Visible == true)
            //    {
            //        //                Console.WriteLine("---- c0");
            //        CData.GemForm.sgfrmMsg2.Display_Hide();
            //    }
            //    else
            //    {
            //        //                Console.WriteLine("---- c1");
            //        CData.GemForm.sgfrmMsg2.Display();
            //        //sgfrmMsg.ShowGrid(m_pHostLOT);
            //    }
            //}
            //else
            {
                if (CData.GemForm.sgfrmMsg.Visible == true)
                {
                    //                Console.WriteLine("---- c0");
                    CData.GemForm.sgfrmMsg.Display_Hide();
                }
                else
                {
                    //                Console.WriteLine("---- c1");
                    CData.GemForm.sgfrmMsg.Display();
                    //sgfrmMsg.ShowGrid(m_pHostLOT);
                }
            }
            // 2021.10.13 SungTae End
        }

        private void btn_Clr_Event(object sender, EventArgs e)
        {
            Button mBtn = sender as Button;
            uint uData;
            if ("LDress_Clr" == mBtn.Tag.ToString())
            {
                uData = 1;
                lbl_LDrsId.Text = CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Dress_Name[0] = "";
                CData.GemForm.OnMatVerifyRequest("", uData);
            }
            else if ("RDress_Clr" == mBtn.Tag.ToString())
            {
                uData = 2;
                lbl_RDrsId.Text = CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Dress_Name[1] = "";
                CData.GemForm.OnMatVerifyRequest("", uData);
            }
            else if ("LWheel_Clr" == mBtn.Tag.ToString())
            {
                uData = 1;
                lbl_LWhlId.Text = CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[0]
                                = CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Serial_Num[0]  = "";

                CData.GemForm.ToolSerial_Number_Set(lbl_LWhlId.Text, uData);
                CData.GemForm.OnToolVerifyRequest(lbl_LWhlId.Text, uData);
            }
            else if ("RWheel_Clr" == mBtn.Tag.ToString())
            {
                uData = 2;
                lbl_RWhlId.Text = CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Name[1] 
                                = CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Serial_Num[1] = "";
                
                CData.GemForm.ToolSerial_Number_Set(lbl_RWhlId.Text, uData);
                CData.GemForm.OnToolVerifyRequest(lbl_RWhlId.Text, uData);

            }
        }
        private void btn_REdit_Event(object sender, EventArgs e)
        {
            Button mBtn = sender as Button;
            BeginInvoke(new Action(() => mBtn.Enabled = false));

            using (frmTxt mForm = new frmTxt("Input Right Wheel Serial Num"))
            {
                if (mForm.ShowDialog() == DialogResult.OK)
                {
                    if (mForm.Val != "")
                    {
                        CData.GemForm.ToolSerial_Number_Set(mForm.Val, 2);                                                
                    }
                    else
                    {
                        CMsg.Show(eMsg.Warning, "Warning", "Not Input Wheel Serial Num");
                    }
                }
            }
            BeginInvoke(new Action(() => mBtn.Enabled = true));
        }

        private void btn_LEdit_Event(object sender, EventArgs e)
        {
            Button mBtn = sender as Button;
            BeginInvoke(new Action(() => mBtn.Enabled = false));

            using (frmTxt mForm = new frmTxt("Input Left Wheel Serial Num"))
            {
                if (mForm.ShowDialog() == DialogResult.OK)
                {
                    if (mForm.Val != "")
                    {
                        CData.GemForm.ToolSerial_Number_Set(mForm.Val, 1);
                    }
                    else
                    {
                        CMsg.Show(eMsg.Warning, "Warning", "Not Input Wheel Serial Num");
                    }
                }
            }

            BeginInvoke(new Action(() => mBtn.Enabled = true));
        }

        //// == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == ==
        //// 2021-05-03, jhLee : Multi-LOT 운영을 위한 LOT 정보 관리
        //
        // LOT 정보를 보여주는 Grid의 내용을 갱신하여준다.
        public void UpdateLotInfoDisply()
        {
            TLotInfo rInfo; //  = new TLotInfo();

            int i;
            // string sLotID;

            //// LOT 대기 Grid에 등록된 데이터 수 만큼 표시 
            //for (i = 0; i < CData.LotMgr.GetCount(); i++)
            //{
            //    rInfo = CData.LotMgr.GetLotInfo(i);            // 지정 순번의 LOT 정보 조회
            //    if (rInfo == null) continue;

            //    // LOT의 상태에 따라 색과 상태 정보를 달리 표시해준다.
            //    if (rInfo.nState == ELotState.eWait)       // 해당 LOT이  대기중이라면
            //    {
            //        gridLotList.Rows[i].Cells[2].Value = "W";
            //        gridLotList.Rows[i].Cells[1].Style.BackColor = Color.White;
            //    }
            //    else
            //    {
            //        if (rInfo.nState == ELotState.eRun)
            //        {
            //            gridLotList.Rows[i].Cells[2].Value = "R";       // 진행중
            //        }
            //        else if (rInfo.nState == ELotState.eClose)
            //        {
            //            gridLotList.Rows[i].Cells[2].Value = "C";       // 투입완료
            //        }
            //        else if (rInfo.nState == ELotState.eComplete)
            //        {
            //            gridLotList.Rows[i].Cells[2].Value = "E";       // 작업종료
            //        }

            //        // 지정된 색으로 표시해준다.
            //        gridLotList.Rows[i].Cells[1].Style.BackColor = GetLotColor(rInfo.nType);
            //    }


            //    // Strip In/Out 수량 표시
            //    gridLotList.Rows[i].Cells[3].Value = $"{rInfo.iTInCnt}";
            //    gridLotList.Rows[i].Cells[4].Value = $"{rInfo.iTOutCnt}";
            //}
            
            // LOT 표시 Label의 내용도 변경시켜준다.
            for (i = 0; i < MAX_DISP_LOT; i++)       // 화면에 LOT 대기 상황 표시
            {
                if (i < CData.LotMgr.GetCount())
                {
                    rInfo = CData.LotMgr.GetLotInfo(i);            // 지정 순번의 LOT 정보 조회

                    if (rInfo == null) continue;

                    // 각 LOT의 정보 표시
                    m_LotLabel[i].Text  = rInfo.sLotName;
                    m_LotInput[i].Text  = rInfo.iTInCnt.ToString();
                    m_LotOutput[i].Text = rInfo.iTOutCnt.ToString();

                    //if (rInfo.nState == ELotState.eWait)       // 해당 LOT이  대기중이라면
                    //    m_LotLabel[i].BackColor = Color.White;
                    //else
                        m_LotLabel[i].BackColor = /*rInfo.LotColor;*/GetLotColor(rInfo.nType);
                }
                else
                {
                    m_LotLabel[i].Text      = "";
                    m_LotLabel[i].BackColor = Color.Silver;
                    m_LotInput[i].Text      = "";
                    m_LotOutput[i].Text     = "";
                }
            }

        }//of UpdateLotInfoDisply

        // LOT별 색상 지정
        public Color GetLotColor(int nType)
        {
            // 지정된 색으로 표시해준다.
            // 현재는 3가지 종류
            if      (nType == 0)    { return Color.Lime; }
            else if (nType == 1)    { return Color.Aqua; }
            else if (nType == 2)    { return Color.Violet; }

            return Color.White;
        }

        private void tmrUpdateDataSpl1_Tick(object sender, EventArgs e)
        {
            tmrUpdateDataSpl1.Enabled = false;

            label67.Text = "Seq Step = " + Convert.ToString(CSpl_485.It.Spl1_SeqStep);

            label68.Text = "오류코드            " + Convert.ToString(CSpl_485.It.Spl1_Data.Add2100h); // 오류코드: Pr.06-17에서 Pr.06-22를 따름
            //label69.Text = "주파수 명령         " + Convert.ToString(CSpl_485.It.Spl1_Data.Add2102h); // 주파수 명령(F)
            //label71.Text = "출력 주파수         " + Convert.ToString(CSpl_485.It.Spl1_Data.Add2103h); // 출력 주파수(H)
            label72.Text = "Enc Pluse값         " + Convert.ToString(CSpl_485.It.Spl1_Data.Add2116h); // 다기능 디스플레이(Pr.00-04) : Enc Pluse값 "G xxxx PLS"
            //label73.Text = "Bit  신호           " + Convert.ToString(CSpl_485.It.Spl1_Data.Add2119h); // Bit  신호
            label74.Text = "출력 전류           " + Convert.ToString(CSpl_485.It.Spl1_Data.Add2200h); // 출력 전류을 표시(A)
            //label75.Text = "실제 출력 주파수    " + Convert.ToString(CSpl_485.It.Spl1_Data.Add2202h); // 실제 출력 주파수를 표시(H)
            label76.Text = "엔코더 피드백(RPM)  " + Convert.ToString(CSpl_485.It.Spl1_Data.Add2207h); // 드라이브 또는 엔코더 피드백으로 모터속도를 rpm으로 표시(r00: 정회전 속도, -00:역회전 속도)
            label77.Text = "출력 토크           " + Convert.ToString(CSpl_485.It.Spl1_Data.Add2208h); // 드라이브에  의해 측정된 정/역 출력 토크 N-m을 나타냄 (t0.0:정토크, -0.0:역토크)
            //label78.Text = "PG 피드백           " + Convert.ToString(CSpl_485.It.Spl1_Data.Add2209h); // PG 피드백을 표시(NOTE 1처럼)
            //label79.Text = "IGBT온도            " + Convert.ToString(CSpl_485.It.Spl1_Data.Add220Eh); // 드라이브 전력 모듈의 IGBT온도를 'C로 나타냄(c.)
            //label80.Text = "캐패시턴스의 온도   " + Convert.ToString(CSpl_485.It.Spl1_Data.Add220Fh); // 캐패시턴스의 온도를 'C로 나타냄(i.)
            //label81.Text = "실제 모터의 회전 수 " + Convert.ToString(CSpl_485.It.Spl1_Data.Add2215h); // 실제 모터의 회전 수(PG카드의 PG1)(P.) 실제 운전 방향이 바뀌거나 키패드가 멈춤에서 0을 나타내며 9에서 시작함. 최대는 65535(P.)
            //label82.Text = "펄스 입력 위치      " + Convert.ToString(CSpl_485.It.Spl1_Data.Add2217h); // 펄스 입력 위치(PG카드의 PG2)(4.)
            //label83.Text = "Pr.00-05의 출력 값  " + Convert.ToString(CSpl_485.It.Spl1_Data.Add221Fh); // Pr.00-05의 출력 값
                                                                                                  
            label84.Text = CSpl_485.It.m_Spl1_ReciveData;

            if (CSpl_485.It.GetErrorState_Spl2) label85.Text = CSpl_485.It.GetErrorMsg_Spl1();

            tmrUpdateDataSpl1.Enabled = true;
        }

        private void tmrUpdateDataSpl2_Tick(object sender, EventArgs e)
        {
            tmrUpdateDataSpl2.Enabled = false;

            label103.Text = "Seq Step = " + Convert.ToString(CSpl_485.It.Spl2_SeqStep);

            label102.Text = "오류코드            " + Convert.ToString(CSpl_485.It.Spl2_Data.Add2100h); // 오류코드: Pr.06-17에서 Pr.06-22를 따름
            //label101.Text = "주파수 명령         " + Convert.ToString(CSpl_485.It.Spl2_Data.Add2102h); // 주파수 명령(F)
            //label100.Text = "출력 주파수         " + Convert.ToString(CSpl_485.It.Spl2_Data.Add2103h); // 출력 주파수(H)
            label99.Text = "Enc Pluse값         " + Convert.ToString(CSpl_485.It.Spl2_Data.Add2116h); // 다기능 디스플레이(Pr.00-04) : Enc Pluse값 "G xxxx PLS"
            //label98.Text = "Bit  신호           " + Convert.ToString(CSpl_485.It.Spl2_Data.Add2119h); // Bit  신호
            label97.Text = "출력 전류           " + Convert.ToString(CSpl_485.It.Spl2_Data.Add2200h); // 출력 전류을 표시(A)
            //label96.Text = "실제 출력 주파수    " + Convert.ToString(CSpl_485.It.Spl2_Data.Add2202h); // 실제 출력 주파수를 표시(H)
            label95.Text = "엔코더 피드백(RPM)  " + Convert.ToString(CSpl_485.It.Spl2_Data.Add2207h); // 드라이브 또는 엔코더 피드백으로 모터속도를 rpm으로 표시(r00: 정회전 속도, -00:역회전 속도)
            label94.Text = "출력 토크           " + Convert.ToString(CSpl_485.It.Spl2_Data.Add2208h); // 드라이브에  의해 측정된 정/역 출력 토크 N-m을 나타냄 (t0.0:정토크, -0.0:역토크)
            //label93.Text = "PG 피드백           " + Convert.ToString(CSpl_485.It.Spl2_Data.Add2209h); // PG 피드백을 표시(NOTE 1처럼)
            //label92.Text = "IGBT온도            " + Convert.ToString(CSpl_485.It.Spl2_Data.Add220Eh); // 드라이브 전력 모듈의 IGBT온도를 'C로 나타냄(c.)
            //label91.Text = "캐패시턴스의 온도   " + Convert.ToString(CSpl_485.It.Spl2_Data.Add220Fh); // 캐패시턴스의 온도를 'C로 나타냄(i.)
            //label90.Text = "실제 모터의 회전 수 " + Convert.ToString(CSpl_485.It.Spl2_Data.Add2215h); // 실제 모터의 회전 수(PG카드의 PG1)(P.) 실제 운전 방향이 바뀌거나 키패드가 멈춤에서 0을 나타내며 9에서 시작함. 최대는 65535(P.)
            //label89.Text = "펄스 입력 위치      " + Convert.ToString(CSpl_485.It.Spl2_Data.Add2217h); // 펄스 입력 위치(PG카드의 PG2)(4.)
            //label88.Text = "Pr.00-05의 출력 값  " + Convert.ToString(CSpl_485.It.Spl2_Data.Add221Fh); // Pr.00-05의 출력 값      
                       

            label87.Text = CSpl_485.It.m_Spl2_ReciveData;

            if (CSpl_485.It.GetErrorState_Spl2) label86.Text = CSpl_485.It.GetErrorMsg_Spl2();

            tmrUpdateDataSpl2.Enabled = true;
        }

        // 색 변경
        // gridDInput.Rows[nRow].Cells[2].Style.BackColor = Color.White;
        // 내용 변경
        // gridDInput.Rows[i].Cells[2].Value = sMsg;
    }

    // DataGridView의 DoubleBuffered 속성을 지정할 수 있게 작용한다.
    public static class ExtensionMethods2
    {
        public static void DoubleBuffered(this DataGridView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }
    }
    //// == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == ==
}
