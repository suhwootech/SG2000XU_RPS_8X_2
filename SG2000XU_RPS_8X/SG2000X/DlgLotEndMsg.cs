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
    public partial class CDlgLotEndMsg : Form
    {
        public CDlgLotEndMsg()
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

            // 2021.10.28 SungTae Start : [추가] 
            if(CData.CurCompany == ECompany.ASE_KR)
            {
                btnConfirm.Visible = false;
            }
            // 2021.10.28 SungTae End

            // 종료 처리된 LOT 정보를 가져온다.
            TLotInfo rInfo = null;

            // rInfo = CData.LotMgr.GetUn

            // 완료된 LOT 정보를 가져온다..... 
            // 동시에 여러개의 LOT이 완료된다면 ? 그럴리도 없고, 그렇다하더라도 대표로 1개만 보여주고 데이터처리는 모두 해주면 된다.
            if ( CData.LotMgr.GetStateLotInfo(ELotState.eComplete, ref rInfo))
            {
                StringBuilder sbMsg = new StringBuilder();
                TimeSpan tsDiff = rInfo.dtEnd - rInfo.dtOpen;                   // Open ~ End 까지의 소요시간

                sbMsg.AppendLine("        [  LOT END   ] (Multi-LOT)");
                sbMsg.AppendLine("");
                sbMsg.AppendLine($"LOT Name     : {rInfo.sLotName}");
                sbMsg.AppendLine($"Device Name  : {CData.Dev.sName}");
                sbMsg.AppendLine($"Magzine Qty  : {rInfo.nInMgzCnt}");      // 투입된 Magazine 수량
                sbMsg.AppendLine($"Input Qty    : {rInfo.iTInCnt}");          // 투입된 Strip 수량
                sbMsg.AppendLine($"Output Qty   : {rInfo.iTOutCnt}");        // 배출된 Strip 수량
                sbMsg.AppendLine($"Begin Time   : {rInfo.dtOpen.ToString("HH:mm:ss")}");    // LOT (투입)시작 시간    
                sbMsg.AppendLine($"Finish Time  : {rInfo.dtEnd.ToString("HH:mm:ss")}");    // LOT 완료 시간    
                sbMsg.AppendLine($"Elapsed Time : {tsDiff.Hours}:{tsDiff.Minutes}:{tsDiff.Seconds}");    // 경과시간 표시

                lblMsg.Text = sbMsg.ToString();            // 조립된 문자열을 대입시켜준다.
            }
            else
            {
                lblMsg.Text = "Error, LOT information not found.";
            }
        }


        // 작업자가 확인을 하였다.
        private void btnConfirm_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();

            CData.OpenLotCnt--;     // 2021.10.18 SungTae : [추가] Open Lot 수량을 감소

            // Buzzer가 OFf 되도록 Flag를 설정해준다.
            CData.bLotCompleteBuzzer = false;
        }


        // 버저만 Off 시키도록 한다.
        private void btnBuzzerOff_Click(object sender, EventArgs e)
        {
            // 2021.10.28 SungTae Start : [추가] 
            if (CData.CurCompany == ECompany.ASE_KR)
            {
                DialogResult = DialogResult.OK;
                Close();

                CData.OpenLotCnt--;     // 2021.10.18 SungTae : [추가] Open Lot 수량을 감소
            }
            // 2021.10.28 SungTae End

            // Buzzer가 OFf 되도록 Flag를 설정해준다.
            CData.bLotCompleteBuzzer = false;
        }


        // 폼이 닫히려 한다.
        private void CDlgLotEndMsg_FormClosing(object sender, FormClosingEventArgs e)
        {
            CData.LotMgr.DeleteFinishLot();             // 완료가 된 LOT들은 삭제해준다.
            CData.bLotCompleteBuzzer = false;

            e.Cancel = false;
        }
    }
}
