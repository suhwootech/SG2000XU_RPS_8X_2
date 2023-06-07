using System;
using System.IO;
using System.Text;
using System.Windows.Forms;



/// <summary>
/// 
///  Multi-LOT 기능 사용중에 각 LOT가 종료될 때 작업자에게 LOT 종료 사실을 알려주도록 한다.
///  Buzzer가 울리며, 작업자가 '확인' 버튼을 누르면 Bzzer가 Off 되고, 완료된 LOT 정보는 삭제되게 된다.
///  
/// </summary>
namespace SG2000X
{
    public partial class CDlgLotCloseMsg : Form
    {
        public CDlgLotCloseMsg()
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

            // 종료 처리된 LOT 정보를 가져온다.
            TLotInfo rInfo = null;
            StringBuilder sbMsg = new StringBuilder();

            // rInfo = CData.LotMgr.GetUn

            // 완료된 LOT 정보를 가져온다..... 
            // 동시에 여러개의 LOT이 완료된다면 ? 그럴리도 없고, 그렇다하더라도 대표로 1개만 보여주고 데이터처리는 모두 해주면 된다.
            if ( CData.LotMgr.GetLotInfoName(CData.LotMgr.LoadingLotName, ref rInfo))
            {
                sbMsg.AppendLine("        [  LOT Close Confirm  ] (Multi-LOT)");
                sbMsg.AppendLine("");
                sbMsg.AppendLine($"LOT Name     : {rInfo.sLotName}");
                sbMsg.AppendLine($"Device Name  : {CData.Dev.sName}");
                sbMsg.AppendLine($"Magzine Qty  : {rInfo.nInMgzCnt} of {rInfo.iTotalMgz}");      // 투입된 Magazine 수량
                sbMsg.AppendLine($"Input Qty    : {rInfo.iTInCnt} of {rInfo.iTotalStrip}");      // 투입된 Strip 수량
                sbMsg.AppendLine("");

                sbMsg.AppendLine(CData.LotMgr.UserConfirmMsg);
                sbMsg.AppendLine("");

                sbMsg.AppendLine("Do you want to stop the input of the LOT?");
                sbMsg.AppendLine("");

                lblMsg.Text = sbMsg.ToString();            // 조립된 문자열을 대입시켜준다.
            }
            else
            {
                sbMsg.AppendLine("        [  LOT Close Confirm  ] (Multi-LOT)");
                sbMsg.AppendLine("");
                sbMsg.AppendLine($"LOT Name     : {CData.LotMgr.LoadingLotName}");
                sbMsg.AppendLine("");

                sbMsg.AppendLine(CData.LotMgr.UserConfirmMsg);
                sbMsg.AppendLine("");

                sbMsg.AppendLine("Do you want to stop the input of the LOT?");
                sbMsg.AppendLine("");

                lblMsg.Text = sbMsg.ToString();
            }
        }


        //// 작업자가 확인을 하였다.
        //private void btnConfirm_Click(object sender, EventArgs e)
        //{
        //    DialogResult = DialogResult.OK;
        //    Close();

        //    // Buzzer가 OFf 되도록 Flag를 설정해준다.
        //    CData.bLotCompleteBuzzer = false;
        //}


        //// 버저만 Off 시키도록 한다.
        //private void btnBuzzerOff_Click(object sender, EventArgs e)
        //{
        //    // Buzzer가 OFf 되도록 Flag를 설정해준다.
        //    CData.bLotCompleteBuzzer = false;
        //}


        // 폼이 닫히려 한다.
        private void CDlgLotEndMsg_FormClosing(object sender, FormClosingEventArgs e)
        {
            // CData.bLotCompleteBuzzer = false;
            // 정상적인 선택을 마쳤다.
            if ( (CData.LotMgr.UserConfirmReply == ELotUserConfirm.eReplyYes) || (CData.LotMgr.UserConfirmReply == ELotUserConfirm.eReplyNo))
            {
                // 정상적인 선택
            }
            else
            {
                // 비정상 선택 종료인경우 YES로 대체한다.
                CData.LotMgr.UserConfirmReply = ELotUserConfirm.eReplyYes;          // YES로 응답을 지정한다.
            }

            // Buzzer가 OFf 되도록 Flag를 설정해준다.
            CData.bLotCompleteBuzzer = false;
            e.Cancel = false;
        }


        // 투입을 중지하고 LOT을 종료시킨다.
        private void btnYes_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            CData.LotMgr.UserConfirmReply = ELotUserConfirm.eReplyYes;          // YES로 응답을 지정한다.

            // Buzzer가 OFf 되도록 Flag를 설정해준다.
            CData.bLotCompleteBuzzer = false;

            Close();
        }

        // LOT 투입을 유지한다.
        private void btnNo_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            CData.LotMgr.UserConfirmReply = ELotUserConfirm.eReplyNo;          // NO로 응답을 지정한다.

            // Buzzer가 OFf 되도록 Flag를 설정해준다.
            CData.bLotCompleteBuzzer = false;

            Close();
        }
    }
}
