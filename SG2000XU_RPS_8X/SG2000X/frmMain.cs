using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace SG2000X
{
    public partial class frmMain : Form
    {
        private const uint WMX_RETRY = 5;

        private frmView m_fmView_Ori;
        private frmView2 m_fmView;
        private frmMst m_fmMst;
        // 200323 mjy : 에러화면 유저컨트롤로 변환
        private vwMain_Err m_vwErr = new vwMain_Err();
        // 200323 mjy : Wheel jig 경고창 유저컨트롤로 변환
        private vwMain_Scn m_vwZig = new vwMain_Scn();
        private vwMain_Scn m_vwDrs = new vwMain_Scn();
        // 200803 jym : Wheel 선택 창 출력
        private vwMain_Whl m_vwSelWhl = new vwMain_Whl();
        private vwMain_Msg m_vwSelMsg = new vwMain_Msg();
        // 201023 jym : 추가
        private vwMain_Key m_vwSelKey = new vwMain_Key();

        // 210601 jym : 네트워크 스레드 추가
        private Thread m_thNet;
        private Thread m_thCyl;
        private Thread m_thLog;
        private Thread m_thQc;
        private Thread m_thSpl;
        private System.Windows.Forms.Timer m_tmUpdt;
        private System.Windows.Forms.Timer m_tmView;


        //211202 pjh : DF Net Drive Connect Check
        /// <summary>
        /// Net Drive 연결 상태 Check Thread
        /// </summary>
        private Thread m_thNetCheck;
        //

        /// <summary>
        /// 220328 pjh : ASE KH Card Read Check 후 Read 되면 Level Change
        /// </summary>
        private Thread m_thCardCheck;
        //public ELv RetLv;
        //

        /// <summary>
        /// lot 오픈 후 lot End까지 시간
        /// </summary>
        public static Stopwatch swLotTotal = new Stopwatch();
        /// <summary>
        /// lot 오픈 후 lot End까지 에러 시간
        /// </summary>
        public static Stopwatch swLotJam = new Stopwatch();
        /// <summary>
        /// lot 오픈 후 lot End까지 유휴시간
        /// </summary>
        public static Stopwatch swLotIdle = new Stopwatch();


        private CTim m_Timout_485 = new CTim(); //  2023.03.15 Max

        #region Veiw variable
        private int m_iPage;
        public vwAuto m_vwAuto;
        public vwAuto2 m_vwAuto2;       // 200904 SungTae : Qorvo사의 경우 3 Step 요청으로 추가 
        public vwMan  m_vwManl;
        public vwDev  m_vwRcp ;
        private vwWhl  m_vwWhl ;
        private vwEqu  m_vwEq  ;
        private vwSPC  m_vwSPC ;
        private vwOpt  m_vwOpt ;
        private vwMst  m_vwShw ;
        private vwOpManu m_vwOpMn; //190310 ksg :

        private static bool WhlJigShow  ; //190217 ksg :
        private static bool m_bDrsChange;
        public  static bool bInitPage   ;
        private static bool bChkNewTime ;
        public  static bool bShowWhlView; //190523 ksg :
        public static DateTime dtPMMsg            = new DateTime();    // 2021.07.22 lhs : PM Msg 창 표시한 마지막 시간
        public static DateTime dtIOZTDelayStart   = new DateTime();    // 2021.09.14 lhs : IOZT Delay Check Time
        public static DateTime dtSpgWOnLastTime   = new DateTime();    // 2022.06.14 lhs : Sponge Water On Last Time
        #endregion

        public frmMain()
        {
            CLog.mLogSeq("■ SUHWOO Company Grind Main SW START ■");
            if ((int)ELang.English == CData.Opt.iSelLan)
            {
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            }
            else if ((int)ELang.Korea == CData.Opt.iSelLan)
            {
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ko-KR");
            }
            else if ((int)ELang.China == CData.Opt.iSelLan)
            {
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-Hans");
            }
            CLog.mLogSeq(" #Language : " + Thread.CurrentThread.CurrentUICulture.ToString());
            InitializeComponent();
            CLog.mLogSeq(" #InitializeComponent() Complate");

            CData.CheckLicenseValidation();

            // 프로그램 시작 시간 기록
            CData.ProgS = DateTime.Now;
            CData.IdleS = DateTime.Now; //ksg : 초기화
            CData.JamS  = DateTime.Now; //ksg : 초기화
            CData.RunS  = DateTime.Now; //ksg : 추가 및 초기화
            CData.WarmUpS = DateTime.Now.AddHours(-12); //190520 ksg : 1일 전에서 12시간 전으로 수정 시간 계산시 버그 발생

            #region Splash
            Thread thSplash = new Thread(new ThreadStart(CSph.Show));
            thSplash.IsBackground = true;
            thSplash.Start();
            CSph.Update("Initialize Start ...");
            #endregion

            _Initialize();
            CSph.Update("Initialize Finish ...");
            CLog.mLogSeq(" #Initialize Finish");
            GU.Delay(500);
            CSph.Update("All Axes Servo On");
            //CLog.mLogSeq(" #All Axes Servo On");
            //ksg 설비 부팅 시 Thread 생성 후 All Servo On
            CSQ_Man.It.Seq = ESeq.All_Servo_On;

            CSph.Close();
            thSplash.Join();

            
            if (CDataOption.SplType == eSpindleType.EtherCat)
            {
                CSpl.It.Write_Run(EWay.L, 0);
                CSpl.It.Write_Run(EWay.R, 0);
            }
            // 2023.03.15 Max
            else // RS485 RPS Spindle 추가로 else 구문 추가
            {
                bool bExit = false;
                bool bTimeOutFlag = false;

                if (CSpl_485.It.isCommOpen(EWay.L))
                {
                    // RPM Setting
                    CSpl_485.It.Write_Rpm(EWay.L, 0);
                    m_Timout_485.Set_Delay(300);
                    do
                    {
                        Application.DoEvents();
                        if (m_Timout_485.Chk_Delay()) { bTimeOutFlag = true; bExit = true; }
                        bExit = CSpl_485.It.GetAcceptRPMSpindle(EWay.L);
                    } while (bExit != true);
                    CSpl_485.It.SetAcceptRPMSpindle(EWay.L);

                    if (bTimeOutFlag)
                    {
                        CErr.Show(eErr.LEFT_GRIND_SPINDLE_SET_RPM_ERROR);
                        return;
                    }

                    // Spindle RUN
                    /*
                    bExit = false;
                    bTimeOutFlag = false;
                    CSpl_485.It.Write_Run(EWay.L);
                    m_Timout_485.Set_Delay(300);
                    do
                    {
                        Application.DoEvents();
                        if (m_Timout_485.Chk_Delay()) { bTimeOutFlag = true; bExit = true; }
                        bExit = CSpl_485.It.GetAcceptRunSpindle(EWay.L);
                    } while (bExit != true);
                    CSpl_485.It.SetAcceptRunSpindle(EWay.L);

                    if (bTimeOutFlag)
                    {
                        CErr.Show(eErr.LEFT_GRIND_SPINDLE_RUN_RPM_ERROR);
                        return;
                    }
                    */
                }
                else
                {
                    CErr.Show(eErr.LEFT_GRIND_SPINDLE_RUN_RPM_ERROR);
                    return;
                }


                bExit = false;
                bTimeOutFlag = false;
                if (CSpl_485.It.isCommOpen(EWay.R))
                {
                    // RPM Setting
                    CSpl_485.It.Write_Rpm(EWay.R, 0);
                    m_Timout_485.Set_Delay(300);
                    do
                    {
                        Application.DoEvents();
                        if (m_Timout_485.Chk_Delay()) { bTimeOutFlag = true; bExit = true; }
                        bExit = CSpl_485.It.GetAcceptRPMSpindle(EWay.R);
                    } while (bExit != true);
                    CSpl_485.It.SetAcceptRPMSpindle(EWay.R);

                    if (bTimeOutFlag)
                    {
                        CErr.Show(eErr.RIGHT_GRIND_SPINDLE_SET_RPM_ERROR);
                        return;
                    }

                    // Spindle RUN
                    /*
                    bExit = false;
                    bTimeOutFlag = false;
                    CSpl_485.It.Write_Run(EWay.R);
                    m_Timout_485.Set_Delay(300);
                    do
                    {
                        Application.DoEvents();
                        if (m_Timout_485.Chk_Delay()) { bTimeOutFlag = true; bExit = true; }
                        bExit = CSpl_485.It.GetAcceptRunSpindle(EWay.R);
                    } while (bExit != true);
                    CSpl_485.It.SetAcceptRunSpindle(EWay.R);

                    if (bTimeOutFlag)
                    {
                        CErr.Show(eErr.RIGHT_GRIND_SPINDLE_RUN_RPM_ERROR);
                        return;
                    }
                    */
                }
                else
                {
                    CErr.Show(eErr.RIGHT_GRIND_SPINDLE_SET_RPM_ERROR);
                    return;
                }
            }

            CLog.mLogSeq(" #Spindle Type : "+ CDataOption.SplType); // 주석 해제 RS485 RPS Spindle Motor // 2023.03.15 Max

            WhlJigShow = false; //190217 ksg :
            CData.bInitPage = false;
            bChkNewTime = false;
            bShowWhlView = false; //190523 ksg :
            
            //20191028 ghk_auto_tool_offset
            //SCK+ Wheel충돌 발생 시 Auto Tool Setter 기능 쓰지 않으면 Program 재실행 시 Auto Dressing 들어가지 않음 확인
            //SKW Auto Tool Setter 기능 Off 후 Test 시 현지 에이전트 요청으로 조건 변경
            //if (CDataOption.AutoOffset == eAutoToolOffset.Use)
            //{
            //    CData.DrData[0].bDrs  = true;
            //    CData.DrData[1].bDrs  = true;
            //    CData.DrData[0].bDrsR = true;
            //    CData.DrData[1].bDrsR = true;
            //}
            //CLog.mLogSeq(" #AutoOffset : " + CDataOption.AutoOffset.ToString());
            //210406 pjh : Auto Dressing 조건 변경
            CData.DrData[0].bDrs = true;
            CData.DrData[1].bDrs = true;
            CData.DrData[0].bDrsR = true;
            CData.DrData[1].bDrsR = true;
            //
            //20191106 ghk_lightcontrol
            if (CDataOption.LightControl == eLightControl.Use)
            { CLight.It.Open_Port(); }
            CMot.It.SetMemoryLog("[2000X Boot]");

            //System Info
            CLog.mLogSeq(" #CompanyCode : " + CData.CurCompany.ToString());
            CLog.mLogSeq(" #SW Vesion : " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());            
            CLog.mLogSeq(" #OS Version : " + Environment.OSVersion);
            CLog.mLogSeq(" #PC Name : " + Environment.MachineName + " / " + Environment.UserDomainName + " / " + Environment.UserName);

            if (CData.CurCompany == ECompany.ASE_KR && (CData.Dev.aData[(int)EWay.L].sWhl.Equals("") || CData.Dev.aData[(int)EWay.R].sWhl.Equals("")) && GV.DEV_WHL_SYNC)
            {
                CMsg.ShowWhl();
            }
            lbl_Ver.Text = "RPS Delta Ver " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private void _M_tmUpdt_Tick(object sender, EventArgs e)
        {
            lbl_DryrunChk.Visible = CData.Opt.bDryAuto; // 220923 syc : 하웨이 Dry run 상태 표시 요청

            //20190730 ghk_view_gl
            if (CData.CurCompany == ECompany.ASE_K12 || CData.CurCompany == ECompany.USI)
            {
                if (CData.LotInfo.bLotOpen)
                {
                    if (CDataOption.eDfserver == eDfserver.Use)
                    {
                        lblSt_GLState.Visible = true;
                        lblSt_GLState.Text = CData.dfInfo.sGl;
                    }
                }
                else
                {
                    lblSt_GLState.Visible = false;
                }
            }

            if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC ||      // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                CData.CurCompany == ECompany.ASE_KR || CData.CurCompany == ECompany.SST ||
                CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK || CData.CurCompany == ECompany.JCET) //191202 ksg : ->200121 ksg : SCK 추가, 200625 lks
            {
                WheelChangeCheckSeq();
                DresserChangeCheckSeq();

                if (CData.WhlChgSeq.bStart && CData.WhlChgSeq.bBtnHide) MainBtnHide(true);
                else MainBtnHide(false);
            }
            //koo : Qorvo Lot Rework
            // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
            //if(CData.CurCompany == ECompany.Qorvo || (CData.CurCompany == ECompany.Qorvo_DZ) || CData.CurCompany == ECompany.SST) //191202 ksg :
            if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC ||
                CData.CurCompany == ECompany.SST)
            {
                if (CData.bPreLotEndMsgShow)
                {
                    CData.bPreLotEndMsgShow = false;
                    if (CData.LotInfo.iTOutCnt != CData.LotInfo.iTotalStrip)
                    {
                        string InputCountmsg = "Strip Input Count : " + CData.LotInfo.iTotalStrip.ToString();
                        string OutputCountmsg = "Strip Output Count : " + CData.LotInfo.iTOutCnt.ToString();
                        string msg = InputCountmsg + Environment.NewLine + OutputCountmsg + Environment.NewLine + "Work quantity is incorrect. Do you want to work again?";
                        if (CMsg.Show(eMsg.Warning, "Warning", msg) == DialogResult.OK)
                        {
                            CData.LotInfo.iTotalMgz++;
                            CSq_OfL.It.m_GetQcWorkEnd = false;

                            int iMax;
                            if (CData.CurCompany == ECompany.SkyWorks) iMax = (int)EPart.OFR;
                            else iMax = (int)EPart.OFL;
                            for (int i = 0; i < iMax; i++)
                            {
                                CData.Parts[i].iStat = ESeq.Idle;
                                CData.Parts[i].iSlot_No = 0;
                                for (int j = 0; j < CData.Dev.iMgzCnt; j++)
                                {
                                    CData.Parts[(int)EPart.ONL].iSlot_info[j] = 0;
                                }
                                CData.Parts[i].sBcr = "";
                                CData.Parts[i].bExistStrip = false;
                                // 2020.12.12 JSKim St
                                CData.Parts[i].iStripStat = 0;
                                // 2020.12.12 JSKim Ed
                            }
                            CData.Parts[(int)EPart.OFP].iMGZ_No++; //190714 ksg :
                            CData.Parts[(int)EPart.GRDL].iMGZ_No++; //190714 ksg :
                            CData.Parts[(int)EPart.GRDR].iMGZ_No++; //190714 ksg :
                            CData.Parts[(int)EPart.OFL].iStat = ESeq.OFL_BtmRecive;
                            if (CData.CurCompany == ECompany.SkyWorks) CData.Parts[(int)EPart.OFR].iStat = ESeq.OFL_BtmRecive;

                            return;
                        }
                        else
                        {
                            CData.LotInfo.iTotalStrip = CData.LotInfo.iTOutCnt;
                        }
                    }
                }
            }


            // 2021-06-04, jhLee, Multi-LOT, LOT이 종료될 경우 사용자에게 표시해준다.
            if (CDataOption.UseMultiLOT)
            {
                // Multi-LOT Option 설정 표시
                if (!lblOption1.Visible)
                {
                    lblOption1.Visible = true;
                }
                else    // 현재 Option Lable을 보여주고 있다.
                {
                    if (CData.IsMultiLOT())
                    {
                        if (lblOption1.BackColor != Color.ForestGreen)
                        {
                            lblOption1.BackColor = Color.ForestGreen;
                            lblOption1.ForeColor = Color.White;
                        }
                    }
                    else if (lblOption1.BackColor != Color.FromArgb(64, 64, 64))
                    {
                        lblOption1.BackColor = Color.FromArgb(64, 64, 64);
                        lblOption1.ForeColor = Color.DimGray;
                    }
                }
            }

            // Multi-LOT 사용시에는 LOT 종료 메세지를 따로 이용한다.
            if (CData.IsMultiLOT())
            {
                // Unloading 작업을 마친 LOT의 종료 메세지를 보여주도록 한다.
                if (CData.bLotCompleteMsgShow)
                {
                    CData.bLotCompleteMsgShow = false;      // 사용된 Flag reset
                    CData.bLotCompleteBuzzer = true;        // Buzzer를 울리도록 한다.

                    CMsg.ShowLotEnd();                      // LOT End를 표시해준다.
                }

                // 투입 종료를 할것인지 작업자의 답변을 기다리는 메세지를 보여주고 응답을 받도록 한다.
                if (CData.LotMgr.UserConfirmReply == ELotUserConfirm.eQuestion)
                {
                    CData.LotMgr.UserConfirmReply = ELotUserConfirm.eWaitReply;     // 화면에 보여주고 응답을 기다리는 과정이다.

                    // Buzzer가 ON 되도록 Flag를 설정해준다.
                    CData.bLotCompleteBuzzer = true;
                    CMsg.ShowLotCloseConfirm();             // LOT Close 여부를 묻는다.
                }
            }
            else
            {
                // Multi-LOT 미사용시
                if (CData.bLotEndMsgShow)
                {
                    CData.bLotEndMsgShow = false;
                    //190407 ksg :
                    if (CData.LotInfo.iTInCnt == CData.LotInfo.iTOutCnt)
                    {
                        // 190701-maeng
                        StringBuilder mBuilder = new StringBuilder();
                        mBuilder.AppendLine("Lot End");
                        mBuilder.Append("Name : ");
                        mBuilder.AppendLine(CData.sLastLot);
                        mBuilder.Append("Device : ");
                        mBuilder.AppendLine(CData.Dev.sName);
                        mBuilder.Append("MGZ Count : ");
                        mBuilder.Append(CData.iLastMGZ.ToString());
                        CMsg.Show(eMsg.Notice, "Notice", mBuilder.ToString()); //"Lot End");
                    }
                    else
                    {
                        CMsg.Show(eMsg.Error, "Error", "Lot End Now, But Different In/Out Strip Count. So Check Strip Count");
                    }
                }
            }

            //---------------------------------------
            // 2021.10.04 lhs Start : OnLoader에서 사용자에게 Mgz Pick을 할 것인지 WorkEnd를 할 것인지 묻기 (SCK+ VOC)
            // 조건1 : Strip이 적게 투입된 경우 계속 Mgz Pick을 수행하므로 (CData.LotInfo.iTInCnt    <  CData.LotInfo.iTotalStrip)
            // 조건2 : 투입된 Mgz이 사용자가 설정한 Mgz 이상일 경우 묻기    (CData.Parts[ONL].iMGZ_No >= CData.LotInfo.iTotalMgz)
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            {
                if (CData.eOnLUserConfirm == EOnLUserConfirm.eQuestion)
                {
                    CData.eOnLUserConfirm = EOnLUserConfirm.eWaitReply;

                    string sMsg = string.Format("Input MGZ >= Total MGZ : ({0} >= {1})\r\n", CData.Parts[(int)EPart.ONL].iMGZ_No, CData.LotInfo.iTotalMgz);
                    sMsg += "OnLoader Work End?";
                    if (CMsg.Show(eMsg.Query, "Question : OnLoader", sMsg) == DialogResult.OK) // OnLoader workend/pick을 묻는다
                    {
                        CData.eOnLUserConfirm = EOnLUserConfirm.eReplyYes;
                    }
                    else
                    {
                        CData.eOnLUserConfirm = EOnLUserConfirm.eReplyNo;
                    }
                }
            }
            // 2021.10.04 lhs End
            //---------------------------------------

            if (CSQ_Main.It.m_bPause)
            {
                if (CSQ_Main.It.bSetUpStripStatus) { lblSt_Main.Text = "SET UP STRIP WORK"; }   //210309 pjh : Set Up Strip Display
                else
                {
                    if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                    {
                        string sTxt = "LOAD STOP ON (";
                        sTxt += CSQ_Main.It.m_iStat.ToString().Replace("_", " ").ToUpper();
                        sTxt += ")";
                        lblSt_Main.Text = sTxt;
                    }
                    else
                    {
                        lblSt_Main.Text = "LOAD STOP ON";
                    }
                }
                lblSt_Main.ForeColor = Color.Red;
            }
            else
            {
                lblSt_Main.Text = CSQ_Main.It.m_iStat.ToString().Replace("_", " ").ToUpper();
                lblSt_Main.ForeColor = Color.FromArgb(43, 167, 205);
            }

            // 2021.09.30 lhs Start : Loading Stop 자동 해제되지 않도록 수정 (SCK VOC)
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            {
                // Stop 시키지 않고 현상태 그대로 유지
            }
            else  // 기존 로직
            // 2021.09.30 lhs End
            {
                if (CSQ_Main.It.m_bPause)
                {
                    if (CSq_OfL.It.Get_StripNone())
                    {
                        CSQ_Main.It.m_bPause = false;
                        CSQ_Main.It.m_bToStop = true;
                        CData.bWhlDrsLimitAlarmSkip = false; //200730 jhc : Wheel/Dresser Limit Alaram 보류용 (ASE-Kr VOC) - LOADING STOP OFF 시 Wheel/Dresser Limit Alarm 보류 해제
                    }
                }
            }

            // 2021-06-02, jhLee : Multi-LOT, On-Loader부 Light-Curtain이 감지되면 정지되었던 Mgz Pickup 동작을 재개할 수 있도록 Flag를 지정해준다.
            if (CData.IsMultiLOT())
            {
                // ON -> OFF 로 변경되었을때만 Flag를 설정해준다.

                if (!CSq_OnL.It.Chk_LightCurtain())          // Light-Curtain OFF
                {
                    if (lblSt_LeftArea.Visible == true)     // 이전에 ON 상태였다.
                    {
                        CData.LotMgr.IsLightCurtailDetect = true;   // Set Flag
                    }
                }
            }

            // 2021-02-05, jhLee : Area sensor 감지 변수를 기존 Main Sequence에서 각각의 On-Loader와 Off-Loader로 이관시켰다.
            lblSt_LeftArea.Visible = CSq_OnL.It.Chk_LightCurtain();
            lblSt_RightArea.Visible = CSq_OfL.It.Chk_LightCurtain();


            if ((CData.Whls[0].dWhlLimit > (CData.Whls[0].dWhlAf - 0.5)) || (CData.Whls[1].dWhlLimit > (CData.Whls[1].dWhlAf - 0.5)) ||
                (CData.Whls[0].dDrsLimit > (CData.Whls[0].dDrsAf - 0.5)) || (CData.Whls[1].dDrsLimit > (CData.Whls[1].dDrsAf - 0.5)))
            {
                this.lblSt_Main.Font = new System.Drawing.Font("Arial Black", 15F, System.Drawing.FontStyle.Bold);
                lblSt_Main.Text += "\r\n(Warning)Need to Change ";
                if (CData.Whls[0].dWhlLimit > (CData.Whls[0].dWhlAf - 0.5)) { lblSt_Main.Text += "'L Wheel' "; }
                if (CData.Whls[1].dWhlLimit > (CData.Whls[1].dWhlAf - 0.5)) { lblSt_Main.Text += "'R Wheel' "; }
                if (CData.Whls[0].dDrsLimit > (CData.Whls[0].dDrsAf - 0.5)) { lblSt_Main.Text += "'L Dresser' "; }
                if (CData.Whls[1].dDrsLimit > (CData.Whls[1].dDrsAf - 0.5)) { lblSt_Main.Text += "'R Dresser' "; }
            }
            else
            {
                this.lblSt_Main.Font = new System.Drawing.Font("Arial Black", 26F, System.Drawing.FontStyle.Bold);
            }
            lblSt_Sub.Text = CSQ_Man.It.Seq.ToString().Replace("_", " ");
            lblSt_Grp.Text = CDev.It.m_sGrp;
            lblSt_Dev.Text = CData.Dev.sName;

            // 2021.09.07 lhs Start : [JCET VOC] LotEnd시 Device Name 지워 달라고 요청  
            if (CData.bEraseLotName)
            {
                lblSt_Dev.Text = "";
            }
            // 2021.09.07 lhs End

            // Main화면에 현재 시간표시
            lblSt_Time.Text = DateTime.Now.ToString();

            if (CSQ_Main.It.m_bRun) { lblSt_Sub.Visible = false; }
            else { lblSt_Sub.Visible = true; }

            if (CData.bLotTotal) { swLotTotal.Start(); }
            else { swLotTotal.Stop(); }
            if (CData.bLotTotalReset) { swLotTotal.Reset(); CData.bLotTotalReset = false; }

            if (CData.bLotJam) { swLotJam.Start(); }
            else { swLotJam.Stop(); }

            if (CData.bLotJamReset) { swLotJam.Reset(); CData.bLotJamReset = false; }

            if (CData.bLotIdle) { swLotIdle.Start(); }
            else { swLotIdle.Stop(); }

            if (CData.bLotIdleReset) { swLotIdle.Reset(); CData.bLotIdleReset = false; }

            CData.SpcInfo.sTotalTime = swLotTotal.Elapsed.ToString(@"hh\:mm\:ss");

            // Error 화면 표시
            if (CData.ShowErrForm && !m_vwErr.Visible)
            {
                m_vwErr.SetErr(CData.IsErr);
                m_vwErr.BringToFront();
                m_vwErr.Visible = true;
            }
            else if (!CData.ShowErrForm)
            {
                m_vwErr.Visible = false;
            }

            //---------------------------
            // 2021.09.14 lhs Start : SCK/JSCK는 별도로 IOZT Off 로직 구현
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            {
                if (CSQ_Main.It.m_iStat == EStatus.Idle || CSQ_Main.It.m_iStat == EStatus.Stop)
                {
                    // Lot End 일 경우 확인 필요
                    if (CData.Opt.bUseIOZTSolOff && CData.Opt.bIOZT_ManualClick == false)  // Off 기능 사용시 && 수동클릭 아닐 경우 적용
                    {
                        TimeSpan tsDelay = DateTime.Now - dtIOZTDelayStart;
                        if ((int)tsDelay.TotalSeconds >= CData.Opt.nIOZTSolOffDelaySec)  // 시간 체크
                        {
                            if (CIO.It.Get_Y(eY.IOZT_Power))
                            {
                                CIO.It.Set_Y(eY.IOZT_Power, false); // SCK, JSCK  
                            }
                        }
                    }
                }
                else
                {
                    dtIOZTDelayStart = DateTime.Now;  // 초기화
                    if (CSQ_Main.It.m_iStat == EStatus.Auto_Running)
                    {
                        CData.Opt.bIOZT_ManualClick = false;
                    }
                }
            }
            // 2021.09.14 lhs End
            //---------------------------

            //---------------------------
            // 2022.06.14 lhs Start : (SCK+) Sponge Water Auto On/Off
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            {
                if (CSQ_Main.It.m_iStat == EStatus.Idle || CSQ_Main.It.m_iStat == EStatus.Stop)
                {
                    if (CData.Opt.bSpgWtAutoOnOff)
                    {
                        TimeSpan tsWOffIntv = DateTime.Now - CData.dtSpgWOffLastTime;

                        if ((int)tsWOffIntv.TotalMinutes >= CData.Opt.nSpgWtOffMin)  // Off 상태 시간 체크
                        {
                            if (CIO.It.Get_X(eX.DRY_ClnBtmFlow) == false)
                            {
                                CIO.It.Set_Y(eY.DRY_ClnBtmFlow, true);

                                // On 시간 체크
                                dtSpgWOnLastTime = DateTime.Now;
                            }
                        }

                        TimeSpan tsWOnIntv = DateTime.Now - dtSpgWOnLastTime;
                        if ((int)tsWOnIntv.TotalMinutes >= CData.Opt.nSpgWtOnMin)  // On 상태 시간 체크
                        {
                            if (CIO.It.Get_X(eX.DRY_ClnBtmFlow))
                            {
                                CIO.It.Set_Y(eY.DRY_ClnBtmFlow, false);

                                // On 시간 체크
                                CData.dtSpgWOffLastTime = DateTime.Now;

                            }
                        }
                    }
                }
            }
            // 2022.06.14 lhs End
            //---------------------------

            if (CSQ_Main.It.m_iStat != EStatus.Error)
            {
                if (!CIO.It.Get_X(eX.SYS_MC)) { CErr.Show(eErr.SYSTEM_MAIN_CONTROLLER_ERROR); }   //MC 체크 normal = on
                if (!CIO.It.Get_X(eX.SYS_Pneumatic)) { CErr.Show(eErr.SYSTEM_PNEUMATIC_ERROR); }   //Main 공압 체크 normal = on
                if (CSQ_Main.It.m_bPCWCheck)
                {
                    if (!CIO.It.Get_X(eX.GRDL_SplPCW)) { CErr.Show(eErr.LEFT_GRIND_SPINDLE_COOLANT_OFF); }   //왼쪽 스핀들 PCW 체크 normal = on
                    if (!CIO.It.Get_X(eX.GRDR_SplPCW)) { CErr.Show(eErr.RIGHT_GRIND_SPINDLE_COOLANT_OFF); }   //오른쪽 스핀들 PCW 체크 normal = on
                }
                if (CIO.It.Get_X(eX.GRDL_SplPCWTemp)) { CErr.Show(eErr.GRIND_SPINDLE_COOLANT_TEMP); }   //스핀들 PCW 온도 체크 normal = off

                //이오나이저 전원 체크 200622 lks
                if (CData.CurCompany == ECompany.JCET)
                {
                    if (CIO.It.Get_X(eX.IOZL_Alarm)) { CErr.Show(eErr.INRAIL_IONIZER_POWER_ERROR); }
                    if (CIO.It.Get_X(eX.IOZR_Alarm)) { CErr.Show(eErr.DRY_IONIZER_POWER_ERROR); }
                }
            }

            //190110 ksg : Warm Up
            if ((CIO.It.Get_X(eX.GRDR_SplWater) && !CData.Opt.aTblSkip[(int)EWay.R]) ||
                (CIO.It.Get_X(eX.GRDL_SplWater) && !CData.Opt.aTblSkip[(int)EWay.L]))
            {
                CData.WarmUpS = Convert.ToDateTime(DateTime.Now);  // Warm Up 마지막 시간 
            }

            if (CData.bInitPage)
            {
                CData.bInitPage = false;
                ChangePage(0);
                rdb_Auto.Checked = true;
                m_vwManl.InitPage();
                m_vwSPC.InitPage();
                //m_vwOpt .InitPage();
                m_vwEq.InitPage();
            }

            // 2021.06.09 lhs Start : Operator가 아닐 경우 추가
            //if(CData.bLastClick)
            if (CData.bLastClick && CData.Lev != ELv.Operator)
            // 2021.06.09 lhs End 
            {
                CData.bLastClick = false;
                CData.LastClickS = DateTime.Now;
                bChkNewTime = true;
            }

            //190509 ksg :
            if (CData.Opt.iChangeLevel > 0 && m_iPage >= 0 && bChkNewTime)
            {
                //2021.08.20 syc : Qorvo Auto user level change 기능 원하지 않아 기능 라이센스화
                // // 2021.06.09 lhs Start 코드 사용
                if (CDataOption.SkipAutoUserLevelChange == false) //Qorvo 외 타사이트는 적용을 기본으로 사용할 예정 : Skip 비활성화 
                {
                    // 2021.06.09 lhs Start
                    if (CData.Lev == ELv.Operator)
                    {
                        bChkNewTime = false;
                    }
                    // 2021.06.09 lhs End

                    DateTime CurTime = DateTime.Now;
                    TimeSpan CurTime1 = CurTime - CData.LastClickS;

                    if (CurTime1.Hours > 0 || CurTime1.Minutes >= CData.Opt.iChangeLevel)
                    {
                        // 2021.06.09 lhs Start
                        // : 조작중 자동으로 사용자 레벨 바뀌는 시점에 메시지창 표시
                        // : YES or NO 선택, 1분동안 선택안할 경우 자동으로 NO 선택되어 Operator 레벨로 전환됨

                        ////ChangePage(0);
                        //rdb_Auto.Checked = true;
                        //AutoChangeLevel();
                        //bChkNewTime = false;

                        bChkNewTime = false;

                        string strMsg = "It's time for the user level to change automatically !!!\n\r";
                        strMsg += "\n\r";
                        strMsg += "Click YES to extend the time to continue the current operation.\n\r";
                        strMsg += "Click NO to stop working and change to Operator level.\n\r";
                        strMsg += "\n\r";
                        strMsg += "After 1 minute without clicking, NO is selected.\n\r";

                        DialogResult dlgRst = CMsg.Show(eMsg.QueryAndNo, "Notice", strMsg);

                        if (dlgRst == DialogResult.OK)      // 시간연장
                        {
                            CData.bLastClick = true;
                        }
                        else                                // Operator 레벨로 변경
                        {
                            rdb_Auto.Checked = true;
                            AutoChangeLevel();

                            CData.bLastClick = false;
                            bChkNewTime = false;
                        }
                        // 2021.06.09 lhs End
                    }
                }
                //2021.08.20 syc : Qorvo Auto user level change 기능 원하지 않아 기능 라이센스화 
                //업데이트 이전 코드 그대로 사용
                else //Qorvo 외 타사이트는 적용을 기본으로 사용할 예정 : Skip 활성화 
                {
                    DateTime CurTime = DateTime.Now;
                    int LastMinute = CData.LastClickS.Minute;
                    int CurMinute = CurTime.Minute;
                    TimeSpan CurTime1 = CurTime - CData.LastClickS;

                    //if((CurMinute - LastMinute) >= CData.Opt.iChangeLevel)
                    if (CurTime1.Hours > 0 || CurTime1.Minutes >= CData.Opt.iChangeLevel)
                    {
                        //ChangePage(0);
                        rdb_Auto.Checked = true;
                        AutoChangeLevel();
                        bChkNewTime = false;
                    }
                }
            }

            if (bShowWhlView)
            {
                bShowWhlView = false;
                AutoChangeWheelView();
            }
            //20190618 ghk_dfserver
            if (CDataOption.eDfserver == eDfserver.Use)
            {//Ase kr, AseK26 만 적용
                if (CSQ_Main.It.m_iStat != EStatus.Error)
                {
                    if (!CData.Dev.bDfServerSkip)
                    {//Dynamic Function Skip 아닐 경우
                        if (CDf.It.clientSocket.clientSocket == null)
                        {
                            CDf.It.ConnectServer();
                            CDf.It.SendID();
                        }
                        if (!CDf.It.clientSocket.clientSocket.Connected)
                        {
                            CDf.It.ConnectServer();
                            if (CDf.It.iCon == -1)
                            {
                                CErr.Show(eErr.DYNAMIC_FUNCTION_SERVER_NEED_RUN);
                            }
                            CDf.It.SendID();
                        }
                    }
                    else
                    {
                        if (CDf.It.clientSocket.clientSocket != null)
                        {
                            if (CDf.It.clientSocket.clientSocket.Connected)
                            { CDf.It.DisConnectServer(); }
                        }
                    }
                }
            }
            if (CData.CurCompany == ECompany.SkyWorks)
            {
                bool r1 = CSq_OnL.It.m_bHD &&
                          CSq_OfL.It.m_bHD &&
                          CSq_OnP.It.m_bHD &&
                          CSq_OfP.It.m_bHD &&
                          CSq_Inr.It.m_bHD &&
                          CData.L_GRD.m_bHD &&
                          CData.R_GRD.m_bHD &&
                          CSq_Dry.It.m_bHD;
            }

            bool bOnp = false;
            bool bOfp = false;
            if (CDataOption.IsRevPicker)
            {
                bOnp = !CIO.It.Get_Y(eY.ONP_Vacuum1) || !CIO.It.Get_Y(eY.ONP_Vacuum2) || CIO.It.Get_Y(eY.ONP_Ejector);
                bOfp = !CIO.It.Get_Y(eY.OFFP_Vacuum1) || !CIO.It.Get_Y(eY.OFFP_Vacuum2) || CIO.It.Get_Y(eY.OFFP_Ejector);
            }
            else
            {
                bOnp = CIO.It.Get_Y(eY.ONP_Vacuum1) || CIO.It.Get_Y(eY.ONP_Vacuum2) || CIO.It.Get_Y(eY.ONP_Ejector);
                bOfp = CIO.It.Get_Y(eY.OFFP_Vacuum1) || CIO.It.Get_Y(eY.OFFP_Vacuum2) || CIO.It.Get_Y(eY.OFFP_Ejector);
            }

            CIO.It.Set_Y(eY.OFFP_Drain, bOfp ? false : true);
            CIO.It.Set_Y(eY.ONP_Drain, bOnp ? false : true);


            // 2021.07.21 lhs Start : PM Count 체크하여 알림창 표시, 모달리스
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK || CData.CurCompany == ECompany.ASE_K12)//220707 pjh : ASE KH 조건 추가
            {
                CheckPMCount();
            }
            // 2021.07.21 lhs End

            // 2021.08.29 lhs Start : Spindle Load Current Alarm 창 표시
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            {
                //CheckRangeSpindleCurrent(); // Spindle 전류               
            }
            // 2021.08.29 lhs End

            // 2022.06.07 lhs Start : SecsGem 사용시 SecsGem Skip 상태 표시
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            {
                if (CDataOption.SecsUse == eSecsGem.Use && 
                    !CData.Opt.bSecsUse) {   lblSt_SecsGemSkip.Visible = true;   }
                else                     {   lblSt_SecsGemSkip.Visible = false;  }
            }
            // 2022.06.07 lhs End

            // 2022.06.07 lhs Start : Door Skip 상태 표시
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            {
                if (CData.Opt.bDoorSkip)    { lblSt_DoorSkip.Visible = true;    }
                else                        { lblSt_DoorSkip.Visible = false;   }

                lblSt_DoorOpend.Visible =  (CSQ_Main.It.CheckDoor() == false); // 도어 오픈시 표시
            }
            // 2022.06.07 lhs End

            // 2022.07.06 lhs : Wheel Clean Nozzle Skip 메인에 표시
            lblSt_WhNzlSkip.Visible = CDataOption.IsWhlCleaner && CData.Opt.bWhlClnSkip;


            _CardChangeLevel();
           
        }

		private void _M_tmView_Tick(object sender, EventArgs e)
        {
            //190217 ksg :
            if (CData.bShowWhlJig && !m_vwZig.Visible)
            {
                m_vwZig.SetType(false);
                m_vwZig.BringToFront();
                m_vwZig.Visible = true;
            }
            else if (!CData.bShowWhlJig)
            {
                WhlJigShow = false;
                m_vwZig.Visible = false;
            }

            if (CData.bDrsChange && !m_vwDrs.Visible)
            {
                m_vwDrs.SetType(true);
                m_vwDrs.BringToFront();
                m_vwDrs.Visible = true;
            }
            else if (!CData.bDrsChange)
            {
                m_bDrsChange = false;
                m_vwDrs.Visible = false;
            }

            // 200803 jym
            if (CData.VwWhl.bView && !m_vwSelWhl.Visible)
            {
                m_vwSelWhl.Set();
                m_vwSelWhl.BringToFront();
                m_vwSelWhl.Visible = true;
            }
            else if (!CData.VwWhl.bView)
            {
                m_vwSelWhl.Visible = false;
            }

            // 200803 jym
            if (CData.VwMsg.bView && !m_vwSelMsg.Visible)
            {
                m_vwSelMsg.Set();
                m_vwSelMsg.BringToFront();
                m_vwSelMsg.Visible = true;
            }
            else if (!CData.VwMsg.bView)
            {
                m_vwSelMsg.Visible = false;
            }

            // 201023 jym
            if (CData.VwKey.bView && !m_vwSelKey.Visible)
            {
                m_vwSelKey.Set();
                m_vwSelKey.BringToFront();
                m_vwSelKey.Visible = true;
                m_vwSelKey.LoadImage();
            }
            else if (!CData.VwKey.bView)
            {
                m_vwSelKey.Visible = false;
            }

            // 2021.04.09 SungTae Start : Wheel History Auto Delete 추가
            if (DateTime.Today.Hour.ToString("HH") == "00" && DateTime.Today.Minute.ToString("mm") == "00" )
            {
                _AutoDeleteWheelHistory(EWay.L);
                _AutoDeleteWheelHistory(EWay.R);
            }           
            // 2021.04.09 SungTae End

            //211202 pjh : Network 연결 시 Display 설정
            if(CData.m_bNetCheck && CData.Opt.bDFNetUse)
            {
                lbl_ChkNetConnect.BackColor = Color.Cyan;
                lbl_ChkNetConnect.Text = "Network Drive Online";
            }
            else
            {
                lbl_ChkNetConnect.BackColor = Color.Red;
                lbl_ChkNetConnect.Text = "Network Drive Offline";
            }
            //
        }

        private void _Initialize()
        {
            int iRet = 0;
            // Data Class Initialize
            CData.Init();
            try
            {
                if (!CData.VIRTUAL)
                {
                    CSph.Update("WMX create device");
                    try
                    {
                        // WMX device create
                        iRet = CWmx.It.Create();
                        if (iRet != 0)
                        {
                            CErr.Show(eErr.SYSTEM_WMX_CREATE_DEVICE);
                        }
                        else
                        {
                            // Retry 5번
                            for (int i = 1; i <= WMX_RETRY; i++)
                            {
                                CSph.Update("WMX start communication - " + i);
                                // WMX start communication
                                if (!CWmx.It.Open())
                                {
                                    CWmx.It.WLib.StopCommunication();
                                    // 1sec Delay
                                    GU.Delay(1000);
                                    if (i == 5)
                                    {
                                        CErr.Show(eErr.SYSTEM_WMX_START_COMMUNICATION);
                                    }
                                }
                                else
                                { break; }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        CMsg.Show(eMsg.Error, "Error - Controller", ex.Message);
                        CData.WMX = false;
                    }
                }

                CSph.Update("Loading config files");
                try
                {
                    // UPH때문에 변경 해야 함.
                    if (CData.CurCompany == ECompany.Qorvo    ||
                        CData.CurCompany == ECompany.Qorvo_DZ ||
                        CData.CurCompany == ECompany.Qorvo_RT ||        // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                        CData.CurCompany == ECompany.Qorvo_NC ||        // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                        CData.CurCompany == ECompany.SST)
                    { GV.PRB_DELAY = 900; }

                    // 200916 jym : 측정 대기시간 증가
                    if (CData.CurCompany == ECompany.SkyWorks)
                    {
                        GV.PRB_DELAY = 3000;
                        // 200917 jym : 드레싱 1스텝 하강 사용
                        GV.TEMP_DRS_1STEP = true;
                    }

                    //201014 jhc : ASE-Kr도 적용 // 200917 jym : 드레싱 1스텝 하강 사용
                    if (CData.CurCompany == ECompany.ASE_KR)
                    {
                        GV.TEMP_DRS_1STEP = true;
                        GV.ORIENTATION_ONE_TIME_SKIP = true; //201103 jhc : Orientation Check 1회 Skip 기능 추가 (ASE-Kr VOC)
                    }

                    _Init_Config();
                }
                catch (Exception ex)
                { CMsg.Show(eMsg.Error, "Error - Config", ex.Message); }
                CSph.Update("Loading error files");
                try
                {
                    _Init_Errlist();
                }
                catch (Exception ex)
                { CMsg.Show(eMsg.Error, "Error - Error File", ex.Message); }
                CSph.Update("Equipment initialize");
                try
                {
                    _Init_Equip();
                }
                catch (Exception ex)
                { CMsg.Show(eMsg.Error, "Error - Equipment", ex.Message); }
                CSph.Update("View initialize");
                try
                {
                    _Init_Screen();
                }
                catch (Exception ex)
                { CMsg.Show(eMsg.Error, "Error - Screen", ex.Message); }

                //190403 ksg :
                if ((CDataOption.UseQC && CData.Opt.bQcUse) && CData.WMX)
                {
                    CSph.Update("QC initialize");
                    // 2020.10.15 JSKim St
                    /*if (CTcpIp.It.IsConnect())
                    {
                        //CTcpIp.It.TcpIpClose();
                    }*/

                    if (CGxSocket.It.IsConnected())
                    {
                        CGxSocket.It.CloseClient();
                    }

                    Thread.Sleep(1000);
                    // 2020.10.15 JSKim Ed
                    //CTcpIp.It.TcpIpConnect();
                    CGxSocket.It.Connect();
                }
                else if (CData.Opt.bQcUse && CDataOption.UseQC)// only for test
                {
                    
                    if (CData.Opt.bQcSimulation)
                    {

                    }
                    //CSQ7_Dry_SIM.It.startAutoRun();
                    //QC Client
                    
                    if (CGxSocket.It.IsConnected())
                    {
                        CGxSocket.It.CloseClient();
                    }

                    
                    // 2020.10.15 JSKim Ed
                    //CTcpIp.It.TcpIpConnect();
                    CGxSocket.It.Connect();


                }

                //20200311 jhc :
                if (CDataOption.BcrInterface == eBcrInterface.Udp)
                {
                    CSph.Update("2D Vision initialize");
                    CBcr.It.UdpOpen(); //UDP Server Socket 시작
                    Thread.Sleep(100);
                    CBcr.It.UdpRun2DVisionProcess(); //2D Vision SW 실행
                }

                //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)
                //201012 jhc : (KEYENCE BCR)/(RFID) 옵션으로 구분 적용
                if ((CDataOption.RFID == ERfidType.Keyence) && (CDataOption.OnlRfid == eOnlRFID.Use || CDataOption.OflRfid == eOflRFID.Use))
                //if ((CData.CurCompany == ECompany.SPIL) && (CDataOption.OnlRfid == eOnlRFID.Use || CDataOption.OflRfid == eOflRFID.Use))
                {
                    //20200611 jhc : Connection Error 구분 처리
                    int ret = CKeyenceBcr.It.InitializeKeyenceBcr();
                    if (ret != 0)
                    {
                        if (ret == 1)
                        { CMsg.Show(eMsg.Error, "Error : On-Loader MGZ BCR Connection Fail", "Check On-Loader MGZ Barcode Reader"); }
                        else if (ret == 2)
                        { CMsg.Show(eMsg.Error, "Error : Off-Loader MGZ BCR Connection Fail", "Check Off-Loader MGZ Barcode Reader"); }
                        else if (ret == 3)
                        { CMsg.Show(eMsg.Error, "Error : On-Loader and Off-Loader MGZ BCR Connection Fail", "Check On-Loader and Off-Loader MGZ Barcode Reader"); }
                        else
                        { CMsg.Show(eMsg.Error, "Error : MGZ BCR Connection Fail", "Check MGZ Barcode Reader"); }
                    }
                }
                //

                CBcr.It.SaveStatus(false);
            }
            catch (Exception ex)
            {
                if (CData.CurCompany == ECompany.Qorvo) CSQ_Main.It.isSPC = true;//220526 pjh : Qorvo는 Data Save Error 발생 시 Buzzer On
                CMsg.Show(eMsg.Error, "Error", ex.Message);
            }
            finally
            {

                // Main thread
                ThreadStart mTst = new ThreadStart(_Cycle);
                m_thCyl = new Thread(mTst);
                //200424 jym : Priority 추가
                m_thCyl.Priority = ThreadPriority.Highest;
                m_thCyl.Start();

                // 210601 jym : 네트워크 스레드 추가
                //if (CData.CurCompany == ECompany.Qorvo    || CData.CurCompany == ECompany.Qorvo_DZ ||
                //    CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC ||
                //    CData.CurCompany == ECompany.JCET)
                if(CDataOption.UseNetDrive || CDataOption.UseDFNet)//210817 pjh : Net drive License로 관리
                {
                    m_thNet = new Thread(new ThreadStart(_NetDrive));
                    m_thNetCheck = new Thread(new ThreadStart(_NetDriveCheck));//211202 pjh : Network Connection Check Thread
                    m_thNet.Priority = ThreadPriority.Lowest;
                    m_thNetCheck.Priority = ThreadPriority.Lowest;//211202 pjh : Network Connection Check Thread
                    m_thNet.Start();
                    m_thNetCheck.Start();//211202 pjh : Network Connection Check Thread
                }

                m_thLog = new Thread(new ThreadStart(_Log));
                //200424 jym : Priority 추가
                m_thLog.Priority = ThreadPriority.Lowest;
                m_thLog.Start();

                m_thSpl = new Thread(new ThreadStart(_SdoComm));
                m_thSpl.Priority = ThreadPriority.Lowest;
                m_thSpl.Start();

                m_tmUpdt = new System.Windows.Forms.Timer();
                m_tmUpdt.Interval = 100;
                m_tmUpdt.Tick += _M_tmUpdt_Tick;
                m_tmUpdt.Start();

                m_tmView = new System.Windows.Forms.Timer();
                m_tmView.Interval = 10;
                m_tmView.Tick += _M_tmView_Tick;
                m_tmView.Start();

                //m_thQc = new Thread(new ThreadStart(_TcpIpQueuePoress));
                //m_thQc.Start();
                

                //200212 ksg :
                if (CDataOption.SecsUse == eSecsGem.Use)
                {
                    for (int i = 0; i < CData.JSCK_Max_Cnt; i++)
                    {
                        CData.JSCK_Gem_Data[i] = new JSCK_SECSGEM_STRIP_DATA();
                    }
                    CData.GemForm = new frmGEM();
                    CData.GemForm.LastLoad();
                }

                if (CDataOption.SecsUse == eSecsGem.NotUse) //190724 ksg :
                {
                    m_fmView_Ori = new frmView();
                    //m_fmView_Ori.Location = new Point(1280, 0);
                    m_fmView_Ori.Location = new Point(-1280, 0);
                    //m_fmView_Ori.Location = new Point(0, 0); //ksg : view 화면 debug시 사용
                    m_fmView_Ori.Show();
                    if (CData.GemForm != null) CData.GemForm.Log_wt("CData.Opt.bSecsUse = false");
                }
                else
                {
                    m_fmView = new frmView2();
                    m_fmView.Location = new Point(-1280, 0);
                    //m_fmView.Location = new Point(0, 0); //ksg : view 화면 debug시 사용
                    m_fmView.Show();
                    if (CData.GemForm != null) CData.GemForm.Log_wt("CData.Opt.bSecsUse = true");
                }

                // 2021-06-08, jhLee : Multi-LOT 입력/관리 Form
                if (CDataOption.UseMultiLOT)
                {
                    CData.dlgLotInput = new CDlgLotInput();             // LOT 정보 관리하는 Dialog form을 생성한다.
                }

                m_fmMst = new frmMst();
                m_fmMst.Show();

                ////210830 syc : 2004U IV2 -  IV2 연결
                if (CDataOption.Use2004U == true)
                {
                    CSetup.It.Load(); //데이터 로드

                    CSq_OnP.It.initIV2Ctrl(CData.sIV2OnpIP, CData.iIV2OnpPort);
                    CSq_OnP.It.ConnectIV2();
                    //연결

                    CSq_OfP.It.initIV2Ctrl(CData.sIV2OfpIP, CData.iIV2OfpPort);
                    CSq_OfP.It.ConnectIV2();
                    //연결
                }

                if (CDataOption.UseCardReader)
                {
                    string sPath = GV.PATH_USER;

                    if (Directory.Exists(sPath))
                    {
                        sPath += "User.ini";
                        if (File.Exists(sPath))
                        {
                            CCR.It.GetCount(sPath);

                            CCR.It.Load(sPath, CCR.It.iUserCnt);
                        }
                    }
                    lblOpId.Visible = true;
                    lblOpId.Text = "";
                }
            }
        }

        private void _Cycle()
        {
            CLog.mLogSeq("_Cycle() Start");
            while (true)
            {
                if (CData.WMX)
                {
                    CWmx.It.Get_Stat();
                    CIO.It.Get_IO();
                }
                //GV.L_GRD.Cycle();
                CSQ_Main.It.ChkMotorAlram   (); //190714 ksg : Motor Alram 추가
                CSQ_Main.It.InspectEmergency();
                CSQ_Man .It.Cycle           ();
                CSQ_Main.It.AutoRun         ();
                CSQ_Main.It.EquipStatus     ();
                /*
                if (CData.Opt.bQcUse && CTcpIp.It.bIsConnect && CTcpIp.It.IsConnect())
                {
                    //Cycle에서는 메세시 수신처리만 20200612 lks
                    CTcpIp.It.ReciveNewMsg();

                    //CTcpIp.It.ProcessQueueMsg();
                }
                */
                CTenKey.It.Update(); // 190415 ksg :

                //20200311 jhc : UDP Interface for [ Main <<-->> Vision ]
                if(CDataOption.BcrInterface == eBcrInterface.Udp)
                {
                    CBcr.It.UdpMonitor2DVision(CBcr.It.iTimeout); //2D Vision SW가 실행 중인지?, 주기 Reporting을 잘 하고 있는지 체크
                }
                //

                Thread.Sleep(1);
            }
            CLog.mLogSeq("_Cycle() End");
        }

        private void _SdoComm()
        {
            if (CDataOption.SplType == eSpindleType.EtherCat)
            {
                int nLSplCurrent = 0;
                int nRSplCurrent = 0;

                while (true)
                {
                    CData.Spls[0].dLoad = CSpl.It.Get_Load(EWay.L);
                    CData.Spls[1].dLoad = CSpl.It.Get_Load(EWay.R);
                    

                    CData.Spls[0].dTemp_Val = CSpl.It.Get_Spindle_Temp(EWay.L); //201127 : 온도 로그 수집용
                    CData.Spls[1].dTemp_Val = CSpl.It.Get_Spindle_Temp(EWay.R); //201127 : 온도 로그 수집용
                    
                    nLSplCurrent = CSpl.It.Get_Spindle_Current(EWay.L);
                    nRSplCurrent = CSpl.It.Get_Spindle_Current(EWay.R);
                    
                    if ((0 <= nLSplCurrent) && (nLSplCurrent <= GV.SPINDLE_CURRENT_MAX)) //Garbage Data 제거를 위해 범위 제한
                    {
                        CData.Spls[0].nCurrent_Amp = nLSplCurrent;  // EtherCat
                    }
                    if ((0 <= nRSplCurrent) && (nRSplCurrent <= GV.SPINDLE_CURRENT_MAX)) //Garbage Data 제거를 위해 범위 제한
                    {
                        CData.Spls[1].nCurrent_Amp = nRSplCurrent;  // EtherCat
                    }

                    CData.Parts[(int)EPart.GRDL].dChuck_Vacuum = CIO.It.Get_Chuck_Table_Vaccum(EWay.L); //201217 : Table Vacuum 값 로그 수집용
                    CData.Parts[(int)EPart.GRDR].dChuck_Vacuum = CIO.It.Get_Chuck_Table_Vaccum(EWay.R); //201217 : Table Vacuum 값 로그 수집용

                    Thread.Sleep(200);//220321 pjh : ASE KH 장비 구동 중 Spindle Load 기록되지 않는 Issue로 인해 Thread Sleep 추가 후 Test 예정
                }
            }
            // 2023.03.15 Max
            else // RS485 RPS Spindle 추가로 else 구문 추가
            {
                int nLSplCurrent = 0;
                int nRSplCurrent = 0;

                while (true)
                {
                    CData.Spls[0].dLoad = CSpl_485.It.Get_Load(EWay.L);
                    CData.Spls[1].dLoad = CSpl_485.It.Get_Load(EWay.R);


                    CData.Spls[0].dTemp_Val = CSpl_485.It.Get_Spindle_Temp(EWay.L);
                    CData.Spls[1].dTemp_Val = CSpl_485.It.Get_Spindle_Temp(EWay.R);

                    nLSplCurrent = CSpl_485.It.Get_Spindle_Current(EWay.L);
                    nRSplCurrent = CSpl_485.It.Get_Spindle_Current(EWay.R);

                    /*
                    if ((0 <= nLSplCurrent) && (nLSplCurrent <= GV.SPINDLE_CURRENT_MAX)) //Garbage Data 제거를 위해 범위 제한
                    {
                        CData.Spls[0].nCurrent_Amp = nLSplCurrent;  // EtherCat
                    }
                    if ((0 <= nRSplCurrent) && (nRSplCurrent <= GV.SPINDLE_CURRENT_MAX)) //Garbage Data 제거를 위해 범위 제한
                    {
                        CData.Spls[1].nCurrent_Amp = nRSplCurrent;  // EtherCat
                    }
                    */

                    CData.Parts[(int)EPart.GRDL].dChuck_Vacuum = CIO.It.Get_Chuck_Table_Vaccum(EWay.L); //201217 : Table Vacuum 값 로그 수집용
                    CData.Parts[(int)EPart.GRDR].dChuck_Vacuum = CIO.It.Get_Chuck_Table_Vaccum(EWay.R); //201217 : Table Vacuum 값 로그 수집용

                    Thread.Sleep(200);//220321 pjh : ASE KH 장비 구동 중 Spindle Load 기록되지 않는 Issue로 인해 Thread Sleep 추가 후 Test 예정
                }
            }
        }

        private void _TcpIpQueuePoress()
        {
            CLog.mLogSeq("_TcpIpQueuePoress() Start");
            while (true)
            {
                //_TcpIpQueuePoress에서는 수신 된 메세지 처리만
                //if (CData.Opt.bQcUse && CTcpIp.It.bIsConnect && CTcpIp.It.IsConnect())
                if (CData.Opt.bQcUse && CGxSocket.It.IsConnected())
                {
                    //CTcpIp.It.ReciveNewMsg();

                    //CTcpIp.It.ProcessQueueMsg();
                }

                Thread.Sleep(1);
            }
            CLog.mLogSeq("_TcpIpQueuePoress() End");
        }

        /// <summary>
        /// 190813-maeng
        /// 큐에서 로그 빼서 저장
        /// </summary>
        private void _Log()
        {
            string sPath = GV.PATH_LOG;
            string sLogPath = string.Empty;
            DateTime ChkDay = DateTime.Now;
            int iYear = 0;
            int iMonth = 0;
            int iDay = 0;

            while (true)
            {
                tMainLog tVal;
                //if (CData.QueLog.TryDequeue(out tVal))
                //{
                //    CLog.Save(tVal); // 190802-maeng : 로그 데이터 파일로 저장
                //}
                // 200720 jym : 로그 실시간 저장에서 1초 단위로 변경
                int iCnt = CData.QueLog.Count;
                if (iCnt > 0)
                {
                    for (int i = 0; i < iCnt; i++)
                    {
                        CData.QueLog.TryDequeue(out tVal);
                        CLog.Save(tVal);
                    }
                }

                DateTime dtDelDate = DateTime.Today.AddDays(-CData.Opt.iDelPeriod);

                //201005 pjh : Log Delete Period
                if (CData.Opt.iDelPeriod != 0 && ChkDay.Day != DateTime.Now.Day)
                {
                    for (iYear = dtDelDate.Year; iYear > dtDelDate.Year - 3; iYear--)
                    {
                        if (iYear != dtDelDate.Year)
                        {
                            sLogPath = sPath + iYear.ToString();

                            DirectoryInfo diY = new DirectoryInfo(sLogPath);

                            if (diY.Exists)
                            {
                                diY.Delete(true);
                            }
                        }
                    }

                    for (iMonth = dtDelDate.Month; iMonth > 0; iMonth--)
                    {
                        if (iMonth != dtDelDate.Month)
                        {
                            sLogPath = sPath + dtDelDate.Year.ToString("0000") + "\\" + iMonth.ToString("00");

                            DirectoryInfo diM = new DirectoryInfo(sLogPath);

                            if (diM.Exists)
                            {
                                diM.Delete(true);
                            }
                        }
                    }
                    for (iDay = dtDelDate.Day; iDay > 0; iDay--)
                    {
                        sLogPath = sPath + dtDelDate.Year.ToString("0000") + "\\" + dtDelDate.Month.ToString("00") + "\\" + iDay.ToString("00");

                        DirectoryInfo di = new DirectoryInfo(sLogPath);

                        if (di.Exists)
                        {
                            di.Delete(true);
                        }
                    }
                    ChkDay = DateTime.Now;
                }
                //

                Thread.Sleep(1000);
            }
        }

        // 210601 jym : 네트워크 스레드 추가
        /// <summary>
        /// 네트워크 드라이브 저장 스레드
        /// </summary>
        private void _NetDrive()
        {
            while (true)
            {
                int cnt = CData.QueNet.Count;
                string root = @"\\" + CData.Opt.sNetIP;

                if (cnt > 0)
                {                   
                    // 네트워크 접속
                    int ret = CNetDrive.Connect(root, CData.Opt.sNetID, CData.Opt.sNetPw);
                    if (ret == (int)ENetError.NO_ERROR)
                    {
                        TNetDrive temp = new TNetDrive();
                        CData.QueNet.TryDequeue(out temp);
                        DirectoryInfo info = new DirectoryInfo(temp.path);
                        // 폴더 경로가 없으면 생성
                        if (!Directory.Exists(info.Parent.FullName))
                        {
                            Directory.CreateDirectory(info.Parent.FullName);
                        }
                        // 파일 저장
                        CLog.Check_File_Access(temp.path, temp.data, true);
                    }
                    // 네트워크 종료
                    CNetDrive.Disconnect();//210813 pjh : Disconnect Timing 변경
                }

                Thread.Sleep(1000);
            }
        }

        //211202 pjh : DF Net Drive Connect Check
        /// <summary>
        /// Net Drive 연결 상태 Check 함수
        /// </summary>
        private void _NetDriveCheck()
        {
            while (true)
            {
                string root = @"\\" + CData.Opt.sNetIP;

                // 네트워크 접속
                int ret = CNetDrive.Connect(root, CData.Opt.sNetID, CData.Opt.sNetPw);

                if (ret == (int)ENetError.NO_ERROR) { CData.m_bNetCheck = true; }
                else                                { CData.m_bNetCheck = false; }

                // 2022.07.11 SungTae Start : [추가] (ASE-KH VOC) 2006X 설비 D/F Server Issue 관련 Connection 상태 확인용 Log 추가
                string sMsg = _ChkNetDriveConnectionState(ret);

                _SetLog($">> Return Value : {ret}({sMsg}), Connection Check State : {CData.m_bNetCheck}");
                // 2022.07.11 SungTae End

                // 네트워크 종료
                CNetDrive.Disconnect();

                Thread.Sleep(2000);
            }
        }
        //

        private void _CardChangeLevel()
        {
            if (CCR.It.bCRRead && !CCR.It.bUserForm)
            {
                string sID = CCR.It.ReciveMsg;
                string sGetID = "";

                frmLvl mform = new frmLvl();

                if (CCR.It.dicUser.ContainsKey(sID))
                {
                    CCR.It.dicUser.TryGetValue(sID, out sGetID);
                }
                else
                {
                    CCR.It.bCRRead = false;
                    return;
                }

                if (sGetID == "Operator")
                {
                    mform.RetLv = ELv.Operator;
                    CData.Lev = ELv.Operator;

                    lbl_Level.BackColor = Color.FromArgb(41, 199, 201);
                    lbl_Level.Image     = Properties.Resources.Operator64;
                    lbl_Level.Text      = CData.Lev.ToString().ToUpper();

                    lblOpId.BackColor   = Color.FromArgb(41, 199, 201);
                    lbl_Ver.BackColor   = Color.FromArgb(41, 199, 201);
                }
                else if(sGetID == "Technician")
                {
                    mform.RetLv = ELv.Technician;
                    CData.Lev = ELv.Technician;

                    lbl_Level.BackColor = Color.Orange;
                    lbl_Level.Image     = Properties.Resources.Engineer64;
                    lbl_Level.Text      = CData.Lev.ToString().ToUpper();

                    lblOpId.BackColor   = Color.Orange;
                    lbl_Ver.BackColor   = Color.Orange;
                }
                else if(sGetID == "Engineer")
                {
                    mform.RetLv = ELv.Engineer;
                    CData.Lev = ELv.Engineer;

                    lbl_Level.BackColor = Color.FromArgb(210, 56, 80);
                    lbl_Level.Image     = Properties.Resources.Manager64;
                    lbl_Level.Text      = CData.Lev.ToString().ToUpper();

                    lblOpId.BackColor   = Color.FromArgb(210, 56, 80);
                    lbl_Ver.BackColor   = Color.FromArgb(210, 56, 80);
                }

                lblOpId.Text = sID;

                CData.bInitPage = true;
                CBcr.It.SaveStatus(false);
                CCR.It.bCRRead = false;
                HideMenu(CData.Lev);
            }
        }

        // 2021.04.09 SungTae Start : Wheel History Auto Delete 함수 추가
        /// <summary>
        /// Wheel History Auto Delete
        /// </summary>
        private void _AutoDeleteWheelHistory(EWay eWy)
        {
            string sPath = GV.PATH_WHEEL;
            string sFile = "";

            if (eWy == EWay.L)  sPath += "Left\\";
            else                sPath += "Right\\";

            string[] dirs = Directory.GetDirectories(sPath);

            DateTime dtDelDate = DateTime.Today.AddDays(-CData.Opt.iDelPeriodHistory);

            foreach (string sfold in dirs)
            {
                sPath = sfold + "\\WheelHistory\\";

                DirectoryInfo mFile = new DirectoryInfo(sPath);

                if (!Directory.Exists(sPath))   { Directory.CreateDirectory(sPath); }

                foreach (FileSystemInfo mInfo in mFile.GetFileSystemInfos("*.csv"))
                {
                    sFile = sPath + mInfo.Name;

                    if (File.Exists(sFile))
                    {
                        if (File.GetLastWriteTime(sFile) < dtDelDate)
                            File.Delete(sFile);
                        else
                            break;
                    }
                    else
                        break;
                }
            }
        }
        // 2021.04.09 SungTae End

        private void _Init_Screen()
        {
            // 200323 mjy : Error화면 유저컨트롤로 변경
            this.Controls.Add(m_vwErr);
            m_vwErr.Visible = false;
            m_vwErr.Location = new Point(0, 0);

            // 200323 mjy : 휠 지그 화면 유저컨트롤로 변경
            this.Controls.Add(m_vwZig);
            m_vwZig.Visible = false;
            m_vwZig.Location = new Point(0, 0);

            // 200327 mjy : Dresser 교체 화면 추가
            this.Controls.Add(m_vwDrs);
            m_vwDrs.Visible = false;
            m_vwDrs.Location = new Point(0, 0);

            // 200803 jym : Wheel 선택 화면 추가
            this.Controls.Add(m_vwSelWhl);
            m_vwSelWhl.Visible = false;
            m_vwSelWhl.Location = new Point(0, 0);

            // 200827 jym : Wheel 선택 화면 추가
            this.Controls.Add(m_vwSelMsg);
            m_vwSelMsg.Visible = false;
            m_vwSelMsg.Location = new Point(0, 0);

            this.Controls.Add(m_vwSelKey);
            m_vwSelKey.Visible = false;
            m_vwSelKey.Location = new Point(0, 0);

            if (CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)  { m_vwAuto  = new vwAuto();     }   //201112 jhc : 조건 추가
            else                                                    { m_vwAuto2 = new vwAuto2();    }   // 2020.09.08 SungTae

            CSph.Update("View Screen Initialize - Auto");
            m_vwManl = new vwMan   ();
            CSph.Update("View Screen Initialize - Manual");
            m_vwRcp  = new vwDev   ();
            CSph.Update("View Screen Initialize - Device");
            m_vwWhl  = new vwWhl   ();
            CSph.Update("View Screen Initialize - Wheel");
            m_vwEq   = new vwEqu   ();
            CSph.Update("View Screen Initialize - Equipment");
            m_vwSPC  = new vwSPC   ();
            CSph.Update("View Screen Initialize - SPC");
            m_vwOpt  = new vwOpt   ();
            CSph.Update("View Screen Initialize - Option");
            m_vwShw  = new vwMst   ();
            m_vwOpMn = new vwOpManu();
            CSph.Update("View Screen Initialize - OP Manu");

            m_iPage = 0;

            // 2020.09.07 SungTae
            if (CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)
            {
                m_vwAuto.Open();
                HideMenu(CData.Lev);
                pnlMM_Base.Controls.Add(m_vwAuto);
            }
            else
            {
                m_vwAuto2.Open();
                HideMenu(CData.Lev);
                pnlMM_Base.Controls.Add(m_vwAuto2);
            }

            //211202 pjh : Networks Connection Check Display
            if (CDataOption.UseDFNet) lbl_ChkNetConnect.Visible = true;
            else                      lbl_ChkNetConnect.Visible = false;
            //
        }

        public void addMsg(string sMsg)
        {
            m_vwManl.m_vw13QcVision.addMsgList(sMsg);
        }
        private void _Init_Config()
        {
            int iRet = 0;

            if (!Directory.Exists(GV.PATH_EQ_MOTION))
            { Directory.CreateDirectory(GV.PATH_EQ_MOTION); }

            // Last variable load
            CLast.Load();

            // Main setup position load
            iRet = CSetup.It.Load();
            if (iRet != 0)
            {
                CSetup.It.Save();
            }
            CTower.It.Load();
            // 옵션 파일 불러오기
            COpt.It.Load();
            //if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK || CData.CurCompany == ECompany.ASE_K12)
            {
                CMnt.It.Load();     //2021.07.15 lhs : Maintenance 파일 불러오기
            }

            // 200317 mjy : 화면 데이터 구성시 배열 초기화 문제로 해당위치로 옮김
            CData.L_GRD.m_aInspBf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol];
            CData.L_GRD.m_aInspAf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol];
            CData.L_GRD.m_aInspTemp = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol];
            CData.L_GRD.m_abMeaBfErr = new bool[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol];
            CData.L_GRD.m_abMeaAfErr = new bool[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol];
            CData.GrData[0].aMeaBf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol];
            CData.GrData[0].aMeaAf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol];

            CData.R_GRD.m_aInspBf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol];
            CData.R_GRD.m_aInspAf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol];
            CData.R_GRD.m_aInspTemp = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol];
            CData.R_GRD.m_abMeaBfErr = new bool[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol];
            CData.R_GRD.m_abMeaAfErr = new bool[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol];
            CData.GrData[1].aMeaBf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol];
            CData.GrData[1].aMeaAf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol];

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
        }

        private void _Init_Equip()
        {
            bool bRet = false;
            int iRet = 0;

            // IO intialize
            CIO.It.Init();
            GU.Delay(200);

            if (CData.WMX)
            {
                for (int i = 1; i <= 5; i++)
                {
                    if (CMot.It.WmxEvt() == 0)
                    {
                        bRet = true;
                        break;
                    }
                    else
                    {
                        CWmx.It.WLib.eventControl.ClearAllEvent();
                    }
                    GU.Delay(100);
                }

                // Retry 5번 반복 후에도 이벤트 등록 실패 시
                if (!bRet)
                {
                    // 에러후 프로그램 종료 필요
                }

                // Room lamp on
                CIO.It.Set_Y(eY.Y0C, true);
                GU.Delay(100);

                // Main MC on
                CIO.It.Set_Y(eY.SYS_MC, true);
                GU.Delay(100);

                // Z axis MC on
                CIO.It.Set_Y(eY.SYS_ZaxisMC, true);
                GU.Delay(100);

                // Left inverter MC on
                CIO.It.Set_Y(eY.GRDL_SplInverterMC, true);
                GU.Delay(100);

                CIO.It.Set_Y(eY.GRDL_SplPCW, true);
                GU.Delay(100);
                CIO.It.Set_Y(eY.GRDL_SplCDA, true);
                GU.Delay(100);

                // Right inverter MC on
                CIO.It.Set_Y(eY.GRDR_SplInverterMC, true);
                GU.Delay(100);

                CIO.It.Set_Y(eY.GRDR_SplPCW, true);
                GU.Delay(100);
                CIO.It.Set_Y(eY.GRDR_SplCDA, true);
                GU.Delay(100);

                if( (CData.CurCompany == ECompany.SkyWorks) ||
                    (CData.CurCompany == ECompany.Qorvo)    ||
                    (CData.CurCompany == ECompany.Qorvo_DZ) ||
                    (CData.CurCompany == ECompany.Qorvo_RT) ||   // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                    (CData.CurCompany == ECompany.Qorvo_NC) ||   // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                    (CData.CurCompany == ECompany.ASE_K12)  ||
                    (CData.CurCompany == ECompany.SST)      ||
                    (CData.CurCompany == ECompany.USI))
                {
                    CIO.It.Set_Y(eY.INR_OcrAir, true);
                }
            }

            // Probe initialize
            CPrb.It.Initial();
            CSph.Update("Equipment Initialize - Probe");

            //20191108 ghk_light
            if (CDataOption.LightControl == eLightControl.Use)
            {
                CLight.It.Initial();
                iRet = CLight.It.Open_Port();
                CSph.Update("Equipment Initialize - Light controller");
            }

            GU.Delay(200);
            if (!CData.VIRTUAL && CData.WMX)
            {
                iRet = CPrb.It.Open_Port(EWay.L);
                if (iRet != 0)
                {
                    CErr.Show(eErr.LEFT_GIRND_PROBE_RS232_PORT_OPEN_ERROR);
                }
                GU.Delay(100);

                iRet = CPrb.It.Open_Port(EWay.R);
                if (iRet != 0)
                {
                    CErr.Show(eErr.RIGHT_GIRND_PROBE_RS232_PORT_OPEN_ERROR);
                }

                /*
                iRet = CPrb.It.Open_Port(EWay.INR);
                if(iRet != 0)
                {
                    CErr.Show(eErr.INRAIL_PROBE_RS232_PORT_OPEN_ERROR);
                }
                */

                if (CData.CurCompany == ECompany.Qorvo    || CData.CurCompany == ECompany.Qorvo_DZ ||
                    CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC ||        // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                    CData.CurCompany == ECompany.SkyWorks || 
                    CData.CurCompany == ECompany.ASE_K12  || CData.CurCompany == ECompany.ASE_KR   ||
                    CData.CurCompany == ECompany.SST      || CData.CurCompany == ECompany.USI      ||
                    CData.CurCompany == ECompany.SCK      || CData.CurCompany == ECompany.JSCK     || CData.CurCompany == ECompany.JCET) //200206 ksg :D/F 사용 업체 추가로 수정 // 200410 pjh : D/F 사용 업체 추가, 200625 lks
                {
                    iRet = CPrb.It.Open_Port(EWay.INR);
                    if(iRet != 0)
                    {
                        CErr.Show(eErr.INRAIL_PROBE_RS232_PORT_OPEN_ERROR);
                    }
                }
            }

            // Spindle initialize
            //CSpl.It.Initial();
            
            // 2023.03.15 Max
            CSpl_485.It._Initial();    
            
            iRet = CSpl_485.It.InitialComm_Spl1();
            if (iRet != 0)
            {
                CErr.Show(eErr.INRAIL_PROBE_RS232_PORT_OPEN_ERROR);
            }
          
            iRet = CSpl_485.It.InitialComm_Spl2();
            if (iRet != 0)
            {
                CErr.Show(eErr.INRAIL_PROBE_RS232_PORT_OPEN_ERROR);
            }

            CSph.Update("Equipment Initialize - Spindle");
            GU.Delay(200);

            if (CDataOption.RFID == ERfidType.RFID && (CDataOption.OnlRfid == eOnlRFID.Use || CDataOption.OflRfid == eOflRFID.Use))
            {
                CRfid.It.Initial();
                CSph.Update("Equipment Initialize - RFID");
                GU.Delay(200);
            }

            //220325 pjh : ASE Card Reader initialize
            if(CData.CurCompany == ECompany.ASE_K12 && CDataOption.UseCardReader)
            {
                CCR.It.Initial();
                CSph.Update("Equipment Initialize - Card Reader");
                GU.Delay(200);
            }
            //

            // Motion initialize
            CMot.It.Load();
            CSph.Update("Equipment Initialize - Motion");
            GU.Delay(200);
            if (!CData.VIRTUAL && CData.WMX)
            {

            }

            CDf.It.Initial();
            CSph.Update("Equipment Initialize - DF");
        }

        private void _Init_Errlist()
        {
            CErr.Load();
            // 2020.12.07 JSKim St
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            {
                CErr.LoadRadar();
            }
            // 2020.12.07 JSKim Ed
            GU.Delay(200);
        }

        #region frmMain Button
        private void btnMenu_Click(object sender, EventArgs e)
        {
            Button mBtn = sender as Button;
            int iTag = int.Parse(mBtn.Tag.ToString());

            if (iTag == m_iPage)
            { return; }    // 현재 뷰와 새로운 뷰가 같은 경우 리턴
            else
            {
                switch(m_iPage)    // 이전 뷰 Close
                {
                    case 0:
                        // 2020.09.07 SungTae
                        if (CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)
                        { m_vwAuto.Close(); }
                        else
                        { m_vwAuto2.Close(); }
                        break;
                    case 1:
                        m_vwManl.Close();
                        break;
                    case 2:
                        m_vwRcp.Close();
                        break;
                    case 3:
                        m_vwWhl.Close();
                        break;
                    case 4:
                        m_vwEq.Close();
                        break;
                    case 5:
                        m_vwOpt.Close();
                        break;
                    case 6:
                        m_vwSPC.Close();
                        break;
                    case 7:
                        m_vwOpMn.Close();
                        break;
                    case 10:
                        m_vwShw.Close();
                        break;
                }

                pnlMM_Base.Controls.Clear();    // Panel 에서 이전 뷰 삭제
                m_iPage = iTag;    // 인덱스 변경

                switch (m_iPage)    // 신규 뷰 Open 및 표시
                {
                    case 0:
                        // 2020.09.07 SungTae
                        if (CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)
                        {
                            pnlMM_Base.Controls.Add(m_vwAuto);
                            m_vwAuto.Open();
                        }
                        else
                        {
                            pnlMM_Base.Controls.Add(m_vwAuto2);
                            m_vwAuto2.Open();
                        }
                        break;
                    case 1:
                        pnlMM_Base.Controls.Add(m_vwManl);
                        m_vwManl.Open();
                        break;
                    case 2:
                        pnlMM_Base.Controls.Add(m_vwRcp);
                        m_vwRcp.Open();
                        break;
                    case 3:
                        pnlMM_Base.Controls.Add(m_vwWhl);
                        m_vwWhl.Open();
                        break;
                    case 4:
                        pnlMM_Base.Controls.Add(m_vwEq);
                        m_vwEq.Open();
                        break;
                    case 5:
                        pnlMM_Base.Controls.Add(m_vwOpt);
                        m_vwOpt.Open();
                        break;
                    case 6:
                        pnlMM_Base.Controls.Add(m_vwSPC);
                        m_vwSPC.Open();
                        break;
                    case 7:
                        pnlMM_Base.Controls.Add(m_vwOpMn);
                        m_vwOpMn.Open();
                        break;
                    case 10:
                        pnlMM_Base.Controls.Add(m_vwShw);
                        m_vwShw.Open();
                        break;
                }
            }
        }

        private void btnM_Exit_Click(object sender, EventArgs e)
        {
            //if(CData.Lev < eARL.Engineer)
            //{
            //    CMsg.Show(eMsg.Warning, "Warning", "Upper select Engineer");
            //    return;
            //}

            if ((int)CData.Lev < CData.Opt.iLvExit)
            {
                CMsg.Show(eMsg.Warning, "Warning", "Upper select Level");
                return;
            }
            //190130 ksg : 확인 창 추가
            //if (CMsg.Show(eMsg.Notice, "Notice", "Do you want shut Down?") == DialogResult.Cancel) { return; }
            if (CMsg.Show(eMsg.Warning, "Warning", "Do you want shut Down?") == DialogResult.Cancel) { return; }

            if (CData.Opt.bSecsUse)// 191125 ksg :
            {
                if (CData.GemForm != null)
                {
                    //CData.GemForm.LastSave();
                    CData.GemForm.Stop_GEM();
                    CData.GemForm.Close();
                }
            }

            // Last variable save
            CLast.Save();

            // IO release
            CIO.It.Rele();

            // Equip probe release
            CPrb.It.Release();

            //20191112 ghk_spccopy
            m_vwSPC.Close();

            //20191108 ghk_light
            if (CDataOption.LightControl == eLightControl.Use)
            {
                CLight.It.Release();
            }
            //

            // Equip spindle release
            //CSpl.It.Release();

            // 2023.03.15 Max
            CSpl_485.It.Release(EWay.L);
            CSpl_485.It.Release(EWay.R);

            //190418 ksg :
            CRfid.It.Release();

            //220325 pjh : Card reader release
            CCR.It.Release();

            CBcr.It.SaveStatus(true);

            //            m_thCyl.Interrupt();
            m_thCyl.Abort();

            // 210601 jym : 네트워크 스레드 추가
            //if (CData.CurCompany == ECompany.Qorvo    || CData.CurCompany == ECompany.Qorvo_DZ ||
            //    CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC ||
            //    CData.CurCompany == ECompany.JCET)
            if(CDataOption.UseNetDrive || CDataOption.UseDFNet)//210817 pjh : Net drive License로 관리
            {
                m_thNet.Abort();
                m_thNetCheck.Abort();//211202 pjh : Network Connection Check Thread
            }

            m_thLog.Abort();
            //m_thQc.Abort();
            m_thSpl.Abort();

            m_tmUpdt.Stop();
            m_tmUpdt.Dispose();
            m_tmView.Stop();
            m_tmView.Dispose();

            pnlMM_Base.Controls.Clear();

            if (m_vwManl != null)
            {
                m_vwManl.Release();
                m_vwManl.Dispose();
            }

            // 2020.09.07 SungTae
            if (CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)
            {
                if (m_vwAuto != null)
                {
                    m_vwAuto.Release();
                    m_vwAuto.Dispose();
                }
            }
            else
            {
                if (m_vwAuto2 != null)
                {
                    m_vwAuto2.Release();
                    m_vwAuto2.Dispose();
                }
            }

            //if (CData.CurCompany != eCompany.JSCK)
            if (CDataOption.SecsUse != eSecsGem.NotUse)
            {
                if (m_fmView_Ori != null)
                {
                    m_fmView_Ori.Release();
                    m_fmView_Ori.Close();
                    m_fmView_Ori.Dispose();
                }
            }
            else
            {
                if (m_fmView != null)
                {
                    m_fmView.Release();
                    m_fmView.Close();
                    m_fmView.Dispose();
                }
            }
            //190403 ksg :
            // 2020.10.15 JSKim St
            //if (CTcpIp.It.bIsConnect==true) CTcpIp.It.TcpIpClose();//koo : disconnect시 Close()으로 인해 QC 설비 오류 발생.
            //if (CTcpIp.It.bIsConnect == true)
            if(CGxSocket.It.IsConnected())
            {
                //if (CTcpIp.It.IsConnect())
                //{
                //  CTcpIp.It.TcpIpClose();
                //}
                //CTcpIp.It.bIsConnect = false;
                CGxSocket.It.CloseClient();
            }
            // 2020.10.15 JSKim Ed
            //CTcpIp.It.TcpIpClose();//koo : disconnect시 Close()으로 인해 QC 설비 오류 발생.

            //20200311 jhc :
            if (CDataOption.BcrInterface == eBcrInterface.Udp)
            { CBcr.It.UdpClose(); } //UDP Server Close
            //

            //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)
            //201012 jhc : (KEYENCE BCR)/(RFID) 옵션으로 구분 적용
            if ((CDataOption.RFID == ERfidType.Keyence) && (CDataOption.OnlRfid == eOnlRFID.Use || CDataOption.OflRfid == eOflRFID.Use))
            //if ((CData.CurCompany == ECompany.SPIL) && (CDataOption.OnlRfid == eOnlRFID.Use || CDataOption.OflRfid == eOflRFID.Use))
            {
                CKeyenceBcr.It.FinalizeKeyenceBcr();
            }
            //

            if (CData.WMX)
            {
                // WMX stop communication & close device
                int iRet = CWmx.It.Close();
                if (iRet != 0)
                {
                    //2019227 ghk_err
                    CErr.Show(eErr.SYSTEM_WMX_CLOSE_ERROR);

                    return;
                }

                CData.WMX = false;

            }

            // 2020.10.15 JSKim St
            Application.Exit();
            // 2020.10.15 JSKim Ed
            this.Close();
        }
        #endregion

        private void rdb_Menu_CheckedChanged(object sender, EventArgs e)
        {
            CData.bLastClick = true; //190509 ksg 
            RadioButton mBtn = sender as RadioButton;
            int iTag = int.Parse(mBtn.Tag.ToString());

            if (iTag == m_iPage)
            { return; }    // 현재 뷰와 새로운 뷰가 같은 경우 리턴
            else
            {
                CData.bDevOnlyView = false;     // 2020.08.24 JSKim

                switch (m_iPage)    // 이전 뷰 Close
                {
                    case 0:
                        // 2020.09.07 SungTae
                        if (CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)
                        { m_vwAuto.Close(); }
                        else
                        { m_vwAuto2.Close(); }
                        break;
                    case 1:
                        m_vwManl.Close();
                        break;
                    case 2:
                        m_vwRcp.Close();
                        break;
                    case 3:
                        m_vwWhl.Close();
                        break;
                    case 4:
                        m_vwSPC.Close();
                        break;
                    case 5:
                        m_vwOpt.Close();
                        break;
                    case 6:
                        m_vwEq.Close();
                        break;
                    case 7: //190310 ksg :
                        m_vwOpMn.Close(); 
                        break;
                    case 10:
                        m_vwShw.Close();
                        break;
                }

                pnlMM_Base.Controls.Clear();    // Panel 에서 이전 뷰 삭제
                m_iPage = iTag;    // 인덱스 변경

                switch (m_iPage)    // 신규 뷰 Open 및 표시
                {
                    case 0:
                        if (CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)
                        {
                            pnlMM_Base.Controls.Add(m_vwAuto);
                            m_vwAuto.Open();
                        }
                        else
                        {
                            pnlMM_Base.Controls.Add(m_vwAuto2);
                            m_vwAuto2.Open();
                        }

                        HideMenu(CData.Lev);
                        break;
                    case 1:
                        pnlMM_Base.Controls.Add(m_vwManl);
                        m_vwManl.Open();
                        CData.bLastClick = true; //190509 ksg 
                        break;
                    case 2:
                        pnlMM_Base.Controls.Add(m_vwRcp);
                        m_vwRcp.Open();
                        CData.bLastClick = true; //190509 ksg 
                        break;
                    case 3:
                        pnlMM_Base.Controls.Add(m_vwWhl);
                        m_vwWhl.Open();
                        CData.bLastClick = true; //190509 ksg 
                        break;
                    case 4:
                        pnlMM_Base.Controls.Add(m_vwSPC);
                        m_vwSPC.Open();
                        CData.bLastClick = true; //190509 ksg 
                        break;
                    case 5:
                        pnlMM_Base.Controls.Add(m_vwOpt);
                        m_vwOpt.Open();
                        CData.bLastClick = true; //190509 ksg 
                        break;
                    case 6:
                        pnlMM_Base.Controls.Add(m_vwEq);
                        m_vwEq.Open();
                        CData.bLastClick = true; //190509 ksg 
                        break;
                    case 7: //190310 ksg :
                        pnlMM_Base.Controls.Add(m_vwOpMn); 
                        m_vwOpMn.Open();
                        CData.bLastClick = true; //190509 ksg 
                        break;
                    case 10:
                        pnlMM_Base.Controls.Add(m_vwShw);
                        m_vwShw.Open();
                        CData.bLastClick = true; //190509 ksg 
                        break;
                }
            }
        }

        private void lbl_Level_Click(object sender, EventArgs e)
        {
            if (CSQ_Main.It.m_iStat != EStatus.Auto_Running || 
                CSQ_Main.It.m_iStat != EStatus.Manual)
            {
                CData.bLastClick = true; //190509 ksg :
                FileInfo fI;
                CIni m_Ini;

                string sPath = GV.PATH_CONFIG + "PW.cfg";
                string sSec = "";
                string sKey = "";
                string sData = "";

                fI = new FileInfo(sPath);
                m_Ini = new CIni(sPath);
                if(!fI.Exists)
                {
                    fI.Create().Close();
                    sSec = "PW";
                    sKey = "TECHNICIAN";
                    m_Ini.Write(sSec, sKey, sData);
                    sKey = "ENGINEER";
                    m_Ini.Write(sSec, sKey, sData);
                }

                frmLvl mForm = new frmLvl();
                mForm.Location = new Point(MousePosition.X, MousePosition.Y);
                if (mForm.ShowDialog() == DialogResult.OK)
                {
                    CData.Lev = mForm.RetLv;
                    if (CData.Lev == ELv.Operator)
                    {
                        lbl_Level.BackColor = Color.FromArgb(41, 199, 201);
                        lbl_Level.Image     = Properties.Resources.Operator64;
                        lbl_Level.Text      = CData.Lev.ToString().ToUpper();

                        lblOpId.BackColor   = Color.FromArgb(41, 199, 201);
                        lbl_Ver.BackColor   = Color.FromArgb(41, 199, 201);
                    }
                    else if (CData.Lev == ELv.Technician)
                    {
                        lbl_Level.BackColor = Color.Orange;
                        lbl_Level.Image     = Properties.Resources.Engineer64;
                        lbl_Level.Text      = CData.Lev.ToString().ToUpper();

                        lblOpId.BackColor   = Color.Orange;
                        lbl_Ver.BackColor   = Color.Orange;
                    }
                    else if (CData.Lev == ELv.Engineer || CData.Lev == ELv.Master)
                    {
                        lbl_Level.BackColor = Color.FromArgb(210, 56, 80);
                        lbl_Level.Image     = Properties.Resources.Manager64;
                        lbl_Level.Text      = CData.Lev.ToString().ToUpper();

                        lblOpId.BackColor   = Color.FromArgb(210, 56, 80);
                        lbl_Ver.BackColor   = Color.FromArgb(210, 56, 80);
                    }
                    lblOpId.Text = "";
                    CBcr.It.SaveStatus(false);
                }
                HideMenu(CData.Lev);
            }
        }

        private void HideMenu(ELv RetLv)
        {
            //20190309 ghk_level
            #region Auto
            rdb_Man.Visible  = (int)RetLv >= CData.Opt.iLvManual;
            rdb_Dev.Visible  = (int)RetLv >= CData.Opt.iLvDevice;
            rdb_Whl.Visible  = (int)RetLv >= CData.Opt.iLvWheel ;
            rdb_SPC.Visible  = (int)RetLv >= CData.Opt.iLvSpc   ;
            rdb_Opt.Visible  = (int)RetLv >= CData.Opt.iLvOption;
            rdb_Util.Visible = (int)RetLv >= CData.Opt.iLvUtil  ;
            rdb_Mst.Visible  = (RetLv == ELv.Master);
            #endregion
        }

        public void ChangePage(int Page)
        {
            if (Page == m_iPage)
            { return; }    // 현재 뷰와 새로운 뷰가 같은 경우 리턴
            else
            {
                switch (m_iPage)    // 이전 뷰 Close
                {
                    case 0:
                        // 2020.09.07 SungTae
                        if (CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)
                        { m_vwAuto.Close(); }
                        else
                        { m_vwAuto2.Close(); }
                        break;
                    case 1:
                        m_vwManl.Close();
                        break;
                    case 2:
                        m_vwRcp.Close();
                        break;
                    case 3:
                        m_vwWhl.Close();
                        break;
                    case 4:
                        m_vwSPC.Close();
                        break;
                    case 5:
                        m_vwOpt.Close();
                        break;
                    case 6:
                        m_vwEq.Close();
                        break;
                    case 10:
                        m_vwShw.Close();
                        break;
                }

                pnlMM_Base.Controls.Clear();    // Panel 에서 이전 뷰 삭제
                m_iPage = Page;    // 인덱스 변경

                switch (m_iPage)    // 신규 뷰 Open 및 표시
                {
                    case 0:
                        if (CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)
                        {
                            pnlMM_Base.Controls.Add(m_vwAuto);
                            m_vwAuto.Open();
                        }
                        else
                        {
                            pnlMM_Base.Controls.Add(m_vwAuto2);
                            m_vwAuto2.Open();
                        }
                        HideMenu(CData.Lev);
                        break;
                }
            }
        }

        private void lblSt_DeviceName_DoubleClick(object sender, EventArgs e)
        {
            // 2020.08.24 JSKim St
            //if (CSQ_Main.It.m_iStat == EStatus.Auto_Running           ) return;
            //if (!CData.LotInfo.bLotOpen && CData.Lev < ELv.Technician) return;
            //if (CData.LotInfo.bLotOpen && CData.Lev < ELv.Engineer) return;
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK || CData.CurCompany == ECompany.SkyWorks)
            {
                if (CSQ_Main.It.m_iStat == EStatus.Auto_Running             || 
                    CSQ_Main.It.m_iStat == EStatus.Manual                   || 
                    CSQ_Main.It.m_iStat == EStatus.Warm_Up                  || 
                    CSQ_Main.It.m_iStat == EStatus.All_Homing               || 
                    CData.L_GRD.GStep > 14 || CData.R_GRD.GStep > 14        || 
                    (!CData.LotInfo.bLotOpen && CData.Lev < ELv.Technician) || 
                    (CData.LotInfo.bLotOpen  && CData.Lev < ELv.Engineer))
                {
                    CData.bDevOnlyView = true;
                }
                else
                {
                    CData.bDevOnlyView = false;
                }
            }
            else
            {
                if (CSQ_Main.It.m_iStat == EStatus.Auto_Running) return;
                if (!CData.LotInfo.bLotOpen && CData.Lev < ELv.Technician) return;
                if (CData.LotInfo.bLotOpen && CData.Lev < ELv.Engineer) return;
            }
            // 2020.08.24 JSKim Ed

            CData.bLastClick = true; //190509 ksg :

            // 2020.09.07 SungTae
            if (CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)
            { m_vwAuto.Close(); }
            else
            { m_vwAuto2.Close(); }

            m_vwManl  .Close();
            m_vwRcp   .Close();
            m_vwWhl   .Close();
            m_vwSPC   .Close();
            m_vwOpt   .Close();
            m_vwEq    .Close();
            m_vwShw   .Close();
            
            rdb_Auto.Checked = false;
            rdb_Man .Checked = false;
            rdb_Dev .Checked = false;
            rdb_Whl .Checked = false;
            rdb_Mst .Checked = false;
            rdb_SPC .Checked = false;
            rdb_Opt .Checked = false;
            rdb_Util.Checked = false;
            
            pnlMM_Base.Controls.Clear();    // Panel 에서 이전 뷰 삭제
            pnlMM_Base.Controls.Add(m_vwRcp);
            m_vwRcp   .Open();
            m_iPage = 2;

            m_vwRcp.LinkSelectpage();
        }

        private void AutoChangeLevel()
        {
            CData.Lev = ELv.Operator;
            if (CData.Lev == ELv.Operator)
            {
                lbl_Level.BackColor = Color.FromArgb(41, 199, 201);
                lbl_Level.Image     = Properties.Resources.Operator64;
                lbl_Level.Text      = CData.Lev.ToString().ToUpper();

                lbl_Ver.BackColor   = Color.FromArgb(41, 199, 201);
            }
            if(CDataOption.UseCardReader)
            {
                lblOpId.Text = "";
                lblOpId.BackColor = Color.FromArgb(41, 199, 201);
            }
            CBcr.It.SaveStatus(false);
            HideMenu(CData.Lev);
            m_vwManl.InitPage();
        }
        #region Wheel & Dresser Change Part
        #region Auto View Change
        /// <summary>
        /// Wheel Change Auto Change View
        /// </summary>
        private void AutoChangeWheelView()
        {
            // 2020.09.07 SungTae
            if (CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)
            { m_vwAuto.Close(); }
            else
            { m_vwAuto2.Close(); }

            m_vwManl  .Close();
            m_vwRcp   .Close();
            m_vwWhl   .Close();
            m_vwSPC   .Close();
            m_vwOpt   .Close();
            m_vwEq    .Close();
            m_vwShw   .Close();
            
            rdb_Auto.Checked = false;
            rdb_Man .Checked = false;
            rdb_Dev .Checked = false;
            rdb_Whl .Checked = false;
            rdb_Mst .Checked = false;
            rdb_SPC .Checked = false;
            rdb_Opt .Checked = false;
            rdb_Util.Checked = false;
            
            pnlMM_Base.Controls.Clear();    // Panel 에서 이전 뷰 삭제
            pnlMM_Base.Controls.Add(m_vwWhl);
            m_vwWhl   .Open();
            m_iPage = 2;

            m_vwRcp.LinkSelectpage();
            if(!CData.WhlChgSeq.bStart && !CSQ_Main.It.m_bWhlLimitArm && !CSQ_Main.It.m_bDrsLimitArm)
            {
                CErr.Show(eErr.SYSTEM_WHEEL_JIG_CHECK);
                CData.WhlChgSeq.bStart = true;
            }
            else if(CSQ_Main.It.m_bWhlLimitArm)
            {
                CMsg.Show(eMsg.Change, "Wheel Life Alram", "Selection Wheel Infomation");
            }
            else if(CSQ_Main.It.m_bDrsLimitArm)
            {
                CMsg.Show(eMsg.Change, "Dresser Life Alram", "Selection Dresser Infomation");
            }
            
        }
        public void MainBtnHide(bool Hide)
        {
            if(Hide)
            {
                rdb_Auto  .Enabled = false;
                rdb_Man   .Enabled = false;
                rdb_Dev   .Enabled = false;
                rdb_Whl   .Enabled = false;
                rdb_SPC   .Enabled = false;
                rdb_Opt   .Enabled = false;
                rdb_Util  .Enabled = false;
                rdb_OPManu.Enabled = false;
                rdb_OPManu.Enabled = false;
                btnM_Exit .Enabled = false;
                lbl_Level .Enabled = false;
            }
            else
            {
                rdb_Auto  .Enabled = true ;
                rdb_Man   .Enabled = true ;
                rdb_Dev   .Enabled = true ;
                rdb_Whl   .Enabled = true ;
                rdb_SPC   .Enabled = true ;
                rdb_Opt   .Enabled = true ;
                rdb_Util  .Enabled = true ;
                rdb_OPManu.Enabled = true ;
                rdb_OPManu.Enabled = true ;
                btnM_Exit .Enabled = true ;
                lbl_Level .Enabled = true ;
            }
        }
        #endregion
        //190620 ksg :
        #region Wheel Change Seq
        /// <summary>
        /// Wheel Change Seq
        /// </summary>
        public void WheelChangeCheckSeq()
        {
            if(!CData.WhlChgSeq.bStart) return;
            /*
            if((CSQ_Main.It.m_bRun || CSQ_Man.It.Seq != ESeq.Idle) && CData.WhlChgSeq.iStep == 0)
            {
                CData.WhlChgSeq.bStart = false;
                CData.WhlChgSeq.iStep  = 0    ;
            }
            */
            bool xbWheelLZig   = (!CIO.It.Get_X(eX.GRDL_WheelZig));
            bool xbWheelRZig   = (!CIO.It.Get_X(eX.GRDR_WheelZig));

            switch(CData.WhlChgSeq.iStep)
            {
                case 1:
                {
                    if(!CData.WhlChgSeq.bStartShow)
                    {
                        CData.WhlChgSeq.bStartShow = true;

                        if(CMsg.Show(eMsg.Change, "Wheel Change", "Selection Wheel Infomation") == DialogResult.OK)
                        {
                            // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                            //if(CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ) //191202 ksg :
                            if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                                CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC)
                            {
                                CData.bWhlChangeMode = true;       // 2020.09.10 SungTae : Wheel Change 시 PCW Auto Off 기능 보류용 (Qorvo VOC)
                            }

                            if(!CData.WhlChgSeq.bSltLeft && !CData.WhlChgSeq.bSltRight)
                            {
                                CData.WhlChgSeq.bStartShow = false;
                                return;
                            }
                            //CSQ_Man.It.Seq = ESeq.GRD_Wait;
                            CData.WhlChgSeq.iStep = 2;
                        }
                        else 
                        {
                            CData.WhlChgSeq.bStart = false;
                            CData.WhlChgSeq.iStep  = 0    ;

                            CData.bWhlChangeMode   = false;       // 2020.09.10 SungTae : Wheel Change 시 PCW Auto Off 기능 보류용 (Qorvo VOC)
                        }
                    }
                    break;
                }
                case 2:
                {
                    if(!CData.WhlChgSeq.bWhlSelShow)
                    {
                        CData.WhlChgSeq.bWhlSelShow = true;

                        if(CMsg.Show(eMsg.Change, "Wheel Change", "Selection Wheel File") == DialogResult.OK)
                        {
                            CData.WhlChgSeq.iStep = 3;
                            CData.WhlChgSeq.bBtnHide = true;
                        }
                        //else WheelChangeCancel();
                    }
                    break;
                }
                case 3:
                {
                    if(!CData.WhlChgSeq.bWhlSelFShow && CData.WhlChgSeq.bSltWhlFile)
                    {
                        CData.WhlChgSeq.bWhlSelFShow = true ;
                        CData.WhlChgSeq.bBtnHide     = false;
                        if(CMsg.Show(eMsg.Change, "Wheel Change", "Moving Wheel Change Position") == DialogResult.OK)
                        {
                            CSQ_Man.It.Seq = ESeq.GRD_Wait;
                        }
                        //else WheelChangeCancel();
                    }
                    break;
                }

                case 4:
                {
                    //if(CData.CurCompany == eCompany.JSCK)
                    if(CData.CurCompany == ECompany.JSCK || CData.CurCompany == ECompany.ASE_KR || CData.CurCompany == ECompany.JCET) //190904 ksg : Ase Kr도 추가,200625 lks
                    {
                        CData.WhlChgSeq.bStart = false;
                        CData.WhlChgSeq.iStep  = 0    ;
                    }
                    string sMsg = "Wheel Change\n\r After completing wheel replacement, click OK button";
                    if(CData.WhlChgSeq.bSltLeft && !CData.WhlChgSeq.bLWhlChangeShow)
                    {
                        CData.WhlChgSeq.bLWhlChangeShow = true;
                        if(CMsg.Show(eMsg.Change, "Wheel Change - LEFT", sMsg) == DialogResult.OK)
                        {
                            if(CData.WhlChgSeq.bLWhlJigCheck && !xbWheelLZig)
                            {
                                CData.WhlChgSeq.iStep = 5;
                            }
                            else CData.WhlChgSeq.bLWhlChangeShow = false;
                        }
                        //else WheelChangeCancel();
                    }
                    if(CData.WhlChgSeq.bSltRight && !CData.WhlChgSeq.bRWhlChangeShow)
                    {
                        CData.WhlChgSeq.bRWhlChangeShow = true;
                        if(CMsg.Show(eMsg.Change, "Wheel Change - RIGHT", sMsg) == DialogResult.OK)
                        {
                            if(CData.WhlChgSeq.bRWhlJigCheck && !xbWheelRZig)
                            {
                                CData.WhlChgSeq.iStep = 5;
                            }
                            else CData.WhlChgSeq.bRWhlChangeShow = false;
                        }
                        //else WheelChangeCancel();
                    }
                    break;
                }
                case 5:
                {
                    if(!CData.WhlChgSeq.bWhlMeanShow && !CData.WhlChgSeq.bWhlMeanS)
                    {
                        CData.WhlChgSeq.bLWhlJigCheck = false;
                        CData.WhlChgSeq.bRWhlJigCheck = false;
                        CData.WhlChgSeq.bWhlMeanShow = true;
                        string sTitle;
                        if(CData.WhlChgSeq.bSltLeft) sTitle = "Wheel Meansure - LEFT"  ;
                        else                         sTitle = "Wheel Meansure - RIGHT";
                        string sMsg = "Wheel Measure\n\r Click OK button to start automatically.";
                        if(CMsg.Show(eMsg.Change, sTitle, sMsg) == DialogResult.OK)
                        {
                            if(CData.WhlChgSeq.bSltLeft) CSQ_Man.It.Seq  = ESeq.GRL_Wheel_Measure;
                            else                         CSQ_Man.It.Seq  = ESeq.GRR_Wheel_Measure;
                        }
                        else 
                        {
                            //WheelChangeCancel();
                        }
                    }
                    if(CData.WhlChgSeq.bWhlMeanShow && CData.WhlChgSeq.bWhlMeanS && !CData.WhlChgSeq.bWhlMeanF)
                    {
                        CData.WhlChgSeq.bWhlMeanF = true;
                        string sTitle;
                        if(CData.WhlChgSeq.bSltLeft) sTitle = "Wheel Meansure - LEFT"  ;
                        else                         sTitle = "Wheel Meansure - RIGHT";
                        string sMsg = "Wheel Measure\n\r Wheel Meansure Finish\r\n Click OK Button";
                        if(CMsg.Show(eMsg.Change, sTitle, sMsg) == DialogResult.OK)
                        {
                            CData.WhlChgSeq.iStep = 6;
                        }
                        //else WheelChangeCancel();
                    }
                    break;
                }
                case 6:
                {
                    if(!CData.WhlChgSeq.bWhlDressShow && !CData.WhlChgSeq.bWhlDressS)
                    {
                        CData.WhlChgSeq.bWhlDressShow = true;
                        string sTitle;
                        if(CData.WhlChgSeq.bSltLeft) sTitle = "Wheel Dressing - LEFT"  ;
                        else                         sTitle = "Wheel Dressing - RIGHT";
                        string sMsg = "Wheel Dressing\n\r Click OK button to start automatically";
                        if(CMsg.Show(eMsg.Change, sTitle, sMsg) == DialogResult.OK)
                        {
                            if(CData.WhlChgSeq.bSltLeft) CSQ_Man.It.Seq  = ESeq.GRL_Dressing;
                            else                         CSQ_Man.It.Seq  = ESeq.GRR_Dressing;
                        }
                        else 
                        {
                            //WheelChangeCancel();
                        }
                    }
                    if(CData.WhlChgSeq.bWhlMeanShow && CData.WhlChgSeq.bWhlDressS && !CData.WhlChgSeq.bWhlDressF)
                    {
                        CData.WhlChgSeq.bWhlDressF = true;
                        string sTitle;
                        if(CData.WhlChgSeq.bSltLeft) sTitle = "Wheel Dressing - LEFT"  ;
                        else                         sTitle = "Wheel Dressing - RIGHT";
                        string sMsg = "Wheel Measure\n\r Wheel Dressing Finish\n\r Click OK Button";
                        if(CMsg.Show(eMsg.Change, sTitle, sMsg) == DialogResult.OK)
                        {
                            CData.WhlChgSeq.iStep = 7;
                        }
                        //else WheelChangeCancel();
                    }
                    break;
                }
                case 7:
                {
                    if(!CData.WhlChgSeq.bWhlCompShow)
                    {
                        CData.WhlChgSeq.bWhlCompShow = true;
                        string sTitle;
                        if(CData.WhlChgSeq.bSltLeft) sTitle = "Wheel Change - LEFT" ;
                        else                         sTitle = "Wheel Change - RIGHT";
                        string sMsg = "Wheel change complete";
                        if(CMsg.Show(eMsg.Change, sTitle, sMsg) == DialogResult.OK)
                        {
                            CData.WhlChgSeq.bStart = false;
                            CData.WhlChgSeq.iStep  = 0    ;
                        }
                        else 
                        {
                            //WheelChangeCancel();
                        }

                        CData.bWhlChangeMode = false;       // 2020.09.10 SungTae : Wheel Change 시 PCW Auto Off 기능 보류용 (Qorvo VOC)
                    }
                    break;
                }
            }

        }
        /*
        public void WheelChangeCancel()
        {
            CData.WhlChgSeq.bStart          = true ;
            CData.WhlChgSeq.iStep           = 1    ;
            CData.WhlChgSeq.bStartShow      = false;
            CData.WhlChgSeq.bSltLeft        = false;
            CData.WhlChgSeq.bSltRight       = false;
            CData.WhlChgSeq.bSltWhlFile     = false;
            CData.WhlChgSeq.bWhlSelShow     = false;
            CData.WhlChgSeq.bWhlSelFShow    = false;
            CData.WhlChgSeq.bLWhlChangeShow = false;
            CData.WhlChgSeq.bRWhlChangeShow = false;
            CData.WhlChgSeq.bWhlMeanS       = false;
            CData.WhlChgSeq.bWhlMeanF       = false;
            CData.WhlChgSeq.bWhlMeanShow    = false;
            CData.WhlChgSeq.bWhlDressS      = false;
            CData.WhlChgSeq.bWhlDressF      = false;
            CData.WhlChgSeq.bWhlDressShow   = false;
            CData.WhlChgSeq.bWhlCompShow    = false;
        }*/
        #endregion 

        //190620 ksg :
        #region Dresser Change Seq
        /// <summary>
        /// Wheel Change Seq
        /// </summary>
        public void DresserChangeCheckSeq()
        {
            if(!CData.DrsChgSeq.bStart) return;
            /*
            if((CSQ_Main.It.m_bRun || CSQ_Man.It.Seq != ESeq.Idle) && CData.DrsChgSeq.iStep == 0)
            {
                CData.DrsChgSeq.bStart = false;
                CData.DrsChgSeq.iStep  = 0    ;
            }
            */
            switch(CData.DrsChgSeq.iStep)
            {
                case 1:
                {//Select Dresser 
                    if(!CData.DrsChgSeq.bStartShow)
                    {
                        CData.DrsChgSeq.bStartShow = true;
                        if(CMsg.Show(eMsg.Change, "Dressing Board Change", "Selection Wheel Infomation") == DialogResult.OK)
                        {
                            if(!CData.DrsChgSeq.bSltLeft && !CData.DrsChgSeq.bSltRight && !CData.DrsChgSeq.bSltDual)
                            {
                                CData.DrsChgSeq.bStartShow = false;
                                return;
                            }
                            CSQ_Man.It.Seq = ESeq.GRD_DRESSER_REPLACE;
                        }
                        else 
                        {
                            //CData.WhlChgSeq.bStartShow = false;
                            CData.DrsChgSeq.bStart = false;
                            CData.DrsChgSeq.iStep  = 0    ;
                        }
                    }
                    break;
                }
                case 2:
                {//Dressing Board Change After Click
                    string sMsg = "Dressing Board Change\n\r After completing Dressing Board replacement, click OK button.";
                    if(CData.DrsChgSeq.bSltDual && !CData.DrsChgSeq.bDDrsChangeShow)
                    {
                        CData.DrsChgSeq.bDDrsChangeShow = true;
                        if(CMsg.Show(eMsg.Change, "Dressing Board Change - DUAL", sMsg) == DialogResult.OK)
                        {
                            CData.DrsChgSeq.iStep = 5;
                        }
                        //else DresserChangeCancel();
                    }
                    else
                    {
                        if(CData.DrsChgSeq.bSltLeft && !CData.DrsChgSeq.bLDrsChangeShow)
                        {
                            CData.DrsChgSeq.bLDrsChangeShow = true;
                            if(CMsg.Show(eMsg.Change, "Dressing Board Change - LEFT", sMsg) == DialogResult.OK)
                            {
                                CData.DrsChgSeq.iStep = 3;
                            }
                            //else DresserChangeCancel();
                        }
                        if(CData.DrsChgSeq.bSltRight && !CData.DrsChgSeq.bRDrsChangeShow)
                        {
                            CData.DrsChgSeq.bRDrsChangeShow = true;
                            if(CMsg.Show(eMsg.Change, "Dressing Board Change - RIGHT", sMsg) == DialogResult.OK)
                            {
                                CData.DrsChgSeq.iStep = 3;
                            }
                            //else DresserChangeCancel();
                        }
                    }
                    
                    break;
                }
                case 3:
                {//Dresser Board Meansure(Onny Select One(Left Or Right))
                    if(!CData.DrsChgSeq.bDrsMeanShow && !CData.DrsChgSeq.bDrsMeanS)
                    {
                        CData.DrsChgSeq.bDrsMeanShow = true;
                        string sTitle;
                        if(CData.DrsChgSeq.bSltLeft) sTitle = "Dressing Board Meansure - LEFT"  ;
                        else                         sTitle = "Dressing Board Meansure - RIGHT";
                        string sMsg = "Dressing Board Measure\n\r Click OK button to start automatically.";
                        if(CMsg.Show(eMsg.Change, sTitle, sMsg) == DialogResult.OK)
                        {
                            if(CData.DrsChgSeq.bSltLeft) CSQ_Man.It.Seq  = ESeq.GRL_Dresser_Measure;
                            else                         CSQ_Man.It.Seq  = ESeq.GRR_Dresser_Measure;
                        }
                        else 
                        {
                            //DresserChangeCancel();
                        }
                    }
                    if(CData.DrsChgSeq.bDrsMeanShow && CData.DrsChgSeq.bDrsMeanS && !CData.DrsChgSeq.bDrsMeanF)
                    {
                        CData.DrsChgSeq.bDrsMeanF = true;
                        string sTitle;
                        if(CData.DrsChgSeq.bSltLeft) sTitle = "Dressing Board Meansure - LEFT"  ;
                        else                         sTitle = "Dressing Board Meansure - RIGHT";
                        string sMsg = "Dressing Board Measure\n\r Wheel Meansure Finish\r\n Click OK Button";
                        if(CMsg.Show(eMsg.Change, sTitle, sMsg) == DialogResult.OK)
                        {
                            CData.DrsChgSeq.iStep = 4;
                        }
                        //else DresserChangeCancel();
                    }
                    break;
                }
                case 4:
                {//finish
                    if(!CData.DrsChgSeq.bDrsCompShow)
                    {
                        CData.DrsChgSeq.bDrsCompShow = true;
                        string sTitle;
                        if(CData.DrsChgSeq.bSltLeft) sTitle = "Dressing Board Change - LEFT" ;
                        else                         sTitle = "Dressing Board Change - RIGHT";
                        string sMsg = "Dressing Board change complet";
                        if(CMsg.Show(eMsg.Change, sTitle, sMsg) == DialogResult.OK)
                        {
                            CData.DrsChgSeq.bStart = false;
                            CData.DrsChgSeq.iStep  = 0    ;
                        }
                        else 
                        {
                            //DresserChangeCancel();
                        }
                    }
                    break;
                }
                case 5:
                {//Dresser Board Meansure(Dual Select)
                    if(!CData.DrsChgSeq.bDrsMeanShow && !CData.DrsChgSeq.bDrsMeanDS)
                    {
                        CData.DrsChgSeq.bDrsMeanShow = true;
                        string sTitle;
                        sTitle = "Dressing Board Meansure - DUAL"  ;
                        string sMsg = "Dressing Board Measure\n\r Click OK button to start automatically.";
                        if(CMsg.Show(eMsg.Change, sTitle, sMsg) == DialogResult.OK)
                        {
                            CSQ_Man.It.Seq  = ESeq.GRL_Dresser_Measure;
                        }
                        else 
                        {
                            //DresserChangeCancel();
                        }
                    }
                    if(CData.DrsChgSeq.bDrsMeanShow && CData.DrsChgSeq.bDrsMeanDS && !CData.DrsChgSeq.bDrsMeanDF)
                    {
                        CData.DrsChgSeq.bDrsMeanDF = true;
                        string sTitle;
                        sTitle = "Dressing Board Meansure - DUAL"  ;
                        string sMsg = "Dressing Board Measure\n\r Dressing Board Meansure Finish\r\n Click OK Button";
                        if(CMsg.Show(eMsg.Change, sTitle, sMsg) == DialogResult.OK)
                        {
                            CData.DrsChgSeq.iStep = 6;
                        }
                        //else DresserChangeCancel();
                    }
                    break;
                }
                case 6:
                {//finish
                    if(!CData.DrsChgSeq.bDrsCompShow)
                    {
                        CData.DrsChgSeq.bDrsCompShow = true;
                        string sTitle;
                        sTitle = "Dressing Board Change - DUAL" ;
                        string sMsg = "Dressing Board change complete";
                        if(CMsg.Show(eMsg.Change, sTitle, sMsg) == DialogResult.OK)
                        {
                            CData.DrsChgSeq.bStart = false;
                            CData.DrsChgSeq.iStep  = 0    ;
                        }
                        else 
                        {
                            //DresserChangeCancel();
                        }
                    }
                    break;
                }
            }

        }
        public void DresserChangeCancel()
        {
            CData.DrsChgSeq.bStart          = true ;
            CData.DrsChgSeq.iStep           = 1    ;
            CData.DrsChgSeq.bSltLeft        = false;
            CData.DrsChgSeq.bSltRight       = false;
            CData.DrsChgSeq.bSltDual        = false;
            CData.DrsChgSeq.bStartShow      = false;
            CData.DrsChgSeq.bLDrsChangeShow = false;
            CData.DrsChgSeq.bRDrsChangeShow = false;
            CData.DrsChgSeq.bDDrsChangeShow = false;
            CData.DrsChgSeq.bDrsMeanS       = false;
            CData.DrsChgSeq.bDrsMeanF       = false;
            CData.DrsChgSeq.bDrsMeanDS      = false;
            CData.DrsChgSeq.bDrsMeanDF      = false;
            CData.DrsChgSeq.bDrsMeanShow    = false;
            CData.DrsChgSeq.bDrsCompShow    = false;
        }
        #endregion

        #endregion

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            //20191106 ghk_lightcontrol
            if (CDataOption.LightControl == eLightControl.Use)
            { CLight.It.Close_Port(); }
        }

        // 2021.07.21 lhs Start : PM Count 체크하여 알림창 표시, 모달리스
        private void CheckPMCount()
        {
            // Set Count와 Current Count 비교하여 조건이 맞으면 표시하고 아니면 리턴
            string  strMsg      = "";
            bool    bShowMsg    = false;    // 창 표시
            bool[]  bShow       = new bool[6];
            int[]   nSetCnt     = new int[6];
            int[]   nCurrCnt    = new int[6];

            // 조건 체크
            nSetCnt[0] = CData.tPM.nLT_Sponge_Check_SetCnt;         nCurrCnt[0] = CData.tPM.nLT_Sponge_Check_CurrCnt;
            nSetCnt[1] = CData.tPM.nRT_Sponge_Check_SetCnt;         nCurrCnt[1] = CData.tPM.nRT_Sponge_Check_CurrCnt;
            nSetCnt[2] = CData.tPM.nOffP_Sponge_Check_SetCnt;       nCurrCnt[2] = CData.tPM.nOffP_Sponge_Check_CurrCnt;
            nSetCnt[3] = CData.tPM.nLT_Sponge_Change_SetCnt;        nCurrCnt[3] = CData.tPM.nLT_Sponge_Change_CurrCnt;
            nSetCnt[4] = CData.tPM.nRT_Sponge_Change_SetCnt;        nCurrCnt[4] = CData.tPM.nRT_Sponge_Change_CurrCnt;
            nSetCnt[5] = CData.tPM.nOffP_Sponge_Change_SetCnt;      nCurrCnt[5] = CData.tPM.nOffP_Sponge_Change_CurrCnt;
			
            for (int i = 0; i < 6; i++)
			{
                if (nSetCnt[i] > 0) // 2022.01.26 : 0보다 크다는 조건 추가
                {
                    bShow[i] = nSetCnt[i] <= nCurrCnt[i];
                    if (bShow[i]) { bShowMsg = true; }
                }
            }

            // 조건이 아니면 리턴
            if (!bShowMsg)
            {
                CData.bBuzzPMMsgWnd         = false;  // 2021.10.22 lhs  메시지창 표시 안된 상태 -> Buzzer 끄기
                CData.nShowStatePMMsgWnd    = 0;
                return;
            }

            // 창이 있으면 리턴
            Form fc = Application.OpenForms["frmMsg"];
            if (fc != null && CData.nShowStatePMMsgWnd == 1)  // 메시지창이 있으면 리턴
            {
                CData.bBuzzPMMsgWnd = true; // 2021.10.22 lhs  // 메시지창 표시 상태 -> Buzzer 발생 목적
                dtPMMsg = DateTime.Now;
                return;
            }

            CSQ_Main.It.m_bPause = true;

            // 일정시간 동안 표시 안함
            TimeSpan tsWait = DateTime.Now - dtPMMsg;
            if( (tsWait.TotalMinutes <= CData.tPM.nPMMsg_ReDisp_Minute) && CData.nShowStatePMMsgWnd == 2 )
            {
                CData.bBuzzPMMsgWnd = false; // 2021.10.22 lhs  메시지창 표시 안된 상태 -> Buzzer 끄기
                return;
			}

            // 메시지 내용 추가
            if (bShow[0]) { strMsg += string.Format("Check left table sponge count (Check) : Set({0}) <= Current({1})\r\n",     nSetCnt[0], nCurrCnt[0]);   }
            if (bShow[1]) { strMsg += string.Format("Check right table sponge count (Check) : Set({0}) <= Current({1})\r\n",    nSetCnt[1], nCurrCnt[1]);   }
			if (bShow[2]) { strMsg += string.Format("Check OffPicker sponge count (Check) : Set({0}) <= Current({1})\r\n",      nSetCnt[2], nCurrCnt[2]);   }
			if (bShow[3]) { strMsg += string.Format("Check left table sponge Count (Change) : Set({0}) <= Current({1})\r\n",    nSetCnt[3], nCurrCnt[3]);   }
			if (bShow[4]) { strMsg += string.Format("Check right table sponge count (Change) : Set({0}) <= Current({1})\r\n",   nSetCnt[4], nCurrCnt[4]);   }
			if (bShow[5]) { strMsg += string.Format("Check OffPicker sponge count (Change) : Set({0}) <= Current({1})\r\n",     nSetCnt[5], nCurrCnt[5]);   }

            CMsg.ShowModeless(eMsg.Notice, "Check PM Count", strMsg);
            dtPMMsg = DateTime.Now;

            CData.bBuzzPMMsgWnd         = true;  // 2021.10.22 lhs  메시지창 표시 상태 -> Buzzer 발생 목적
            CData.nShowStatePMMsgWnd    = 1;

        }
        // 2021.07.21 lhs End

        // 2022.08.30 lhs : Spindle Current Max/Min 알람
        private void CheckRangeSpindleCurrent()
        {
            for (int nWy = 0; nWy < 2; nWy++)
            {
                // Current Max 
                if (CData.GrData[nWy].bSplMaxCurrFlag && 
                    CData.GrData[nWy].nSplMaxCurr > CData.Dev.aData[nWy].nSpdCurrHL)
                {
                    CData.GrData[nWy].bSplMaxCurrFlag = false;  // 연속적으로 창 표시 안되도록

                    string sTitle   = "Spindle Load Current Alarm";
                    string sWy      = (nWy == 0) ? "Left" : "Right";
                    string sMsg     = sWy + " Spindle load current가 설정한 High Range를 Over 하였습니다.\n\r장비 점검이 필요합니다.";
                    
                    _SetLog($"{sTitle} : {sMsg}");
                    
                    if (CMsg.Show(eMsg.Notice, sTitle, sMsg) == DialogResult.OK)
                    {
                        CData.GrData[nWy].nSplMaxCurr = 0;
                        _SetLog($"OK Click, {nWy}, nSplMaxCurr = 0(초기화)");
                    }                    
                }

                // Current Min 
                if (CData.GrData[nWy].bSplMinCurrFlag &&
                    CData.GrData[nWy].nSplMinCurr < CData.Dev.aData[nWy].nSpdCurrLL)
                {
                    CData.GrData[nWy].bSplMinCurrFlag   = false;      // 연속적으로 창 표시 안되도록

                    string sTitle   = "Spindle Load Current Alarm";
                    string sWy      = (nWy == 0) ? "Left" : "Right";
                    string sMsg     = sWy + " Spindle load current가 설정한 Low Range를 Over 하였습니다.\n\r장비 점검이 필요합니다.";

                    if (CMsg.Show(eMsg.Notice, sTitle, sMsg) == DialogResult.OK)
                    {
                        CData.GrData[nWy].nSplMinCurr = 999999;
                        _SetLog($"OK Click, {nWy}, nSplMinCurr = 999999(초기화)");
                    }
                }
            }
        }

        private void lblSt_Sub_Click(object sender, EventArgs e)
        {
            CSQ_Main.It.m_iStat = EStatus.Idle;
        }

        // 2022.07.11 SungTae Start : [추가] (ASE-KH VOC) Network Drive Connection Check 시 Return Value 이력 남기기 위해 추가
        private void _SetLog(string sMsg)
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            string sCls = this.Name;
            string sMth = sf.GetMethod().Name;

            CLog.Save_Log(eLog.None, eLog.OPL, string.Format("{0}.cs {1}()\t{2}", sCls, sMth, sMsg));
        }

        private string _ChkNetDriveConnectionState(int nRet)
        {
            string sErrMsg = string.Empty;

            switch(nRet)
            {
                case 0:     sErrMsg = $"NO_ERROR";                          break;
                case 53:    sErrMsg = $"ERROR_NO_NET_OR_BAD_SERVER";        break;
                case 1326:  sErrMsg = $"ERROR_BAD_USER_OR_PASSWORD";        break;
                case 5:     sErrMsg = $"ERROR_ACCESS_DENIED";               break;
                case 85:    sErrMsg = $"ERROR_ALREADY_ASSIGNED";            break;
                case 66:    sErrMsg = $"ERROR_BAD_DEV_TYPE";                break;
                case 1200:  sErrMsg = $"ERROR_BAD_DEVICE";                  break;
                case 67:    sErrMsg = $"ERROR_BAD_NET_NAME";                break;
                case 1206:  sErrMsg = $"ERROR_BAD_PROFILE";                 break;
                case 1204:  sErrMsg = $"ERROR_BAD_PROVIDER";                break;
                case 170:   sErrMsg = $"ERROR_BUSY";                        break;
                case 1223:  sErrMsg = $"ERROR_CANCELLED";                   break;
                case 1205:  sErrMsg = $"ERROR_CANNOT_OPEN_PROFILE";         break;
                case 1202:  sErrMsg = $"ERROR_DEVICE_ALREADY_REMEMBERED";   break;
                case 1208:  sErrMsg = $"ERROR_EXTENDED_ERROR";              break;
                case 86:    sErrMsg = $"ERROR_INVALID_PASSWORD";            break;
                case 1203:  sErrMsg = $"ERROR_NO_NET_OR_BAD_PATH";          break;
                case 487:   sErrMsg = $"ERROR_INVALID_ADDRESS";             break;
                case 54:    sErrMsg = $"ERROR_NETWORK_BUSY";                break;
                case 59:    sErrMsg = $"ERROR_UNEXP_NET_ERR";               break;
                case 87:    sErrMsg = $"ERROR_INVALID_PARAMETER";           break;
                case 1219:  sErrMsg = $"ERROR_MULTIPLE_CONNECTION";         break;
            }

            return sErrMsg;
        }
		// 2022.07.11 SungTae End



        // 2022.09.20 lhs 테스트용 : 추후 지울것
        //private void button1_Click(object sender, EventArgs e)
        //{
        //    double dTotalDep = 0.024444;
        //    double dCycleDep = 0.005;
        //    int iRet = 0;

        //    // 2022.09.20 lhs : 소수점 4자리까지 유효하게
        //    dTotalDep = Math.Round(dTotalDep, 4);
        //    dCycleDep = Math.Round(dCycleDep, 4);

        //    iRet = (int)(dTotalDep / dCycleDep);

        //    double dRemainder = dTotalDep % dCycleDep;
        //    dRemainder = Math.Round(dRemainder, 4);

        //    _SetLog(string.Format("Total thickness : {0}mm  Cycle thickness : {1}mm", dTotalDep, dCycleDep));

        //    int nCount = iRet;
        //}


		// 2022.09.01 lhs 테스트용 : 추후 지울것
		//private void button2_Click(object sender, EventArgs e)
		//{
		//    int nWy = 0;
		//    CData.GrData[nWy].bSplMaxCurrFlag = true;
		//    CData.GrData[nWy].nSplMaxCurr = 3000;
		//    CData.GrData[nWy].bSplMinCurrFlag = true;
		//    CData.GrData[nWy].nSplMinCurr = 100;

		//    nWy = 1;
		//    CData.GrData[nWy].bSplMaxCurrFlag = true;
		//    CData.GrData[nWy].nSplMaxCurr = 3000;
		//    CData.GrData[nWy].bSplMinCurrFlag = true;
		//    CData.GrData[nWy].nSplMinCurr = 100;
		//}


	}
}
