using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace SG2000X
{
    public partial class vwEqu_08Bcr : UserControl
    {
        /// <summary>
        /// 화면에 정보 업데이트 타이머
        /// </summary>
        private Timer m_tmUpdt;

        private bool m_bBcrRead = false;
        public vwEqu_08Bcr()
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

            m_tmUpdt = new Timer();
            m_tmUpdt.Interval = 50;
            m_tmUpdt.Tick += _M_tmUpdt_Tick;

#if true //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)
            _setRfidOptionVisible();
#else
            //if (CData.CurCompany != eCompany.JSCK)
            if (CDataOption.OflRfid == eOflRFID.NotUse) //200121 ksg :
            {
                btn_TopRfid.Visible = false;
                btn_BtmRfid.Visible = false;
                lbl_Rfid   .Visible = false;
            }
            else
            {
                btn_TopRfid.Visible = true;
                btn_BtmRfid.Visible = true;
                lbl_Rfid   .Visible = true;
            }
            //200121 ksg :
            if(CDataOption.OnlRfid == eOnlRFID.NotUse)
            {
                btn_OnlRfid.Visible = false;
                lbl_OnlRfid.Visible = false;
            }
            else
            {
                btn_OnlRfid.Visible = false;
                lbl_OnlRfid.Visible = false;
            }
#endif

            //191029 ksg :
            if(CData.CurCompany != ECompany.SkyWorks)
            {
                lbl_Ocr.Visible      = false;
                lbl_OcrTitle.Visible = false;
            }
            else
            {
                lbl_Ocr.Visible      = true;
                lbl_OcrTitle.Visible = true;
            }

#if false //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)
            //200120 ksg:
            if(CDataOption.OnlRfid == eOnlRFID.Use)
            {
                btn_OnlRfid.Visible = true;
                lbl_OnlRfid.Visible = true;
            }
            else
            {
                btn_OnlRfid.Visible = false;
                lbl_OnlRfid.Visible = false;
            }
#endif

            //20200311 jhc : 2D Vision
            if(CDataOption.BcrInterface == eBcrInterface.Udp)
            {
                btn_Ori.Visible         = true;
                lbl_OriTitle.Visible    = true;
                lbl_Ori.Visible         = true;
            }
            else
            {
                btn_Ori.Visible         = false;
                lbl_OriTitle.Visible    = false;
                lbl_Ori.Visible         = false;
            }
            //
            //20200319 jhc : 2D Vision 잠시 테스트 용 - 출하전 삭제(감출) 예정
            if (CDataOption.BcrInterface == eBcrInterface.Udp){
                btn_VisAuto.Visible         = true;
                btn_VisUiAuto.Visible       = true;
                btn_VisUiTrain.Visible      = true;
                lbl_VisProcessTitle.Visible = true;
                lbl_VisProcess.Visible      = true;
                lbl_VisConnTitle.Visible    = true;
                lbl_VisConn.Visible         = true;
                lbl_VisULevelTitle.Visible  = true;
                lbl_VisULevel.Visible       = true;
                lbl_VisUIModeTitle.Visible  = true;
                lbl_VisUIMode.Visible       = true;
                lbl_VisStatusTitle.Visible  = true;
                lbl_VisStatus.Visible       = true;

                // 2020-10-13, jhLee 한솔루션 테스트용
                btn_BCROnly.Visible         = true;
                btn_OCROnly.Visible         = true;
                btn_BCROCR.Visible          = true;
            }
            else
            {
                btn_VisAuto.Visible         = false;
                btn_VisUiAuto.Visible       = false;
                btn_VisUiTrain.Visible      = false;
                lbl_VisProcessTitle.Visible = false;
                lbl_VisProcess.Visible      = false;
                lbl_VisConnTitle.Visible    = false;
                lbl_VisConn.Visible         = false;
                lbl_VisULevelTitle.Visible  = false;
                lbl_VisULevel.Visible       = false;
                lbl_VisUIModeTitle.Visible  = false;
                lbl_VisUIMode.Visible       = false;
                lbl_VisStatusTitle.Visible  = false;
                lbl_VisStatus.Visible       = false;

                // 2020-10-13, jhLee 한솔루션 테스트용
                btn_BCROnly.Visible         = false;
                btn_OCROnly.Visible         = false;
                btn_BCROCR.Visible          = false;
            }
            //
        }

        #region Private method
        private void _M_tmUpdt_Tick(object sender, EventArgs e)
        {
            //190418 ksg :
            if (CRfid.It.ReciveMsg != "")
            {
                lbl_Rfid.Text = CRfid.It.ReciveMsg;
            }
            if(m_bBcrRead)
            {
                CBcr.It.Load();
                if(!CData.Bcr.bStatusBcr)
                {
                    m_bBcrRead = false;
                    lbl_Bcr.Text = CData.Bcr.sBcr;
                    lbl_Ocr.Text = CData.Bcr.sOcr;
                    //20200311 jhc : 2D Vision
                    lbl_Ori.Text = CData.Bcr.bRetOri.ToString();
                    //
                }
            }
            //200120 ksg :
            if(CRfid.It.ReciveMsgOnl != "")
            {
                lbl_OnlRfid.Text = CRfid.It.ReciveMsgOnl;
            }
            else lbl_OnlRfid.Text = "";

            //20200318 jhc : 2D Vision 잠시 테스트 용 - 출하전 삭제(감출) 예정
            if(CDataOption.BcrInterface == eBcrInterface.Udp){
                lbl_VisProcess.Text  = CData.Bcr.bRun.ToString();
                lbl_VisConn.Text     = CData.Bcr.bCon.ToString();
                lbl_VisULevel.Text   = CData.Bcr.iUserLevel.ToString();
                lbl_VisUIMode.Text   = CData.Bcr.iUIMode.ToString();
                lbl_VisStatus.Text   = CBcr.It.eBcrCommStatus.ToString();
            }
            //

            //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)
            //201012 jhc : (KEYENCE BCR)/(RFID) 옵션으로 구분 적용
            if (CDataOption.RFID == ERfidType.Keyence && CDataOption.OnlRfid == eOnlRFID.Use)
            //if (CData.CurCompany == ECompany.SPIL && CDataOption.OnlRfid == eOnlRFID.Use)
            { _UpdateKeyenceBcrState(EKeyenceBcrPos.OnLd); }
            //201012 jhc : (KEYENCE BCR)/(RFID) 옵션으로 구분 적용
            if (CDataOption.RFID == ERfidType.Keyence && CDataOption.OflRfid == eOflRFID.Use)
            //if (CData.CurCompany == ECompany.SPIL && CDataOption.OflRfid == eOflRFID.Use)
            { _UpdateKeyenceBcrState(EKeyenceBcrPos.OffLd); }
            //
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
            _Set();

            //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)
            //201012 jhc : (KEYENCE BCR)/(RFID) 옵션으로 구분 적용
            if ((CDataOption.RFID == ERfidType.Keyence) && (CDataOption.OnlRfid == eOnlRFID.Use || CDataOption.OflRfid == eOflRFID.Use))
            //if ((CData.CurCompany == ECompany.SPIL) && (CDataOption.OnlRfid == eOnlRFID.Use || CDataOption.OflRfid == eOflRFID.Use))
            { _SetInitKeyenceBcrState(); }
            //

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
        #endregion

        private void btn_Bcr_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CData.bLastClick = true; //190509 ksg :
            string sTemp = "";
            //sTemp = CBcr.It.Chk_Bcr();
            if(CDataOption.BcrPro == eBcrProtocol.Old) sTemp = CBcr.It.Chk_Bcr    ();
            else                                               CBcr.It.Chk_Bcr_InR();
            lbl_Bcr.Text = sTemp;
            lbl_Ocr.Text = CData.Bcr.sOcr;
            GU.Delay(1000);
            m_bBcrRead = true;
        }

        private void btn_TopRfid_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CData.bLastClick = true; //190509 ksg :
            CRfid.It.ReciveMsg = "";
            lbl_Rfid.Text = "";

            CRfid.It.ReadRfid(false);
            //lbl_Rfid.Text = CRfid.It.ReciveMsg;
        }

        private void btn_BtmRfid_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CData.bLastClick = true; //190509 ksg :
            CRfid.It.ReciveMsg = "";
            lbl_Rfid.Text = "";
            CRfid.It.ReadRfid(true);

            //lbl_Rfid.Text = CRfid.It.ReciveMsg;
        }

        private void btn_OnlRfid_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CData.bLastClick = true; //190509 ksg :
            CRfid.It.ReciveMsgOnl = "";
            lbl_OnlRfid.Text = "";
            CRfid.It.ReadRfidOnl(true);
        }

        //20200311 jhc : 2D Vision
        private void btn_Ori_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CData.bLastClick = true; //190509 ksg :
            string sTemp = "";
            if(CDataOption.BcrPro == eBcrProtocol.Old) CBcr.It.Chk_Ori    ();
            else                                       CBcr.It.Chk_Ori_InR();
            lbl_Ori.Text = sTemp;
            GU.Delay(1000);
            m_bBcrRead = true;
        }

        private void btn_VisAuto_Click(object sender, EventArgs e)
        {
            CBcr.It.UdpRequest(eBcrCommand.ChangeAutoMode, "");
        }

        private void btn_VisUiAuto_Click(object sender, EventArgs e)
        {
            CBcr.It.UdpRequest(eBcrCommand.ChangeUI, "0");
        }

        private void btn_VisUiTrain_Click(object sender, EventArgs e)
        {
            CBcr.It.UdpRequest(eBcrCommand.ChangeUI, "1");
        }


        //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)
        private void btn_OnL_BcrConnect_Click(object sender, EventArgs e)
        {
            btn_OnL_BcrConnect.Enabled = false;
            btn_OnL_BcrConnect.Refresh();
            if (CKeyenceBcr.It.IsConnected(EKeyenceBcrPos.OnLd))
            {
                CKeyenceBcr.It.Disconnect(EKeyenceBcrPos.OnLd);
            }
            else
            {
                CKeyenceBcr.It.Connect(EKeyenceBcrPos.OnLd);
            }
            btn_OnL_BcrConnect.Enabled = true;
            btn_OnL_BcrConnect.Refresh();
        }

        private void btn_OnL_LON_Click(object sender, EventArgs e)
        {
            CKeyenceBcr.It.SendCommand(EKeyenceBcrPos.OnLd, eKeyenceBcrCmd.LON);
        }

        private void btn_OnL_LOFF_Click(object sender, EventArgs e)
        {
            CKeyenceBcr.It.SendCommand(EKeyenceBcrPos.OnLd, eKeyenceBcrCmd.LOFF);
        }

        private void btn_OnL_BCLR_Click(object sender, EventArgs e)
        {
            CKeyenceBcr.It.SendCommand(EKeyenceBcrPos.OnLd, eKeyenceBcrCmd.BCLR);
        }

        private void btn_OffL_BcrConnect_Click(object sender, EventArgs e)
        {
            btn_OffL_BcrConnect.Enabled = false;
            btn_OffL_BcrConnect.Refresh();
            if (CKeyenceBcr.It.IsConnected(EKeyenceBcrPos.OffLd))
            {
                CKeyenceBcr.It.Disconnect(EKeyenceBcrPos.OffLd);
            }
            else
            {
                CKeyenceBcr.It.Connect(EKeyenceBcrPos.OffLd);
            }
            btn_OffL_BcrConnect.Enabled = true;
            btn_OffL_BcrConnect.Refresh();
        }

        private void btn_OffL_LON_Click(object sender, EventArgs e)
        {
            CKeyenceBcr.It.SendCommand(EKeyenceBcrPos.OffLd, eKeyenceBcrCmd.LON);
        }

        private void btn_OffL_LOFF_Click(object sender, EventArgs e)
        {
            CKeyenceBcr.It.SendCommand(EKeyenceBcrPos.OffLd, eKeyenceBcrCmd.LOFF);
        }

        private void btn_OffL_BCLR_Click(object sender, EventArgs e)
        {
            CKeyenceBcr.It.SendCommand(EKeyenceBcrPos.OffLd, eKeyenceBcrCmd.BCLR);
        }

        private void _SetInitKeyenceBcrState()
        {
            //201012 jhc : (KEYENCE BCR)/(RFID) 옵션으로 구분 적용
            if (CDataOption.RFID != ERfidType.Keyence)
            //if (CData.CurCompany != ECompany.SPIL)
            { return; }
            
            //OnLoader BCR
            if (CDataOption.OnlRfid == eOnlRFID.Use)
            {
                if (CKeyenceBcr.It.IsConnected(EKeyenceBcrPos.OnLd))
                { btn_OnL_BcrConnect.Text = "DISCONNECT"; }
                else
                { btn_OnL_BcrConnect.Text = "CONNECT"; }

                lbl_OnL_BcrTcp.Text = CKeyenceBcr.It.sReaderIP[(int)EKeyenceBcrPos.OnLd] + ":" + CKeyenceBcr.It.iBcrPort.ToString();

                if (CKeyenceBcr.It.IsConnected(EKeyenceBcrPos.OnLd))
                { lbl_OnL_BcrTcp.BackColor = Color.ForestGreen; }
                else
                { lbl_OnL_BcrTcp.BackColor = Color.FloralWhite; }
            }
            //OffLoader BCR
            if (CDataOption.OflRfid == eOflRFID.Use)
            {
                if (CKeyenceBcr.It.IsConnected(EKeyenceBcrPos.OffLd))
                { btn_OffL_BcrConnect.Text = "DISCONNECT"; }
                else
                { btn_OffL_BcrConnect.Text = "CONNECT"; }

                lbl_OffL_BcrTcp.Text = CKeyenceBcr.It.sReaderIP[(int)EKeyenceBcrPos.OffLd] + ":" + CKeyenceBcr.It.iBcrPort.ToString();

                if (CKeyenceBcr.It.IsConnected(EKeyenceBcrPos.OffLd))
                { lbl_OffL_BcrTcp.BackColor = Color.ForestGreen; }
                else
                { lbl_OffL_BcrTcp.BackColor = Color.FloralWhite; }
            }
        }

        private void _UpdateKeyenceBcrState(EKeyenceBcrPos ePos)
        {
            bool bConnected = CKeyenceBcr.It.IsConnected(ePos);

            string sBtnConnText = "";
            if (ePos == EKeyenceBcrPos.OnLd)
            { sBtnConnText = btn_OnL_BcrConnect.Text; }
            else if (ePos == EKeyenceBcrPos.OffLd)
            { sBtnConnText = btn_OffL_BcrConnect.Text; }
                           
            if (bConnected && sBtnConnText.Equals("CONNECT"))
            {
                if (ePos == EKeyenceBcrPos.OnLd)
                {//OnLoader BCR
                    btn_OnL_BcrConnect.Text = "DISCONNECT";
                    btn_OnL_BcrConnect.Refresh();
                    lbl_OnL_BcrTcp.BackColor = Color.ForestGreen;
                    lbl_OnL_BcrTcp.Refresh();
                }
                else if (ePos == EKeyenceBcrPos.OffLd)
                {//OffLoader BCR
                    btn_OffL_BcrConnect.Text = "DISCONNECT";
                    btn_OffL_BcrConnect.Refresh();
                    lbl_OffL_BcrTcp.BackColor = Color.ForestGreen;
                    lbl_OffL_BcrTcp.Refresh();
                }
            }
            else if (!bConnected && sBtnConnText.Equals("DISCONNECT"))
            {
                if (ePos == EKeyenceBcrPos.OnLd)
                {//OnLoader BCR
                    btn_OnL_BcrConnect.Text = "CONNECT";
                    btn_OnL_BcrConnect.Refresh();
                    lbl_OnL_BcrTcp.BackColor = Color.FloralWhite;
                    lbl_OnL_BcrTcp.Refresh();
                }
                else if (ePos == EKeyenceBcrPos.OffLd)
                {//OffLoader BCR
                    btn_OffL_BcrConnect.Text = "CONNECT";
                    btn_OffL_BcrConnect.Refresh();
                    lbl_OffL_BcrTcp.BackColor = Color.FloralWhite;
                    lbl_OffL_BcrTcp.Refresh();
                }
            }

            if (ePos == EKeyenceBcrPos.OnLd)
            {
                lbl_OnL_BcrBcr.Text         = CData.KeyBcr[(int)ePos].sBcr;
                lbl_OnL_BcrState.Text       = CData.KeyBcr[(int)ePos].eState.ToString();
                lbl_OnL_BcrCmd.Text         = CData.KeyBcr[(int)ePos].sCmd;
                lbl_OnL_BcrCmdResult.Text   = CData.KeyBcr[(int)ePos].sCmdResult;
                lbl_OnL_BcrErr.Text         = CData.KeyBcr[(int)ePos].sErrCode;
            }
            else if (ePos == EKeyenceBcrPos.OffLd)
            {
                lbl_OffL_BcrBcr.Text        = CData.KeyBcr[(int)ePos].sBcr;
                lbl_OffL_BcrState.Text      = CData.KeyBcr[(int)ePos].eState.ToString();
                lbl_OffL_BcrCmd.Text        = CData.KeyBcr[(int)ePos].sCmd;
                lbl_OffL_BcrCmdResult.Text  = CData.KeyBcr[(int)ePos].sCmdResult;
                lbl_OffL_BcrErr.Text        = CData.KeyBcr[(int)ePos].sErrCode;
            }
        }

        //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)
        private void _setRfidOptionVisible()
        {
            //201012 jhc : (KEYENCE BCR)/(RFID) 옵션으로 구분 적용
            if (CDataOption.RFID == ERfidType.Keyence)
            //if (CData.CurCompany == ECompany.SPIL)
            {
                //RFID : 항상 감춤
                btn_OnlRfid.Visible = false;
                lbl_OnlRfid.Visible = false;

                btn_TopRfid.Visible = false;
                btn_BtmRfid.Visible = false;
                lbl_Rfid   .Visible = false;
                
                //KEYENCE BCR : 조건별 버튼 표시/감춤
                if (CDataOption.OnlRfid == eOnlRFID.Use)
                { pnl_OnL_Bcr.Visible = true; }
                else
                { pnl_OnL_Bcr.Visible = false; }
                if (CDataOption.OflRfid == eOflRFID.Use)
                { pnl_OffL_Bcr.Visible = true; }
                else
                { pnl_OffL_Bcr.Visible = false; }
            }
            else
            {
                //RFID : 조건별 버튼 표시/감춤
                if (CDataOption.OnlRfid == eOnlRFID.Use)
                {
                    btn_OnlRfid.Visible = true;
                    lbl_OnlRfid.Visible = true;
                }
                else
                {
                    btn_OnlRfid.Visible = false;
                    lbl_OnlRfid.Visible = false;
                }

                if (CDataOption.OflRfid == eOflRFID.Use)
                {
                    btn_TopRfid.Visible = true;
                    btn_BtmRfid.Visible = true;
                    lbl_Rfid   .Visible = true;
                }
                else
                {
                    btn_TopRfid.Visible = false;
                    btn_BtmRfid.Visible = false;
                    lbl_Rfid   .Visible = false;
                }

                //KEYENCE BCR : 항상 감춤
                pnl_OnL_Bcr.Visible = false;
                pnl_OffL_Bcr.Visible = false;
            }
        }

        // 2020-10-13 jhLee : 한솔루션 신규 2D Vision program을 테스트하기 위한 개별 Read 명령
        private void btn_BCROnly_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CData.bLastClick = true;

            lbl_Bcr.Text = "";
            lbl_Ocr.Text = "";
            // Request Command [2D Read : In-Rail]
            int nRet = CBcr.It.UdpRequest(eBcrCommand.BcrInr, "");

            GU.Delay(2000);
            m_bBcrRead = true;
        }

        // OCR을 읽는다.
        private void btn_OCROnly_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CData.bLastClick = true;

            lbl_Bcr.Text = "";
            lbl_Ocr.Text = "";

            // Request Command : OCR Read
            int nRet = CBcr.It.UdpRequest(eBcrCommand.OcrInr, "");

            GU.Delay(2000);
            m_bBcrRead = true;
        }

        // BCR과 OCR을 동시에 읽는다.
        private void btn_BCROCR_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CData.bLastClick = true;

            lbl_Bcr.Text = "";
            lbl_Ocr.Text = "";

            int nRet = CBcr.It.UdpRequest(eBcrCommand.BcrOcrInr, "");

            GU.Delay(2000);
            m_bBcrRead = true;
        }
        //
    }
}
