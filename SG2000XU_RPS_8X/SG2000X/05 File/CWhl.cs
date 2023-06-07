using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SG2000X
{
    public class CWhl : CStn<CWhl>
    {
        private CWhl()
        {
            CData.Whls[(int)EWay.L].aUsedP = new tStep[2];
            CData.Whls[(int)EWay.L].aNewP  = new tStep[2];

            CData.Whls[(int)EWay.R].aUsedP = new tStep[2];
            CData.Whls[(int)EWay.R].aNewP  = new tStep[2];
        }

        public void InitWhl(out tWhl tDst)
        {
            tDst = new tWhl();
            tDst.aUsedP = new tStep[2];
            tDst.aNewP  = new tStep[2];

            tDst.dWhlO   = 180;
            tDst.dWhltH  = 0;
            tDst.iGtc    = 0;
            tDst.iGdc    = 0;
            tDst.dWhltL  = 0;
            tDst.dWhloL  = 0;
            tDst.dWhldoL = 0;
            tDst.dWhldcL = 0;

            tDst.aUsedP[0].eMode     = eStepMode.Both;
            tDst.aUsedP[0].dTotalDep = 0;
            tDst.aUsedP[0].dCycleDep = 0;
            tDst.aUsedP[0].dTblSpd   = 0;
            tDst.aUsedP[0].iSplSpd   = 0;
            tDst.aUsedP[0].eDir      = eStartDir.Forward;

            tDst.aUsedP[1].eMode     = eStepMode.Both;
            tDst.aUsedP[1].dTotalDep = 0;
            tDst.aUsedP[1].dCycleDep = 0;
            tDst.aUsedP[1].dTblSpd   = 0;
            tDst.aUsedP[1].iSplSpd   = 0;
            tDst.aUsedP[1].eDir      = eStartDir.Forward;

            tDst.aNewP[0].eMode     = eStepMode.Both;
            tDst.aNewP[0].dTotalDep = 0;
            tDst.aNewP[0].dCycleDep = 0;
            tDst.aNewP[0].dTblSpd   = 0;
            tDst.aNewP[0].iSplSpd   = 0;
            tDst.aNewP[0].eDir      = eStartDir.Forward;

            tDst.aNewP[1].eMode     = eStepMode.Both;
            tDst.aNewP[1].dTotalDep = 0;
            tDst.aNewP[1].dCycleDep = 0;
            tDst.aNewP[1].dTblSpd   = 0;
            tDst.aNewP[1].iSplSpd   = 0;
            tDst.aNewP[1].eDir      = eStartDir.Forward;

            tDst.iDrsC     = 0;
            tDst.dWhlFtH   = 0.0;
            tDst.dWhlBf    = 0.0;
            tDst.dWhlAf    = 0.0;
            tDst.dWhlLimit = 1;

            tDst.sDrsName  = "";
            tDst.dDrsOuter = 75;
            tDst.dDrsH     = 0.0;
            tDst.dDrsBf    = 0.0;
            tDst.dDrsAf    = 0.0;
            tDst.dDrsLimit = 1;
        }

        public int Save(EWay eWy, tWhl tSrc)
        {
            string sPath = GV.PATH_WHEEL;

            if (eWy == EWay.L)
            { sPath += "Left\\"; }
            else
            { sPath += "Right\\"; }

            if (!Directory.Exists(GV.PATH_WHEEL))
            { Directory.CreateDirectory(GV.PATH_WHEEL); }

            if(CDataOption.UseDeviceWheel)
            {
                tSrc.dWhlLimit = CData.Whls[(int)eWy].dWhlLimit;
                tSrc.dDrsLimit = CData.Whls[(int)eWy].dDrsLimit;
            }

            sPath += tSrc.sWhlName + "\\WheelInfo.whl";
            return Save(sPath, tSrc);
        }

        public int Save(string sPath, tWhl tSrc)
        {
            int iRet = 0;
            StringBuilder mSB = new StringBuilder();

            if (!Directory.Exists(GV.PATH_WHEEL))
            { Directory.CreateDirectory(GV.PATH_WHEEL); }

            mSB.AppendLine("[Information]");
            mSB.AppendLine("Name="                     + tSrc.sWhlName    );
            mSB.AppendLine("Outer="                    + tSrc.dWhlO       );
            mSB.AppendLine("Tip Height="               + tSrc.dWhltH      );
            mSB.AppendLine("Virtual Tip Height="       + tSrc.dWhlvH      );
            mSB.AppendLine("Total Grinding Count="     + tSrc.iGtc        );
            mSB.AppendLine("Dressing After Count="     + tSrc.iGdc        );
            mSB.AppendLine("Total Loss="               + tSrc.dWhltL      );
            mSB.AppendLine("1Strip Loss="              + tSrc.dWhloL      );
            mSB.AppendLine("1Dressing Loss="           + tSrc.dWhldoL     );
            mSB.AppendLine("Dressing Cycle Loss="      + tSrc.dWhldcL     );
            mSB.AppendLine("Aircut Thickness="         + tSrc.dDair       );
            // 2020.11.23 JSKim St
            mSB.AppendLine("Aircut Replace Thickness=" + tSrc.dDairRep    );
            // 2020.11.23 JSKim Ed
            mSB.AppendLine("First Tip Height="         + tSrc.dWhlFtH     );
            mSB.AppendLine("Before Thickness="         + tSrc.dWhlBf      );
            mSB.AppendLine("After Thickness="          + tSrc.dWhlAf      );
            mSB.AppendLine("Limit="                    + tSrc.dWhlLimit   );
            mSB.AppendLine("Regrinding Offset="        + tSrc.dReGrdOffset);
            // 200727 jym : 휠 히스토리 내용추가
            mSB.AppendLine("Part No=" + tSrc.sPartNo);
            mSB.AppendLine("Mesh=" + tSrc.sMesh);
            mSB.AppendLine();
            mSB.AppendLine("[Dresser Information]");
            mSB.AppendLine("Name="             + tSrc.sDrsName );
            mSB.AppendLine("Outer="            + tSrc.dDrsOuter);
            mSB.AppendLine("Height="           + tSrc.dDrsH    );
            mSB.AppendLine("Before Thickness=" + tSrc.dDrsBf   );
            mSB.AppendLine("After Thickness="  + tSrc.dDrsAf   );
            mSB.AppendLine("Limit="            + tSrc.dDrsLimit);
            mSB.AppendLine();
            mSB.AppendLine("[Used Parameter 01]");
            mSB.AppendLine("Mode="          + tSrc.aUsedP[0].eMode.ToString());
            mSB.AppendLine("Total Depth="   + tSrc.aUsedP[0].dTotalDep       );
            mSB.AppendLine("Cycle Depth="   + tSrc.aUsedP[0].dCycleDep       );
            mSB.AppendLine("Table Speed="   + tSrc.aUsedP[0].dTblSpd         );
            mSB.AppendLine("Spindle Speed=" + tSrc.aUsedP[0].iSplSpd         );
            mSB.AppendLine("Direction="     + tSrc.aUsedP[0].eDir .ToString());
            mSB.AppendLine();
            mSB.AppendLine("[Used Parameter 02]");
            mSB.AppendLine("Mode="          + tSrc.aUsedP[1].eMode.ToString());
            mSB.AppendLine("Total Depth="   + tSrc.aUsedP[1].dTotalDep       );
            mSB.AppendLine("Cycle Depth="   + tSrc.aUsedP[1].dCycleDep       );
            mSB.AppendLine("Table Speed="   + tSrc.aUsedP[1].dTblSpd         );
            mSB.AppendLine("Spindle Speed=" + tSrc.aUsedP[1].iSplSpd         );
            mSB.AppendLine("Direction="     + tSrc.aUsedP[1].eDir .ToString());
            mSB.AppendLine();
            mSB.AppendLine("[New Parameter 01]");
            mSB.AppendLine("Mode="          + tSrc.aNewP[0].eMode.ToString());
            mSB.AppendLine("Total Depth="   + tSrc.aNewP[0].dTotalDep       );
            mSB.AppendLine("Cycle Depth="   + tSrc.aNewP[0].dCycleDep       );
            mSB.AppendLine("Table Speed="   + tSrc.aNewP[0].dTblSpd         );
            mSB.AppendLine("Spindle Speed=" + tSrc.aNewP[0].iSplSpd         );
            mSB.AppendLine("Direction="     + tSrc.aNewP[0].eDir .ToString());
            mSB.AppendLine();
            mSB.AppendLine("[New Parameter 02]");
            mSB.AppendLine("Mode="          + tSrc.aNewP[1].eMode.ToString());
            mSB.AppendLine("Total Depth="   + tSrc.aNewP[1].dTotalDep       );
            mSB.AppendLine("Cycle Depth="   + tSrc.aNewP[1].dCycleDep       );
            mSB.AppendLine("Table Speed="   + tSrc.aNewP[1].dTblSpd         );
            mSB.AppendLine("Spindle Speed=" + tSrc.aNewP[1].iSplSpd         );
            mSB.AppendLine("Direction="     + tSrc.aNewP[1].eDir .ToString());

            //2020.07.11 lks
            CCheckChange.CheckChanged("WHEEL USE", sPath, CCheckChange.ReadOldFile(sPath), mSB.ToString());

            CLog.Check_File_Access(sPath, mSB.ToString(), false);

            if (File.Exists(sPath) == false)
            { iRet = 1; }

            CLast.Save();

            return iRet;
        }

        //211122 pjh : Dresser 정보 저장
        public int SaveDrs(EWay eWy, TDrs tDrs)
        {
            string sPath = GV.PATH_DRESSER;

            if (eWy == EWay.L)
            {
                sPath += "Left\\"; 
            }
            else
            {
                sPath += "Right\\"; 
            }

            if (!Directory.Exists(GV.PATH_DRESSER))
            { Directory.CreateDirectory(GV.PATH_DRESSER); }

            //211208 pjh : Wheel History에 Dresser Name 입력
            CData.WhlsLog[(int)eWy].sDrsName = CData.DrsLog[(int)eWy].sDrsName = tDrs.sDrsName;
            //

            sPath += tDrs.sDrsName + "\\DresserInfo.txt";

            return SaveDrs(sPath, tDrs);
        }
        public int SaveDrs(string sPath, TDrs tDrs)
        {
            int iRet = 0;
            StringBuilder mSB = new StringBuilder();

            if (!Directory.Exists(GV.PATH_DRESSER))
            { Directory.CreateDirectory(GV.PATH_DRESSER); }

            mSB.AppendLine("[Information]");
            mSB.AppendLine("Name = "   + tDrs.sDrsName );
            mSB.AppendLine("Outer = " + tDrs.dDrsOuter);
            mSB.AppendLine("Height = " + tDrs.dDrsH);

            CCheckChange.CheckChanged("DRESSER USE", sPath, CCheckChange.ReadOldFile(sPath), mSB.ToString());

            CLog.Check_File_Access(sPath, mSB.ToString(), false);

            if (File.Exists(sPath) == false)
            { iRet = 1; }

            CLast.Save();

            return iRet;
        }
        //
        public int Load(EWay eWy, string sName, out tWhl tDst)
        {
            string sPath = GV.PATH_WHEEL;

            if (eWy == EWay.L)
            { sPath += "Left\\" + sName + "\\"; }
            else
            { sPath += "Right\\" + sName + "\\"; }

            sPath += "WheelInfo.whl";
            tDst   = new tWhl();
            tDst.aUsedP = new tStep[2];
            tDst.aNewP  = new tStep[2];

            if (!File.Exists(sPath))
            { return 1; }

            return Load(sPath, out tDst);
        }

        public int Load(string sPath, out tWhl tDst)
        {
            int iRet = 0;
            string sSec = "";
            string[] sTemp;

            tDst = new tWhl();
            tDst.aUsedP = new tStep[2];
            tDst.aNewP  = new tStep[2];
            sTemp = sPath.Split('\\');

            CIni mIni = new CIni(sPath);

            tDst.dtLast = File.GetLastWriteTime(sPath);

            sSec = "Information";
            mIni.Write(sSec, "Name", sTemp[5]);
            tDst.sWhlName     = mIni.Read (sSec, "Name");
            tDst.dWhlO        = mIni.ReadD(sSec, "Outer");
            tDst.dWhltH       = mIni.ReadD(sSec, "Tip Height");
            tDst.dWhlvH       = mIni.ReadD(sSec, "Virtual Tip Height");
            tDst.iGtc         = mIni.ReadI(sSec, "Total Grinding Count");
            tDst.iGdc         = mIni.ReadI(sSec, "Dressing After Count");
            tDst.dWhltL       = mIni.ReadD(sSec, "Total Loss");
            tDst.dWhloL       = mIni.ReadD(sSec, "1Strip Loss");
            tDst.dWhldoL      = mIni.ReadD(sSec, "1Dressing Loss");
            tDst.dWhldcL      = mIni.ReadD(sSec, "Dressing Cycle Loss");
            tDst.dDair        = mIni.ReadD(sSec, "Aircut Thickness");
            // 2020.11.23 JSKim St
            tDst.dDairRep     = mIni.ReadD(sSec, "Aircut Replace Thickness");
            // 2020.11.23 JSKim Ed
            tDst.dWhlBf       = mIni.ReadD(sSec, "Before Thickness");
            tDst.dWhlAf       = mIni.ReadD(sSec, "After Thickness");
            if(!CDataOption.UseDeviceWheel) tDst.dWhlLimit    = mIni.ReadD(sSec, "Limit");
            //20190421 ghk_휠 리그라인딩 옵셋
            tDst.dReGrdOffset = mIni.ReadD(sSec, "Regrinding Offset");
            // 200727 jym : 휠 히스토리 내용추가
            tDst.sPartNo      = mIni.Read (sSec, "Part No");
            tDst.sMesh        = mIni.Read (sSec, "Mesh");

            // 2020.11.23 JSKim St
            if (tDst.dDairRep == 0)
            {
                tDst.dDairRep = tDst.dDair;
            }
            // 2020.11.23 JSKim Ed

            sSec = "Dresser Information";
            tDst.sDrsName  = mIni.Read (sSec, "Name");
            tDst.dDrsOuter = mIni.ReadD(sSec, "Outer");
            tDst.dDrsH     = mIni.ReadD(sSec, "Height");
            tDst.dDrsBf    = mIni.ReadD(sSec, "Before Thickness");
            tDst.dDrsAf    = mIni.ReadD(sSec, "After Thickness");
            if (!CDataOption.UseDeviceWheel) tDst.dDrsLimit = mIni.ReadD(sSec, "Limit");

            sSec = "Used Parameter 01";
            tDst.aUsedP[0].eMode     = (eStepMode)Enum.Parse(typeof(eStepMode), mIni.Read(sSec, "Mode"));
            tDst.aUsedP[0].dTotalDep = mIni.ReadD(sSec, "Total Depth");
            tDst.aUsedP[0].dCycleDep = mIni.ReadD(sSec, "Cycle Depth");
            tDst.aUsedP[0].dTblSpd   = mIni.ReadD(sSec, "Table Speed");
            tDst.aUsedP[0].iSplSpd   = mIni.ReadI(sSec, "Spindle Speed");
            tDst.aUsedP[0].eDir      = (eStartDir)Enum.Parse(typeof(eStartDir), mIni.Read(sSec, "Direction"));

            sSec = "Used Parameter 02";
            tDst.aUsedP[1].eMode = (eStepMode)Enum.Parse(typeof(eStepMode), mIni.Read(sSec, "Mode"));
            tDst.aUsedP[1].dTotalDep = mIni.ReadD(sSec, "Total Depth");
            tDst.aUsedP[1].dCycleDep = mIni.ReadD(sSec, "Cycle Depth");
            tDst.aUsedP[1].dTblSpd = mIni.ReadD(sSec, "Table Speed");
            tDst.aUsedP[1].iSplSpd = mIni.ReadI(sSec, "Spindle Speed");
            tDst.aUsedP[1].eDir = (eStartDir)Enum.Parse(typeof(eStartDir), mIni.Read(sSec, "Direction"));

            sSec = "New Parameter 01";
            tDst.aNewP[0].eMode = (eStepMode)Enum.Parse(typeof(eStepMode), mIni.Read(sSec, "Mode"));
            tDst.aNewP[0].dTotalDep = mIni.ReadD(sSec, "Total Depth");
            tDst.aNewP[0].dCycleDep = mIni.ReadD(sSec, "Cycle Depth");
            tDst.aNewP[0].dTblSpd = mIni.ReadD(sSec, "Table Speed");
            tDst.aNewP[0].iSplSpd = mIni.ReadI(sSec, "Spindle Speed");
            tDst.aNewP[0].eDir = (eStartDir)Enum.Parse(typeof(eStartDir), mIni.Read(sSec, "Direction"));

            sSec = "New Parameter 02";
            tDst.aNewP[1].eMode = (eStepMode)Enum.Parse(typeof(eStepMode), mIni.Read(sSec, "Mode"));
            tDst.aNewP[1].dTotalDep = mIni.ReadD(sSec, "Total Depth");
            tDst.aNewP[1].dCycleDep = mIni.ReadD(sSec, "Cycle Depth");
            tDst.aNewP[1].dTblSpd = mIni.ReadD(sSec, "Table Speed");
            tDst.aNewP[1].iSplSpd = mIni.ReadI(sSec, "Spindle Speed");
            tDst.aNewP[1].eDir = (eStartDir)Enum.Parse(typeof(eStartDir), mIni.Read(sSec, "Direction"));

            return iRet;
        }

        //211122 Dresser Data Load
        public int LoadDrs(EWay eWy, string sName, out TDrs tDrs)
        {
            string sPath = GV.PATH_DRESSER;

            if (eWy == EWay.L)
            { sPath += "Left\\" + sName + "\\"; }
            else
            { sPath += "Right\\" + sName + "\\"; }

            sPath += "DresserInfo.txt";
            tDrs = new TDrs();

            if (!File.Exists(sPath))
            { return 1; }

            return LoadDrs(sPath, out tDrs);
        }
        public int LoadDrs(string sPath, out TDrs tDrs)
        {
            int iRet = 0;
            string sSec = "";
            string[] sTemp;

            tDrs = new TDrs();            
            sTemp = sPath.Split('\\');

            CIni mIni = new CIni(sPath);

            sSec = "Information";
            mIni.Write(sSec, "Name", sTemp[5]);
            tDrs.sDrsName = mIni.Read(sSec, "Name");
            tDrs.dDrsOuter = mIni.ReadD(sSec, "Outer");
            tDrs.dDrsH = mIni.ReadD(sSec, "Height");

            return iRet;
        }
        //
        public int SaveNew(EWay eWy, tWhl tSrc)
        {
            int iRet = 0;
            string sPath = GV.PATH_WHEEL;
            StringBuilder mSB = new StringBuilder();

            if (eWy == EWay.L)
            { sPath += "Left\\"; }
            else
            { sPath += "Right\\"; }

            if (!Directory.Exists(GV.PATH_WHEEL))
            { Directory.CreateDirectory(GV.PATH_WHEEL); }

            sPath += tSrc.sWhlName + "\\WheelInfo.whl";

            if (!Directory.Exists(GV.PATH_WHEEL))
            { Directory.CreateDirectory(GV.PATH_WHEEL); }

            mSB.AppendLine("[Information]");
            mSB.AppendLine("Name="                     + tSrc.sWhlName    );
            mSB.AppendLine("Outer="                    + tSrc.dWhlO       );
            mSB.AppendLine("Tip Height="               + 0.0);
            mSB.AppendLine("Virtual Tip Height="       + 0.0);
            mSB.AppendLine("Total Grinding Count="     + 0.0);
            mSB.AppendLine("Dressing After Count="     + 0);
            mSB.AppendLine("Total Loss="               + 0.0);
            mSB.AppendLine("1Strip Loss="              + 0.0);
            mSB.AppendLine("1Dressing Loss="           + 0.0);
            mSB.AppendLine("Dressing Cycle Loss="      + 0.0);
            mSB.AppendLine("Aircut Thickness="         + tSrc.dDair       );
            // 2020.11.23 JSKim St
            mSB.AppendLine("Aircut Replace Thickness=" + tSrc.dDairRep);
            // 2020.11.23 JSKim Ed
            mSB.AppendLine("First Tip Height="  + 0.0);
            mSB.AppendLine("Before Thickness="  + 0.0);
            mSB.AppendLine("After Thickness="   + 0.0);
            mSB.AppendLine("Limit="             + tSrc.dWhlLimit   );
            //20190421 ghk_휠 리그라인딩 옵셋
            mSB.AppendLine("Regrinding Offset=" + tSrc.dReGrdOffset);
            mSB.AppendLine("Part No="           + tSrc.sPartNo);
            mSB.AppendLine("Mesh="              + tSrc.sMesh);
            mSB.AppendLine();
            mSB.AppendLine("[Dresser Information]");
            //mSB.AppendLine("Name="              + "");     
            mSB.AppendLine("Name="              + tSrc.sDrsName);  // 2022.02.21 lhs : DrsName 저장 (SCK+ 요청사항)
            mSB.AppendLine("Outer="             + tSrc.dDrsOuter);
            mSB.AppendLine("Height="            + 0.0);
            mSB.AppendLine("Before Thickness="  + 0.0);
            mSB.AppendLine("After Thickness="   + 0.0);
            mSB.AppendLine("Limit="             + tSrc.dDrsLimit);
            mSB.AppendLine();
            mSB.AppendLine("[Used Parameter 01]");
            mSB.AppendLine("Mode="              + tSrc.aUsedP[0].eMode.ToString());
            mSB.AppendLine("Total Depth="       + tSrc.aUsedP[0].dTotalDep       );
            mSB.AppendLine("Cycle Depth="       + tSrc.aUsedP[0].dCycleDep       );
            mSB.AppendLine("Table Speed="       + tSrc.aUsedP[0].dTblSpd         );
            mSB.AppendLine("Spindle Speed="     + tSrc.aUsedP[0].iSplSpd         );
            mSB.AppendLine("Direction="         + tSrc.aUsedP[0].eDir .ToString());
            mSB.AppendLine();
            mSB.AppendLine("[Used Parameter 02]");
            mSB.AppendLine("Mode="              + tSrc.aUsedP[1].eMode.ToString());
            mSB.AppendLine("Total Depth="       + tSrc.aUsedP[1].dTotalDep       );
            mSB.AppendLine("Cycle Depth="       + tSrc.aUsedP[1].dCycleDep       );
            mSB.AppendLine("Table Speed="       + tSrc.aUsedP[1].dTblSpd         );
            mSB.AppendLine("Spindle Speed="     + tSrc.aUsedP[1].iSplSpd         );
            mSB.AppendLine("Direction="         + tSrc.aUsedP[1].eDir .ToString());
            mSB.AppendLine();
            mSB.AppendLine("[New Parameter 01]");
            mSB.AppendLine("Mode="              + tSrc.aNewP[0].eMode.ToString());
            mSB.AppendLine("Total Depth="       + tSrc.aNewP[0].dTotalDep       );
            mSB.AppendLine("Cycle Depth="       + tSrc.aNewP[0].dCycleDep       );
            mSB.AppendLine("Table Speed="       + tSrc.aNewP[0].dTblSpd         );
            mSB.AppendLine("Spindle Speed="     + tSrc.aNewP[0].iSplSpd         );
            mSB.AppendLine("Direction="         + tSrc.aNewP[0].eDir .ToString());
            mSB.AppendLine();
            mSB.AppendLine("[New Parameter 02]");
            mSB.AppendLine("Mode="              + tSrc.aNewP[1].eMode.ToString());
            mSB.AppendLine("Total Depth="       + tSrc.aNewP[1].dTotalDep       );
            mSB.AppendLine("Cycle Depth="       + tSrc.aNewP[1].dCycleDep       );
            mSB.AppendLine("Table Speed="       + tSrc.aNewP[1].dTblSpd         );
            mSB.AppendLine("Spindle Speed="     + tSrc.aNewP[1].iSplSpd         );
            mSB.AppendLine("Direction="         + tSrc.aNewP[1].eDir .ToString());

            //2020.07.11 lks
            CCheckChange.CheckChanged("WHEEL NEW", sPath, null, mSB.ToString());

            CLog.Check_File_Access(sPath, mSB.ToString(), false);

            if (File.Exists(sPath) == false)
            { iRet = 1; }

            CLast.Save();

            return iRet;
        }
        //211122 pjh : Dresser File Copy 시 사용
        public int SaveNewDrs(EWay eWy, TDrs tDrs)
        {

            int iRet = 0;
            string sPath = GV.PATH_DRESSER;
            StringBuilder mSB = new StringBuilder();

            if (eWy == EWay.L)
            { sPath += "Left\\"; }
            else
            { sPath += "Right\\"; }

            if (!Directory.Exists(GV.PATH_DRESSER))
            { Directory.CreateDirectory(GV.PATH_DRESSER); }

            sPath += tDrs.sDrsName + "\\DresserInfo.txt";

            if (!Directory.Exists(GV.PATH_DRESSER))
            { Directory.CreateDirectory(GV.PATH_DRESSER); }

            mSB.AppendLine("[Information]");
            mSB.AppendLine("Name = " + tDrs.sDrsName); ;
            mSB.AppendLine("Outer = " + tDrs.dDrsOuter);
            mSB.AppendLine("Height = " + 0.0);

            CCheckChange.CheckChanged("DRESSER NEW", sPath, CCheckChange.ReadOldFile(sPath), mSB.ToString());

            CLog.Check_File_Access(sPath, mSB.ToString(), false);

            if (File.Exists(sPath) == false)
            { iRet = 1; }

            return iRet;
        }
        //
        public int SaveNewHistory(string sPath)
        {
            int iRet = 0;
            string sWriteLine = "";
            FileInfo fI;
            //koo 191002 : Speed Write StreamWriter sw;

            fI = new FileInfo(sPath);
            if (!fI.Exists)
            {
                fI.Create().Close();

                sWriteLine = "Date,";
                sWriteLine += "Measure Type,";
                sWriteLine += "Tip Height[mm],";
                sWriteLine += "Tip Loss[mm],";
                sWriteLine += "Dresser Name,";
                sWriteLine += "Dresser Height[mm],";
                sWriteLine += "Dresser Loss[mm],";
                sWriteLine += "Grinding Count[ea],";
                sWriteLine += "Dressing Count[ea],";
                sWriteLine += "Cycle Dressing Strip,";
                sWriteLine += "1 Strip Loss[mm],";
                sWriteLine += "AirCut[mm],";
                sWriteLine += "Step1 Total Thickness[mm],";
                sWriteLine += "Step1 Cycle Thickness[mm],";
                sWriteLine += "Step1 Table Speed[mm/s],";
                sWriteLine += "Step1 Spindle RPM,";
                sWriteLine += "Step2 Total Thickness[mm],";
                sWriteLine += "Step2 Cycle Thickness[mm],";
                sWriteLine += "Step2 Table Speed[mm/s],";
                sWriteLine += "Step2 Spindle RPM,";
                CLog.Check_File_Access(sPath,sWriteLine,true);
            }

            return iRet;
        }
        public int SaveHistory(EWay eWy)
        {
            int iRet = 0;
            string sWriteLine = "";
            FileInfo fI;
            //koo 191002 : Speed Write StreamWriter sw;

            string sPath = GV.PATH_WHEEL;

            // 2021.04.06 SungTae Start : Wheel History 파일의 무한정 크기 증가로 인한 랙 발생으로 Daily 별로 생성하도록 변경
            if (eWy == EWay.L)
            {
                sPath += "Left\\";
                //sPath += CData.Whls[0].sWhlName + "\\";
            }
            else
            {
                sPath += "Right\\";
                //sPath += CData.Whls[1].sWhlName + "\\";
            }

            sPath += CData.Whls[(int)eWy].sWhlName + "\\WheelHistory\\";

            if(!Directory.Exists(sPath))
            {
                Directory.CreateDirectory(sPath);
            }

            //sPath += "WheelHistory.csv";
            sPath += DateTime.Now.ToString("yyyy-MM-dd") + ".csv";
            // 2021.04.06 SungTae End

            fI = new FileInfo(sPath);
            if(!fI.Exists)
            {
                SaveNewHistory(sPath);
            }

            CData.WhlsLog[(int)eWy].sDate = DateTime.Now.ToString("yyyy_MM_dd HH:mm:ss");      

            sWriteLine = CData.WhlsLog[(int)eWy].sDate + ",";
            sWriteLine += CData.WhlsLog[(int)eWy].sMeaType + ",";

            if (CSQ_Main.It.m_iStat == EStatus.Manual)
            {
                sWriteLine += CData.WhlAf[(int)eWy].ToString() + ",";
            }
            else
            {
                sWriteLine += CData.WhlsLog[(int)eWy].dWhltipH.ToString() + ",";
            }

            sWriteLine += CData.WhlsLog[(int)eWy].dWhltL        .ToString() + ",";
            sWriteLine += CData.WhlsLog[(int)eWy].sDrsName + ",";
            sWriteLine += CData.WhlsLog[(int)eWy].dDrsH         .ToString() + ",";
            sWriteLine += CData.WhlsLog[(int)eWy].dDrsL         .ToString() + ",";
            sWriteLine += CData.WhlsLog[(int)eWy].iGtc          .ToString() + ",";
            sWriteLine += CData.WhlsLog[(int)eWy].iDrsC         .ToString() + ",";
            sWriteLine += CData.WhlsLog[(int)eWy].iDrsCycleStrip.ToString() + ",";
            sWriteLine += CData.WhlsLog[(int)eWy].dOneStripLoss .ToString() + ",";
            sWriteLine += CData.WhlsLog[(int)eWy].dAirCut       .ToString() + ",";
            sWriteLine += CData.WhlsLog[(int)eWy].dStep1Total   .ToString() + ",";
            sWriteLine += CData.WhlsLog[(int)eWy].dStep1Cycle   .ToString() + ",";
            sWriteLine += CData.WhlsLog[(int)eWy].dStep1TbSpeed .ToString() + ",";
            sWriteLine += CData.WhlsLog[(int)eWy].iStep1Rpm     .ToString() + ",";
            sWriteLine += CData.WhlsLog[(int)eWy].dStep2Total   .ToString() + ",";
            sWriteLine += CData.WhlsLog[(int)eWy].dStep2Cycle   .ToString() + ",";
            sWriteLine += CData.WhlsLog[(int)eWy].dStep2TbSpeed .ToString() + ",";
            sWriteLine += CData.WhlsLog[(int)eWy].iStep2Rpm     .ToString() + ",";
            // 200727 jym : 휠 히스토리 내용추가
            if (CData.CurCompany == ECompany.ASE_KR && GV.WHEEL_HISTORY)
            {
                sWriteLine += (eWy == EWay.L) ? "Left," : "Right,";     // Table
                sWriteLine += CData.Whls[(int)eWy].sWhlName + ",";      // Wheel ID
                sWriteLine += CData.Whls[(int)eWy].sPartNo + ",";  // Wheel part NO
                sWriteLine += CData.Whls[(int)eWy].sMesh + ",";  // Mesh
                sWriteLine += CData.Whls[(int)eWy].dWhlBf + ",";        // Start wheel tip
                sWriteLine += CData.GetWLoss((int)eWy) + ",";           // wheel loss
                sWriteLine += CData.Whls[(int)eWy].dWhlAf + ",";        // end wheel tip
                sWriteLine += CData.Whls[(int)eWy].dDrsBf + ",";        // Start dresser tip
                sWriteLine += CData.GetDLoss((int)eWy) + ",";           // dresser loss
                sWriteLine += CData.Whls[(int)eWy].dDrsAf + ",";        // end dresser tip
            }
            CLog.Check_File_Access(sPath,sWriteLine,true);
            return iRet;
        }
		//220106 pjh : Dresser History 저장 함수
        public int SaveNewDrsHistory(string sPath)
        {
            int iRet = 0;
            string sWriteLine = "";
            FileInfo fI;
            //koo 191002 : Speed Write StreamWriter sw;

            fI = new FileInfo(sPath);
            if (!fI.Exists)
            {
                fI.Create().Close();

                sWriteLine  = "Date,";
                sWriteLine += "Measure Type,";
                sWriteLine += "Dresser Name,";
                sWriteLine += "Dresser Height[mm],";
                sWriteLine += "Dresser Loss[mm],";

                CLog.Check_File_Access(sPath, sWriteLine, true);
            }

            return iRet;
        }
        //211230 pjh :
        public int SaveDrsHistory(EWay eWy)
        {
            int iRet = 0;

            string sWriteLine = "";
            FileInfo fI;
            //koo 191002 : Speed Write StreamWriter sw;

            string sPath = GV.PATH_DRESSER;

            if (eWy == EWay.L)
            {
                sPath += "Left\\";
            }
            else
            {
                sPath += "Right\\";
            }

            sPath += CData.Drs[(int)eWy].sDrsName + "\\DresserHistory\\";

            if (!Directory.Exists(sPath))
            {
                Directory.CreateDirectory(sPath);
            }

            sPath += DateTime.Now.ToString("yyyy-MM-dd") + ".csv";
            // 2021.04.06 SungTae End

            fI = new FileInfo(sPath);
            if (!fI.Exists)
            {
                SaveNewDrsHistory(sPath);
            }

            CData.DrsLog[(int)eWy].sDate = DateTime.Now.ToString("yyyy_MM_dd HH:mm:ss");

            sWriteLine  = CData.DrsLog[(int)eWy].sDate + ",";
            sWriteLine += CData.DrsLog[(int)eWy].sMeaType + ",";
            sWriteLine += CData.DrsLog[(int)eWy].sDrsName + ",";
            sWriteLine += CData.DrsLog[(int)eWy].dDrsH.ToString() + ",";
            sWriteLine += CData.DrsLog[(int)eWy].dDrsL.ToString() + ",";
            CLog.Check_File_Access(sPath, sWriteLine, true);
            return iRet;
        }
		//

        //211122 pjh : Dresser 정보 초기화
        public void InitDresser(out TDrs tdrs)
        {
            tdrs.sDrsName  = "";
            tdrs.dDrsH     = 0.0;
            tdrs.dDrsOuter = 75.0;
        }
        //
    }
}
