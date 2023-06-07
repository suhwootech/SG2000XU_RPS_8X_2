using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace SG2000X
{
    public partial class vwOpt_02Ldr : UserControl
    {
        public vwOpt_02Ldr()
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

            //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)
            if (CDataOption.RFID == ERfidType.Keyence) //201012 jhc : (KEYENCE BCR)/(RFID) 옵션으로 구분 적용 //if (CData.CurCompany == ECompany.SPIL)
            {
                lbl_RFID_T.Text = "MGZ Barcode Setting";
                ckb_OnlRfidUse.Text = "On loader MGZ barcode skip";
                ckb_OflRfidUse.Text = "Off loader MGZ barcode skip";
            }

            // 2022.01.14 SungTae Start : [추가]
            if(CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ)
            {
                ckb_MgzUnloadType.Visible = true;
            }
            else
            {
                ckb_MgzUnloadType.Visible = false;
            }
            // 2022.01.14 SungTae 

#if true //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)
            _setRfidOptionVisible();
#else
            //
            //200204 ksg :
            if(CData.CurCompany != ECompany.JSCK && CData.CurCompany != ECompany.SCK)
            {
                pnl_RfidUse.Visible = false;
            }
            else pnl_RfidUse.Visible = true;
#endif
        }

        #region Basic method
        /// <summary>
        /// View가 화면에 그려질때 실행해야 하는 함수
        /// </summary>
        public void Open()
        {
            Set();
        }

        /// <summary>
        /// View가 화면에서 지워질때 실행해야 하는 함수
        /// </summary>
        public void Close()
        {
        }

        public void Set()
        {
            txt_EmptySlotCnt.Text           = CData.Opt.iEmptySlotCnt.ToString();
            cmb_Emit.SelectedIndex          = (int)CData.Opt.eEmitMgz;              //20200513 jhc : 매거진 배출 위치 변경 추가
            ckb_MgzMatchingOn.Checked       = CData.Opt.bOfLMgzMatchingOn;          //190214 ksg :
            ckb_MgzFirstTopSlot.Checked     = CData.Opt.bFirstTopSlot;              //190503 ksg : 
            ckb_MgzFirstTopSlotOff.Checked  = CData.Opt.bFirstTopSlotOff;           //190503 ksg : 
            ckb_OnlRfidUse.Checked          = CData.Opt.bOnlRfidSkip;               //200203 ksg :
            ckb_OflRfidUse.Checked          = CData.Opt.bOflRfidSkip;               //200203 ksg :
            ckb_MgzUnloadType.Checked       = CData.Opt.bOfLMgzUnloadType;          // 2021.12.08 : [추가] (Qorvo향 VOC) 
        }

        public void Get()
        {
            int.TryParse(txt_EmptySlotCnt.Text, out CData.Opt.iEmptySlotCnt);

            CData.Opt.eEmitMgz          = (EMgzWay)cmb_Emit.SelectedIndex;  //20200513 jhc : 매거진 배출 위치 변경 추가
            CData.Opt.bOfLMgzMatchingOn = ckb_MgzMatchingOn.Checked;        //190214 ksg :
            CData.Opt.bFirstTopSlot     = ckb_MgzFirstTopSlot.Checked;      //190503 ksg : 
            CData.Opt.bFirstTopSlotOff  = ckb_MgzFirstTopSlotOff.Checked;   //190503 ksg :
            CData.Opt.bOnlRfidSkip      = ckb_OnlRfidUse.Checked;           //200203 ksg :
            CData.Opt.bOflRfidSkip      = ckb_OflRfidUse.Checked;           //200203 ksg : 
            CData.Opt.bOfLMgzUnloadType = ckb_MgzUnloadType.Checked;        // 2021.12.08 : [추가] (Qorvo향 VOC) 

            //int.TryParse(txt_AutoLDStopCount.Text, out CData.Opt.iAutoLoadingStopCount);    // 2020-11-17, jhLee : Loading stop 투입 수량
            CData.Opt.iAutoLoadingStopCount = (CData.Opt.iAutoLoadingStopCount <= 1) ? 1 : CData.Opt.iAutoLoadingStopCount;     // 최소/최대 범위 체크
            CData.Opt.iAutoLoadingStopCount = (CData.Opt.iAutoLoadingStopCount > 10) ? 10 : CData.Opt.iAutoLoadingStopCount;
        }

        /// <summary>
        /// 해당 View가 Dispose(종료)될 때 실행 됨 
        /// View의 Dispose함수 실행하면 자동으로 실행됨
        /// </summary>
        private void _Release()
        {
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

        //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)
        private void _setRfidOptionVisible()
        {
            if (CData.CurCompany == ECompany.SCK  || 
                CData.CurCompany == ECompany.JSCK || 
                CData.CurCompany == ECompany.JCET) //200625 lks
            {
                pnl_RfidUse.Visible = true;
                ckb_OnlRfidUse.Visible = true;
                ckb_OflRfidUse.Visible = true;
            }
            //201012 jhc : (KEYENCE BCR)/(RFID) 옵션으로 구분 적용
            else if ((CDataOption.RFID == ERfidType.Keyence) && 
            //else if ((CData.CurCompany == ECompany.SPIL) && 
                     (CDataOption.OnlRfid == eOnlRFID.Use || CDataOption.OflRfid == eOflRFID.Use))
            {
                pnl_RfidUse.Visible = true;

                if (CDataOption.OnlRfid == eOnlRFID.Use)
                { ckb_OnlRfidUse.Visible = true; }
                else
                { ckb_OnlRfidUse.Visible = false; }

                if (CDataOption.OflRfid == eOflRFID.Use)
                { ckb_OflRfidUse.Visible = true; }
                else
                { ckb_OflRfidUse.Visible = false; }
            }
            else
            { pnl_RfidUse.Visible = false; }
        }

        #endregion
    }
}
