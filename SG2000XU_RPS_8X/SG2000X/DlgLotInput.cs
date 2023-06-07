using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices; // for DrawingControl
using System.Windows.Forms;

namespace SG2000X
{
    public partial class CDlgLotInput : Form
    {
        //public string Val { get; private set; }
        //private CTim m_Delay = new CTim();
        ////20190618 ghk_dfserver
        //private bool m_bLotClick = false;
        //private bool m_bLotOp = false;
        ////

        public CDlgLotInput()
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

            // Multi-LOT 기능을 사용시에는 LOT 표시 Grid의 성능 향상을 위해 Double Buffer 기능 활성화
            DrawingControl.SetDoubleBuffered(gridLotList);        // Enable double buffer


            // txt_Dev.Text = CData.Dev.sName;

            //cmbMgzQty.Items.Clear();
            //for (int i = 1; i <= CData.Opt.iLotCnt; i++)
            //{
            //    cmbMgzQty.Items.Add(i);
            //}
            cmbMgzQty.SelectedIndex = 0;

        }

        //CData.LotInfo.sLotName = txt_Lot.Text;
        //                CData.LotInfo.iLotOpenHour = DateTime.Now.Hour; //20200827 lks  // 2020.09.11 JSKim Add
        //                //CData.LotInfo.sToolId     = txt_LToolId.Text       ;  //190410 ksg : ASE secs/gem 용
        //                CData.LotInfo.iTotalMgz = cmb_Cnt.SelectedIndex + 1;
        //                if (txt_StripCount.Text == "") txt_StripCount.Text = "1";
        //                CData.LotInfo.iTotalStrip = Convert.ToInt32(txt_StripCount.Text); //koo : Qorvo Lot Rework
        //                CData.LotInfo.bLotOpen = true;
        //                CData.LotInfo.bLotEnd = false;
        //                CData.LotInfo.iTInCnt = 0;
        //                CData.LotInfo.iTOutCnt = 0;
        //                CData.nLotVerifyCount = 0;          // 2021-01-18, jhLee : SECS/GEM의 Lot Validation Info를 통해 Lot을 Open하며, 중복된 Lot 정보 초기화를 막는다. 

        //                CData.LotInfo.i18PMeasuredCount = 0; //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
        //                CData.bWhlDrsLimitAlarmSkip = false; //200730 jhc : Wheel/Dresser Limit Alaram 보류용 (ASE-Kr VOC) - Lot 초기화 시 Wheel/Dresser Limit Alarm 보류 해제

        //                CData.LotInfo.dtOpen = DateTime.Now; //200116 jym : Lot open 시간 초기화
        //                CData.LotInfo.dtEnd = DateTime.Now; //200116 jym : Lot end 시간 초기화
        //                CData.bLotEndShow = false;
        //                CData.bLotEndMsgShow = false;  //190521 ksg :
        //                CData.SpcInfo.sStartTime = "";
        //                CData.SpcInfo.sEndTime = "";
        //                CData.SpcInfo.sIdleTime = "";
        //                CData.SpcInfo.sJamTime = "";
        //                CData.SpcInfo.sRunTime = "";
        //                CData.SpcInfo.sTotalTime = "";
        //                //CData.swLotTotal.Reset();
        //                CData.bLotTotalReset = true;
        //                //CData.swLotIdle.Reset();
        //                CData.bLotIdleReset = true;
        //                //CData.swLotJam.Reset();
        //                CData.bLotJamReset = true;
        //                //lotinfo 데이터 입력
        //                CData.SpcInfo.sLotName = CData.LotInfo.sLotName;
        //                CData.SpcInfo.sDevice = CData.Dev.sName;
        //                CData.SpcInfo.sWhlSerial_L = CData.Whls[0].sWhlName;
        //                CData.SpcInfo.sWhlSerial_R = CData.Whls[1].sWhlName;
        //                CSq_OfL.It.m_GetQcWorkEnd = false; //190408 ksg : Qc


        // 창을 보여준다.
        public void ShowForm()
        {
            this.Show();

            // 창이 보여질 때 곧바로 내용을 갱신해준다.
            UpdateGridList();
            UpdateLotInfoDisply();

            tmrUpdate.Enabled = true;           // 화면 갱신 시작
        }


        // 창을 닫아준다.
        private void btn_Can_Click(object sender, EventArgs e)
        {
            // DialogResult = DialogResult.Cancel;
            tmrUpdate.Enabled = false;           // 화면 갱신 중지
            this.Hide();
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
            bool ChkOffLPicker  = CIO.It.Get_X(eX.OFFP_Vacuum       );

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

        private void cmb_Cnt_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        // 입력된 새로운 대기열에 LOT를 입력한다.
        private void btnAdd_Click(object sender, EventArgs e)
        {
            string sLotID = txtLotName.Text.Trim();         // 입력된 LOT 이름
            int nStripQty = 0;                              // 투입될 Strip 수량
            int nMgzQty = 0;                                // 투입될 Magazine 수량

            int.TryParse(cmbMgzQty.Text, out nMgzQty);      // 입력값 반영
            int.TryParse(txtStripQty.Text, out nStripQty);

            
            if (sLotID == "") return;                       // 널이면 처리 불가

            // 기존 입력된 LOT Queue에 존재하지 않는 경우에만 추가할 수있다.
            if (CData.LotMgr.IsExistLot(sLotID) == true)
            {
                Debug.WriteLine($"* DlgLotInput Error, Already exist LOT : {sLotID}");
                CMsg.Show(eMsg.Warning, "Warning", $"The LOT name already exists.");
                return; 
            }


            // Magzine 수량이나 Strip 수량을 0으로 설정하였다면 작업자에게 경고를 하고 확인을 받는다.


            if ((nMgzQty <= 0) && (nStripQty <= 0)) // 두 값모두 0이 입력되었다면 확인을 받는다.
            {
                if (CMsg.Show(eMsg.Warning, "Warning", $"The quantity of Magazine and Strip is 0. Do you want to enter LOT information? ?") == DialogResult.Cancel) { return; }
                nMgzQty = 0;            // 만약 음수가 입력될 수 도 있으니 0으로 다시 초기화
                nStripQty = 0;            // 만약 음수가 입력될 수 도 있으니 0으로 다시 초기화
            }
            else
            {
                // 둘 중 하나라도 0값이 입력되면 확인을 받는다.
                if (nMgzQty <= 0)
                {
                    if (CMsg.Show(eMsg.Warning, "Warning", $"The quantity of Magazine is 0. Do you want to enter LOT information? ?") == DialogResult.Cancel) { return; }
                    nMgzQty = 0;            // 만약 음수가 입력될 수 도 있으니 0으로 다시 초기화
                }

                if (nStripQty <= 0)
                {
                    if (CMsg.Show(eMsg.Warning, "Warning", $"The quantity of Strip is 0. Do you want to enter LOT information? ?") == DialogResult.Cancel) { return; }
                    nStripQty = 0;            // 만약 음수가 입력될 수 도 있으니 0으로 다시 초기화
                }
            }


            // 새로운 LOT으로 추가한다.

            CData.LotMgr.AddLot(sLotID, nMgzQty, nStripQty);                  // 지정 LOT ID를 추가한다.
            CData.LotMgr.ListUpdateFlag = true;

            txtLotName.Text = "";                       // 처리된 문자열은 초기화
            cmbMgzQty.SelectedIndex = 0;                // Magazine 수량 초기화
            txtStripQty.Text = "0";                     // Strip 수량 초기화
        }


        // LOT 정보를 보여주는 Grid의 내용을 갱신하여준다.
        public void UpdateLotInfoDisply()
        {
            TLotInfo rInfo; //  = new TLotInfo();

            int i;

            DrawingControl.SuspendDrawing(gridLotList);       // 표시 갱신 중지

            // LOT 대기 Grid에 LogMgr에 등록된 데이터 수 만큼 표시 
            for (i = 0; i < CData.LotMgr.GetCount(); i++)
            {
                rInfo = CData.LotMgr.GetLotInfo(i);            // 지정 순번의 LOT 정보 조회
                if (rInfo == null) continue;

                // LOT의 상태에 따라 색과 상태 정보를 달리 표시해준다.
                if (rInfo.nState == ELotState.eWait)       // 해당 LOT이  대기중이라면
                {
                    gridLotList.Rows[i].Cells[5].Value = "Wait";
                    gridLotList.Rows[i].Cells[1].Style.BackColor = Color.White;
                }
                else
                {
                    if (rInfo.nState == ELotState.eRun)
                    {
                        gridLotList.Rows[i].Cells[5].Value = "Run";       // 진행중
                    }
                    else if (rInfo.nState == ELotState.eClose)
                    {
                        gridLotList.Rows[i].Cells[5].Value = "Close";       // 투입완료
                    }
                    else if (rInfo.nState == ELotState.eComplete)
                    {
                        gridLotList.Rows[i].Cells[5].Value = "End";       // 작업종료
                    }
                    // 2021.08.30 SungTae Start : [추가]
                    else if (rInfo.nState == ELotState.eAbort)
                    {
                        gridLotList.Rows[i].Cells[5].Value = "Abort";       // User에 의한 투입 종료
                    }
                    // 2021.08.30 SungTae End
                    else
                    {
                        gridLotList.Rows[i].Cells[5].Value = "Error";       // 오류발생
                    }

                    // 지정된 색으로 표시해준다.
                    gridLotList.Rows[i].Cells[1].Style.BackColor = CData.LotMgr.GetLotTypeColor(rInfo.nType);
                }

                // 현재 적용되는 LOT 투입 종료 모드표시
                gridLotList.Rows[i].Cells[2].Value = $"{(int)rInfo.nLotEndMode}";

                // 등록시 설정된 Magazine / Strip 수량과 현재 수량을 표시한다.
                //      rLotInfo.iTotalStrip = nStripQty;       // 투입될 Strip 수량 설정
                //      rLotInfo.iTotalMgz = nMgzQty;           // 투입될 Magzine 수량 설정
                //      rLotInfo.nInMgzCnt = 0;                 // 투입된 Magzine 수량 초기화
                //      rLotInfo.iTInCnt = 0;                   // 투입된 Strip 수량 초기화
                //
                gridLotList.Rows[i].Cells[3].Value = (rInfo.iTotalMgz > 0) ? $"{rInfo.nInMgzCnt}/{rInfo.iTotalMgz}" : $"{rInfo.nInMgzCnt}";
                gridLotList.Rows[i].Cells[4].Value = (rInfo.iTotalStrip > 0) ? $"{rInfo.iTInCnt}/{rInfo.iTotalStrip}" : $"{rInfo.iTInCnt}";


                // Strip In/Out 수량 표시
                gridLotList.Rows[i].Cells[6].Value = $"{rInfo.iTInCnt}";
                gridLotList.Rows[i].Cells[7].Value = $"{rInfo.iTOutCnt}";
            }

            DrawingControl.ResumeDrawing(gridLotList);          // grid 표시 재개

        }//of UpdateLotInfoDisply


        // LOT 정보를 표시해주는 그리드의 항목 구성을 새로이 변경해준다.
        private void UpdateGridList()
        {
            if (gridLotList.Rows.Count == CData.LotMgr.GetCount()) return;          // Lot Mgr에 등록된 수량에 변화가 없다면 수정할 내용 없음

            TLotInfo pInfo;

            gridLotList.SuspendLayout();
            DrawingControl.SuspendDrawing(gridLotList);       // 표시 갱신 중지

            gridLotList.Rows.Clear();       // 기존 항목을 모두 삭제해 준다.

            for (int i = 0; i < CData.LotMgr.GetCount(); i++)
            {
                pInfo = CData.LotMgr.GetLotInfo(i);

                if (pInfo != null)
                {
                    gridLotList.Rows.Add(i + 1, pInfo.sLotName, "", "", "");
                }
                
            }

            gridLotList.ResumeLayout();
            DrawingControl.ResumeDrawing(gridLotList);
        }


        private void CDlgLotInput_Load(object sender, EventArgs e)
        {

        }

        private void CDlgLotInput_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;

            this.Hide();
        }


        // 일정 주기마다 데이터 내용을 갱신하여준다.
        private void tmrUpdate_Tick(object sender, EventArgs e)
        {
            // List 구성 요소가 변경되었다면 Grid는 새로이 그려준다.
            if ( CData.LotMgr.ListUpdateFlag )
            {
                UpdateGridList();
            }

            // List 항목 내용이 변경되었다면 내용을 갱신해준다.
            if (CData.LotMgr.DataUpdateFlag || CData.LotMgr.ListUpdateFlag)
            {
                CData.LotMgr.DataUpdateFlag = false;
                CData.LotMgr.ListUpdateFlag = false;

                UpdateLotInfoDisply();
            }

            // 2022.04.29 SungTae Start : [수정] ASE-KH Multi-LOT(Auto-In/Out) 관련 Site Option 추가
            // 2021.08.19 SungTae Start : [추가] Multi-LOT 관련
            //if (CData.CurCompany == ECompany.ASE_KR)
            if (CData.CurCompany == ECompany.ASE_KR || CData.CurCompany == ECompany.ASE_K12)
            // 2022.04.29 SungTae End
            {
                // On-line Remote Mode일 경우엔 비활성화
                if (CData.Opt.bSecsUse && CData.SecsGem_Data.nRemote_Flag == Convert.ToInt32(SECSGEM.JSCK.eCont.Remote))
                {
                    lbl_Lot.Visible         = false;
                    lbl_Cnt.Visible         = false;
                    lbl_StripCount.Visible  = false;

                    txtLotName.Visible      = false;
                    cmbMgzQty.Visible       = false;
                    txtStripQty.Visible     = false;
                    btnAdd.Visible          = false;
                    btnComplete.Visible     = false;
                }
                else // Off-line Mode 또는 On-line Local Mode일 경우에만 활성화
                {
                    lbl_Lot.Visible         = true;
                    lbl_Cnt.Visible         = true;
                    lbl_StripCount.Visible  = true;

                    txtLotName.Visible      = true;
                    cmbMgzQty.Visible       = true;
                    txtStripQty.Visible     = true;
                    btnAdd.Visible          = true;
                    btnComplete.Visible     = true;
                }
            }
            // 2021.08.19 SungTae End
        }


        // 지정 LOT 정보를 삭제한다.
        private void btnDelete_Click(object sender, EventArgs e)
        {
            int nRow = -1;

            try
            {
                nRow = gridLotList.SelectedRows[0].Index;
            }
            catch (Exception ex)
            {
                // 지정이 되지 않았다.               
            }

            if (nRow >= 0)
            {
                string sLotID = gridLotList.Rows[nRow].Cells[1].Value.ToString();

                // 현재 Wait 상태가 아닌 경우에는 삭제할 수 없다.
                //
                ELotState nState = CData.LotMgr.GetLotState(sLotID);            // 지정 LOT의 State 조회
                if (nState != ELotState.eWait)
                {
                    // 엔지니어 Level 미만은 Wait 상태가 아닌 LOT 정보를 삭제할 수 없다.
                    if ( CData.Lev < ELv.Engineer)
                    {
                        CMsg.Show(eMsg.Notice, "Information", "Can not delete this LOT Information : Not WAIT State, Need Engineer Level");
                        return;
                    }


                    // 지정 LOT에 속하는 Strip이 설비내에 존재하는가 ?
                    int nCount = CData.LotMgr.GetLOTStripCount(sLotID);
                    if (nCount > 0)
                    {
                        CMsg.Show(eMsg.Notice, "Information", $"Can not delete this LOT Information : Strip Count is {nCount}");
                        return;
                    }
                }

                if (CMsg.Show(eMsg.Warning, "Warning", $"Do you want LOT Remove ? \n\n LOT Name : [{sLotID}]") == DialogResult.Cancel) { return; }


                // 만약 On-Loader와 Off-Loader에 Magazine을 들고 있는 상황이라면 내려놓도록 지령해야한다.
                //
                CData.LotMgr.CheckMgzPlace(sLotID);         // 삭제된 LOT에 해당되는 Magazine을 잡고있다면 내려놓는다.
                CData.LotMgr.DeleteLotInfo(sLotID);         // 지정 LOT 정보를 삭제한다.
                CData.LotMgr.ListUpdateFlag = true;         // 항목 변경
            }
        }


        // 각종 생산 데이터를 Clear 한다.
        // 특히 투입/배출 누적량 초기화
        private void btnClearInform_Click(object sender, EventArgs e)
        {
            // 설비가 가동중이 아닐경우에만 수행 가능

            //ksg 추가
            if ((CSQ_Main.It.m_iStat == EStatus.Auto_Running) || (CSQ_Main.It.m_iStat == EStatus.Manual) )
            {
                CMsg.Show(eMsg.Notice, "Information", $"Can not clear data : System is not STOP state");
                return;
            }

            // LOT 정보가 없는 경우에만 수행 가능
            if (CData.LotMgr.GetCount() > 0)
            {
                CMsg.Show(eMsg.Notice, "Information", $"Can not clear data : LOT Information exist");
                return;
            }


            if (CMsg.Show(eMsg.Warning, "Warning", $"Do you want Clear Product data ?") == DialogResult.Cancel) { return; }

            // 각종 생산정보를 초기화 한다.
            CData.LotMgr.ClearLotInfo();
        }



        /*
         * 
         * 
         *         // == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == == ==

                // 2021-05-03, jhLee : Multi-LOT 운영을 위한 LOT 정보 관리

                // 1. 새로운 LOT ID를 입력한다.
                private void btnLotAdd_Click(object sender, EventArgs e)
                {
                    string sLotID = txtLotID.Text;


                    // 기존에 존재하지 않는 경우에 추가할 수있다.
                    if (CData.LotMgr.IsExistLot(sLotID) == false)
                    {
                        gridLotList.Rows.Add(gridLotList.RowCount, sLotID, "", "");
                        CData.LotMgr.AddLot(sLotID, 0, 0);                  // 지정 LOT ID를 추가한다.
                    }
                    else
                        Debug.WriteLine($"* Error, Already exist LOT : {sLotID}");

                }


                // 2. 지정 LOT ID의 이름을 변경한다.
                private void btnLotRename_Click(object sender, EventArgs e)
                {
                    int nRow = -1;
                    try
                    {
                        nRow = gridLotList.SelectedRows[0].Index;
                    }
                    catch (Exception ex)
                    {
                        // 지정이 되지 않았다.               
                    }

                    if ( nRow >= 0 )
                    {
                        gridLotList.Rows[nRow].Cells[1].Value = txtLotID.Text;

                    }

                }

                // 3. 지정 LOT ID를 삭제한다.
                private void btnLotDelete_Click(object sender, EventArgs e)
                {
                    int nRow = -1;
                    try
                    {
                        nRow = gridLotList.SelectedRows[0].Index;
                    }
                    catch (Exception ex)
                    {
                        // 지정이 되지 않았다.               
                    }

                    if (nRow >= 0)
                    {
                        string sLotID = gridLotList.Rows[nRow].Cells[1].Value.ToString();
                        CData.LotMgr.DeleteLotInfo(sLotID);                  // 지정 LOT ID를 추가한다.

                        gridLotList.Rows.RemoveAt(nRow);            // 지정 Index의 Row를 삭제한다.
                    }
                }

                // 4. 전체 LOT 를 삭제한다. (아직 투입되지 않은 항목만)
                private void btnLotClear_Click(object sender, EventArgs e)
                {
                    gridLotList.Rows.Clear();
                    CData.LotMgr.Clear();                  // 모든 LOT 정보 삭제
                }


         * 
         * 
         * 
        */

        //   private void timer_DfServer_Tick(object sender, EventArgs e)
        //   {
        //       //20190618 ghk_dfserver
        //       if(m_bLotClick)
        //       {

        //           if(CDf.It.ReciveAck((int)ECMD.scLotOpen))
        //           {
        //               if (CData.dfInfo.sGl == "GL1")
        //               {
        //                   CData.LotInfo.sLotName   = txt_Lot    .Text;
        //                   CData.LotInfo.sToolId    = txt_LToolId.Text;
        //                   CData.LotInfo.iTotalMgz  = cmb_Cnt.SelectedIndex + 1;
        //                   CData.LotInfo.bLotOpen   = true;
        //                   CData.LotInfo.bLotEnd    = false;
        //                   CData.LotInfo.iTInCnt    = 0;
        //                   CData.LotInfo.iTOutCnt   = 0;

        //                   CData.LotInfo.i18PMeasuredCount = 0; //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
        //                   CData.bWhlDrsLimitAlarmSkip = false; //200730 jhc : Wheel/Dresser Limit Alaram 보류용 (ASE-Kr VOC) - Lot 초기화 시 Wheel/Dresser Limit Alarm 보류 해제

        //                   CData.LotInfo.dtOpen = DateTime.Now; //200116 jym : Lot open 시간 초기화
        //                   CData.LotInfo.dtEnd = DateTime.Now; //200116 jym : Lot end 시간 초기화
        //                   CData.bLotEndShow        = false;
        //                   CData.bLotEndMsgShow     = false;
        //                   CData.SpcInfo.sStartTime = "";
        //                   CData.SpcInfo.sEndTime   = "";
        //                   CData.SpcInfo.sIdleTime  = "";
        //                   CData.SpcInfo.sJamTime   = "";
        //                   CData.SpcInfo.sRunTime   = "";
        //                   CData.SpcInfo.sTotalTime = "";
        //                   CData.bLotTotalReset     = true;
        //                   CData.bLotIdleReset      = true;
        //                   CData.bLotJamReset       = true;
        //                   //lotinfo 데이터 입력
        //                   CData.SpcInfo.sLotName     = CData.LotInfo.sLotName;
        //                   CData.SpcInfo.sDevice      = CData.Dev.sName;
        //                   CData.SpcInfo.sWhlSerial_L = CData.Whls[0].sWhlName;
        //                   CData.SpcInfo.sWhlSerial_R = CData.Whls[1].sWhlName;
        //                   CSq_OfL.It.m_GetQcWorkEnd  = false;

        //	// 2021-01-?? YYY : for New QC-Vision
        //                   //CTcpIp.It.bResultGood      = false;
        //                   //CTcpIp.It.bResultFail      = false;
        //                   CQcVisionCom.rcvQCReadyQueryGOOD = CQcVisionCom.rcvQCReadyQueryNG = false;

        //                   CSQ_Main.It.m_bPause       = false;

        //                   m_bLotClick = false;
        //                   timer_DfServer.Enabled = false;
        //                   DialogResult = DialogResult.OK;                        
        //               }
        //               else
        //               {
        //                   CDf.It.SendLotExist();
        //                   m_bLotClick = false;
        //                   m_bLotOp = true;
        //                   return;
        //               }
        //           }
        //       }
        //       if(m_bLotOp)
        //       {
        //           if(CDf.It.ReciveAck((int)ECMD.scLotExist))
        //           {
        //               CData.LotInfo.sLotName     = txt_Lot.Text;
        //               CData.LotInfo.sToolId      = txt_LToolId.Text;
        //               CData.LotInfo.iTotalMgz    = cmb_Cnt.SelectedIndex + 1;
        //               CData.LotInfo.bLotOpen     = true;
        //               CData.LotInfo.bLotEnd      = false;
        //               CData.LotInfo.iTInCnt      = 0;
        //               CData.LotInfo.iTOutCnt     = 0;

        //               CData.LotInfo.i18PMeasuredCount = 0; //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
        //               CData.bWhlDrsLimitAlarmSkip = false; //200730 jhc : Wheel/Dresser Limit Alaram 보류용 (ASE-Kr VOC) - Lot 초기화 시 Wheel/Dresser Limit Alarm 보류 해제

        //               CData.LotInfo.dtOpen = DateTime.Now; //200116 jym : Lot open 시간 초기화
        //               CData.LotInfo.dtEnd = DateTime.Now; //200116 jym : Lot end 시간 초기화
        //               CData.bLotEndShow          = false;
        //               CData.bLotEndMsgShow       = false;
        //               CData.SpcInfo.sStartTime   = "";
        //               CData.SpcInfo.sEndTime     = "";
        //               CData.SpcInfo.sIdleTime    = "";
        //               CData.SpcInfo.sJamTime     = "";
        //               CData.SpcInfo.sRunTime     = "";
        //               CData.SpcInfo.sTotalTime   = "";
        //               CData.bLotTotalReset       = true;
        //               CData.bLotIdleReset        = true;
        //               CData.bLotJamReset         = true;
        //               //lotinfo 데이터 입력
        //               CData.SpcInfo.sLotName     = CData.LotInfo.sLotName;
        //               CData.SpcInfo.sDevice      = CData.Dev.sName;
        //               CData.SpcInfo.sWhlSerial_L = CData.Whls[0].sWhlName;
        //               CData.SpcInfo.sWhlSerial_R = CData.Whls[1].sWhlName;
        //               CSq_OfL.It.m_GetQcWorkEnd  = false;

        //// 2021-01-?? YYY : for New QC-Vision
        //               //CTcpIp.It.bResultGood      = false;
        //               //CTcpIp.It.bResultFail      = false;
        //               CQcVisionCom.rcvQCReadyQueryGOOD = CQcVisionCom.rcvQCReadyQueryNG = false; //

        //               CSQ_Main.It.m_bPause       = false;

        //               m_bLotOp               = false;
        //               timer_DfServer.Enabled = false;
        //               DialogResult = DialogResult.OK;
        //           }
        //           else
        //           {
        //               m_bLotOp = false;
        //               timer_DfServer.Enabled = false;
        //               CMsg.Show(eMsg.Error, "Error", "No GL1 file on sever!");
        //           }
        //       }
        //   }



    }




    public static class DrawingControl
    {
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);

        private const int WM_SETREDRAW = 11;

        /// <summary>
        /// Some controls, such as the DataGridView, do not allow setting the DoubleBuffered property.
        /// It is set as a protected property. This method is a work-around to allow setting it.
        /// Call this in the constructor just after InitializeComponent().
        /// </summary>
        /// <param name="control">The Control on which to set DoubleBuffered to true.</param>
        public static void SetDoubleBuffered(Control control)
        {
            // if not remote desktop session then enable double-buffering optimization
            if (!System.Windows.Forms.SystemInformation.TerminalServerSession)
            {

                // set instance non-public property with name "DoubleBuffered" to true
                typeof(Control).InvokeMember("DoubleBuffered",
                                             System.Reflection.BindingFlags.SetProperty |
                                                System.Reflection.BindingFlags.Instance |
                                                System.Reflection.BindingFlags.NonPublic,
                                             null,
                                             control,
                                             new object[] { true });
            }
        }

        /// <summary>
        /// Suspend drawing updates for the specified control. After the control has been updated
        /// call DrawingControl.ResumeDrawing(Control control).
        /// </summary>
        /// <param name="control">The control to suspend draw updates on.</param>
        public static void SuspendDrawing(Control control)
        {
            SendMessage(control.Handle, WM_SETREDRAW, false, 0);
        }

        /// <summary>
        /// Resume drawing updates for the specified control.
        /// </summary>
        /// <param name="control">The control to resume draw updates on.</param>
        public static void ResumeDrawing(Control control)
        {
            SendMessage(control.Handle, WM_SETREDRAW, true, 0);
            control.Refresh();
        }
    }

}
