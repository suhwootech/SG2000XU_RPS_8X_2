using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace SG2000X
{
    public partial class frmSph : Form
    {
        delegate void StringDelegate(string Text);
        delegate void StringWithStatusDelegate(string Text, TypeOfMessage tom);
        delegate void SplashShowCloseDelegate();

        bool m_bClose = false;
        public frmSph()
        {
            InitializeComponent();

            if (CDataOption.Package == ePkg.Strip)
            { lbl_Title.Text = "SG - 2000X"; }
            else
            { lbl_Title.Text = "SG - 2000U"; }

            lbl_Ver.Text = "RPS Delta  Version " +  Assembly.GetExecutingAssembly().GetName().Version.ToString();
            object[] aObj = Assembly.GetExecutingAssembly().GetCustomAttributes(false);
            foreach(object obj in aObj)
            {
                if (obj.GetType() == typeof(AssemblyCopyrightAttribute))
                {
                    AssemblyCopyrightAttribute mCopy = (AssemblyCopyrightAttribute)obj;
                    lbl_Copy.Text = mCopy.Copyright;
                }
            }

        }

        public void ShowScreen()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new SplashShowCloseDelegate(ShowScreen));
                return;
            }
            this.Show();
            Application.Run(this);
        }

        public void CloseScreen()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new SplashShowCloseDelegate(CloseScreen));
                return;
            }
            m_bClose = true;
            this.Close();
        }

        public void Update(string Text)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new StringDelegate(Update), new object[] { Text });
                return;
            }

            // 라벨 내용 변경
            lbl_Txt.Text = Text;
        }

        public void Update(string Text, TypeOfMessage tom)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new StringWithStatusDelegate(Update), new object[] { Text, tom });
                return;
            }

            //switch (tom)
            //{
            //    case Typeo
            //}
        }

        private void frmSph_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!m_bClose)
            { e.Cancel = true; }
        }
    }
}
