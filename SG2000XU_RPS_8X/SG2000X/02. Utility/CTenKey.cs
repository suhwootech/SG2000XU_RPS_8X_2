using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SG2000X
{
    class CTenKey : CStn<CTenKey>
    {
        public int m_iSetNum = 0;
        private CTim m_Delay = new CTim();
        static string sxPreTenNum = "";

        private CTenKey()
        {

        }

        public void Initial()
        {

        }

        public void Release()
        {

        }

        public void Update()
        {
            GetInput ();
            SetOutput();
        }

        public void GetInput()
        {
            string sxTenNum = "";
            int    Temp;

            bool bx147C =  CIO.It.Get_X(eX.TNK_147C);
            bool bx2580 =  CIO.It.Get_X(eX.TNK_2580);
            bool bx369S =  CIO.It.Get_X(eX.TNK_3695);
            bool bx123  =  CIO.It.Get_X(eX.TNK_123 );
            bool bx456  =  CIO.It.Get_X(eX.TNK_456 );
            bool bx789  =  CIO.It.Get_X(eX.TNK_789 );
            bool bxC0S  =  CIO.It.Get_X(eX.TNK_C0S );

                 if(bx123 && bx147C) sxTenNum = "1";
            else if(bx123 && bx2580) sxTenNum = "2";
            else if(bx123 && bx369S) sxTenNum = "3";
            else if(bx456 && bx147C) sxTenNum = "4";
            else if(bx456 && bx2580) sxTenNum = "5";
            else if(bx456 && bx369S) sxTenNum = "6";
            else if(bx789 && bx147C) sxTenNum = "7";
            else if(bx789 && bx2580) sxTenNum = "8";
            else if(bx789 && bx369S) sxTenNum = "9";
            else if(bxC0S && bx147C) sxTenNum = "C";
            else if(bxC0S && bx2580) sxTenNum = "0";
            else                     sxTenNum = "E";

            if(m_Delay.OnDelay((bxC0S && bx369S), 2000))
            {
                sxTenNum = "S";
            }

            if(sxPreTenNum == sxTenNum) return;

            sxPreTenNum = sxTenNum;

            if(sxTenNum == "C") //
            {
                m_iSetNum = 0;
                return;
            }

            if(sxTenNum == "S")
            {
                //메뉴얼 함수 호출 하는 곳
                int num;
                num = m_iSetNum;
                CSQ_Man.It.Tenkey(m_iSetNum);
                return;
            }

            if(sxTenNum == "E") return;

            if(m_iSetNum != 0)
            {
                Temp = m_iSetNum * 10 + Convert.ToInt32(sxTenNum);
                if(Temp > 199) m_iSetNum = Temp % 100;
                else           m_iSetNum = Temp      ;
            }
            else
            {
                m_iSetNum = Convert.ToInt32(sxTenNum);
            }

        }

        public void SetOutput()
        {
            int i1Val   =  m_iSetNum % 10       ;
            int i10Val  = (m_iSetNum % 100) / 10;
            int i100Val =  m_iSetNum / 100      ;

            SetOutput(true , i1Val );
            SetOutput(false, i10Val);

            if(i100Val > 0) CIO.It.Set_Y((int)eY.TNK_100_1, true);
            else            CIO.It.Set_Y((int)eY.TNK_100_1, false );

        }

        public void SetOutput(bool _bLow, int _iVal)
        {
            int a,b,c,d;

            if(_bLow) {a = (int)eY.TNK_1_1 ;  b = (int)eY.TNK_1_2 ; c = (int)eY.TNK_1_4 ; d = (int)eY.TNK_1_8 ;}
            else      {a = (int)eY.TNK_10_1;  b = (int)eY.TNK_10_2; c = (int)eY.TNK_10_4; d = (int)eY.TNK_10_8;}

                 if(_iVal == 0) {CIO.It.Set_Y(a, true ); CIO.It.Set_Y(b, true ); CIO.It.Set_Y(c, true ); CIO.It.Set_Y(d, true );}
            else if(_iVal == 1) {CIO.It.Set_Y(a, false); CIO.It.Set_Y(b, true ); CIO.It.Set_Y(c, true ); CIO.It.Set_Y(d, true );}
            else if(_iVal == 2) {CIO.It.Set_Y(a, true ); CIO.It.Set_Y(b, false); CIO.It.Set_Y(c, true ); CIO.It.Set_Y(d, true );}
            else if(_iVal == 3) {CIO.It.Set_Y(a, false); CIO.It.Set_Y(b, false); CIO.It.Set_Y(c, true ); CIO.It.Set_Y(d, true );}
            else if(_iVal == 4) {CIO.It.Set_Y(a, true ); CIO.It.Set_Y(b, true ); CIO.It.Set_Y(c, false); CIO.It.Set_Y(d, true );}
            else if(_iVal == 5) {CIO.It.Set_Y(a, false); CIO.It.Set_Y(b, true ); CIO.It.Set_Y(c, false); CIO.It.Set_Y(d, true );}
            else if(_iVal == 6) {CIO.It.Set_Y(a, true ); CIO.It.Set_Y(b, false); CIO.It.Set_Y(c, false); CIO.It.Set_Y(d, true );}
            else if(_iVal == 7) {CIO.It.Set_Y(a, false); CIO.It.Set_Y(b, false); CIO.It.Set_Y(c, false); CIO.It.Set_Y(d, true );}
            else if(_iVal == 8) {CIO.It.Set_Y(a, true ); CIO.It.Set_Y(b, true ); CIO.It.Set_Y(c, true ); CIO.It.Set_Y(d, false);}
            else if(_iVal == 9) {CIO.It.Set_Y(a, false); CIO.It.Set_Y(b, true ); CIO.It.Set_Y(c, true ); CIO.It.Set_Y(d, false);}
        }
    }
}
