using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Diagnostics;
using System.IO;

namespace SG2000X
{
    public partial class vwEqu_12Light : UserControl
    {
        /// <summary>
        /// 화면에 정보 업데이트 타이머
        /// </summary>
        private Timer m_tmUpdt;
        public vwEqu_12Light()
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
        }

        #region Private method
        private void _M_tmUpdt_Tick(object sender, EventArgs e)
        {
            bool bConn = CLight.It.Chk_Open();
            
            btn_Open.Enabled = !bConn;
            btn_Close.Enabled = bConn;
            pnl_Para.Enabled = !bConn;            
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
            CLight.It.Load();
            
            cmb_Port  .SelectedItem  =      CData.Light.sPort             ;
            cmb_Baud  .SelectedItem  =      CData.Light.iBaud  .ToString();
            cmb_Parity.SelectedIndex = (int)CData.Light.eParity           ;
            cmb_DBits .SelectedItem  =      CData.Light.iDtBits.ToString();
            cmb_SBits .SelectedIndex = (int)CData.Light.eStBits           ;
            ckb_Rts   .Checked       =      CData.Light.bRts              ;
            nud_Val1  .Value         =      CData.LightVal1               ;
            nud_Val2  .Value         =      CData.LightVal2               ;
        }

        /// <summary>
        /// 화면에 값을 데이터로 저장
        /// </summary>
        private void _Get()
        {
            CData.Light.sPort = cmb_Port.SelectedItem.ToString();
            int.TryParse(cmb_Baud.Text, out CData.Light.iBaud);
            Enum.TryParse(cmb_Parity.Text, out CData.Light.eParity);
            int.TryParse(cmb_DBits.Text, out CData.Light.iDtBits);
            Enum.TryParse(cmb_SBits.Text, out CData.Light.eStBits);
            CData.Light.bRts = ckb_Rts.Checked;
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
            FileInfo fi = new FileInfo(GV.PATH_EQ_LIGHT + "Light.lgt");
            cmb_Port.DataSource = SerialPort.GetPortNames();

            _Set();

            if (fi.Exists)
            {
                CLight.It.Open_Port();
                CLight.It.LightOn1();
                CLight.It.LightOn2();
            }

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

        private void btn_Save_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            int iRet = 0;

            _Get();

            iRet = CLight.It.Save();
            if (iRet != 0)
            {
                CMsg.Show(eMsg.Error, "Error", "Light config file save fail");
            }
            else
            {
                CMsg.Show(eMsg.Notice, "Notice", "Light config file save success");
            }
        }

        private void btn_Open_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            int iRet = 0;

            _Set();

            iRet = CLight.It.Open_Port();
            if (iRet != 0)
            {
                CErr.Show(eErr.LIGHT_RS232_PORT_OPEN_ERROR);
            }
            else
            { CMsg.Show(eMsg.Notice, "Notice", "Light open success"); }
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            int iRet = 0;

            iRet = CLight.It.Close_Port();
            if (iRet != 0)
            {
                CErr.Show(eErr.LIGHT_RS232_PORT_CLOSE_ERROR);              
            }
            else
            { CMsg.Show(eMsg.Notice, "Notice", "Light close success"); }
        }

        private void btn_On1_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            string sRet = "";

            txt_Log.AppendText("> " + CLight.It.m_sCmd[(int)ELightCMD.scOn1] + "\r\n");

            sRet = CLight.It.LightOn1();

            txt_Log.AppendText("< " + sRet);
        }

        private void btn_Off1_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            string sRet = "";

            txt_Log.AppendText("> " + CLight.It.m_sCmd[(int)ELightCMD.scOff1] + "\r\n");

            sRet = CLight.It.LightOff1();

            txt_Log.AppendText("< " + sRet);
        }

        private void btn_Set1_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            int iVal = 0;
            string sRet = "";

            iVal = (int)nud_Val1.Value;

            CData.LightVal1 = iVal;
            CLight.It.Save();

            txt_Log.AppendText("> " + CLight.It.m_sCmd[(int)ELightCMD.scBright1] + iVal.ToString() + "\r\n");

            sRet = CLight.It.LightBright1(iVal);

            txt_Log.AppendText("< " + sRet);
        }
        private void nud_Val1_ValueChanged(object sender, EventArgs e)
        {
            _SetLog("Click");
            int iVal = 0;
            string sRet = "";

            iVal = (int)nud_Val1.Value;

            CData.LightVal1 = iVal;
            CLight.It.Save();

            txt_Log.AppendText("> " + CLight.It.m_sCmd[(int)ELightCMD.scBright1] + iVal.ToString() + "\r\n");

            sRet = CLight.It.LightBright1(iVal);

            txt_Log.AppendText("< " + sRet);
        }

        private void btn_On2_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            string sRet = "";

            txt_Log.AppendText("> " + CLight.It.m_sCmd[(int)ELightCMD.scOn2] + "\r\n");

            sRet = CLight.It.LightOn2();

            txt_Log.AppendText("< " + sRet);
        }

        private void btn_Off2_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            string sRet = "";

            txt_Log.AppendText("> " + CLight.It.m_sCmd[(int)ELightCMD.scOff2] + "\r\n");

            sRet = CLight.It.LightOff2();

            txt_Log.AppendText("< " + sRet);
        }

        private void btn_Set2_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            int iVal = 0;
            string sRet = "";

            iVal = (int)nud_Val2.Value;

            CData.LightVal2 = iVal;
            CLight.It.Save();

            txt_Log.AppendText("> " + CLight.It.m_sCmd[(int)ELightCMD.scBright2] + iVal.ToString() + "\r\n");

            sRet = CLight.It.LightBright2(iVal);

            txt_Log.AppendText("< " + sRet);
        }

        private void nud_Val2_ValueChanged(object sender, EventArgs e)
        {
            _SetLog("Click");
            int iVal = 0;
            string sRet = "";

            iVal = (int)nud_Val2.Value;

            CData.LightVal2 = iVal;
            CLight.It.Save();

            txt_Log.AppendText("> " + CLight.It.m_sCmd[(int)ELightCMD.scBright2] + iVal.ToString() + "\r\n");

            sRet = CLight.It.LightBright2(iVal);

            txt_Log.AppendText("< " + sRet);
        }
    }
}
