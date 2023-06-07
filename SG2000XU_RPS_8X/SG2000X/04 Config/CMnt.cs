using System;
using System.IO;
using System.Text;

namespace SG2000X
{
    public class CMnt : CStn<CMnt>
    {
        private CMnt()
        {
            _Init();
        }

        private void _Init()
        {
            // 초기값
            CData.tPM.nLT_Sponge_Check_SetCnt       = 10000;    
            CData.tPM.nLT_Sponge_Check_CurrCnt      = 0;    
            CData.tPM.bLT_Sponge_Check_Use          = true;   // Use 
            
            CData.tPM.nRT_Sponge_Check_SetCnt       = 10000;    
            CData.tPM.nRT_Sponge_Check_CurrCnt      = 0;    
            CData.tPM.bRT_Sponge_Check_Use          = true;   // Use     

            CData.tPM.nOffP_Sponge_Check_SetCnt     = 10000;    
            CData.tPM.nOffP_Sponge_Check_CurrCnt    = 0;
            CData.tPM.bOffP_Sponge_Check_Use        = true;

            CData.tPM.nLT_Sponge_Change_SetCnt      = 50000;    
            CData.tPM.nLT_Sponge_Change_CurrCnt     = 0;    
            CData.tPM.bLT_Sponge_Change_Use         = true;   // Use     

            CData.tPM.nRT_Sponge_Change_SetCnt      = 50000;    
            CData.tPM.nRT_Sponge_Change_CurrCnt     = 0;    
            CData.tPM.bRT_Sponge_Change_Use         = true;   // Use 

            CData.tPM.nOffP_Sponge_Change_SetCnt    = 50000;    
            CData.tPM.nOffP_Sponge_Change_CurrCnt   = 0;
            CData.tPM.bOffP_Sponge_Change_Use       = true;

            CData.tPM.nPMMsg_ReDisp_Minute          = 10;       // 10분

        }

        public void Save()
        {
            string sPath = GV.PATH_CONFIG + "Maintenance.cfg";
            StringBuilder mSB = new StringBuilder();

            mSB.AppendLine("[PM Count]");

            mSB.AppendLine("nLT_Sponge_Check_SetCnt="       + CData.tPM.nLT_Sponge_Check_SetCnt);
            mSB.AppendLine("nLT_Sponge_Check_CurrCnt="      + CData.tPM.nLT_Sponge_Check_CurrCnt);
            //mSB.AppendLine("bLT_Sponge_Check_Use="          + CData.tPM.bLT_Sponge_Check_Use);    // 저장 안함

            mSB.AppendLine("nRT_Sponge_Check_SetCnt="       + CData.tPM.nRT_Sponge_Check_SetCnt);
            mSB.AppendLine("nRT_Sponge_Check_CurrCnt="      + CData.tPM.nRT_Sponge_Check_CurrCnt);
            //mSB.AppendLine("bRT_Sponge_Check_Use="          + CData.tPM.bRT_Sponge_Check_Use);    // 저장 안함

            mSB.AppendLine("nOffP_Sponge_Check_SetCnt="     + CData.tPM.nOffP_Sponge_Check_SetCnt);
            mSB.AppendLine("nOffP_Sponge_Check_CurrCnt="    + CData.tPM.nOffP_Sponge_Check_CurrCnt);
            //mSB.AppendLine("bOffP_Sponge_Check_Use="        + CData.tPM.bOffP_Sponge_Check_Use);   // 저장 안함

            mSB.AppendLine("nLT_Sponge_Change_SetCnt="      + CData.tPM.nLT_Sponge_Change_SetCnt);
            mSB.AppendLine("nLT_Sponge_Change_CurrCnt="     + CData.tPM.nLT_Sponge_Change_CurrCnt);
            //mSB.AppendLine("bLT_Sponge_Change_Use="         + CData.tPM.bLT_Sponge_Change_Use);   // 저장 안함

            mSB.AppendLine("nRT_Sponge_Change_SetCnt="      + CData.tPM.nRT_Sponge_Change_SetCnt);
            mSB.AppendLine("nRT_Sponge_Change_CurrCnt="     + CData.tPM.nRT_Sponge_Change_CurrCnt);
            //mSB.AppendLine("bRT_Sponge_Change_Use="         + CData.tPM.bRT_Sponge_Change_Use);     // 저장 안함

            mSB.AppendLine("nOffP_Sponge_Change_SetCnt="    + CData.tPM.nOffP_Sponge_Change_SetCnt);
            mSB.AppendLine("nOffP_Sponge_Change_CurrCnt="   + CData.tPM.nOffP_Sponge_Change_CurrCnt);
            // mSB.AppendLine("bOffP_Sponge_Change_Use="       + CData.tPM.bOffP_Sponge_Change_Use);    // 저장 안함

            mSB.AppendLine("nPMMsg_ReDisp_Minute="          + CData.tPM.nPMMsg_ReDisp_Minute);

            //2020.07.11 lks
            CCheckChange.CheckChanged("MAINTENANCE", sPath, CCheckChange.ReadOldFile(sPath), mSB.ToString());

            CLog.Check_File_Access(sPath, mSB.ToString(), false);
        }

        public void Load()
        {
            string sPath = GV.PATH_CONFIG + "Maintenance.cfg";
            if (!File.Exists(sPath))
            { return; }

            _Init();
            IniFile mIni = new IniFile();
            mIni.Load(sPath);

            CData.tPM.nLT_Sponge_Check_SetCnt       = mIni["PM Count"]["nLT_Sponge_Check_SetCnt"].ToInt();
            CData.tPM.nLT_Sponge_Check_CurrCnt      = mIni["PM Count"]["nLT_Sponge_Check_CurrCnt"].ToInt();
            //CData.tPM.bLT_Sponge_Check_Use          = mIni["PM Count"]["bLT_Sponge_Check_Use"].ToBool();

            CData.tPM.nRT_Sponge_Check_SetCnt       = mIni["PM Count"]["nRT_Sponge_Check_SetCnt"].ToInt();
            CData.tPM.nRT_Sponge_Check_CurrCnt      = mIni["PM Count"]["nRT_Sponge_Check_CurrCnt"].ToInt();
            //CData.tPM.bRT_Sponge_Check_Use          = mIni["PM Count"]["bRT_Sponge_Check_Use"].ToBool();
                        
            CData.tPM.nOffP_Sponge_Check_SetCnt     = mIni["PM Count"]["nOffP_Sponge_Check_SetCnt"].ToInt();
            CData.tPM.nOffP_Sponge_Check_CurrCnt    = mIni["PM Count"]["nOffP_Sponge_Check_CurrCnt"].ToInt();
            //CData.tPM.bOffP_Sponge_Check_Use        = mIni["PM Count"]["bOffP_Sponge_Check_Use"].ToBool();

            CData.tPM.nLT_Sponge_Change_SetCnt      = mIni["PM Count"]["nLT_Sponge_Change_SetCnt"].ToInt();
            CData.tPM.nLT_Sponge_Change_CurrCnt     = mIni["PM Count"]["nLT_Sponge_Change_CurrCnt"].ToInt();
            //CData.tPM.bLT_Sponge_Change_Use         = mIni["PM Count"]["bLT_Sponge_Change_Use"].ToBool();

            CData.tPM.nRT_Sponge_Change_SetCnt      = mIni["PM Count"]["nRT_Sponge_Change_SetCnt"].ToInt();
            CData.tPM.nRT_Sponge_Change_CurrCnt     = mIni["PM Count"]["nRT_Sponge_Change_CurrCnt"].ToInt();
            //CData.tPM.bRT_Sponge_Change_Use         = mIni["PM Count"]["bRT_Sponge_Change_Use"].ToBool();

            CData.tPM.nOffP_Sponge_Change_SetCnt    = mIni["PM Count"]["nOffP_Sponge_Change_SetCnt"].ToInt();
            CData.tPM.nOffP_Sponge_Change_CurrCnt   = mIni["PM Count"]["nOffP_Sponge_Change_CurrCnt"].ToInt();
            //CData.tPM.bOffP_Sponge_Change_Use       = mIni["PM Count"]["bOffP_Sponge_Change_Use"].ToBool();

            CData.tPM.nPMMsg_ReDisp_Minute          = mIni["PM Count"]["nPMMsg_ReDisp_Minute"].ToInt();

        }


    }
}
