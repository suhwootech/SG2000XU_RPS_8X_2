using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SG2000X
{
    public partial class frmLot : Form
    {
        public string Val { get; private set; }
        private CTim m_Delay = new CTim();
        //20190618 ghk_dfserver
        private bool m_bLotClick = false;
        private bool m_bLotOp = false;
        //

        public frmLot()
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

            txt_Dev.Text = CData.Dev.sName;

            cmb_Cnt.Items.Clear();
            for (int i = 1; i <= CData.Opt.iLotCnt; i++)
            {
                cmb_Cnt.Items.Add(i);
            }
            cmb_Cnt.SelectedIndex = 0;

            //190802 ksg : 추가
            if(CDataOption.SecsUse == eSecsGem.Use)
            {
                if(CData.CurCompany == ECompany.ASE_KR)
                {
                    lbl_LToolId.Visible = true;
                    txt_LToolId.Visible = true;
                }
                else if(CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK || CData.CurCompany == ECompany.JCET) //200121 ksg :,200625 lks
                {
                    lbl_LToolId.Visible = true;
                    txt_LToolId.Visible = true;
                    lbl_RToolId.Visible = true;
                    txt_RToolId.Visible = true;
                    lbl_LDrsId .Visible = true;
                    txt_LMTId  .Visible = true;
                    lbl_RDrsId .Visible = true;
                    txt_RMTId  .Visible = true;
					txt_LMTId.Text = CData.LotInfo.sLDrsId;
                    txt_RMTId.Text = CData.LotInfo.sRDrsId;
                    txt_LToolId.Text = CData.LotInfo.sLToolId;
                    txt_RToolId.Text = CData.LotInfo.sRToolId;
                }
            }
            else
            {
                lbl_LToolId.Visible = false;
                txt_LToolId.Visible = false;
                lbl_RToolId.Visible = false;
                txt_RToolId.Visible = false;
                lbl_LDrsId .Visible = false;
                txt_LMTId  .Visible = false;
                lbl_RDrsId .Visible = false;
                txt_RMTId  .Visible = false;
            }

            // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
            //if(CData.CurCompany == ECompany.Qorvo || (CData.CurCompany == ECompany.Qorvo_DZ) || CData.CurCompany == ECompany.SST) //191202 ksg :
            if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC ||
                CData.CurCompany == ECompany.SST)
            {
                lbl_StripCount.Visible = true;
                txt_StripCount.Visible = true;
            }
            else
            {
                lbl_StripCount.Visible = false;
                txt_StripCount.Visible = false;
            }

            if (CDataOption.SecsGemVer == eSecsGemVer.StandardB) 
            {
                lbl_StripCount.Visible = true;
                txt_StripCount.Visible = true;
            }

            //20190618 ghk_dfserver
            cmb_GrdLay.SelectedIndex = 0;
            if (CDataOption.eDfserver == eDfserver.NotUse)
            {
                lbl_GrdLay.Visible = false;
                cmb_GrdLay.Visible = false;
            }
            else
            {
                if (CData.Dev.bDfServerSkip)
                {
                    lbl_GrdLay.Visible = false;
                    cmb_GrdLay.Visible = false;
                }
                else
                {
                    lbl_GrdLay.Visible = true;
                    cmb_GrdLay.Visible = true;
                    if(CData.CurCompany == ECompany.ASE_KR)
                    {
                        int iMax = cmb_GrdLay.Items.Count;

                        for (int i = (iMax - 1); i >= 0; i--)
                        {
                            cmb_GrdLay.Items.RemoveAt(i);
                        }
                        cmb_GrdLay.Items.Insert(0, "TOP");
                        cmb_GrdLay.Items.Insert(1, "BOTTOM");
                    }
                    if (CData.CurCompany == ECompany.USI)
                    {
                        int iMax = cmb_GrdLay.Items.Count;

                        for (int i = (iMax - 1); i >= 0; i--)
                        {
                            cmb_GrdLay.Items.RemoveAt(i);
                        }
                        cmb_GrdLay.Items.Insert(0, "GL1");
                        cmb_GrdLay.Items.Insert(1, "GL2");
                    }
                }
            }
            if(CDataOption.UseSetUpStrip)
            {//210309 pjh : Set up strip Use Check Box Option
                ckb_SetUpStripUse.Visible = true;
                ckb_SetUpStripUse.Enabled = true;
            }
            else
            {
                ckb_SetUpStripUse.Visible = false;
                ckb_SetUpStripUse.Enabled = false;
            }
            //
        }

        private void btn_Open_Click(object sender, EventArgs e)
        {
            // 2020.10.29 JSKim St
            BeginInvoke(new Action(() => btn_Open.Enabled = false));

            try
            {
                // 2020.10.29 JSKim Ed

                // 2021.06.22 lhs Start : 잘못된 문자가 있는지 체크
                string sLotText = txt_Lot.Text.ToString();
                int nIdx = sLotText.IndexOfAny(Path.GetInvalidFileNameChars());
                if(nIdx != -1) // -1이면 발견되지 않음.
                {
                    CMsg.Show(eMsg.Warning, "warning", "Invalid characters such as \", <, >, or | ");
                    DialogResult = DialogResult.Cancel; //190122 ksg : 위치 위 조건문으로 옮김
                    return;
                }
                // 2021.06.22 lhs End

                //211216 pjh : D/F Server 사용 시 Lot Name에 _ 사용하지 못하도록 수정(중복 Lot Name 검사 및 Rename 시 "_" 기준으로 Parsing하기 때문)
                if (CData.CurCompany == ECompany.ASE_K12 && CDataOption.UseDFNet && CData.Opt.bDFNetUse && sLotText.Contains("_"))
                {
                    CMsg.Show(eMsg.Error, "Error", "It's not available to put '_' during use D/F Server");
                    DialogResult = DialogResult.Cancel;
                    return;
                }
                //

                if ((txt_Lot.Text == "") && (CData.Opt.bSecsUse == true))
				{
					CMsg.Show(eMsg.Warning, "warning", "No Input Lot Name");
					DialogResult = DialogResult.Cancel; //190122 ksg : 위치 위 조건문으로 옮김
					return;
				}

                //btn_Open.Enabled = false;
                /*
                if(CData.CurCompany == eCompany.AseKr)
                {
                    if(txt_ToolId.Text == "")
                    {
                        CMsg.Show(eMsg.Warning, "warning", "No Input Tool Name");
                        DialogResult = DialogResult.Cancel; //190122 ksg : 위치 위 조건문으로 옮김
                        return;
                    }
                }
                */

                // 201110 jym START : Skyworks batch file 실행 
                if (CData.CurCompany == ECompany.SkyWorks)
                {
                    // 배치 파일 존재 유무 파악
                    if (!File.Exists(GV.SKW_BATCH))
                    {   
                        // 배치 파일 없을 시 없이 진행 할 것인지 취소할 것인지 확인창 출력
                        if (CMsg.Show(eMsg.Warning, "Warning", "Batch file not exist.") == DialogResult.Cancel)
                        {
                            return;
                        }
                    }
                    else
                    {
                        // 불러올 디바이스 경로
                        string sPath = GV.PATH_DEVICE + CDev.It.m_sGrp + "\\" + CData.Dev.sName + ".dev";
                        // 배치파일 실행 결과
                        bool bRet = false;
                        FileInfo mInfo;
                        // 배치파일 실행 시간
                        DateTime dtPre = DateTime.Now;
                        // 타임아웃 시간
                        DateTime dtOut = dtPre.AddSeconds(5);
                        // 배치 파일 실행
                        Process.Start(GV.SKW_BATCH);

                        // 타임아웃 체크 동안 배치파일 실행 결과 확인
                        while(DateTime.Now < dtOut)
                        {
                            // Device 파일 정보 획득
                            mInfo = new FileInfo(sPath);
                            // 해당 디바이스 파일이 배치 파일 시작 전까지 변경이 되었는지 확인
                            if (mInfo.LastWriteTime.Ticks >= dtPre.Ticks)
                            {
                                bRet = true;
                                break;
                            }

                            Application.DoEvents();
                        }

                        if (!bRet)
                        {
                            // 배치 파일 실행 이후 디바이스 파일 변경 없을 시 에러
                            CMsg.Show(eMsg.Error, "Error", "Batch file not running.");
                            return;
                        }
                        else
                        {
                            // 정상적일 시 바뀐 디바이스 파일 불러오기
                            CDev.It.Load(sPath);
                        }
                    }
                }
                // 201110 jym END

                // 2021-02-01, jhLee, Skyworks VOC, Not send JobChange at Lot-Open
                //
                //190403 ksg :
                if (CData.Opt.bQcUse && CDataOption.UseQC)
                {
                    //CTcpIp.It.SaveLogOnly("QC Communicate START");
                    CGxSocket.It.writeLog("QC Communicate START");
                    string[] Temp;
                    string[] Temp1;
                    Temp = CData.DevCur.Split('\\');
                    Temp1 = Temp[5].Split('.');
                    /*if (!CTcpIp.It.IsConnect() && !CTcpIp.It.bIsConnect)
                    {
                        //CTcpIp.It.TcpIpConnect();
                        m_Delay.Wait(1000);
                    }*/
                    if (!CGxSocket.It.IsConnected())
                    {
                        CGxSocket.It.Connect();
                        m_Delay.Wait(1000);
                    }
                    //CTcpIp.It.bJob_Ok = false;
                    //CTcpIp.It.SendChageDevice(Temp[4], Temp1[0]);
                    CQcVisionCom.rcvQCJobChangeOK = false;
                    CQcVisionCom.rcvQCJobChangeNG = false;
                    CGxSocket.It.SendChageDevice(Temp[4], Temp1[0]);
                    int retryCount = 3;
                    while (retryCount > 0)
                    {
                        m_Delay.Wait(3000);
                        //if (CTcpIp.It.bJob_Ok)
                        if (CQcVisionCom.rcvQCJobChangeOK)
                            break;
                        else
                        {
                            //CTcpIp.It.SendChageDevice(Temp[4], Temp1[0]);
                            CGxSocket.It.SendChageDevice(Temp[4], Temp1[0]);
                        }
                        if (retryCount < 0) break;
                        retryCount--;
                    }
                    //CTcpIp.It.SaveLogOnly("Retry Count : " + retryCount);
                    CGxSocket.It.writeLog("Retry Count: " + retryCount);
                    //if (!CTcpIp.It.bJob_Ok)
                    if (!CQcVisionCom.rcvQCJobChangeOK)
                    {
                        CMsg.Show(eMsg.Warning, "warning", "Qc Vision Can't Change Device File");
                        btn_Open.Enabled = true;
                        return;
                    }
                    //CTcpIp.It.bJob_Ok = false;
                    CQcVisionCom.rcvQCJobChangeOK = false;

                    //CTcpIp.It.SaveLogOnly("QC Communicate END");
                    CGxSocket.It.writeLog("QC Communicate END");
                }

                //if (CSpc.It.ChkFindSameLot(txt_Lot.Text.ToString()))
                //211213 pjh : 중복되는 Lot Name이 있으면 자동으로 Number 추가하여 저장
                if (CData.CurCompany == ECompany.ASE_K12)
                {
                    CData.LotInfo.sNewLotName = txt_Lot.Text.ToString();
                    CSpc.iAddNum = 0;

                    if (CSpc.It.ChkFindSameLot(txt_Lot.Text.ToString()))
                    {
                        while (CSpc.It.ChkFindSameLot(CData.LotInfo.sNewLotName) && CSpc.iAddNum <= 20)
                        {
                            CSpc.It.SaveNewLotName(CData.LotInfo.sNewLotName, txt_Lot.Text.ToString());
                        }
                        if (CSpc.iAddNum > 20)
                        {
                            CMsg.Show(eMsg.Error, "Error", "Added number count goes over 20");
                            return;
                        }
                    }
                }
                //
                else if(CSpc.It.ChkFindSameLot(txt_Lot.Text.ToString()))
                {
                    CMsg.Show(eMsg.Error, "Error", "Before maked same lot. Please check again lot name");
                    btn_Open.Enabled = true;
                    return;
                }

                if (!CheckStatus())
                {
                    CMsg.Show(eMsg.Error, "ERROR", "Magazin Or Strip Remove Please");
                    btn_Open.Enabled = true;
                    return;
                }
                else
                {
                    //20190618 ghk_dfserver
                    //if ((CData.CurCompany == eCompany.AseKr || CData.CurCompany == eCompany.AseK26) && !CData.Dev.bDfServerSkip)
                    if (CDataOption.eDfserver == eDfserver.Use && !CData.Dev.bDfServerSkip)
                    {
                        timer_DfServer.Enabled  = true;
                        //20190626 ghk_dfserver
                        CData.LotInfo.sLotName      = txt_Lot.Text;
                        //
                        CData.dfInfo.sLotName       = CData.LotInfo.sLotName;
                        CData.LotInfo.iLotOpenHour  = DateTime.Now.Hour; //20200827 lks  // 2020.09.11 JSKim Add
                        CData.dfInfo.sGl            = cmb_GrdLay.Items[cmb_GrdLay.SelectedIndex].ToString();
                        if (CData.CurCompany == ECompany.ASE_KR)
                        {
                            if (cmb_GrdLay.Items[cmb_GrdLay.SelectedIndex].ToString() == "TOP")
                            {
                                CData.dfInfo.sGl = "GL1";
                            }
                            else
                            {
                                CData.dfInfo.sGl = "GL2";
                            }
                        }
                        CDf.It.SendLotOpen();

                        m_bLotClick = true;
                    }
                    else
                    {
                        CData.LotInfo.sLotName      = txt_Lot.Text;
                        CData.LotInfo.iLotOpenHour  = DateTime.Now.Hour; //20200827 lks  // 2020.09.11 JSKim Add
                        //CData.LotInfo.sToolId     = txt_LToolId.Text       ;  //190410 ksg : ASE secs/gem 용
                        CData.LotInfo.iTotalMgz     = cmb_Cnt.SelectedIndex + 1;
                        if (txt_StripCount.Text == "") txt_StripCount.Text = "1";
                        CData.LotInfo.iTotalStrip   = Convert.ToInt32(txt_StripCount.Text); //koo : Qorvo Lot Rework
                        CData.LotInfo.bLotOpen      = true;
                        CData.LotInfo.bLotEnd       = false;
                        CData.LotInfo.iTInCnt       = 0;
                        CData.LotInfo.iTOutCnt      = 0;

                        // 2021-06-17, jhLee : 최초 1회 LOT Verification 처리를 Skyworks 전용으로 변경
                        //                      Skyworks 외에 ASE-KR 및 SPIL에서 2번째 처리되는 LOT이 처리않되는 문제가 발생하여 Skyworks 전용으로 전환하였다.
                        // 2021.06.02. SungTae Start : [수정] 연속으로 LOT OPEN이 안되는 문제로 인해 ASE-KR에서는 아래 조건 SKIP.
                        if (CData.CurCompany == ECompany.SkyWorks)
                        {
                            CData.nLotVerifyCount = 0;          // 2021-01-18, jhLee : SECS/GEM의 Lot Validation Info를 통해 Lot을 Open하며, 중복된 Lot 정보 초기화를 막는다. 
                        }
                        // 2021.06.02. SungTae End

                        CData.LotInfo.i18PMeasuredCount = 0; //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                        CData.bWhlDrsLimitAlarmSkip = false; //200730 jhc : Wheel/Dresser Limit Alaram 보류용 (ASE-Kr VOC) - Lot 초기화 시 Wheel/Dresser Limit Alarm 보류 해제

                        CData.LotInfo.dtOpen        = DateTime.Now; //200116 jym : Lot open 시간 초기화
                        CData.LotInfo.dtEnd         = DateTime.Now; //200116 jym : Lot end 시간 초기화
                        CData.bLotEndShow           = false;
                        CData.bLotEndMsgShow        = false;  //190521 ksg :
                        
                        CData.SpcInfo.sStartTime    = "";
                        CData.SpcInfo.sEndTime      = "";
                        CData.SpcInfo.sIdleTime     = "";
                        CData.SpcInfo.sJamTime      = "";
                        CData.SpcInfo.sRunTime      = "";
                        CData.SpcInfo.sTotalTime    = "";
                        //CData.swLotTotal.Reset();
                        CData.bLotTotalReset        = true;
                        //CData.swLotIdle.Reset();
                        CData.bLotIdleReset         = true;
                        //CData.swLotJam.Reset();
                        CData.bLotJamReset          = true;
                        
                        //lotinfo 데이터 입력
                        CData.SpcInfo.sLotName      = CData.LotInfo.sLotName;
                        CData.SpcInfo.sDevice       = CData.Dev.sName;
                        CData.SpcInfo.sWhlSerial_L  = CData.Whls[0].sWhlName;
                        CData.SpcInfo.sWhlSerial_R  = CData.Whls[1].sWhlName;

                        CSq_OfL.It.m_GetQcWorkEnd   = false; //190408 ksg : Qc
						
						// 2021-01-?? YYY : for New QC-Vision
                        //CTcpIp.It.bResultGood = false; //190408 ksg : Qc
                        //CTcpIp.It.bResultFail = false; //190408 ksg : Qc
                        CQcVisionCom.rcvQCReadyQueryGOOD = CQcVisionCom.rcvQCReadyQueryNG = false;

                        CSQ_Main.It.m_bPause = false; //190506 ksg :

                        if (CDataOption.SecsUse == eSecsGem.Use)
                        {
                            if (CData.CurCompany == ECompany.ASE_KR)
                            {
                                CData.LotInfo.sToolId = txt_LToolId.Text;
                            } // ASE secs/gem 용
                            else if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK || CData.CurCompany == ECompany.JCET) //200121 ksg :,200625 lks
                            {
                                CData.LotInfo.sLToolId  = txt_LToolId.Text;
                                CData.LotInfo.sRToolId  = txt_RToolId.Text;
                                CData.LotInfo.sLDrsId   = txt_LMTId.Text;
                                CData.LotInfo.sRDrsId   = txt_RMTId.Text;
                            }
                        }
                        DialogResult = DialogResult.OK;
                    }
                    if(ckb_SetUpStripUse.Checked)
                    {//210309 pjh : Lot Open 시 Auto Load Stop 기능 활성화
                        CSQ_Main.It.m_bAutoLoadingStop  = true;
                        CSQ_Main.It.bSetUpStripStatus   = true;
                        CSQ_Main.It.bSetUpStripMsg      = false;
                        CSQ_Main.It.m_iLoadingCount     = 0;

                        CSQ_Man.It.bOnMGZPlaceDone      = false;
                        CSQ_Man.It.bOffMGZBtmPlaceDone  = false;
                        CSQ_Man.It.bOffMGZTopPlaceDone  = false;

                    }
                    else  // 2021-005-24, jhLee, Setup Strip 기능을 미사용 설정시 Option 값을 False해주는 부분이 없어서 보강
                    {
                        CSQ_Main.It.m_bAutoLoadingStop = false;
                        CSQ_Main.It.bSetUpStripStatus = false;          // Setup strip 기능 미사용
                    }
					//210726 pjh : Lot End 변수 초기화 조건 변경
                    CSQ_Main.It.m_iLoadingCount = 0;
                    CSQ_Man.It.bOnMGZPlaceDone = false;
                    CSQ_Man.It.bOffMGZBtmPlaceDone = false;
                    CSQ_Man.It.bOffMGZTopPlaceDone = false;
					//	
                }
                //btn_Open.Enabled = true;
            // 2020.10.29 JSKim St
            }
            finally
            {
                BeginInvoke(new Action(() => btn_Open.Enabled = true));
            }
            // 2020.10.29 JSKim Ed
        }

        private void btn_Can_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        public bool CheckStatus()
        {
            bool Ret = true;

            bool ChkLoadMgz     = CSq_OnL.It.Chk_ClampMgz ();
            bool ChkOffLoadMgz  = CSq_OfL.It.Get_BtmMgzChk();
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
		private void txt_StripCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            //koo : Qorvo Lot Rework
            //숫자만 입력되도록 필터링
            if(!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))    //숫자와 백스페이스를 제외한 나머지를 바로 처리
            {
                e.Handled = true;
            }
        }
        private void timer_DfServer_Tick(object sender, EventArgs e)
        {
            //20190618 ghk_dfserver
            if(m_bLotClick)
            {
                if(CDf.It.ReciveAck((int)ECMD.scLotOpen))
                {
                    if (CData.dfInfo.sGl == "GL1")
                    {
                        CData.LotInfo.sLotName   = txt_Lot    .Text;
                        CData.LotInfo.sToolId    = txt_LToolId.Text;
                        CData.LotInfo.iTotalMgz  = cmb_Cnt.SelectedIndex + 1;
                        CData.LotInfo.bLotOpen   = true;
                        CData.LotInfo.bLotEnd    = false;
                        CData.LotInfo.iTInCnt    = 0;
                        CData.LotInfo.iTOutCnt   = 0;

                        CData.LotInfo.i18PMeasuredCount = 0; //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                        CData.bWhlDrsLimitAlarmSkip = false; //200730 jhc : Wheel/Dresser Limit Alaram 보류용 (ASE-Kr VOC) - Lot 초기화 시 Wheel/Dresser Limit Alarm 보류 해제

                        CData.LotInfo.dtOpen        = DateTime.Now; //200116 jym : Lot open 시간 초기화
                        CData.LotInfo.dtEnd         = DateTime.Now; //200116 jym : Lot end 시간 초기화
                        CData.bLotEndShow           = false;
                        CData.bLotEndMsgShow        = false;
                        CData.SpcInfo.sStartTime    = "";
                        CData.SpcInfo.sEndTime      = "";
                        CData.SpcInfo.sIdleTime     = "";
                        CData.SpcInfo.sJamTime      = "";
                        CData.SpcInfo.sRunTime      = "";
                        CData.SpcInfo.sTotalTime    = "";
                        CData.bLotTotalReset        = true;
                        CData.bLotIdleReset         = true;
                        CData.bLotJamReset          = true;
                        //lotinfo 데이터 입력
                        CData.SpcInfo.sLotName      = CData.LotInfo.sLotName;
                        CData.SpcInfo.sDevice       = CData.Dev.sName;
                        CData.SpcInfo.sWhlSerial_L  = CData.Whls[0].sWhlName;
                        CData.SpcInfo.sWhlSerial_R  = CData.Whls[1].sWhlName;
                        CSq_OfL.It.m_GetQcWorkEnd   = false;
						
						// 2021-01-?? YYY : for New QC-Vision
                        //CTcpIp.It.bResultGood      = false;
                        //CTcpIp.It.bResultFail      = false;
                        CQcVisionCom.rcvQCReadyQueryGOOD = CQcVisionCom.rcvQCReadyQueryNG = false;

                        CSQ_Main.It.m_bPause        = false;
                        
                        m_bLotClick             = false;
                        timer_DfServer.Enabled  = false;
                        DialogResult            = DialogResult.OK;                        
                    }
                    else
                    {
                        CDf.It.SendLotExist();
                        m_bLotClick = false;
                        m_bLotOp = true;
                        return;
                    }
                }
            }
            if(m_bLotOp)
            {
                if(CDf.It.ReciveAck((int)ECMD.scLotExist))
                {
                    CData.LotInfo.sLotName     = txt_Lot.Text;
                    CData.LotInfo.sToolId      = txt_LToolId.Text;
                    CData.LotInfo.iTotalMgz    = cmb_Cnt.SelectedIndex + 1;
                    CData.LotInfo.bLotOpen     = true;
                    CData.LotInfo.bLotEnd      = false;
                    CData.LotInfo.iTInCnt      = 0;
                    CData.LotInfo.iTOutCnt     = 0;

                    CData.LotInfo.i18PMeasuredCount = 0; //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                    CData.bWhlDrsLimitAlarmSkip = false; //200730 jhc : Wheel/Dresser Limit Alaram 보류용 (ASE-Kr VOC) - Lot 초기화 시 Wheel/Dresser Limit Alarm 보류 해제

                    CData.LotInfo.dtOpen        = DateTime.Now; //200116 jym : Lot open 시간 초기화
                    CData.LotInfo.dtEnd         = DateTime.Now; //200116 jym : Lot end 시간 초기화
                    CData.bLotEndShow           = false;
                    CData.bLotEndMsgShow        = false;
                    CData.SpcInfo.sStartTime    = "";
                    CData.SpcInfo.sEndTime      = "";
                    CData.SpcInfo.sIdleTime     = "";
                    CData.SpcInfo.sJamTime      = "";
                    CData.SpcInfo.sRunTime      = "";
                    CData.SpcInfo.sTotalTime    = "";
                    CData.bLotTotalReset        = true;
                    CData.bLotIdleReset         = true;
                    CData.bLotJamReset          = true;
                    //lotinfo 데이터 입력
                    CData.SpcInfo.sLotName      = CData.LotInfo.sLotName;
                    CData.SpcInfo.sDevice       = CData.Dev.sName;
                    CData.SpcInfo.sWhlSerial_L  = CData.Whls[0].sWhlName;
                    CData.SpcInfo.sWhlSerial_R  = CData.Whls[1].sWhlName;
                    CSq_OfL.It.m_GetQcWorkEnd   = false;
					
					// 2021-01-?? YYY : for New QC-Vision
                    //CTcpIp.It.bResultGood      = false;
                    //CTcpIp.It.bResultFail      = false;
                    CQcVisionCom.rcvQCReadyQueryGOOD = CQcVisionCom.rcvQCReadyQueryNG = false; //
					
                    CSQ_Main.It.m_bPause       = false;

                    m_bLotOp                = false;
                    timer_DfServer.Enabled  = false;
                    DialogResult            = DialogResult.OK;
                }
                else
                {
                    m_bLotOp = false;
                    timer_DfServer.Enabled = false;
                    CMsg.Show(eMsg.Error, "Error", "No GL1 file on sever!");
                }
            }
        }



        private void btnTId_L_Click(object sender, EventArgs e)
        {
            if (CData.GemForm == null) return;
            string sToolId = string.Empty;
            uint nToolLoc = 1;

            if (txt_LToolId.Text == string.Empty)
            {
                CMsg.Show(eMsg.Error, "Error", "Please input ID.");

                return;
            }

            sToolId = txt_LToolId.Text;
            string sSerial_Num = nToolLoc + "Serial_Num";
            CData.GemForm.OnToolVerifyRequest(sToolId, nToolLoc);
        }

        private void btnTId_R_Click(object sender, EventArgs e)
        {
            if (CData.GemForm == null) return;
            string sToolId = string.Empty;
            uint nToolLoc = 2;

            if (txt_RToolId.Text == string.Empty)
            {
                CMsg.Show(eMsg.Error, "Error", "Please input ID.");

                return;
            }

            sToolId = txt_RToolId.Text;
            string sSerial_Num = nToolLoc + "Serial_Num";
            CData.GemForm.OnToolVerifyRequest(sToolId, nToolLoc);
        }

        private void btnMId_L_Click(object sender, EventArgs e)
        {
            string sMatId = string.Empty;
            uint nMatLoc = 1;

            if (txt_LMTId.Text == string.Empty)
            {
                CMsg.Show(eMsg.Error, "Error", "Please input ID.");
                return;
            }
            sMatId = txt_LMTId.Text;
            if (CData.GemForm != null) CData.GemForm.OnMatVerifyRequest(sMatId, nMatLoc);
        }

        private void btnMId_R_Click(object sender, EventArgs e)
        {
            string sMatId = string.Empty;
            uint nMatLoc = 2;

            if (txt_RMTId.Text == string.Empty)
            {
                CMsg.Show(eMsg.Error, "Error", "Please input ID.");

                return;
            }
            sMatId = txt_RMTId.Text;
            if (CData.GemForm != null) CData.GemForm.OnMatVerifyRequest(sMatId, nMatLoc);
        }
    }
}
