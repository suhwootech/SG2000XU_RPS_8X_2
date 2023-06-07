using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace SG2000X
{
    public partial class vwEqu : UserControl
    {
        private int m_iPage = -1;

        private vwEqu_01Wmx m_vw01Wmx = new vwEqu_01Wmx();
        private vwEqu_02Mot m_vw02Mot = new vwEqu_02Mot();
        private vwEqu_03Spl m_vw03Spl = new vwEqu_03Spl();
        private vwEqu_04In  m_vw04In  = new vwEqu_04In ();
        private vwEqu_05Out m_vw05Out = new vwEqu_05Out();
        private vwEqu_06Prb m_vw06Prb = new vwEqu_06Prb();
        private vwEqu_07Twl m_vw07Twl = new vwEqu_07Twl();
        private vwEqu_08Bcr m_vw08Bcr = new vwEqu_08Bcr();
        private vwEqu_09Rep m_vw09Rep = new vwEqu_09Rep();
        private vwEqu_10Sgm m_vw10Sgm = new vwEqu_10Sgm();
        private vwEqu_11Dfs m_vw11Dfs = new vwEqu_11Dfs();        
        private vwEqu_12Light m_vw12Light = new vwEqu_12Light(); //20191108 ghk_light
        private vwEqu_13Cbm m_vw13Cbm = new vwEqu_13Cbm();       //20200905 LCY Count Base Monitor Mode
        private vwEqu_14Tm8 m_vw14Tm8 = new vwEqu_14Tm8();       //2020.10.05 lhs Table Measure 8Points
        private vwEqu_15IV2 m_vw15IV2 = new vwEqu_15IV2();       //210827 syc : 2004U IV2 

        public vwEqu()
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


            m_iPage = 1;
        }

        private void _Release()
        {
            m_vw01Wmx.Dispose();
            m_vw02Mot.Dispose();
            m_vw03Spl.Dispose();
            m_vw04In .Dispose();
            m_vw05Out.Dispose();
            m_vw06Prb.Dispose();
            m_vw07Twl.Dispose();
            m_vw08Bcr.Dispose();
            m_vw09Rep.Dispose();
            m_vw10Sgm.Dispose();
            m_vw11Dfs.Dispose();
            //20191108 ghk_light
            m_vw12Light.Dispose();
            m_vw13Cbm.Dispose();
            m_vw14Tm8.Dispose();    //2020.10.05 lhs
            m_vw15IV2.Dispose(); //210827 syc : 2004U IV2
        }

        private void _Open()
        {
            switch(m_iPage)
            {
                case 1:
                    m_vw01Wmx.Open();
                    pnl_Base.Controls.Add(m_vw01Wmx);
                    break;
                case 2:
                    m_vw02Mot.Open();
                    pnl_Base.Controls.Add(m_vw02Mot);
                    break;
                case 3:
                    m_vw03Spl.Open();
                    pnl_Base.Controls.Add(m_vw03Spl);
                    break;
                case 4:
                    m_vw04In.Open();
                    pnl_Base.Controls.Add(m_vw04In);
                    break;
                case 5:
                    m_vw05Out.Open();
                    pnl_Base.Controls.Add(m_vw05Out);
                    break;
                case 6:
                    m_vw06Prb.Open();
                    pnl_Base.Controls.Add(m_vw06Prb);
                    break;
                case 7:
                    m_vw07Twl.Open();
                    pnl_Base.Controls.Add(m_vw07Twl);
                    break;
                case 8:
                    m_vw08Bcr.Open();
                    pnl_Base.Controls.Add(m_vw08Bcr);
                    break;
                case 9:
                    m_vw09Rep.Open();
                    pnl_Base.Controls.Add(m_vw09Rep);
                    break;
                case 10:
                    m_vw10Sgm.Open();
                    pnl_Base.Controls.Add(m_vw10Sgm);
                    break;
                case 11:
                    m_vw11Dfs.Open();
                    pnl_Base.Controls.Add(m_vw11Dfs);
                    break;
                //20191108 ghk_light
                case 12:
                    m_vw12Light.Open();
                    pnl_Base.Controls.Add(m_vw12Light);
                    break;
                case 13:
                    m_vw13Cbm.Open();
                    pnl_Base.Controls.Add(m_vw13Cbm);
                    break;
                case 14:
                    m_vw14Tm8.Open();
                    pnl_Base.Controls.Add(m_vw14Tm8);   //2020.10.05 lhs
                    break;
                //210827 syc : 2004U IV2
                case 15:
                    m_vw15IV2.Open();
                    pnl_Base.Controls.Add(m_vw15IV2);
                    break;

            }
        }

        private void _Close()
        {
            switch (m_iPage)
            {
                case 1:
                    m_vw01Wmx.Close();
                    break;
                case 2:
                    m_vw02Mot.Close();
                    break;
                case 3:
                    m_vw03Spl.Close();
                    break;
                case 4:
                    m_vw04In.Close();
                    break;
                case 5:
                    m_vw05Out.Close();
                    break;
                case 6:
                    m_vw06Prb.Close();
                    break;
                case 7:
                    m_vw07Twl.Close();
                    break;
                case 8:
                    m_vw08Bcr.Close();
                    break;
                case 9:
                    m_vw09Rep.Close();
                    break;
                case 10:
                    m_vw10Sgm.Close();
                    break;
                case 11:
                    m_vw11Dfs.Close();
                    break;
                //20191108 ghk_light
                case 12:
                    m_vw12Light.Close();
                    break;
                case 13:
                    m_vw13Cbm.Close();
                    break;
                case 14:
                    m_vw14Tm8.Close();  //2020.10.05 lhs
                    break;
                //210827 syc : 2004U IV2
                case 15:
                    m_vw15IV2.Close();
                    break;
            }

            pnl_Base.Controls.Clear();
        }

        private void _HideMenu()
        {
            ELv RetLv = CData.Lev;
            rdbMn_Mot.Visible = (int)RetLv >= CData.Opt.iLvUtilMot;
            rdbMn_Spl.Visible = (int)RetLv >= CData.Opt.iLvUtilSpd;
            rdbMn_In.Visible = (int)RetLv >= CData.Opt.iLvUtilIn;
            rdbMn_Out.Visible = (int)RetLv >= CData.Opt.iLvUtilOut;
            rdbMn_Prb.Visible = (int)RetLv >= CData.Opt.iLvUtilPrb;
            rdbMn_TwL.Visible = (int)RetLv >= CData.Opt.iLvUtilTw;
            rdbMn_Bcr.Visible = (int)RetLv >= CData.Opt.iLvUtilBcr;
            rdbMn_Repeat.Visible = (int)RetLv >= CData.Opt.iLvUtilRepeat;
            rdbMn_IV2.Visible = CDataOption.Use2004U; //210827 syc : 2004U IV2 

            if (CData.CurCompany != ECompany.ASE_KR)
            {
                rdbMn_SGem.Visible = false;
            }
            rdbMn_Tm8.Visible = CDataOption.UseTableMeas8p;  //201022 lhs

            if (RetLv == ELv.Master)
            { rdb_CBM.Visible = true; }
            else
            { rdb_CBM.Visible = false; }
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

        public void Open()
        {
            _HideMenu();
            _Open();

            if(CDataOption.eDfserver == eDfserver.NotUse)
            {
                rdbMn_Df.Visible = false;
            }

            //20191106 ghk_lightcontrol
            if (CDataOption.LightControl == eLightControl.NotUse)
            {
                rdbMn_Light.Visible = false;
            }
            //
        }

        public void Close()
        {
            _Close();
        }

        private void rdbMn_CheckedChanged(object sender, EventArgs e)
        {
            CData.bLastClick = true; //190509 ksg :
            RadioButton mRdb = sender as RadioButton;
            int iNext = int.Parse(mRdb.Tag.ToString());

            _SetLog(string.Format("Click, {0} -> {1}", m_iPage, iNext));

            if (m_iPage != iNext)
            {
                _Close();

                m_iPage = iNext;

                _Open();
            }
        }

        public void InitPage()
        {
            rdbMn_Equ.Checked = true;
        }
    }
}
