using SuhwooUtil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace LicManager
{
    public partial class LicenseManager : Form
    {
        //소문자로 된 문자열을 암호화 하여 사용 할 것
        string managerValue = "dcda946909e89edd8991d4b2f24dac21b9056a02b666fde5b5453aca27f999c0";
        string inputValue = string.Empty;
        bool isControl = false;
        bool isLoaded = false;

        //string filePath = @"D:\Suhwoo\SG2000X\Config\suhwoo.lic";
        string filePath = @"D:\Suhwoo\SG2000X\Config";//\suhwoo.lic";

        public LicenseManager()
        {
            InitializeComponent();
        }

        private void LicenseManager_Load(object sender, EventArgs e)
        {
            btnFile.Enabled = false;
            btnLoad.Enabled = false;
            btnSave.Enabled = false;
            rtxtContents.ReadOnly = true;
            cbIsSecureFile.Visible = false;

            isControl = false;

            //rtxtContents.Text = SuhwooUtils.StringToSHA256(managerValue);
        }


        private void LicenseManager_KeyDown(object sender, KeyEventArgs e)
        {
            if (!isControl)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //MessageBox.Show(inputValue + " / " + SuhwooUtils.EncryptSHA256(inputValue) + " / " + managerValue);
                    if (SuhwooUtils.EncryptSHA256(inputValue).Equals(managerValue))
                    {
                        btnFile.Enabled = true;
                        btnLoad.Enabled = true;
                        //btnSave.Enabled = true;
                        rtxtContents.ReadOnly = false;

                        // 2020.11.27 JSKim St
                        if (filePath.Substring(filePath.Length - 4) == ".lic")
                        {
                            filePath = Path.GetDirectoryName(filePath);
                        }
                        // 2020.11.27 JSKim Ed

                        //cbIsSecureFile.Visible = true;
                        string[] licfiles = Directory.GetFiles(filePath, "suhwoo*.lic");
                        if (licfiles.Length > 1)
                        {
                            txtLicenseFilePath.Text = "The .lic file must be one...";
                        }
                        else if (licfiles.Length == 1)
                        {
                            filePath = licfiles[0];
                            //filePath = licfiles[0];//"suhwoo.lic";
                        }
                        else
                        {
                            txtLicenseFilePath.Text = "File not exists...";
                        }

                        if (File.Exists(filePath))
                            txtLicenseFilePath.Text = filePath;
                        else
                            txtLicenseFilePath.Text = "File not exists...";


                    }
                    else
                    {
                        inputValue = string.Empty;
                    }
                }
                else
                {
                    inputValue += new KeysConverter().ConvertToString(e.KeyValue).ToLower();
                }
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            string contents = ReadFile(txtLicenseFilePath.Text);

            //if (cbIsSecureFile.Checked)
            //    rtxtContents.Text = SuhwooUtils.DecryptAES256(contents);
            //else
            //    rtxtContents.Text = contents;

            string loadString;
            SuhwooUtils.DecryptAES256(contents, out loadString);

            rtxtContents.Text = loadString;

            if (!string.IsNullOrEmpty(rtxtContents.Text))
            {
                isLoaded = true;
                btnSave.Enabled = true;
            }
            else
            {
                MessageBox.Show("LOAD Failed. (Decrypt)", "SAVE");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!isLoaded) return;

            if (MessageBox.Show("Are you sure want to SAVE?", "SAVE", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                string savestring;
                if (SuhwooUtils.EncryptAES256(rtxtContents.Text, out savestring))
                {
                    SaveFile(txtLicenseFilePath.Text, savestring);

                    MessageBox.Show("SAVE Complate.", "SAVE");
                }
                else
                {
                    MessageBox.Show("SAVE Failed. (Encrypt)", "SAVE");
                }
            }
        }

        public void SaveFile(string path, string contents)
        {
            try
            {
                //백업
                File.Copy(path, path.Substring(0,path.Length-2) + DateTime.Now.ToString("_yyyyMMdd_HHmmss"));

                //저장
                File.WriteAllText(path, contents);
            }
            catch (Exception err)
            {
                MessageBox.Show("ERROR::" + err.Message);
            }
        }

        private string ReadFile(string path)
        {
            string ret = "";

            if (File.Exists(path))
            {
                try
                {
                    ret = File.ReadAllText(path);
                }
                catch (Exception err)
                {
                    MessageBox.Show("ERROR::" + err.Message);
                }
            }
            else
            {
                MessageBox.Show("ERROR::" + "FILE NOT EXISTS");
            }

            return ret;
        }

        private void rtxtContents_TextChanged(object sender, EventArgs e)
        {
            string strings = "\\<.+?\\>";
            MatchCollection stringMatches = Regex.Matches(rtxtContents.Text, strings);

            string comment = "\\<!--.+?\\-->";
            MatchCollection commentMatches = Regex.Matches(rtxtContents.Text, comment);

            // saving the original caret position + forecolor
            int originalIndex = rtxtContents.SelectionStart;
            int originalLength = rtxtContents.SelectionLength;
            Color originalColor = Color.Red;

            // MANDATORY - focuses a label before highlighting (avoids blinking)
            txtLicenseFilePath.Focus();

            // removes any previous highlighting (so modified words won't remain highlighted)
            rtxtContents.SelectionStart = 0;
            rtxtContents.SelectionLength = rtxtContents.Text.Length;
            rtxtContents.SelectionColor = originalColor;

            // scanning...
            foreach (Match m in stringMatches)
            {
                rtxtContents.SelectionStart = m.Index;
                rtxtContents.SelectionLength = m.Length;
                rtxtContents.SelectionColor = Color.Blue;
            }

            foreach (Match m in commentMatches)
            {
                rtxtContents.SelectionStart = m.Index;
                rtxtContents.SelectionLength = m.Length;
                rtxtContents.SelectionColor = Color.Green;
            }

            // restoring the original colors, for further writing
            rtxtContents.SelectionStart = originalIndex;
            rtxtContents.SelectionLength = originalLength;
            rtxtContents.SelectionColor = originalColor;

            // giving back the focus
            rtxtContents.Focus();
        }

        private void btnFile_Click(object sender, EventArgs e)
        {
            digFile.Filter = "*.lic|";
            digFile.FileName = "suhwoo.lic";
             
            if (digFile.ShowDialog() == DialogResult.OK)
            {
                txtLicenseFilePath.Text = digFile.FileName;
                // 2020.11.27 JSKim St
                string contents = ReadFile(txtLicenseFilePath.Text);

                string loadString;
                SuhwooUtils.DecryptAES256(contents, out loadString);

                rtxtContents.Text = loadString;

                if (!string.IsNullOrEmpty(rtxtContents.Text))
                {
                    isLoaded = true;
                    btnSave.Enabled = true;
                }
                else
                {
                    MessageBox.Show("LOAD Failed. (Decrypt)", "SAVE");
                }
                // 2020.11.27 JSKim Ed
            }
        }
    }
}
