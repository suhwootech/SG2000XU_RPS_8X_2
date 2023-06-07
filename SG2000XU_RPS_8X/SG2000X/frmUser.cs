using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SG2000X
{
    /// <summary>
    /// 220325 pjh : User 등록 Form
    /// </summary>
    public partial class frmUser : Form
    {
        private Timer m_tmUpdt;

        public frmUser()
        {
            InitializeComponent();

            //220325 pjh : Form 내용 Update용 타이머
            m_tmUpdt = new Timer();
            m_tmUpdt.Interval = 50;
            m_tmUpdt.Tick += _M_tmUpdt_Tick;
            //
        }

        private void btn_Can_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        //220325 pjh :
        public void CloseForm()
        {
            if (m_tmUpdt.Enabled) { m_tmUpdt.Stop(); }
            CCR.It.bUserForm = false;
            CCR.It.bCRRead = false;

            Close();
        }
        //

        public void OpenForm()
        {
            //string sPath = GV.PATH_USER + "User.ini\\";
            //int i = lbx_UserList.Items.Count;
            if (!m_tmUpdt.Enabled) { m_tmUpdt.Start(); }//Form Update Timer
            if (cmb_Level.SelectedItem == null) cmb_Level.SelectedIndex = 0;
            CCR.It.bUserForm = true;
            CCR.It.bCRRead = false;

            InitForm();

            _ListUp();
        }

        private void _M_tmUpdt_Tick(object sender, EventArgs e)
        {
            if (CCR.It.ReciveMsg != "")
            {
                //CCR.It.InitData(out tUser);

                txt_UserID.Text = CCR.It.ReciveMsg;

                CCR.It.ReciveMsg = "";
            }
        }

        private void _Get()
        {
            //220325 pjh : 중복되는 Id가 있으면 최신 정보로 갱신
            if(CCR.It.dicUser.ContainsKey(txt_UserID.Text))
            {
                CCR.It.dicUser.Remove(txt_UserID.Text);
            }
            //
            CCR.It.dicUser.Add(txt_UserID.Text, cmb_Level.SelectedItem.ToString());//Dictinary에 Item(Level) 및 Key(User ID) 저장
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            CData.bLastClick = true;
            Button mBtn = sender as Button;
            BeginInvoke(new Action(() => mBtn.Enabled = false));

            string sPath = GV.PATH_USER;
            string sPathUser = "";

            try
            {
                if (!Directory.Exists(sPath))
                { Directory.CreateDirectory(sPath); }

                sPathUser += sPath + "User.ini";

                if(!File.Exists(sPathUser))
                {
                    CCR.It.dicUser.Clear();
                }
                _Get();

                CCR.It.Save(sPathUser);

                _ListUp();
            }
            catch
            {

            }
            finally
            {
                BeginInvoke(new Action(() => mBtn.Enabled = true));
            }
        }

        private void _ListUp()
        {
            string sPath = GV.PATH_USER;

            lbx_UserList.Items.Clear();
            
            if(Directory.Exists(sPath))
            {
                sPath += "User.ini";
                
                
                if (File.Exists(sPath))
                {
                    CCR.It.GetCount(sPath);

                    if (CCR.It.iUserCnt != 0)
                    {
                        CCR.It.Load(sPath, CCR.It.iUserCnt);

                        foreach (var pair in CCR.It.dicUser)
                        {
                            lbx_UserList.Items.Add(pair.Key);
                        }
                    }
                }
            }
        }

        private void lbx_UserList_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sPath = GV.PATH_USER + "User.ini";
            ListBox mLbx = sender as ListBox;
            string sItem = "";
            string sLevel = "";

            if (lbx_UserList.SelectedItem != null)
            {
                sItem = lbx_UserList.SelectedItem.ToString();
            }
            if(sItem == "" || sItem == null)
            {
                
            }
            else
            {
                if(CCR.It.dicUser.ContainsKey(sItem))
                {
                    CCR.It.dicUser.TryGetValue(sItem, out sLevel);
                    txt_UserID.Text = sItem;
                    cmb_Level.SelectedItem = sLevel;
                }
            }
        }

        private void InitForm()
        {
            txt_UserID.Text = "";
            cmb_Level.SelectedIndex = 0;
        }

        private void btn_Del_Click(object sender, EventArgs e)
        {
            CData.bLastClick = true;
            Button mBtn = sender as Button;
            BeginInvoke(new Action(() => mBtn.Enabled = false));

            string sItem = lbx_UserList.SelectedItem.ToString();
            string sPath = GV.PATH_USER;
            int iCnt = lbx_UserList.Items.Count;

            try
            {
                if(sItem == "" || sItem == null)
                {

                }
                else
                {
                    if (!Directory.Exists(sPath)) return;
                    sPath += "User.ini";

                    if (CCR.It.dicUser.ContainsKey(sItem))
                    {
                        CCR.It.dicUser.Remove(sItem);
                        CCR.It.Save(sPath);
                    }
                }
                _ListUp();
            }
            catch(Exception ex)
            {

            }
            finally
            { BeginInvoke(new Action(() => mBtn.Enabled = true)); }
        }
    }
}
