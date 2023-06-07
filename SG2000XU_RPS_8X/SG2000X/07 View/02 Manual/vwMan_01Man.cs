using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace SG2000X
{
    public partial class vwMan_01Man : UserControl
    {
        /// <summary>
        /// 화면에 정보 업데이트 타이머
        /// </summary>
        private Timer m_tmUpdt;

        public vwMan_01Man()
        {
            if      ((int)ELang.English == CData.Opt.iSelLan)   {   System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");     }
            else if ((int)ELang.Korea   == CData.Opt.iSelLan)   {   System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ko-KR");     }
            else if ((int)ELang.China   == CData.Opt.iSelLan)   {   System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("zh-Hans");   }
            
            InitializeComponent();

            m_tmUpdt = new Timer();
            m_tmUpdt.Interval = 50;
            m_tmUpdt.Tick += _M_tmUpdt_Tick;

            btn_SrvOn.Tag   = ESeq.All_Servo_On;
            btn_SrvOff.Tag  = ESeq.All_Servo_Off;
            btn_AllH.Tag    = ESeq.All_Home;
            btn_Idle.Tag    = ESeq.Warm_Up;

            if(CData.CurCompany != ECompany.SCK && CData.CurCompany != ECompany.JSCK)
            {
                pnl_AutoWarm.Visible = false;
            }
        }

        #region Private method
        private void _M_tmUpdt_Tick(object sender, EventArgs e)
        {
            if (CSQ_Man.It.bBtnShow && (CSQ_Main.It.m_iStat == EStatus.Idle || CSQ_Main.It.m_iStat == EStatus.Stop || CSQ_Main.It.m_iStat == EStatus.Error))
            if (CSQ_Man.It.bBtnShow && (CSQ_Main.It.m_iStat == EStatus.Idle || CSQ_Main.It.m_iStat == EStatus.Stop) )
            {
                CSQ_Man.It.bBtnShow = true;
                _EnableBtn(true);
            }

            //20191104 ghk_doorlock
            if (CSQ_Main.It.m_iStat != EStatus.Auto_Running && CSQ_Main.It.m_iStat != EStatus.Manual && CSQ_Main.It.m_iStat != EStatus.Warm_Up && CSQ_Main.It.m_iStat != EStatus.All_Homing && CSQ_Main.It.m_iStat != EStatus.Error)
            {//200909 pjh : EStatus.Error 조건 추가
                //
                if (!CIO.It.Get_Y(eY.GRD_DoorLock))
                {
                    btn0_DoorLock.Enabled   = true;
                    btn0_DoorUnLock.Enabled = false;
                }
                else
                {
                    btn0_DoorLock.Enabled   = false;
                    btn0_DoorUnLock.Enabled = true;
                }
                //
            }
            else
            {
                btn0_DoorLock.Enabled   = false;
                btn0_DoorUnLock.Enabled = false;
            }
            //

            ckb_RLamp.Checked  = CIO.It.Get_Y(eY.SYS_RoomLamp);
            ckb_FWater.Checked = CIO.It.Get_Y(eY.GRD_FrontWater);

            if (CSQ_Man.It.Seq == ESeq.Warm_Up)
            {
                if (CData.Warm >= DateTime.Now)
                {
                    TimeSpan tSpan = CData.Warm.Subtract(DateTime.Now);
                    lbl_IdleTime.Text = tSpan.Minutes.ToString("00") + " : " + tSpan.Seconds.ToString("00");
                }
            }
            else
            {
                lbl_IdleTime.Text = CData.Opt.iWmUT.ToString("00") + " : 00";
            }

            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK || CData.CurCompany == ECompany.JCET)
            { 
                lbl_AutoWarm.Text = CData.AutoWarmSpan.ToString(@"hh\:mm\:ss"); 
            }

            //190624 ksg : HomeDone 보는거 바꿈                 
            bool Onl_X = Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.OnLoader_X       ));
            bool Onl_Y = Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.OnLoader_Y       ));
            bool Onl_Z = Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.OnLoader_Z       ));
            bool Inr_X = Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.OnLoader_X       ));
            bool Inr_Y = Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.OnLoader_Y       ));
            bool Onp_X = Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.OnLoaderPicker_X ));
            bool Onp_Z = Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.OnLoaderPicker_Z ));
            bool Onp_Y = Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.OnLoaderPicker_Y ));
            bool LGZ_X = Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.LeftGrindZone_X  ));
            bool LGZ_Y = Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.LeftGrindZone_Y  ));
            bool LGZ_Z = Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.LeftGrindZone_Z  ));
            bool RGZ_X = Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.RightGrindZone_X ));
            bool RGZ_Y = Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.RightGrindZone_Y ));
            bool RGZ_Z = Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.RightGrindZone_Z ));
            bool Ofp_X = Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.OffLoaderPicker_X));
            bool Ofp_Z = Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.OffLoaderPicker_Z));
            bool Dry_X = Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.DryZone_X        ));
            bool Dry_Z = Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.DryZone_Z        ));
            bool Dry_R = Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.DryZone_Air      ));
            bool Ofl_X = Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.OffLoader_X      ));
            bool Ofl_Y = Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.OffLoader_Y      ));
            bool Ofl_Z = Convert.ToBoolean(CMot.It.Chk_HDI((int)EAx.OffLoader_Z      ));
            bool OnlHD = Onl_X && Onl_Y && Onl_Z;
            bool InrHD = Onl_X && Onl_Y         ;
            bool OnpHD = false;
            if (CDataOption.CurEqu == eEquType.Pikcer) OnpHD = Onp_X && Onp_Z && Onp_Y;
            else OnpHD = Onp_X && Onp_Z;
            bool LGZHD = LGZ_X && LGZ_Y && LGZ_Z ;
            bool RGZHD = RGZ_X && RGZ_Y && RGZ_Z ;
            bool OfpHD = Ofp_X && Ofp_Z          ;
            bool DryHD = Dry_X && Dry_Z && Dry_R ;
            bool OflHD = Ofl_X && Ofl_Y && Ofl_Z ;

            lbl_OnlHD.BackColor = GV.CR_X[Convert.ToInt32(OnlHD)];
            lbl_InrHD.BackColor = GV.CR_X[Convert.ToInt32(InrHD)];
            lbl_OnpHD.BackColor = GV.CR_X[Convert.ToInt32(OnpHD)];
            lbl_GrlHD.BackColor = GV.CR_X[Convert.ToInt32(LGZHD)];
            lbl_GrrHD.BackColor = GV.CR_X[Convert.ToInt32(RGZHD)];
            lbl_OfpHD.BackColor = GV.CR_X[Convert.ToInt32(OfpHD)];
            lbl_DryHD.BackColor = GV.CR_X[Convert.ToInt32(DryHD)];
            lbl_OflHD.BackColor = GV.CR_X[Convert.ToInt32(OflHD)];
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
        /// 버튼 Enable 변경
        /// </summary>
        /// <param name="bVal"></param>
        public void _EnableBtn(bool bVal)
        {
            //200319 ksg : 버튼 레벨에 맞게 수정함
            //btn_SrvOn  .Enabled = bVal;
            //btn_SrvOff .Enabled = bVal;
            if((int)CData.Lev >= CData.Opt.iLvAllSvOn ) btn_SrvOn  .Enabled = bVal;
            if((int)CData.Lev >= CData.Opt.iLvAllSvOff) btn_SrvOff .Enabled = bVal;
            btn_AllH   .Enabled = bVal;
            btn_Idle   .Enabled = bVal; 
            btn_AllWOff.Enabled = bVal;
        }

        private void _WarmUpTimeCnt()
        {
            TimeSpan sTemp = CSQ_Main.It.WarmUpTime();
            int      iTime = 0; //190521 ksg
            int      Hour  = sTemp.Hours;
            int      Day   = sTemp.Days ;
            if (Day < 1)
            {
                if      (Hour >= 2) { iTime = 30; }
                else if (Hour >= 1) { iTime = 15; }
                else                { iTime = 10; }
            }
            else
            {
                iTime = 30;
            }

            if (CData.CurCompany == ECompany.Qorvo      || CData.CurCompany == ECompany.Qorvo_DZ ||
                CData.CurCompany == ECompany.Qorvo_RT   || CData.CurCompany == ECompany.Qorvo_NC || // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                CData.CurCompany == ECompany.SST)
                CSQ_Main.It.SaveWarmUpTime(iTime); //191202 ksg :
        }

        /// <summary>
        /// Manual Manual view에 조작 로그 저장 함수
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
            //200211 ksg :
            /*
            if (CSQ_Man.It.bBtnShow || (CSQ_Main.It.m_iStat == eStatus.Idle || CSQ_Main.It.m_iStat == eStatus.Stop))
            { _EnableBtn(true); }
            else
            { _EnableBtn(false); }
            */
            if (CSQ_Man.It.bBtnShow && (CSQ_Main.It.m_iStat == EStatus.Idle || CSQ_Main.It.m_iStat == EStatus.Stop))
            { _EnableBtn(true); }
            else
            { _EnableBtn(false); }

            _WarmUpTimeCnt();
            // WarmUp 시간 표시
            txt_IdleTime.Text = CData.Opt.iWmUT.ToString();
            lbl_IdleTime.Text = string.Format("{0} : 00", CData.Opt.iWmUT);

            btn_Save    .Enabled = (int)CData.Lev >= CData.Opt.iLvWarmSet;
            txt_IdleTime.Enabled = (int)CData.Lev >= CData.Opt.iLvWarmSet;
            pnl_WmSave  .Visible = (int)CData.Lev >= CData.Opt.iLvWarmSet; //190717 ksg :

            //20191204 ghk_level
            btn_SrvOn.Enabled = (int)CData.Lev >= CData.Opt.iLvAllSvOn;
            btn_SrvOff.Enabled = (int)CData.Lev >= CData.Opt.iLvAllSvOff;
            //

            ckb_RLamp.Checked = CIO.It.Get_Y(eY.SYS_RoomLamp);

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

        private void btn_Cycle_Click(object sender, EventArgs e)
        {
            //190627 ksg : 
            _EnableBtn(false);
            CSQ_Man.It.bBtnShow = false;

            CData.bLastClick = true; //190509 ksg :
            Button mBtn = sender as Button;
            ESeq ESeq;
            Enum.TryParse(mBtn.Tag.ToString(), out ESeq);
            //190226 ksg :추가

            _SetLog(string.Format("N, {0} click", ESeq.ToString()));

                        //201118 pjh : 매뉴얼 동작 시 Servo On Check
            if (!CSQ_Main.It.Chk_AllSrvOn() && (ESeq != ESeq.All_Servo_On && ESeq != ESeq.All_Servo_Off))
            {
                CSQ_Man.It.bBtnShow = true; //190627 ksg :
                CMsg.Show(eMsg.Warning, "Warning", "First All Servo On Please");
                return;
            }
            //

            //190414 ksg :
            if (!((ESeq == ESeq.All_Home) || (ESeq == ESeq.All_Servo_On) || (ESeq == ESeq.All_Servo_Off) || (ESeq == ESeq.ONL_Home) ||
                 (ESeq == ESeq.INR_Home) || (ESeq == ESeq.ONP_Home) || (ESeq == ESeq.GRL_Home) || (ESeq == ESeq.GRR_Home) ||
                 (ESeq == ESeq.OFP_Home) || (ESeq == ESeq.DRY_Home) || (ESeq == ESeq.OFL_Home)))
            {
                if (!CSQ_Main.It.ChkAllHomeDone())
                {
                    CSQ_Man.It.bBtnShow = true; //190627 ksg :
                    CMsg.Show(eMsg.Warning, "Warning", "First All Home Please");
                    return;
                }
            }


            if (ESeq == ESeq.Warm_Up)
            {
                bool bStrip = false;
                if (CDataOption.Use2004U) { bStrip = CIO.It.Get_X(eX.GRDL_Unit_Vacuum_4U) || CIO.It.Get_X(eX.GRDR_Unit_Vacuum_4U) || CIO.It.Get_X(eX.GRDL_Carrier_Vacuum_4U) || CIO.It.Get_X(eX.GRDR_Carrier_Vacuum_4U); }
                else                      
                {
                    // 2022.06.10 lhs Start : (SCK+) 옵션 설정으로 자재가 있어도 WarmUp 수행
                    if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                    {
                        if (CData.Opt.bWarmUpWithStrip) {   bStrip = false; } // Strip이 없는 것으로 처리
                        else                            {   bStrip = CIO.It.Get_X(eX.GRDL_TbVaccum) || CIO.It.Get_X(eX.GRDR_TbVaccum); }
                    }
                    else                                {   bStrip = CIO.It.Get_X(eX.GRDL_TbVaccum) || CIO.It.Get_X(eX.GRDR_TbVaccum); } // 기존
                }
                if (bStrip)
                {
                    CSQ_Man.It.bBtnShow = true; //190627 ksg :
                    CMsg.Show(eMsg.Notice, "Notice", "Strip exist on the table. Please check");
                    return;
                }

                // 2020.12.31 lhs Start
                // Rear Cover가 열여 있으면 다시 한번 사용자 확인
                // 1. 휠노즐 장작된 설비 
                // 2. 뒷커버 열림 확인 
                bool bClosed = CIO.It.Get_X(eX.GRD_CoverRear);
                if (CDataOption.IsWhlCleaner && bClosed == false)
                {
                    string sMsg = "";
                    sMsg = "Grinding Rear-Cover Opened !!! \n\r";
                    sMsg += "\n\r";
                    sMsg += "Please check !!! \n\r";
                    sMsg += "\n\r";
                    sMsg += "Do you want continue?";

                    if (CMsg.Show(eMsg.Query, "Warning", sMsg) == DialogResult.Cancel)
                    {
                        CSQ_Man.It.bBtnShow = true;
                        return;
                    }
                }
                // 2020.12.31 lhs End
            }

            //190615 pjh:
            //Dry Stick Check
            if (ESeq == ESeq.All_Home)
            {
                if (CDataOption.Dryer == eDryer.Rotate)
                {
                    if(CIO.It.Get_X(eX.DRY_BtmTargetPos))
                    {
                        CMsg.Show(eMsg.Warning, "Warning", "Please Check Dry Bottom Stick");
                        return;
                    }
                }
            }

            // 도어 체크
            if (CData.CurCompany == ECompany.Qorvo      || CData.CurCompany == ECompany.Qorvo_DZ    ||
                CData.CurCompany == ECompany.Qorvo_RT   || CData.CurCompany == ECompany.Qorvo_NC    || // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                CData.CurCompany == ECompany.SCK        || CData.CurCompany == ECompany.JSCK        || // 2022.06.11 lhs     : SCK, JSCK 추가
                CData.CurCompany == ECompany.SST)
            {
                if(!CSQ_Main.It.CheckDoor())  // Qorvo.... SST, SCK,SCK+
                {
                    CSQ_Man.It.bBtnShow = true; 
                    CMsg.Show(eMsg.Warning, "Warning", "Door close Please");
                    return;
                }
            }            

            //ksg 추가
            if (CSQ_Main.It.m_iStat == EStatus.Manual)
            {
                CSQ_Man.It.bBtnShow = true; //190627 ksg :
                CMsg.Show(eMsg.Error, "Error", "During Before Manual Run now, So wait finish run");
                return;
            }
            else
            {
                //CData.L_GRD.m_tStat.bDrsR = true;
                CSQ_Man.It.Seq = ESeq;
                //190410 ksg :
                _EnableBtn(false);
                CSQ_Man.It.bBtnShow = false;
                // 201023 jym : 추가
                CData.IsATW = false;
            }
        }

        private void btn_Stop_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            //ksg : 스톱 함수가 여긴가?
            CSQ_Man.It.bBtnShow = true;
            if (CSQ_Main.It.m_iStat == EStatus.Manual)
            {
                CSQ_Man.It.bStop = true;
            }            
        }

        //190108 ksg : WarmUp Time 설정 WarmUp Start Btn 옆에서 설정 할 수 있도록 수정함.
        private void btn_Save_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            int.TryParse(txt_IdleTime.Text, out CData.Opt.iWmUT);

            string sPath = GV.PATH_CONFIG + "Option.cfg";
            string sSec = "";

            CIni mIni = new CIni(sPath);
            sSec = "Warm Up";
            mIni.Write(sSec, "Time", CData.Opt.iWmUT);            
        }

        //190228 ksg : All Water Off
        private void btn_AllWaterOff_Click(object sender, EventArgs e)
        {
            _SetLog("Click");
            CData.bLastClick = true; //190509 ksg :            
            CSQ_Man.It.WaterOnOff(false); //syc : All Water On/Off
            //200616 pjh : 
            CSQ_Man.It.StopWater();
            CMsg.Show(eMsg.Notice,"Notice","All Water Off OK");
            /*
            if (CIO.It.Get_Y(eY.GRDL_TbFlow     )) CIO.It.Set_Y(eY.GRDL_TbFlow     , false); //Talbe 
            if (CIO.It.Get_Y(eY.GRDL_TopClnFlow )) CIO.It.Set_Y(eY.GRDL_TopClnFlow , false); //Grind Left Top Cleaner
            if (CIO.It.Get_Y(eY.GRDL_SplWater   )) CIO.It.Set_Y(eY.GRDL_SplWater   , false); //Grind Left Spindle Water
            if (CIO.It.Get_Y(eY.GRDL_SplBtmWater)) CIO.It.Set_Y(eY.GRDL_SplBtmWater, false); //Grind Left Spindle Bottom Water
            //200515 myk : Wheel Cleaner Water 추가
            if (CIO.It.Get_Y(eY.GRDL_WhlCleaner )) CIO.It.Set_Y(eY.GRDL_WhlCleaner , false); //Grind Left Wheel Cleaner Water

            if (CIO.It.Get_Y(eY.GRDR_TbFlow     )) CIO.It.Set_Y(eY.GRDR_TbFlow     , false); //Talbe 
            if (CIO.It.Get_Y(eY.GRDR_TopClnFlow )) CIO.It.Set_Y(eY.GRDR_TopClnFlow , false); //Grind Right Top Cleaner
            if (CIO.It.Get_Y(eY.GRDR_SplWater   )) CIO.It.Set_Y(eY.GRDR_SplWater   , false); //Grind Right Spindle Water
            if (CIO.It.Get_Y(eY.GRDR_SplBtmWater)) CIO.It.Set_Y(eY.GRDR_SplBtmWater, false); //Grind Right Spindle Bottom Water
            //200515 myk : Wheel Cleaner Water 추가
            if (CIO.It.Get_Y(eY.GRDR_WhlCleaner)) CIO.It.Set_Y(eY.GRDR_WhlCleaner, false); //Grind Left Wheel Cleaner Water
            //190430 ksg :
            if (CIO.It.Get_Y(eY.DRY_ClnBtmFlow)) CIO.It.Set_Y(eY.DRY_ClnBtmFlow, false);//Y6F Btm clean Water
            //191217 syc_water knife off
            CIO.It.Set_Y(eY.GRDL_TopWaterKnife, false);
            CIO.It.Set_Y(eY.GRDR_TopWaterKnife, false);          
            */  
        }

        private void ckb_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox mCkb = sender as CheckBox;
            int iTag = int.Parse(mCkb.Tag.ToString());

            switch (iTag)
            {
                case 0:    // Lamp
                    {
                        if (mCkb.Checked)
                            mCkb.Text = CLang.It.GetLanguage("ROOM LAMP") + "\r\n" + CLang.It.GetLanguage("ON");
                        else
                            mCkb.Text = CLang.It.GetLanguage("ROOM LAMP") + "\r\n" + CLang.It.GetLanguage("OFF");
                        break;
                    }

                case 1:    // Front water
                    {
                        if (mCkb.Checked)
                            mCkb.Text = CLang.It.GetLanguage("FRONT") + "\r\n" + CLang.It.GetLanguage("WATER") + "\r\n" + CLang.It.GetLanguage("ON");
                        else
                            mCkb.Text = CLang.It.GetLanguage("FRONT") + "\r\n" + CLang.It.GetLanguage("WATER") + "\r\n" + CLang.It.GetLanguage("OFF");
                        break;
                    }
            }
        }

        private void ckb_Click(object sender, EventArgs e)
        {
            CheckBox mCkb = sender as CheckBox;
            int iTag = int.Parse(mCkb.Tag.ToString());
            _SetLog("Click, Tag:" + iTag);

            switch (iTag)
            {
                case 0:    // Lamp
                    CIO.It.Set_Y(eY.SYS_RoomLamp, mCkb.Checked);
                    break;

                case 1:    // Front water
                    CIO.It.Set_Y(eY.GRD_FrontWater, mCkb.Checked);
                    break;
            }
        }

        private void btn0_DoorLock_Click(object sender, EventArgs e)
        {
            btn0_DoorLock.Enabled = false;
            btn0_DoorUnLock.Enabled = true;

            // 2020.10.20 JSKim St
            if (CData.Opt.bQcUse)
            {
                //if (CTcpIp.It.IsConnect() && CTcpIp.It.bIsConnect)
                if(CGxSocket.It.IsConnected())
                {
                    if (CSQ_Main.It.m_bCheckLock != true || CSQ_Main.It.m_bFirstLock == false)
                    {
                        //CTcpIp.It.SendDoorLock(true);
                        CGxSocket.It.SendMessage("DoorLock,On");
                        CSQ_Main.It.m_bCheckLock = true;
                        CSQ_Main.It.m_bFirstLock = true;
                    }
                }
            }
            // 2020.10.20 JSKim Ed

            CIO.It.Set_Y(eY.ONL_DoorLock, true);
            CIO.It.Set_Y(eY.OFFL_DoorLock, true);
            CIO.It.Set_Y(eY.GRD_DoorLock, true);
            CSQ_Man.It.m_bDoor = true;
        }

        private void btn0_DoorUnLock_Click(object sender, EventArgs e)
        {
            btn0_DoorLock.Enabled = true;
            btn0_DoorUnLock.Enabled = false;

            // 2020.10.20 JSKim St
            if (CData.Opt.bQcUse)
            {
                //if (CTcpIp.It.IsConnect() && CTcpIp.It.bIsConnect)
                if (CGxSocket.It.IsConnected())
                {
                    if (CSQ_Main.It.m_bCheckLock != false || CSQ_Main.It.m_bFirstLock == false)
                    {
                        //CTcpIp.It.SendDoorLock(false);
                        CGxSocket.It.SendMessage("DoorLock,Off");
                        CSQ_Main.It.m_bCheckLock = false;
                        CSQ_Main.It.m_bFirstLock = true;
                    }
                }
            }
            // 2020.10.20 JSKim Ed

            CIO.It.Set_Y(eY.ONL_DoorLock, false);
            CIO.It.Set_Y(eY.OFFL_DoorLock, false);
            CIO.It.Set_Y(eY.GRD_DoorLock, false);
            CSQ_Man.It.m_bDoor = false;
        }
    }
}
