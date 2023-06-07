using System;
using System.Windows.Forms;
using System.IO;

namespace SG2000X
{
    public partial class vwMain_Key : UserControl
    {
        public vwMain_Key()
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
                btn_Buzz.Visible = false;

                // 2021-01-27, jhLee : 입력 글자수를 10글자로 제한하며, 숫자만 입력이 가능하게 만든다.
                txt_Key.MaxLength = 10;
                txt_Key.KeyPress += OnlyNumber_KeyPress;
            }
        }

        public void Set()
        {
            txt_Key.Text = "";
            lbl_Help.Text = "";
            CData.BuzzOff = false;
        }

        // 2021-01-27, jhLee : VOC 숫자만 입력되게 한다.
        private void OnlyNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            //숫자와 '.' 만 입력되도록 필터링
            //if (!(char.IsDigit(e.KeyChar) || (e.KeyChar == Convert.ToChar(Keys.Back)) || (e.KeyChar == Convert.ToChar('.'))))    //숫자와 백스페이스를 제외한 나머지를 바로 처리
            if (!(char.IsDigit(e.KeyChar) || (e.KeyChar == Convert.ToChar(Keys.Back))))//210804 pjh : 고객사 요청 사항으로 "." 도 입력되지 않게끔 수정
            {
                e.Handled = true;
            }

        }

        private void btn_Keyin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_Key.Text))
            {
                lbl_Help.Text = "Need input strip ID !!!";
                return;
            }

            // 2021-01-25, jhLee : Skyworks VOC, 입력된 글자수가 10자리가 아니라면 비정상 입력이므로 다시 입력을 요청한다.
            if (CData.CurCompany == ECompany.SkyWorks)
            {
                if (txt_Key.Text.Length != 10)          // 10글자가 아니라면 입력 오류이다.
                {
                    lbl_Help.Text = "Need input need 10 letters !!";
                    return;
                }
            }


            //if (CData.CurCompany == ECompany.ASE_K12)
            //{
            //    if (CheckBcrPass(sPass))
            //    {
            //        CData.Bcr.sBcr = txt_Bcr.Text;

            //        if (CDataOption.CurEqu == eEquType.Nomal)
            //        {//바코드 인레일에 있을 경우
            //            CSq_Inr.It.m_bBcrKeyIn = true;
            //        }
            //        else
            //        {//바코드 온로더피커에 잇을 경우
            //            CSq_OnP.It.m_bBcrKeyIn = true;
            //        }

            //        lbl_BcrMsg.Text = "";

            //        return;
            //    }
            //    else
            //    {
            //        lbl_BcrMsg.Text = "Wrong Passwaord";
            //        return;
            //    }
            //}
            ////else if(CData.CurCompany == eCompany.JSCK)
            //else if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK || CData.CurCompany == ECompany.JCET) //200121 ksg : , 200625 lks
            //{
            //    if (txt_BcrPass.Text == "")
            //    {
            //        lbl_BcrMsg.Text = "Need Input Bcr Id";
            //        return;
            //    }

            //    if (txt_Bcr.Text == txt_BcrPass.Text)
            //    {
            //        CData.Bcr.sBcr = txt_Bcr.Text;

            //        if (CDataOption.CurEqu == eEquType.Nomal)
            //        {//바코드 인레일에 있을 경우
            //            CSq_Inr.It.m_bBcrKeyIn = true;
            //        }
            //        else
            //        {//바코드 온로더피커에 잇을 경우
            //            CSq_OnP.It.m_bBcrKeyIn = true;
            //        }

            //        lbl_BcrMsg.Text = "";
            //    }
            //    else
            //    {
            //        lbl_BcrMsg.Text = "Different input Bcr";
            //        return;
            //    }
            //}
            //else
            {
                CData.Bcr.sBcr = txt_Key.Text;
                //20191204 ghk_bcrocr
                CData.Bcr.sOcr = txt_Key.Text;

                if (CDataOption.CurEqu == eEquType.Nomal)
                {//바코드 인레일에 있을 경우
                    CSq_Inr.It.m_bBcrKeyIn = true;
                    Manual_Strip_ID_Key_In();
                }
                else
                {//바코드 온로더피커에 잇을 경우
                    CSq_OnP.It.m_bBcrKeyIn = true;
                }
            }

            SaveKeyInLog();

            CData.BuzzOff = false;

            CData.VwKey.bSelc = true;
            CData.VwKey.bOk = true;
            CData.VwKey.bCan = false;
            CData.VwKey.bView = false;
        }

        private void btn_Err_Click(object sender, EventArgs e)
        {
            if (CDataOption.CurEqu == eEquType.Nomal)
            {//바코드 인레일에 있을 경우
                CSq_Inr.It.m_bBcrErr = true;
            }
            else
            {//바코드 온로더 피커에 있을 경우
                CSq_OnP.It.m_bBcrErr = true;
            }

            CData.VwMsg.bSelc = true;
            CData.VwMsg.bOk = false;
            CData.VwMsg.bCan = true;
            CData.VwMsg.bView = false;
        }

        /// <summary>
        /// Skyworks OCR 수동 입력에 대해 요청 건.
        /// SECS/GEM 사용 시 수동으로 OCR 입력 후에 Align 시퀸스로 변경
        ///  20201022 LCY
        /// </summary>
        private void Manual_Strip_ID_Key_In()
        {
            //_SetLog("Manual_Strip_ID_Key_In");
            // 201028 jym : 조건문이 틀려서 수정
            if (CDataOption.SecsUse == eSecsGem.NotUse)
            {
                //_SetLog("CDataOption.SecsUse != eSecsGem.NotUse return");
                return;
            }
            if (!CData.Parts[(int)EPart.INR].bExistStrip)
            {
                //_SetLog("!CData.Parts[(int)EPart.INR].bExistStrip return");
                return;
            }

            // 201028 jym : OCR 변수 바뀌어 수정
            //if (CData.GemForm != null) CData.JSCK_Gem_Data[3].sInRail_Strip_ID = CData.Parts[(int)EPart.INR].sBcr;
            if (CData.GemForm != null) CData.JSCK_Gem_Data[(int)EDataShift.INR_DF_MEAS/*3*/].sInRail_Strip_ID = CData.Bcr.sOcr;
            CData.Parts[(int)EPart.INR].iStat = ESeq.INR_Align;
            //_SetLog("CData.Parts[(int)EPart.INR].iStat = ESeq.INR_Align");
            //CData.Parts[(int)EPart.INR].iStat = (CData.Dev.bOriSkip) ? ESeq.INR_Align : ESeq.INR_CheckOri;
        }

        private void btn_Buzz_Click(object sender, EventArgs e)
        {
            CData.BuzzOff = true;
        }

        public void LoadImage()
        {//210707 pjh : OCR Key in 시 마지막 이미지 Key in 창에 Load
            string sPath = GV.PATH_ImageLoad + "bcr.bmp";
            string sUsePath = GV.PATH_ImageLoad + "2000Xbcr.bmp";

            if (!File.Exists(sUsePath))
            { File.Copy(sPath, sUsePath); }

            if (File.Exists(sUsePath))
            {
                File.Copy(sPath, sUsePath, true);
                picb_OCRImg.LoadAsync(sUsePath);
                picb_OCRImg.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            else
                return;
        }

        public void SaveKeyInLog()
        {
            string sLine = string.Empty;
            string sDay = string.Empty;

            DateTime dtNow = DateTime.Now;
            string sDir = GV.PATH_LOG + dtNow.Year.ToString("0000") + "\\" + dtNow.Month.ToString("00") + "\\" + dtNow.Day.ToString("00") + "\\KeyInLog\\";

            if (!Directory.Exists(sDir))
            {
                Directory.CreateDirectory(sDir.ToString());
            }

            sDay = DateTime.Now.ToString("[yyyyMMdd-HHmmss fff]");
            sLine = sDay + "\t";
            sLine += "[LOT NAME : ";
            sLine += CData.LotInfo.sLotName + "]" + "\t";
            sLine += "[KEY IN NUMBER : ";
            sLine += CData.Bcr.sOcr + "]";

            CLogManager Log = new CLogManager(sDir, null, null);
            
            Log.WriteLine(sLine);
        }
    }
}
