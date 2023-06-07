using System;
using System.IO;
using System.Windows.Forms;

namespace SG2000X
{
    public partial class frmMsgChange : Form
    {
        int m_SelectNum    = 0;
        int m_SelectNumDrs = 0;
        public frmMsgChange(eMsg iMsg, string sTitle, string sMsg)
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
            
            if(CData.WhlChgSeq.bStart && CData.WhlChgSeq.iStep == 1) pnl_WhlChange   .Visible = true ;
            else                                                     pnl_WhlChange   .Visible = false;
            if(CData.DrsChgSeq.bStart && CData.DrsChgSeq.iStep == 1) pnl_DrsChange   .Visible = true ;
            else                                                     pnl_DrsChange   .Visible = false;
            if(CSQ_Main.It.m_bWhlLimitArm                          ) pnl_SelectJob   .Visible = true ;
            else                                                     pnl_SelectJob   .Visible = false;  
            if(CSQ_Main.It.m_bDrsLimitArm                          ) pnl_SelectJobDrs.Visible = true ;
            else                                                     pnl_SelectJobDrs.Visible = false;  

            if (iMsg == eMsg.Change)
            {
                BackColor       = System.Drawing.Color.Firebrick;
                //190711 ksg : 업체 요청으로 수정 
                //if((CData.WhlChgSeq.iStep == 1) || (CData.DrsChgSeq.iStep == 1)) btn_Can.Visible = true;
                if((CData.WhlChgSeq.iStep == 1) || (CData.DrsChgSeq.iStep == 1))
                {
                    btn_Back.Visible = false;
                    btn_Can.Visible  = true ;
                }
                else if(CSQ_Main.It.m_bWhlLimitArm)                                                          
                { 
                    btn_Ok.Location  = btn_Can.Location;
                    btn_Can.Visible  = false;
                    btn_Back.Visible = false;
                    CSQ_Main.It.m_bWhlLimitArm = false;
                }
                else if(CSQ_Main.It.m_bDrsLimitArm)                                                          
                { 
                    btn_Ok.Location  = btn_Can.Location;
                    btn_Can.Visible  = false;
                    btn_Back.Visible = false;
                    CSQ_Main.It.m_bDrsLimitArm = false;
                }
                else
                {
                    btn_Can.Visible  = true;
                    if((CData.WhlChgSeq.iStep > 5) || (CData.WhlChgSeq.iStep == 5 && CData.WhlChgSeq.bWhlMeanS)) btn_Back.Visible = false;
                    else                                                                                         btn_Back.Visible = true ;
                }
            }

            // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
            //if(CData.CurCompany == ECompany.Qorvo || (CData.CurCompany == ECompany.Qorvo_DZ) || CData.CurCompany == ECompany.SST) //191202 ksg :
            if (CData.CurCompany == ECompany.Qorvo || CData.CurCompany == ECompany.Qorvo_DZ ||
                CData.CurCompany == ECompany.Qorvo_RT || CData.CurCompany == ECompany.Qorvo_NC ||
                CData.CurCompany == ECompany.SST)
            {
                iMsg   = eMsg.Error;
                //sTitle = "Notice";
            }

            lbl_Title.Text = sTitle;
            lbl_Msg  .Text = sMsg  ;

            //200730 jhc : Wheel/Dresser Limit Alaram 보류용 (ASE-Kr VOC)
            if(CData.CurCompany == ECompany.ASE_KR)
            {
                btn_JobConti.Text = "Wheel Change\r\n- After current loaded strip\r\n(LOADING STOP ON)";
                btn_JobContiDrs.Text = "Dresser Change\r\n- After current loaded strip\r\n(LOADING STOP ON)";
            }
            //
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            CData.bLastClick = true; //191111 ksg :
            if(m_SelectNum == 1)
            {
                //WheelChangeCancel();
                CData.WhlChgSeq.bStart = true ;
                CData.WhlChgSeq.iStep  = 1    ;
            }
            if(m_SelectNumDrs == 1)
            {
                CData.DrsChgSeq.bStart = true ;
                CData.DrsChgSeq.iStep  = 1    ;
            }

            //200730 jhc : Wheel/Dresser Limit Alaram 보류용 (ASE-Kr VOC)
            if (m_SelectNum == 1 || m_SelectNumDrs == 1)
            {
                CData.bWhlDrsLimitAlarmSkip = false; //Wheel/Dresser 교체 시도 시 Wheel/Dresser Limit Alarm 보류 해제
            }
            else if (m_SelectNum == 2 || m_SelectNumDrs == 2)
            {
                CData.bWhlDrsLimitAlarmSkip = true; // Wheel/Dresser Limit Alarm 보류
                CSQ_Main.It.m_bPause        = true; // LOADING STOP ON
            }
            //

            Close();
        }

        private void btn_Can_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            CData.bLastClick = true; //191111 ksg :
            if(CData.WhlChgSeq.bStart) WheelChangeCancel  ();
            if(CData.DrsChgSeq.bStart) DresserChangeCancel();
            Close();
        }

        private void btn_LWheel_Click(object sender, EventArgs e)
        {
            CData.bLastClick = true; //191111 ksg :
            if(CData.WhlChgSeq.bSltLeft) 
            {
                CData.WhlChgSeq.bSltLeft = false;
                btn_LWheel.BackColor = System.Drawing.Color.DarkGray;
            }
            else
            {
                CData.WhlChgSeq.bSltLeft = true ;
                btn_LWheel.BackColor = System.Drawing.Color.Red;
                if(CData.WhlChgSeq.bSltRight) 
                {
                    CData.WhlChgSeq.bSltRight = false;
                    btn_RWheel.BackColor = System.Drawing.Color.DarkGray;
                }
            }
        }

        private void btn_RWheel_Click(object sender, EventArgs e)
        {
            CData.bLastClick = true; //191111 ksg :
            if(CData.WhlChgSeq.bSltRight) 
            {
                CData.WhlChgSeq.bSltRight = false;
                btn_RWheel.BackColor = System.Drawing.Color.DarkGray;
            }
            else
            {
                CData.WhlChgSeq.bSltRight = true ;
                btn_RWheel.BackColor = System.Drawing.Color.Red;
                if(CData.WhlChgSeq.bSltLeft) 
                {
                    CData.WhlChgSeq.bSltLeft = false;
                    btn_LWheel.BackColor = System.Drawing.Color.DarkGray;
                }
            }
        }

        private void btn_LDrs_Click(object sender, EventArgs e)
        {
            CData.bLastClick = true; //191111 ksg :
            if(CData.DrsChgSeq.bSltLeft) 
            {
                CData.DrsChgSeq.bSltLeft = false;
                btn_LDrs.BackColor = System.Drawing.Color.DarkGray;
            }
            else
            {
                CData.DrsChgSeq.bSltLeft = true ;
                btn_LDrs.BackColor = System.Drawing.Color.Red;
                if(CData.DrsChgSeq.bSltRight) 
                {
                    CData.DrsChgSeq.bSltRight = false;
                    btn_RDrs.BackColor = System.Drawing.Color.DarkGray;
                }
                if(CData.DrsChgSeq.bSltDual) 
                {
                    CData.DrsChgSeq.bSltDual = false;
                    btn_DDrs.BackColor = System.Drawing.Color.DarkGray;
                }
            }
        }

        private void btn_RDrs_Click(object sender, EventArgs e)
        {
            CData.bLastClick = true; //191111 ksg :
            if(CData.DrsChgSeq.bSltRight) 
            {
                CData.DrsChgSeq.bSltRight = false;
                btn_RDrs.BackColor = System.Drawing.Color.DarkGray;
            }
            else
            {
                CData.DrsChgSeq.bSltRight = true ;
                btn_RDrs.BackColor = System.Drawing.Color.Red;
                if(CData.DrsChgSeq.bSltLeft) 
                {
                    CData.DrsChgSeq.bSltLeft = false;
                    btn_LDrs.BackColor = System.Drawing.Color.DarkGray;
                }
                if(CData.DrsChgSeq.bSltDual) 
                {
                    CData.DrsChgSeq.bSltDual = false;
                    btn_DDrs.BackColor = System.Drawing.Color.DarkGray;
                }
            }
        }

        private void btn_DDrs_Click(object sender, EventArgs e)
        {
            CData.bLastClick = true; //191111 ksg :
            if(CData.DrsChgSeq.bSltDual) 
            {
                CData.DrsChgSeq.bSltDual = false;
                btn_DDrs.BackColor = System.Drawing.Color.DarkGray;
            }
            else
            {
                CData.DrsChgSeq.bSltDual = true ;
                btn_DDrs.BackColor = System.Drawing.Color.Red;
                if(CData.DrsChgSeq.bSltLeft) 
                {
                    CData.DrsChgSeq.bSltLeft = false;
                    btn_LDrs.BackColor = System.Drawing.Color.DarkGray;
                }
                if(CData.DrsChgSeq.bSltRight) 
                {
                    CData.DrsChgSeq.bSltRight = false;
                    btn_RDrs.BackColor = System.Drawing.Color.DarkGray;
                }
            }
        }

        private void btn_Back_Click(object sender, EventArgs e)
        {
            CData.bLastClick = true; //191111 ksg :
            if(CData.WhlChgSeq.bStart)
            {
                switch(CData.WhlChgSeq.iStep)
                {
                    case 2:
                        {
                            CData.WhlChgSeq.bStartShow      = false;
                            CData.WhlChgSeq.bSltLeft        = false;
                            CData.WhlChgSeq.bSltRight       = false;
                            CData.WhlChgSeq.bLWhlChangeShow = false;
                            CData.WhlChgSeq.bWhlSelShow     = false;
                            CData.WhlChgSeq.iStep = 1;
                            Close();
                            break;
                        }
                        
                    case 3:
                        {
                            CData.WhlChgSeq.bWhlSelShow  = false;
                            CData.WhlChgSeq.bWhlSelFShow = false;
                            CData.WhlChgSeq.bSltWhlFile  = false;
                            CData.WhlChgSeq.iStep        = 2;
                            Close();
                            break;
                        }
                    case 4:
                        {
                            CData.WhlChgSeq.bWhlSelFShow    = false;
                            CData.WhlChgSeq.bLWhlChangeShow = false;
                            CData.WhlChgSeq.bRWhlChangeShow = false;
                            CData.WhlChgSeq.bLWhlJigCheck   = false;
                            CData.WhlChgSeq.bRWhlJigCheck   = false;
                            CData.WhlChgSeq.iStep           = 3;
                            Close();
                            break;
                        }
                    case 5:
                        {
                            CData.WhlChgSeq.bLWhlChangeShow = false;
                            CData.WhlChgSeq.bRWhlChangeShow = false;
                            CData.WhlChgSeq.bWhlMeanShow    = false;
                            CData.WhlChgSeq.bWhlMeanS       = false;
                            CData.WhlChgSeq.bWhlMeanF       = false;
                            CData.WhlChgSeq.iStep           = 4;
                            Close();
                            break;
                        }
                    case 6:
                        {
                            CData.WhlChgSeq.bWhlMeanShow  = false;
                            CData.WhlChgSeq.bWhlMeanS     = false;
                            CData.WhlChgSeq.bWhlMeanF     = false;
                            CData.WhlChgSeq.bWhlDressShow = false;
                            CData.WhlChgSeq.bWhlDressS    = false;
                            CData.WhlChgSeq.bWhlMeanShow  = false;
                            CData.WhlChgSeq.bWhlDressF    = false;
                            CData.WhlChgSeq.iStep         = 5;
                            Close();
                            break;
                        }
                    case 7:
                        {
                            CData.WhlChgSeq.bWhlDressShow = false;
                            CData.WhlChgSeq.bWhlDressS    = false;
                            CData.WhlChgSeq.bWhlMeanShow  = false;
                            CData.WhlChgSeq.bWhlDressF    = false;
                            CData.WhlChgSeq.bWhlCompShow  = false;
                            CData.WhlChgSeq.iStep         = 6;
                            Close();
                            break;
                        }
                        
                }
            }
            else if(CData.DrsChgSeq.bStart)
            {
                switch(CData.DrsChgSeq.iStep)
                {
                    case 2:
                        {
                            CData.DrsChgSeq.bStartShow      = false; 
                            CData.DrsChgSeq.bSltDual        = false; 
                            CData.DrsChgSeq.bDDrsChangeShow = false; 
                            CData.DrsChgSeq.bSltLeft        = false; 
                            CData.DrsChgSeq.bLDrsChangeShow = false; 
                            CData.DrsChgSeq.bSltRight       = false; 
                            CData.DrsChgSeq.bRDrsChangeShow = false; 
                            CData.DrsChgSeq.iStep           = 1;
                            Close();
                            break;
                        }
                    case 3:
                        {
                            CData.DrsChgSeq.bDDrsChangeShow = false; 
                            CData.DrsChgSeq.bLDrsChangeShow = false; 
                            CData.DrsChgSeq.bRDrsChangeShow = false; 
                            CData.DrsChgSeq.bDrsMeanShow    = false; 
                            CData.DrsChgSeq.bDrsMeanS       = false; 
                            CData.DrsChgSeq.bDrsMeanF       = false; 
                            CData.DrsChgSeq.iStep           = 2;
                            Close();
                            break;
                        }
                    case 4:
                        {
                            CData.DrsChgSeq.bDrsMeanShow = false; 
                            CData.DrsChgSeq.bDrsMeanS    = false; 
                            CData.DrsChgSeq.bDrsMeanShow = false; 
                            CData.DrsChgSeq.bDrsMeanS    = false; 
                            CData.DrsChgSeq.bDrsMeanF    = false; 
                            CData.DrsChgSeq.iStep        = 3;
                            Close();
                            break;
                        }
                    case 5:
                        {
                            CData.DrsChgSeq.bDDrsChangeShow = false; 
                            CData.DrsChgSeq.bLDrsChangeShow = false; 
                            CData.DrsChgSeq.bRDrsChangeShow = false; 
                            CData.DrsChgSeq.bDrsMeanShow    = false; 
                            CData.DrsChgSeq.bDrsMeanDS      = false; 
                            CData.DrsChgSeq.bDrsMeanDF      = false;
                            CData.DrsChgSeq.iStep           = 2;
                            Close();
                            break;
                        }
                    case 6:
                        {
                            CData.DrsChgSeq.bDrsMeanShow = false; 
                            CData.DrsChgSeq.bDrsMeanDS   = false; 
                            CData.DrsChgSeq.bDrsMeanDF   = false; 
                            CData.DrsChgSeq.bDrsCompShow = false; 
                            CData.DrsChgSeq.iStep        = 5;
                            Close();
                            break;
                        }
                }
            }
        }

        public void WheelChangeCancel()
        {
            CData.bLastClick = true; //191111 ksg :
            //CData.WhlChgSeq.bStart          = true ;
            //CData.WhlChgSeq.iStep           = 1    ;
            CData.WhlChgSeq.bStart          = false;
            CData.WhlChgSeq.iStep           = 0    ;
            CData.WhlChgSeq.bStartShow      = false;
            CData.WhlChgSeq.bBtnHide        = false;
            CData.WhlChgSeq.bSltLeft        = false;
            CData.WhlChgSeq.bSltRight       = false;
            CData.WhlChgSeq.bSltWhlFile     = false;
            CData.WhlChgSeq.bWhlSelShow     = false;
            CData.WhlChgSeq.bWhlSelFShow    = false;
            CData.WhlChgSeq.bLWhlChangeShow = false;
            CData.WhlChgSeq.bRWhlChangeShow = false;
            CData.WhlChgSeq.bWhlMeanS       = false;
            CData.WhlChgSeq.bWhlMeanF       = false;
            CData.WhlChgSeq.bWhlMeanShow    = false;
            CData.WhlChgSeq.bWhlDressS      = false;
            CData.WhlChgSeq.bWhlDressF      = false;
            CData.WhlChgSeq.bWhlDressShow   = false;
            CData.WhlChgSeq.bWhlCompShow    = false;
        }
        public void DresserChangeCancel()
        {
            CData.bLastClick = true; //191111 ksg :
            //CData.DrsChgSeq.bStart          = true ;
            //CData.DrsChgSeq.iStep           = 1    ;
            CData.DrsChgSeq.bStart          = false;
            CData.DrsChgSeq.iStep           = 0    ;
            CData.DrsChgSeq.bSltLeft        = false;
            CData.DrsChgSeq.bSltRight       = false;
            CData.DrsChgSeq.bSltDual        = false;
            CData.DrsChgSeq.bStartShow      = false;
            CData.DrsChgSeq.bLDrsChangeShow = false;
            CData.DrsChgSeq.bRDrsChangeShow = false;
            CData.DrsChgSeq.bDDrsChangeShow = false;
            CData.DrsChgSeq.bDrsMeanS       = false;
            CData.DrsChgSeq.bDrsMeanF       = false;
            CData.DrsChgSeq.bDrsMeanDS      = false;
            CData.DrsChgSeq.bDrsMeanDF      = false;
            CData.DrsChgSeq.bDrsMeanShow    = false;
            CData.DrsChgSeq.bDrsCompShow    = false;
        }

        private void btn_JobWheel_Click(object sender, EventArgs e)
        {
            CData.bLastClick = true; //191111 ksg :
            m_SelectNum = 1;
            btn_JobWheel.BackColor = System.Drawing.Color.Red;
            btn_JobConti.BackColor = System.Drawing.Color.DarkGray;
        }

        private void btn_JobConti_Click(object sender, EventArgs e)
        {
            CData.bLastClick = true; //191111 ksg :
            m_SelectNum = 2;
            btn_JobWheel.BackColor = System.Drawing.Color.DarkGray;
            btn_JobConti.BackColor = System.Drawing.Color.Red;
        }

        private void btn_JobDrs_Click(object sender, EventArgs e)
        {
            CData.bLastClick = true; //191111 ksg :
            m_SelectNumDrs = 1;
            btn_JobDrs     .BackColor = System.Drawing.Color.Red;
            btn_JobContiDrs.BackColor = System.Drawing.Color.DarkGray;
        }

        private void btn_JobContiDrs_Click(object sender, EventArgs e)
        {
            CData.bLastClick = true; //191111 ksg :
            m_SelectNumDrs = 2;
            btn_JobDrs     .BackColor = System.Drawing.Color.Red;
            btn_JobContiDrs.BackColor = System.Drawing.Color.DarkGray;
        }
    }
}
