using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Text;

namespace SG2000X
{
    public partial class vwWhl : UserControl
    {
        /// <summary>
        /// 리스트에서 선택된 항목의 인덱스
        /// </summary>
        //private int m_iList;
        private static CTim m_Kill = new CTim();
        private int m_iLeft;
        private int m_iRight;
        //211129 pjh : Dresser Left/Right Index
        private int m_iLeft_Drs;
        private int m_iRight_Drs;
        //
        /// <summary>
        /// 현재 탭 페이지의 인덱스
        /// </summary>
        private EWay m_eWy;
        //2021.05.19 lhs Start 
        /// <summary>
        /// true = History 모두 보기(WheelHistory.csv와 날짜별 데이터), false = 날짜별 데이터에서 From ~ To 보기
        /// </summary>
        private bool m_bHistoryAllView;
        //2021.05.19 lhs End

        /// <summary>
        /// 220104 pjh
        /// true = Dresser History 모두 보기, false = 날짜별 Data에서 From ~ To 보기
        /// </summary>
        private bool m_bDrsHistoryAllView;

        /// <summary>
        /// Wheel Data Struct
        /// 현재 리스트에서 선택된 휠 파일의 데이터 구조체
        /// </summary>
        private tWhl m_tWhl;

        //211122 pjh : Dresser 별도 관리
        /// <summary>
        /// Dresser 정보 구조체
        /// </summary>
        private TDrs m_tDrs;

        public vwWhl()
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

            m_iLeft       = -1;
            m_iRight      = -1;
            m_eWy         = EWay.L;
            m_tWhl        = new tWhl();
            m_tWhl.aUsedP = new tStep[2];
            m_tWhl.aNewP  = new tStep[2];

            if(CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)   
            {   SetHistoryAllView(true);    }
            else
            {   SetHistoryAllView(false);   }

            if (CData.CurCompany != ECompany.Qorvo    && CData.CurCompany != ECompany.Qorvo_DZ &&
                CData.CurCompany != ECompany.Qorvo_RT && CData.CurCompany != ECompany.Qorvo_NC &&       // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가 
                CData.CurCompany != ECompany.ASE_KR   && CData.CurCompany != ECompany.SST      &&
                CData.CurCompany != ECompany.SCK      && CData.CurCompany != ECompany.JSCK) //200121 ksg :
            {
                btnL_WhlChange   .Visible = false;
                btnL_DrsChange   .Visible = false;
                btnR_WhlChange   .Visible = false;
                btnR_DrsChange   .Visible = false;
                btnL_ChangeCancel.Visible = false;
                btnR_ChangeCancel.Visible = false;
            }

            // 2020.11.23 JSKim St
            if (CDataOption.IsDrsAirCutReplace)
            {
                label95.Text = "Dresser Information(Using)";
                panel13.Visible = true;
                label103.Text = "Dresser Information(Replace)";

                label45.Text = "Dresser Information(Using)";
                panel14.Visible = true;
                label105.Text = "Dresser Information(Replace)";
            }
            else
            {
                label95.Text = "Dresser Information";
                panel13.Visible = false;
                label103.Text = "Dresser Information(Replace)";

                label45.Text = "Dresser Information";
                panel14.Visible = false;
                label105.Text = "Dresser Information(Replace)";
            }
            // 2020.11.23 JSKim Ed

            // 200727 jym : 휠 히스토리 내용추가
            if (CData.CurCompany == ECompany.ASE_KR && GV.WHEEL_HISTORY)
            {
                dgvL_HIst.Columns.Clear();
                dgvL_HIst.Columns.Add("dgcL_Date", "Date");
                dgvL_HIst.Columns.Add("dgcL_Tbl", "Table Position");
                dgvL_HIst.Columns.Add("dgcL_ID", "Wheel ID");
                dgvL_HIst.Columns.Add("dgcL_Part", "Wheel part NO");
                dgvL_HIst.Columns.Add("dgcL_Mesh", "Mesh");
                dgvL_HIst.Columns.Add("dgcL_WhlBf", "Start Tip Height[mm]");
                dgvL_HIst.Columns.Add("dgcL_WhlLoss", "Tip Loss[mm]");
                dgvL_HIst.Columns.Add("dgcL_WhlAf", "End Tip Height[mm]");
                dgvL_HIst.Columns.Add("dgcL_Drs", "Dresser Name");
                dgvL_HIst.Columns.Add("dgcL_DrsBf", "Start Dresser Height[mm]");
                dgvL_HIst.Columns.Add("dgcL_DrsLoss", "Dresser Loss[mm]");
                dgvL_HIst.Columns.Add("dgcL_WhlBf", "End Dresser Height[mm]");
                dgvL_HIst.Columns.Add("dgcL_GrdCnt", "Grinding Count[ea]");
                dgvL_HIst.Columns.Add("dgcL_DrsCnt", "Dressing Count[ea]");
                dgvL_HIst.Columns.Add("dgcL_DrsPeriod", "Cycle Dressing Strip");
                dgvL_HIst.Columns.Add("dgcL_1Strip", "1 Strip Loss[mm]");

                dgvR_HIst.Columns.Clear();
                dgvR_HIst.Columns.Add("dgcR_Date", "Date");
                dgvR_HIst.Columns.Add("dgcR_Tbl", "Table Position");
                dgvR_HIst.Columns.Add("dgcR_ID", "Wheel ID");
                dgvR_HIst.Columns.Add("dgcR_Part", "Wheel part NO");
                dgvR_HIst.Columns.Add("dgcR_Mesh", "Mesh");
                dgvR_HIst.Columns.Add("dgcR_WhlBf", "Start Tip Height[mm]");
                dgvR_HIst.Columns.Add("dgcR_WhlLoss", "Tip Loss[mm]");
                dgvR_HIst.Columns.Add("dgcR_WhlAf", "End Tip Height[mm]");
                dgvR_HIst.Columns.Add("dgcR_Drs", "Dresser Name");
                dgvR_HIst.Columns.Add("dgcR_DrsBf", "Start Dresser Height[mm]");
                dgvR_HIst.Columns.Add("dgcR_DrsLoss", "Dresser Loss[mm]");
                dgvR_HIst.Columns.Add("dgcR_WhlBf", "End Dresser Height[mm]");
                dgvR_HIst.Columns.Add("dgcR_GrdCnt", "Grinding Count[ea]");
                dgvR_HIst.Columns.Add("dgcR_DrsCnt", "Dressing Count[ea]");
                dgvR_HIst.Columns.Add("dgcR_DrsPeriod", "Cycle Dressing Strip");
                dgvR_HIst.Columns.Add("dgcR_1Strip", "1 Strip Loss[mm]");

                lblL_Part.Visible = true;
                lblR_Part.Visible = true;
                txtL_Part.Visible = true;
                txtR_Part.Visible = true;
                lblL_Mesh.Visible = true;
                lblR_Mesh.Visible = true;
                txtL_Mesh.Visible = true;
                txtR_Mesh.Visible = true;
            }
			//220106 pjh : Dresser History Column 명칭
            if(CDataOption.UseSeperateDresser)
            {
                dgvL_DrsHIst.Columns.Clear();
                dgvL_DrsHIst.Columns.Add("dgcL_Date", "Date");
                dgvL_DrsHIst.Columns.Add("dgcL_Tbl", "Measure Type");                
                dgvL_DrsHIst.Columns.Add("dgcL_Drs", "Dresser Name");
                dgvL_DrsHIst.Columns.Add("dgcL_DrsBf", "Dresser Height[mm]");
                dgvL_DrsHIst.Columns.Add("dgcL_DrsLoss", "Dresser Loss[mm]");

                dgvR_DrsHIst.Columns.Clear();
                dgvR_DrsHIst.Columns.Add("dgcR_Date", "Date");
                dgvR_DrsHIst.Columns.Add("dgcR_Tbl", "Measure Type");
                dgvR_DrsHIst.Columns.Add("dgcR_Drs", "Dresser Name");
                dgvR_DrsHIst.Columns.Add("dgcR_DrsBf", "Dresser Height[mm]");
                dgvR_DrsHIst.Columns.Add("dgcR_DrsLoss", "Dresser Loss[mm]");

                lblL_Part.Visible = true;
                lblR_Part.Visible = true;
                txtL_Part.Visible = true;
                txtR_Part.Visible = true;
                lblL_Mesh.Visible = true;
                lblR_Mesh.Visible = true;
                txtL_Mesh.Visible = true;
                txtR_Mesh.Visible = true;
            }
			//
        }

        #region Basic method
        public void Open()
        {
            string sName = "";

            //211012 pjh : Device에서 Wheel 관리 시 Wheel 메뉴 Disable
            if (CDataOption.UseDeviceWheel){ LabelEnable(false); }
            else                           { LabelEnable(true);  }
            //

            if(CDataOption.UseSeperateDresser)
            {
                if (m_eWy == EWay.L && CData.Whls[0].sWhlName != "")
                {
                    sName = CData.Whls[0].sWhlName;
                    lbxL_List_Whl.SelectedItem = CData.Whls[0].sWhlName;
                }
                else if (m_eWy == EWay.R && CData.Whls[1].sWhlName != "")
                {
                    sName = CData.Whls[1].sWhlName;
                    lbxR_List_Whl.SelectedItem = CData.Whls[1].sWhlName;
                }
                tbcon_L.Visible = true;
                lbxL_List.Visible = false;
                tbcon_R.Visible = true;
                lbxR_List.Visible = false;
            }
            else
            {
                if (m_eWy == EWay.L && CData.Whls[0].sWhlName != "")
                {
                    sName = CData.Whls[0].sWhlName;
                    lbxL_List.SelectedItem = CData.Whls[0].sWhlName;
                }
                else if (m_eWy == EWay.R && CData.Whls[1].sWhlName != "")
                {
                    sName = CData.Whls[1].sWhlName;
                    lbxR_List.SelectedItem = CData.Whls[1].sWhlName;
                }
                lbxL_List.Visible = true;
                tbcon_L.Visible = false;
                lbxR_List.Visible = true;
                tbcon_R.Visible = false;
            }

            if (CData.CurCompany != ECompany.Qorvo    && CData.CurCompany != ECompany.Qorvo_DZ &&
                CData.CurCompany != ECompany.Qorvo_RT && CData.CurCompany != ECompany.Qorvo_NC &&       // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가 
                CData.CurCompany != ECompany.ASE_KR   && CData.CurCompany != ECompany.SST      &&
                CData.CurCompany != ECompany.SCK      && CData.CurCompany != ECompany.JSCK) //200121 ksg :
            {
                btnL_WhlChange   .Visible = false;
                btnL_DrsChange   .Visible = false;
                btnR_WhlChange   .Visible = false;
                btnR_DrsChange   .Visible = false;
                btnL_ChangeCancel.Visible = false;
                btnR_ChangeCancel.Visible = false;
            }

            _ListUp();
            //211122 pjh :
            _ListUpDrs();

            if (CDataOption.UseSeperateDresser)
            {
                txtL_DrName.ReadOnly = true;
                txtL_DrName.BackColor = Color.LightGray;
                txtR_DrName.ReadOnly = true;
                txtR_DrName.BackColor = Color.LightGray;
            }
            //

            HideMenu(CData.Lev);

            // 2021.05.18 lhs Start
            //// 2021.04.06 SungTae Start : 
            //dtpHistoryL_From.Format = DateTimePickerFormat.Custom;
            //dtpHistoryL_From.CustomFormat = "yyyy-MM-dd";
            //dtpHistoryR_From.Format = DateTimePickerFormat.Custom;
            //dtpHistoryR_From.CustomFormat = "yyyy-MM-dd";
            //// 2021.04.06 SungTae End

            dtpHistoryL_From.Format = DateTimePickerFormat.Custom;
            dtpHistoryL_From.CustomFormat = "yyyy-MM-dd";
            dtpHistoryL_To.Format = DateTimePickerFormat.Custom;
            dtpHistoryL_To.CustomFormat = "yyyy-MM-dd";

            dtpHistoryR_From.Format = DateTimePickerFormat.Custom;
            dtpHistoryR_From.CustomFormat = "yyyy-MM-dd";
            dtpHistoryR_To.Format = DateTimePickerFormat.Custom;
            dtpHistoryR_To.CustomFormat = "yyyy-MM-dd";
        	
			//220106 pjh : Dresser History 조회 기간 표시 설정
            if(CDataOption.UseSeperateDresser)
            {
                dtpDrsHistoryL_From.Format = DateTimePickerFormat.Custom;
                dtpDrsHistoryL_From.CustomFormat = "yyyy-MM-dd";
                dtpDrsHistoryL_To.Format = DateTimePickerFormat.Custom;
                dtpDrsHistoryL_To.CustomFormat = "yyyy-MM-dd";

                dtpDrsHistoryR_From.Format = DateTimePickerFormat.Custom;
                dtpDrsHistoryR_From.CustomFormat = "yyyy-MM-dd";
                dtpDrsHistoryR_To.Format = DateTimePickerFormat.Custom;
                dtpDrsHistoryR_To.CustomFormat = "yyyy-MM-dd";
            }
            // 2021.05.18 lhs End

            //if (sName != "")
            //{ CWhl.It.Load(m_eWy, sName, out m_tWhl); }

            //_Set();
        }

        public void Close()
        {

        }

        public void Release()
        {

        }

        private void _Set()
        {
            string sPath = GV.PATH_WHEEL;

            if (m_eWy == EWay.L)
            {
                // Wheel information
                txtL_Name    .Text = m_tWhl.sWhlName;
                txtL_WhDt    .Text = m_tWhl.dtLast   .ToString();
                txtL_WhOuter .Text = m_tWhl.dWhlO    .ToString();
                txtL_WhTip   .Text = m_tWhl.dWhltH   .ToString();
                txtL_WhToCnt .Text = m_tWhl.iGtc     .ToString();
                txtL_WhDrCnt .Text = m_tWhl.iGdc     .ToString();
                txtL_WhToLoss.Text = m_tWhl.dWhltL   .ToString();
                txtL_WhStLoss.Text = m_tWhl.dWhloL   .ToString();
                txtL_WhDrLoss.Text = m_tWhl.dWhldoL  .ToString();
                txtL_WhCyLoss.Text = m_tWhl.dWhldcL  .ToString();
                //txtL_Air     .Text = m_tWhl.dDair    .ToString();
                // 2020.11.23 JSKim St
                //txtL_Air_Rep .Text = m_tWhl.dDairRep .ToString();
                // 2020.11.23 JSKim Ed

                // 200727 jym : 휠 히스토리 내용추가
                if (CData.CurCompany == ECompany.ASE_KR && GV.WHEEL_HISTORY)
                {
                    txtL_Part.Text = m_tWhl.sPartNo;
                    txtL_Mesh.Text = m_tWhl.sMesh;
                }

                // Dresser information
                if (CDataOption.UseSeperateDresser)
                {
                    txtL_DrName.Text  = m_tDrs.sDrsName;
                    txtL_DrOuter.Text = m_tDrs.dDrsOuter.ToString();
                    txtL_DrHei.Text   = m_tDrs.dDrsH.ToString();
                }
                else
                {
                    txtL_DrName.Text  = m_tWhl.sDrsName;
                    txtL_DrOuter.Text = m_tWhl.dDrsOuter.ToString();
                    txtL_DrHei.Text   = m_tWhl.dDrsH.ToString();
                }
         		//220106 pjh : Device Wheel 사용 할 때 Device 파라미터로 표시
                if (CDataOption.UseDeviceWheel)
                {
                    //211012 pjh : Dressing Air Cut
                    txtL_Air.Text = CDev.It.a_tWhl[0].dDair.ToString();

                    if (CDataOption.IsDrsAirCutReplace)
                    {
                        txtL_Air_Rep.Text = CDev.It.a_tWhl[0].dDairRep.ToString();
                    }

                    // Using 01
                    cmbL_U1Mod.SelectedIndex = (int)CDev.It.a_tWhl[0].aUsedP[0].eMode;
                    txtL_U1To.Text = CDev.It.a_tWhl[0].aUsedP[0].dTotalDep.ToString();
                    txtL_U1Cy.Text = CDev.It.a_tWhl[0].aUsedP[0].dCycleDep.ToString();
                    txtL_U1Tb.Text = CDev.It.a_tWhl[0].aUsedP[0].dTblSpd.ToString();
                    txtL_U1Sp.Text = CDev.It.a_tWhl[0].aUsedP[0].iSplSpd.ToString();
                    cmbL_U1Dir.SelectedIndex = (int)CDev.It.a_tWhl[0].aUsedP[0].eDir;

                    // Using 02
                    cmbL_U2Mod.SelectedIndex = (int)CDev.It.a_tWhl[0].aUsedP[1].eMode;
                    txtL_U2To.Text = CDev.It.a_tWhl[0].aUsedP[1].dTotalDep.ToString();
                    txtL_U2Cy.Text = CDev.It.a_tWhl[0].aUsedP[1].dCycleDep.ToString();
                    txtL_U2Tb.Text = CDev.It.a_tWhl[0].aUsedP[1].dTblSpd.ToString();
                    txtL_U2Sp.Text = CDev.It.a_tWhl[0].aUsedP[1].iSplSpd.ToString();
                    cmbL_U2Dir.SelectedIndex = (int)CDev.It.a_tWhl[0].aUsedP[1].eDir;

                    // Replace 01
                    cmbL_R1Mod.SelectedIndex = (int)CDev.It.a_tWhl[0].aNewP[0].eMode;
                    txtL_R1To.Text = CDev.It.a_tWhl[0].aNewP[0].dTotalDep.ToString();
                    txtL_R1Cy.Text = CDev.It.a_tWhl[0].aNewP[0].dCycleDep.ToString();
                    txtL_R1Tb.Text = CDev.It.a_tWhl[0].aNewP[0].dTblSpd.ToString();
                    txtL_R1Sp.Text = CDev.It.a_tWhl[0].aNewP[0].iSplSpd.ToString();
                    cmbL_R1Dir.SelectedIndex = (int)CDev.It.a_tWhl[0].aNewP[0].eDir;

                    // Replace 02
                    cmbL_R2Mod.SelectedIndex = (int)CDev.It.a_tWhl[0].aNewP[1].eMode;
                    txtL_R2To.Text = CDev.It.a_tWhl[0].aNewP[1].dTotalDep.ToString();
                    txtL_R2Cy.Text = CDev.It.a_tWhl[0].aNewP[1].dCycleDep.ToString();
                    txtL_R2Tb.Text = CDev.It.a_tWhl[0].aNewP[1].dTblSpd.ToString();
                    txtL_R2Sp.Text = CDev.It.a_tWhl[0].aNewP[1].iSplSpd.ToString();
                    cmbL_R2Dir.SelectedIndex = (int)CDev.It.a_tWhl[0].aNewP[1].eDir;

                    //211210 pjh : Skyworks Wheel&Dresser Limit Device에서 편집
                    txtL_WhlLimit.Text = CDev.It.a_tWhl[0].dWhlLimit.ToString();
                    txtL_DrsLimit.Text = CDev.It.a_tWhl[0].dDrsLimit.ToString();
                    //
                }
                else
                {
                    txtL_Air.Text = m_tWhl.dDair.ToString();

                    if (CDataOption.IsDrsAirCutReplace)
                    {
                        txtL_Air_Rep.Text = m_tWhl.dDairRep.ToString();
                    }

                    // Using 01
                    cmbL_U1Mod.SelectedIndex = (int)m_tWhl.aUsedP[0].eMode;
                    txtL_U1To.Text = m_tWhl.aUsedP[0].dTotalDep.ToString();
                    txtL_U1Cy.Text = m_tWhl.aUsedP[0].dCycleDep.ToString();
                    txtL_U1Tb.Text = m_tWhl.aUsedP[0].dTblSpd.ToString();
                    txtL_U1Sp.Text = m_tWhl.aUsedP[0].iSplSpd.ToString();
                    cmbL_U1Dir.SelectedIndex = (int)m_tWhl.aUsedP[0].eDir;

                    // Using 02
                    cmbL_U2Mod.SelectedIndex = (int)m_tWhl.aUsedP[1].eMode;
                    txtL_U2To.Text = m_tWhl.aUsedP[1].dTotalDep.ToString();
                    txtL_U2Cy.Text = m_tWhl.aUsedP[1].dCycleDep.ToString();
                    txtL_U2Tb.Text = m_tWhl.aUsedP[1].dTblSpd.ToString();
                    txtL_U2Sp.Text = m_tWhl.aUsedP[1].iSplSpd.ToString();
                    cmbL_U2Dir.SelectedIndex = (int)m_tWhl.aUsedP[1].eDir;

                    // Replace 01
                    cmbL_R1Mod.SelectedIndex = (int)m_tWhl.aNewP[0].eMode;
                    txtL_R1To.Text = m_tWhl.aNewP[0].dTotalDep.ToString();
                    txtL_R1Cy.Text = m_tWhl.aNewP[0].dCycleDep.ToString();
                    txtL_R1Tb.Text = m_tWhl.aNewP[0].dTblSpd.ToString();
                    txtL_R1Sp.Text = m_tWhl.aNewP[0].iSplSpd.ToString();
                    cmbL_R1Dir.SelectedIndex = (int)m_tWhl.aNewP[0].eDir;

                    // Replace 02
                    cmbL_R2Mod.SelectedIndex = (int)m_tWhl.aNewP[1].eMode;
                    txtL_R2To.Text = m_tWhl.aNewP[1].dTotalDep.ToString();
                    txtL_R2Cy.Text = m_tWhl.aNewP[1].dCycleDep.ToString();
                    txtL_R2Tb.Text = m_tWhl.aNewP[1].dTblSpd.ToString();
                    txtL_R2Sp.Text = m_tWhl.aNewP[1].iSplSpd.ToString();
                    cmbL_R2Dir.SelectedIndex = (int)m_tWhl.aNewP[1].eDir;

                    //211210 pjh : Skyworks Wheel&Dresser Limit Device에서 편집
                    txtL_WhlLimit.Text = m_tWhl.dWhlLimit.ToString();
                    txtL_DrsLimit.Text = m_tWhl.dDrsLimit.ToString();
                    //
                }
                ////히스토리
                //int iRowCnt = 0;
                //string[] sData;
                //string[] sHeader;
                //string sRowValue = "";

                //// 2021.04.06 SungTae Start : Wheel History 파일의 무한정 크기 증가로 인한 랙 발생으로 Daily 별로 Load하도록 변경
                //sPath += "Left\\";
                ////sPath += m_tWhl.sWhlName + "\\";
                ////sPath += "WheelHistory.csv";
                //sPath += m_tWhl.sWhlName + "\\WheelHistory\\";
                //sPath += dtpHistory.Text + ".csv";
                //// 2021.04.06 SungTae End

                //FileInfo fI = new FileInfo(sPath);
                //dgvL_HIst.Rows.Clear();
                //if(fI.Exists)
                //{
                //    //koo 191101 : error window
                //    try
                //    {
                //        string filename = Path.GetFileName(sPath);
                //        if (CLog.killps(filename)== true) m_Kill.Wait(2000);
                //        StreamReader sr = new StreamReader(sPath, Encoding.GetEncoding("euc-kr"));
                //        sRowValue = sr.ReadLine();
                //        //koo 191106 error split
                //        if (sRowValue==null)
                //        {
                //            string temp = sPath + " is Empty";
                //            CMsg.Show(eMsg.Error, "Error", temp);
                //            return ;
                //        }
                //        sHeader = sRowValue.Split(',');
                //        while (sr.Peek() > -1)
                //        {
                //            sRowValue = sr.ReadLine();
                //            //koo 191106 error split
                //            if (sRowValue == null)
                //            {
                //                string temp = sPath + " is Empty";
                //                CMsg.Show(eMsg.Error, "Error", temp);
                //                return;
                //            }
                //            sData = sRowValue.Split(',');
                //            dgvL_HIst.Rows.Add();
                //            int iCnt = sData.Length - 1;
                //            if (CData.CurCompany == ECompany.ASE_KR && GV.WHEEL_HISTORY)
                //            {
                //                dgvL_HIst[0, iRowCnt].Value = sData[0]; // Date
                //                dgvL_HIst[1, iRowCnt].Value = (iCnt > 20) ? sData[20] : ""; // Table
                //                dgvL_HIst[2, iRowCnt].Value = (iCnt > 20) ? sData[21] : ""; // Wheel ID
                //                dgvL_HIst[3, iRowCnt].Value = (iCnt > 20) ? sData[22] : ""; // Wheel Part
                //                dgvL_HIst[4, iRowCnt].Value = (iCnt > 20) ? sData[23] : "0"; // Mesh
                //                dgvL_HIst[5, iRowCnt].Value = (iCnt > 20) ? sData[24] : "0"; // Start Wheel Tip
                //                dgvL_HIst[6, iRowCnt].Value = (iCnt > 20) ? sData[25] : "0"; // Wheel Loss
                //                dgvL_HIst[7, iRowCnt].Value = (iCnt > 20) ? sData[26] : "0"; // End Wheel Tip
                //                dgvL_HIst[8, iRowCnt].Value = sData[4]; // Dresser name
                //                dgvL_HIst[9, iRowCnt].Value  = (iCnt > 20) ? sData[27] : "0"; // Start Dresser Tip
                //                dgvL_HIst[10, iRowCnt].Value = (iCnt > 20) ? sData[28] : "0"; // Dresser Loss
                //                dgvL_HIst[11, iRowCnt].Value = (iCnt > 20) ? sData[29] : "0"; // End Dresser Tip
                //                dgvL_HIst[12, iRowCnt].Value = sData[7]; // Grinding count
                //                dgvL_HIst[13, iRowCnt].Value = sData[8]; // Grinding count
                //                dgvL_HIst[14, iRowCnt].Value = sData[9]; // Grinding count
                //                dgvL_HIst[15, iRowCnt].Value = sData[10]; // Grinding count
                //            }
                //            else
                //            {
                //                iCnt = 20;
                //                for (int i = 0; i < iCnt; i++)
                //                {
                //                    dgvL_HIst[i, iRowCnt].Value = sData[i];
                //                }
                //            }

                //            iRowCnt++;
                //        }
                //        sr.Close();
                //    }
                //    catch (Exception ex)
                //    {
                //        CMsg.Show(eMsg.Error, "Error", ex.Message);
                //        return ;
                //    }
                //}
                ////
            }
            else
            {
                // Wheel information
                txtR_Name    .Text = m_tWhl.sWhlName;
                txtR_WhDt    .Text = m_tWhl.dtLast   .ToString();
                txtR_WhOuter .Text = m_tWhl.dWhlO    .ToString();
                txtR_WhTip   .Text = m_tWhl.dWhltH   .ToString();
                txtR_WhToCnt .Text = m_tWhl.iGtc     .ToString();
                txtR_WhDrCnt .Text = m_tWhl.iGdc     .ToString();
                txtR_WhToLoss.Text = m_tWhl.dWhltL   .ToString();
                txtR_WhStLoss.Text = m_tWhl.dWhloL   .ToString();
                txtR_WhDrLoss.Text = m_tWhl.dWhldoL  .ToString();
                txtR_WhCyLoss.Text = m_tWhl.dWhldcL  .ToString();
                //txtR_Air     .Text = m_tWhl.dDair    .ToString();
                // 2020.11.23 JSKim St
                //txtR_Air_Rep .Text = m_tWhl.dDairRep .ToString();
                // 2020.11.23 JSKim Ed
                //if (CDataOption.UseDeviceWheel) txtR_WhlLimit.Text = CData.Whls[1].dWhlLimit.ToString();
                //else txtR_WhlLimit.Text = CData.Whls[1].dWhlLimit.ToString();

                // 200727 jym : 휠 히스토리 내용추가
                if (CData.CurCompany == ECompany.ASE_KR && GV.WHEEL_HISTORY)
                {
                    txtR_Part.Text = m_tWhl.sPartNo;
                    txtR_Mesh.Text = m_tWhl.sMesh;
                }

                // Dresser information
                if (CDataOption.UseSeperateDresser)
                {
                    txtR_DrName.Text  = m_tDrs.sDrsName;
                    txtR_DrOuter.Text = m_tDrs.dDrsOuter.ToString();
                    txtR_DrHei.Text   = m_tDrs.dDrsH.ToString();
                }
                else
                {
                    txtR_DrName.Text  = m_tWhl.sDrsName;
                    txtR_DrOuter.Text = m_tWhl.dDrsOuter.ToString();
                    txtR_DrHei.Text   = m_tWhl.dDrsH.ToString();
                }
                //if(CDataOption.UseDeviceWheel) txtR_DrsLimit.Text = CData.Whls[1].dDrsLimit.ToString();
                //else txtR_DrsLimit.Text = m_tWhl.dDrsLimit.ToString();
         		//220106 pjh : Device Wheel 사용 할 때 Device 파라미터로 표시
                if (CDataOption.UseDeviceWheel)
                {
                    //211012 pjh : Dressing Air Cut
                    txtR_Air.Text = CDev.It.a_tWhl[1].dDair.ToString();

                    if (CDataOption.IsDrsAirCutReplace)
                    {
                        txtR_Air_Rep.Text = CDev.It.a_tWhl[1].dDairRep.ToString();
                    }

                    // Using 01
                    cmbR_U1Mod.SelectedIndex = (int)CDev.It.a_tWhl[1].aUsedP[0].eMode;
                    txtR_U1To.Text = CDev.It.a_tWhl[1].aUsedP[0].dTotalDep.ToString();
                    txtR_U1Cy.Text = CDev.It.a_tWhl[1].aUsedP[0].dCycleDep.ToString();
                    txtR_U1Tb.Text = CDev.It.a_tWhl[1].aUsedP[0].dTblSpd.ToString();
                    txtR_U1Sp.Text = CDev.It.a_tWhl[1].aUsedP[0].iSplSpd.ToString();
                    cmbR_U1Dir.SelectedIndex = (int)CDev.It.a_tWhl[1].aUsedP[0].eDir;

                    // Using 02
                    cmbR_U2Mod.SelectedIndex = (int)CDev.It.a_tWhl[1].aUsedP[1].eMode;
                    txtR_U2To.Text = CDev.It.a_tWhl[1].aUsedP[1].dTotalDep.ToString();
                    txtR_U2Cy.Text = CDev.It.a_tWhl[1].aUsedP[1].dCycleDep.ToString();
                    txtR_U2Tb.Text = CDev.It.a_tWhl[1].aUsedP[1].dTblSpd.ToString();
                    txtR_U2Sp.Text = CDev.It.a_tWhl[1].aUsedP[1].iSplSpd.ToString();
                    cmbR_U2Dir.SelectedIndex = (int)CDev.It.a_tWhl[1].aUsedP[1].eDir;

                    // Replace 01
                    cmbR_R1Mod.SelectedIndex = (int)CDev.It.a_tWhl[1].aNewP[0].eMode;
                    txtR_R1To.Text = CDev.It.a_tWhl[1].aNewP[0].dTotalDep.ToString();
                    txtR_R1Cy.Text = CDev.It.a_tWhl[1].aNewP[0].dCycleDep.ToString();
                    txtR_R1Tb.Text = CDev.It.a_tWhl[1].aNewP[0].dTblSpd.ToString();
                    txtR_R1Sp.Text = CDev.It.a_tWhl[1].aNewP[0].iSplSpd.ToString();
                    cmbR_R1Dir.SelectedIndex = (int)CDev.It.a_tWhl[1].aNewP[0].eDir;

                    // Replace 02
                    cmbR_R2Mod.SelectedIndex = (int)CDev.It.a_tWhl[1].aNewP[1].eMode;
                    txtR_R2To.Text = CDev.It.a_tWhl[1].aNewP[1].dTotalDep.ToString();
                    txtR_R2Cy.Text = CDev.It.a_tWhl[1].aNewP[1].dCycleDep.ToString();
                    txtR_R2Tb.Text = CDev.It.a_tWhl[1].aNewP[1].dTblSpd.ToString();
                    txtR_R2Sp.Text = CDev.It.a_tWhl[1].aNewP[1].iSplSpd.ToString();
                    cmbR_R2Dir.SelectedIndex = (int)CDev.It.a_tWhl[1].aNewP[1].eDir;

                    //211210 pjh : Skyworks Wheel&Dresser Limit Device에서 편집
                    txtR_WhlLimit.Text = CDev.It.a_tWhl[1].dWhlLimit.ToString();
                    txtR_DrsLimit.Text = CDev.It.a_tWhl[1].dDrsLimit.ToString();
                    //
                }
                else
                {
                    //211012 pjh : Dressing Air Cut
                    txtR_Air.Text = m_tWhl.dDair.ToString();

                    if (CDataOption.IsDrsAirCutReplace)
                    {
                        txtR_Air_Rep.Text = m_tWhl.dDairRep.ToString();
                    }

                    // Using 01
                    cmbR_U1Mod.SelectedIndex = (int)m_tWhl.aUsedP[0].eMode;
                    txtR_U1To.Text = m_tWhl.aUsedP[0].dTotalDep.ToString();
                    txtR_U1Cy.Text = m_tWhl.aUsedP[0].dCycleDep.ToString();
                    txtR_U1Tb.Text = m_tWhl.aUsedP[0].dTblSpd.ToString();
                    txtR_U1Sp.Text = m_tWhl.aUsedP[0].iSplSpd.ToString();
                    cmbR_U1Dir.SelectedIndex = (int)m_tWhl.aUsedP[0].eDir;

                    // Using 02
                    cmbR_U2Mod.SelectedIndex = (int)m_tWhl.aUsedP[1].eMode;
                    txtR_U2To.Text = m_tWhl.aUsedP[1].dTotalDep.ToString();
                    txtR_U2Cy.Text = m_tWhl.aUsedP[1].dCycleDep.ToString();
                    txtR_U2Tb.Text = m_tWhl.aUsedP[1].dTblSpd.ToString();
                    txtR_U2Sp.Text = m_tWhl.aUsedP[1].iSplSpd.ToString();
                    cmbR_U2Dir.SelectedIndex = (int)m_tWhl.aUsedP[1].eDir;

                    // Replace 01
                    cmbR_R1Mod.SelectedIndex = (int)m_tWhl.aNewP[0].eMode;
                    txtR_R1To.Text = m_tWhl.aNewP[0].dTotalDep.ToString();
                    txtR_R1Cy.Text = m_tWhl.aNewP[0].dCycleDep.ToString();
                    txtR_R1Tb.Text = m_tWhl.aNewP[0].dTblSpd.ToString();
                    txtR_R1Sp.Text = m_tWhl.aNewP[0].iSplSpd.ToString();
                    cmbR_R1Dir.SelectedIndex = (int)m_tWhl.aNewP[0].eDir;

                    // Replace 02
                    cmbR_R2Mod.SelectedIndex = (int)m_tWhl.aNewP[1].eMode;
                    txtR_R2To.Text = m_tWhl.aNewP[1].dTotalDep.ToString();
                    txtR_R2Cy.Text = m_tWhl.aNewP[1].dCycleDep.ToString();
                    txtR_R2Tb.Text = m_tWhl.aNewP[1].dTblSpd.ToString();
                    txtR_R2Sp.Text = m_tWhl.aNewP[1].iSplSpd.ToString();
                    cmbR_R2Dir.SelectedIndex = (int)m_tWhl.aNewP[1].eDir;

                    //211210 pjh : Skyworks Wheel&Dresser Limit Device에서 편집
                    txtR_WhlLimit.Text = m_tWhl.dWhlLimit.ToString();
                    txtR_DrsLimit.Text = m_tWhl.dDrsLimit.ToString();
                    //
                }
                ////히스토리
                //int iRowCnt = 0;
                //string[] sData;
                //string[] sHeader;
                //string sRowValue = "";

                //// 2021.04.06 SungTae Start : Wheel History 파일의 무한정 크기 증가로 인한 랙 발생으로 Daily 별로 Load하도록 변경
                //sPath += "Right\\";
                ////sPath += m_tWhl.sWhlName + "\\";
                ////sPath += "WheelHistory.csv";
                //sPath += m_tWhl.sWhlName + "\\WheelHistory\\";
                //sPath += dtpHistory.Text + ".csv";
                //// 2021.04.06 SungTae End

                //FileInfo fI = new FileInfo(sPath);
                //dgvR_HIst.Rows.Clear();
                //if (fI.Exists)
                //{
                //    //koo 191101 : error window 
                //    try
                //    {
                //        string filename = Path.GetFileName(sPath);
                //        if (CLog.killps(filename)== true) m_Kill.Wait(2000);
                //        StreamReader sr = new StreamReader(sPath, Encoding.GetEncoding("euc-kr"));
                //        sRowValue = sr.ReadLine();
                //        //koo 191106 error split
                //        if (sRowValue==null)
                //        {
                //            string temp = sPath + " is Empty";
                //            CMsg.Show(eMsg.Error, "Error", temp);
                //            return ;
                //        }
                //        sHeader = sRowValue.Split(',');
                //        while (sr.Peek() > -1)
                //        {
                //            sRowValue = sr.ReadLine();
                //            //koo 191106 error split
                //            if (sRowValue==null)
                //            {
                //                string temp = sPath + " is Empty";
                //                CMsg.Show(eMsg.Error, "Error", temp);
                //                return ;
                //            }

                //            sData = sRowValue.Split(',');
                //            dgvR_HIst.Rows.Add();
                //            int iCnt = sData.Length - 1;
                //            if (CData.CurCompany == ECompany.ASE_KR && GV.WHEEL_HISTORY)
                //            {
                //                dgvR_HIst[0, iRowCnt].Value = sData[0]; // Date
                //                dgvR_HIst[1, iRowCnt].Value = (iCnt > 20) ? sData[20] : ""; // Table
                //                dgvR_HIst[2, iRowCnt].Value = (iCnt > 20) ? sData[21] : ""; // Wheel ID
                //                dgvR_HIst[3, iRowCnt].Value = (iCnt > 20) ? sData[22] : ""; // Wheel Part
                //                dgvR_HIst[4, iRowCnt].Value = (iCnt > 20) ? sData[23] : "0"; // Mesh
                //                dgvR_HIst[5, iRowCnt].Value = (iCnt > 20) ? sData[24] : "0"; // Start Wheel Tip
                //                dgvR_HIst[6, iRowCnt].Value = (iCnt > 20) ? sData[25] : "0"; // Wheel Loss
                //                dgvR_HIst[7, iRowCnt].Value = (iCnt > 20) ? sData[26] : "0"; // End Wheel Tip
                //                dgvR_HIst[8, iRowCnt].Value = sData[4]; // Dresser name
                //                dgvR_HIst[9, iRowCnt].Value  = (iCnt > 20) ? sData[27] : "0"; // Start Dresser Tip
                //                dgvR_HIst[10, iRowCnt].Value = (iCnt > 20) ? sData[28] : "0"; // Dresser Loss
                //                dgvR_HIst[11, iRowCnt].Value = (iCnt > 20) ? sData[29] : "0"; // End Dresser Tip
                //                dgvR_HIst[12, iRowCnt].Value = sData[7]; // Grinding count
                //                dgvR_HIst[13, iRowCnt].Value = sData[8]; // Grinding count
                //                dgvR_HIst[14, iRowCnt].Value = sData[9]; // Grinding count
                //                dgvR_HIst[15, iRowCnt].Value = sData[10]; // Grinding count
                //            }
                //            else
                //            {
                //                iCnt = 20;
                //                for (int i = 0; i < iCnt; i++)
                //                {
                //                    dgvR_HIst[i, iRowCnt].Value = sData[i];
                //                }
                //            }
                //            iRowCnt++;
                //        }
                //        sr.Close();
                //    }
                //    catch (Exception ex)
                //    {
                //        CMsg.Show(eMsg.Error, "Error", ex.Message);
                //        return ;
                //    }
                //}
                ////
            }

            //_HistoryUp(m_eWy);  2021.05.18 lhs 삭제

            // 2021.05.18 lhs Start
            if (m_bHistoryAllView)
            {
                SetHistoryAllView(true);
                _HistoryUp_All(m_eWy);
            }
            else
            {
                DateTime t1 = dtpHistoryL_From.Value;
                DateTime t2 = dtpHistoryL_To.Value;
                SetHistoryAllView(false);
                _HistoryUp(m_eWy, t1, t2);
            }
			//220106 pjh : Dresser History View
            if (CDataOption.UseSeperateDresser)
            {
                if (m_bDrsHistoryAllView)
                {
                    SetDrsHistoryAllView(true);
                    _DrsHistoryUp_All(m_eWy);
                }
                else
                {
                    DateTime t1 = dtpDrsHistoryL_From.Value;
                    DateTime t2 = dtpDrsHistoryL_To.Value;
                    SetDrsHistoryAllView(false);
                    _DrsHistoryUp(m_eWy, t1, t2);
                }
            }
            // 2021.05.18 lhs End
        }

        private void _Get()
        {
            if (m_eWy == EWay.L)
            {
                // Wheel information
                m_tWhl.sWhlName = txtL_Name.Text;
                double.TryParse(txtL_WhOuter .Text, out m_tWhl.dWhlO   );
                double.TryParse(txtL_WhTip   .Text, out m_tWhl.dWhltH  );
                int   .TryParse(txtL_WhToCnt .Text, out m_tWhl.iGtc    );
                int   .TryParse(txtL_WhDrCnt .Text, out m_tWhl.iGdc    );
                double.TryParse(txtL_WhToLoss.Text, out m_tWhl.dWhltL  );
                double.TryParse(txtL_WhStLoss.Text, out m_tWhl.dWhloL  );
                double.TryParse(txtL_WhDrLoss.Text, out m_tWhl.dWhldoL );
                double.TryParse(txtL_WhCyLoss.Text, out m_tWhl.dWhldcL );
                double.TryParse(txtL_Air     .Text, out m_tWhl.dDair   );
                // 2020.11.23 JSKim St
                double.TryParse(txtL_Air_Rep .Text, out m_tWhl.dDairRep);
                // 2020.11.23 JSKim Ed

                if (Convert.ToDouble(txtL_WhlLimit.Text) < 1.0)
                { txtL_WhlLimit.Text = "1"; }
                if(Convert.ToDouble(txtL_WhlLimit.Text) > GV.WHEEL_LIMIT_MAX)
                { txtL_WhlLimit.Text = GV.WHEEL_LIMIT_MAX.ToString(); }
                double.TryParse(txtL_WhlLimit.Text, out m_tWhl.dWhlLimit);
                // 200727 jym : 휠 히스토리 내용추가
                if (CData.CurCompany == ECompany.ASE_KR && GV.WHEEL_HISTORY)
                {
                    m_tWhl.sPartNo = txtL_Part.Text;
                    m_tWhl.sMesh = txtL_Mesh.Text;
                }

                // Dresser information
                //211122 pjh : 
                if (CDataOption.UseSeperateDresser)
                {
                    CData.Drs[0].sDrsName = lbxL_List_Drs.SelectedItem.ToString();

                    if (txtL_DrOuter.Text == "0")
                    {
                        CData.Drs[0].dDrsOuter = m_tWhl.dDrsOuter;
                    }

                    double.TryParse(txtL_DrOuter.Text, out CData.Drs[0].dDrsOuter);
                    double.TryParse(txtL_DrHei.Text, out CData.Drs[0].dDrsH);
                    m_tWhl.dDrsAf = CData.Drs[0].dDrsH;
                }
                else
                {
                    m_tWhl.sDrsName = txtL_DrName.Text;
                    double.TryParse(txtL_DrOuter.Text, out m_tWhl.dDrsOuter);
                    double.TryParse(txtL_DrHei.Text, out m_tWhl.dDrsH);
                }
                //
                if (Convert.ToDouble(txtL_DrsLimit.Text) < 1.0)
                { txtL_DrsLimit.Text = "1"; }
                if (Convert.ToDouble(txtL_DrsLimit.Text) > GV.DRESSER_LIMIT_MAX)
                { txtL_DrsLimit.Text = GV.DRESSER_LIMIT_MAX.ToString(); }
                double.TryParse(txtL_DrsLimit.Text, out m_tWhl.dDrsLimit);

                // Using 01
                m_tWhl.aUsedP[0].eMode = (eStepMode)cmbL_U1Mod.SelectedIndex;
                double.TryParse(txtL_U1To.Text, out m_tWhl.aUsedP[0].dTotalDep);
                double.TryParse(txtL_U1Cy.Text, out m_tWhl.aUsedP[0].dCycleDep);
                double.TryParse(txtL_U1Tb.Text, out m_tWhl.aUsedP[0].dTblSpd  );
                int   .TryParse(txtL_U1Sp.Text, out m_tWhl.aUsedP[0].iSplSpd  );
                m_tWhl.aUsedP[0].eDir = (eStartDir)cmbL_U1Dir.SelectedIndex;

                // Using 02
                m_tWhl.aUsedP[1].eMode = (eStepMode)cmbL_U2Mod.SelectedIndex;
                double.TryParse(txtL_U2To.Text, out m_tWhl.aUsedP[1].dTotalDep);
                double.TryParse(txtL_U2Cy.Text, out m_tWhl.aUsedP[1].dCycleDep);
                double.TryParse(txtL_U2Tb.Text, out m_tWhl.aUsedP[1].dTblSpd  );
                int   .TryParse(txtL_U2Sp.Text, out m_tWhl.aUsedP[1].iSplSpd  );
                m_tWhl.aUsedP[1].eDir = (eStartDir)cmbL_U2Dir.SelectedIndex;

                // Replace 01
                m_tWhl.aNewP[0].eMode = (eStepMode)cmbL_R1Mod.SelectedIndex;
                double.TryParse(txtL_R1To.Text, out m_tWhl.aNewP[0].dTotalDep);
                double.TryParse(txtL_R1Cy.Text, out m_tWhl.aNewP[0].dCycleDep);
                double.TryParse(txtL_R1Tb.Text, out m_tWhl.aNewP[0].dTblSpd  );
                int   .TryParse(txtL_R1Sp.Text, out m_tWhl.aNewP[0].iSplSpd  );
                m_tWhl.aNewP[0].eDir = (eStartDir)cmbL_R1Dir.SelectedIndex;

                // Replace 02
                m_tWhl.aNewP[1].eMode = (eStepMode)cmbL_R2Mod.SelectedIndex;
                double.TryParse(txtL_R2To.Text, out m_tWhl.aNewP[1].dTotalDep);
                double.TryParse(txtL_R2Cy.Text, out m_tWhl.aNewP[1].dCycleDep);
                double.TryParse(txtL_R2Tb.Text, out m_tWhl.aNewP[1].dTblSpd  );
                int.TryParse(txtL_R2Sp.Text   , out m_tWhl.aNewP[1].iSplSpd  );
                m_tWhl.aNewP[1].eDir = (eStartDir)cmbL_R2Dir.SelectedIndex;
            }
            else
            {
                // Wheel information
                m_tWhl.sWhlName = txtR_Name.Text;
                double.TryParse(txtR_WhOuter .Text, out m_tWhl.dWhlO   );
                double.TryParse(txtR_WhTip   .Text, out m_tWhl.dWhltH  );
                int   .TryParse(txtR_WhToCnt .Text, out m_tWhl.iGtc    );
                int   .TryParse(txtR_WhDrCnt .Text, out m_tWhl.iGdc    );
                double.TryParse(txtR_WhToLoss.Text, out m_tWhl.dWhltL  );
                double.TryParse(txtR_WhStLoss.Text, out m_tWhl.dWhloL  );
                double.TryParse(txtR_WhDrLoss.Text, out m_tWhl.dWhldoL );
                double.TryParse(txtR_WhCyLoss.Text, out m_tWhl.dWhldcL );
                double.TryParse(txtR_Air     .Text, out m_tWhl.dDair   );
                // 2020.11.23 JSKim St
                double.TryParse(txtR_Air_Rep .Text, out m_tWhl.dDairRep);
                // 2020.11.23 JSKim Ed

                if (Convert.ToDouble(txtR_WhlLimit.Text) < 1.0)
                { txtR_WhlLimit.Text = "1"; }
                if (Convert.ToDouble(txtR_WhlLimit.Text) > GV.WHEEL_LIMIT_MAX)
                { txtR_WhlLimit.Text = GV.WHEEL_LIMIT_MAX.ToString(); }
                double.TryParse(txtR_WhlLimit.Text, out m_tWhl.dWhlLimit);
                // 200727 jym : 휠 히스토리 내용추가
                if (CData.CurCompany == ECompany.ASE_KR && GV.WHEEL_HISTORY)
                {
                    m_tWhl.sPartNo = txtR_Part.Text;
                    m_tWhl.sMesh = txtR_Mesh.Text;
                }

                // Dresser information
                //211122 pjh :
                if (CDataOption.UseSeperateDresser)
                {
                    CData.Drs[1].sDrsName = lbxR_List_Drs.SelectedItem.ToString();

                    if (txtR_DrOuter.Text == "0")
                    {
                        CData.Drs[1].dDrsOuter = m_tWhl.dDrsOuter;
                    }

                    double.TryParse(txtR_DrOuter.Text, out CData.Drs[1].dDrsOuter);
                    double.TryParse(txtR_DrHei.Text, out CData.Drs[1].dDrsH);
                    m_tWhl.dDrsAf = CData.Drs[1].dDrsH;
                }
                else
                {
                    m_tWhl.sDrsName = txtR_DrName.Text;
                    double.TryParse(txtR_DrOuter.Text, out m_tWhl.dDrsOuter);
                    double.TryParse(txtR_DrHei.Text, out m_tWhl.dDrsH);
                }
                //
                if (Convert.ToDouble(txtR_DrsLimit.Text) < 1.0)
                { txtR_DrsLimit.Text = "1"; }
                if (Convert.ToDouble(txtR_DrsLimit.Text) > GV.DRESSER_LIMIT_MAX)
                { txtR_DrsLimit.Text = GV.DRESSER_LIMIT_MAX.ToString(); }
                double.TryParse(txtR_DrsLimit.Text, out m_tWhl.dDrsLimit);

                // Using 01
                m_tWhl.aUsedP[0].eMode = (eStepMode)cmbR_U1Mod.SelectedIndex;
                double.TryParse(txtR_U1To.Text, out m_tWhl.aUsedP[0].dTotalDep);
                double.TryParse(txtR_U1Cy.Text, out m_tWhl.aUsedP[0].dCycleDep);
                double.TryParse(txtR_U1Tb.Text, out m_tWhl.aUsedP[0].dTblSpd  );
                int   .TryParse(txtR_U1Sp.Text, out m_tWhl.aUsedP[0].iSplSpd  );
                m_tWhl.aUsedP[0].eDir = (eStartDir)cmbR_U1Dir.SelectedIndex;

                // Using 02
                m_tWhl.aUsedP[1].eMode = (eStepMode)cmbR_U2Mod.SelectedIndex;
                double.TryParse(txtR_U2To.Text, out m_tWhl.aUsedP[1].dTotalDep);
                double.TryParse(txtR_U2Cy.Text, out m_tWhl.aUsedP[1].dCycleDep);
                double.TryParse(txtR_U2Tb.Text, out m_tWhl.aUsedP[1].dTblSpd  );
                int   .TryParse(txtR_U2Sp.Text, out m_tWhl.aUsedP[1].iSplSpd  );
                m_tWhl.aUsedP[1].eDir = (eStartDir)cmbR_U2Dir.SelectedIndex;

                // Replace 01
                m_tWhl.aNewP[0].eMode = (eStepMode)cmbR_R1Mod.SelectedIndex;
                double.TryParse(txtR_R1To.Text, out m_tWhl.aNewP[0].dTotalDep);
                double.TryParse(txtR_R1Cy.Text, out m_tWhl.aNewP[0].dCycleDep);
                double.TryParse(txtR_R1Tb.Text, out m_tWhl.aNewP[0].dTblSpd  );
                int   .TryParse(txtR_R1Sp.Text, out m_tWhl.aNewP[0].iSplSpd  );
                m_tWhl.aNewP[0].eDir = (eStartDir)cmbR_R1Dir.SelectedIndex;

                // Replace 02
                m_tWhl.aNewP[1].eMode = (eStepMode)cmbR_R2Mod.SelectedIndex;
                double.TryParse(txtR_R2To.Text, out m_tWhl.aNewP[1].dTotalDep);
                double.TryParse(txtR_R2Cy.Text, out m_tWhl.aNewP[1].dCycleDep);
                double.TryParse(txtR_R2Tb.Text, out m_tWhl.aNewP[1].dTblSpd  );
                int   .TryParse(txtR_R2Sp.Text, out m_tWhl.aNewP[1].iSplSpd  );
                m_tWhl.aNewP[1].eDir = (eStartDir)cmbR_R2Dir.SelectedIndex;
            }
        }

        private void _ListUp()
        {
            string sPath = GV.PATH_WHEEL;
            //211119 pjh : Dresser 정보 별도로 관리
            if (CDataOption.UseSeperateDresser)
            {
                if (m_eWy == EWay.L)
                {
                    lbxL_List_Whl.Items.Clear();
                    sPath += "Left\\";

                    if (Directory.Exists(sPath))
                    {
                        DirectoryInfo mFile = new DirectoryInfo(sPath);
                        //foreach (FileSystemInfo mInfo in mFile.GetFileSystemInfos("*.whl"))
                        foreach (DirectoryInfo mInfo in mFile.GetDirectories())
                        {
                            //lbxL_List.Items.Add(mInfo.Name.Replace(".whl", ""));
                            lbxL_List_Whl.Items.Add(mInfo.Name);
                        }
                        lbxL_List_Whl.SelectedItem = CData.Whls[0].sWhlName;
                    }
                    else
                    { Directory.CreateDirectory(sPath); }
                }
                else
                {
                    lbxR_List_Whl.Items.Clear();
                    sPath = GV.PATH_WHEEL + "Right\\";

                    if (Directory.Exists(sPath))
                    {
                        DirectoryInfo mFile = new DirectoryInfo(sPath);
                        //foreach (FileSystemInfo mInfo in mFile.GetFileSystemInfos("*.whl"))
                        foreach (DirectoryInfo mInfo in mFile.GetDirectories())
                        {
                            //lbxR_List.Items.Add(mInfo.Name.Replace(".whl", ""));
                            lbxR_List_Whl.Items.Add(mInfo.Name);
                        }
                        lbxR_List_Whl.SelectedItem = CData.Whls[1].sWhlName;
                    }
                    else
                    { Directory.CreateDirectory(sPath); }
                }
            }
            else
            {
                if (m_eWy == EWay.L)
                {
                    lbxL_List.Items.Clear();
                    sPath += "Left\\";

                    if (Directory.Exists(sPath))
                    {
                        DirectoryInfo mFile = new DirectoryInfo(sPath);
                        //foreach (FileSystemInfo mInfo in mFile.GetFileSystemInfos("*.whl"))
                        foreach (DirectoryInfo mInfo in mFile.GetDirectories())
                        {
                            //lbxL_List.Items.Add(mInfo.Name.Replace(".whl", ""));
                            lbxL_List.Items.Add(mInfo.Name);
                        }
                        lbxL_List.SelectedItem = CData.Whls[0].sWhlName;
                    }
                    else
                    { Directory.CreateDirectory(sPath); }
                }
                else
                {
                    lbxR_List.Items.Clear();
                    sPath = GV.PATH_WHEEL + "Right\\";

                    if (Directory.Exists(sPath))
                    {
                        DirectoryInfo mFile = new DirectoryInfo(sPath);
                        //foreach (FileSystemInfo mInfo in mFile.GetFileSystemInfos("*.whl"))
                        foreach (DirectoryInfo mInfo in mFile.GetDirectories())
                        {
                            //lbxR_List.Items.Add(mInfo.Name.Replace(".whl", ""));
                            lbxR_List.Items.Add(mInfo.Name);
                        }
                        lbxR_List.SelectedItem = CData.Whls[1].sWhlName;
                    }
                    else
                    { Directory.CreateDirectory(sPath); }
                }
            }
        }

        //211122 pjh :
        private void _ListUpDrs()
        {
            string sPath = GV.PATH_DRESSER;
            
            if (m_eWy == EWay.L)
            {
                lbxL_List_Drs.Items.Clear();
                sPath += "Left\\";

                if (Directory.Exists(sPath))
                {
                    DirectoryInfo mFile = new DirectoryInfo(sPath);
                    //foreach (FileSystemInfo mInfo in mFile.GetFileSystemInfos("*.whl"))
                    foreach (DirectoryInfo mInfo in mFile.GetDirectories())
                    {
                        lbxL_List_Drs.Items.Add(mInfo.Name);
                    }
                    lbxL_List_Drs.SelectedItem = CData.Drs[0].sDrsName;
                }
                else
                { Directory.CreateDirectory(sPath); }
            }
            else
            {
                lbxR_List_Drs.Items.Clear();
                sPath = GV.PATH_DRESSER + "Right\\";

                if (Directory.Exists(sPath))
                {
                    DirectoryInfo mFile = new DirectoryInfo(sPath);
                    //foreach (FileSystemInfo mInfo in mFile.GetFileSystemInfos("*.whl"))
                    foreach (DirectoryInfo mInfo in mFile.GetDirectories())
                    {
                        //lbxR_List.Items.Add(mInfo.Name.Replace(".whl", ""));
                        lbxR_List_Drs.Items.Add(mInfo.Name);
                    }
                    lbxR_List_Drs.SelectedItem = CData.Drs[1].sDrsName;
                }
                else
                { Directory.CreateDirectory(sPath); }
            }
        }

        // 2021.04.06 SungTae Start : Wheel History 파일의 무한정 크기 증가로 인한 랙 발생으로 Daily 별로 Load하도록 변경
        private void _HistoryUp(EWay eWay)
        {
            string sPath = GV.PATH_WHEEL;
            
            int iRowCnt = 0;
            string[] sData;
            string[] sHeader;
            string sRowValue = "";

            if (eWay == EWay.L) sPath += "Left\\";
            else                sPath += "Right\\";

            sPath += m_tWhl.sWhlName + "\\WheelHistory\\";

            if (eWay == EWay.L) sPath += dtpHistoryL_From.Text + ".csv";
            else                sPath += dtpHistoryR_From.Text + ".csv";

            FileInfo fI = new FileInfo(sPath);

            if (eWay == EWay.L) dgvL_HIst.Rows.Clear();
            else                dgvR_HIst.Rows.Clear();

            if (fI.Exists)
            {
                try
                {
                    string filename = Path.GetFileName(sPath);

                    if (CLog.killps(filename) == true) m_Kill.Wait(2000);

                    StreamReader sr = new StreamReader(sPath, Encoding.GetEncoding("euc-kr"));
                    sRowValue = sr.ReadLine();
                    
                    if (sRowValue == null)
                    {
                        string temp = sPath + " is Empty";
                        CMsg.Show(eMsg.Error, "Error", temp);
                        return;
                    }

                    sHeader = sRowValue.Split(',');

                    while (sr.Peek() > -1)
                    {
                        sRowValue = sr.ReadLine();
                        
                        if (sRowValue == null)
                        {
                            string temp = sPath + " is Empty";
                            CMsg.Show(eMsg.Error, "Error", temp);
                            return;
                        }

                        sData = sRowValue.Split(',');

                        if (eWay == EWay.L) dgvL_HIst.Rows.Add();
                        else                dgvR_HIst.Rows.Add();

                        int iCnt = sData.Length - 1;

                        if (CData.CurCompany == ECompany.ASE_KR && GV.WHEEL_HISTORY)
                        {
                            if (eWay == EWay.L)
                            {
                                dgvL_HIst[0, iRowCnt].Value = sData[0]; // Date
                                dgvL_HIst[1, iRowCnt].Value = (iCnt > 20) ? sData[20] : ""; // Table
                                dgvL_HIst[2, iRowCnt].Value = (iCnt > 20) ? sData[21] : ""; // Wheel ID
                                dgvL_HIst[3, iRowCnt].Value = (iCnt > 20) ? sData[22] : ""; // Wheel Part
                                dgvL_HIst[4, iRowCnt].Value = (iCnt > 20) ? sData[23] : "0"; // Mesh
                                dgvL_HIst[5, iRowCnt].Value = (iCnt > 20) ? sData[24] : "0"; // Start Wheel Tip
                                dgvL_HIst[6, iRowCnt].Value = (iCnt > 20) ? sData[25] : "0"; // Wheel Loss
                                dgvL_HIst[7, iRowCnt].Value = (iCnt > 20) ? sData[26] : "0"; // End Wheel Tip
                                dgvL_HIst[8, iRowCnt].Value = sData[4]; // Dresser name
                                dgvL_HIst[9, iRowCnt].Value = (iCnt > 20) ? sData[27] : "0"; // Start Dresser Tip
                                dgvL_HIst[10, iRowCnt].Value = (iCnt > 20) ? sData[28] : "0"; // Dresser Loss
                                dgvL_HIst[11, iRowCnt].Value = (iCnt > 20) ? sData[29] : "0"; // End Dresser Tip
                                dgvL_HIst[12, iRowCnt].Value = sData[7]; // Grinding count
                                dgvL_HIst[13, iRowCnt].Value = sData[8]; // Grinding count
                                dgvL_HIst[14, iRowCnt].Value = sData[9]; // Grinding count
                                dgvL_HIst[15, iRowCnt].Value = sData[10]; // Grinding count
                            }
                            else
                            {
                                dgvR_HIst[0, iRowCnt].Value = sData[0]; // Date
                                dgvR_HIst[1, iRowCnt].Value = (iCnt > 20) ? sData[20] : ""; // Table
                                dgvR_HIst[2, iRowCnt].Value = (iCnt > 20) ? sData[21] : ""; // Wheel ID
                                dgvR_HIst[3, iRowCnt].Value = (iCnt > 20) ? sData[22] : ""; // Wheel Part
                                dgvR_HIst[4, iRowCnt].Value = (iCnt > 20) ? sData[23] : "0"; // Mesh
                                dgvR_HIst[5, iRowCnt].Value = (iCnt > 20) ? sData[24] : "0"; // Start Wheel Tip
                                dgvR_HIst[6, iRowCnt].Value = (iCnt > 20) ? sData[25] : "0"; // Wheel Loss
                                dgvR_HIst[7, iRowCnt].Value = (iCnt > 20) ? sData[26] : "0"; // End Wheel Tip
                                dgvR_HIst[8, iRowCnt].Value = sData[4]; // Dresser name
                                dgvR_HIst[9, iRowCnt].Value = (iCnt > 20) ? sData[27] : "0"; // Start Dresser Tip
                                dgvR_HIst[10, iRowCnt].Value = (iCnt > 20) ? sData[28] : "0"; // Dresser Loss
                                dgvR_HIst[11, iRowCnt].Value = (iCnt > 20) ? sData[29] : "0"; // End Dresser Tip
                                dgvR_HIst[12, iRowCnt].Value = sData[7]; // Grinding count
                                dgvR_HIst[13, iRowCnt].Value = sData[8]; // Grinding count
                                dgvR_HIst[14, iRowCnt].Value = sData[9]; // Grinding count
                                dgvR_HIst[15, iRowCnt].Value = sData[10]; // Grinding count
                            }
                        }
                        else
                        {
                            iCnt = 20;
                            for (int i = 0; i < iCnt; i++)
                            {
                                if (eWay == EWay.L) dgvL_HIst[i, iRowCnt].Value = sData[i];
                                else                dgvR_HIst[i, iRowCnt].Value = sData[i];
                            }
                        }

                        iRowCnt++;
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
        // 2021.04.06 SungTae End

        // 2021.05.18 lhs Start
        // Wheel History 를 설정한 기간 만큼 표시
        private void _HistoryUp_All(EWay eWay)
        {
            string sPath = GV.PATH_WHEEL;
            
            //string[] sHeader;
            int     nRowIdx     = 0;

            if (eWay == EWay.L) sPath += "Left\\";
            else                sPath += "Right\\";
            sPath += m_tWhl.sWhlName + "\\";

            string sOnePath = sPath;                        // 하나의 파일에 저장한 파일의 경로 (WheelHistory.csv)
            string sDayPath = sPath + "WheelHistory\\";     // 날짜별로 저장한 파일의 경로
            string sFileName;

            // 그리드뷰 클리어
            if (eWay == EWay.L) dgvL_HIst.Rows.Clear();
            else                dgvR_HIst.Rows.Clear();

            // 전체 표시일 경우는 OneFile, DayFile 모두 읽어 표시
            //-----------------------------
            // OneFile이 있으면 표시
            //-----------------------------
            DirectoryInfo di = new DirectoryInfo(sOnePath);
            if (di.Exists) 
            {
                sFileName = sOnePath + "WheelHistory.csv";
                FileInfo fI = new FileInfo(sFileName);
                if (fI.Exists)
                {
                    // 파일사이즈가 크면 
                    if (fI.Length > 500000) 
                    {
                        string sNote = string.Format("The file size is too large and it takes a long time to display. Should I continue anyway?");
                        if (CMsg.Show(eMsg.Warning, "Warning", sNote) == DialogResult.Cancel)
                        {
                            return;
                        }
                    }
                    // History 표시
                    nRowIdx = DisplayGridView(eWay, sFileName, nRowIdx); // 연속적인 Row Index를 만들려고...
                }
            }

            //-----------------------------
            // DayFile 있으면 모두 표시
            //-----------------------------
            // 디렉토리 내의 파일 정보 얻기
            di = new DirectoryInfo(sDayPath);
            if (!di.Exists) { return; }

            FileInfo[] arrFi = di.GetFiles("*.csv");
            int nFileCnt = arrFi.Length;     // 파일 갯수
            if (nFileCnt == 0) { return; }   // 날짜별 파일이 없으면

            for (int i = 0; i < nFileCnt; i++)
            {
                sFileName = arrFi[i].FullName;
                nRowIdx = DisplayGridView(eWay, sFileName, nRowIdx); // 연속적인 Row Index를 만들려고...
            }
            //-----------------------------
        }
        // 2021.05.18 lhs End

        // 2021.05.18 lhs Start
        // Wheel History 를 설정한 기간 만큼 표시
        private void _HistoryUp(EWay eWay, DateTime t1, DateTime t2)
        {
            string sPath = GV.PATH_WHEEL;

            if (eWay == EWay.L) sPath += "Left\\";
            else                sPath += "Right\\";
            sPath += m_tWhl.sWhlName + "\\WheelHistory\\";

            // 그리드뷰 클리어
            if (eWay == EWay.L) dgvL_HIst.Rows.Clear();
            else                dgvR_HIst.Rows.Clear();

            //---------------------
            // 디렉토리 내의 파일 정보 얻기
            DirectoryInfo di = new DirectoryInfo(sPath);
            if(!di.Exists)  {   return; }
            
            FileInfo[] arrFi = di.GetFiles("*.csv");
            int nFileCnt = arrFi.Length;            // 파일 갯수
            if(nFileCnt == 0)   {   return;     }   // 날짜별 파일이 없으면
            //---------------------

            //---------------------
            // From ~ To 파일 읽기
            //---------------------
            TimeSpan DayCnt = t2 - t1;
            int nDayCnt = int.Parse(DayCnt.Days.ToString()) + 1;
            int nRowIdx = 0;

            for (int nD = 0; nD < nDayCnt; nD++)
            {
                double dAdd = double.Parse(nD.ToString());
                string sFile = t1.AddDays(dAdd).ToString("yyyy-MM-dd");
                sFile += ".csv";
                string sFileName = sPath + sFile;
                
                FileInfo fI = new FileInfo(sFileName);
                if (fI.Exists)
                {
                    nRowIdx = DisplayGridView(eWay, sFileName, nRowIdx); // 연속적인 Row Index를 만들려고...
                }
            }
        }
        // 2021.05.18 lhs End

		//220106 pjh : Dresser History Display 함수
        private void _DrsHistoryUp(EWay eWay, DateTime t1, DateTime t2)
        {
            string sPath = GV.PATH_DRESSER;

            if (eWay == EWay.L) sPath += "Left\\";
            else sPath += "Right\\";
            sPath += m_tDrs.sDrsName + "\\DresserHistory\\";

            // 그리드뷰 클리어
            if (eWay == EWay.L) dgvL_DrsHIst.Rows.Clear();
            else dgvR_DrsHIst.Rows.Clear();

            //---------------------
            // 디렉토리 내의 파일 정보 얻기
            DirectoryInfo di = new DirectoryInfo(sPath);
            if (!di.Exists) { return; }

            FileInfo[] arrFi = di.GetFiles("*.csv");
            int nFileCnt = arrFi.Length;            // 파일 갯수
            if (nFileCnt == 0) { return; }   // 날짜별 파일이 없으면
            //---------------------

            //---------------------
            // From ~ To 파일 읽기
            //---------------------
            TimeSpan DayCnt = t2 - t1;
            int nDayCnt = int.Parse(DayCnt.Days.ToString()) + 1;
            int nRowIdx = 0;

            for (int nD = 0; nD < nDayCnt; nD++)
            {
                double dAdd = double.Parse(nD.ToString());
                string sFile = t1.AddDays(dAdd).ToString("yyyy-MM-dd");
                sFile += ".csv";
                string sFileName = sPath + sFile;

                FileInfo fI = new FileInfo(sFileName);
                if (fI.Exists)
                {
                    nRowIdx = DisplayDrsGridView(eWay, sFileName, nRowIdx); // 연속적인 Row Index를 만들려고...
                }
            }
        }
        private void _DrsHistoryUp_All(EWay eWay)
        {
            string sPath = GV.PATH_DRESSER;

            //string[] sHeader;
            int nRowIdx = 0;

            if (eWay == EWay.L) sPath += "Left\\";
            else sPath += "Right\\";
            sPath += m_tDrs.sDrsName + "\\";

            string sOnePath = sPath;                        // 하나의 파일에 저장한 파일의 경로 (WheelHistory.csv)
            string sDayPath = sPath + "DresserHistory\\";     // 날짜별로 저장한 파일의 경로
            string sFileName;

            // 그리드뷰 클리어
            if (eWay == EWay.L) dgvL_DrsHIst.Rows.Clear();
            else dgvR_DrsHIst.Rows.Clear();

            // 전체 표시일 경우는 OneFile, DayFile 모두 읽어 표시
            //-----------------------------
            // OneFile이 있으면 표시
            //-----------------------------
            DirectoryInfo di = new DirectoryInfo(sOnePath);
            if (di.Exists)
            {
                sFileName = sOnePath + "DresserHistory.csv";
                FileInfo fI = new FileInfo(sFileName);
                if (fI.Exists)
                {
                    // 파일사이즈가 크면 
                    if (fI.Length > 500000)
                    {
                        string sNote = string.Format("The file size is too large and it takes a long time to display. Should I continue anyway?");
                        if (CMsg.Show(eMsg.Warning, "Warning", sNote) == DialogResult.Cancel)
                        {
                            return;
                        }
                    }
                    // History 표시
                    nRowIdx = DisplayDrsGridView(eWay, sFileName, nRowIdx); // 연속적인 Row Index를 만들려고...
                }
            }

            //-----------------------------
            // DayFile 있으면 모두 표시
            //-----------------------------
            // 디렉토리 내의 파일 정보 얻기
            di = new DirectoryInfo(sDayPath);
            if (!di.Exists) { return; }

            FileInfo[] arrFi = di.GetFiles("*.csv");
            int nFileCnt = arrFi.Length;     // 파일 갯수
            if (nFileCnt == 0) { return; }   // 날짜별 파일이 없으면

            for (int i = 0; i < nFileCnt; i++)
            {
                sFileName = arrFi[i].FullName;
                nRowIdx = DisplayDrsGridView(eWay, sFileName, nRowIdx); // 연속적인 Row Index를 만들려고...
            }
            //-----------------------------
        }

        private int DisplayGridView(EWay eWay, string sPath, int iRowCnt)
        {
            string[] sData;
            string[] sHeader;
            try
            {
                string filename = Path.GetFileName(sPath);
                if (CLog.killps(filename) == true) m_Kill.Wait(2000);

                StreamReader sr = new StreamReader(sPath, Encoding.GetEncoding("euc-kr"));

                //------------------
                // 헤더 라인 읽기
                string sRowValue = sr.ReadLine();
                if (sRowValue == null)
                {
                    string temp = sPath + " is Empty";
                    CMsg.Show(eMsg.Error, "Error", temp);
                    return iRowCnt;
                }
                sHeader = sRowValue.Split(',');
                //------------------

                while (sr.Peek() > -1)
                {
                    sRowValue = sr.ReadLine();
                    if (sRowValue == null)
                    {
                        string temp = sPath + " is Empty";
                        CMsg.Show(eMsg.Error, "Error", temp);
                        return iRowCnt;
                    }
                    sData = sRowValue.Split(',');
                    int iCnt = sData.Length - 1;

                    if (eWay == EWay.L) { dgvL_HIst.Rows.Add(); }
                    else                { dgvR_HIst.Rows.Add(); }

                    if (CData.CurCompany == ECompany.ASE_KR && GV.WHEEL_HISTORY)
                    {
                        if (eWay == EWay.L)
                        {
                            dgvL_HIst[0, iRowCnt].Value = sData[0]; // Date
                            dgvL_HIst[1, iRowCnt].Value = (iCnt > 20) ? sData[20] : ""; // Table
                            dgvL_HIst[2, iRowCnt].Value = (iCnt > 20) ? sData[21] : ""; // Wheel ID
                            dgvL_HIst[3, iRowCnt].Value = (iCnt > 20) ? sData[22] : ""; // Wheel Part
                            dgvL_HIst[4, iRowCnt].Value = (iCnt > 20) ? sData[23] : "0"; // Mesh
                            dgvL_HIst[5, iRowCnt].Value = (iCnt > 20) ? sData[24] : "0"; // Start Wheel Tip
                            dgvL_HIst[6, iRowCnt].Value = (iCnt > 20) ? sData[25] : "0"; // Wheel Loss
                            dgvL_HIst[7, iRowCnt].Value = (iCnt > 20) ? sData[26] : "0"; // End Wheel Tip
                            dgvL_HIst[8, iRowCnt].Value = sData[4]; // Dresser name
                            dgvL_HIst[9, iRowCnt].Value = (iCnt > 20) ? sData[27] : "0"; // Start Dresser Tip
                            dgvL_HIst[10, iRowCnt].Value = (iCnt > 20) ? sData[28] : "0"; // Dresser Loss
                            dgvL_HIst[11, iRowCnt].Value = (iCnt > 20) ? sData[29] : "0"; // End Dresser Tip
                            dgvL_HIst[12, iRowCnt].Value = sData[7]; // Grinding count
                            dgvL_HIst[13, iRowCnt].Value = sData[8]; // Grinding count
                            dgvL_HIst[14, iRowCnt].Value = sData[9]; // Grinding count
                            dgvL_HIst[15, iRowCnt].Value = sData[10]; // Grinding count
                        }
                        else
                        {
                            dgvR_HIst[0, iRowCnt].Value = sData[0]; // Date
                            dgvR_HIst[1, iRowCnt].Value = (iCnt > 20) ? sData[20] : ""; // Table
                            dgvR_HIst[2, iRowCnt].Value = (iCnt > 20) ? sData[21] : ""; // Wheel ID
                            dgvR_HIst[3, iRowCnt].Value = (iCnt > 20) ? sData[22] : ""; // Wheel Part
                            dgvR_HIst[4, iRowCnt].Value = (iCnt > 20) ? sData[23] : "0"; // Mesh
                            dgvR_HIst[5, iRowCnt].Value = (iCnt > 20) ? sData[24] : "0"; // Start Wheel Tip
                            dgvR_HIst[6, iRowCnt].Value = (iCnt > 20) ? sData[25] : "0"; // Wheel Loss
                            dgvR_HIst[7, iRowCnt].Value = (iCnt > 20) ? sData[26] : "0"; // End Wheel Tip
                            dgvR_HIst[8, iRowCnt].Value = sData[4]; // Dresser name
                            dgvR_HIst[9, iRowCnt].Value = (iCnt > 20) ? sData[27] : "0"; // Start Dresser Tip
                            dgvR_HIst[10, iRowCnt].Value = (iCnt > 20) ? sData[28] : "0"; // Dresser Loss
                            dgvR_HIst[11, iRowCnt].Value = (iCnt > 20) ? sData[29] : "0"; // End Dresser Tip
                            dgvR_HIst[12, iRowCnt].Value = sData[7]; // Grinding count
                            dgvR_HIst[13, iRowCnt].Value = sData[8]; // Grinding count
                            dgvR_HIst[14, iRowCnt].Value = sData[9]; // Grinding count
                            dgvR_HIst[15, iRowCnt].Value = sData[10]; // Grinding count
                        }
                    }
                    else
                    {
                        iCnt = 20;
                        for (int i = 0; i < iCnt; i++)
                        {
                            if (eWay == EWay.L) {   dgvL_HIst[i, iRowCnt].Value = sData[i]; }
                            else                {   dgvR_HIst[i, iRowCnt].Value = sData[i]; }
                        }
                    }
                    
                    iRowCnt++;
                }
                sr.Close();
                
                return iRowCnt; // Row Index
            }
            catch (Exception ex)
            {
                if (CData.CurCompany == ECompany.Qorvo) CSQ_Main.It.isSPC = true;//220526 pjh : Qorvo는 Data Save Error 발생 시 Buzzer On
                CMsg.Show(eMsg.Error, "Error", ex.Message);
                return iRowCnt;
            }
        }

		//220106 pjh : Dresser History 내용 UI 표시 함수
        private int DisplayDrsGridView(EWay eWay, string sPath, int iRowCnt)
        {
            string[] sData;
            string[] sHeader;
            try
            {
                string filename = Path.GetFileName(sPath);
                if (CLog.killps(filename) == true) m_Kill.Wait(2000);

                StreamReader sr = new StreamReader(sPath, Encoding.GetEncoding("euc-kr"));

                //------------------
                // 헤더 라인 읽기
                string sRowValue = sr.ReadLine();
                if (sRowValue == null)
                {
                    string temp = sPath + " is Empty";
                    CMsg.Show(eMsg.Error, "Error", temp);
                    return iRowCnt;
                }
                sHeader = sRowValue.Split(',');
                //------------------

                while (sr.Peek() > -1)
                {
                    sRowValue = sr.ReadLine();
                    if (sRowValue == null)
                    {
                        string temp = sPath + " is Empty";
                        CMsg.Show(eMsg.Error, "Error", temp);
                        return iRowCnt;
                    }
                    sData = sRowValue.Split(',');
                    int iCnt = sData.Length - 1;

                    if (eWay == EWay.L) { dgvL_DrsHIst.Rows.Add(); }
                    else { dgvR_DrsHIst.Rows.Add(); }

                    iCnt = 5;
                    for (int i = 0; i < iCnt; i++)
                    {
                        if (eWay == EWay.L) { dgvL_DrsHIst[i, iRowCnt].Value = sData[i]; }
                        else { dgvR_DrsHIst[i, iRowCnt].Value = sData[i]; }
                    }

                    iRowCnt++;
                }
                sr.Close();

                return iRowCnt; // Row Index
            }
            catch (Exception ex)
            {
                if (CData.CurCompany == ECompany.Qorvo) CSQ_Main.It.isSPC = true;//220526 pjh : Qorvo는 Data Save Error 발생 시 Buzzer On
                CMsg.Show(eMsg.Error, "Error", ex.Message);
                return iRowCnt;
            }
        }

        /// <summary>
        /// 휠파일이 중복되는지 판단    true:중복  false:미중복
        /// </summary>
        /// <param name="sVal"></param>
        /// <returns></returns>
        private bool _Contain(string sVal)
        {
            if (m_eWy == EWay.L)
            {
                IEnumerable<string> aVal = lbxL_List.Items.OfType<string>();
                return aVal.Contains(sVal);
            }
            else
            {
                IEnumerable<string> aVal = lbxR_List.Items.OfType<string>();
                return aVal.Contains(sVal);
            }
        }

        #endregion

        private void lbx_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) { return; }

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                e = new DrawItemEventArgs(e.Graphics, e.Font, e.Bounds, e.Index, e.State ^ DrawItemState.Selected, e.ForeColor, Color.FromArgb(43, 173, 175));
            }

            ListBox mLbx = sender as ListBox;
            int iX = e.Bounds.X + (e.Font.Height/2);
            int iY = e.Bounds.Y + (e.Font.Height / 2);

            e.DrawBackground();
            e.Graphics.DrawString(mLbx.Items[e.Index].ToString(), e.Font, Brushes.Black, iX, iY, StringFormat.GenericDefault);
            e.DrawFocusRectangle();
        }

        private void tabWhl_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabPage mTp = sender as TabPage;

            int iIdx = 0;
            m_eWy = (EWay)int.Parse(tabWhl.SelectedTab.Tag.ToString());

            //211119 pjh : Dresser 정보 별도로 관리
            if (CDataOption.UseSeperateDresser)
            {
                if (m_eWy == EWay.L)
                {
                    // lbxL_List.SelectedIndex = -1;
                    //lbxL_List.SelectedIndex = m_iLeft;
                    if (m_iLeft >= 0) { lbxL_List_Whl.SetSelected(m_iLeft, true); }

                    iIdx = m_iLeft;
                }
                else
                {
                    //lbxR_List.SelectedIndex = -1;
                    if (m_iRight >= 0) { lbxR_List_Whl.SetSelected(m_iRight, true); }
                    //lbxR_List.SelectedIndex = m_iRight;
                    iIdx = m_iRight;
                }
            }
            else
            {
                if (m_eWy == EWay.L)
                {
                    // lbxL_List.SelectedIndex = -1;
                    //lbxL_List.SelectedIndex = m_iLeft;
                    if (m_iLeft >= 0) { lbxL_List.SetSelected(m_iLeft, true); }

                    iIdx = m_iLeft;
                }
                else
                {
                    //lbxR_List.SelectedIndex = -1;
                    if (m_iRight >= 0) { lbxR_List.SetSelected(m_iRight, true); }
                    //lbxR_List.SelectedIndex = m_iRight;
                    iIdx = m_iRight;
                }
            }

            _Set();
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            CData.bLastClick = true; //190509 ksg :
            Button mBtn = sender as Button;
            BeginInvoke(new Action(() => mBtn.Enabled = false));

            int iRet = 0;
            int iIdx = 0;
            //211122 pjh :
            int iRetDrs = 0;
            //

            if (m_eWy == EWay.L)
            { iIdx = m_iLeft; }
            else
            { iIdx = m_iRight; }

            //190711 ksg :
            if(CData.WhlChgSeq.bStart && CData.WhlChgSeq.iStep > 1  && CData.WhlChgSeq.bSltLeft && m_eWy != EWay.L)
            {
                CMsg.Show(eMsg.Warning, "Warning", "Select file wrong. Select left wheel file please");
                BeginInvoke(new Action(() => mBtn.Enabled = true));
                return;
            }
            if(CData.WhlChgSeq.bStart && CData.WhlChgSeq.iStep > 1 && CData.WhlChgSeq.bSltRight && m_eWy != EWay.R)
            {
                CMsg.Show(eMsg.Warning, "Warning", "Select file wrong. Select right wheel file please");
                BeginInvoke(new Action(() => mBtn.Enabled = true));
                return;
            }

            //211210 pjh : Wheel File 및 Dresser File이 선택되지 않으면 Message 표시
            if (CDataOption.UseSeperateDresser)
            {
                if (m_eWy == EWay.L)
                {
                    if (lbxL_List_Whl.SelectedIndex < 0)
                    {
                        CMsg.Show(eMsg.Error, "Error", "Left Wheel file is not selected. Please select wheel file");
                        BeginInvoke(new Action(() => mBtn.Enabled = true));
                        return;
                    }
                    else if (lbxL_List_Drs.SelectedIndex < 0)
                    {
                        CMsg.Show(eMsg.Error, "Error", "Left Dresser file is not selected. Please select dresser file");
                        BeginInvoke(new Action(() => mBtn.Enabled = true));
                        return;
                    }
                    else if (lbxL_List_Whl.SelectedIndex < 0 && lbxL_List_Drs.SelectedIndex < 0)
                    {
                        CMsg.Show(eMsg.Error, "Error", "LeftWheel&Dresser file is not selected. Please select both file");
                        BeginInvoke(new Action(() => mBtn.Enabled = true));
                        return;
                    }
                }
                else
                {
                    if (lbxR_List_Whl.SelectedIndex < 0)
                    {
                        CMsg.Show(eMsg.Error, "Error", "Right Wheel file is not selected. Please select wheel file");
                        BeginInvoke(new Action(() => mBtn.Enabled = true));
                        return;
                    }
                    else if (lbxR_List_Drs.SelectedIndex < 0)
                    {
                        CMsg.Show(eMsg.Error, "Error", "Right Dresser file is not selected. Please select dresser file");
                        BeginInvoke(new Action(() => mBtn.Enabled = true));
                        return;
                    }
                    else if (lbxR_List_Whl.SelectedIndex < 0 && lbxL_List_Drs.SelectedIndex < 0)
                    {
                        CMsg.Show(eMsg.Error, "Error", "RIght Wheel&Dresser file is not selected. Please select both file");
                        BeginInvoke(new Action(() => mBtn.Enabled = true));
                        return;
                    }
                }
            }
            //

            try
            {
                if (iIdx < 0)
                {
                    CErr.Show(eErr.SYSTEM_WHEEL_FILE_NOT_SELECTE);
                }
                else
                {
                    EWay eWy = (EWay)m_eWy;
                    string sNote = "";
                    string sName = "";

                    // 현재 휠 파일 이름 획득

                    //211119 pjh : Dresser 정보 별도로 관리
                    if (CDataOption.UseSeperateDresser)
                    {
                        if (m_eWy == EWay.L)
                        {
                            sName = lbxL_List_Whl.SelectedItem.ToString();
                        }
                        else
                        {
                            sName = lbxR_List_Whl.SelectedItem.ToString();
                        }
                    }
                    else
                    {
                        if (m_eWy == EWay.L)
                        {
                            sName = lbxL_List.SelectedItem.ToString();
                        }
                        else
                        {
                            sName = lbxR_List.SelectedItem.ToString();
                        }
                    }

                    // 저장 여부 확인
                    sNote = string.Format("Do you want [{0}] wheel file?", sName);
                    if (CMsg.Show(eMsg.Warning, "Warning", sNote) == DialogResult.OK)
                    {
                        _Get();

                        iRet = CWhl.It.Save(eWy, m_tWhl);
                        if (CDataOption.UseSeperateDresser) iRetDrs = CWhl.It.SaveDrs(eWy, m_tDrs);
                        //if (iRet != 0)
                        if (iRet != 0 || (CDataOption.UseSeperateDresser && iRet!=0 && iRetDrs != 0))
                        {
                            CErr.Show(eErr.SYSTEM_WHEEL_FILE_SAVE_ERROR);
                        }
                        else
                        {
                            _Set();
                            if (m_eWy == EWay.L)
                            {
                                CData.Whls[0] = m_tWhl;
                                //211122 pjh : Dresser 별도 관리 기능
                                if (CDataOption.UseSeperateDresser)
                                {
                                    txtL_DrHei.Text = CData.Drs[0].dDrsH.ToString();
                                    CData.Whls[0].dDrsAf = CData.DrsAf[0] = CData.Drs[0].dDrsH;
                                    //CData.Drs[0] = m_tDrs;
                                }
                                //
                            }
                            else
                            {
                                CData.Whls[1] = m_tWhl;
                                //211122 pjh : Dresser 별도 관리 기능
                                if (CDataOption.UseSeperateDresser)
                                {
                                    txtR_DrHei.Text = CData.Drs[1].dDrsH.ToString();
                                    CData.Whls[1].dDrsAf = CData.DrsAf[1] = CData.Drs[1].dDrsH;
                                    //CData.Drs[1] = m_tDrs;
                                }
                                //
                            }
                            CMsg.Show(eMsg.Notice, "Notice", "Wheel file save success");
                            CData.WhlChgSeq.bSltWhlFile = true; //190711 ksg :추가
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
        }

        private void btn_New_Click(object sender, EventArgs e)
        {
            CData.bLastClick = true; //190509 ksg :
            Button mBtn = sender as Button;
            BeginInvoke(new Action(() => mBtn.Enabled = false));

            string sPath = GV.PATH_WHEEL;
            string sPathWhl = "";

            try
            {
                if (m_eWy == EWay.L) { sPath += "Left\\"; }
                else { sPath += "Right\\"; }

                using (frmTxt mForm = new frmTxt(m_eWy.ToString() + " Wheel Name"))
                {
                    if (mForm.ShowDialog() == DialogResult.Cancel) { return; }

                    //if (File.Exists(sPath + mForm.Val + ".whl"))
                    if (Directory.Exists(sPath + mForm.Val))
                    {
                        CMsg.Show(eMsg.Error, "Error", "Wheel name exist !!!");
                        return;
                    }
                    //sPath += mForm.Val + ".whl";
                    sPath += mForm.Val + "\\";
                    Directory.CreateDirectory(sPath);

                    //sPath += mForm.Val + ".whl";
                    sPathWhl += sPath + "WheelInfo.whl";

                    // 휠 데이터 초기값
                    CWhl.It.InitWhl(out m_tWhl);
                    m_tWhl.sWhlName = mForm.Val;

                    CWhl.It.Save(sPathWhl, m_tWhl);

                    // 2021.04.06 SungTae Start : Wheel History 파일의 무한정 크기 증가로 인한 랙 발생으로 Daily 별로 생성하도록 변경
                    //sPath += "WheelHistory.csv";

                    // 2021.06.09 lhs Start : 폴더 없을시 만들기
                    //sPath += "WheelHistory\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".csv";
                    sPath += "WheelHistory\\";
                    if (!Directory.Exists(sPath))
                    {
                        Directory.CreateDirectory(sPath);
                    }
                    sPath += DateTime.Now.ToString("yyyy-MM-dd") + ".csv";
                    // 2021.06.09 lhs End
                    // 2021.04.06 SungTae End

                    CWhl.It.SaveNewHistory(sPath);

                    _ListUp();
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                BeginInvoke(new Action(() => mBtn.Enabled = true));
            }
        }

        private void btn_Copy_Click(object sender, EventArgs e)
        {
            CData.bLastClick = true; //190509 ksg :
            Button mBtn = sender as Button;
            BeginInvoke(new Action(() => mBtn.Enabled = false));

            int iIdx = 0;
            string sSrc = "";
            string sDst = "";
            string sPath = "";

            //211119 pjh : Dresser 정보 별도로 관리
            if(CDataOption.UseSeperateDresser)
            {
                if (m_eWy == EWay.L)
                { iIdx = lbxL_List_Whl.SelectedIndex; }
                else
                { iIdx = lbxR_List_Whl.SelectedIndex; ; }
            }
            else
            {
                if (m_eWy == EWay.L)
                { iIdx = lbxL_List.SelectedIndex; }
                else
                { iIdx = lbxR_List.SelectedIndex; ; }
            }
            //if (m_eWy == EWay.L)
            //{ iIdx = lbxL_List.SelectedIndex; }
            //else
            //{ iIdx = lbxR_List.SelectedIndex; ; }

            try
            {
                if (iIdx < 0)
                {
                    CMsg.Show(eMsg.Error, "Error", "Not selected wheel");
                    return;
                }

                using (frmTxt mForm = new frmTxt("Save As Wheel Name"))
                {
                    if (mForm.ShowDialog() == DialogResult.OK)
                    {
                        //211119 pjh : Dresser 정보 별도로 관리
                        if(CDataOption.UseSeperateDresser)
                        {
                            //if (m_eWy == EWay.L) { sSrc = GV.PATH_WHEEL + lbxL_List.SelectedValue.ToString() + ".whl"; }
                            if (m_eWy == EWay.L)
                            {
                                sSrc = GV.PATH_WHEEL + "Left\\" + lbxL_List_Whl.SelectedItem.ToString() + "\\";
                                sDst = GV.PATH_WHEEL + "Left\\";
                            }
                            //else { sSrc = GV.PATH_WHEEL + lbxR_List.SelectedValue.ToString() + ".whl"; }\
                            else
                            {
                                sSrc = GV.PATH_WHEEL + "Right\\" + lbxR_List_Whl.SelectedItem.ToString() + "\\";
                                sDst = GV.PATH_WHEEL + "Right\\";
                            }
                        }
                        else
                        {
                            //if (m_eWy == EWay.L) { sSrc = GV.PATH_WHEEL + lbxL_List.SelectedValue.ToString() + ".whl"; }
                            if (m_eWy == EWay.L)
                            {
                                sSrc = GV.PATH_WHEEL + "Left\\" + lbxL_List.SelectedItem.ToString() + "\\";
                                sDst = GV.PATH_WHEEL + "Left\\";
                            }
                            //else { sSrc = GV.PATH_WHEEL + lbxR_List.SelectedValue.ToString() + ".whl"; }\
                            else
                            {
                                sSrc = GV.PATH_WHEEL + "Right\\" + lbxR_List.SelectedItem.ToString() + "\\";
                                sDst = GV.PATH_WHEEL + "Right\\";
                            }
                        }
                        
                        //sDst = GV.PATH_WHEEL + mForm.Val + "\\" + mForm.Val + ".whl";
                        // 원본과 동일한지 확인
                        sDst += mForm.Val + "\\";
                        if (sSrc == sDst)
                        {
                            CMsg.Show(eMsg.Error, "Error", "Wheel name same !!!");
                            return;
                        }
                        // 동일한 이름 확인
                        if (Directory.Exists(sDst))
                        {
                            CMsg.Show(eMsg.Error, "Error", "Wheel name exist !!!");
                            return;
                        }
                        else
                        {
                            Directory.CreateDirectory(sDst);
                            /*
                            if (m_eWy == EWay.L)
                            {
                                sSrc += lbxL_List.SelectedItem.ToString() + ".whl";
                            }
                            else
                            {
                                sSrc += lbxR_List.SelectedItem.ToString() + ".whl";
                            }
                            */
                            sSrc += "WheelInfo.whl";
                        }

                        // 2021.04.06 SungTae Start : Wheel History 파일의 무한정 크기 증가로 인한 랙 발생으로 Daily 별로 생성하도록 변경
                        //sPath = sDst + "WheelHistory.csv";
                        
                        // 2021.06.09 lhs Start : 폴더 없을시 만들기
                        //sPath = sDst + "WheelHistory\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".csv";
                        sPath = sDst + "WheelHistory\\";
                        if (!Directory.Exists(sPath))
                        {
                            Directory.CreateDirectory(sPath);
                        }
                        sPath += DateTime.Now.ToString("yyyy-MM-dd") + ".csv";
                        // 2021.06.09 lhs End

                        // 2021.04.06 SungTae End

                        sDst += "WheelInfo.whl";
                        
                        // 파일 복사
                        FileSystem.CopyFile(sSrc, sDst, UIOption.AllDialogs);
                        CCheckChange.SaveAs("WHEEL", sSrc, sDst); //200716 lks

                        // 복사한 파일의 이름 변수 변경을 위해 로드 -> 이름변경 -> 세이브
                        CWhl.It.Load(sDst, out m_tWhl);
                        m_tWhl.sWhlName = mForm.Val;
                        CWhl.It.Save(sDst, m_tWhl);
                        //휠 파라미터와 휠 이름 제외한 휠 정보 초기화
                        CWhl.It.SaveNew(m_eWy, m_tWhl);
                        CWhl.It.SaveNewHistory(sPath);

                        _ListUp();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally { BeginInvoke(new Action(() => mBtn.Enabled = true)); }
        }

        private void btn_Del_Click(object sender, EventArgs e)
        {
            CData.bLastClick = true; //190509 ksg :
            Button mBtn = sender as Button;
            BeginInvoke(new Action(() => mBtn.Enabled = false));

            int iIdx = 0;
            string sName = "";
            string sPath = GV.PATH_WHEEL;

            if (m_eWy == EWay.L) { iIdx = m_iLeft; }
            else { iIdx = m_iRight; }

            try
            {
                if (iIdx < 0)
                {
                    CMsg.Show(eMsg.Error, "Error", "Not selected wheel");
                    return;
                }
                //211119 pjh : Dresser 정보 별도로 관리
                if (CDataOption.UseSeperateDresser)
                {
                    if (m_eWy == EWay.L)
                    {
                        sName = lbxL_List_Whl.SelectedItem.ToString();
                        sPath += "Left\\";
                        if (CData.Whls[0].sWhlName == sName)
                        {
                            CMsg.Show(eMsg.Error, "Error", "Current using wheel");
                            return;
                        }
                    }
                    else
                    {
                        sName = lbxR_List_Whl.SelectedItem.ToString();
                        sPath += "Right\\";

                        if (CData.Whls[1].sWhlName == sName)
                        {
                            CMsg.Show(eMsg.Error, "Error", "Current using wheel");
                            return;
                        }
                    }
                }
                else
                {
                    if (m_eWy == EWay.L)
                    {
                        sName = lbxL_List.SelectedItem.ToString();
                        sPath += "Left\\";
                        if (CData.Whls[0].sWhlName == sName)
                        {
                            CMsg.Show(eMsg.Error, "Error", "Current using wheel");
                            return;
                        }
                    }
                    else
                    {
                        sName = lbxR_List.SelectedItem.ToString();
                        sPath += "Right\\";

                        if (CData.Whls[1].sWhlName == sName)
                        {
                            CMsg.Show(eMsg.Error, "Error", "Current using wheel");
                            return;
                        }
                    }
                }

                // 현재 사용중인지 확인

                // 경로 생성
                sPath = sPath + sName;
                //File.Delete(sPath);
                CCheckChange.DeleteLog("WHEEL", sPath); //200716 lks

                Directory.Delete(sPath, true);

                CWhl.It.InitWhl(out m_tWhl);
                //if (m_eWy == EWay.L) { lbxL_List.ClearSelected(); }

                //211119 pjh : Dresser 정보 별도로 관리
                if (CDataOption.UseSeperateDresser)
                {
                    if (m_eWy == EWay.L)
                    {
                        lbxL_List_Whl.ClearSelected();
                        m_iLeft--;
                    }
                    //else { lbxR_List.ClearSelected(); }
                    else
                    {
                        lbxR_List_Whl.ClearSelected();
                        m_iRight--;
                    }
                }
                else
                {
                    if (m_eWy == EWay.L)
                    {
                        lbxL_List.ClearSelected();
                        m_iLeft--;
                    }
                    //else { lbxR_List.ClearSelected(); }
                    else
                    {
                        lbxR_List.ClearSelected();
                        m_iRight--;
                    }
                }

                _ListUp();
            }
            catch (Exception ex)
            {

            }
            finally
            { BeginInvoke(new Action(() => mBtn.Enabled = true)); }
        }

        private void lbx_List_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox mLbx = sender as ListBox;
            int iIdx = 0;

            //211119 pjh : Dresser 정보 별도로 관리
            if (CDataOption.UseSeperateDresser)
            {
                if (m_eWy == EWay.L)
                {
                    iIdx = lbxL_List_Whl.SelectedIndex;
                }
                else
                {
                    iIdx = m_iRight;
                    iIdx = lbxR_List_Whl.SelectedIndex;
                }
            }
            else
            {
                if (m_eWy == EWay.L)
                {
                    iIdx = lbxL_List.SelectedIndex;
                }
                else
                {
                    iIdx = m_iRight;
                    iIdx = lbxR_List.SelectedIndex;
                }
            }

            if (iIdx < 0)
            {
                CWhl.It.InitWhl(out m_tWhl);
                _Set();
            }
            else
            {
                if (m_eWy == EWay.L) { m_iLeft  = iIdx; }
                else                 { m_iRight = iIdx; }
                CWhl.It.Load(m_eWy, mLbx.SelectedItem.ToString(), out m_tWhl);

                _Set();
            }
        }

        private void rdb_SubMenu_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton mRdb = sender as RadioButton;
            if (m_eWy == EWay.L)
            {
                tabSub1.SelectTab(int.Parse(mRdb.Tag.ToString()));
            }
            else
            {
                tabSub2.SelectTab(int.Parse(mRdb.Tag.ToString()));
            }
            
        }

        private void rdbMn_CheckedChanged(object sender, EventArgs e)
        {
            CData.bLastClick = true; //190509 ksg :
            RadioButton mRdb = sender as RadioButton;
            tabWhl.SelectTab(int.Parse(mRdb.Tag.ToString()));
            _ListUp();
            //211122 pjh :
            _ListUpDrs();
            //
        }

        private void HideMenu(ELv RetLv)
        {
            btnL_New      .Enabled = (int)RetLv >= CData.Opt.iLvWhlNew      ;
            btnR_New      .Enabled = (int)RetLv >= CData.Opt.iLvWhlNew      ;
            btnL_SaveAs   .Enabled = (int)RetLv >= CData.Opt.iLvWhlSaveAs   ;
            btnR_SaveAs   .Enabled = (int)RetLv >= CData.Opt.iLvWhlSaveAs   ;
            btnL_Del      .Enabled = (int)RetLv >= CData.Opt.iLvWhlDel      ;
            btnR_Del      .Enabled = (int)RetLv >= CData.Opt.iLvWhlDel      ;
            btnL_Save     .Enabled = (int)RetLv >= CData.Opt.iLvWhlSave     ;
            btnR_Save     .Enabled = (int)RetLv >= CData.Opt.iLvWhlSave     ;

            if (CData.CurCompany == ECompany.Qorvo    || CData.CurCompany == ECompany.Qorvo_DZ ||
                CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC ||   // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                CData.CurCompany == ECompany.ASE_KR   || CData.CurCompany == ECompany.SST      ||
                CData.CurCompany == ECompany.SCK      || CData.CurCompany == ECompany.JSCK     || CData.CurCompany == ECompany.JCET) //200121 ksg : , 200625 lks
            {
                btnL_WhlChange   .Visible = (int)RetLv >= CData.Opt.iLvWhlChange   ; //190717 ksg : 추가
                btnR_WhlChange   .Visible = (int)RetLv >= CData.Opt.iLvWhlChange   ; //190717 ksg : 추가
                btnL_DrsChange   .Visible = (int)RetLv >= CData.Opt.iLvWhlDrsChange; //190717 ksg : 추가
                btnR_DrsChange   .Visible = (int)RetLv >= CData.Opt.iLvWhlDrsChange; //190717 ksg : 추가
                btnL_ChangeCancel.Visible = (int)RetLv >= CData.Opt.iLvWhlDrsChange;
                btnR_ChangeCancel.Visible = (int)RetLv >= CData.Opt.iLvWhlDrsChange;
            }
            
        }

        private void btn_WhlChange_Click(object sender, EventArgs e)
        {
            if(CData.WhlChgSeq.bStart || CData.DrsChgSeq.bStart) return;
            //190920 ksg : Adding
            if((CSQ_Main.It.m_iStat != EStatus.Stop && CSQ_Main.It.m_iStat != EStatus.Idle) || (CSQ_Man.It.Seq != ESeq.Idle)) 
            {
                CMsg.Show(eMsg.Notice, "Notice", "Can't start wheel change. First check machine status");
                return;
            }
            CData.WhlChgSeq.bStart          = true ;
            CData.WhlChgSeq.iStep           = 1    ;
            CData.WhlChgSeq.bBtnHide        = false;
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
            CData.WhlChgSeq.bLWhlJigCheck   = false;
            CData.WhlChgSeq.bRWhlJigCheck   = false;

            CData.bWhlDrsLimitAlarmSkip = false; //200730 jhc : Wheel/Dresser Limit Alaram 보류용 (ASE-Kr VOC) - Wheel/Dresser 교체 시도 시 Wheel/Dresser Limit Alarm 보류 해제
        }

        private void btn_DrsChange_Click(object sender, EventArgs e)
        {
            if(CData.DrsChgSeq.bStart || CData.DrsChgSeq.bStart) return;

            //190920 ksg : Adding
            if((CSQ_Main.It.m_iStat != EStatus.Stop && CSQ_Main.It.m_iStat != EStatus.Idle) || (CSQ_Man.It.Seq != ESeq.Idle)) 
            {
                CMsg.Show(eMsg.Notice, "Notice", "Can't start dresser change. First check machine status.");
                return;
            }

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

            CData.bWhlDrsLimitAlarmSkip = false; //200730 jhc : Wheel/Dresser Limit Alaram 보류용 (ASE-Kr VOC) - Wheel/Dresser 교체 시도 시 Wheel/Dresser Limit Alarm 보류 해제
        }

        private void btn_ChangeCancel_Click(object sender, EventArgs e)
        {
            if(CMsg.Show(eMsg.Notice, "Wheel Change", "Wheel Change Cancel") == DialogResult.OK)
            {
                WheelChangeCancel  ();
                DresserChangeCancel();
            }
        }

         private void WheelChangeCancel()
        {
            CData.WhlChgSeq.bStart          = false;
            CData.WhlChgSeq.iStep           = 0    ;
            CData.WhlChgSeq.bStartShow      = false;
            CData.WhlChgSeq.bBtnHide        = false;
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

            CData.bWhlChangeMode = false;       // 2020.09.10 SungTae : Wheel Change 시 PCW Auto Off 기능 보류용 (Qorvo VOC)
        }
        private void DresserChangeCancel()
        {
            CData.DrsChgSeq.bStart          = false;
            CData.DrsChgSeq.iStep           = 0    ;
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

        private void dtpHistoryL_ValueChanged(object sender, EventArgs e)
        {
            // 2021.05.18 lhs Start
            //_HistoryUp(EWay.L);

            DateTimePicker obj = sender as DateTimePicker;

            DateTime t1 = dtpHistoryL_From.Value;
            DateTime t2 = dtpHistoryL_To.Value;
            int nRet = DateTime.Compare(t1, t2);
            if (nRet > 0)
            {
                if (obj.Name.Contains("From"))  {   dtpHistoryL_From.Value = dtpHistoryL_To.Value;  }
                else                            {   dtpHistoryL_To.Value = dtpHistoryL_From.Value;  }
                return;
            }

            SetHistoryAllView(false);
            _HistoryUp(EWay.L, t1, t2); 
            // 2021.05.18 lhs End
        }

        private void dtpHistoryR_ValueChanged(object sender, EventArgs e)
        {
            // 2021.05.18 lhs Start
            //_HistoryUp(EWay.R);

            DateTimePicker obj = sender as DateTimePicker;

            DateTime t1 = dtpHistoryR_From.Value;
            DateTime t2 = dtpHistoryR_To.Value;
            int nRet = DateTime.Compare(t1, t2);
            if (nRet > 0)
            {
                if (obj.Name.Contains("From"))  { dtpHistoryR_From.Value = dtpHistoryR_To.Value; }
                else                            { dtpHistoryR_To.Value   = dtpHistoryR_From.Value; }
                return;
            }
            SetHistoryAllView(false);
            _HistoryUp(EWay.R, t1, t2);
            // 2021.05.18 lhs End
        }

        private void btnHistoryL_All_Click(object sender, EventArgs e)
        {
            SetHistoryAllView(true);
            _HistoryUp_All(EWay.L);
        }

        private void btnHistoryR_All_Click(object sender, EventArgs e)
        {
            SetHistoryAllView(true);
            _HistoryUp_All(EWay.R);
        }

        private void SetHistoryAllView(bool bAllView)
        {
            m_bHistoryAllView = bAllView;
            if (bAllView)
            {
                btnHistoryL_All.ForeColor = Color.Yellow;
                btnHistoryR_All.ForeColor = Color.Yellow;

                lblHistoryL_From.ForeColor = Color.White;
                lblHistoryL_To.ForeColor = Color.White;
                lblHistoryR_From.ForeColor = Color.White;
                lblHistoryR_To.ForeColor = Color.White;
            }
            else
            {
                btnHistoryL_All.ForeColor = Color.White;
                btnHistoryR_All.ForeColor = Color.White;

                lblHistoryL_From.ForeColor = Color.Yellow;
                lblHistoryL_To.ForeColor   = Color.Yellow;
                lblHistoryR_From.ForeColor = Color.Yellow;
                lblHistoryR_To.ForeColor   = Color.Yellow;
            }
        }

		//220106 : 전체 기간 Dresser History View 함수
        private void SetDrsHistoryAllView(bool bAllView)
        {
            m_bDrsHistoryAllView = bAllView;
            if (bAllView)
            {
                btnDrsHistoryL_All.ForeColor = Color.Yellow;
                btnDrsHistoryR_All.ForeColor = Color.Yellow;

                lblDrsHistoryL_From.ForeColor = Color.White;
                lblDrsHistoryL_To.ForeColor = Color.White;
                lblDrsHistoryR_From.ForeColor = Color.White;
                lblDrsHistoryR_To.ForeColor = Color.White;
            }
            else
            {
                btnDrsHistoryL_All.ForeColor = Color.White;
                btnDrsHistoryR_All.ForeColor = Color.White;

                lblDrsHistoryL_From.ForeColor = Color.Yellow;
                lblDrsHistoryL_To.ForeColor = Color.Yellow;
                lblDrsHistoryR_From.ForeColor = Color.Yellow;
                lblDrsHistoryR_To.ForeColor = Color.Yellow;
            }
        }

        //211014 pjh : Device Wheel 사용 시 Wheel File 편집 제한
        public void LabelEnable(bool bVal)
        {
            if(bVal)
            {
                //Left
                //Air Cut
                txtL_Air.Enabled   = true;
                //Using Step 1
                cmbL_U1Mod.Enabled = true;
                txtL_U1To.Enabled  = true;
                txtL_U1Cy.Enabled  = true;
                txtL_U1Tb.Enabled  = true;
                txtL_U1Sp.Enabled  = true;
                cmbL_U1Dir.Enabled = true;
                //Using Step 2
                cmbL_U2Mod.Enabled = true;
                txtL_U2To.Enabled  = true;
                txtL_U2Cy.Enabled  = true;
                txtL_U2Tb.Enabled  = true;
                txtL_U2Sp.Enabled  = true;
                cmbL_U2Dir.Enabled = true;
                //Replace Step 1
                cmbL_R1Mod.Enabled = true;
                txtL_R1To.Enabled  = true;
                txtL_R1Cy.Enabled  = true;
                txtL_R1Tb.Enabled  = true;
                txtL_R1Sp.Enabled  = true;
                cmbL_R1Dir.Enabled = true;
                //Replace Step 2
                cmbL_R2Mod.Enabled = true;
                txtL_R2To.Enabled  = true;
                txtL_R2Cy.Enabled  = true;
                txtL_R2Tb.Enabled  = true;
                txtL_R2Sp.Enabled  = true;
                cmbL_R2Dir.Enabled = true;
                if (CDataOption.IsDrsAirCutReplace)
                {
                    txtL_Air_Rep.Enabled = true;
                }
                //Wheel&Dresser Limit
                txtL_WhlLimit.Enabled = true;
                txtL_DrsLimit.Enabled = true;

                //Right
                //Air Cut
                txtR_Air.Enabled   = true;
                //Using Step 1
                cmbR_U1Mod.Enabled = true;
                txtR_U1To.Enabled  = true;
                txtR_U1Cy.Enabled  = true;
                txtR_U1Tb.Enabled  = true;
                txtR_U1Sp.Enabled  = true;
                cmbR_U1Dir.Enabled = true;
                //Using Step 2
                cmbR_U2Mod.Enabled = true;
                txtR_U2To.Enabled  = true;
                txtR_U2Cy.Enabled  = true;
                txtR_U2Tb.Enabled  = true;
                txtR_U2Sp.Enabled  = true;
                cmbR_U2Dir.Enabled = true;
                //Replace Step 1
                cmbR_R1Mod.Enabled = true;
                txtR_R1To.Enabled  = true;
                txtR_R1Cy.Enabled  = true;
                txtR_R1Tb.Enabled  = true;
                txtR_R1Sp.Enabled  = true;
                cmbR_R1Dir.Enabled = true;
                //Replace Step 2
                cmbR_R2Mod.Enabled = true;
                txtR_R2To.Enabled  = true;
                txtR_R2Cy.Enabled  = true;
                txtR_R2Tb.Enabled  = true;
                txtR_R2Sp.Enabled  = true;
                cmbR_R2Dir.Enabled = true;
                if (CDataOption.IsDrsAirCutReplace)
                {
                    txtR_Air_Rep.Enabled = true;
                }
                //Wheel&Dresser Limit
                txtR_WhlLimit.Enabled = true;
                txtR_DrsLimit.Enabled = true;
            }
            else
            {
                //Left
                //Air Cut
                txtL_Air.Enabled = false;
                //Using Step 1
                cmbL_U1Mod.Enabled = false;
                txtL_U1To.Enabled  = false;
                txtL_U1Cy.Enabled  = false;
                txtL_U1Tb.Enabled  = false;
                txtL_U1Sp.Enabled  = false;
                cmbL_U1Dir.Enabled = false;
                //Using Step 2
                cmbL_U2Mod.Enabled = false;
                txtL_U2To.Enabled  = false;
                txtL_U2Cy.Enabled  = false;
                txtL_U2Tb.Enabled  = false;
                txtL_U2Sp.Enabled  = false;
                cmbL_U2Dir.Enabled = false;
                //Replace Step 1
                cmbL_R1Mod.Enabled = false;
                txtL_R1To.Enabled  = false;
                txtL_R1Cy.Enabled  = false;
                txtL_R1Tb.Enabled  = false;
                txtL_R1Sp.Enabled  = false;
                cmbL_R1Dir.Enabled = false;
                //Replace Step 2
                cmbL_R2Mod.Enabled = false;
                txtL_R2To.Enabled  = false;
                txtL_R2Cy.Enabled  = false;
                txtL_R2Tb.Enabled  = false;
                txtL_R2Sp.Enabled  = false;
                cmbL_R2Dir.Enabled = false;
                if(CDataOption.IsDrsAirCutReplace)
                {
                    txtL_Air_Rep.Enabled = false;
                }
                //Wheel&Dresser Limit
                txtL_WhlLimit.Enabled = false;
                txtL_DrsLimit.Enabled = false;
                //Right
                //Air Cut
                txtR_Air.Enabled   = false;
                //Using Step 1
                cmbR_U1Mod.Enabled = false;
                txtR_U1To.Enabled  = false;
                txtR_U1Cy.Enabled  = false;
                txtR_U1Tb.Enabled  = false;
                txtR_U1Sp.Enabled  = false;
                cmbR_U1Dir.Enabled = false;
                //Using Step 2
                cmbR_U2Mod.Enabled = false;
                txtR_U2To.Enabled  = false;
                txtR_U2Cy.Enabled  = false;
                txtR_U2Tb.Enabled  = false;
                txtR_U2Sp.Enabled  = false;
                cmbR_U2Dir.Enabled = false;
                //Replace Step 1
                cmbR_R1Mod.Enabled = false;
                txtR_R1To.Enabled  = false;
                txtR_R1Cy.Enabled  = false;
                txtR_R1Tb.Enabled  = false;
                txtR_R1Sp.Enabled  = false;
                cmbR_R1Dir.Enabled = false;
                //Replace Step 2
                cmbR_R2Mod.Enabled = false;
                txtR_R2To.Enabled  = false;
                txtR_R2Cy.Enabled  = false;
                txtR_R2Tb.Enabled  = false;
                txtR_R2Sp.Enabled  = false;
                cmbR_R2Dir.Enabled = false;
                if (CDataOption.IsDrsAirCutReplace)
                {
                    txtR_Air_Rep.Enabled = false;
                }
                //Wheel&Dresser Limit
                txtR_WhlLimit.Enabled = false;
                txtR_DrsLimit.Enabled = false;
            }
        }

        //211122 pjh : 신규 Dresser File 생성
        private void btn_New_Drs_Click(object sender, EventArgs e)
        {
            CData.bLastClick = true; //190509 ksg :
            Button mBtn = sender as Button;
            BeginInvoke(new Action(() => mBtn.Enabled = false));

            string sPath = GV.PATH_DRESSER;
            string sPathDrs = "";

            try
            {
                if (m_eWy == EWay.L) { sPath += "Left\\"; }
                else { sPath += "Right\\"; }

                using (frmTxt mForm = new frmTxt(m_eWy.ToString() + " Dresser Name"))
                {
                    if (mForm.ShowDialog() == DialogResult.Cancel) { return; }

                    //if (File.Exists(sPath + mForm.Val + ".whl"))
                    if (Directory.Exists(sPath + mForm.Val))
                    {
                        CMsg.Show(eMsg.Error, "Error", "Dresser name exist !!!");
                        return;
                    }
                    //sPath += mForm.Val + ".whl";
                    sPath += mForm.Val + "\\";
                    Directory.CreateDirectory(sPath);

                    //sPath += mForm.Val + ".whl";
                    sPathDrs += sPath + "DresserInfo.txt";

                    // 휠 데이터 초기값
                    CWhl.It.InitDresser(out m_tDrs);
                    m_tDrs.sDrsName = mForm.Val;
                    //if(m_tDrs.dDrsH == 0.0)
                    //{
                    //    m_tDrs.dDrsH = CData.Whls[(int)m_eWy].dDrsAf;
                    //}

                    CWhl.It.SaveDrs(sPathDrs, m_tDrs);

                    _ListUpDrs();
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                BeginInvoke(new Action(() => mBtn.Enabled = true));
            }
        }
        //
        //211122 pjh : Dresser File 선택
        private void lbx_List_Drs_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox mLbx = sender as ListBox;
            int iIdx = 0;

            
            if (m_eWy == EWay.L)
            {
                iIdx = lbxL_List_Drs.SelectedIndex;
            }
            else
            {
                iIdx = m_iRight_Drs;
                iIdx = lbxR_List_Drs.SelectedIndex;
            }
            

            if (iIdx < 0)
            {
                CWhl.It.InitDresser(out m_tDrs);
                _Set();
            }
            else
            {
                if (m_eWy == EWay.L) { m_iLeft_Drs = iIdx; }
                else { m_iRight_Drs = iIdx; }
                CWhl.It.LoadDrs(m_eWy, mLbx.SelectedItem.ToString(), out m_tDrs);

                _Set();
            }
        }
        //
        //211122 pjh : Dresser File Copy
        private void btn_Copy_Drs_Click(object sender, EventArgs e)
        {
            CData.bLastClick = true; //190509 ksg :
            Button mBtn = sender as Button;
            BeginInvoke(new Action(() => mBtn.Enabled = false));

            int iIdx = 0;
            string sSrc = "";
            string sDst = "";

            if (m_eWy == EWay.L)
            { iIdx = lbxL_List_Drs.SelectedIndex; }
            else
            { iIdx = lbxR_List_Drs.SelectedIndex; ; }

            try
            {
                if (iIdx < 0)
                {
                    CMsg.Show(eMsg.Error, "Error", "Not selected Dresser");
                    return;
                }

                using (frmTxt mForm = new frmTxt("Save As Dresser Name"))
                {
                    if (mForm.ShowDialog() == DialogResult.OK)
                    {
                        if (m_eWy == EWay.L)
                        {
                            sSrc = GV.PATH_DRESSER + "Left\\" + lbxL_List_Drs.SelectedItem.ToString() + "\\";
                            sDst = GV.PATH_DRESSER + "Left\\";
                        }
                        else
                        {
                            sSrc = GV.PATH_DRESSER + "Right\\" + lbxR_List_Drs.SelectedItem.ToString() + "\\";
                            sDst = GV.PATH_DRESSER + "Right\\";
                        }
                        
                        // 원본과 동일한지 확인
                        sDst += mForm.Val + "\\";
                        if (sSrc == sDst)
                        {
                            CMsg.Show(eMsg.Error, "Error", "Dresser name same !!!");
                            return;
                        }
                        // 동일한 이름 확인
                        if (Directory.Exists(sDst))
                        {
                            CMsg.Show(eMsg.Error, "Error", "Dresser name exist !!!");
                            return;
                        }
                        else
                        {
                            Directory.CreateDirectory(sDst);
                            
                            sSrc += "DresserInfo.txt";
                        }

                        sDst += "DresserInfo.txt";

                        // 파일 복사
                        FileSystem.CopyFile(sSrc, sDst, UIOption.AllDialogs);
                        CCheckChange.SaveAs("Dresser", sSrc, sDst); //200716 lks

                        // 복사한 파일의 이름 변수 변경을 위해 로드 -> 이름변경 -> 세이브
                        CWhl.It.LoadDrs(sDst, out m_tDrs);
                        m_tDrs.sDrsName = mForm.Val;
                        CWhl.It.SaveDrs(sDst, m_tDrs);
                        //휠 파라미터와 휠 이름 제외한 휠 정보 초기화
                        CWhl.It.SaveNewDrs(m_eWy, m_tDrs);

                        _ListUpDrs();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally { BeginInvoke(new Action(() => mBtn.Enabled = true)); }
        }
        //
        //211122 pjh : Dresser File 삭제
        private void btn_Del_Drs_Click(object sender, EventArgs e)
        {
            CData.bLastClick = true; //190509 ksg :
            Button mBtn = sender as Button;
            BeginInvoke(new Action(() => mBtn.Enabled = false));

            int iIdx = 0;
            string sName = "";
            string sPath = GV.PATH_DRESSER;

            if (m_eWy == EWay.L) { iIdx = m_iLeft_Drs; }
            else { iIdx = m_iRight_Drs; }

            try
            {
                if (iIdx < 0)
                {
                    CMsg.Show(eMsg.Error, "Error", "Not selected Dresser");
                    return;
                }
                            
                if (m_eWy == EWay.L)
                {
                    sName = lbxL_List_Drs.SelectedItem.ToString();
                    sPath += "Left\\";
                    if (CData.Drs[0].sDrsName == sName)
                    {
                        CMsg.Show(eMsg.Error, "Error", "Current using dresser");
                        return;
                    }
                }
                else
                {
                    sName = lbxR_List_Drs.SelectedItem.ToString();
                    sPath += "Right\\";

                    if (CData.Drs[1].sDrsName == sName)
                    {
                        CMsg.Show(eMsg.Error, "Error", "Current using dresser");
                        return;
                    }
                }
                
                // 현재 사용중인지 확인

                // 경로 생성
                sPath = sPath + sName;
                //File.Delete(sPath);
                CCheckChange.DeleteLog("DRESSER", sPath); //200716 lks

                Directory.Delete(sPath, true);

                CWhl.It.InitDresser(out m_tDrs);
                
                if (m_eWy == EWay.L)
                {
                    lbxL_List.ClearSelected();
                    m_iLeft_Drs--;
                }
                else
                {
                    lbxR_List.ClearSelected();
                    m_iRight_Drs--;
                }
                
                _ListUpDrs();
            }
            catch (Exception ex)
            {

            }
            finally
            { BeginInvoke(new Action(() => mBtn.Enabled = true)); }
        }

        private void btnDrsHistoryL_All_Click(object sender, EventArgs e)
        {
            SetDrsHistoryAllView(true);
            _DrsHistoryUp_All(EWay.L);
        }

        private void btnDrsHistoryR_All_Click(object sender, EventArgs e)
        {
            SetDrsHistoryAllView(true);
            _DrsHistoryUp_All(EWay.R);
        }

        private void dtpDrsHistoryL_ValueChanged(object sender, EventArgs e)
        {
            DateTimePicker obj = sender as DateTimePicker;

            DateTime t1 = dtpDrsHistoryL_From.Value;
            DateTime t2 = dtpDrsHistoryL_To.Value;
            int nRet = DateTime.Compare(t1, t2);
            if (nRet > 0)
            {
                if (obj.Name.Contains("From")) { dtpDrsHistoryL_From.Value = dtpDrsHistoryL_To.Value; }
                else { dtpDrsHistoryL_To.Value = dtpDrsHistoryL_From.Value; }
                return;
            }
            SetDrsHistoryAllView(false);
            _DrsHistoryUp(EWay.L, t1, t2);
        }

        private void dtpDrsHistoryR_ValueChanged(object sender, EventArgs e)
        {            
            DateTimePicker obj = sender as DateTimePicker;

            DateTime t1 = dtpDrsHistoryR_From.Value;
            DateTime t2 = dtpDrsHistoryR_To.Value;
            int nRet = DateTime.Compare(t1, t2);
            if (nRet > 0)
            {
                if (obj.Name.Contains("From")) { dtpDrsHistoryR_From.Value = dtpDrsHistoryR_To.Value; }
                else { dtpDrsHistoryR_To.Value = dtpDrsHistoryR_From.Value; }
                return;
            }
            SetDrsHistoryAllView(false);
            _DrsHistoryUp(EWay.R, t1, t2);
        }

        
        //
    }
}
