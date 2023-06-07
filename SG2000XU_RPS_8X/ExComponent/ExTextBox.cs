using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Extended
{
    public enum ExTxtType
    {
        Int,
        Double
    }

    public enum ExSign
    {
        Signed,
        Unsigned
    }

    public class ExTextBox : TextBox
    {
        public ExTextBox()
        {

        }

        private double m_dMin;
        [DefaultValue(0)]
        public double ExMin
        {
            get { return m_dMin; }
            set { m_dMin = value; }
        }

        private double m_dMax;
        [DefaultValue(9999)]
        public double ExMax
        {
            get { return m_dMax; }
            set { m_dMax = value; }
        }

        private ExTxtType m_eTy = ExTxtType.Double;
        //[DefaultValue(ExTxtType.Double)]
        public ExTxtType ExType
        {
            get { return m_eTy; }
            set { m_eTy = value; }
        }

        private ExSign m_eSign = ExSign.Signed;
        [DefaultValue(ExSign.Unsigned)]
        public ExSign ExSigned
        {
            get { return m_eSign; }
            set { m_eSign = value; }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            bool bPt = false;
            if (m_eTy == ExTxtType.Double)
            { bPt = true; }

            bool bMi = false;
            if (m_eSign == ExSign.Signed)
            { bMi = true; }

            TypingOnlyNumber(e, bPt, bMi);


        }

        protected override void OnLeave(EventArgs e)
        {
            double dVal = 0;

            if (Text != "" && double.TryParse(Text, out dVal))
            {
                if (dVal > m_dMax)
                { Text = m_dMax.ToString(); }

                if (dVal < m_dMin)
                { Text = m_dMin.ToString(); }
            }
        }

        private void TypingOnlyNumber(KeyPressEventArgs e, bool bPoint, bool bMinus)
        {
            bool isValidInput = false;
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                if (bPoint)
                { if (e.KeyChar == '.') isValidInput = true; }
                if (bMinus)
                { if (e.KeyChar == '-') isValidInput = true; }

                if (!isValidInput) e.Handled = true;
            }

            if (bPoint)
            {
                if (e.KeyChar == '.' && (string.IsNullOrEmpty(Text.Trim()) || Text.IndexOf('.') > -1)) e.Handled = true;
            }
            if (bMinus)
            {
                if (e.KeyChar == '-' && (!string.IsNullOrEmpty(Text.Trim()) || Text.IndexOf('-') > -1)) e.Handled = true;
            }
        }
    }

}

