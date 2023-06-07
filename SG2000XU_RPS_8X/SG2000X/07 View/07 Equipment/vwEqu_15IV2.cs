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
    public partial class vwEqu_15IV2 : UserControl
    {
        private CIV2Ctrl pCtrlIV2 = new CIV2Ctrl(3);

        private Timer m_tmUpdt;

        private string sIV2Path = "D:\\Suhwoo\\SG2000X\\Equip\\IV2"; //상대경로로 변경해서 디덱토리 생성을 생각해보자
        private string sLastLog = ""; //

        /// <summary>
        /// 메뉴얼 확인 버튼
        /// </summary>
        private enum eManualSelect
        {
            Onp    = 1,
            Ofp    = 2,
            OnpOfp = 3,
            Max    = 4
        }
        /// <summary>
        /// 현재 선택된 메뉴얼 위치 
        /// Onp 인지 Ofp 인지 둘다 동시에 보낼건지
        /// 초기 라디오 버튼 선택 값은 Onp 이기때문에 1로 설정
        /// </summary>
        private int iManualSelected = 1;

        public vwEqu_15IV2()
        {
            InitializeComponent();

            m_tmUpdt = new Timer();
            m_tmUpdt.Interval = 50;
            m_tmUpdt.Tick += _M_tmUpdt_Tick;
        }
        private void _M_tmUpdt_Tick(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {

            }
        }

        public void Get()
        {
            int dVal = 0;

            int.TryParse(txt_OnpPort.Text, out dVal);
            CData.iIV2OnpPort = dVal;

            int.TryParse(txt_OfpPort.Text, out dVal);
            CData.iIV2OfpPort = dVal;

            CData.sIV2OnpIP = txt_OnpIP.Text;
            CData.sIV2OfpIP = txt_OfpIP.Text;
        }

        //210830 syc : 2004U IV2
        public void Set()
        {
            txt_OnpPort.Text = CData.iIV2OnpPort.ToString();
            txt_OnpIP  .Text = CData.sIV2OnpIP             ;
            txt_OfpPort.Text = CData.iIV2OfpPort.ToString();
            txt_OfpIP  .Text = CData.sIV2OfpIP             ;
        }

        //210830 syc : 2004U IV2
        public void SaveData()
        {
            Get();
            if (CSetup.It.Save() == 0)
            {
                CMsg.Show(eMsg.Notice, "Notice", "Save Complete");                
            }
            
        }

        //210830 syc : 2004U IV2
        public void LoadData()
        {
            CSetup.It.Load();
            Set();
        }
        public void Open()
        {
            // 타이머 멈춤 상태면 타이머 다시 시작
            if (!m_tmUpdt.Enabled) { m_tmUpdt.Start(); }
            LoadData();
            CSq_OnP.It.ShowLog(lbx_OnpLog);
            CSq_OfP.It.ShowLog(lbx_OfpLog);
        }

        public void Close()
        {
            // 타이머 실행 중이면 타이머 멈춤
            if (m_tmUpdt.Enabled) { m_tmUpdt.Stop(); }
            CSq_OnP.It.ShowLog(null);
            CSq_OfP.It.ShowLog(null);
        }

        /// <summary>
        /// 라디오 버튼 체인지 이벤트
        /// 트리거 및 명령어를 OnLoader에 보낼건지 OffLoader에 보낼건지
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManualCheckedChange(object sender, EventArgs e)
        {
            RadioButton rdb = sender as RadioButton;
            switch (rdb.Tag.ToString())
            {
                case "Onp":
                    {
                        iManualSelected = (int)eManualSelect.Onp;
                        break;
                    }
                case "Ofp":
                    {
                        iManualSelected = (int)eManualSelect.Ofp;
                        break;
                    }
                case "OnpOfp":
                    {
                        iManualSelected = (int)eManualSelect.OnpOfp;
                        break;
                    }
                default:
                    {
                        iManualSelected = (int)eManualSelect.Max; //에러
                        break;
                    }
            }
        }

        /// <summary>
        /// 데이터 저장
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Save_Click(object sender, EventArgs e)
        {
            SaveData();

            CSq_OnP.It.CloseIV2();
            CSq_OnP.It.initIV2Ctrl(CData.sIV2OnpIP, CData.iIV2OnpPort);
            CSq_OnP.It.ConnectIV2();
            //연결

            CSq_OfP.It.CloseIV2();
            CSq_OfP.It.initIV2Ctrl(CData.sIV2OfpIP, CData.iIV2OfpPort);
            CSq_OfP.It.ConnectIV2();
            //연결
        }

        private void Tri_Click(object sender, EventArgs e)
        {
            if (iManualSelected == (int)eManualSelect.Onp)
            {
                CSq_OnP.It.TriggerIV2();
            }
            else if (iManualSelected == (int)eManualSelect.Ofp)
            {
                CSq_OfP.It.TriggerIV2();
            }
            else if (iManualSelected == (int)eManualSelect.OnpOfp)
            {
                CSq_OnP.It.TriggerIV2();
                CSq_OfP.It.TriggerIV2();
            }
            else
            {
                return;// 여기오면 안됨
            }
        }

        private void Disconnect_Click(object sender, EventArgs e)
        {
            if (iManualSelected == (int)eManualSelect.Onp)
            {
                CSq_OnP.It.CloseIV2();
            }
            else if (iManualSelected == (int)eManualSelect.Ofp)
            {
                CSq_OfP.It.CloseIV2();
            }
            else if (iManualSelected == (int)eManualSelect.OnpOfp)
            {
                CSq_OnP.It.CloseIV2();
                CSq_OfP.It.CloseIV2();
            }
            else
            {
                return;// 여기오면 안됨
            }
        }
        private void SendMsg_Click(object sender, EventArgs e)
        {
            
            Button mBtn = sender as Button;

            if (iManualSelected == (int)eManualSelect.Onp)
            {
                CSq_OnP.It.SendMsg(txt_Send.Text);
            }
            else if (iManualSelected == (int)eManualSelect.Ofp)
            {
                CSq_OfP.It.SendMsg(txt_Send.Text);
            }
            else if (iManualSelected == (int)eManualSelect.OnpOfp)
            {
                CSq_OnP.It.SendMsg(txt_Send.Text);
                CSq_OfP.It.SendMsg(txt_Send.Text);
            }
            else
            {
                return;// 여기오면 안됨
            }
        }
    }
}
