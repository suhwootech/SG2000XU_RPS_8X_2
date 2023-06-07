using System.IO;
using System.Text;

namespace SG2000X
{
    public class CTower : CStn<CTower>
    {
        private CTower()
        {
            // Config 폴더 없을 시 폴더 생성
            if (Directory.Exists(GV.PATH_CONFIG) == false)
            { Directory.CreateDirectory(GV.PATH_CONFIG); }
        }

        public void Save()
        {
            string sPath = GV.PATH_CONFIG + "TowerSet.twr";
            StringBuilder mSB = new StringBuilder();

            int nMax_Twr = (int)ETowerStatus.MAX_TWR_STATUS;

            for (int nTw = 0; nTw < nMax_Twr; nTw++)
            {
                mSB.AppendLine("[" + ((ETowerStatus)nTw).ToString() + "]");
                mSB.AppendLine("Red="    + (int)CData.m_TowerInfo[nTw].Red );
                mSB.AppendLine("Yellow=" + (int)CData.m_TowerInfo[nTw].Yel );
                mSB.AppendLine("Green="  + (int)CData.m_TowerInfo[nTw].Grn );
                mSB.AppendLine("Buzzer=" + (int)CData.m_TowerInfo[nTw].Buzz);
                mSB.AppendLine();
            }
            
            CCheckChange.CheckChanged("TOWER", sPath, CCheckChange.ReadOldFile(sPath), mSB.ToString());

            CLog.Check_File_Access(sPath, mSB.ToString(), false);   
        }

        public bool Load()
        {
            bool bRet = true;

            string sPath = GV.PATH_CONFIG + "TowerSet.twr";
            string sSec = "";

            if (!File.Exists(sPath))
            {
                bRet = false;
            }
            else
            {
                CIni mIni = new CIni(sPath);
                sSec = "Tower Lamp";

                int nMax_Twr = (int)ETowerStatus.MAX_TWR_STATUS;
            
                for (int nTw = 0; nTw < nMax_Twr; nTw++)
                {
                    sSec = ((ETowerStatus)nTw).ToString();

                    CData.m_TowerInfo[nTw].Red  = (ELamp)mIni.ReadI(sSec, "Red"   );
                    CData.m_TowerInfo[nTw].Yel  = (ELamp)mIni.ReadI(sSec, "Yellow");
                    CData.m_TowerInfo[nTw].Grn  = (ELamp)mIni.ReadI(sSec, "Green" );
                    CData.m_TowerInfo[nTw].Buzz = (EBuzz)mIni.ReadI(sSec, "Buzzer");
                }
            }

            return bRet;
        }
    }
}
