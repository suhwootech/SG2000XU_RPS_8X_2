using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace SG2000X
{
    // 2021.07.14 lhs : Maintance 화면 - PM Count (Life Time) SCK+ VOC
    /// <summary>
    /// Maintance 화면 (PM Count) 
    /// </summary>
    /// 

    public partial class vwOpt_09Mnt : UserControl
    {
        private Timer m_tmUpdt;

        public vwOpt_09Mnt()
        {
            if      ((int)ELang.English == CData.Opt.iSelLan)   { System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US"); }
            else if ((int)ELang.Korea == CData.Opt.iSelLan)     { System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ko-KR"); }
            else if ((int)ELang.China == CData.Opt.iSelLan)     { System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-Hans"); }

            InitializeComponent();

            m_tmUpdt = new Timer();
            m_tmUpdt.Interval = 200;
            m_tmUpdt.Tick += _M_tmUpdt_Tick;
        }

        #region Basic method
        /// <summary>
        /// View가 화면에 그려질때 실행해야 하는 함수
        /// </summary>
        public void Open()
        {

            /*
            if (CSQ_Man.It.bBtnShow && (CSQ_Main.It.m_iStat == EStatus.Idle || CSQ_Main.It.m_iStat == EStatus.Stop))
            { _EnableBtn(true); }
            else
            { _EnableBtn(false); }
            */
            Set();

			// 타이머 멈춤 상태면 타이머 다시 시작
			if (!m_tmUpdt.Enabled)
			{
				m_tmUpdt.Start();
			}

		}

		/// <summary>
		/// View가 화면에서 지워질때 실행해야 하는 함수
		/// </summary>
		public void Close()
        {
			// 타이머 실행 중이면 타이머 멈춤
			if (m_tmUpdt.Enabled)
			{
				m_tmUpdt.Stop();
			}

		}

		public void Set()
        {
            if (CData.Opt.aTC_Cnt[0] == 0 || CData.Opt.aTblSkip[0]) { CData.tPM.bLT_Sponge_Check_Use    = false;    CData.tPM.bLT_Sponge_Change_Use     = false;    }
            else                                                    { CData.tPM.bLT_Sponge_Check_Use    = true;     CData.tPM.bLT_Sponge_Change_Use     = true;     }
            if (CData.Opt.aTC_Cnt[1] == 0 || CData.Opt.aTblSkip[1]) { CData.tPM.bRT_Sponge_Check_Use    = false;    CData.tPM.bRT_Sponge_Change_Use     = false;    }
            else                                                    { CData.tPM.bRT_Sponge_Check_Use    = true;     CData.tPM.bRT_Sponge_Change_Use     = true;     }
            if (CData.Opt.bOfpBtmCleanSkip)                         { CData.tPM.bOffP_Sponge_Check_Use  = false;    CData.tPM.bOffP_Sponge_Change_Use   = false;    }
            else                                                    { CData.tPM.bOffP_Sponge_Check_Use  = true;     CData.tPM.bOffP_Sponge_Change_Use   = true;     }

            txtM_LT_Sponge_Check_Set.Text       = CData.tPM.nLT_Sponge_Check_SetCnt.ToString();
            txtM_LT_Sponge_Check_Curr.Text      = CData.tPM.nLT_Sponge_Check_CurrCnt.ToString();
            lblM_LT_Sponge_Check_Use.Text       = CData.tPM.bLT_Sponge_Check_Use ? "USE" : "SKIP";

            txtM_RT_Sponge_Check_Set.Text       = CData.tPM.nRT_Sponge_Check_SetCnt.ToString();
            txtM_RT_Sponge_Check_Curr.Text      = CData.tPM.nRT_Sponge_Check_CurrCnt.ToString();
            lblM_RT_Sponge_Check_Use.Text       = CData.tPM.bRT_Sponge_Check_Use ? "USE" : "SKIP";

            txtM_OffP_Sponge_Check_Set.Text     = CData.tPM.nOffP_Sponge_Check_SetCnt.ToString();
            txtM_OffP_Sponge_Check_Curr.Text    = CData.tPM.nOffP_Sponge_Check_CurrCnt.ToString();
            lblM_OffP_Sponge_Check_Use.Text     = CData.tPM.bOffP_Sponge_Check_Use ? "USE" : "SKIP";

            txtM_LT_Sponge_Change_Set.Text      = CData.tPM.nLT_Sponge_Change_SetCnt.ToString();
            txtM_LT_Sponge_Change_Curr.Text     = CData.tPM.nLT_Sponge_Change_CurrCnt.ToString();
            lblM_LT_Sponge_Change_Use.Text      = CData.tPM.bLT_Sponge_Change_Use ? "USE" : "SKIP";

            txtM_RT_Sponge_Change_Set.Text      = CData.tPM.nRT_Sponge_Change_SetCnt.ToString();
            txtM_RT_Sponge_Change_Curr.Text     = CData.tPM.nRT_Sponge_Change_CurrCnt.ToString();
            lblM_RT_Sponge_Change_Use.Text      = CData.tPM.bRT_Sponge_Change_Use ? "USE" : "SKIP";

            txtM_OffP_Sponge_Change_Set.Text    = CData.tPM.nOffP_Sponge_Change_SetCnt.ToString();
            txtM_OffP_Sponge_Change_Curr.Text   = CData.tPM.nOffP_Sponge_Change_CurrCnt.ToString();
            lblM_OffP_Sponge_Change_Use.Text    = CData.tPM.bOffP_Sponge_Change_Use ? "USE" : "SKIP";

            txtM_PMMsg_ReDisp_Minute.Text       = CData.tPM.nPMMsg_ReDisp_Minute.ToString();

        }

        public void Get()
        {
            // Set Count 항목만
            int.TryParse(txtM_LT_Sponge_Check_Set.Text,     out CData.tPM.nLT_Sponge_Check_SetCnt);
            int.TryParse(txtM_RT_Sponge_Check_Set.Text,     out CData.tPM.nRT_Sponge_Check_SetCnt);
            int.TryParse(txtM_OffP_Sponge_Check_Set.Text,   out CData.tPM.nOffP_Sponge_Check_SetCnt);
            
            int.TryParse(txtM_LT_Sponge_Change_Set.Text,    out CData.tPM.nLT_Sponge_Change_SetCnt);
            int.TryParse(txtM_RT_Sponge_Change_Set.Text,    out CData.tPM.nRT_Sponge_Change_SetCnt);
            int.TryParse(txtM_OffP_Sponge_Change_Set.Text,  out CData.tPM.nOffP_Sponge_Change_SetCnt);
            
            int.TryParse(txtM_PMMsg_ReDisp_Minute.Text,     out CData.tPM.nPMMsg_ReDisp_Minute);

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

        private void _M_tmUpdt_Tick(object sender, EventArgs e)
        {
            // PM Count : Current Count 표시
            txtM_LT_Sponge_Check_Curr.Text      = CData.tPM.nLT_Sponge_Check_CurrCnt.ToString();
            txtM_RT_Sponge_Check_Curr.Text      = CData.tPM.nRT_Sponge_Check_CurrCnt.ToString();
            txtM_OffP_Sponge_Check_Curr.Text    = CData.tPM.nOffP_Sponge_Check_CurrCnt.ToString();
            
            txtM_LT_Sponge_Change_Curr.Text     = CData.tPM.nLT_Sponge_Change_CurrCnt.ToString();
            txtM_RT_Sponge_Change_Curr.Text     = CData.tPM.nRT_Sponge_Change_CurrCnt.ToString();
            txtM_OffP_Sponge_Change_Curr.Text   = CData.tPM.nOffP_Sponge_Change_CurrCnt.ToString();
            
        }

        #endregion

        private void btnM_Sponge_Count_Reset_Click(object sender, EventArgs e)
        {
            _SetLog("Reset Click");

            // 1. 먼저 레벨 및 암호 확인
            if (CData.Lev < ELv.Technician)
            {
                _SetLog("Warning : Upper select Level");
                CMsg.Show(eMsg.Warning, "Warning", "Upper select Level");
                return;
            }
            if (CheckPwd() == false)
            {
                _SetLog("CheckPwd() : Fail");
                return;
            }

            // 2. 현재 Count 리셋
            Button mBtn = sender as Button;
            string sIdx = mBtn.Tag.ToString();

            if      (sIdx == "0") { CData.tPM.nLT_Sponge_Check_CurrCnt      = 0;    _SetLog("nLT_Sponge_Check_CurrCnt = 0");   }
            else if (sIdx == "1") { CData.tPM.nRT_Sponge_Check_CurrCnt      = 0;    _SetLog("nRT_Sponge_Check_CurrCnt = 0");   }
            else if (sIdx == "2") { CData.tPM.nOffP_Sponge_Check_CurrCnt    = 0;    _SetLog("nOffP_Sponge_Check_CurrCnt = 0"); }
            else if (sIdx == "3") { CData.tPM.nLT_Sponge_Change_CurrCnt     = 0;    CData.tPM.nLT_Sponge_Check_CurrCnt  = 0;    _SetLog("nLT_Sponge_Change_CurrCnt = 0, nLT_Sponge_Check_CurrCnt = 0"); }
            else if (sIdx == "4") { CData.tPM.nRT_Sponge_Change_CurrCnt     = 0;    CData.tPM.nRT_Sponge_Check_CurrCnt  = 0;    _SetLog("nRT_Sponge_Change_CurrCnt = 0, nRT_Sponge_Check_CurrCnt = 0"); }
            else if (sIdx == "5") { CData.tPM.nOffP_Sponge_Change_CurrCnt   = 0;    CData.tPM.nOffP_Sponge_Check_CurrCnt = 0;   _SetLog("nOffP_Sponge_Change_CurrCnt = 0, nOffP_Sponge_Check_CurrCnt = 0"); }
            else    {  }// 미설정

            // 저장버튼 클릭
            _SetLog("Reset -> Save");
            btnSave_Mnt.PerformClick();
        }

		private void btnSave_Mnt_Click(object sender, EventArgs e)
		{
            _SetLog("Save Click");

            Get();
			
            CMnt.It.Save();
            _SetLog("Maintenance.cfg Save !!!");
            
            Set();

			CMsg.Show(eMsg.Notice, "Notice", "Maintenance data save complete");
        }

        private bool CheckPwd()
        {
			frmPwd mForm = new frmPwd(CData.Lev)
			{
				Location = Cursor.Position
			};

			if (mForm.ShowDialog() == DialogResult.OK)
            {
                Close();
                return true;
			}
            return false; 
		}


	}
}
