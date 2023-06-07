using System.Drawing;
using System.Windows.Forms;

namespace SG2000X
{
    public static class CMsg
    {
        public static DialogResult Show(eMsg iMsg, string sTitle, string sMsg)
        {
            //190620 ksg : Wheel Change Msg 추가
            //using (frmMsg mForm = new frmMsg(iMsg, sTitle, sMsg))
            //{
            //    return mForm.ShowDialog();
            //}
            if (iMsg == eMsg.Change)
            {
                using (frmMsgChange mForm = new frmMsgChange(iMsg, sTitle, CLang.It.GetConvertMsg(sMsg)))
                {
                    return mForm.ShowDialog();
                }
            }
            else
            {
                using (frmMsg mForm = new frmMsg(iMsg, sTitle, CLang.It.GetConvertMsg(sMsg)))
                {
                    return mForm.ShowDialog();
                }
            }
        }

        // 2021-06-04, jhLee, Multi-LOT 종료를 표시하기 위한 함수 추가
        public static DialogResult ShowLotEnd()
        {
            using (CDlgLotEndMsg pDlg = new CDlgLotEndMsg())
            {
                return pDlg.ShowDialog();
            }
        }


        // 2021-07-13, jhLee, Multi-LOT의 투입 종료여부를 표시하고 응답을 받기 위한 함수 추가
        public static DialogResult ShowLotCloseConfirm()
        {
            using (CDlgLotCloseMsg pDlg = new CDlgLotCloseMsg())
            {

                return pDlg.ShowDialog();
            }
        }

        // 2020.10.28 JSKim St
        //public static DialogResult Show(eMsg iMsg, string sTitle, string sMsg, Point tPt)
        public static DialogResult Show(eMsg iMsg, string sTitle, string sMsg, Point tPt, bool bCenter = false)
        // 2020.10.28 JSKim Ed
        {
            //190620 ksg : Wheel Change Msg 추가
            //using (frmMsg mForm = new frmMsg(iMsg, sTitle, sMsg))
            //{
            //    return mForm.ShowDialog();
            //}
            if (iMsg == eMsg.Change)
            {
                using (frmMsgChange mForm = new frmMsgChange(iMsg, sTitle, CLang.It.GetConvertMsg(sMsg)))
                {
                    // 2020.10.28 JSKim St
                    if (bCenter)
                    {
                        tPt.X -= mForm.Size.Width / 2;
                        tPt.Y -= mForm.Size.Height / 2;
                    }
                    // 2020.10.28 JSKim Ed
                    mForm.Location = tPt;
                    return mForm.ShowDialog();
                }
            }
            else
            {
                using (frmMsg mForm = new frmMsg(iMsg, sTitle, CLang.It.GetConvertMsg(sMsg)))
                {
                    // 2020.10.28 JSKim St
                    if (bCenter)
                    {
                        tPt.X -= mForm.Size.Width / 2;
                        tPt.Y -= mForm.Size.Height / 2;
                    }
                    // 2020.10.28 JSKim Ed
                    mForm.Location = tPt;
                    return mForm.ShowDialog();
                }
            }
        }

        // 2021.07.21 lhs Start : 모달리스 창
        public static void ShowModeless(eMsg iMsg, string sTitle, string sMsg)
        {
            frmMsg mForm = new frmMsg(iMsg, sTitle, CLang.It.GetConvertMsg(sMsg));
			mForm.Show();
        }

        public static void ShowWhl()
        {
            CData.VwWhl = new TMsg();
            CData.VwWhl.bView = true;
        }

        public static void ShowMsg()
        {
            CData.VwMsg = new TMsg();
            CData.VwMsg.bView = true;
        }
    }
}
