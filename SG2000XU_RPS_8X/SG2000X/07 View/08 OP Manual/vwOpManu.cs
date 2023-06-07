using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SG2000X
{
    public partial class vwOpManu : UserControl
    {
        private Timer m_tmMain;

        public vwOpManu()
        {
            /*
            if ((int)eLanguage.English == Constants.LANGUAGE)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            }
            else if ((int)eLanguage.Korea == Constants.LANGUAGE)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ko-KR");
            }
            else if ((int)eLanguage.China == Constants.LANGUAGE)
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-Hans");
            }
            */
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

            m_tmMain = new Timer();
            m_tmMain.Interval = 50;
            m_tmMain.Tick += _M_tmMain_Tick;
        }

        public void Open()
        {
            if (m_tmMain != null && !m_tmMain.Enabled)
            {
                m_tmMain.Start();
            }
            //20191204 ghk_level
            _HideMenu();
            //
        }

        public void Close()
        {
            if (m_tmMain != null && m_tmMain.Enabled)
            {
                m_tmMain.Stop();
            }
        }

        public void Release()
        {
            if (m_tmMain != null)
            {
                m_tmMain.Stop();
                m_tmMain.Dispose();
                m_tmMain = null;
            }
        }

        private void _M_tmMain_Tick(object sender, EventArgs e)
        {
            //Left Table Vaccum
            if(CIO.It.Get_Y(eY.GRDL_TbVacuum)) lbl_LTable_Vac.BackColor = Color.Lime ;
            else                               lbl_LTable_Vac.BackColor = Color.FromArgb(60,60,60);
            //Right Table Vaccum
            if(CIO.It.Get_Y(eY.GRDR_TbVacuum)) lbl_RTable_Vac.BackColor = Color.Lime ;
            else                               lbl_RTable_Vac.BackColor = Color.FromArgb(60,60,60);

            //Left Table Eject
            if(CIO.It.Get_Y(eY.GRDL_TbEjector)) lbl_LTable_Eje.BackColor = Color.Lime ;
            else                                lbl_LTable_Eje.BackColor = Color.FromArgb(60,60,60);

            //Right Table Eject
            if(CIO.It.Get_Y(eY.GRDR_TbEjector)) lbl_RTable_Eje.BackColor = Color.Lime ;
            else                                lbl_RTable_Eje.BackColor = Color.FromArgb(60,60,60);

        }

        private void OpManual(object sender, EventArgs e)
        {
            CData.bLastClick = true; //190509 ksg :
            Button mBtn = sender as Button;

            // 2020.10.22 JSKim St
            bool bPumpRunResult = false;
            // 2020.10.22 JSKim Ed
            int iTag = int.Parse(mBtn.Tag.ToString());
            int iLoader; //ksg : 0 -> OnLoader, 1 -> OffLoader 
            EWay eWy   ;
            eX   eIPRun;
            // 2020.10.22 JSKim St
            eX   eIAddPRun;
            // 2020.10.22 JSKim Ed
            eY   eOPRun, eOVac , eDo;

            if (int.Parse(mBtn.Parent.Tag.ToString()) == 0)
            { 
                eWy = EWay.L; 
                iLoader = 0;
            }
            else
            { 
                eWy = EWay.R; 
                iLoader = 1;
            }

            //eIPRun = eX.PUMPL_Run;

            eIPRun    = eX.PUMPL_Run;
            // 2020.10.22 JSKim St
            eIAddPRun = eX.ADD_PUMPL_Run;
            // 2020.10.22 JSKim Ed
            eOPRun    = eY.PUMPL_Run;
            eOVac     = eY.GRDL_TbVacuum;
           
            if(eWy == EWay.R)
            {
                eIPRun    = eX.PUMPR_Run;
                // 2020.10.22 JSKim St
                eIAddPRun = eX.ADD_PUMPR_Run;
                // 2020.10.22 JSKim Ed
                eOPRun    = eY.PUMPR_Run;
                eOVac     = eY.GRDR_TbVacuum;
            }

            if(!(CSQ_Main.It.m_iStat != EStatus.Stop || CSQ_Main.It.m_iStat != EStatus.Idle)|| CSQ_Main.It.m_iStat == EStatus.Manual)
            {
                CMsg.Show(eMsg.Warning, "Warning", "The machine is in Run. First stop Please");
                iTag = 0;
                return;
            }

            switch(iTag)
            {   
                case 1 :  //Vac On

                    //201208 jhc : Manual 동작 상호 배제 (Vacuum/Eject vs. Table Water)
                    if (eWy == EWay.L)
                    {
                        CData.L_GRD.ActWater(false); //L-Table Water OFF
                        CData.L_GRD.ActEject(false); //L-Table Eject OFF
                    }
                    else
                    {
                        CData.R_GRD.ActWater(false); //R-Table Water OFF
                        CData.R_GRD.ActEject(false); //R-Table Eject OFF
                    }
                    //

                    if (eWy == EWay.L)
                    { eDo = eY.GRDL_TbVacuum; }
                    else
                    { eDo = eY.GRDR_TbVacuum; }

                    if(eWy == EWay.L && CIO.It.Get_Y(eOVac))
                    {
                        CIO.It.Set_Y(eOVac, false);
                    }
                    if(eWy == EWay.R && CIO.It.Get_Y(eOVac))
                    {
                        CIO.It.Set_Y(eOVac, false);
                    }

                    // 2020.10.22 JSKim St
                    if (CData.CurCompany == ECompany.JCET && CData.Opt.bDualPumpUse == true)
                    {
                        bPumpRunResult = CIO.It.Get_X(eIPRun) == false || CIO.It.Get_X(eIAddPRun) == false;
                    }
                    else
                    {
                        bPumpRunResult = CIO.It.Get_X(eIPRun) == false;
                    }
                    // 2020.10.22 JSKim Ed

                    // 2020.10.22 JSKim St
                    //if (!CIO.It.Get_X(eIPRun))
                    if (bPumpRunResult)
                    // 2020.10.22 JSKim Ed
                    {
                        CIO.It.Set_Y(eOPRun, true);

                        CTim mTOut = new CTim();
                        mTOut.Set_Delay(10000);
                        while (true)
                        {
                            if (CIO.It.Get_X(eIPRun))
                            { break; }
                            if (mTOut.Chk_Delay())
                            {
                                if (eWy == EWay.L)
                                { CErr.Show(eErr.PUMP_LEFT_RUN_TIMEOUT); return; }
                                else
                                { CErr.Show(eErr.PUMP_RIGHT_RUN_TIMEOUT); return; }
                            }
                            Application.DoEvents();
                        }
                        // 2020.11.07 lhs Start
                        if (CData.CurCompany == ECompany.JCET && CData.Opt.bDualPumpUse == true)
                        {
                            while (true)
                            {
                                if (CIO.It.Get_X(eIAddPRun))
                                { break; }
                                if (mTOut.Chk_Delay())
                                {
                                    if (eWy == EWay.L)
                                    { CErr.Show(eErr.ADD_PUMP_LEFT_RUN_TIMEOUT_ERROR); return; }
                                    else
                                    { CErr.Show(eErr.ADD_PUMP_RIGHT_RUN_TIMEOUT_ERROR); return; }
                                }
                                Application.DoEvents();
                            }
                        }
                        // 2020.11.07 lhs End

                        CIO.It.Set_Y(eDo, true);
                    }
                    CIO.It.Set_Y(eDo, true); 
                    break;

                case 2: //Vac Off
                    if (eWy == EWay.L)
                    { eDo = eY.GRDL_TbVacuum; }
                    else
                    { eDo = eY.GRDR_TbVacuum; }

                    CIO.It.Set_Y(eDo, false);
                    break;

                case 3: //Eject On

                    //201208 jhc : Manual 동작 상호 배제 (Vacuum/Eject vs. Table Water)
                    if (eWy == EWay.L)
                    {
                        CData.L_GRD.ActWater(false);  //L-Table Water OFF
                        CData.L_GRD.ActVacuum(false); //L-Table Vacuum OFF
                    }
                    else
                    {
                        CData.R_GRD.ActWater(false);  //R-Table Water OFF
                        CData.R_GRD.ActVacuum(false); //R-Table Vacuum OFF
                    }
                    //

                    if (eWy == EWay.L)
                    { eDo = eY.GRDL_TbEjector; }
                    else
                    { eDo = eY.GRDR_TbEjector; }

                    if(CIO.It.Get_Y(eOVac)) CIO.It.Set_Y(eOVac, false);

                    CIO.It.Set_Y(eDo, true);
                    break;

                case 4: //Eject Off
                    if (eWy == EWay.L)
                    { eDo = eY.GRDL_TbEjector; }
                    else
                    { eDo = eY.GRDR_TbEjector; }

                    CIO.It.Set_Y(eDo, false);
                    break;

                case 5:
                    if(CSQ_Main.It.m_iStat == EStatus.Manual)
                    {
                        CMsg.Show(eMsg.Error, "Error", "During Before Manual Run now, So wait finish run");
                        return;
                    }

                    if(CMsg.Show(eMsg.Warning, "Warring", "Do you want magazin pick up?") != DialogResult.OK) return;

                    if(iLoader == 0)
                    {
                        CSQ_Man.It.Seq = ESeq.ONL_Pick;
                    }
                    else 
                    {
                        //20200513 jym : 매거진 배출 위치 변경 추가
                        CSQ_Man.It.Seq = (CData.Opt.eEmitMgz == EMgzWay.Top) ? ESeq.OFL_TopPick : ESeq.OFL_BtmPick;
                    }

                    break;

                case 6:
                    if(CSQ_Main.It.m_iStat == EStatus.Manual)
                    {
                        CMsg.Show(eMsg.Error, "Error", "During Before Manual Run now, So wait finish run");
                        return;
                    }

                    if(CMsg.Show(eMsg.Warning, "Warring", "Do you want magazin place?") != DialogResult.OK) return;

                    if(iLoader == 0)
                    {
                        CSQ_Man.It.Seq = ESeq.ONL_Place;
                    }
                    else 
                    {
                        //20200513 jym : 매거진 배출 위치 변경 추가
                        CSQ_Man.It.Seq = (CData.Opt.eEmitMgz == EMgzWay.Top) ? ESeq.OFL_TopPlace : ESeq.OFL_BtmPlace;
                    }

                    break;

                case 7:
                    if(eWy == EWay.L)
                    {
                        double CurPos = CMot.It.Get_FP ((int)EAx.LeftGrindZone_Z);
                        if(CurPos > CData.SPos.dGRD_Z_Able[0] + 1)
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Left Z-Axis Pos is very Low. Move able Pos. Please");
                            return;
                        }
                    }
                    else
                    {
                        double CurPos = CMot.It.Get_FP ((int)EAx.RightGrindZone_Z);
                        if(CurPos > CData.SPos.dGRD_Z_Able[1] + 1)
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Right Z-Axis Pos is very Low. Move able Pos. Please");
                            return;
                        }
                    }

                    if(eWy == EWay.L) CMot.It.Mv_N((int)EAx.LeftGrindZone_Y , CData.SPos.dGRD_Y_DrsChange[0]);
                    else              CMot.It.Mv_N((int)EAx.RightGrindZone_Y, CData.SPos.dGRD_Y_DrsChange[1]);
                    break;

                case 8:
                    if(eWy == EWay.L)
                    {
                        double CurPos = CMot.It.Get_FP ((int)EAx.LeftGrindZone_Z);
                        if(CurPos > CData.SPos.dGRD_Z_Able[0] + 1)
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Left Z-Axis Pos is very Low. Move able Pos. Please");
                            return;
                        }
                    }
                    else
                    {
                        double CurPos = CMot.It.Get_FP ((int)EAx.RightGrindZone_Z);
                        if(CurPos > CData.SPos.dGRD_Z_Able[1] + 1)
                        {
                            CMsg.Show(eMsg.Warning, "Warning", "Right Z-Axis Pos is very Low. Move able Pos. Please");
                            return;
                        }
                    }

                    if(eWy == EWay.L) CMot.It.Mv_N((int)EAx.LeftGrindZone_Y , CData.SPos.dGRD_Y_Wait[0]);
                    else              CMot.It.Mv_N((int)EAx.RightGrindZone_Y, CData.SPos.dGRD_Y_Wait[1]);

                    break;
            }
        }

        //20191204 ghk_level
        private void _HideMenu()
        {
            ELv RetLv = CData.Lev;
            panel2.Visible = (int)RetLv >= CData.Opt.iLvOpDrsPos;
            panel1.Visible = (int)RetLv >= CData.Opt.iLvOpDrsPos;
        }
    }
}
