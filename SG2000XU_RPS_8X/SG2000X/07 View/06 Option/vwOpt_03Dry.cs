using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace SG2000X
{
    public partial class vwOpt_03Dry : UserControl
    {
        public vwOpt_03Dry()
        {
            if      ((int)ELang.English == CData.Opt.iSelLan)   {   System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");     }
            else if ((int)ELang.Korea   == CData.Opt.iSelLan)   {   System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ko-KR");     }
            else if ((int)ELang.China   == CData.Opt.iSelLan)   {   System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-Hans");   }
            InitializeComponent();

            bool bVisible = (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK);
            pnl_DelayTime.Visible   = bVisible; // 2022.01.26 lhs : Dryer Z축 Up 후에 안정화 시간 갖기, Pusher 걸리지 않고 동작시키기 위해
            pnl_SpongeOnOff.Visible = bVisible; // 2022.06.13 lhs : Sponge Water Auto On/Off

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
            ckb_DryStickSkip.Checked = CData.Opt.bDryStickSkip;

            txt_ZUpStableDelay.Text = CData.Opt.nDryZUpStableDelay.ToString(); // 2022.01.26 lhs Start : Dryer Z축 Up 후에 안정화 시간 갖기, Pusher 걸리지 않고 동작시키기 위해

            ckb_SpgWtAutoOnOff.Checked  = CData.Opt.bSpgWtAutoOnOff;
            txt_SpgWtOnMin.Text         = CData.Opt.nSpgWtOnMin.ToString();
            txt_SpgWtOffMin.Text        = CData.Opt.nSpgWtOffMin.ToString();


        }

        public void Get()
        {
            CData.Opt.bDryStickSkip      = ckb_DryStickSkip.Checked;

            int.TryParse(txt_ZUpStableDelay.Text, out CData.Opt.nDryZUpStableDelay); // 2022.01.26 lhs Start : Dryer Z축 Up 후에 안정화 시간 갖기, Pusher 걸리지 않고 동작시키기 위해

            CData.Opt.bSpgWtAutoOnOff   = ckb_SpgWtAutoOnOff.Checked;
            int.TryParse(txt_SpgWtOnMin.Text,  out CData.Opt.nSpgWtOnMin);
            int.TryParse(txt_SpgWtOffMin.Text, out CData.Opt.nSpgWtOffMin);

        }

        /// <summary>
        /// 해당 View가 Dispose(종료)될 때 실행 됨 
        /// View의 Dispose함수 실행하면 자동으로 실행됨
        /// </summary>
        private void _Release()
        {
        }

        private void _M_tmUpdt_Tick(object sender, EventArgs e)
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

        #endregion
    }
}
