using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SG2000X
{
    public partial class vwEqu_04In : UserControl
    {
        /// <summary>
        /// 화면에 정보 업데이트 타이머
        /// </summary>
        private Timer m_tmUpdt;
        public vwEqu_04In()
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

            pnlIO_In_Main.Controls.Clear();
            pnlIO_In_OnLoader.Controls.Clear();
            pnlIO_In_OffLoader.Controls.Clear();

            // Input - Main setting
            for (int i = 0; i < CIO.It.MAIN_IN_SIZE; i++)
            {
                Label mLbl = new Label();
                mLbl.Name = "lbl_" + CIO.It.aMain_InTxt[i].Substring(0, 5);
                mLbl.Size = new Size(478, 20);
                mLbl.Location = new Point(10, 10 + (i * (mLbl.Height + 5)));
                mLbl.Text = CIO.It.aMain_InTxt[i];
                mLbl.TextAlign = ContentAlignment.MiddleLeft;
                mLbl.Tag = ((CIO.It.MAIN_IN_ADDR * 8) + i);
                pnlIO_In_Main.Controls.Add(mLbl);
            }

            // Input - On Loader setting
            for (int i = 0; i < CIO.It.LDR_IN_SIZE; i++)
            {
                Label mLbl = new Label();
                mLbl.Name = "lbl_" + CIO.It.aLdr_InTxt[i].Substring(0, 5);
                mLbl.Size = new Size(478, 20);
                mLbl.Location = new Point(10, 10 + (i * (mLbl.Height + 5)));
                mLbl.Text = CIO.It.aLdr_InTxt[i];
                mLbl.TextAlign = ContentAlignment.MiddleLeft;
                mLbl.Tag = ((CIO.It.LDR_IN_ADDR * 8) + i);
                pnlIO_In_OnLoader.Controls.Add(mLbl);
            }

            // Input - Off Loader setting
            for (int i = 0; i < CIO.It.ODR_IN_SIZE; i++)
            {
                Label mLbl = new Label();
                mLbl.Name = "lbl_" + CIO.It.aOdr_InTxt[i].Substring(0, 5);
                mLbl.Size = new Size(478, 20);
                mLbl.Location = new Point(10, 10 + (i * (mLbl.Height + 5)));
                mLbl.Text = CIO.It.aOdr_InTxt[i];
                mLbl.TextAlign = ContentAlignment.MiddleLeft;
                mLbl.Tag = ((CIO.It.ODR_IN_ADDR * 8) + i);
                pnlIO_In_OffLoader.Controls.Add(mLbl);
            }
        }

        #region Private method
        private void _M_tmUpdt_Tick(object sender, EventArgs e)
        {
            // Input - Main
            for (int i = 0; i < CIO.It.MAIN_IN_SIZE; i++)
            {
                pnlIO_In_Main.Controls[i].BackColor = GV.CR_X[CIO.It.aInput[i]];
            }

            // Input - On Loader
            for (int i = 0; i < CIO.It.LDR_IN_SIZE; i++)
            {
                pnlIO_In_OnLoader.Controls[i].BackColor = GV.CR_X[CIO.It.aInput[CIO.It.MAIN_IN_SIZE + i]];
            }

            // Input - Off Loader
            for (int i = 0; i < CIO.It.ODR_IN_SIZE; i++)
            {
                pnlIO_In_OffLoader.Controls[i].BackColor = GV.CR_X[CIO.It.aInput[CIO.It.MAIN_IN_SIZE + CIO.It.LDR_IN_SIZE + i]];
            }
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

        #region Public method
        /// <summary>
        /// View가 화면에 그려질때 실행해야 하는 함수
        /// </summary>
        public void Open()
        {
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
    }
}
