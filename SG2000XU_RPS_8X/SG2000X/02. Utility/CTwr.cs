using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SG2000X
{
    public class CTwr : CStn<CTwr>
    {
        
        private CTim m_mFlick = new CTim(); 
        public bool m_bFlick;

         private CTwr()
        {

        }

        public void TowerStatus(int iTWStatus)
        {
            bool BuzSkip = CSQ_Main.It.m_bBuzSkip;

            if(m_bFlick) 
            {
                if(m_mFlick.OnDelay(m_bFlick, 500))
                {
                    m_bFlick = false;
                }
            }
            else 
            {
                if(m_mFlick.OnDelay(!m_bFlick, 500))
                {
                    m_bFlick = true;
                }
            }

            // 200326 mjy : 라이트 커튼 감지 조건 추가
            //if (CData.bLcDetect)
            if (CData.bLcDetect || CData.bBuzzPMMsgWnd)  // 2021.10.22 lhs CData.bBuzzPMMsgWnd 추가
            {
                CIO.It.Set_Y(eY.SYS_TwlRed,     false   );
                CIO.It.Set_Y(eY.SYS_TwlYellow,  m_bFlick);
                CIO.It.Set_Y(eY.SYS_TwlGreen,   false   );
            }
            else
            {
                switch (CData.m_TowerInfo[iTWStatus].Red)
                {
                    case ELamp.Off:     { CIO.It.Set_Y(eY.SYS_TwlRed, false);       break; }
                    case ELamp.On:      { CIO.It.Set_Y(eY.SYS_TwlRed, true);        break; }
                    case ELamp.Flick:   { CIO.It.Set_Y(eY.SYS_TwlRed, m_bFlick);    break; }
                }
                switch (CData.m_TowerInfo[iTWStatus].Yel)
                {
                    case ELamp.Off:     { CIO.It.Set_Y(eY.SYS_TwlYellow, false);    break; }
                    case ELamp.On:      { CIO.It.Set_Y(eY.SYS_TwlYellow, true);     break; }
                    case ELamp.Flick:   { CIO.It.Set_Y(eY.SYS_TwlYellow, m_bFlick); break; }
                }
                switch (CData.m_TowerInfo[iTWStatus].Grn)
                {
                    case ELamp.Off:     { CIO.It.Set_Y(eY.SYS_TwlGreen, false);     break; }
                    case ELamp.On:      { CIO.It.Set_Y(eY.SYS_TwlGreen, true);      break; }
                    case ELamp.Flick:   { CIO.It.Set_Y(eY.SYS_TwlGreen, m_bFlick);  break; }
                }
            }

            // Buzzer - maeng181121
            if(BuzSkip)
            {
                CIO.It.Set_Y(eY.SYS_BuzzK1, false);
                CIO.It.Set_Y(eY.SYS_BuzzK2, false);
                CIO.It.Set_Y(eY.SYS_BuzzK3, false);
                return;
            }

            // 200326 mjy : 라이트 커튼 감지 조건 추가
            //if (CData.bLcDetect)
            if (CData.bLcDetect || CData.bBuzzPMMsgWnd)  // 2021.10.22 lhs CData.bBuzzPMMsgWnd 추가
            {
                CIO.It.Set_Y(eY.SYS_BuzzK1, false);
                CIO.It.Set_Y(eY.SYS_BuzzK2, false);
                CIO.It.Set_Y(eY.SYS_BuzzK3, true);
            }
            else if (CData.VwKey.bView && !CData.BuzzOff)
            {
                CIO.It.Set_Y(eY.SYS_BuzzK1, false);
                CIO.It.Set_Y(eY.SYS_BuzzK2, true);
                CIO.It.Set_Y(eY.SYS_BuzzK3, false);
            }
            else
            {
                switch (CData.m_TowerInfo[iTWStatus].Buzz)
                {
                    case EBuzz.Off:
                        CIO.It.Set_Y(eY.SYS_BuzzK1, false);
                        CIO.It.Set_Y(eY.SYS_BuzzK2, false);
                        CIO.It.Set_Y(eY.SYS_BuzzK3, false);
                        break;
                    case EBuzz.Buzz1:
                        CIO.It.Set_Y(eY.SYS_BuzzK1, true);
                        CIO.It.Set_Y(eY.SYS_BuzzK2, false);
                        CIO.It.Set_Y(eY.SYS_BuzzK3, false);
                        break;
                    case EBuzz.Buzz2:
                        CIO.It.Set_Y(eY.SYS_BuzzK1, false);
                        CIO.It.Set_Y(eY.SYS_BuzzK2, true);
                        CIO.It.Set_Y(eY.SYS_BuzzK3, false);
                        break;
                    case EBuzz.Buzz3:
                        CIO.It.Set_Y(eY.SYS_BuzzK1, false);
                        CIO.It.Set_Y(eY.SYS_BuzzK2, false);
                        CIO.It.Set_Y(eY.SYS_BuzzK3, true);
                        break;
                }
            }
        }    
    }
}
