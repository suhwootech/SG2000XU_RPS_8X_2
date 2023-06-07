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
    public partial class vwMain_Err : UserControl
    {
        private int nRaderErrorCode = 0; /// Max 2020103 : SCK+ Rader & Error Text Modify

        public vwMain_Err()
        {
            InitializeComponent();
        }

        public void SetErr(eErr eErr)
        {
            // 2022-08-23, jhLee : Huawei에서 프로그램 실행시 비정상 종료 발생, 디버깅 결과 아래 sNo를 수치형으로 변경하면서 예외 발생,
            //                      이를 방지하기 위한 코드로 변경하였다.

            // Error 지정 오류
            if (((int)eErr < 0) || ((int)eErr >= (int)eErr.ERR_MAXCOUNT))
            {
                CLog.Save_Log(eLog.None, eLog.OPL, $"vwMain_Err, eErr out of range : {(int)eErr}, {eErr}");
                return;
            }

            if (CData.ErrList[(int)eErr].sName == null)
            { return; }


            int nNo = 0;
            string sValue = CData.ErrList[(int)eErr].sNo;
            
            // 문자로 된 Error No를 수치형으로 변환
            if (!Int32.TryParse(sValue, out nNo))
            {
                CLog.Save_Log(eLog.None, eLog.OPL, $"vwMain_Err, {(int)eErr} ({eErr}) Number is missing {sValue}");
                return;
            }

            string sPath = "";
            FileInfo mFile;


            //old lbl_No.Text = "ERR" + Convert.ToInt32(CData.ErrList[(int)eErr].sNo).ToString("D3");
            lbl_No.Text = $"ERR{nNo:#000}";                     // 3자리 문자열로 변환
            lbl_Name.Text = CData.ErrList[(int)eErr].sName;
            txt_Action.Text = CData.ErrList[(int)eErr].sAction.Replace("*", "\r\n");
            //old sPath = GV.PATH_ERRIMG + Convert.ToInt32(CData.ErrList[(int)eErr].sNo).ToString("D3") + ".png";
            sPath = GV.PATH_ERRIMG + $"{nNo:#000}.png";     // 이미지 경로 지정

            /// <summary>
            /// Max 2020103 : SCK+ Rader & Error Text Modify
            /// </summary>
            pnl_CloseWithPws.Visible = false;
            btn_Close.Visible = true;
            btn_Close.BringToFront();

            // 2020.12.07 JSKim St
            //if (CData.ErrList[(int)eErr].iRaderUseCnt != 0)
            //{
            //    string sRaderCnt, sRaderSetCnt;
            //    int nRaderCnt, nRaderSetCnt;

            //    sRaderSetCnt = CData.ErrList[(int)eErr].iRaderUseCnt.ToString();
            //    nRaderSetCnt = Convert.ToInt16(sRaderSetCnt);
            //    sRaderCnt = CData.ErrList[(int)eErr].iRaderCounter.ToString();
            //    nRaderCnt = Convert.ToInt16(sRaderCnt);
            //    if (nRaderCnt >= nRaderSetCnt)
            //    {
            //        nRaderErrorCode = (int)eErr;
            //        lbl_RaderErrMsg.Text = " Rader Conter Over";
            //        pnl_CloseWithPws.Location = btn_Close.Location;
            //        pnl_CloseWithPws.Visible = true;
            //        pnl_CloseWithPws.BringToFront();
            //        btn_Close.Visible = false;
            //    }
            //}
            if (CData.ErrList[(int)eErr].bRadarUse == true && CData.ErrList[(int)eErr].iRadarOptionCnt != 0)
            {
                string sRaderCnt, sRaderSetCnt;
                int nRaderCnt, nRaderSetCnt;

                sRaderSetCnt = CData.ErrList[(int)eErr].iRadarOptionCnt.ToString();
                nRaderSetCnt = Convert.ToInt16(sRaderSetCnt);
                sRaderCnt = CData.ErrList[(int)eErr].iRadarErrorCnt.ToString();
                nRaderCnt = Convert.ToInt16(sRaderCnt);

                if (nRaderCnt >= nRaderSetCnt)
                {
                    nRaderErrorCode = (int)eErr;
                    lbl_RaderErrMsg.Text = " Radar Counter Over";
                    pnl_CloseWithPws.Location = btn_Close.Location;
                    pnl_CloseWithPws.Visible = true;
                    pnl_CloseWithPws.BringToFront();
                    btn_Close.Visible = false;
                }
            }
            // 2020.12.07 JSKim Ed

            mFile = new FileInfo(sPath);
            if (mFile.Exists)
            { pbx_Img.Load(sPath); }
            else
            { pbx_Img.Image = null; }
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            CData.ShowErrForm = false;
        }

        private void btn_Close_Pws_Click(object sender, EventArgs e)
        {
            _SetLog_Rader("Click Close Pws");

            CIni m_Ini;
            string sPath = GV.PATH_CONFIG + "PW.cfg";
            string sSec = "";
            string sKey = "";
            //string sData = "";
            string sEng = "";
            string sManger = "";

            m_Ini = new CIni(sPath);
            sSec = "PW";
            sKey = "TECHNICIAN";
            sEng = m_Ini.Read(sSec, sKey);
            sKey = "ENGINEER";
            sManger = m_Ini.Read(sSec, sKey);

            if (txt_RaderClearPws.Text == sEng || txt_RaderClearPws.Text == sManger)
            {
                _SetLog_Rader("Click Close Pws OK");
                // 2020.10.26 JSKim St
                txt_RaderClearPws.Text = "";
                // 2020.10.26 JSKim Ed

                // 2020.12.07 JSKim St
                //CData.ErrList[nRaderErrorCode].iRaderCounter = 0;
                //CErr.SaveErr();
                CData.ErrList[nRaderErrorCode].iRadarErrorCnt = 0;
                CErr.SaveRadar();
                // 2020.12.07 JSKim Ed

                CData.ShowErrForm = false;
            }
            else
            {
                _SetLog_Rader("Click Close Pws Fail");
                // 2020.10.26 JSKim St
                txt_RaderClearPws.Text = "";
                // 2020.10.26 JSKim Ed

                CMsg.Show(eMsg.Warning, "Warning", "Wrong Passwaord");
                return;
            }
        }

        /// <summary>
        /// Max 2020103 : SCK+ Rader & Error Text Modify
        /// </summary>
        private void _SetLog_Rader(string sMsg)
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            string sCls = this.Name;
            string sMth = sf.GetMethod().Name;

            CLog.Save_Log(eLog.None, eLog.OPL, string.Format("{0}.cs {1}() {2} Lv:{3}", sCls, sMth, sMsg, CData.Lev));
        }
    }
}
