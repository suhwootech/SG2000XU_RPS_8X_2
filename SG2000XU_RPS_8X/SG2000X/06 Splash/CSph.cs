using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SG2000X
{
    public enum TypeOfMessage
    {
        Success,
        Warning,
        Error
    }
    public static class CSph
    {
        static frmSph m_Form = null;

        public static void Show()
        {
            if (m_Form == null)
            {
                m_Form = new frmSph();
                m_Form.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                m_Form.ShowScreen();
            }
        }

        public static void Close()
        {
            if (m_Form != null)
            {
                m_Form.CloseScreen();
                m_Form = null;
            }
        }

        public static void Update(string Text)
        {
            if (m_Form != null)
            {
                m_Form.Update(Text);
            }
        }

        public static void Update(string Text, TypeOfMessage tom)
        {
            if (m_Form != null)
            {
                m_Form.Update(Text, tom);
            }
        }
    }
}
