using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace SG2000X
{
    public partial class vwOpt_06Lvs : UserControl
    {
        public vwOpt_06Lvs()
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

            if (CData.CurCompany == ECompany.SkyWorks)
            {
                label258.Visible = false;
                cbLv_WhlChange.Visible = false;
                label264.Visible = false;
                cbLv_WhlDrsChange.Visible = false;
            }
        }

        #region Basic method
        /// <summary>
        /// View가 화면에 그려질때 실행해야 하는 함수
        /// </summary>
        public void Open()
        {
            Set();
            
            ViewUpdate();   // 2020.10.09 SungTae : Add
        }

        /// <summary>
        /// View가 화면에서 지워질때 실행해야 하는 함수
        /// </summary>
        public void Close()
        {
        }

        public void Set()
        {
            #region Auto
            cbLv_Manual.SelectedIndex = CData.Opt.iLvManual;
            cbLv_Device.SelectedIndex = CData.Opt.iLvDevice;
            cbLv_Wheel.SelectedIndex = CData.Opt.iLvWheel;
            cbLv_Spc.SelectedIndex = CData.Opt.iLvSpc;
            cbLv_Option.SelectedIndex = CData.Opt.iLvOption;
            cbLv_Util.SelectedIndex = CData.Opt.iLvUtil;
            cbLv_Exit.SelectedIndex = CData.Opt.iLvExit;
            #endregion

            #region Manual
            cbLv_WarmSet.SelectedIndex = CData.Opt.iLvWarmSet;
            cbLv_OnL.SelectedIndex = CData.Opt.iLvOnL;
            cbLv_Inr.SelectedIndex = CData.Opt.iLvInr;
            cbLv_Onp.SelectedIndex = CData.Opt.iLvOnp;
            cbLv_GrL.SelectedIndex = CData.Opt.iLvGrL;
            cbLv_Grd.SelectedIndex = CData.Opt.iLvGrd;
            cbLv_Grr.SelectedIndex = CData.Opt.iLvGrr;
            cbLv_Ofp.SelectedIndex = CData.Opt.iLvOfp;
            cbLv_Dry.SelectedIndex = CData.Opt.iLvDry;
            cbLv_OfL.SelectedIndex = CData.Opt.iLvOfL;
            //20191204 ghk_level
            cbLv_AllSvOn.SelectedIndex = CData.Opt.iLvAllSvOn;
            cbLv_AllSvOff.SelectedIndex = CData.Opt.iLvAllSvOff;
            //
            #endregion

            #region Device
            cbLv_GpNew.SelectedIndex = CData.Opt.iLvGpNew;
            cbLv_GpSaveAs.SelectedIndex = CData.Opt.iLvGpSaveAs;
            cbLv_GpDel.SelectedIndex = CData.Opt.iLvGpDel;
            cbLv_DvNew.SelectedIndex = CData.Opt.iLvDvNew;
            cbLv_DvSaveAs.SelectedIndex = CData.Opt.iLvDvSaveAs;
            cbLv_DvDel.SelectedIndex = CData.Opt.iLvDvDel;
            cbLv_DvLoad.SelectedIndex = CData.Opt.iLvDvLoad;
            cbLv_DvCurrent.SelectedIndex = CData.Opt.iLvDvCurrent;
            cbLv_DvSave.SelectedIndex = CData.Opt.iLvDvSave;
            //20191203 ghk_level
            cbLv_DvParaEnable.SelectedIndex = CData.Opt.iLvDvParaEnable;
            cbLv_DvPosView.SelectedIndex = CData.Opt.iLvDvPosView;
            //
            #endregion

            #region Wheel
            cbLv_WhlNew.SelectedIndex = CData.Opt.iLvWhlNew;
            cbLv_WhlSaveAs.SelectedIndex = CData.Opt.iLvWhlSaveAs;
            cbLv_WhlDel.SelectedIndex = CData.Opt.iLvWhlDel;
            cbLv_WhlSave.SelectedIndex = CData.Opt.iLvWhlSave;
            cbLv_WhlChange.SelectedIndex = CData.Opt.iLvWhlChange; //190717 ksg : 추가
            cbLv_WhlDrsChange.SelectedIndex = CData.Opt.iLvWhlDrsChange; //190717 ksg : 추가
            #endregion

            #region Spc
            cbLv_SpcGpSave.SelectedIndex = CData.Opt.iLvSpcGpSave;
            cbLv_SpcErrList.SelectedIndex = CData.Opt.iLvSpcErrList;
            cbLv_SpcErrHis.SelectedIndex = CData.Opt.iLvSpcErrHis;
            cbLv_SpcErrHisView.SelectedIndex = CData.Opt.iLvSpcErrHisView;
            cbLv_SpcErrHisSave.SelectedIndex = CData.Opt.iLvSpcErrHisSave;
            #endregion

            #region Option
            cbLv_OptSysPos.SelectedIndex    = CData.Opt.iLvOptSysPos;
            cbLv_OptTbGrd.SelectedIndex     = CData.Opt.iLvOptTbGrd;
            // 2020.10.09 SungTae Start : Add only Qorvo
            cbLv_OptLoader.SelectedIndex    = CData.Opt.iLvOptLoader;
            cbLv_OptRailDry.SelectedIndex   = CData.Opt.iLvOptRailDry;
            cbLv_OptPicker.SelectedIndex    = CData.Opt.iLvOptPicker;
            cbLv_OptGrind.SelectedIndex     = CData.Opt.iLvOptGrind;
            // 2020.10.09 SungTae End
            // 2021.07.15 lhs Start
            cbLv_OptGeneral.SelectedIndex   = CData.Opt.iLvOptGen;      
            cbLv_OptMnt.SelectedIndex       = CData.Opt.iLvOptMnt;      
            // 2021.07.15 lhs End
            #endregion

            #region Util
            cbLv_UtilMot.SelectedIndex = CData.Opt.iLvUtilMot;
            cbLv_UtilSpd.SelectedIndex = CData.Opt.iLvUtilSpd;
            cbLv_UtilIn.SelectedIndex = CData.Opt.iLvUtilIn;
            cbLv_UtilOut.SelectedIndex = CData.Opt.iLvUtilOut;
            cbLv_UtilPrb.SelectedIndex = CData.Opt.iLvUtilPrb;
            cbLv_UtilTw.SelectedIndex = CData.Opt.iLvUtilTw;
            cbLv_UtilBcr.SelectedIndex = CData.Opt.iLvUtilBcr;
            cbLv_UtilRepeat.SelectedIndex = CData.Opt.iLvUtilRepeat;
            #endregion

            #region OpManual
            //20191204 ghk_level
            cbLv_OpDrsPos.SelectedIndex = CData.Opt.iLvOpDrsPos;

            // 2020-10-21, jhLee : Skyworks VOC, 작업자 레벨에 따라 Magazine의 Strip 존재여부 편집 기능을 사용 권한을 지정한다.
            cbLv_OpStripExistEdit.SelectedIndex = CData.Opt.iLvOpStripExistEdit;
            #endregion
        }

        public void Get()
        {
            #region Auto
            CData.Opt.iLvManual = cbLv_Manual.SelectedIndex;
            CData.Opt.iLvDevice = cbLv_Device.SelectedIndex;
            CData.Opt.iLvWheel = cbLv_Wheel.SelectedIndex;
            CData.Opt.iLvSpc = cbLv_Spc.SelectedIndex;
            CData.Opt.iLvOption = cbLv_Option.SelectedIndex;
            CData.Opt.iLvUtil = cbLv_Util.SelectedIndex;
            CData.Opt.iLvExit = cbLv_Exit.SelectedIndex;
            #endregion

            #region Manual
            CData.Opt.iLvWarmSet = cbLv_WarmSet.SelectedIndex;
            CData.Opt.iLvOnL = cbLv_OnL.SelectedIndex;
            CData.Opt.iLvInr = cbLv_Inr.SelectedIndex;
            CData.Opt.iLvOnp = cbLv_Onp.SelectedIndex;
            CData.Opt.iLvGrL = cbLv_GrL.SelectedIndex;
            CData.Opt.iLvGrd = cbLv_Grd.SelectedIndex;
            CData.Opt.iLvGrr = cbLv_Grr.SelectedIndex;
            CData.Opt.iLvOfp = cbLv_Ofp.SelectedIndex;
            CData.Opt.iLvDry = cbLv_Dry.SelectedIndex;
            CData.Opt.iLvOfL = cbLv_OfL.SelectedIndex;
            //20191204 ghk_level
            CData.Opt.iLvAllSvOn = cbLv_AllSvOn.SelectedIndex;
            CData.Opt.iLvAllSvOff = cbLv_AllSvOff.SelectedIndex;
            //
            #endregion

            #region Device
            CData.Opt.iLvGpNew = cbLv_GpNew.SelectedIndex;
            CData.Opt.iLvGpSaveAs = cbLv_GpSaveAs.SelectedIndex;
            CData.Opt.iLvGpDel = cbLv_GpDel.SelectedIndex;
            CData.Opt.iLvDvNew = cbLv_DvNew.SelectedIndex;
            CData.Opt.iLvDvSaveAs = cbLv_DvSaveAs.SelectedIndex;
            CData.Opt.iLvDvDel = cbLv_DvDel.SelectedIndex;
            CData.Opt.iLvDvLoad = cbLv_DvLoad.SelectedIndex;
            CData.Opt.iLvDvCurrent = cbLv_DvCurrent.SelectedIndex;
            CData.Opt.iLvDvSave = cbLv_DvSave.SelectedIndex;
            //20191203 ghk_level
            CData.Opt.iLvDvParaEnable = cbLv_DvParaEnable.SelectedIndex;
            CData.Opt.iLvDvPosView = cbLv_DvPosView.SelectedIndex;
            //
            #endregion

            #region Wheel
            CData.Opt.iLvWhlNew = cbLv_WhlNew.SelectedIndex;
            CData.Opt.iLvWhlSaveAs = cbLv_WhlSaveAs.SelectedIndex;
            CData.Opt.iLvWhlDel = cbLv_WhlDel.SelectedIndex;
            CData.Opt.iLvWhlSave = cbLv_WhlSave.SelectedIndex;
            CData.Opt.iLvWhlChange = cbLv_WhlChange.SelectedIndex; //190717 ksg : 추가
            CData.Opt.iLvWhlDrsChange = cbLv_WhlDrsChange.SelectedIndex; //190717 ksg : 추가
            #endregion

            #region Spc
            CData.Opt.iLvSpcGpSave = cbLv_SpcGpSave.SelectedIndex;
            CData.Opt.iLvSpcErrList = cbLv_SpcErrList.SelectedIndex;
            CData.Opt.iLvSpcErrHis = cbLv_SpcErrHis.SelectedIndex;
            CData.Opt.iLvSpcErrHisView = cbLv_SpcErrHisView.SelectedIndex;
            CData.Opt.iLvSpcErrHisSave = cbLv_SpcErrHisSave.SelectedIndex;
            #endregion

            #region Option
            CData.Opt.iLvOptSysPos  = cbLv_OptSysPos.SelectedIndex;
            CData.Opt.iLvOptTbGrd   = cbLv_OptTbGrd.SelectedIndex;
            // 2020.10.09 SungTae Start : Add only Qorvo
            CData.Opt.iLvOptLoader  = cbLv_OptLoader.SelectedIndex;
            CData.Opt.iLvOptRailDry = cbLv_OptRailDry.SelectedIndex;
            CData.Opt.iLvOptPicker  = cbLv_OptPicker.SelectedIndex;
            CData.Opt.iLvOptGrind   = cbLv_OptGrind.SelectedIndex;
            // 2020.10.09 SungTae End
            // 2021.07.15 lhs Start
            CData.Opt.iLvOptGen     = cbLv_OptGeneral.SelectedIndex;           
            CData.Opt.iLvOptMnt     = cbLv_OptMnt.SelectedIndex;           
            // 2021.07.15 lhs End
            #endregion

            #region Util
            CData.Opt.iLvUtilMot = cbLv_UtilMot.SelectedIndex;
            CData.Opt.iLvUtilSpd = cbLv_UtilSpd.SelectedIndex;
            CData.Opt.iLvUtilIn = cbLv_UtilIn.SelectedIndex;
            CData.Opt.iLvUtilOut = cbLv_UtilOut.SelectedIndex;
            CData.Opt.iLvUtilPrb = cbLv_UtilPrb.SelectedIndex;
            CData.Opt.iLvUtilTw = cbLv_UtilTw.SelectedIndex;
            CData.Opt.iLvUtilBcr = cbLv_UtilBcr.SelectedIndex;
            CData.Opt.iLvUtilRepeat = cbLv_UtilRepeat.SelectedIndex;
            #endregion

            #region OpManual
            //20191204 ghk_level
            CData.Opt.iLvOpDrsPos = cbLv_OpDrsPos.SelectedIndex;

            // 2020-10-21, jhLee : Skyworks VOC, 작업자 레벨에 따라 Magazine의 Strip 존재여부 편집 기능을 사용 권한을 지정한다.
            CData.Opt.iLvOpStripExistEdit = cbLv_OpStripExistEdit.SelectedIndex;
            #endregion
        }

        // 2020.10.09 SungTae Start : Add only Qorvo
        public void ViewUpdate()
        {
            if (CData.CurCompany == ECompany.Qorvo      || CData.CurCompany == ECompany.Qorvo_DZ ||
                CData.CurCompany == ECompany.Qorvo_RT   || CData.CurCompany == ECompany.Qorvo_NC ||     // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                CData.CurCompany == ECompany.SCK        || CData.CurCompany == ECompany.JSCK )          // 2021.07.15 lhs : SCK, JSCK 추가 
            {
                label2.Visible = true;
                label3.Visible = true;
                label4.Visible = true;
                label5.Visible = true;

                cbLv_OptLoader.Visible  = true;
                cbLv_OptRailDry.Visible = true;
                cbLv_OptPicker.Visible  = true;
                cbLv_OptGrind.Visible   = true;

                // 2021.07.15 lhs Start
                if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                {
                    label7.Visible          = true; // General label
                    label6.Visible          = true; // Maintenance label
                    cbLv_OptGeneral.Visible = true; // General
                    cbLv_OptMnt.Visible     = true; // Maintenance    
				}
                // 2021.07.15 lhs End
            }
            else
            {
                label2.Visible = false;
                label3.Visible = false;
                label4.Visible = false;
                label5.Visible = false;
                label7.Visible = false;  // General label        // 2021.07.15 lhs      
                label6.Visible = false;  // Maintenance label    // 2021.07.15 lhs

                cbLv_OptLoader.Visible  = false;
                cbLv_OptRailDry.Visible = false;
                cbLv_OptPicker.Visible  = false;
                cbLv_OptGrind.Visible   = false;
                cbLv_OptGeneral.Visible = false; // General     // 2021.07.15 lhs
                cbLv_OptMnt.Visible     = false; // Maintenance // 2021.07.15 lhs
            }
        }
        // 2020.10.09 SungTae End

        /// <summary>
        /// 해당 View가 Dispose(종료)될 때 실행 됨 
        /// View의 Dispose함수 실행하면 자동으로 실행됨
        /// </summary>
        private void _Release()
        {
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

        #endregion
    }
}
