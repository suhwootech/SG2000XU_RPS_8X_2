using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace SG2000X
{
    public partial class vwEqu_07Twl : UserControl
    {
        /// <summary>
        /// 화면에 정보 업데이트 타이머
        /// </summary>
        private Timer m_tmUpdt;

        public vwEqu_07Twl()
        {
            if      ((int)ELang.English == CData.Opt.iSelLan)   {   System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");     }
            else if ((int)ELang.Korea   == CData.Opt.iSelLan)   {   System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ko-KR");     }
            else if ((int)ELang.China   == CData.Opt.iSelLan)   {   System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-Hans");   }

            InitializeComponent();

            m_tmUpdt = new Timer();
            m_tmUpdt.Interval = 50;
            m_tmUpdt.Tick += _M_tmUpdt_Tick;

            dgvTw_List.Rows.Clear();

			int nMax_Twr = (int)ETowerStatus.MAX_TWR_STATUS;

			int nRIdx = 0;  // DataGridView Row Index
            for (int nTwIdx = 0; nTwIdx < nMax_Twr; nTwIdx++)
            {
                if (nTwIdx == (int)ETowerStatus.SPCAlarm)       // SPCAlarm 은 Qorvo만 표시
                {
                    if (CData.CurCompany != ECompany.Qorvo) {   continue;   }
                }
                
                if (nTwIdx == (int)ETowerStatus.LoadingStop)    // LoadingStop 은 SCK/SCK+만 표시
                {
                    if (CData.CurCompany != ECompany.SCK && CData.CurCompany != ECompany.JSCK)  {   continue;   }
                }                

                dgvTw_List.Rows.Add();
				dgvTw_List.Rows[nRIdx].Cells[0].Value = ((ETowerStatus)nTwIdx).ToString();
				dgvTw_List.Rows[nRIdx].Cells[1].Value = CData.m_TowerInfo[nTwIdx].Red.ToString();
				dgvTw_List.Rows[nRIdx].Cells[2].Value = CData.m_TowerInfo[nTwIdx].Yel.ToString();
				dgvTw_List.Rows[nRIdx].Cells[3].Value = CData.m_TowerInfo[nTwIdx].Grn.ToString();
				dgvTw_List.Rows[nRIdx].Cells[4].Value = CData.m_TowerInfo[nTwIdx].Buzz.ToString();

				nRIdx++;
			}
		}

        #region Private method
        private void _M_tmUpdt_Tick(object sender, EventArgs e)
        {
            
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
            int nMax_Twr = (int)ETowerStatus.MAX_TWR_STATUS;

            int nRIdx = 0;  // DataGridView Row Index
            for (int nTwIdx = 0; nTwIdx < nMax_Twr; nTwIdx++)
            {
                if (nTwIdx == (int)ETowerStatus.SPCAlarm)       // SPCAlarm 은 Qorvo만 표시
                {
                    if (CData.CurCompany != ECompany.Qorvo) {   continue;   }
                }
                
                if (nTwIdx == (int)ETowerStatus.LoadingStop)    // LoadingStop 은 SCK/SCK+만 표시
                {
                    if (CData.CurCompany != ECompany.SCK && CData.CurCompany != ECompany.JSCK)  {   continue;   }
                }

                dgvTw_List.Rows[nRIdx].Cells[0].Value = ((ETowerStatus)nTwIdx).ToString();
                dgvTw_List.Rows[nRIdx].Cells[1].Value = CData.m_TowerInfo[nTwIdx].Red.ToString();
                dgvTw_List.Rows[nRIdx].Cells[2].Value = CData.m_TowerInfo[nTwIdx].Yel.ToString();
                dgvTw_List.Rows[nRIdx].Cells[3].Value = CData.m_TowerInfo[nTwIdx].Grn.ToString();
                dgvTw_List.Rows[nRIdx].Cells[4].Value = CData.m_TowerInfo[nTwIdx].Buzz.ToString();

                nRIdx++;
            }
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

        private void btnTw_Start_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            int iTwIdx = 0; // 전체 Index
            if (dgvTw_List.SelectedRows == null) { return; }
            else 
            {
                //------------------------------------
                // 2022.06.17 lhs : 선택된 인덱스와 전체 인덱스가 다르므로 ETowerStatus의 String(명칭)으로 인덱스를 찾자 !!!
                String sSelTw = dgvTw_List.SelectedRows[0].Cells[0].Value.ToString();
                foreach(ETowerStatus eTw in Enum.GetValues(typeof(ETowerStatus)))
                {
					if (sSelTw == eTw.ToString())
					{
                        iTwIdx = (int)eTw;       
                        break;
					}
				}
                //------------------------------------
            }

            if (CData.WMX)
            {
                CTwr.It.TowerStatus(iTwIdx);
            }
        }

        private void btnTw_Stop_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            if (CData.WMX)
            {
                CIO.It.Set_Y(eY.SYS_TwlRed,     false);
                CIO.It.Set_Y(eY.SYS_TwlYellow,  false);
                CIO.It.Set_Y(eY.SYS_TwlGreen,   false);
                CIO.It.Set_Y(eY.SYS_BuzzK1,     false);
                CIO.It.Set_Y(eY.SYS_BuzzK2,     false);
                CIO.It.Set_Y(eY.SYS_BuzzK3,     false);
            }
        }

        private void btnTw_Save_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            int iTwIdx = 0;
            if (dgvTw_List.SelectedRows == null) { return; }
            else
            {
                //------------------------------------
                // 2022.06.17 lhs : 선택된 인덱스와 전체 인덱스가 다르므로 ETowerStatus의 String(명칭)으로 인덱스를 찾자 !!!
                String sSelTw = dgvTw_List.SelectedRows[0].Cells[0].Value.ToString();
                foreach (ETowerStatus eTw in Enum.GetValues(typeof(ETowerStatus)))
                {
                    if (sSelTw == eTw.ToString())
                    {
                        iTwIdx = (int)eTw;
                        break;
                    }
                }
                //------------------------------------
            }

            if      (rdbTw_ROff.Checked)    { CData.m_TowerInfo[iTwIdx].Red = ELamp.Off;   }
            else if (rdbTw_ROn.Checked)     { CData.m_TowerInfo[iTwIdx].Red = ELamp.On;    }
            else if (rdbTw_RFck.Checked)    { CData.m_TowerInfo[iTwIdx].Red = ELamp.Flick; }
                                                                
            if      (rdbTw_YOff.Checked)    { CData.m_TowerInfo[iTwIdx].Yel = ELamp.Off;   }
            else if (rdbTw_YOn.Checked)     { CData.m_TowerInfo[iTwIdx].Yel = ELamp.On;    }
            else if (rdbTw_YFck.Checked)    { CData.m_TowerInfo[iTwIdx].Yel = ELamp.Flick; }
                                                                
            if      (rdbTw_GOff.Checked)    { CData.m_TowerInfo[iTwIdx].Grn = ELamp.Off;   }
            else if (rdbTw_GOn.Checked)     { CData.m_TowerInfo[iTwIdx].Grn = ELamp.On;    }
            else if (rdbTw_GFck.Checked)    { CData.m_TowerInfo[iTwIdx].Grn = ELamp.Flick; }
                                                                
            if      (rdbTw_BzOff.Checked)   { CData.m_TowerInfo[iTwIdx].Buzz = EBuzz.Off;  }
            else if (rdbTw_Bz1.Checked)     { CData.m_TowerInfo[iTwIdx].Buzz = EBuzz.Buzz1;}
            else if (rdbTw_Bz2.Checked)     { CData.m_TowerInfo[iTwIdx].Buzz = EBuzz.Buzz2;}
            else if (rdbTw_Bz3.Checked)     { CData.m_TowerInfo[iTwIdx].Buzz = EBuzz.Buzz3;}

            CTower.It.Save();

            _Set();
        }

        private void dgvTw_List_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int iTwIdx = 0;
            if (dgvTw_List.SelectedRows == null) { return; }
            else
            {
                //------------------------------------
                // 2022.06.17 lhs : 선택된 인덱스와 전체 인덱스가 다르므로 ETowerStatus의 String(명칭)으로 인덱스를 찾자 !!!
                String sSelTw = dgvTw_List.SelectedRows[0].Cells[0].Value.ToString();
                foreach (ETowerStatus eTw in Enum.GetValues(typeof(ETowerStatus)))
                {
                    if (sSelTw == eTw.ToString())
                    {
                        iTwIdx = (int)eTw;
                        break;
                    }
                }
                //------------------------------------
            }

            switch (CData.m_TowerInfo[iTwIdx].Red)
            {
                case ELamp.Off:     { rdbTw_ROff.Checked    = true; break; }
                case ELamp.On:      { rdbTw_ROn.Checked     = true; break; }
                case ELamp.Flick:   { rdbTw_RFck.Checked    = true; break; }
            }
            switch (CData.m_TowerInfo[iTwIdx].Yel)
            {
                case ELamp.Off:     { rdbTw_YOff.Checked    = true; break; }
                case ELamp.On:      { rdbTw_YOn.Checked     = true; break; }
                case ELamp.Flick:   { rdbTw_YFck.Checked    = true; break; }
            }
            switch (CData.m_TowerInfo[iTwIdx].Grn)
            {
                case ELamp.Off:     { rdbTw_GOff.Checked    = true; break; }
                case ELamp.On:      { rdbTw_GOn.Checked     = true; break; }
                case ELamp.Flick:   { rdbTw_GFck.Checked    = true; break; }
            }
            switch (CData.m_TowerInfo[iTwIdx].Buzz)
            {
                case EBuzz.Off:     { rdbTw_BzOff.Checked   = true; break; }
                case EBuzz.Buzz1:   { rdbTw_Bz1.Checked     = true; break; }
                case EBuzz.Buzz2:   { rdbTw_Bz2.Checked     = true; break; }
                case EBuzz.Buzz3:   { rdbTw_Bz3.Checked     = true; break; }
            }
        }
    }
}
