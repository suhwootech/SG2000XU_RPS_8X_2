using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace SG2000X
{
    public partial class vwDev_02Prm_8IV2 : UserControl
    {
        public int ONPX = (int)EAx.OnLoaderPicker_X;
        public int ONPY = (int)EAx.OnLoaderPicker_Y;
        public int ONPZ = (int)EAx.OnLoaderPicker_Z;
        public int OFPX = (int)EAx.OffLoaderPicker_X;
        public int OFPZ = (int)EAx.OffLoaderPicker_Z;

        public vwDev_02Prm_8IV2()
        {
            InitializeComponent();
        }
        public void Open()
        {
            Set();
        }
        public void Close()
        {

        }
        public void Get()
        {
            double dPos = 0;
            string sVal = "";

            //211015 syc : 2004U 기존 텍스트 파일 콤보박스로 변환
            sVal = cmb_IV2Onp1.Text;
            CData.Dev.sIV2_ONP1_Para = sVal;

            sVal = cmb_IV2Onp2.Text;
            CData.Dev.sIV2_ONP2_Para = sVal;

            sVal = cmb_IV2Ofp1.Text;
            CData.Dev.sIV2_OFP1_Para = sVal;

            sVal = cmb_IV2Ofp2.Text;
            CData.Dev.sIV2_OFP2_Para = sVal;

            sVal = cmb_IV2OfpCover.Text;
            CData.Dev.sIV2_OFPCover_Para = sVal;


            double.TryParse(txtS_OnP1X.Text, out dPos);
            CData.Dev.dIV2_ONP1_X = dPos;

            double.TryParse(txtS_OnP1Y.Text, out dPos);
            CData.Dev.dIV2_ONP1_Y = dPos;

            double.TryParse(txtS_OnP1Z.Text, out dPos);
            CData.Dev.dIV2_ONP1_Z = dPos;


            double.TryParse(txtS_OnP2X.Text, out dPos);
            CData.Dev.dIV2_ONP2_X = dPos;

            double.TryParse(txtS_OnP2Y.Text, out dPos);
            CData.Dev.dIV2_ONP2_Y = dPos;

            double.TryParse(txtS_OnP2Z.Text, out dPos);
            CData.Dev.dIV2_ONP2_Z = dPos;


            double.TryParse(txtS_OFP1X.Text, out dPos);
            CData.Dev.dIV2_OFP1_X = dPos;

            double.TryParse(txtS_OFP1Z.Text, out dPos);
            CData.Dev.dIV2_OFP1_Z = dPos;



            double.TryParse(txtS_OFP2X.Text, out dPos);
            CData.Dev.dIV2_OFP2_X = dPos;

            double.TryParse(txtS_OFP1Z.Text, out dPos);
            CData.Dev.dIV2_OFP2_Z = dPos;


            double.TryParse(txtS_OFPCvX.Text, out dPos);
            CData.Dev.dIV2_OFPCover_X = dPos;

            double.TryParse(txtS_OFPCvZ.Text, out dPos);
            CData.Dev.dIV2_OFPCover_Z = dPos;

            CData.Dev.bIV2_ONP2_Use = ckb_ONP_2nd.Checked;
            CData.Dev.bIV2_OFP2_Use = ckb_OFP_2nd.Checked;
        }
        public void Set()
        {
            //211019 syc : 2004U 형식 바꾸기
            cmb_IV2Onp1.Text     = CData.Dev.sIV2_ONP1_Para;
            cmb_IV2Onp2.Text     = CData.Dev.sIV2_ONP2_Para;
            cmb_IV2Ofp1.Text     = CData.Dev.sIV2_OFP1_Para;
            cmb_IV2Ofp2.Text     = CData.Dev.sIV2_OFP2_Para;
            cmb_IV2OfpCover.Text = CData.Dev.sIV2_OFPCover_Para;

            txtS_OnP1X.Text = CData.Dev.dIV2_ONP1_X.ToString();
            txtS_OnP1Y.Text = CData.Dev.dIV2_ONP1_Y.ToString();
            txtS_OnP1Z.Text = CData.Dev.dIV2_ONP1_Z.ToString();

            txtS_OnP2X.Text = CData.Dev.dIV2_ONP2_X.ToString();
            txtS_OnP2Y.Text = CData.Dev.dIV2_ONP2_Y.ToString();
            txtS_OnP2Z.Text = CData.Dev.dIV2_ONP2_Z.ToString();

            txtS_OFP1X.Text = CData.Dev.dIV2_OFP1_X.ToString();
            txtS_OFP1Z.Text = CData.Dev.dIV2_OFP1_Z.ToString();

            txtS_OFP2X.Text = CData.Dev.dIV2_OFP2_X.ToString();
            txtS_OFP2Z.Text = CData.Dev.dIV2_OFP2_Z.ToString();

            txtS_OFPCvX.Text = CData.Dev.dIV2_OFPCover_X.ToString();
            txtS_OFPCvZ.Text = CData.Dev.dIV2_OFPCover_Z.ToString();

            ckb_ONP_2nd.Checked = CData.Dev.bIV2_ONP2_Use;
            ckb_OFP_2nd.Checked = CData.Dev.bIV2_OFP2_Use;
        }

        /// <summary>
        /// Get button 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Get_Click(object sender, EventArgs e)
        {
            Button mBtn = sender as Button;            
            string sTag = mBtn.Tag.ToString();
            int iAx = 0;

            // 다른방법 있는지 조언 구해보기 (급한거 아니니 나중에)
            if      (sTag == "txtS_OnP1X" || sTag == "txtS_OnP2X")                                                    { iAx = ONPX; }
            else if (sTag == "txtS_OnP1Y" || sTag == "txtS_OnP2Y")                                                    { iAx = ONPY; }
            else if (sTag == "txtS_OnP1Z" || sTag == "txtS_OnP2Z")                                                    { iAx = ONPZ; }
            else if (sTag == "txtS_OFP1X" || sTag == "txtS_OFP2X" || sTag == "txtS_OFPCvX" || sTag == "txtS_OFPCvPX") { iAx = OFPX; }
            else if (sTag == "txtS_OFP1Z" || sTag == "txtS_OFP2Z" || sTag == "txtS_OFPCvZ" || sTag == "txtS_OFPCvPZ") { iAx = OFPZ; }
            else return;            

            string sTxt = mBtn.Tag.ToString();
            mBtn.Parent.Controls[sTxt].Text = CMot.It.Get_FP(iAx).ToString();
        }

        /// <summary>
        /// Move button 클릭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Move_Click(object sender, EventArgs e)
        {
            //Auto Run, Manual 상태일 경우 동작 X
            if (CSQ_Main.It.m_iStat == EStatus.Auto_Running || CSQ_Main.It.m_iStat == EStatus.Manual) return;

            Button mBtn = sender as Button;
            string sTag = mBtn.Tag.ToString();
            int iAx = 0;
            int iRt = 0;
            double dPos = 0;

            if      (sTag == "txtS_OnP1X" || sTag == "txtS_OnP2X")                          { iAx = ONPX; }
            else if (sTag == "txtS_OnP1Y" || sTag == "txtS_OnP2Y")                          { iAx = ONPY; }
            else if (sTag == "txtS_OnP1Z" || sTag == "txtS_OnP2Z")                          { iAx = ONPZ; }
            else if (sTag == "txtS_OFP1X" || sTag == "txtS_OFP2X" || sTag == "txtS_OFPCvX") { iAx = OFPX; }
            else if (sTag == "txtS_OFP1Z" || sTag == "txtS_OFP2Z" || sTag == "txtS_OFPCvZ") { iAx = OFPZ; }
            else return;

            string sTxt = mBtn.Tag.ToString();
            if (!double.TryParse(mBtn.Parent.Controls[sTxt].Text, out dPos))
            {
                // 포지션 더블형 변환 에러
                // Error
            }
            else
            {
                if (CMsg.Show(eMsg.Warning, "Warning", "Can you move position -> " + dPos + "mm") == DialogResult.OK)
                {
                    iRt = CMot.It.Mv_N(iAx, dPos);
                    if (iRt != 0)
                    {
                        // Error
                    }
                    else
                    {
                        while (true)
                        {
                            if (CMot.It.Get_Mv(iAx, dPos))
                            { break; }
                            Application.DoEvents();
                        }
                    }
                }
            }
        }
    }
}
