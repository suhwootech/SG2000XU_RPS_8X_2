using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace SG2000X
{
    public partial class vwDev_02Prm_7Advanced : UserControl
    {
        public vwDev_02Prm_7Advanced()
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

            _SetVisible();
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

        //koo : Qorvo 언어변환. --------------------------------------------------------------------------------
        private void SetWriteLanguage(string controlName, string text)
        {
            Control[] ctrls = this.Controls.Find(controlName, true);
            ctrls[0].GetType().GetProperty("Text").SetValue(ctrls[0], text, null);
        }

        private void _SetVisible()
        {
            pnl_WheelLossCorrect.Visible = CDataOption.UseWheelLossCorrect; //201203 jhc : 드레싱 후 N번째 그라인딩 시 휠 소모량 보정 기능
            pnl_GrindCondition.Visible = CDataOption.UseAdvancedGrindCondition; //201225 jhc : 고급 Grind Condition 설정/체크 기능 (Spindle Current, Table Vacuum Low Limit 설정)
        }

        private void _WheelLossCopy(object sender, EventArgs e)
        {
            Button mBtn = sender as Button;

            int Select = Convert.ToInt32(mBtn.Tag);

            double [] dWheelLoss = new double[GV.WHEEL_LOSS_CORRECT_STRIP_MAX];
            string sTextFrom = "";
            string sTextTo = "";
            
            if (Select == 0)
            {
                //Left >> Right Copy
                sTextFrom = "txtP_WheelLossL";
                sTextTo = "txtP_WheelLossR";
            }
            else if (Select == 1)
            {
                //Right >> Left Copy
                sTextFrom = "txtP_WheelLossR";
                sTextTo = "txtP_WheelLossL";
            }
            else
            {
                return; //잘못된 버튼 Tag
            }
            
            for (int i = 0; i < GV.WHEEL_LOSS_CORRECT_STRIP_MAX; i++)
            {
                //201207 jhc : DEBUG
                double.TryParse(((TextBox)(this.Controls.Find(sTextFrom+(i+1).ToString(), true)[0])).Text, out dWheelLoss[i]);
                //dWheelLoss[i] = Convert.ToDouble(((TextBox)(this.Controls.Find(sTextFrom+(i+1).ToString(), true)[0])).Text);
                dWheelLoss[i] = GU.Truncate(dWheelLoss[i],4);
                ((TextBox)(Controls.Find(sTextTo+(i+1).ToString(), true)[0])).Text = dWheelLoss[i].ToString("0.0000");            
            }
        }

        private void txtP_WheelLoss_TextChanged(object sender, EventArgs e)
        {
            TextBox txtBox = sender as TextBox;
            int select = Convert.ToInt32(txtBox.Parent.Tag);

            string s1 = "";
            string s2 = "";

            if (select == 0)
            {
                s1 = "txtP_WheelLossL";
                s2 = "txtP_WheelLossSumL";
            }
            else if (select == 1)
            {
                s1 = "txtP_WheelLossR";
                s2 = "txtP_WheelLossSumR";
            }

            double dVal = 0.0;
            double dSum = 0.0;
            for (int i = 0; i < GV.WHEEL_LOSS_CORRECT_STRIP_MAX; i++)
            {
                //201207 jhc : DEBUG
                double.TryParse(((TextBox)(this.Controls.Find(s1+(i+1).ToString(), true)[0])).Text, out dVal);
                //dVal = Convert.ToDouble(((TextBox)(this.Controls.Find(s1+(i+1).ToString(), true)[0])).Text);
                dVal = GU.Truncate(dVal,4);
                dSum += dVal;
            }
            ((TextBox)(Controls.Find(s2.ToString(), true)[0])).Text = dSum.ToString("0.0000");
        }
        #endregion

        #region Public method
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

        /// <summary>
        /// 데이터를 화면에 출력
        /// </summary>
        public void Set()
        {
            //201203 jhc : 드레싱 후 N번째 그라인딩 시 휠 소모량 보정 기능
            for (int i = 0; i < GV.WHEEL_LOSS_CORRECT_STRIP_MAX; i++)
            {
                ((TextBox)(Controls.Find("txtP_WheelLossL"+(i+1), true)[0])).Text = CData.Dev.aData[0].dWheelLoss[i].ToString("0.0000");
                ((TextBox)(Controls.Find("txtP_WheelLossR"+(i+1), true)[0])).Text = CData.Dev.aData[1].dWheelLoss[i].ToString("0.0000");
            }
            ((TextBox)(Controls.Find("txtP_TotalWheelLossLimitL", true)[0])).Text = CData.Dev.aData[0].dTotalWheelLossLimit.ToString("0.0000");
            ((TextBox)(Controls.Find("txtP_TotalWheelLossLimitR", true)[0])).Text = CData.Dev.aData[1].dTotalWheelLossLimit.ToString("0.0000");
            //

            //201225 jhc : 고급 Grind Condition 설정/체크 기능 (Spindle Current, Table Vacuum Low Limit 설정)
            for (int i = 0; i < 2; i++)
            {
                ((TextBox)(Controls.Find("txtP_CurrentT"+(i+1)+"DrsLow", true)[0])).Text = CData.Dev.aData[i].nSpindleCurrentDressLow.ToString();                       //Lower Limit of Spindle Current when Dressing
                ((TextBox)(Controls.Find("txtP_CurrentT"+(i+1)+"DrsHigh", true)[0])).Text = CData.Dev.aData[i].nSpindleCurrentDressHigh.ToString();                     //Upper Limit of Spindle Current when Dressing
                ((TextBox)(Controls.Find("txtP_CurrentT"+(i+1)+"GrdLow", true)[0])).Text = CData.Dev.aData[i].nSpindleCurrentGrindLow.ToString();                       //Lower Limit of Spindle Current when Grinding
                ((TextBox)(Controls.Find("txtP_CurrentT"+(i+1)+"GrdHigh", true)[0])).Text = CData.Dev.aData[i].nSpindleCurrentGrindHigh.ToString();                     //Upper Limit of Spindle Current when Grinding
                ((TextBox)(Controls.Find("txtP_VacT"+(i+1)+"GrdLow", true)[0])).Text = (Math.Abs(CData.Dev.aData[i].dTableVacuumGrindLimit)).ToString("0.0");           //Lower Limit of Table Vacuum when Grinding : Vacuum (-) Value
                ((TextBox)(Controls.Find("txtP_VacT"+(i+1)+"LowHoldTime", true)[0])).Text = (Math.Abs(CData.Dev.aData[i].dTableVacuumLowHoldTime)).ToString("0.000");   //Table Vacuum Lower Limit Holding Time (이 시간 이상 Lower Limit 유지 시 Alarm)
            }
            //..
        }

        public void Get()
        {
            //201203 jhc : 드레싱 후 N번째 그라인딩 시 휠 소모량 보정 기능
            for (int i = 0; i < GV.WHEEL_LOSS_CORRECT_STRIP_MAX; i++)
            {
                //201207 jhc : DEBUG
                double.TryParse(((TextBox)(this.Controls.Find("txtP_WheelLossL"+(i+1), true)[0])).Text, out CData.Dev.aData[0].dWheelLoss[i]);
                double.TryParse(((TextBox)(this.Controls.Find("txtP_WheelLossR"+(i+1), true)[0])).Text, out CData.Dev.aData[1].dWheelLoss[i]);
                CData.Dev.aData[0].dWheelLoss[i] = GU.Truncate(CData.Dev.aData[0].dWheelLoss[i],4);
                CData.Dev.aData[1].dWheelLoss[i] = GU.Truncate(CData.Dev.aData[1].dWheelLoss[i],4);
            }
            //201207 jhc : DEBUG
            double.TryParse(((TextBox)(this.Controls.Find("txtP_TotalWheelLossLimitL", true)[0])).Text, out CData.Dev.aData[0].dTotalWheelLossLimit);
            double.TryParse(((TextBox)(this.Controls.Find("txtP_TotalWheelLossLimitR", true)[0])).Text, out CData.Dev.aData[1].dTotalWheelLossLimit);
            CData.Dev.aData[0].dTotalWheelLossLimit = GU.Truncate(CData.Dev.aData[0].dTotalWheelLossLimit,4);
            CData.Dev.aData[1].dTotalWheelLossLimit = GU.Truncate(CData.Dev.aData[1].dTotalWheelLossLimit,4);
            //

            //201225 jhc : 고급 Grind Condition 설정/체크 기능 (Spindle Current, Table Vacuum Low Limit 설정)
            for (int i = 0; i < 2; i++)
            {
                int.TryParse(((TextBox)(this.Controls.Find("txtP_CurrentT"+(i+1)+"DrsLow", true)[0])).Text, out CData.Dev.aData[i].nSpindleCurrentDressLow);        //Lower Limit of Spindle Current when Dressing
                int.TryParse(((TextBox)(this.Controls.Find("txtP_CurrentT"+(i+1)+"DrsHigh", true)[0])).Text, out CData.Dev.aData[i].nSpindleCurrentDressHigh);      //Upper Limit of Spindle Current when Dressing
                int.TryParse(((TextBox)(this.Controls.Find("txtP_CurrentT"+(i+1)+"GrdLow", true)[0])).Text, out CData.Dev.aData[i].nSpindleCurrentGrindLow);        //Lower Limit of Spindle Current when Grinding
                int.TryParse(((TextBox)(this.Controls.Find("txtP_CurrentT"+(i+1)+"GrdHigh", true)[0])).Text, out CData.Dev.aData[i].nSpindleCurrentGrindHigh);      //Upper Limit of Spindle Current when Grinding
                double.TryParse(((TextBox)(this.Controls.Find("txtP_VacT"+(i+1)+"GrdLow", true)[0])).Text, out CData.Dev.aData[i].dTableVacuumGrindLimit);          //Lower Limit of Table Vacuum when Grinding
                double.TryParse(((TextBox)(this.Controls.Find("txtP_VacT"+(i+1)+"LowHoldTime", true)[0])).Text, out CData.Dev.aData[i].dTableVacuumLowHoldTime);    //Table Vacuum Lower Limit Holding Time (이 시간 이상 Lower Limit 유지 시 Alarm)
                //Truncate
                CData.Dev.aData[i].dTableVacuumGrindLimit = GU.Truncate((-(Math.Abs(CData.Dev.aData[i].dTableVacuumGrindLimit))),1); //Vacuum (-) Value
                CData.Dev.aData[i].dTableVacuumLowHoldTime = GU.Truncate(CData.Dev.aData[i].dTableVacuumLowHoldTime,3); //Table Vacuum Lower Limit Holding Time (이 시간 이상 Lower Limit 유지 시 Alarm)
                // Low, High Value Arrangement
                if (CData.Dev.aData[i].nSpindleCurrentDressHigh != 0) { GU.Arrange(ref CData.Dev.aData[i].nSpindleCurrentDressLow, ref CData.Dev.aData[i].nSpindleCurrentDressHigh); }
                if (CData.Dev.aData[i].nSpindleCurrentGrindHigh != 0) { GU.Arrange(ref CData.Dev.aData[i].nSpindleCurrentGrindLow, ref CData.Dev.aData[i].nSpindleCurrentGrindHigh); }
            }
            //..
        }
        #endregion
    }
}
