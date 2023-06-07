using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace SG2000X
{
    public partial class frmMst : Form
    {
        private Timer m_tmUp   ;
        private Timer m_tmTcpIp;

        private FileInfo m_fi;
        private string m_sPath;

        private int btnTimer = 0;

        public frmMst()
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

            m_tmUp = new Timer();
            m_tmUp.Interval = 50;
            m_tmUp.Tick += _M_tmUp_Tick;
            m_tmUp.Start();

            m_tmTcpIp = new Timer();
            m_tmTcpIp.Interval = 2000;
            m_tmTcpIp.Tick += _M_TcpIp_Tick;
            m_tmTcpIp.Start();

            //2021-07-01, jhLee : QC 사용에 대한 사이트 조건을 제거, Qorvo 및 Skyworks에서 사용
            if(CDataOption.UseQC == false) // delete .CurCompany != ECompany.SkyWorks) 
            {
                lbl_QcStatus.Visible   = false;
                btn_Connect .Visible   = false;
                btn_Disconnect.Visible = false;
            }

            m_sPath = Application.StartupPath + "\\SG2000X.exe";
            m_fi = new FileInfo(m_sPath);

            lbl_Ver.Text = "RPS Delta Version " +  Assembly.GetExecutingAssembly().GetName().Version.ToString() + "\r\n " + m_fi.CreationTime.ToString("yyyy-MM-dd");
            //lbl_Company.Text = CData.CurCompany.ToString(); 기존 구문
            //211005 pjh : License 파일 사용 시 입력한 Company Name으로 사용
            if (!CData.useLicense)
            { lbl_Company.Text = CData.CurCompany.ToString(); }
            else
            { lbl_Company.Text = CData.CurCompanyName.ToString(); }
            //
        }

        private void _M_tmUp_Tick(object sender, EventArgs e)
        {
            if (CData.Lev == ELv.Master)
            {
                lbl_Main.Visible = true;
                lbl_Main.Text = CSQ_Main.It.EAutoStep.ToString();
                lbl_Main_T.Visible = true;

                btn_Disconnect.Visible = true;
            }
            else
            {
                lbl_Main.Visible = false;
                lbl_Main_T.Visible = false;

                btn_Disconnect.Visible = false;
            }

            lbl_Onl.Text = CData.Parts[(int)EPart.ONL ].iStat.ToString();
            lbl_Inr.Text = CData.Parts[(int)EPart.INR ].iStat.ToString();
            lbl_Onp.Text = CData.Parts[(int)EPart.ONP ].iStat.ToString();
            lbl_Grl.Text = CData.Parts[(int)EPart.GRDL].iStat.ToString();
            lbl_Grr.Text = CData.Parts[(int)EPart.GRDR].iStat.ToString();
            lbl_Ofp.Text = CData.Parts[(int)EPart.OFP ].iStat.ToString();
            lbl_Dry.Text = CData.Parts[(int)EPart.DRY ].iStat.ToString();
            lbl_Ofl.Text = CData.Parts[(int)EPart.OFL ].iStat.ToString();

            lbl_OnlSt.Text = CSq_OnL.It.Step.ToString();
            lbl_InrSt.Text = CSq_Inr.It.Step.ToString();
            lbl_OnpSt.Text = CSq_OnP.It.Step.ToString();
            lbl_GrlSt.Text = CData  .L_GRD.Step.ToString();
            lbl_GrrSt.Text = CData  .R_GRD.Step.ToString();
            lbl_OfpSt.Text = CSq_OfP.It.Step.ToString();
            lbl_DrySt.Text = CSq_Dry.It.Step.ToString();
            lbl_OflSt.Text = CSq_OfL.It.Step.ToString();

            lbl_GrlGSt.Text = CData.L_GRD.GStep.ToString();
            lbl_GrrGSt.Text = CData.R_GRD.GStep.ToString();

            lblM_OnlHD.BackColor = GV.CR_X[Convert.ToInt32(CSq_OnL.It   .m_bHD)];
            lblM_InrHD.BackColor = GV.CR_X[Convert.ToInt32(CSq_Inr.It   .m_bHD)];
            lblM_OnpHD.BackColor = GV.CR_X[Convert.ToInt32(CSq_OnP.It   .m_bHD)];
            lblM_GrlHD.BackColor = GV.CR_X[Convert.ToInt32(CData  .L_GRD.m_bHD)];
            lblM_GrrHD.BackColor = GV.CR_X[Convert.ToInt32(CData  .R_GRD.m_bHD)];
            lblM_OfpHD.BackColor = GV.CR_X[Convert.ToInt32(CSq_OfP.It   .m_bHD)];
            lblM_DryHD.BackColor = GV.CR_X[Convert.ToInt32(CSq_Dry.It   .m_bHD)];
            lblM_OflHD.BackColor = GV.CR_X[Convert.ToInt32(CSq_OfL.It   .m_bHD)];

            //190612 ksg :
            //20191022 ghk_spindle_type
            //if (CSpl.It.bUse232) 
            //if (CDataOption.SplType == eSpindleType.Rs232)
            //{
            //    lbl_SplStatus.BackColor = System.Drawing.Color.DarkOrange;
            //    lbl_SplStatus.Text = "RS232";
            //}
            //else
            //{
            //    lbl_SplStatus.BackColor = System.Drawing.Color.Lime;
            //    lbl_SplStatus.Text = "eThercat";
            //}
            // 2023.03.15 Max
            lbl_SplStatus.BackColor = System.Drawing.Color.DarkOrange;
            lbl_SplStatus.Text = "RS485";

            //190618 ksg :
            if (CDataOption.CurEqu == eEquType.Nomal) 
            {
                lbl_BCRStatus.ForeColor = System.Drawing.Color.DarkOrange;
                lbl_BCRStatus.Text      = "InRail";
            }
            else
            {
                lbl_BCRStatus.ForeColor = System.Drawing.Color.Lime;
                lbl_BCRStatus.Text      = "Picker";
            }
        }

        private void _M_TcpIp_Tick(object sender, EventArgs e)
        {
            //if (btnTimer > -1)
            //{
            //    btnTimer++;
            //    if (btnTimer > 2)
            //    {
            //        btnTimer = -1;
            //        btn_Connect.Enabled = true;
            //    }
            //}
            
            //if (CTcpIp.It.IsConnect() && CTcpIp.It.bIsConnect)
            if(CGxSocket.It.IsConnected())
            {
                lbl_QcStatus.Text = "Connect On";
            }
            else
            {
                lbl_QcStatus.Text = "Connect Off";
            }
        }

        private void frmMst_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_tmUp != null)
            {
            if (m_tmUp.Enabled)
            {m_tmUp.Stop(); }
            m_tmUp.Dispose();
            m_tmUp = null;
            }
        }
        //190406 ksg : Qc
        private void btn_Connect_Click(object sender, EventArgs e)
        {
            //if(!CTcpIp.It.C)
            /*CTcpIp.It.TcpIpConnect();
            btn_Connect.Enabled = false;
            btnTimer = 0;
            */
            QcConnect();
        }

        private void QcConnect()
        {
            if (CGxSocket.It.IsConnected() == false)
            {
                CGxSocket.It.Connect();
            }

            // if (CDataOption.UseQC == false) return;         // 라이센스에서 QC를 사용하지 않는다고 설정하였다면 QC와 연결을 시도하지 않도록 한다.

            //        if (btn_Connect.Enabled)
            //        {
			            //// 2021-01-?? YYY, QC Vision 연결
            //            // 2020.10.15 JSKim St
            //            //CTcpIp.It.TcpIpConnect();
            //            //if (CTcpIp.It.IsConnect())
            //            //{
            //            //  CTcpIp.It.TcpIpClose();
            //            //}

            //            //if (CGxSocket.It.IsConnected())
            //            //{
            //            //    CGxSocket.It.CloseClient();
            //            //}
            //            //System.Threading.Thread.Sleep(1000);
            //            //// 2020.10.15 JSKim Ed

            //            //CTcpIp.It.TcpIpConnect();
            //            CGxSocket.It.Connect();
            //            btn_Connect.Enabled = false;
            //            btnTimer = 0;
            //        }
        }

        //190406 ksg : Qc
        private void btn_Disconnect_Click(object sender, EventArgs e)
        {
			// 2021-01-?? YYY, QC Vision 연결종료
            // 2020.10.15 JSKim St
            //if (CTcpIp.It.IsConnect())
            //{
            //  CTcpIp.It.TcpIpClose();
            //}

            if (CGxSocket.It.IsConnected())
            {
                CGxSocket.It.CloseClient();         // 연결을 끊는다.
            }
        }
    }
}
