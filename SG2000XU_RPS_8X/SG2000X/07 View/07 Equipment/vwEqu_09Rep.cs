using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace SG2000X
{
    public partial class vwEqu_09Rep : UserControl
    {
        /// <summary>
        /// 화면에 정보 업데이트 타이머
        /// </summary>
        private Timer m_tmUpdt;
        public vwEqu_09Rep()
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
            //왼쪽 베이스 측정값
            double dMaxBase_L = 0.0;
            double dMinBase_L = 0.0;
            double dTtvBase_L = 0.0;
            txt_Base1_L.Text = CData.ProbeTest[(int)EWay.L].dBase[0].ToString();
            txt_Base2_L.Text = CData.ProbeTest[(int)EWay.L].dBase[1].ToString();
            txt_Base3_L.Text = CData.ProbeTest[(int)EWay.L].dBase[2].ToString();
            txt_Base4_L.Text = CData.ProbeTest[(int)EWay.L].dBase[3].ToString();
            txt_Base5_L.Text = CData.ProbeTest[(int)EWay.L].dBase[4].ToString();
            txt_Base6_L.Text = CData.ProbeTest[(int)EWay.L].dBase[5].ToString();
            txt_Base7_L.Text = CData.ProbeTest[(int)EWay.L].dBase[6].ToString();
            txt_Base8_L.Text = CData.ProbeTest[(int)EWay.L].dBase[7].ToString();
            txt_Base9_L.Text = CData.ProbeTest[(int)EWay.L].dBase[8].ToString();
            txt_Base10_L.Text = CData.ProbeTest[(int)EWay.L].dBase[9].ToString();

            dMaxBase_L = CData.ProbeTest[(int)EWay.L].dBase.Max();
            dMinBase_L = CData.ProbeTest[(int)EWay.L].dBase.Min();
            dTtvBase_L = Math.Round((dMaxBase_L - dMinBase_L), 4);
            txt_BaseTtv_L.Text = dTtvBase_L.ToString();

            //왼쪽 센터 측정값
            double dMaxCenter_L = 0.0;
            double dMinCenter_L = 0.0;
            double dTtvCenter_L = 0.0;
            txt_Center1_L.Text = CData.ProbeTest[(int)EWay.L].dCenter[0].ToString();
            txt_Center2_L.Text = CData.ProbeTest[(int)EWay.L].dCenter[1].ToString();
            txt_Center3_L.Text = CData.ProbeTest[(int)EWay.L].dCenter[2].ToString();
            txt_Center4_L.Text = CData.ProbeTest[(int)EWay.L].dCenter[3].ToString();
            txt_Center5_L.Text = CData.ProbeTest[(int)EWay.L].dCenter[4].ToString();
            txt_Center6_L.Text = CData.ProbeTest[(int)EWay.L].dCenter[5].ToString();
            txt_Center7_L.Text = CData.ProbeTest[(int)EWay.L].dCenter[6].ToString();
            txt_Center8_L.Text = CData.ProbeTest[(int)EWay.L].dCenter[7].ToString();
            txt_Center9_L.Text = CData.ProbeTest[(int)EWay.L].dCenter[8].ToString();
            txt_Center10_L.Text = CData.ProbeTest[(int)EWay.L].dCenter[9].ToString();

            dMaxCenter_L = CData.ProbeTest[(int)EWay.L].dCenter.Max();
            dMinCenter_L = CData.ProbeTest[(int)EWay.L].dCenter.Min();
            dTtvCenter_L = Math.Round((dMaxCenter_L - dMinCenter_L), 4);
            txt_CenterTtv_L.Text = dTtvCenter_L.ToString();

            //왼쪽 블럭 측정값
            double dMaxBlock_L = 0.0;
            double dMinBlock_L = 0.0;
            double dTtvBlock_L = 0.0;
            CData.ProbeTest[(int)EWay.L].dBlock[0] = Math.Round((CData.ProbeTest[(int)EWay.L].dCenter[0] - CData.ProbeTest[(int)EWay.L].dBase[0]), 4);
            CData.ProbeTest[(int)EWay.L].dBlock[1] = Math.Round((CData.ProbeTest[(int)EWay.L].dCenter[1] - CData.ProbeTest[(int)EWay.L].dBase[1]), 4);
            CData.ProbeTest[(int)EWay.L].dBlock[2] = Math.Round((CData.ProbeTest[(int)EWay.L].dCenter[2] - CData.ProbeTest[(int)EWay.L].dBase[2]), 4);
            CData.ProbeTest[(int)EWay.L].dBlock[3] = Math.Round((CData.ProbeTest[(int)EWay.L].dCenter[3] - CData.ProbeTest[(int)EWay.L].dBase[3]), 4);
            CData.ProbeTest[(int)EWay.L].dBlock[4] = Math.Round((CData.ProbeTest[(int)EWay.L].dCenter[4] - CData.ProbeTest[(int)EWay.L].dBase[4]), 4);
            CData.ProbeTest[(int)EWay.L].dBlock[5] = Math.Round((CData.ProbeTest[(int)EWay.L].dCenter[5] - CData.ProbeTest[(int)EWay.L].dBase[5]), 4);
            CData.ProbeTest[(int)EWay.L].dBlock[6] = Math.Round((CData.ProbeTest[(int)EWay.L].dCenter[6] - CData.ProbeTest[(int)EWay.L].dBase[6]), 4);
            CData.ProbeTest[(int)EWay.L].dBlock[7] = Math.Round((CData.ProbeTest[(int)EWay.L].dCenter[7] - CData.ProbeTest[(int)EWay.L].dBase[7]), 4);
            CData.ProbeTest[(int)EWay.L].dBlock[8] = Math.Round((CData.ProbeTest[(int)EWay.L].dCenter[8] - CData.ProbeTest[(int)EWay.L].dBase[8]), 4);
            CData.ProbeTest[(int)EWay.L].dBlock[9] = Math.Round((CData.ProbeTest[(int)EWay.L].dCenter[9] - CData.ProbeTest[(int)EWay.L].dBase[9]), 4);

            txt_Block1_L.Text = CData.ProbeTest[(int)EWay.L].dBlock[0].ToString();
            txt_Block2_L.Text = CData.ProbeTest[(int)EWay.L].dBlock[1].ToString();
            txt_Block3_L.Text = CData.ProbeTest[(int)EWay.L].dBlock[2].ToString();
            txt_Block4_L.Text = CData.ProbeTest[(int)EWay.L].dBlock[3].ToString();
            txt_Block5_L.Text = CData.ProbeTest[(int)EWay.L].dBlock[4].ToString();
            txt_Block6_L.Text = CData.ProbeTest[(int)EWay.L].dBlock[5].ToString();
            txt_Block7_L.Text = CData.ProbeTest[(int)EWay.L].dBlock[6].ToString();
            txt_Block8_L.Text = CData.ProbeTest[(int)EWay.L].dBlock[7].ToString();
            txt_Block9_L.Text = CData.ProbeTest[(int)EWay.L].dBlock[8].ToString();
            txt_Block10_L.Text = CData.ProbeTest[(int)EWay.L].dBlock[9].ToString();

            dMaxBlock_L = CData.ProbeTest[(int)EWay.L].dBlock.Max();
            dMinBlock_L = CData.ProbeTest[(int)EWay.L].dBlock.Min();
            dTtvBlock_L = Math.Round((dMaxBlock_L - dMinBlock_L), 4);
            txt_BlockTtv_L.Text = dTtvBlock_L.ToString();

            //오른쪽 베이스 측정값
            double dMaxBase_R = 0.0;
            double dMinBase_R = 0.0;
            double dTtvBase_R = 0.0;
            txt_Base1_R.Text = CData.ProbeTest[(int)EWay.R].dBase[0].ToString();
            txt_Base2_R.Text = CData.ProbeTest[(int)EWay.R].dBase[1].ToString();
            txt_Base3_R.Text = CData.ProbeTest[(int)EWay.R].dBase[2].ToString();
            txt_Base4_R.Text = CData.ProbeTest[(int)EWay.R].dBase[3].ToString();
            txt_Base5_R.Text = CData.ProbeTest[(int)EWay.R].dBase[4].ToString();
            txt_Base6_R.Text = CData.ProbeTest[(int)EWay.R].dBase[5].ToString();
            txt_Base7_R.Text = CData.ProbeTest[(int)EWay.R].dBase[6].ToString();
            txt_Base8_R.Text = CData.ProbeTest[(int)EWay.R].dBase[7].ToString();
            txt_Base9_R.Text = CData.ProbeTest[(int)EWay.R].dBase[8].ToString();
            txt_Base10_R.Text = CData.ProbeTest[(int)EWay.R].dBase[9].ToString();

            dMaxBase_R = CData.ProbeTest[(int)EWay.R].dBase.Max();
            dMinBase_R = CData.ProbeTest[(int)EWay.R].dBase.Min();
            dTtvBase_R = Math.Round((dMaxBase_R - dMinBase_R), 4);
            txt_BaseTtv_R.Text = dTtvBase_R.ToString();

            //오른쪽 센터 측정값
            double dMaxCenter_R = 0.0;
            double dMinCenter_R = 0.0;
            double dTtvCenter_R = 0.0;
            txt_Center1_R.Text = CData.ProbeTest[(int)EWay.R].dCenter[0].ToString();
            txt_Center2_R.Text = CData.ProbeTest[(int)EWay.R].dCenter[1].ToString();
            txt_Center3_R.Text = CData.ProbeTest[(int)EWay.R].dCenter[2].ToString();
            txt_Center4_R.Text = CData.ProbeTest[(int)EWay.R].dCenter[3].ToString();
            txt_Center5_R.Text = CData.ProbeTest[(int)EWay.R].dCenter[4].ToString();
            txt_Center6_R.Text = CData.ProbeTest[(int)EWay.R].dCenter[5].ToString();
            txt_Center7_R.Text = CData.ProbeTest[(int)EWay.R].dCenter[6].ToString();
            txt_Center8_R.Text = CData.ProbeTest[(int)EWay.R].dCenter[7].ToString();
            txt_Center9_R.Text = CData.ProbeTest[(int)EWay.R].dCenter[8].ToString();
            txt_Center10_R.Text = CData.ProbeTest[(int)EWay.R].dCenter[9].ToString();

            dMaxCenter_R = CData.ProbeTest[(int)EWay.R].dCenter.Max();
            dMinCenter_R = CData.ProbeTest[(int)EWay.R].dCenter.Min();
            dTtvCenter_R = Math.Round((dMaxCenter_R - dMinCenter_R), 4);
            txt_CenterTtv_R.Text = dTtvCenter_R.ToString();

            //오른쪽 블럭 측정값
            double dMaxBlock_R = 0.0;
            double dMinBlock_R = 0.0;
            double dTtvBlock_R = 0.0;
            CData.ProbeTest[(int)EWay.R].dBlock[0] = Math.Round((CData.ProbeTest[(int)EWay.R].dCenter[0] - CData.ProbeTest[(int)EWay.R].dBase[0]), 4);
            CData.ProbeTest[(int)EWay.R].dBlock[1] = Math.Round((CData.ProbeTest[(int)EWay.R].dCenter[1] - CData.ProbeTest[(int)EWay.R].dBase[1]), 4);
            CData.ProbeTest[(int)EWay.R].dBlock[2] = Math.Round((CData.ProbeTest[(int)EWay.R].dCenter[2] - CData.ProbeTest[(int)EWay.R].dBase[2]), 4);
            CData.ProbeTest[(int)EWay.R].dBlock[3] = Math.Round((CData.ProbeTest[(int)EWay.R].dCenter[3] - CData.ProbeTest[(int)EWay.R].dBase[3]), 4);
            CData.ProbeTest[(int)EWay.R].dBlock[4] = Math.Round((CData.ProbeTest[(int)EWay.R].dCenter[4] - CData.ProbeTest[(int)EWay.R].dBase[4]), 4);
            CData.ProbeTest[(int)EWay.R].dBlock[5] = Math.Round((CData.ProbeTest[(int)EWay.R].dCenter[5] - CData.ProbeTest[(int)EWay.R].dBase[5]), 4);
            CData.ProbeTest[(int)EWay.R].dBlock[6] = Math.Round((CData.ProbeTest[(int)EWay.R].dCenter[6] - CData.ProbeTest[(int)EWay.R].dBase[6]), 4);
            CData.ProbeTest[(int)EWay.R].dBlock[7] = Math.Round((CData.ProbeTest[(int)EWay.R].dCenter[7] - CData.ProbeTest[(int)EWay.R].dBase[7]), 4);
            CData.ProbeTest[(int)EWay.R].dBlock[8] = Math.Round((CData.ProbeTest[(int)EWay.R].dCenter[8] - CData.ProbeTest[(int)EWay.R].dBase[8]), 4);
            CData.ProbeTest[(int)EWay.R].dBlock[9] = Math.Round((CData.ProbeTest[(int)EWay.R].dCenter[9] - CData.ProbeTest[(int)EWay.R].dBase[9]), 4);

            txt_Block1_R.Text = CData.ProbeTest[(int)EWay.R].dBlock[0].ToString();
            txt_Block2_R.Text = CData.ProbeTest[(int)EWay.R].dBlock[1].ToString();
            txt_Block3_R.Text = CData.ProbeTest[(int)EWay.R].dBlock[2].ToString();
            txt_Block4_R.Text = CData.ProbeTest[(int)EWay.R].dBlock[3].ToString();
            txt_Block5_R.Text = CData.ProbeTest[(int)EWay.R].dBlock[4].ToString();
            txt_Block6_R.Text = CData.ProbeTest[(int)EWay.R].dBlock[5].ToString();
            txt_Block7_R.Text = CData.ProbeTest[(int)EWay.R].dBlock[6].ToString();
            txt_Block8_R.Text = CData.ProbeTest[(int)EWay.R].dBlock[7].ToString();
            txt_Block9_R.Text = CData.ProbeTest[(int)EWay.R].dBlock[8].ToString();
            txt_Block10_R.Text = CData.ProbeTest[(int)EWay.R].dBlock[9].ToString();

            dMaxBlock_R = CData.ProbeTest[(int)EWay.R].dBlock.Max();
            dMinBlock_R = CData.ProbeTest[(int)EWay.R].dBlock.Min();
            dTtvBlock_R = Math.Round((dMaxBlock_R - dMinBlock_R), 4);
            txt_BlockTtv_R.Text = dTtvBlock_R.ToString();
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

        private void btn_SaveLeft_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CData.bLastClick = true; //190509 ksg :
            string sPath = "";
            string sLine = "";

            StreamWriter sw;
            sfd_Repeat.ShowDialog();
            if (sfd_Repeat.FileName != "")
            {
                sPath = sfd_Repeat.FileName;

                sw = new StreamWriter(sPath, false, Encoding.GetEncoding("euc-kr"));

                sLine = "LEFT";
                sw.WriteLine(sLine);

                sLine = "NO,BASE,CENTER,BLOCK";
                sw.WriteLine(sLine);

                sLine = "1,";
                sLine += txt_Base1_L.Text + ",";
                sLine += txt_Center1_L.Text + ",";
                sLine += txt_Block1_L.Text;
                sw.WriteLine(sLine);

                sLine = "2,";
                sLine += txt_Base2_L.Text + ",";
                sLine += txt_Center2_L.Text + ",";
                sLine += txt_Block2_L.Text;
                sw.WriteLine(sLine);

                sLine = "3,";
                sLine += txt_Base3_L.Text + ",";
                sLine += txt_Center3_L.Text + ",";
                sLine += txt_Block3_L.Text;
                sw.WriteLine(sLine);

                sLine = "4,";
                sLine += txt_Base4_L.Text + ",";
                sLine += txt_Center4_L.Text + ",";
                sLine += txt_Block4_L.Text;
                sw.WriteLine(sLine);

                sLine = "5,";
                sLine += txt_Base5_L.Text + ",";
                sLine += txt_Center5_L.Text + ",";
                sLine += txt_Block5_L.Text;
                sw.WriteLine(sLine);

                sLine = "6,";
                sLine += txt_Base6_L.Text + ",";
                sLine += txt_Center6_L.Text + ",";
                sLine += txt_Block6_L.Text;
                sw.WriteLine(sLine);

                sLine = "7,";
                sLine += txt_Base7_L.Text + ",";
                sLine += txt_Center7_L.Text + ",";
                sLine += txt_Block7_L.Text;
                sw.WriteLine(sLine);

                sLine = "8,";
                sLine += txt_Base8_L.Text + ",";
                sLine += txt_Center8_L.Text + ",";
                sLine += txt_Block8_L.Text;
                sw.WriteLine(sLine);

                sLine = "9,";
                sLine += txt_Base9_L.Text + ",";
                sLine += txt_Center9_L.Text + ",";
                sLine += txt_Block9_L.Text;
                sw.WriteLine(sLine);

                sLine = "10,";
                sLine += txt_Base10_L.Text + ",";
                sLine += txt_Center10_L.Text + ",";
                sLine += txt_Block10_L.Text;
                sw.WriteLine(sLine);

                sLine = "TTV,";
                sLine += txt_BaseTtv_L.Text + ",";
                sLine += txt_CenterTtv_L.Text + ",";
                sLine += txt_BlockTtv_L.Text;
                sw.WriteLine(sLine);

                sw.Close();
            }
        }

        private void btn_SaveRight_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CData.bLastClick = true; //190509 ksg :
            string sPath = "";
            string sLine = "";

            StreamWriter sw;
            sfd_Repeat.ShowDialog();
            if (sfd_Repeat.FileName != "")
            {
                sPath = sfd_Repeat.FileName;

                sw = new StreamWriter(sPath, false, Encoding.GetEncoding("euc-kr"));

                sLine = "RIGHT";
                sw.WriteLine(sLine);

                sLine = "NO,BASE,CENTER,BLOCK";
                sw.WriteLine(sLine);

                sLine = "1,";
                sLine += txt_Base1_R.Text + ",";
                sLine += txt_Center1_R.Text + ",";
                sLine += txt_Block1_R.Text;
                sw.WriteLine(sLine);

                sLine = "2,";
                sLine += txt_Base2_R.Text + ",";
                sLine += txt_Center2_R.Text + ",";
                sLine += txt_Block2_R.Text;
                sw.WriteLine(sLine);

                sLine = "3,";
                sLine += txt_Base3_R.Text + ",";
                sLine += txt_Center3_R.Text + ",";
                sLine += txt_Block3_R.Text;
                sw.WriteLine(sLine);

                sLine = "4,";
                sLine += txt_Base4_R.Text + ",";
                sLine += txt_Center4_R.Text + ",";
                sLine += txt_Block4_R.Text;
                sw.WriteLine(sLine);

                sLine = "5,";
                sLine += txt_Base5_R.Text + ",";
                sLine += txt_Center5_R.Text + ",";
                sLine += txt_Block5_R.Text;
                sw.WriteLine(sLine);

                sLine = "6,";
                sLine += txt_Base6_R.Text + ",";
                sLine += txt_Center6_R.Text + ",";
                sLine += txt_Block6_R.Text;
                sw.WriteLine(sLine);

                sLine = "7,";
                sLine += txt_Base7_R.Text + ",";
                sLine += txt_Center7_R.Text + ",";
                sLine += txt_Block7_R.Text;
                sw.WriteLine(sLine);

                sLine = "8,";
                sLine += txt_Base8_R.Text + ",";
                sLine += txt_Center8_R.Text + ",";
                sLine += txt_Block8_R.Text;
                sw.WriteLine(sLine);

                sLine = "9,";
                sLine += txt_Base9_R.Text + ",";
                sLine += txt_Center9_R.Text + ",";
                sLine += txt_Block9_R.Text;
                sw.WriteLine(sLine);

                sLine = "10,";
                sLine += txt_Base10_R.Text + ",";
                sLine += txt_Center10_R.Text + ",";
                sLine += txt_Block10_R.Text;
                sw.WriteLine(sLine);

                sLine = "TTV,";
                sLine += txt_BaseTtv_R.Text + ",";
                sLine += txt_CenterTtv_R.Text + ",";
                sLine += txt_BlockTtv_R.Text;
                sw.WriteLine(sLine);

                sw.Close();
            }
        }

        private void btn_MeaStart_L_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CData.bLastClick = true; //190509 ksg :
            Button mBtn = sender as Button;
            ESeq ESeq;
            Enum.TryParse(mBtn.Tag.ToString(), out ESeq);

            // 도어 체크  // 2022.06.11 lhs
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            {
                if (!CSQ_Main.It.CheckDoor()) // SCK,SCK+
                {
                    CSQ_Man.It.bBtnShow = true;
                    CMsg.Show(eMsg.Warning, "Warning", "Door close Please");
                    return;
                }
            }

            if (CSQ_Main.It.m_iStat == EStatus.Manual)
            {
                CMsg.Show(eMsg.Error, "Error", "During Before Manual Run now, So wait finish run.");
                return;
            }
            else
            {
                CSQ_Man.It.Seq = ESeq;
            }
        }


        private void btn_Clear_L_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CData.bLastClick = true; //190509 ksg :
            if (CSQ_Main.It.m_iStat == EStatus.Manual)
            {
                CMsg.Show(eMsg.Error, "Error", "During Before Manual Run now, So wait finish run");
                return;
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    CData.ProbeTest[0].dBase[i] = 999;
                    CData.ProbeTest[0].dCenter[i] = 999;
                    CData.ProbeTest[0].dBlock[i] = 999;
                }
            }
        }

        private void btn_Clear_R_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CData.bLastClick = true; //190509 ksg :
            if (CSQ_Main.It.m_iStat == EStatus.Manual)
            {
                CMsg.Show(eMsg.Error, "Error", "During Before Manual Run now, So wait finish run.");
                return;
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    CData.ProbeTest[1].dBase[i] = 999;
                    CData.ProbeTest[1].dCenter[i] = 999;
                    CData.ProbeTest[1].dBlock[i] = 999;
                }
            }
        }
    }
}
