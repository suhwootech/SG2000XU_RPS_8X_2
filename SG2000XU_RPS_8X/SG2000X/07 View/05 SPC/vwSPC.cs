using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SG2000X
{
    public partial class vwSPC : UserControl
    {
        /// <summary>
        /// Max 2020103 : SCK+ Rader & Error Text Modify
        /// </summary>
        private Timer m_tmUpdt_SPC = null;

        private int m_iMtbaCnt = 0;
        private int m_iMtbfCnt = 0;

        private double m_dMtba = 0;
        private double m_dMtbf = 0;
        private static CTim m_Kill = new CTim();

        //20191112 ghk_spccopy
        private string m_sLotInfoCopy = "";
        private string m_sLotNameCopy = "";
        //

        //20191112 ghk_chartzoom
        private int m_iChartMaxS_X = 0;
        private int m_iChartMaxE_X = 0;
        private int m_iChartTtvS_X = 0;
        private int m_iChartTtvE_X = 0;

        private double m_dChartMaxS_Y = 0;
        private double m_dChartMaxE_Y = 0;
        private double m_dChartTtvS_Y = 0;
        private double m_dChartTtvE_Y = 0;
        //

        // 2020.12.10 JSKim St
        private string m_strErrPath = "";
        private string m_strInfoPath = "";
        // 2020.12.10 JSKim Ed

        public vwSPC()
        {
            /*
            if ((int)eLanguage.English == Constants.LANGUAGE)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            }
            else if ((int)eLanguage.Korea == Constants.LANGUAGE)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ko-KR");
            }
            else if ((int)eLanguage.China == Constants.LANGUAGE)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-Hans");
            }
            */
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

            // 2020.12.07 JSKim St
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            {
                btn_RadarList.Visible = true;

                // 2020.12.10 JSKim St
                label25.Text = "TOTAL";
                label23.Text = "RUN";
                label29.Text = "IDLE";
                label27.Text = "ERROR";

                label34.Visible = true;
                label35.Visible = true;
                lb_JamCnt.Visible = true;
                btn_LotErrView.Visible = false;
                // 2020.12.10 JSKim Ed
            }
            else
            {
                btn_RadarList.Visible = false;

                // 2020.12.10 JSKim St
                label25.Text = "TOTAL TIME";
                label23.Text = "RUN TIME";
                label29.Text = "IDLE TIME";
                label27.Text = "JAM TIME";

                label34.Visible = false;
                label35.Visible = false;
                lb_JamCnt.Visible = false;
                btn_LotErrView.Visible = false;
                // 2020.12.10 JSKim Ed
            }
            // 2020.12.07 JSKim Ed
        }

        // 2020.12.09 JSKim St
        ///// <summary>
        ///// Max 2020103 : SCK+ Rader & Error Text Modify
        ///// </summary>        
        //private void _M_tmUpdt_SPC_Tick(object sender, EventArgs e)
        //{
        //    string Temp;
        //    int nRunHour = 0;
        //    int nRunMin = 0;
        //    int nRunSec = 0;
        //    int nRunTimeSum = 0;

        //    int nStopHour = 0;
        //    int nStopMin = 0;
        //    int nStopSec = 0;
        //    int nStopTimeSum = 0;

        //    int nJamHour = 0;
        //    int nJamMin = 0;
        //    int nJamSec = 0;
        //    int nJamTimeSum = 0;

        //    int nMTBF = 0;
        //    int nMTBA = 0;
        //    int nMTTR = 0;
        //    int nMTXX_Hour = 0;
        //    int nMTXX_Min = 0;
        //    int nMTXX_Sec = 0;
        //    DateTime tmpDateTime;

        //    Temp = CData.SwTotalRunTim_MC.Elapsed.ToString(@"hh\:mm\:ss");
        //    tmpDateTime = Convert.ToDateTime(Temp);
        //    nRunHour = tmpDateTime.Hour + CData.dtTotalRunTim_MC.Hour;
        //    nRunMin = tmpDateTime.Minute + CData.dtTotalRunTim_MC.Minute;
        //    if (nRunMin >= 60) { nRunHour++; nRunMin = nRunMin - 60; }
        //    nRunSec = tmpDateTime.Second + CData.dtTotalRunTim_MC.Second;
        //    if (nRunSec >= 60) { nRunMin++; nRunSec = nRunSec - 60; }
        //    Temp = string.Format("{0:00}:{1:00}:{2:00}", nRunHour, nRunMin, nRunSec);
        //    lb_totRunTim.Text = Convert.ToString(Temp);
        //    nRunTimeSum = (nRunHour * 60 * 60) + (nRunMin * 60) + nRunSec;

        //    Temp = CData.SwTotalStopTim_MC.Elapsed.ToString(@"hh\:mm\:ss");
        //    tmpDateTime = Convert.ToDateTime(Temp);
        //    nStopHour = tmpDateTime.Hour + CData.dtTotalStopTim_MC.Hour;
        //    nStopMin = tmpDateTime.Minute + CData.dtTotalStopTim_MC.Minute;
        //    if (nStopMin >= 60) { nStopHour++; nStopMin = nStopMin - 60; }
        //    nStopSec = tmpDateTime.Second + CData.dtTotalStopTim_MC.Second;
        //    if (nStopSec >= 60) { nStopMin++; nStopSec = nStopSec - 60; }
        //    Temp = string.Format("{0:00}:{1:00}:{2:00}", nStopHour, nStopMin, nStopSec);
        //    lb_totStopTim.Text = Convert.ToString(Temp);
        //    nStopTimeSum = (nStopHour * 60 * 60) + (nStopMin * 60) + nStopSec;

        //    Temp = CData.SwTotalJamTim_MC.Elapsed.ToString(@"hh\:mm\:ss");
        //    tmpDateTime = Convert.ToDateTime(Temp);
        //    nJamHour = tmpDateTime.Hour + CData.dtTotalJamTim_MC.Hour;
        //    nJamMin = tmpDateTime.Minute + CData.dtTotalJamTim_MC.Minute;
        //    if (nJamMin >= 60) { nJamHour++; nJamMin = nJamMin - 60; }
        //    nJamSec = tmpDateTime.Second + CData.dtTotalJamTim_MC.Second;
        //    if (nJamSec >= 60) { nJamMin++; nJamSec = nJamSec - 60; }
        //    Temp = string.Format("{0:00}:{1:00}:{2:00}", nJamHour, nJamMin, nJamSec);
        //    lb_totJamTim.Text = Convert.ToString(Temp);
        //    nJamTimeSum = (nJamHour * 60 * 60) + (nJamMin * 60) + nJamSec;

        //    // MTBF : 무 고장간격 --> 가동시간 / 고장시간 
        //    if (nJamTimeSum > 0)
        //    {
        //        nMTBF = nRunTimeSum / nJamTimeSum;
        //        nMTXX_Hour = nMTBF / 3600;
        //        nMTXX_Min = (nMTBF - (nMTXX_Hour * 3600)) / 60;
        //        nMTXX_Sec = (nMTBF - (nMTXX_Hour * 3600) - (nMTXX_Min * 60));
        //        Temp = string.Format("{0:00}:{1:00}:{2:00}", nMTXX_Hour, nMTXX_Min, nMTXX_Sec);
        //        lb_MTBF_Tim.Text = Convert.ToString(Temp);
        //    }

        //    // MTBA : 고장간의 평균 간격 --> 가동시간 / 고장 건수
        //    if (CData.nTotalJamCount > 0)
        //    {
        //        nMTBA = nRunTimeSum / CData.nTotalJamCount;
        //        nMTXX_Hour = nMTBA / 3600;
        //        nMTXX_Min = (nMTBA - (nMTXX_Hour * 3600)) / 60;
        //        nMTXX_Sec = (nMTBA - (nMTXX_Hour * 3600) - (nMTXX_Min * 60));
        //        Temp = string.Format("{0:00}:{1:00}:{2:00}", nMTXX_Hour, nMTXX_Min, nMTXX_Sec);
        //        lb_MTBA_Tim.Text = Convert.ToString(Temp);
        //    }

        //    // MTTR : 평균 수리시간 --> 전체고장수리시간 / 고장 건수
        //    if (CData.nTotalJamCount > 0)
        //    {
        //        nMTTR = nJamTimeSum / CData.nTotalJamCount;
        //        nMTXX_Hour = nMTTR / 3600;
        //        nMTXX_Min = (nMTTR - (nMTXX_Hour * 3600)) / 60;
        //        nMTXX_Sec = (nMTTR - (nMTXX_Hour * 3600) - (nMTXX_Min * 60));
        //        Temp = string.Format("{0:00}:{1:00}:{2:00}", nMTXX_Hour, nMTXX_Min, nMTXX_Sec);
        //        lb_MTTR_Tim.Text = Convert.ToString(Temp);
        //    }
        //}
        //// 2020.12.09 JSKim Ed

        #region Basic method
        public void Open()
        {
            // 2020.12.09 JSKim St
            ///// <summary>
            ///// Max 2020103 : SCK+ Rader & Error Text Modify
            ///// </summary>
            //if (m_tmUpdt_SPC == null)
            //{
            //    m_tmUpdt_SPC = new Timer();
            //    m_tmUpdt_SPC.Interval = 250;
            //    m_tmUpdt_SPC.Tick += _M_tmUpdt_SPC_Tick;
            //}
            //// 타이머 멈춤 상태면 타이머 다시 시작
            //if (!m_tmUpdt_SPC.Enabled) { m_tmUpdt_SPC.Start(); }
            // 2020.12.09 JSKim Ed

            _ListUp();

            // 2021.04.12 SungTae Start : 고객사(ASE-KR) 요청에 따라 Error Info를 한 곳에서 확인하기 위해 추가
            if (CData.CurCompany != ECompany.ASE_KR)
            {
                LoadErrLIst();
                LoadErrHistory();
            }
            else
            {
                LoadErrList2();
                LoadErrHistory2();
            }
            // 2021.04.12 SungTae End

            HideMenu(CData.Lev);
        }

        public void Close()
        {
            /// <summary>
            /// Max 2020103 : SCK+ Rader & Error Text Modify
            /// </summary>
            CErr.SaveErr();

            /// <summary>
            /// Max 2020103 : SCK+ Rader & Error Text Modify
            /// SPC Update 타이머에 대한 모든 리소스 해제
            /// </summary>            
            if (m_tmUpdt_SPC != null)
            {
                if (!m_tmUpdt_SPC.Enabled) { m_tmUpdt_SPC.Stop(); }
                m_tmUpdt_SPC.Dispose();
                m_tmUpdt_SPC = null;
            }

            //20191112 ghk_spccopy
            if (m_sLotInfoCopy != "")
            {
                FileInfo fLotInfo = new FileInfo(m_sLotInfoCopy);

                if (fLotInfo.Exists)
                { fLotInfo.Delete(); }
            }
            if(m_sLotNameCopy != "")
            {
                FileInfo fLotName = new FileInfo(m_sLotNameCopy);

                if (fLotName.Exists)
                { fLotName.Delete(); }
            }
            //
        }

        public void Release()
        {
            Close();
        }

        public void _Set()
        {

        }

        public void _Get()
        {

        }
        #endregion

        private void vwSPC_Load(object sender, EventArgs e)
        {
            Open();
            dtpFrom.Format = DateTimePickerFormat.Custom;
            dtpFrom.CustomFormat = "yyyy-MM-dd";
            dtpTo.Format = DateTimePickerFormat.Custom;
            dtpTo.CustomFormat = "yyyy-MM-dd";

            dtp_ErrHisFrom.Format = DateTimePickerFormat.Custom;
            dtp_ErrHisFrom.CustomFormat = "yyyy-MM-dd";
            dtp_ErrHisTo.Format = DateTimePickerFormat.Custom;
            dtp_ErrHisTo.CustomFormat = "yyyy-MM-dd";

            // 2021.04.12 SungTae Start : 고객사(ASE-KR) 요청에 따라 Error Info를 한 곳에서 확인하기 위해 추가
            dtp_ErrHisFrom2.Format = DateTimePickerFormat.Custom;
            dtp_ErrHisFrom2.CustomFormat = "yyyy-MM-dd";
            dtp_ErrHisTo2.Format = DateTimePickerFormat.Custom;
            dtp_ErrHisTo2.CustomFormat = "yyyy-MM-dd";
            // 2021.04.12 SungTae End

            tabGraph.TabPages.RemoveAt(2);
        }

        public void LoadErrLIst()
        {
            dgv_ErrList.Rows.Clear();
            for(int iRow = 0; iRow < CData.ErrList.Length; iRow++)
            {
                dgv_ErrList.Rows.Add();
                // Row 크기조정 불가
                dgv_ErrList.Rows[iRow].Resizable = DataGridViewTriState.False;
                dgv_ErrList[0, iRow].Value = CData.ErrList[iRow].sNo;
                dgv_ErrList[1, iRow].Value = CData.ErrList[iRow].sName;
                dgv_ErrList[2, iRow].Value = CData.ErrList[iRow].sAction;
            }
        }

        public void LoadErrHistory()
        {
            int iCnt = 0;
            int iNo = 0;
            string sPath = GV.PATH_ERRLOG;
            string sDateFrom = dtp_ErrHisFrom.Text.Replace("-", ""); //dtp_ErrHisFrom
            string sDateTo = dtp_ErrHisTo.Text.Replace("-", ""); ;  //dtp_ErrHisTo
            string sLine = "";
            string[] sValue;

            FileInfo fi;
            DirectoryInfo di;
            DirectoryInfo[] aDi;
            StreamReader sr;

            if (sDateFrom == "")
            { sDateFrom = DateTime.Now.ToString("yyyyMMdd"); }
            if (sDateTo == "")
            { sDateTo = DateTime.Now.ToString("yyyyMMdd"); }

            di = new DirectoryInfo(sPath);
            aDi = di.GetDirectories();

            iCnt = aDi.Length;
            dgv_ErrHis.Rows.Clear();

            for (int iDir = 0; iDir < iCnt; iDir++)
            {
                if ((Convert.ToInt64(sDateFrom) <= Convert.ToInt64(aDi[iDir].Name)) && (Convert.ToInt64(sDateTo) >= Convert.ToInt64(aDi[iDir].Name)))
                {
                    sPath = GV.PATH_ERRLOG;
                    sPath += aDi[iDir].Name + "\\ErrLog.csv";
                    //koo 191101 : error window 
                    try
                    {
                        string filename = Path.GetFileName(sPath);
                        if (CLog.killps(filename)== true) m_Kill.Wait(2000);
                        sr = new StreamReader(sPath, Encoding.GetEncoding("euc-kr"));
                        sLine = sr.ReadLine();
                        while (sr.Peek() > -1)
                        {
                            sLine = sr.ReadLine();
                        
                            fi = new FileInfo(sPath);
                            TimeSpan tSpan;
                            if (fi.Exists)
                            {
                                //koo 191106 error split
                                if (sLine==null)
                                {
                                    string temp = sPath + " is Empty";
                                    CMsg.Show(eMsg.Error, "Error", temp);
                                    return ;
                                }
                                sValue = sLine.Split(',');
                                dgv_ErrHis.Rows.Add();
                                dgv_ErrHis[0, iNo].Value = (iNo + 1).ToString();    //일련번호
                                dgv_ErrHis[1, iNo].Value = sValue[0];    //에러 번호
                                dgv_ErrHis[2, iNo].Value = sValue[1];    //에러 이름
                                dgv_ErrHis[3, iNo].Value = sValue[2];    //발생 시간
                                dgv_ErrHis[4, iNo].Value = sValue[3];    //조치 시간

                                if (sValue[3] != "NONE")
                                {
                                    tSpan = Convert.ToDateTime(sValue[3]) - Convert.ToDateTime(sValue[2]);
                                    if (tSpan.TotalMinutes >= CData.Opt.iSetTime && tSpan.TotalSeconds > 0)
                                    { m_iMtbfCnt++; }
                                    else
                                    { m_iMtbaCnt++; }
                                }
                                iNo++;
                            }
                        }
                        sr.Close();

                    }
                    catch (Exception ex)
                    {
                        if (CData.CurCompany == ECompany.Qorvo) CSQ_Main.It.isSPC = true;//220526 pjh : Qorvo는 Data Save Error 발생 시 Buzzer On
                        CMsg.Show(eMsg.Error, "Error", ex.Message);
                        return ;
                    }
                }
            }

            CalMtbaMtbf();
            
            lb_Mtba.Text    = m_dMtba.ToString();
            lb_Mtbf.Text    = m_dMtbf.ToString();
            lb_Working.Text = CData.Opt.dDayWorking.ToString();
            lb_Renewal.Text = "00";
            lb_SetTime.Text = CData.Opt.iSetTime.ToString();

            // 2021.05.26 SungTae Start : [추가] Qorvo에서 하기 원인의 Issue가 발견되어 조치
            // 원인 : MTBA & MTBF Count 초기화를 하지 않고 계속 누적을 하여 Error History를 Load할 때마다 값이 계속 바뀌어 버리는 문제 야기.
            m_iMtbfCnt = m_iMtbaCnt = 0;
            // 2021.05.26 SungTae End

            ClearAll();
        }

        // 2021.04.12 SungTae Start : 고객사(ASE-KR) 요청에 따라 Error Info를 한 곳에서 확인하기 위해 추가
        public void LoadErrList2()
        {
            dgv_ErrList2.Rows.Clear();

            for (int iRow = 0; iRow < CData.ErrList.Length; iRow++)
            {
                dgv_ErrList2.Rows.Add();
                // Row 크기조정 불가
                dgv_ErrList2.Rows[iRow].Resizable = DataGridViewTriState.False;
                dgv_ErrList2[0, iRow].Value = CData.ErrList[iRow].sNo;
                dgv_ErrList2[1, iRow].Value = CData.ErrList[iRow].sName;
                dgv_ErrList2[2, iRow].Value = CData.ErrList[iRow].sAction;
            }
        }

        public void LoadErrHistory2()
        {
            int iCnt = 0;
            int iNo = 0;
            string sPath = GV.PATH_ERRLOG;
            string sDateFrom = dtp_ErrHisFrom2.Text.Replace("-", ""); //dtp_ErrHisFrom
            string sDateTo = dtp_ErrHisTo2.Text.Replace("-", ""); ;  //dtp_ErrHisTo
            string sLine = "";
            string[] sValue;
            FileInfo fi;
            DirectoryInfo di;
            DirectoryInfo[] aDi;
            StreamReader sr;

            if (sDateFrom == "")    { sDateFrom = DateTime.Now.ToString("yyyyMMdd"); }
            if (sDateTo == "")      { sDateTo = DateTime.Now.ToString("yyyyMMdd"); }

            di = new DirectoryInfo(sPath);
            aDi = di.GetDirectories();

            iCnt = aDi.Length;
            dgv_ErrHistory.Rows.Clear();

            for (int iDir = 0; iDir < iCnt; iDir++)
            {
                if ((Convert.ToInt64(sDateFrom) <= Convert.ToInt64(aDi[iDir].Name)) && (Convert.ToInt64(sDateTo) >= Convert.ToInt64(aDi[iDir].Name)))
                {
                    sPath = GV.PATH_ERRLOG;
                    sPath += aDi[iDir].Name + "\\ErrLog.csv";
                    //koo 191101 : error window 
                    try
                    {
                        string filename = Path.GetFileName(sPath);
                        if (CLog.killps(filename) == true) m_Kill.Wait(2000);
                        sr = new StreamReader(sPath, Encoding.GetEncoding("euc-kr"));
                        sLine = sr.ReadLine();
                        while (sr.Peek() > -1)
                        {
                            sLine = sr.ReadLine();

                            fi = new FileInfo(sPath);
                            TimeSpan tSpan;
                            if (fi.Exists)
                            {
                                //koo 191106 error split
                                if (sLine == null)
                                {
                                    string temp = sPath + " is Empty";
                                    CMsg.Show(eMsg.Error, "Error", temp);
                                    return;
                                }

                                sValue = sLine.Split(',');

                                dgv_ErrHistory.Rows.Add();
                                dgv_ErrHistory[0, iNo].Value = (iNo + 1).ToString();    //일련번호
                                dgv_ErrHistory[1, iNo].Value = sValue[0];    //에러 번호
                                dgv_ErrHistory[2, iNo].Value = sValue[1];    //에러 이름
                                dgv_ErrHistory[3, iNo].Value = sValue[2];    //발생 시간
                                dgv_ErrHistory[4, iNo].Value = sValue[3];    //조치 시간

                                if (sValue[3] != "NONE")
                                {
                                    tSpan = Convert.ToDateTime(sValue[3]) - Convert.ToDateTime(sValue[2]);

                                    if (tSpan.TotalMinutes >= CData.Opt.iSetTime && tSpan.TotalSeconds > 0)
                                    { m_iMtbfCnt++; }
                                    else
                                    { m_iMtbaCnt++; }
                                }

                                iNo++;
                            }
                        }

                        sr.Close();
                    }
                    catch (Exception ex)
                    {
                        if (CData.CurCompany == ECompany.Qorvo) CSQ_Main.It.isSPC = true;//220526 pjh : Qorvo는 Data Save Error 발생 시 Buzzer On
                        CMsg.Show(eMsg.Error, "Error", ex.Message);
                        return;
                    }
                }
            }

            CalMtbaMtbf();

            //lb_Mtba.Text = m_dMtba.ToString();
            //lb_Mtbf.Text = m_dMtbf.ToString();
            //lb_Working.Text = CData.Opt.dDayWorking.ToString();
            //lb_Renewal.Text = "00";
            //lb_SetTime.Text = CData.Opt.iSetTime.ToString();

            ClearAll();
        }
        // 2021.04.12 SungTae End

        public void CalMtbaMtbf()
        {
            //CData.Opt.dDayWorking;  하루 작업 시간
            //CData.Opt.iPeriod;    산정 기간
            //CData.Opt.iRenewal;   갱신 시간
            //CData.Opt.iSetTime;   조치 경과 시간
            int iMin = 0;
            
            string sDateFrom = dtp_ErrHisFrom.Text; //dtp_ErrHisFrom
            string sDateTo = dtp_ErrHisTo.Text; ;  //dtp_ErrHisTo

            if (sDateFrom == "")
            { sDateFrom = DateTime.Now.ToString("yyyy-MM-dd"); }

            if (sDateTo == "")
            { sDateTo = DateTime.Now.ToString("yyyy-MM-dd"); }

            TimeSpan tSpan;
            tSpan = Convert.ToDateTime(sDateTo) - Convert.ToDateTime(sDateFrom);
            iMin = (tSpan.Days + 1) * 24 * 60;

            //syc : Qorvo 수정 - 
            //m_dMtbf = iMin / (m_iMtbfCnt + 1);
            //m_dMtba = iMin / (m_iMtbaCnt + 1);

            if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ)
            {
                if (m_iMtbfCnt == 0) { m_iMtbfCnt++; }
                if (m_iMtbaCnt == 0) { m_iMtbaCnt++; }

                m_dMtbf = iMin / m_iMtbfCnt;
                m_dMtba = iMin / m_iMtbaCnt;
            }
            else
            {
                m_dMtbf = iMin / (m_iMtbfCnt + 1);
                m_dMtba = iMin / (m_iMtbaCnt + 1);
            }

            if (CData.Opt.iSetTime == 0)
            { m_dMtba = m_dMtbf; }
        }

        public void _ListUp()
        {
            int iCnt = 0;
            DirectoryInfo[] aVal;
            string sPath = GV.PATH_SPC + "LotLog";

            if (Directory.Exists(sPath) == false)
            { Directory.CreateDirectory(sPath); }

            lbxS_Date.Items.Clear();
            DirectoryInfo mFile = new DirectoryInfo(sPath);
            aVal = mFile.GetDirectories();
            iCnt = aVal.Length;

            for (int i = 0; i < iCnt; i++)
            {
                //lbxS_Date.Items.Add(aVal[i].Name);
                lbxS_Date.Items.Insert(0, aVal[i].Name);
            }
        }

        public void _listUpLot(string sPath)
        {
            int iCnt = 0;
            DirectoryInfo[] aVal;

            if (Directory.Exists(sPath) == false)
            { Directory.CreateDirectory(sPath); }

            lbxS_Lotname.Items.Clear();
            DirectoryInfo mFile = new DirectoryInfo(sPath);
            aVal = mFile.GetDirectories();
            iCnt = aVal.Length;

            for (int i = 0; i < iCnt; i++)
            {
                lbxS_Lotname.Items.Add(aVal[i].Name);
            }
        }

        public void _listUpTb(string sPath)
        {
            int iCnt = 0;
            DirectoryInfo[] aVal;

            if (Directory.Exists(sPath) == false)
            { Directory.CreateDirectory(sPath); }

            lbxS_Tb.Items.Clear();
            DirectoryInfo mFile = new DirectoryInfo(sPath);
            aVal = mFile.GetDirectories();
            iCnt = aVal.Length;

            for (int i = 0; i < iCnt; i++)
            {
                lbxS_Tb.Items.Add(aVal[i].Name);
            }
        }

        public void _listUpFile(string sPath)
        {
            if (Directory.Exists(sPath) == true)
            {
                lbxS_File.Items.Clear();
                DirectoryInfo mFile = new DirectoryInfo(sPath);
                foreach (FileSystemInfo mInfo in mFile.GetFileSystemInfos("*.ini"))
                {
                    lbxS_File.Items.Add(mInfo.Name.Replace(".ini", ""));
                }
            }
        }

        public void _LoadLotInfo(string _sPath)
        {
            string sRowVal = "";
            string[] sHeader;
            //0 = day, 1 = LotName, 2 = Device, 3 = Target, 4 = StartTiem, 5 = EndTime, 6 = RunTime, 7 = IdleTime, 8 = JamTime, 9 = TotalTime, 10 = Uph, 11 = WorkStrip, 12 = WhLSerial, 13 = WhRSerial
            string[] sData;
            string sPath = _sPath + "\\LotInfo.csv";
            // 2020.12.10 JSKim St
            m_strErrPath = _sPath + "\\LotErrInfo.csv";
            // 2020.12.10 JSKim Ed
            FileInfo fi = new FileInfo(sPath);
            // 2020.12.10 JSKim St
            FileInfo fi_Err = new FileInfo(m_strErrPath);
            // 2020.12.10 JSKim Ed

            //20191112 ghk_spccopy
            if (CData.LotInfo.bLotOpen)
            {
                if (CData.LotInfo.sLotName == lbxS_Lotname.SelectedItem.ToString())
                {
                    if (fi.Exists)
                    {
                        m_sLotInfoCopy = _sPath + "\\LotInfoCopy.csv";

                        fi.CopyTo(m_sLotInfoCopy, true);
                        fi = new FileInfo(m_sLotInfoCopy);
                        sPath = m_sLotInfoCopy;
                    }
                }
            }
            //

            // 2020.12.10 JSKim St
            m_strInfoPath = sPath;
            // 2020.12.10 JSKim Ed

            if (fi.Exists)
            {
                //koo 191101 : error window 
                try
                {
                    string filename = Path.GetFileName(sPath);
                    if (CLog.killps(filename)== true) m_Kill.Wait(2000);
                    StreamReader sr = new StreamReader(sPath, Encoding.GetEncoding("euc-kr"));
                    sRowVal = sr.ReadLine();
                    //koo 191106 error split
                    if (sRowVal==null)
                    {
                        string temp = sPath + " is Empty";
                        CMsg.Show(eMsg.Error, "Error", temp);
                        return ;
                    }
                    sHeader = sRowVal.Split(',');
                    sRowVal = sr.ReadLine();
                    //koo 191106 error split
                    if (sRowVal==null)
                    {
                        string temp = sPath + " is Empty";
                        CMsg.Show(eMsg.Error, "Error", temp);
                        return ;
                    }
                    sData   = sRowVal.Split(',');
                    sr.Close();
                }
                catch (Exception ex)
                {
                    if (CData.CurCompany == ECompany.Qorvo) CSQ_Main.It.isSPC = true;//220526 pjh : Qorvo는 Data Save Error 발생 시 Buzzer On
                    CMsg.Show(eMsg.Error, "Error", ex.Message);
                    return ;
                }

                lb_Day.Text       = sData[0];
                // 2020.12.10 JSKim St
                lb_LotName.Text   = sData[1];
                // 2020.12.10 JSKim Ed
                lb_Mode.Text      = sData[2];
                lb_Dev.Text       = sData[3];
                lb_Target.Text    = sData[4];
                lb_Starttime.Text = sData[5];
                lb_Endtime.Text   = sData[6];
                lb_Runtime.Text   = sData[7];
                lb_Idletime.Text  = sData[8];
                lb_Jamtime.Text   = sData[9];
                lb_Totaltime.Text = sData[10];
                lb_Uph      .Text = sData[11];
                lb_Workstrip.Text = sData[12];
                lb_WhL      .Text = sData[13];
                lb_whR      .Text = sData[14];
                // 2020.12.10 JSKim St
                lb_JamCnt.Text    = sData[15];
                // 2020.12.10 JSKim Ed

                // 2020.12.10 JSKim St
                if (fi_Err.Exists)
                {
                    btn_LotErrView.Visible = true;
                }
                else
                {
                    btn_LotErrView.Visible = false;
                }
                // 2020.12.10 JSKim Ed
            }
        }

        public void _LoadGraph(string _sPath)
        {
            int iCnt = 0;
            string sRowVal = "";
            string sBcr = "";
            string[] sHeader;
            //0 = day, 1 = LotName, 2 = MGZNo, 3 = SlotNo, 4 = Table, 5 = Max, 6 = Min, 7 = Mean, 8 = Ttv, 9 = Target, 10 = TargetLim, 11 = TtvLimit
            string[] sData;

            List<double> lMax         = new List<double>();
            List<double> lMin         = new List<double>();
            List<double> lMean        = new List<double>();
            List<double> lTtv         = new List<double>();
            List<double> lTarget      = new List<double>();
            List<double> lTargetLimit = new List<double>();
            List<double> lTtvLimit    = new List<double>();
            List<double> lStev        = new List<double>();
            List<string> lGrdMode = new List<string>();
            List<string> lWay = new List<string>();

            double dMean = 0.0;

            FileInfo fi = new FileInfo(_sPath);

            if (fi.Exists)
            {
                //koo 191101 : error window 
                try
                {
                    string filename = Path.GetFileName(_sPath);
                    if (CLog.killps(filename)== true) m_Kill.Wait(2000);
                    StreamReader sr = new StreamReader(_sPath, Encoding.GetEncoding("euc-kr"));
                    sRowVal = sr.ReadLine();
                    //koo 191106 error split
                    if (sRowVal==null)
                    {
                        string temp = _sPath + " is Empty";
                        CMsg.Show(eMsg.Error, "Error", temp);
                        return ;
                    }
                    sHeader = sRowVal.Split(',');
                    while (sr.Peek() > -1)
                    {
                        sRowVal = sr.ReadLine();
                        //koo 191106 error split
                        if (sRowVal==null)
                        {
                            string temp = _sPath + " is Empty";
                            CMsg.Show(eMsg.Error, "Error", temp);
                            return ;
                        }
                        sData   = sRowVal.Split(',');
                        sBcr    = sData[4];

                        lWay.Add(sData[5]);

                        if (sData[6] == "")
                        { lMax.Add(0.0);}
                        else
                        { lMax.Add(Convert.ToDouble(sData[6]));}

                        if(sData[7] == "")
                        { lMin.Add(0.0);}
                        else
                        { lMin.Add(Convert.ToDouble(sData[7]));}

                        if(sData[8] == "")
                        { lMean.Add(0.0);}
                        else
                        { lMean.Add(Convert.ToDouble(sData[8]));}

                        if(sData[9] == "")
                        { lTtv.Add(0.0);}
                        else
                        { lTtv.Add(Convert.ToDouble(sData[9]));}

                        if(sData[10] == "")
                        { lTarget.Add(0.0);}
                        else
                        { lTarget.Add(Convert.ToDouble(sData[10]));}

                        if(sData[11] == "")
                        { lTargetLimit.Add(0.0);}
                        else
                        { lTargetLimit.Add(Convert.ToDouble(sData[11]));}

                        if(sData[12] == "")
                        { lTtvLimit.Add(0.0);}
                        else
                        { lTtvLimit.Add(Convert.ToDouble(sData[12]));}

                        lGrdMode.Add(sData[13]);
                     }
                    sr.Close();
                }
                catch (Exception ex)
                {
                    if (CData.CurCompany == ECompany.Qorvo) CSQ_Main.It.isSPC = true;//220526 pjh : Qorvo는 Data Save Error 발생 시 Buzzer On
                    CMsg.Show(eMsg.Error, "Error", ex.Message);
                    return ;
                }
                for (int i = 0; i < lMean.Count; i++)
                {
                    iCnt++;
                    dMean += lMean[i];
                }

                dMean = Math.Round((dMean / iCnt), 4);      // Mean값의 평균 계산
                for (int j = 0; j < lMean.Count; j++)
                {
                    if(lGrdMode[j] == "STEP" && lWay[j] == "L")
                    {//스텝 모드일때 오른쪽 데이터만 그래프에 표시
                        continue;
                    }
                    chart_Max.ChartAreas[0].AxisY.Maximum = lTarget[j] + 0.03;
                    chart_Max.ChartAreas[0].AxisY.Minimum = lTarget[j] - 0.03;
                    lStev.Add(Math.Round(Math.Abs(dMean - lMean[j]))); 
                    chart_Max.Series["ser_Max" ].Points.AddXY(j + 1, lMax[j] );
                    chart_Max.Series["ser_Min" ].Points.AddXY(j + 1, lMin[j] );
                    chart_Max.Series["ser_Mean"].Points.AddXY(j + 1, lMean[j]);
                    if (lTargetLimit[j] != 0)
                    {
                        chart_Max.Series["ser_TarMax"].Points.AddXY(j + 1, lTarget[j] + lTargetLimit[j]);
                        chart_Max.Series["ser_TarMin"].Points.AddXY(j + 1, lTarget[j] - lTargetLimit[j]);
                    }
                    chart_Ttv.Series["ser_Ttv"].Points.AddXY(j + 1, lTtv[j]);
                    if (lTtvLimit[j] != 0)
                    {
                        chart_Ttv.Series["ser_TtvMax"].Points.AddXY(j + 1, lTtvLimit[j]);
                    }
                    //chart_Stev.Series["ser_Stev"].Points.AddXY(j + 1, lStev[j]);
                }
                //lb_Bcr.Text = sBcr;

                //2020.10.04 jhLee :  확장된 데이터 표시를 지원하는 경우 Min/Max/Mean 데이터를 표시한다.
                if (CDataOption.nDispDataExtend != 0)     
                {
                    double dMaxValue = Math.Round(lMax.Max(), 4);    // Max값
                    double dMinValue = Math.Round(lMin.Min(), 4);    // Min값
                    double dMeanValue = Math.Round( ((dMaxValue + dMinValue) / 2.0), 4);    // Min, Max의 평균을 구한다.

                    // Chart의 Title을 통해 값을 직접 표시한다.
                    chart_Max.Titles["TitleAverage"].Text = 
                        string.Format("MAX : {0:0.0000}\nMIN : {1:0.0000}\nMean : {2:0.0000}", dMaxValue, dMinValue, dMeanValue);
                }
            }
        }
        //190319 ksg : ASE kr용
        public void _LoadGraph_Add(string _sPath)
        {
            int iCnt = 0;
            string sRowVal = "";
            string sBcr = "";
            string[] sHeader;
            //0 = day       , 1 = LotName, 2 = MGZNo, 3  = SlotNo, 4  = Barcode, 5  = Table , 6  = Before Max, 7  = Before Min, 
            //8 = Before Avg, 9 = Max    , 10 = Min , 11 = Mean  , 12 = Ttv    , 13 = Target, 14 = TargetLim , 15 = TtvLimit
            string[] sData;

            List<double> lMax         = new List<double>();
            List<double> lMin         = new List<double>();
            List<double> lMean        = new List<double>();
            List<double> lTtv         = new List<double>();
            List<double> lTarget      = new List<double>();
            List<double> lTargetLimit = new List<double>();
            List<double> lTtvLimit    = new List<double>();
            List<double> lStev        = new List<double>();

            double dMean = 0.0;

            FileInfo fi = new FileInfo(_sPath);

            if (fi.Exists)
            {
                StreamReader sr = new StreamReader(_sPath, Encoding.GetEncoding("euc-kr"));
                sRowVal = sr.ReadLine();
                //koo 191106 error split
                if (sRowVal==null)
                {
                    string temp = _sPath + " is Empty";
                    CMsg.Show(eMsg.Error, "Error", temp);
                    return ;
                }
                sHeader = sRowVal.Split(',');
                while (sr.Peek() > -1)
                {
                    sRowVal = sr.ReadLine();
                    //koo 191106 error split
                    if (sRowVal==null)
                    {
                        string temp = _sPath + " is Empty";
                        CMsg.Show(eMsg.Error, "Error", temp);
                        return ;
                    }
                    sData   = sRowVal.Split(',');
                    sBcr    = sData[4];

                    if(sData[9] == "")
                    { lMax.Add(0.0);}
                    else
                    { lMax.Add(Convert.ToDouble(sData[9]));}

                    if(sData[10] == "")
                    { lMin.Add(0.0);}
                    else
                    { lMin.Add(Convert.ToDouble(sData[10]));}

                    if(sData[11] == "")
                    { lMean.Add(0.0);}
                    else
                    { lMean.Add(Convert.ToDouble(sData[11]));}

                    if(sData[12] == "")
                    { lTtv.Add(0.0);}
                    else
                    { lTtv.Add(Convert.ToDouble(sData[12]));}

                    if(sData[13] == "")
                    { lTarget.Add(0.0);}
                    else
                    { lTarget.Add(Convert.ToDouble(sData[13]));}

                    if(sData[14] == "")
                    { lTargetLimit.Add(0.0);}
                    else
                    { lTargetLimit.Add(Convert.ToDouble(sData[14]));}

                    if(sData[15] == "")
                    { lTtvLimit.Add(0.0);}
                    else
                    { lTtvLimit.Add(Convert.ToDouble(sData[15]));}
                }
                sr.Close();
                for (int i = 0; i < lMean.Count; i++)
                {
                    iCnt++;
                    dMean += lMean[i];
                }
                dMean = Math.Round((dMean / iCnt), 4);
                chart_Max.ChartAreas[0].AxisY.Maximum = lTarget[0] + 0.03;
                chart_Max.ChartAreas[0].AxisY.Minimum = lTarget[0] - 0.03;
                for (int j = 0; j < lMean.Count; j++)
                {
                    lStev.Add(Math.Round(Math.Abs(dMean - lMean[j]))); 
                    chart_Max.Series["ser_Max" ].Points.AddXY(j + 1, lMax[j] );
                    chart_Max.Series["ser_Min" ].Points.AddXY(j + 1, lMin[j] );
                    chart_Max.Series["ser_Mean"].Points.AddXY(j + 1, lMean[j]);
                    if (lTargetLimit[j] != 0)
                    {
                        chart_Max.Series["ser_TarMax"].Points.AddXY(j + 1, lTarget[j] + lTargetLimit[j]);
                        chart_Max.Series["ser_TarMin"].Points.AddXY(j + 1, lTarget[j] - lTargetLimit[j]);
                    }
                    chart_Ttv.Series["ser_Ttv"].Points.AddXY(j + 1, lTtv[j]);
                    if (lTtvLimit[j] != 0)
                    {
                        chart_Ttv.Series["ser_TtvMax"].Points.AddXY(j + 1, lTtvLimit[j]);
                    }
                    //chart_Stev.Series["ser_Stev"].Points.AddXY(j + 1, lStev[j]);
                }
                //lb_Bcr.Text = sBcr;
            }
        }

        private void ListMap(string sPath)
        {
            int iRow = 0;
            int iCol = 0;
            string sBcr = "";
            string sSec = "";
            string sKey = "";
            string[] sData;
            CIni mIni = new CIni(sPath + ".ini");
            iRow = mIni.ReadI("ETC", "Row");
            iCol = mIni.ReadI("ETC", "Col");
            sBcr = mIni.Read("ETC", "BARCODE");
            double[,] dData = new double[iRow, iCol];
            
            lb_Bcr.Text = sBcr;
            
            grdMap.Rows.Clear();
            grdMap.Columns.Clear();
            grdMap.Refresh();
            grdMap.ColumnCount = iCol;
            if (CDataOption.Package == ePkg.Strip)
            {
                grdMap.RowCount = iRow;
                grdMap.RowTemplate.Height = (grdMap.Height / (iRow));

                for (int iR = 0; iR < iRow; iR++)
                {
                    sKey = "R" + string.Format("{0:00}", iR);
                    sData = mIni.Read("Data", sKey).Split('_');
                    for (int iC = 0; iC < iCol; iC++)
                    {
                        if (sData[iC].Trim() != "")
                        {
                            dData[iR, iC] = Convert.ToDouble(sData[iC]);
                            grdMap[iC, iR].Value = dData[iR, iC];
                        }
                    }
                }
                grdMap.RowTemplate.Height = (grdMap.Height / (iRow));
            }
            else
            {
                grdMap.RowCount = iRow * CData.Dev.iUnitCnt;
                grdMap.RowTemplate.Height = (grdMap.Height / (iRow * CData.Dev.iUnitCnt));

                for (int iU = 0; iU < CData.Dev.iUnitCnt; iU++)
                {
                    sSec = "Unit " + (iU + 1) + " Data";   
                    for (int iR = 0; iR < iRow; iR++)
                    {
                        sKey = "R" + string.Format("{0:00}", iR);
                        sData = mIni.Read(sSec, sKey).Split('_');
                        for (int iC = 0; iC < iCol; iC++)
                        {
                            if (sData.Length < iCol)
                            {
                                dData[iR, iC] = 0;
                                grdMap[iC, iR + (iU * iRow)].Value = 0;
                            }
                            else if (sData[iC].Trim() != "")
                            {
                                dData[iR, iC] = Convert.ToDouble(sData[iC]);
                                grdMap[iC, iR + (iU * iRow)].Value = dData[iR, iC];
                            }
                        }
                    }
                }
                grdMap.RowTemplate.Height = (grdMap.Height / (iRow * CData.Dev.iUnitCnt));
            }
            
        }

        private void lbxS_Date_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sPath = "";

            // 2020.12.09 JSKim St
            lb_Bcr.Text = "";
            // 2020.12.09 JSKim Ed

            lbxS_Lotname.Items.Clear();
            lbxS_Tb.Items.Clear();
            lbxS_File.Items.Clear();
            ClearMap();
            ClearLotInfo();
            ClearGraph();

            sPath = GV.PATH_SPC + "LotLog\\";
            if (lbxS_Date.SelectedIndex >= 0)
            {
                sPath += lbxS_Date.SelectedItem.ToString();
                _listUpLot(sPath);
            }
        }

        private void lbxS_Lotname_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sPath = "";

            //20191112 ghk_chartzoom
            chart_Max.ChartAreas[0].AxisX.ScaleView.ZoomReset();
            chart_Max.ChartAreas[0].AxisY.ScaleView.ZoomReset();
            chart_Ttv.ChartAreas[0].AxisX.ScaleView.ZoomReset();
            chart_Ttv.ChartAreas[0].AxisY.ScaleView.ZoomReset();

            // 2020.12.09 JSKim St
            lb_Bcr.Text = "";
            // 2020.12.09 JSKim Ed

            lbxS_Tb.Items.Clear();
            lbxS_File.Items.Clear();
            ClearMap();
            ClearLotInfo();
            ClearGraph();

            sPath = GV.PATH_SPC + "LotLog\\";
            if (lbxS_Date.SelectedIndex >= 0)
            {
                sPath += lbxS_Date.SelectedItem.ToString() + "\\";
                if (lbxS_Lotname.SelectedIndex >= 0)
                {
                    sPath += lbxS_Lotname.SelectedItem.ToString();
                    _listUpTb(sPath);
                    _LoadLotInfo(sPath);

                    //20191112 ghk_spccopy
                    m_sLotNameCopy = sPath;
                    //

                    sPath += "\\" + lbxS_Lotname.SelectedItem.ToString() + ".csv";

                    //20191112 ghk_spccopy
                    m_sLotNameCopy += "\\" + lbxS_Lotname.SelectedItem.ToString() + "copy.csv";
                    if (CData.LotInfo.bLotOpen)
                    {
                        if (CData.LotInfo.sLotName == lbxS_Lotname.SelectedItem.ToString())
                        {
                            FileInfo fi = new FileInfo(sPath);

                            if (fi.Exists)
                            {
                                fi.CopyTo(m_sLotNameCopy, true);
                                fi = new FileInfo(m_sLotNameCopy);
                                sPath = m_sLotNameCopy;
                            }
                        }
                    }
                    //

                    //190319 ksg:
                    //_LoadGraph(sPath);
                    if (CData.CurCompany == ECompany.ASE_KR) _LoadGraph_Add(sPath);
                    else                                    _LoadGraph(sPath);
                }
            }
        }

        private void lbxS_Tb_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sPath = "";

            // 2020.12.09 JSKim St
            lb_Bcr.Text = "";
            // 2020.12.09 JSKim Ed

            lbxS_File.Items.Clear();
            ClearMap();

            sPath = GV.PATH_SPC + "LotLog\\";

            if (lbxS_Date.SelectedIndex >= 0)
            {
                sPath += lbxS_Date.SelectedItem.ToString() + "\\";

                if (lbxS_Lotname.SelectedIndex >= 0)
                {
                    sPath += lbxS_Lotname.SelectedItem.ToString() + "\\";

                    if (lbxS_Tb.SelectedIndex >= 0)
                    {
                        sPath += lbxS_Tb.SelectedItem.ToString();
                        _listUpFile(sPath);
                    }
                }
            }
        }

        private void lbxS_File_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sPath = "";
            sPath = GV.PATH_SPC + "LotLog\\";

            if (lbxS_Date.SelectedIndex >= 0)
            {
                sPath += lbxS_Date.SelectedItem.ToString() + "\\";

                if (lbxS_Lotname.SelectedIndex >= 0)
                {
                    sPath += lbxS_Lotname.SelectedItem.ToString() + "\\";

                    if (lbxS_Tb.SelectedIndex >= 0)
                    {
                        sPath += lbxS_Tb.SelectedItem.ToString() + "\\";

                        if (lbxS_File.SelectedIndex >= 0)
                        {
                            sPath += lbxS_File.SelectedItem.ToString();
                            ListMap(sPath);
                        }
                    }
                }
            }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            string sPath = "";
            sfd_Chart.ShowDialog();
            if (sfd_Chart.FileName != "")
            {
                sPath = Path.GetDirectoryName(sfd_Chart.FileName) + "\\" + Path.GetFileNameWithoutExtension(sfd_Chart.FileName);
                switch (sfd_Chart.FilterIndex)
                {
                    case 1:
                        {
                            chart_Max.SaveImage(sPath + "_Max_Min_Mean.jpg", System.Drawing.Imaging.ImageFormat.Png);
                            chart_Ttv.SaveImage(sPath + "_Ttv.jpg", System.Drawing.Imaging.ImageFormat.Png);
                            //chart_Stev.SaveImage(sPath + "_Stev.jpg", System.Drawing.Imaging.ImageFormat.Png);
                            break;
                        }

                    case 2:
                        {
                            chart_Max.SaveImage(sPath + "_Max_Min_Mean.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                            chart_Ttv.SaveImage(sPath + "_Ttv.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                            //chart_Stev.SaveImage(sPath + "_Stev.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                            break;
                        }

                    case 3:
                        {
                            chart_Max.SaveImage(sPath + "_Max_Min_Mean.jpg", System.Drawing.Imaging.ImageFormat.Bmp);
                            chart_Ttv.SaveImage(sPath + "_Ttv.jpg", System.Drawing.Imaging.ImageFormat.Bmp);
                            //chart_Stev.SaveImage(sPath + "_Stev.jpg", System.Drawing.Imaging.ImageFormat.Bmp);
                            break;
                        }
                }
            }
        }

        private void btn_View_Click(object sender, EventArgs e)
        {

            int iCnt = 0;
            DirectoryInfo[] aVal;

            string sFirst = "";
            string sEnd = "";
            string sPath = GV.PATH_SPC + "LotLog";

            sFirst = dtpFrom.Text.Replace("-", "");
            sEnd = dtpTo.Text.Replace("-", "");

            if (Directory.Exists(sPath) == false)
            { return; }

            DirectoryInfo mFile = new DirectoryInfo(sPath);
            aVal = mFile.GetDirectories();
            iCnt = aVal.Length;
            lbxS_Date.Items.Clear();
            for (int i = 0; i < iCnt; i++)
            {
                //200228 ksg :
                int ChkNum = 0;
                bool ChkName = int.TryParse(aVal[i].Name, out ChkNum);
                if(!ChkName) continue;
                //if (Convert.ToInt64(aVal[i].Name) > Convert.ToInt64(sFirst) && Convert.ToInt64(aVal[i].Name) < Convert.ToInt64(sEnd))
                if (Convert.ToInt64(aVal[i].Name) >= Convert.ToInt64(sFirst) && Convert.ToInt64(aVal[i].Name) <= Convert.ToInt64(sEnd)) //190903 ksg : 검색과 같은 날도 포함되게 수정
                {
                    //lbxS_Date.Items.Add(aVal[i].Name);
                    lbxS_Date.Items.Insert(0, aVal[i].Name);
                }
            }

            ClearAll();
        }

        private void btn_Reset_Click(object sender, EventArgs e)
        {
            _ListUp();
            ClearAll();
        }

        private void ClearAll()
        {
            // 2020.12.09 JSKim St
            lb_Bcr.Text = "";
            // 2020.12.09 JSKim Ed

            ClearLotInfo();
            ClearGraph();
            ClearMap();
            lbxS_Lotname.Items.Clear();
            lbxS_Tb.Items.Clear();
            lbxS_File.Items.Clear();
        }

        private void ClearLotInfo()
        {
            lb_Day.Text       = "";
            // 2020.12.10 JSKim St
            lb_LotName.Text   = "";
            // 2020.12.10 JSKim Ed
            lb_Dev.Text       = "";
            // 2020.12.10 JSKim St
            lb_Mode.Text      = "";
            // 2020.12.10 JSKim Ed
            lb_Target.Text    = "";
            lb_Starttime.Text = "";
            lb_Endtime.Text   = "";
            lb_Runtime.Text   = "";
            lb_Idletime.Text  = "";
            lb_Jamtime.Text   = "";
            lb_Totaltime.Text = "";
            lb_Uph.Text = "";
            lb_Workstrip.Text = "";
            lb_WhL.Text       = "";
            lb_whR.Text       = "";
            // 2020.12.10 JSKim St
            lb_JamCnt.Text    = "";
            // 2020.12.10 JSKim Ed

            // 2020.12.10 JSKim St
            m_strErrPath      = "";
            m_strInfoPath     = "";
  		    btn_LotErrView.Visible = false;
            // 2020.12.10 JSKim Ed
        }

        private void ClearGraph()
        {
            foreach (var series in chart_Max.Series)
            {
                series.Points.Clear();
            }
            foreach (var series in chart_Ttv.Series)
            {
                series.Points.Clear();
            }
            foreach (var series in chart_Stev.Series)
            {
                series.Points.Clear();
            }

            //2020.10.04 jhLee : SPIL에서 요청한 Min/Max/표시 clear
            if (CDataOption.nDispDataExtend != 0)
            {
                chart_Max.Titles["TitleAverage"].Text =                 // 초기값 표시
                    string.Format("MAX : {0:0.0000}\nMIN : {1:0.0000}\nMean : {2:0.0000}", 0.0, 0.0, 0.0);
            }
        }


        private void ClearMap()
        {
            grdMap.Rows.Clear();
            grdMap.Columns.Clear();
        }

        private void vwSPC_Load_1(object sender, EventArgs e)
        {
            tabGraph.TabPages.RemoveAt(2);
        }

        private void rdb_Spc_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton mRdb = sender as RadioButton;
            tabSpc.SelectTab(int.Parse(mRdb.Tag.ToString()));
        }

        private void dgv_ErrList_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv_ErrList.SelectedCells.Count == 0)
            { return; }
            // 빈 행을 선택시 예외처리 
            // 190328 -maeng
            if (dgv_ErrList.SelectedRows[0].Cells[0].Value == null)
            { return; }
            int iCellNo = dgv_ErrList.SelectedCells[0].RowIndex;
            FileInfo fi;

            if (dgv_ErrList.SelectedCells[0].RowIndex == (int)eErr.ERR_MAXCOUNT)
            {
                return;
            }
            txt_ErrEnum.Text    = CData.ErrList[iCellNo].sName;
            txt_ErrNo.Text      = "ERR" + Convert.ToInt32(CData.ErrList[iCellNo].sNo).ToString("D3");
            txt_ErrName.Text    = CData.ErrList[iCellNo].sName;
            txt_ErrAction.Text  = CData.ErrList[iCellNo].sAction.Replace("*", "\r\n");
            txt_ErrImgPath.Text = GV.PATH_ERRIMG + Convert.ToInt32(CData.ErrList[iCellNo].sNo).ToString("D3") + ".png";

            // 2021.09.06 SungTae Start : [추가] (ASE-KR VOC) Error명 한글 표시 요청
            if (CData.CurCompany == ECompany.ASE_KR)
            {
                txt_ErrName_KOR.Text = CData.ErrList[iCellNo].sKorName;
            }
            // 2021.09.06 SungTae End

            // 2020.12.07 JSKim St - 기존 Radar 삭제
            /// <summary>
            /// Max 2020103 : SCK+ Rader & Error Text Modify
            /// </summary>
            //if ("0" != CData.ErrList[iCellNo].iRaderUseCnt.ToString())
            //{
            //    cb_RaderUse.Checked = true;
            //    txt_RaderCounter.Text = CData.ErrList[iCellNo].iRaderUseCnt.ToString();
            //}
            //else
            //{
            //    cb_RaderUse.Checked = false;
            //    txt_RaderCounter.Text = CData.ErrList[iCellNo].iRaderUseCnt.ToString();
            //}
            // 2020.12.07 JSKim Ed

            fi = new FileInfo(txt_ErrImgPath.Text);
            if (fi.Exists)
            { pic_ErrImg.Load(txt_ErrImgPath.Text); }
            else
            { pic_ErrImg.Image = null; }
        }

        private void btn_ErrHIsView_Click(object sender, EventArgs e)
        {
            LoadErrHistory();
        }

        private void HideMenu(ELv RetLv)
        {
            btn_Save.Enabled = (int)RetLv >= CData.Opt.iLvSpcGpSave;
            rdb_ErrList.Visible = (int)RetLv >= CData.Opt.iLvSpcErrList;

            // 2021.04.12 SungTae Start : 고객사(ASE-KR) 요청에 따라 Error Info를 한 곳에서 확인하기 위해 수정
            //rdb_ErrHis.Visible = (int)RetLv >= CData.Opt.iLvSpcErrHis;
            //btn_ErrHIsView.Enabled = (int)RetLv >= CData.Opt.iLvSpcErrHisView;
            //btn_ErrHisSave.Enabled = (int)RetLv >= CData.Opt.iLvSpcErrHisSave;
            if (CData.CurCompany != ECompany.ASE_KR)
            {
                rdb_ErrList.Text = "ERROR LIST";
                rdb_ErrHis.Visible = (int)RetLv >= CData.Opt.iLvSpcErrHis;
                btn_ErrHIsView.Enabled = (int)RetLv >= CData.Opt.iLvSpcErrHisView;
                btn_ErrHisSave.Enabled = (int)RetLv >= CData.Opt.iLvSpcErrHisSave;

                btn_ErrHisView2.Enabled = false;
                btn_ErrHisSave2.Enabled = false;

                pnl_TabCtrlErr.Visible = true;
                tab_ErrorInfo.Visible = false;

                txt_ErrName_KOR.Visible = false;     // 2021.09.06 SungTae : [추가] (ASE-KR VOC) Error명 한글 표시 요청
            }
            else
            {
                rdb_ErrList.Text = "ERROR";
                rdb_ErrHis.Visible = false;

                btn_ErrHIsView.Enabled = false;
                btn_ErrHisSave.Enabled = false;

                btn_ErrHisView2.Enabled = (int)RetLv >= CData.Opt.iLvSpcErrHisView;
                btn_ErrHisSave2.Enabled = (int)RetLv >= CData.Opt.iLvSpcErrHisSave;

                pnl_TabCtrlErr.Visible = false;
                tab_ErrorInfo.Visible = true;

                txt_ErrName_KOR.Visible = true;     // 2021.09.06 SungTae : [추가] (ASE-KR VOC) Error명 한글 표시 요청
            }
            // 2021.04.12 SungTae End


            //201004, jhLee : SPIL사의 요청으로 Min/Max/Mean 값 표시하도록 구현
            if (CDataOption.nDispDataExtend == 0)     // SPIL 외에는 Min/Max 표시 그래프와 범례를 감춘다.
            {
                chart_Max.Titles["TitleAverage"].Visible = false;       //  표시하지 않음
            }
            else
            {
                chart_Max.Titles["TitleAverage"].Visible = true;        //  표시함.
                chart_Max.Titles["TitleAverage"].Text =                 // 초기값 표시
                        string.Format("MAX : {0:0.0000}\nMIN : {1:0.0000}\nMean : {2:0.0000}", 0.0, 0.0, 0.0);
            }

        }

        public void InitPage()
        {
            tabSpc.SelectTab(0);
            rdb_Spc.Checked = true;
        }

        private void btn_ErrHisSave_Click(object sender, EventArgs e)
        {
            string sPath = "";
            string sLine = "";

            StreamWriter sw;
            sfd_ErrHis.ShowDialog();

            if(sfd_ErrHis.FileName != "")
            {
                sPath = sfd_ErrHis.FileName;

                sw = new StreamWriter(sPath, true, Encoding.GetEncoding("euc-kr"));

                sLine = col_No.HeaderText + ",";
                sLine += Col_ErrNo.HeaderText + ",";
                sLine += col_ErrName.HeaderText + ",";
                sLine += col_ErrOccurDate.HeaderText + ",";
                sLine += col_ErrRelease.HeaderText;
                sw.WriteLine(sLine);

                for(int iRow = 0; iRow < dgv_ErrHis.Rows.Count - 1; iRow++)
                {
                    sLine = "";
                    sLine = dgv_ErrHis.Rows[iRow].Cells[0].Value.ToString() + ",";
                    sLine += dgv_ErrHis.Rows[iRow].Cells[1].Value.ToString() + ",";
                    sLine += dgv_ErrHis.Rows[iRow].Cells[2].Value.ToString() + ",";
                    sLine += dgv_ErrHis.Rows[iRow].Cells[3].Value.ToString() + ",";
                    sLine += dgv_ErrHis.Rows[iRow].Cells[4].Value.ToString() + ",";
                    sw.WriteLine(sLine);
                }

                sLine = "FROM,";
                sLine += dtp_ErrHisFrom.Text;
                sw.WriteLine(sLine);

                sLine = "TO,";
                sLine += dtp_ErrHisTo.Text;
                sw.WriteLine(sLine);

                sLine = "MTBA," + lb_Mtba.Text;
                sw.WriteLine(sLine);

                sLine = "MTBF," + lb_Mtbf.Text;
                sw.WriteLine(sLine);

                sw.Close();
            }
        }

        private void chart_Max_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                m_iChartMaxS_X = (int)chart_Max.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X);

                if (e.Location.Y >= 0)  { m_dChartMaxS_Y = (double)chart_Max.ChartAreas[0].AxisY.PixelPositionToValue(e.Location.Y); }
                else                    { m_dChartMaxS_Y = 0; }
            }
        }

        private void chart_Max_MouseUp(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                m_iChartMaxE_X = (int)chart_Max.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X);

                if (e.Location.Y >= 0)  { m_dChartMaxE_Y = (double)chart_Max.ChartAreas[0].AxisY.PixelPositionToValue(e.Location.Y); }
                else                    { m_dChartMaxE_Y = 0; }

                if (m_iChartMaxE_X > m_iChartMaxS_X)
                { chart_Max.ChartAreas[0].AxisX.ScaleView.Zoom(m_iChartMaxS_X, m_iChartMaxE_X); }
                if (m_dChartMaxE_Y < m_dChartMaxS_Y)
                { chart_Max.ChartAreas[0].AxisY.ScaleView.Zoom(m_dChartMaxE_Y, m_dChartMaxS_Y); }
            }
        }

        private void chart_Max_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                chart_Max.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                chart_Max.ChartAreas[0].AxisY.ScaleView.ZoomReset();
            }
        }

        private void chart_Ttv_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                m_iChartTtvS_X = (int)chart_Ttv.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X);

                if (e.Location.Y >= 0)  { m_dChartTtvS_Y = (double)chart_Ttv.ChartAreas[0].AxisY.PixelPositionToValue(e.Location.Y); }
                else                    { m_dChartTtvS_Y = 0; }
            }
        }

        private void chart_Ttv_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                m_iChartTtvE_X = (int)chart_Ttv.ChartAreas[0].AxisX.PixelPositionToValue(e.Location.X);

                if (e.Location.Y >= 0)  { m_dChartTtvE_Y = (double)chart_Ttv.ChartAreas[0].AxisY.PixelPositionToValue(e.Location.Y); }
                else                    { m_dChartTtvE_Y = 0; }

                if (m_iChartTtvE_X > m_iChartTtvS_X)    { chart_Ttv.ChartAreas[0].AxisX.ScaleView.Zoom(m_iChartTtvS_X, m_iChartTtvE_X); }
                if (m_dChartTtvE_Y < m_dChartTtvS_Y)    { chart_Ttv.ChartAreas[0].AxisY.ScaleView.Zoom(m_dChartTtvE_Y, m_dChartTtvS_Y); }
            }
        }

        private void chart_Ttv_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                chart_Ttv.ChartAreas[0].AxisX.ScaleView.ZoomReset();
                chart_Ttv.ChartAreas[0].AxisY.ScaleView.ZoomReset();
            }
        }

        /// <summary>
        /// Max 2020103 : SCK+ Rader & Error Text Modify
        /// </summary>
        private void btn_ErrTextModify_Click(object sender, EventArgs e)
        {
            if (dgv_ErrList.SelectedCells.Count == 0)               { return; }

            // 빈 행을 선택시 예외처리 
            // 190328 -maeng
            if (dgv_ErrList.SelectedRows[0].Cells[0].Value == null) { return; }
            
            int iCellNo = dgv_ErrList.SelectedCells[0].RowIndex;

            if (dgv_ErrList.SelectedCells[0].RowIndex == (int)eErr.ERR_MAXCOUNT)
            {
                return;
            }

            dgv_ErrList[2, iCellNo].Value = txt_ErrAction.Text;
            CData.ErrList[iCellNo].sAction = dgv_ErrList[2, iCellNo].Value.ToString();

            CErr.SaveErr();

            CErr.Load();
        }

        // 2020.12.07 JSKim St - 기존 Radar 삭제
        ///// <summary>
        ///// Max 2020103 : SCK+ Rader & Error Text Modify
        ///// </summary>
        //private void btn_RaderSetModify_Click(object sender, EventArgs e)
        //{
        //    if (dgv_ErrList.SelectedCells.Count == 0)
        //    { return; }
        //    // 빈 행을 선택시 예외처리 
        //    // 190328 -maeng
        //    if (dgv_ErrList.SelectedRows[0].Cells[0].Value == null)
        //    { return; }

        //    int iCellNo = dgv_ErrList.SelectedCells[0].RowIndex;

        //    if (dgv_ErrList.SelectedCells[0].RowIndex == (int)eErr.ERR_MAXCOUNT)
        //    {
        //        return;
        //    }

        //    CErr.RaderSetModify(iCellNo, cb_RaderUse.Checked, txt_RaderCounter.Text.ToString());

        //    CErr.Load();
        //}
        // 2020.12.07 JSKim Ed

        // 2020.12.09 JSKim St - MTBF, MTBA, MTTR 동작이 안됨... 삭제
        ///// <summary>
        ///// Max 2020103 : SCK+ Rader & Error Text Modify
        ///// </summary>
        //private void btn_MTBF_MTBA_MTTR_reset_Click(object sender, EventArgs e)
        //{
        //    CData.SwTotalRunTim_MC.Reset();
        //    CData.SwTotalStopTim_MC.Reset();
        //    CData.SwTotalJamTim_MC.Reset();

        //    string Temp;
        //    Temp = @"00:00:00";
        //    CData.dtTotalRunTim_MC = Convert.ToDateTime(Temp);
        //    CData.dtTotalStopTim_MC = Convert.ToDateTime(Temp);
        //    CData.dtTotalJamTim_MC = Convert.ToDateTime(Temp);
        //}
        // 2020.12.09 JSKim Ed

        // 2020.12.07 JSKim St
        private void btn_RadarList_Click(object sender, EventArgs e)
        {
            vwSPC_Radar mSPC_RadarForm = new vwSPC_Radar();
            mSPC_RadarForm.ShowDialog();
        }
        // 2020.12.07 JSKim Ed

        // 2020.12.10 JSKim St
        private void btn_LotErrView_Click(object sender, EventArgs e)
        {
            vwSPC_LotErrInfo mSPC_LotErrInfoForm = new vwSPC_LotErrInfo(m_strErrPath, m_strInfoPath);
            mSPC_LotErrInfoForm.ShowDialog();
        }
        // 2020.12.10 JSKim Ed

        // 2021.04.12 SungTae Start : 고객사(ASE-KR) 요청에 따라 Error Info를 한 곳에서 확인하기 위해 추가
        private void btn_ErrHisSave2_Click(object sender, EventArgs e)
        {
            string sPath = "";
            string sLine = "";

            StreamWriter sw;
            sfd_ErrHis.ShowDialog();

            if (sfd_ErrHis.FileName != "")
            {
                sPath = sfd_ErrHis.FileName;

                sw = new StreamWriter(sPath, true, Encoding.GetEncoding("euc-kr"));

                sLine = col_No.HeaderText + ",";
                sLine += Col_ErrNo.HeaderText + ",";
                sLine += col_ErrName.HeaderText + ",";
                sLine += col_ErrOccurDate.HeaderText + ",";
                sLine += col_ErrRelease.HeaderText;

                sw.WriteLine(sLine);

                for (int iRow = 0; iRow < dgv_ErrHistory.Rows.Count - 1; iRow++)
                {
                    sLine = "";
                    sLine = dgv_ErrHistory.Rows[iRow].Cells[0].Value.ToString() + ",";
                    sLine += dgv_ErrHistory.Rows[iRow].Cells[1].Value.ToString() + ",";
                    sLine += dgv_ErrHistory.Rows[iRow].Cells[2].Value.ToString() + ",";
                    sLine += dgv_ErrHistory.Rows[iRow].Cells[3].Value.ToString() + ",";
                    sLine += dgv_ErrHistory.Rows[iRow].Cells[4].Value.ToString() + ",";
                    sw.WriteLine(sLine);
                }

                sLine = "FROM,";
                sLine += dtp_ErrHisFrom.Text;
                sw.WriteLine(sLine);

                sLine = "TO,";
                sLine += dtp_ErrHisTo.Text;
                sw.WriteLine(sLine);

                //sLine = "MTBA," + lb_Mtba.Text;
                //sw.WriteLine(sLine);

                //sLine = "MTBF," + lb_Mtbf.Text;
                //sw.WriteLine(sLine);

                sw.Close();
            }
        }

        private void btn_ErrHisView2_Click(object sender, EventArgs e)
        {
            LoadErrHistory2();
        }

        private void dgv_ErrList2_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv_ErrList2.SelectedCells.Count == 0)                  return;
            if (dgv_ErrList2.SelectedRows[0].Cells[0].Value == null)    return;

            int iCellNo = dgv_ErrList2.SelectedCells[0].RowIndex;

            FileInfo fi;

            if (dgv_ErrList2.SelectedCells[0].RowIndex == (int)eErr.ERR_MAXCOUNT)
            {
                return;
            }

            txt_ErrEnum.Text    = CData.ErrList[iCellNo].sName;
            txt_ErrNo.Text      = "ERR" + Convert.ToInt32(CData.ErrList[iCellNo].sNo).ToString("D3");
            txt_ErrName.Text    = CData.ErrList[iCellNo].sName;
            txt_ErrAction.Text  = CData.ErrList[iCellNo].sAction.Replace("*", "\r\n");
            txt_ErrImgPath.Text = GV.PATH_ERRIMG + Convert.ToInt32(CData.ErrList[iCellNo].sNo).ToString("D3") + ".png";

            // 2021.09.06 SungTae Start : [추가] (ASE-KR VOC) Error명 한글 표시 요청
            if (CData.CurCompany == ECompany.ASE_KR)
            {
                txt_ErrName_KOR.Text = CData.ErrList[iCellNo].sKorName;
            }
            // 2021.09.06 SungTae End

            fi = new FileInfo(txt_ErrImgPath.Text);

            if (fi.Exists)  pic_ErrImg.Load(txt_ErrImgPath.Text);
            else            pic_ErrImg.Image = null;
        }

        private void dgv_ErrHistory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            FileInfo fi;

            int iErrNo = 0;

            if (e.RowIndex == -1) return;   // 2021.06.28 SungTae : [추가] 발생 Alarm List를 정렬해서 보기 위해 Header Title 클릭 시 SW 강제 종료되는 현상때문에 추가
            
            string sNum = dgv_ErrHistory.Rows[e.RowIndex].Cells[1].FormattedValue.ToString();

            if (sNum != "")
            {
                iErrNo = Convert.ToInt32(sNum);

                txt_ErrEnum.Text    = CData.ErrList[iErrNo - 1].sName;
                txt_ErrNo.Text      = "ERR" + Convert.ToInt32(CData.ErrList[iErrNo - 1].sNo).ToString("D3");
                txt_ErrName.Text    = CData.ErrList[iErrNo - 1].sName;
                txt_ErrAction.Text  = CData.ErrList[iErrNo - 1].sAction.Replace("*", "\r\n");
                txt_ErrImgPath.Text = GV.PATH_ERRIMG + Convert.ToInt32(CData.ErrList[iErrNo - 1].sNo).ToString("D3") + ".png";

                // 2021.09.06 SungTae Start : [추가] (ASE-KR VOC) Error명 한글 표시 요청
                if (CData.CurCompany == ECompany.ASE_KR)
                {
                    txt_ErrName_KOR.Text = CData.ErrList[iErrNo - 1].sKorName;
                }
                // 2021.09.06 SungTae End

                fi = new FileInfo(txt_ErrImgPath.Text);

                if (fi.Exists)  pic_ErrImg.Load(txt_ErrImgPath.Text);
                else            pic_ErrImg.Image = null;
            }
            else
            {
                txt_ErrEnum.Text    = "";
                txt_ErrNo.Text      = "ERR";
                txt_ErrName.Text    = "";
                txt_ErrAction.Text  = "";
                txt_ErrImgPath.Text = "";

                // 2021.09.06 SungTae Start : [추가] (ASE-KR VOC) Error명 한글 표시 요청
                if (CData.CurCompany == ECompany.ASE_KR)
                {
                    txt_ErrName_KOR.Text = "";
                }
                // 2021.09.06 SungTae End

                pic_ErrImg.Image = null;
            }
        }
        // 2021.04.12 SungTae End
    }
}
