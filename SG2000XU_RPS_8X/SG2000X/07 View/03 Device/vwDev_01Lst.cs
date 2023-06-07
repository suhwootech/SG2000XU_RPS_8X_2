using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.VisualBasic.FileIO;
using System.IO;
using System.Threading;     // for Sleep

namespace SG2000X
{
    public partial class vwDev_01Lst : UserControl
    {
        public delegate void ListEvt(bool bVal);
        public event ListEvt GoParm;
        private string m_sGrp;
        private string m_sDev;
        private CTim m_Delay = new CTim();

        public vwDev_01Lst()
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
        /// 데이터를 화면에 출력
        /// </summary>
        private void _Set()
        {

        }

        /// <summary>
        /// 화면에 값을 데이터로 저장
        /// </summary>
        private void _Get()
        {

        }

        private void _GrpListUp()
        {
            int iCnt = 0;
            DirectoryInfo[] aVal;

            if (!Directory.Exists(GV.PATH_DEVICE))
            { Directory.CreateDirectory(GV.PATH_DEVICE); }

            lbxM_Grp.Items.Clear();
            DirectoryInfo mFile = new DirectoryInfo(GV.PATH_DEVICE);
            aVal = mFile.GetDirectories();
            iCnt = aVal.Length;
            for (int i = 0; i < iCnt; i++)
            {
                lbxM_Grp.Items.Add(aVal[i].Name);
            }

            lbxM_Grp.ClearSelected();
            lbxM_Dev.ClearSelected();
            lbxM_Dev.Items.Clear();
            m_sGrp = "";
            m_sDev = "";
        }

        private void _RcpListUp()
        {
            string sPath = GV.PATH_DEVICE + m_sGrp + "\\";

            if (Directory.Exists(sPath) == true)
            {
                lbxM_Dev.Items.Clear();
                DirectoryInfo mFile = new DirectoryInfo(sPath);
                foreach (FileSystemInfo mInfo in mFile.GetFileSystemInfos("*.dev"))
                {
                    lbxM_Dev.Items.Add(mInfo.Name.Replace(".dev", ""));
                }
            }

            lbxM_Dev.ClearSelected();
            m_sDev = "";
            btnM_DApp.Enabled = false;
        }

        private void _HideMenu()
        {
            ELv RetLv = CData.Lev;
            btnM_GNew.Enabled = (int)RetLv >= CData.Opt.iLvGpNew;
            btnM_GCopy.Enabled = (int)RetLv >= CData.Opt.iLvGpSaveAs;
            btnM_GDel.Enabled = (int)RetLv >= CData.Opt.iLvGpDel;

            if (RetLv == ELv.Master && CData.LotInfo.bLotOpen)
            { btnM_DCur.Visible = true; }
            else if (RetLv > ELv.Operator && !CData.LotInfo.bLotOpen)
            { btnM_DCur.Visible = true; }
            else if (RetLv == ELv.Operator)
            { btnM_DCur.Visible = false; }
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
        #endregion

        #region Public method
        /// <summary>
        /// View가 화면에 그려질때 실행해야 하는 함수
        /// </summary>
        public void Open()
        {
            _HideMenu();

            _GrpListUp();

            btnM_DApp.Enabled = false;
            btnM_DCopy.Enabled = false;
            btnM_DDel.Enabled = false;
            btnM_DBcr .Enabled = false; //190922 ksg :

            if (CData.CurCompany == ECompany.Qorvo    ||
                CData.CurCompany == ECompany.Qorvo_DZ ||
                CData.CurCompany == ECompany.Qorvo_RT ||        // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                CData.CurCompany == ECompany.Qorvo_NC ||        // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                CData.CurCompany == ECompany.SST      ||    
                CData.CurCompany == ECompany.ASE_KR   ||  //200107 syc :
                CData.CurCompany == ECompany.JCET     )   // 2021.01.14 lhs: JCET 추가
            {
                btnM_DBcr.Visible = true; //190922 ksg :             
            }
            else
            {
                btnM_DBcr.Visible = false; //190922 ksg :
            }

             if (CData.LotInfo.bLotOpen)
            {//랏 오픈 일 경우
                if (CData.Lev == ELv.Master)
                {//마스터 일 경우
                    btnM_DCur.Visible = true;
                }
                else
                {//마스터 아닐 경우
                    btnM_DCur.Visible = false;
                }
            }
            else
            {//랏 오픈 아닐 경우
                if ((int)CData.Lev >= CData.Opt.iLvDvCurrent)
                {//옵션 설정 레벨 보다 큰 경우
                    btnM_DCur.Visible = true;
                }
                else
                {//옵션 설정 레벨 보다 작은 경우
                    btnM_DCur.Visible = false;
                }
            }
        }

        /// <summary>
        /// View가 화면에서 지워질때 실행해야 하는 함수
        /// </summary>
        public void Close()
        {
        }
        #endregion

        // Group folder new button
        private void btnM_GNew_Click(object sender, EventArgs e)
        {
            Button mBtn = sender as Button;
            BeginInvoke(new Action(() => mBtn.Enabled = false));

            string sPath = GV.PATH_DEVICE;

            try
            {
                using (frmTxt mForm = new frmTxt("New Group Name"))
                {
                    if (mForm.ShowDialog() == DialogResult.Cancel) { return; }

                    // 2021.08.10 lhs Start : 잘못된 문자가 있는지 체크
                    string sText = mForm.Val.ToString();
                    int nIdx = sText.IndexOfAny(Path.GetInvalidFileNameChars());
                    if (nIdx != -1) // -1이면 발견되지 않음.
                    {
                        CMsg.Show(eMsg.Warning, "warning", "Invalid characters such as \", <, >, or | ");
                        return;
                    }
                    // 2021.08.10 lhs End

                    if (Directory.Exists(sPath + mForm.Val))
                    {
                        CMsg.Show(eMsg.Error, "Error", "Group name exist !!!");
                        return;
                    }
                    sPath = sPath + mForm.Val; //190130 ksg : Device에 New가 되지 않아 수정
                    Directory.CreateDirectory(sPath);
                    _GrpListUp();
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                BeginInvoke(new Action(() => mBtn.Enabled = true));
            }
            CData.bLastClick = true; //190509 ksg :
        }

        // Group folder copy button
        private void btnM_GCopy_Click(object sender, EventArgs e)
        {
             Button mBtn = sender as Button;
            BeginInvoke(new Action(() => mBtn.Enabled = false));

            string sSrc = "";
            string sDst = "";

            try
            {
                // 그룹 선택 여부 확인
                if (lbxM_Grp.SelectedIndex < 0 || m_sGrp == "")
                {
                    CMsg.Show(eMsg.Error, "Error", "Not selected group");
                    return;
                }

                using (frmTxt mForm = new frmTxt("Save As Group Name"))
                {
                    if (mForm.ShowDialog() == DialogResult.OK)
                    {
                        sSrc = GV.PATH_DEVICE + lbxM_Grp.SelectedItem.ToString();
                        sDst = GV.PATH_DEVICE + mForm.Val;

                        // 2021.08.10 lhs Start : 잘못된 문자가 있는지 체크
                        string sText = mForm.Val.ToString();
                        int nIdx = sText.IndexOfAny(Path.GetInvalidFileNameChars());
                        if (nIdx != -1) // -1이면 발견되지 않음.
                        {
                            CMsg.Show(eMsg.Warning, "warning", "Invalid characters such as \", <, >, or | ");
                            return;
                        }
                        // 2021.08.10 lhs End

                        if (Directory.Exists(GV.PATH_DEVICE + mForm.Val))
                        {
                            CMsg.Show(eMsg.Error, "Error", "Group name exist !!!");
                            return;
                        }

                        FileSystem.CopyDirectory(sSrc, sDst, UIOption.AllDialogs);
                        CCheckChange.SaveAs("DEVICE", sSrc, sDst); //200716 lks
                        _GrpListUp();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                BeginInvoke(new Action(() => mBtn.Enabled = true));
            }

            CData.bLastClick = true; //190509 ksg :
        }

        // Group folder delete button
        private void btnM_GDel_Click(object sender, EventArgs e)
        {
            Button mBtn = sender as Button;
            BeginInvoke(new Action(() => mBtn.Enabled = false));

            try
            {
                // 그룹 선택 여부 확인
                if (lbxM_Grp.SelectedIndex < 0 || m_sGrp == "")
                {
                    CMsg.Show(eMsg.Error, "Error", "Not selected group");
                    return;
                }

                if (CMsg.Show(eMsg.Warning, "Warning", "Do you want delete " + m_sGrp + " group?") == DialogResult.Cancel) { return; }                

                FileSystem.DeleteDirectory(GV.PATH_DEVICE + m_sGrp, UIOption.AllDialogs, RecycleOption.SendToRecycleBin);

                CCheckChange.DeleteLog("DEVICE", GV.PATH_DEVICE + m_sGrp); //200716 lks

                _GrpListUp();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                BeginInvoke(new Action(() => mBtn.Enabled = true));
            }

            CData.bLastClick = true; //190509 ksg :
        }

        // Receipe file new button
        private void btnM_RNew_Click(object sender, EventArgs e)
        {
            Button mBtn = sender as Button;
            BeginInvoke(new Action(() => mBtn.Enabled = false));

            string sPath = GV.PATH_DEVICE;

            try
            {
                // 그룹 선택 여부 확인
                if (lbxM_Grp.SelectedIndex < 0 || m_sGrp == "")
                {
                    CMsg.Show(eMsg.Error, "Error", "Not selected group");
                    return;
                }

                sPath += m_sGrp + "\\";

                using (frmTxt mForm = new frmTxt("New Device Name"))
                {
                    if (mForm.ShowDialog() == DialogResult.Cancel) { return; }

                    // 2021.08.10 lhs Start : 잘못된 문자가 있는지 체크
                    string sText = mForm.Val.ToString();
                    int nIdx = sText.IndexOfAny(Path.GetInvalidFileNameChars());
                    if (nIdx != -1) // -1이면 발견되지 않음.
                    {
                        CMsg.Show(eMsg.Warning, "warning", "Invalid characters such as \", <, >, or | ");
                        return;
                    }
                    // 2021.08.10 lhs End

                    if (File.Exists(sPath + mForm.Val + ".dev"))
                    {
                        CMsg.Show(eMsg.Error, "Error", "Device name exist !!!");
                        return;
                    }

                    sPath += mForm.Val + ".dev";
                    tDev tRcp;
                    CDev.It.InitDev(out tRcp);
                    tRcp.sName = mForm.Val;
                    CDev.It.Save(sPath, tRcp);

                    //20190624 josh
                    //jsck secsgem
                    //CEID 999021 recipe created
                    if (CData.GemForm != null)
                    {
                        if (m_sGrp == "GEM" || CData.GemForm.UseRecipeFolder)
                        {
                            CData.GemForm.Set_PPIDA(m_sGrp + "\\"+tRcp.sName);
                        }
                    }

                    _RcpListUp();

                    CMsg.Show(eMsg.Notice, "Notice", "Device file create success");
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                BeginInvoke(new Action(() => mBtn.Enabled = true));
            }

            CData.bLastClick = true; //190509 ksg :
        }

        // Receipe file copy button
        private void btnM_RCopy_Click(object sender, EventArgs e)
        {
             Button mBtn = sender as Button;
            BeginInvoke(new Action(() => mBtn.Enabled = false));

            string sSrc = "";
            string sDst = "";

            try
            {
                if (lbxM_Grp.SelectedIndex < 0 || m_sGrp == "")
                {
                    CMsg.Show(eMsg.Error, "Error", "Not selected group");
                    return;
                }

                if (lbxM_Dev.SelectedIndex < 0 || m_sDev == "")
                {
                    CMsg.Show(eMsg.Error, "Error", "Not selected device");
                    return;
                }

                using (frmTxt mForm = new frmTxt("Save As Device Name"))
                {
                    if (mForm.ShowDialog() == DialogResult.OK)
                    {
                        sSrc = GV.PATH_DEVICE + m_sGrp + "\\" + m_sDev + ".dev";
                        sDst = GV.PATH_DEVICE + m_sGrp + "\\" + mForm.Val + ".dev";

                        // 2021.08.10 lhs Start : 잘못된 문자가 있는지 체크
                        string sText = mForm.Val.ToString();
                        int nIdx = sText.IndexOfAny(Path.GetInvalidFileNameChars());
                        if (nIdx != -1) // -1이면 발견되지 않음.
                        {
                            CMsg.Show(eMsg.Warning, "warning", "Invalid characters such as \", <, >, or | ");
                            return;
                        }
                        // 2021.08.10 lhs End

                        // 원본과 동일한 이름인지 판단
                        if (mForm.Val == m_sDev)
                        {
                            CMsg.Show(eMsg.Error, "Error", "Device name same !!!");
                            return;
                        }

                        // 동일한 이름 존재하는지 판단
                        if (File.Exists(GV.PATH_DEVICE+ m_sGrp+ "\\"+ mForm.Val + ".dev")) //190627 josh : 
                        {
                            CMsg.Show(eMsg.Error, "Error", "Device name exist !!!");
                            return;
                        }

                        FileSystem.CopyFile(sSrc, sDst, UIOption.AllDialogs);

                        // 2021.08.10 lhs Start : SaveAs시 파일을 그대로 복사하여 다른이름으로 저장하므로 [Information]의 Name을 바꿔줘야 함.
                        if (!File.Exists(sDst))
                        {
                            CMsg.Show(eMsg.Error, "Error", "File not exist !!!");
                            return;
                        }

                        CIni mIni = new CIni(sDst);
                        string sSc = "Information";
                        
                        mIni.Write(sSc, "Name", mForm.Val);
                        // 2021.08.10 lhs End

                        CCheckChange.SaveAs("DEVICE", sSrc, sDst); //200716 lks

                        //20190624 josh
                        //jsck secsgem
                        //CEID 999021 recipe created
                        if (CData.GemForm != null)
                        {
                            if (m_sGrp == "GEM" || CData.GemForm.UseRecipeFolder)
                            {
                                CData.GemForm.Set_PPIDA(m_sGrp + "\\" + mForm.Val);
                            }
                        }

                        //if (m_sGrp == "GEM")
                        //{
                        //    if(CData.GemForm != null)CData.GemForm.Set_PPIDA(m_sDev);
                        //}

                        _RcpListUp();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                BeginInvoke(new Action(() => mBtn.Enabled = true));
            }

            CData.bLastClick = true; //190509 ksg :
        }

        // Receipe file delete button
        private void btnM_RDel_Click(object sender, EventArgs e)
        {
            Button mBtn = sender as Button;
            BeginInvoke(new Action(() => mBtn.Enabled = false));

            string sPath = GV.PATH_DEVICE;

            try
            {
                if (lbxM_Grp.SelectedIndex < 0 || m_sGrp == "")
                {
                    CMsg.Show(eMsg.Error, "Error", "Not selected group");
                    return;
                }
                sPath += m_sGrp + "\\";

                if (lbxM_Dev.SelectedIndex < 0 || m_sDev == "")
                {
                    CMsg.Show(eMsg.Error, "Error", "Not selected device");
                    return;
                }
                sPath += m_sDev + ".dev";

                if (CMsg.Show(eMsg.Warning, "Warning", "Do you want delete " + m_sDev + " device?") == DialogResult.Cancel) { return; }

                CCheckChange.DeleteLog("DEVICE", sPath); //200716 lks
                FileSystem.DeleteFile(sPath);

                //20190624 josh
                //jsck secsgem
                //CEID 999024 recipe deleted
                if (CData.GemForm != null)
                {
                    if (m_sGrp == "GEM" || CData.GemForm.UseRecipeFolder)
                    {
                        CData.GemForm.Set_PPIDD(m_sGrp + "\\" + m_sDev);
                    }
                }

                //if (m_sGrp == "GEM")
                //{
                //    if(CData.GemForm != null)CData.GemForm.Set_PPIDD(m_sDev);
                //}

                _RcpListUp();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                BeginInvoke(new Action(() => mBtn.Enabled = true));
            }

            CData.bLastClick = true; //190509 ksg :
        }

        // Receipe file apply button
        private void btnM_RApp_Click(object sender, EventArgs e)
        {
            Button mBtn = sender as Button;
            BeginInvoke(new Action(() => mBtn.Enabled = false));

            string sPath   = GV.PATH_DEVICE + m_sGrp + "\\";
            string sRecipe = m_sGrp + "\\";
            bool bSendToQC = false;                         // QC-Vision 쪽으로 JobChange 명령을 전송하였나 ?

            //210928 pjh : 이전 Data 저장
            if (CData.CurCompany == ECompany.Qorvo)
            {
                CDev.It.SavePreData();
            }
            //

            try
            {
                if (lbxM_Dev.SelectedIndex < 0 || m_sDev == "")
                {
                    CMsg.Show(eMsg.Error, "Error", "Not selected device");
                    return;
                }
                sPath = sPath + m_sDev + ".dev";

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
                        while (DateTime.Now < dtOut)
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
                            CMsg.Show(eMsg.Error, "Error", "Batch file not running(Timeout).");
                            return;
                        }
                    }

                    //
                    // 2021-01-28, jhLee : 새로운 Device 적용시 QC에게도 JobChange를 전송하여 변경을 할 수 있도록 한다.
                    //
                    if ( CData.Opt.bQcUse && CDataOption.UseQC)     // QC-Vision 사용 모드인가 ?
                    {
                        // 만약 QC와의 연결이 되어있지 않다면 연결을 시도한다.
                        if (!CGxSocket.It.IsConnected())
                        {
                            CGxSocket.It.Connect();
                            m_Delay.Wait(1000);                             // 1초간만 대기
                        }

                        CQcVisionCom.rcvQCJobChangeOK = false;
                        CQcVisionCom.rcvQCJobChangeNG = false;

                        // 연결이 되었으면 JobChange을 요청한다.
                        if (CGxSocket.It.IsConnected())
                        {
                            CGxSocket.It.SendChageDevice(m_sGrp, m_sDev);    // QC에게 Job Change 명령을 전송한다.
                            bSendToQC = true;                                // QC-Vision 쪽으로 JobChange 명령을 전송하였다.
                        }
                    }
                    //CDev.It.sPreWhlL = CData.Dev.aData[(int)EWay.L].sSelWhl;
                    //CDev.It.sPreWhlR = CData.Dev.aData[(int)EWay.R].sSelWhl;
                }
                // 201110 jym END

                // 2022.02.24 SungTae Start : [추가] (ASE-KR VOC) Device Change 시 Strip Cleanning Issue 관련 Off-picker Z축 Position 확인 위해 Log 추가
                if (CData.CurCompany == ECompany.ASE_KR)
                {
                    _SetLog($"============[Before Device Information]============");
                    _SetLog($"Device Name           : {CData.Dev.sName}");
                    _SetLog($"Bottom Clean Position : {CData.Dev.dOffP_Z_ClnStart} mm");
                    _SetLog($"Strip Clean Position  : {CData.Dev.dOffP_Z_StripClnStart} mm");
                    _SetLog($"===================================================");
                }
                // 2022.02.24 SungTae End

                CData.DevCur = sPath;
                CDev.It.Load(sPath, true);
                //190211 ksg :
                sRecipe = sRecipe + m_sDev;
                CBcr.It.SaveRecipe(sRecipe);
                CDev.It.m_sGrp = m_sGrp;
                CData.DevGr = CDev.It.m_sGrp; //20200619 jhc : 그룹명 변수 값이 바뀌어, 디바이스 파일 저장 폴더가 바뀌는 현상 개선
                
                // 200803 jym
                if (CData.CurCompany == ECompany.ASE_KR && GV.DEV_WHL_SYNC)
                {
                    int iWy = (int)EWay.L;
                    CWhl.It.Load(EWay.L, CData.Dev.aData[iWy].sWhl, out CData.Whls[iWy]);
                    iWy = (int)EWay.R;
                    CWhl.It.Load(EWay.R, CData.Dev.aData[iWy].sWhl, out CData.Whls[iWy]);
                }

                CData.L_GRD.m_aInspBf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //메뉴얼
                CData.L_GRD.m_aInspAf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //메뉴얼
                CData.L_GRD.m_abMeaBfErr = new bool[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //
                CData.L_GRD.m_abMeaAfErr = new bool[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol];
                CData.L_GRD.m_aInspTemp = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol];
                CData.GrData[0].aMeaBf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //Auto
                CData.GrData[0].aMeaAf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //Auto

                CData.R_GRD.m_aInspBf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //메뉴얼
                CData.R_GRD.m_aInspAf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //메뉴얼
                CData.R_GRD.m_abMeaBfErr = new bool[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //
                CData.R_GRD.m_abMeaAfErr = new bool[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol];
                CData.R_GRD.m_aInspTemp = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol];
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

                if (!CData.LotInfo.bLotOpen) CData.Parts[(int)EPart.ONL].iSlot_info = new int[CData.Dev.iMgzCnt];

                // 2020.10.26 SungTae Start : Modify by Qorbo(Strip 배출 관련)
                //if (!CData.LotInfo.bLotOpen) CData.Parts[(int)EPart.OFL].iSlot_info = new int[CData.Dev.iMgzCnt];
                if (!CData.LotInfo.bLotOpen)
                {
                    // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                    //if((CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ) &&
                    if((CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                        CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC) &&
                            !CData.Opt.bOfLMgzMatchingOn && CData.Dev.iOffMgzCnt != 0)
                    {
#if true //201116 jhc : DEBUG
                        int nOfLSlotCount = Math.Max(CData.Dev.iOffMgzCnt, CData.Dev.iMgzCnt);
                        CData.Parts[(int)EPart.OFL].iSlot_info = new int[nOfLSlotCount];
#else
                        CData.Parts[(int)EPart.OFL].iSlot_info = new int[CData.Dev.iOffMgzCnt];
#endif
                    }
                    else
                    {
                        CData.Parts[(int)EPart.OFL].iSlot_info = new int[CData.Dev.iMgzCnt];
                    }
                }
                // 2020.10.26 SungTae End

                //20190624 ghk_qc
                //if(CData.CurCompany == eCompany.SkyWorks)
                if (CData.Opt.bQcUse)
                {
                    if (!CData.LotInfo.bLotOpen) CData.Parts[(int)EPart.OFR].iSlot_info = new int[CData.Dev.iMgzCnt];  //190325 ksg : Qc
                }

                //CEID 999022 Recipe Loaded
                CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Recipe_Name = sRecipe;
                if ((CData.Opt.bSecsUse == true) && (CData.GemForm != null))// 20200430 LCY
                {
                   // CData.GemForm.Set_PPIDC(m_sDev);
                    CData.GemForm.Set_PPIDC(CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Recipe_Name);
                }


                //
                // 2021-01-28, jhLee : Skyworks, VOC 새로운 Device 적용시 QC의 JobChange 정상 종료 여부를 기다린다.
                //
                if (CData.Opt.bQcUse && (CData.CurCompany == ECompany.SkyWorks))     // QC-Vision 사용 모드인가 ?
                {
                    if (bSendToQC)                                // QC-Vision 쪽으로 JobChange 명령을 전송하였다면 응답을 기다린다.
                    {
                        m_Delay.Set_Delay(2000);                    // 완료 응답을 2초간만 기다려본다.

                        // JobChange에 대한 응답이 OK/NG를 불구하고 왔다면 기다리지 않는다.
                        while (!(CQcVisionCom.rcvQCJobChangeOK || CQcVisionCom.rcvQCJobChangeNG))
                        {
                            // 지정한 시간이 지났으면 기다림을 포기한다.
                            if (m_Delay.Chk_Delay() == true)
                            {
                                break;
                            }

                            Thread.Sleep(10);           // 시간 Delay
                        }
                    } 

                    // QC의 Job Change가 정상적으로 수행되지 못하였다.
                    if (!CQcVisionCom.rcvQCJobChangeOK)
                    {
                        CMsg.Show(eMsg.Warning, "warning", "Qc Vision Can't Change Device File");
                    }
                }//of if Skyworks && QC Use by jhLee

                //211115 pjh : Skyworks Issue로 인한 주석 처리
                //
                //if(CData.CurCompany == ECompany.SkyWorks)
                //{
                //    CData.Whls[0].sWhlName = CData.Dev.aData[(int)EWay.L].sSelWhl;
                //    CData.Whls[1].sWhlName = CData.Dev.aData[(int)EWay.R].sSelWhl;
                //}
                //

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"* btnM_RApp_Click() Exception rise : {ex.Message}");
            }
            finally { BeginInvoke(new Action(() => mBtn.Enabled = true)); }

            GoParm(true);

            CData.bLastClick = true; //190509 ksg :

            //210928 pjh : Grinding Mode 교체 Message 표시
            if (CData.CurCompany == ECompany.Qorvo && !CDev.It.CompareGrdMod(sPath))
            {
                CMsg.Show(eMsg.Notice, "Notice", "Grinding Mode is changed" + CDev.It.sChangedMode);
            }
            //
        }

        private void btnM_DBcr_Click(object sender, EventArgs e)
        {
            Button mBtn = sender as Button;
            BeginInvoke(new Action(() => mBtn.Enabled = false));

            string sPath   = GV.PATH_DEVICE + m_sGrp + "\\";
            string sRecipe = m_sGrp + "\\";

            try
            {
                if (lbxM_Grp.SelectedIndex < 0 || m_sGrp == "")
                {
                    CMsg.Show(eMsg.Error, "Error", "Not selected group");
                    return;
                }

                using (frmTxt mForm = new frmTxt("Input barcode read"))
                {
                    if (mForm.ShowDialog() == DialogResult.OK)
                    {
                        // 동일한 이름 존재하는지 판단
                        if (!File.Exists(GV.PATH_DEVICE+ m_sGrp+ "\\"+ mForm.Val + ".dev")) //190627 josh : 
                        {
                            CMsg.Show(eMsg.Error, "Error", "Device name not exist !!!");
                            return;
                        }

                        sPath = sPath + mForm.Val + ".dev";
                        CData.DevCur = sPath;
                        CDev.It.Load(sPath, true);
                        //190211 ksg :
                        sRecipe = sRecipe + mForm.Val;
                        CBcr.It.SaveRecipe(sRecipe);
                        CDev.It.m_sGrp = m_sGrp;
                        CData.DevGr = CDev.It.m_sGrp; //20200619 jhc : 그룹명 변수 값이 바뀌어, 디바이스 파일 저장 폴더가 바뀌는 현상 개선

                        CData.L_GRD.m_aInspBf    = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //메뉴얼
                        CData.L_GRD.m_aInspAf    = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //메뉴얼
                        CData.L_GRD.m_abMeaBfErr = new bool  [CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //
                        CData.L_GRD.m_abMeaAfErr = new bool  [CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol];
                        CData.L_GRD.m_aInspTemp  = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol];
                        CData.GrData[0].aMeaBf   = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //Auto
                        CData.GrData[0].aMeaAf   = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //Auto

                        CData.R_GRD.m_aInspBf    = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //메뉴얼
                        CData.R_GRD.m_aInspAf    = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //메뉴얼
                        CData.R_GRD.m_abMeaBfErr = new bool  [CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //
                        CData.R_GRD.m_abMeaAfErr = new bool  [CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol];
                        CData.R_GRD.m_aInspTemp  = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol];
                        CData.GrData[1].aMeaBf   = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //Auto
                        CData.GrData[1].aMeaAf   = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //Auto

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

                        if (!CData.LotInfo.bLotOpen) CData.Parts[(int)EPart.ONL].iSlot_info = new int[CData.Dev.iMgzCnt];

                        // 2020.10.26 SungTae Start : Modify by Qorbo(Strip 배출 관련)
                        //if (!CData.LotInfo.bLotOpen) CData.Parts[(int)EPart.OFL].iSlot_info = new int[CData.Dev.iMgzCnt];
                        if (!CData.LotInfo.bLotOpen)
                        {
                            // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                            //if((CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ) &&
                            if((CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                                CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC) &&
                                !CData.Opt.bOfLMgzMatchingOn && CData.Dev.iOffMgzCnt != 0)
                            {
#if true //201116 jhc : DEBUG
                                int nOfLSlotCount = Math.Max(CData.Dev.iOffMgzCnt, CData.Dev.iMgzCnt);
                                CData.Parts[(int)EPart.OFL].iSlot_info = new int[nOfLSlotCount];
#else
                                CData.Parts[(int)EPart.OFL].iSlot_info = new int[CData.Dev.iOffMgzCnt];
#endif
                            }
                            else
                            {
                                CData.Parts[(int)EPart.OFL].iSlot_info = new int[CData.Dev.iMgzCnt];
                            }
                        }
                        // 2020.10.26 SungTae End

                        //20190624 ghk_qc
                        //if(CData.CurCompany == eCompany.SkyWorks)
                        if (CData.Opt.bQcUse)
                        {
                            if (!CData.LotInfo.bLotOpen) CData.Parts[(int)EPart.OFR].iSlot_info = new int[CData.Dev.iMgzCnt];  //190325 ksg : Qc
                        }
                        
                        CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Recipe_Name = sRecipe;
                        
                        if ((CData.Opt.bSecsUse == true) && (CData.GemForm != null))// 20200703 LCY
                        {
                            // CData.GemForm.Set_PPIDC(m_sDev);
                            CData.GemForm.Set_PPIDC(CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Recipe_Name);
                        }


                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                BeginInvoke(new Action(() => mBtn.Enabled = true));
            }

            GoParm(true);

            CData.bLastClick = true; //190509 ksg :
        }

        private void lbx_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) { return; }

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                e = new DrawItemEventArgs(e.Graphics, e.Font, e.Bounds, e.Index, e.State ^ DrawItemState.Selected, e.ForeColor, Color.FromArgb(41, 199, 201));
            }

            ListBox mLbx = sender as ListBox;
            int iX = e.Bounds.X + (e.Font.Height / 2);
            int iY = e.Bounds.Y + (e.Font.Height / 2);

            e.DrawBackground();
            e.Graphics.DrawString(mLbx.Items[e.Index].ToString(), e.Font, Brushes.Black, iX, iY, StringFormat.GenericDefault);
            e.DrawFocusRectangle();
        }

        private void lbxM_Grp_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbxM_Grp.SelectedIndex >= 0)
            {
                m_sGrp = lbxM_Grp.SelectedItem.ToString();

                _RcpListUp();
            }
            if (CData.CurCompany == ECompany.Qorvo    || 
                CData.CurCompany == ECompany.Qorvo_DZ ||
                CData.CurCompany == ECompany.Qorvo_RT ||        // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                CData.CurCompany == ECompany.Qorvo_NC ||        // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가 
                CData.CurCompany == ECompany.SST      || 
                CData.CurCompany == ECompany.ASE_KR   || //200107 syc :
                CData.CurCompany == ECompany.JCET     )  // 2021.01.14 lhs: JCET 추가
            {
                btnM_DBcr.Enabled = true;
            }
        }

        private void libM_Rcp_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool bEn = false;
            if (lbxM_Dev.SelectedIndex >= 0)
            {
                bEn = true;
                m_sDev = lbxM_Dev.SelectedItem.ToString();
            }
            if (CData.LotInfo.bLotOpen)
            {
                btnM_DNew.Enabled = (!bEn) && ((int)CData.Lev >= CData.Opt.iLvDvNew);
                btnM_DApp.Enabled = (!bEn) && ((int)CData.Lev >= CData.Opt.iLvDvLoad);
                btnM_DCopy.Enabled = (!bEn) && ((int)CData.Lev >= CData.Opt.iLvDvSaveAs);
                btnM_DDel.Enabled = (!bEn) && ((int)CData.Lev >= CData.Opt.iLvDvDel);
                btnM_DCur.Enabled = (bEn) && ((int)CData.Lev >= CData.Opt.iLvDvCurrent);

                // 2021-11-08, jhLee : Multi-LOT 사용이라면 더이상의 LOT 정보가 존재하지 않는다면 디바이스 변경가능
                if (CData.IsMultiLOT())
                {
                    // LOT 정보가 없고
                    if (CData.LotMgr.GetCount() <= 0)
                    {
                        // 설비가 정지 상태라면 메뉴를 이용할 수 있도록 한다.
                        if ((CSQ_Main.It.m_iStat == EStatus.Idle) || (CSQ_Main.It.m_iStat == EStatus.Stop))
                        {
                            // 디바이스 변경이 가능하게 설정한다.
                			btnM_DApp.Enabled = (bEn) && ((int)CData.Lev >= CData.Opt.iLvDvLoad);
                        }
                    }
                }

            }
            else
            {
                btnM_DNew.Enabled = (bEn) && ((int)CData.Lev >= CData.Opt.iLvDvNew);
                btnM_DApp.Enabled = (bEn) && ((int)CData.Lev >= CData.Opt.iLvDvLoad);
                btnM_DCopy.Enabled = (bEn) && ((int)CData.Lev >= CData.Opt.iLvDvSaveAs);
                btnM_DDel.Enabled = (bEn) && ((int)CData.Lev >= CData.Opt.iLvDvDel);
                btnM_DCur.Enabled = (!bEn) && ((int)CData.Lev >= CData.Opt.iLvDvCurrent);
            }

        }

        private void btnM_DCur_Click(object sender, EventArgs e)
        {
            Button mBtn = sender as Button;
            BeginInvoke(new Action(() => mBtn.Enabled = false));
            //190220 ksg : 추가
            string[] Temp;
            string[] Temp1;
            Temp = CData.DevCur.Split('\\');
            Temp1 = Temp[5].Split('.');

            try
            {
                if (m_sGrp == "") m_sGrp = Temp[4]; //190220 ksg : 추가
                if (m_sDev == "") m_sDev = Temp1[0]; //190220 ksg : 추가

                CDev.It.Load(CData.DevCur, false);
                m_sGrp = Temp[4];
                m_sDev = Temp1[0];
                string s = this.Parent.Name;

                //20200402 jhc : CURRENT 버튼 > Vision SW에 Recipe Name 전송
                string sRecipe = m_sGrp + "\\" + m_sDev;
                CBcr.It.SaveRecipe(sRecipe);
                //

                CData.L_GRD.m_aInspBf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //메뉴얼
                CData.L_GRD.m_aInspAf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //메뉴얼
                CData.L_GRD.m_abMeaBfErr = new bool[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //
                CData.L_GRD.m_abMeaAfErr = new bool[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol];
                CData.L_GRD.m_aInspTemp = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol];
                CData.GrData[0].aMeaBf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //Auto
                CData.GrData[0].aMeaAf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //Auto

                CData.R_GRD.m_aInspBf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //메뉴얼
                CData.R_GRD.m_aInspAf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //메뉴얼
                CData.R_GRD.m_abMeaBfErr = new bool[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //
                CData.R_GRD.m_abMeaAfErr = new bool[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol];
                CData.R_GRD.m_aInspTemp = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol];
                CData.GrData[1].aMeaBf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //Auto
                CData.GrData[1].aMeaAf = new double[CData.Dev.iRow * CData.Dev.iWinCnt, CData.Dev.iCol]; //Auto

                if (!CData.LotInfo.bLotOpen) CData.Parts[(int)EPart.ONL].iSlot_info = new int[CData.Dev.iMgzCnt];

                // 2020.10.26 SungTae Start : Modify by Qorbo(Strip 배출 관련)
                //if (!CData.LotInfo.bLotOpen) CData.Parts[(int)EPart.OFL].iSlot_info = new int[CData.Dev.iMgzCnt];
                if (!CData.LotInfo.bLotOpen)
                {
                    // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                    //if((CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ) &&
                    if((CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                        CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC) &&
                        !CData.Opt.bOfLMgzMatchingOn && CData.Dev.iOffMgzCnt != 0)
                    {
#if true //201116 jhc : DEBUG
                        int nOfLSlotCount = Math.Max(CData.Dev.iOffMgzCnt, CData.Dev.iMgzCnt);
                        CData.Parts[(int)EPart.OFL].iSlot_info = new int[nOfLSlotCount];
#else
                        CData.Parts[(int)EPart.OFL].iSlot_info = new int[CData.Dev.iOffMgzCnt];
#endif
                    }
                    else
                    {
                        CData.Parts[(int)EPart.OFL].iSlot_info = new int[CData.Dev.iMgzCnt];
                    }
                }
                // 2020.10.26 SungTae End

                //20190624 ghk_qc
                //if(CData.CurCompany == eCompany.SkyWorks)
                if (CData.Opt.bQcUse)
                {
                    if (!CData.LotInfo.bLotOpen) CData.Parts[(int)EPart.OFR].iSlot_info = new int[CData.Dev.iMgzCnt];  //190325 ksg : Qc
                }
            }
            catch (Exception ex)
            {

            }
            finally { BeginInvoke(new Action(() => mBtn.Enabled = true)); }

            GoParm(true);

            CData.bLastClick = true; //190509 ksg :
        }
    }
}
