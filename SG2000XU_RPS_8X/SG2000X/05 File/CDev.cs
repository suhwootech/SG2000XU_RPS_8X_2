using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;       // 2022.02.22 SungTae Start : [추가]

namespace SG2000X
{

    public class CDev : CStn<CDev>
    {
        public string m_sGrp = "";

        /// <summary>
        /// 3 Step / 12 Step 구분하기 위한 Max step count
        /// </summary>
        public static int iStepMaxCnt = 0;

        //
        public string sPreWhlL = "";
        public string sPreWhlR = "";
        //
        //210928 pjh : Device 변경 시 Log와 Grinding Mode 비교를 위한 변수
        public string sPreDev = "";
        public string[] aPreGrd = new string[2];
        public string[] aNowGrd = new string[2];
        public string sChangedMode = "";
        //
        //211005 pjh :
        public tWhl[] a_tWhl = new tWhl[2];
        //
        private CDev()
        {

        }

        public int InitDev(out tDev Dev)
        {
            Dev = new tDev();

            Dev.sName = "Default";
            #region Common Strip
            Dev.dShSide = 0;
            Dev.dLnSide = 0;
            Dev.bDual = eDual.Normal;

            //20190218 ghk_dynamicfunction
            Dev.bDynamicSkip = true;
            //
            //20190618 ghk_dfserver
            Dev.bDfServerSkip = true;
            //
            #endregion

            #region Common Contact
            Dev.iCol = 0;
            Dev.iRow = 0;
            Dev.dChipW = 0;
            Dev.dChipH = 0;
            Dev.dChipWGap = 0;
            Dev.dChipHGap = 0;
            Dev.iWinCnt = 0;
            Dev.aWinSt = new System.Drawing.PointF[5];
            Dev.aFake = new double[2];
            Dev.i18PStripCount = 0; //200712 jhc : 18 포인트 측정 (ASE-KR VOC)

            // 2021.08.02 SungTae Start : [추가] Measure(Before/After/One-point) 시 측정 위치에 대한 Offset 설정 추가(ASE-KR VOC)
            Dev.dMeasOffsetX = 0;
            Dev.dMeasOffsetY = 0;
            // 2021.08.02 SungTae End

            Dev.aUnitCen = new double[GV.PKG_MAX_UNIT];
            #endregion

            #region Common ETC
            Dev.iMgzCnt   = 0;
            Dev.dMgzPitch = 0;
            Dev.iOffMgzCnt   = 0;   // 2020.10.26 SungTae : Add by Qorvo(Strip 배출 관련)
            Dev.dOffMgzPitch = 0;   // 2020.10.26 SungTae : Add by Qorvo(Strip 배출 관련)
            Dev.bDryTop   = false;
            Dev.iDryRPM   = 0;
            Dev.eDryDir   = eDryDir.CCW;
            Dev.iDryCnt   = 0;

            Dev.dPushF    = 150;
            Dev.dPushS    = 60;

            Dev.iDryWtNozzleRPM = 0;  // 2021.03.30 lhs
            Dev.iDryWtNozzleCnt = 0;  // 2021.03.30 lhs

            //190211 ksg :
            Dev.bBcrSkip = false;
            Dev.bOriSkip = false;
            //201103 jhc : Orientation Check 1회 Skip 기능 추가 (ASE-Kr VOC)
            Dev.bOriOneTimeSkipUse = false;
            //
            Dev.bOcrSkip = false;
            Dev.bBcrKeyInSkip = true;

            Dev.bDataShift   = false; //190610 ksg :
            Dev.bDShiftPSkip = false; //200325 ksg : Data Shift Probe Skip

            Dev.aTblCleanVel = new double[2];       // 2021.03.02 SungTae : Device 별로 Table Cleaning Velocity 관리할 수 있도록 고객사 요청에 따른 추가(ASE-KR)
            #endregion

            #region Left, Right Data

            // 2020.09.08 SungTae : 3 Step 기능 추가
            if (CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)  iStepMaxCnt = GV.StepMaxCnt;
            else                                                    iStepMaxCnt = GV.StepMaxCnt_3;

            Dev.aData = new tDevData[2];
            for (int i = 0; i < 2; i++)
            {
                Dev.aData[i].dTotalTh = 0;
                Dev.aData[i].dPcbTh   = 0;
                Dev.aData[i].dMoldTh  = 0;
                Dev.aData[i].dAir     = 0;

                Dev.aData[i].eGrdMod = eGrdMode.Target;
                //Dev.aData[i].aSteps = new tStep[4];
                //Dev.aData[i].aSteps = new tStep[GV.StepMaxCnt];   //200414 ksg : 12 Step  기능 추가
                Dev.aData[i].aSteps = new tStep[iStepMaxCnt];       // 2020.09.08 SungTae : 3 Step 기능 추가에 따른 변경

                Dev.aData[i].aPosBf = new tCont[0, 0];
                Dev.aData[i].aPosAf = new tCont[0, 0];

                if (CDataOption.Use2004U)   // 2022.01.24 lhs : 2004U, Carrier내 Dummy 설정 
                {   
                    Dev.aData[i].bDummy = new bool[0, 0];
                    if (i == 0) // 한번만
                    {
                        Dev.bCopyDummy = new bool[0, 0];
                    }
                }   

                Dev.aData[i].dTpBubSpd  = 0;
                Dev.aData[i].iTpBubCnt  = 0;
                Dev.aData[i].dTpAirSpd  = 0;
                Dev.aData[i].iTpAirCnt  = 0;
                Dev.aData[i].dTpSpnSpd  = 0;
                Dev.aData[i].iTpSpnCnt  = 0;

                Dev.aData[i].eStartMode = eTDStartMode.Max;    
                
                Dev.aData[i].eBaseOnThick = EBaseOnThick.Mold;  // 2021.07.27 lhs (SCK VOC)

                // 2022.09.23 lhs Start : Rough1, 
                Dev.aData[i].bAppCylDepOnFirst  = false; // 첫번째 Griding시에 Z축 StartPos에 cycle depth 적용
                Dev.aData[i].bAppLargeCylDep    = false; // 대량의 Depth Grinding (Slow)
                // 2022.09.23 lhs End

                // 2022.08.10 SungTae Start : [수정] (ASE-KR 개발건) 설정된 Step 수량을 제외한 나머지 Step UI 비활성화
                if (CData.CurCompany == ECompany.ASE_KR)
                {
                    Dev.aData[i].nFixStepCnt = 1;
                }
                // 2022.08.10 SungTae End

                Dev.aData[i].dDrsPrid = 0;
                Dev.aData[i].iDrs_YEnd_Dir = 0;

                // Spindle 부하
                Dev.aData[i].dSpdAuto = 0;
                Dev.aData[i].dSpdError = 0;

                //// 2022.08.30 lhs Start : Spindle Current High/Low Limit 설정 (SCK+ VOC)
                //Dev.aData[i].nSpdCurrHL = 0;
                //Dev.aData[i].nSpdCurrLL = 0;
                //// 2022.08.30 lhs End

                //201203 jhc : 드레싱 후 N번째 그라인딩 시 휠 소모량 보정 기능
                Dev.aData[i].dWheelLoss = new double[GV.WHEEL_LOSS_CORRECT_STRIP_MAX];
                Dev.aData[i].dTotalWheelLossLimit = 0;
                //

                //201225 jhc : 고급 Grind Condition 설정/체크 기능 (Spindle Current, Table Vacuum Low Limit 설정)
                Dev.aData[i].nSpindleCurrentDressLow = 0;  //Lower Limit of Spindle Current when Dressing
                Dev.aData[i].nSpindleCurrentDressHigh = 0; //Upper Limit of Spindle Current when Dressing
                Dev.aData[i].nSpindleCurrentGrindLow = 0;  //Lower Limit of Spindle Current when Grinding
                Dev.aData[i].nSpindleCurrentGrindHigh = 0; //Upper Limit of Spindle Current when Grinding
                Dev.aData[i].dTableVacuumGrindLimit = 0;   //Lower Limit of Table Vacuum when Grinding
                Dev.aData[i].dTableVacuumLowHoldTime = 0;  //201231 : Table Vacuum Lower Limit Holding Time (이 시간 이상 Lower Limit 유지 시 Alarm)
                //..
            }
            #endregion

            Dev.aGrd_Y_Start = new double[2];
            Dev.aGrd_Y_End = new double[2];

            //20191011 ghk_manual_bcr
            Dev.aGrd_Y_Ori = new double[2];

            return 0;
        }

        public int Save(string sPath)
        {
            //200807 jhc : CData.Dev.bDynamicSkip 옵션
            if (CDataOption.MeasureDf == eDfServerType.NotMeasureDf)
            { CData.Dev.bDynamicSkip = true; }
            
            return Save(sPath, CData.Dev);
        }
        //210929 pjh : Device에서 Wheel 정보 저장
        public int _Save(EWay eWy, tWhl tSrc)
        {
            string sPath = GV.PATH_WHEEL;

            if (eWy == EWay.L)
            { sPath += "Left\\"; }
            else
            { sPath += "Right\\"; }

            if (!Directory.Exists(GV.PATH_WHEEL))
            { Directory.CreateDirectory(GV.PATH_WHEEL); }

            sPath += tSrc.sWhlName + "\\WheelInfo.whl";
            return CWhl.It.Save(eWy, tSrc);
        }
        //
        public int Save(string sPath, tDev Dev)
        {
            //2020.07.11 lks
            string oldData = CCheckChange.ReadOldFile(sPath);

            int iRet = 0;
            FileInfo fi = new FileInfo(sPath);
            fi.Delete();

            StringBuilder mSB = new StringBuilder();
            CData.DevCur = sPath;

            mSB.AppendLine("[Information]");
            mSB.AppendLine("Name=" + Dev.sName);
            mSB.AppendLine();
            mSB.AppendLine("[Unit Setting]");
            mSB.AppendLine("Unit Count=" + CData.Dev.iUnitCnt);
            mSB.AppendLine();
            mSB.AppendLine("[Common Strip Information]");
            mSB.AppendLine("Short Side="            + Dev.dShSide         );
            mSB.AppendLine("Long Side="             + Dev.dLnSide         );
            mSB.AppendLine("Dual Mode="             + Dev.bDual.ToString());
            mSB.AppendLine("Measure Mode="          + CData.Dev.bMeasureMode.ToString());    // 2020.09.11 JSKim Add
            mSB.AppendLine("Left Dressing Period="  + Dev.aData[(int)EWay.L].dDrsPrid       );
            mSB.AppendLine("Right Dressing Period=" + Dev.aData[(int)EWay.R].dDrsPrid       );
            mSB.AppendLine("Left Dressing End_Dir="  + Dev.aData[(int)EWay.L].iDrs_YEnd_Dir);
            mSB.AppendLine("Right Dressing End_Dir=" + Dev.aData[(int)EWay.R].iDrs_YEnd_Dir);

            //191125 ksg :
            if (CDataOption.MeasureDf == eDfServerType.NotMeasureDf)
            { Dev.bDynamicSkip = true; }

            //20190218 ghk_dynamicfunction
            mSB.AppendLine("Pcb Height Type="  + CData.Dynamic.iHeightType   );
            mSB.AppendLine("Pcb Range="        + CData.Dynamic.dPcbRange     );
            mSB.AppendLine("Pcb Mean Range="   + CData.Dynamic.dPcbMeanRange );
            mSB.AppendLine("Dynamic Skip="     + Dev.bDynamicSkip.ToString() );
            mSB.AppendLine("Pcb RNR="          + CData.Dynamic.dPcbRnr       );
            mSB.AppendLine("Pcb Align="        + CData.Dynamic.dYAlign       );
            mSB.AppendLine("Pcb Top_Btm ="     + CData.Dev.eMoldSide         );
            mSB.AppendLine("Base Range="       + CData.Dynamic.dBaseRange    ); //20200402 jhc : DF InRail Base 측정 값 허용 범위(+/-)
            
            //20190618 ghk_dfserver
            mSB.AppendLine("DfServer Skip="    + Dev.bDfServerSkip.ToString());
            mSB.AppendLine();

            // 200803 jym : 휠파일 연동
            mSB.AppendLine("[Wheel File]");
            if(CData.CurCompany == ECompany.ASE_KR)
            {
                mSB.AppendLine("Left=" + Dev.aData[(int)EWay.L].sWhl);
                mSB.AppendLine("Right=" + Dev.aData[(int)EWay.R].sWhl);
            }
            //211011 pjh : Device에 Wheel 정보 관리하는 기능 License로 관리
            if (CDataOption.UseDeviceWheel)
            {
                // 2021.10.22 SungTae Start : [수정] (Skyworks VOC) Wheel Name은 기록하지 말라는 고객사 요청으로 수정
                if (CData.CurCompany != ECompany.SkyWorks)
                {
                    mSB.AppendLine("Left=" + Dev.aData[(int)EWay.L].sSelWhl);
                }
                // 2021.10.22 SungTae End

                mSB.AppendLine("[L - Dressing Air Cut]");
                mSB.AppendLine("Air Cut=" + a_tWhl[0].dDair);
                mSB.AppendLine();
                //211013 pjh : Replace Dressing Parameter
                if (CDataOption.IsDrsAirCutReplace)
                {
                    mSB.AppendLine("[L - Replace Dressing Air Cut]");
                    mSB.AppendLine("Air Cut=" + a_tWhl[0].dDairRep);
                    mSB.AppendLine();
                }
                mSB.AppendLine("[L - Used Parameter 01]");
                mSB.AppendLine("Mode=" + a_tWhl[0].aUsedP[0].eMode.ToString());
                mSB.AppendLine("Total Depth=" + a_tWhl[0].aUsedP[0].dTotalDep);
                mSB.AppendLine("Cycle Depth=" + a_tWhl[0].aUsedP[0].dCycleDep);
                mSB.AppendLine("Table Speed=" + a_tWhl[0].aUsedP[0].dTblSpd);
                mSB.AppendLine("Spindle Speed=" + a_tWhl[0].aUsedP[0].iSplSpd);
                mSB.AppendLine("Direction=" + a_tWhl[0].aUsedP[0].eDir.ToString());
                mSB.AppendLine();
                mSB.AppendLine("[L - Used Parameter 02]");
                mSB.AppendLine("Mode=" + a_tWhl[0].aUsedP[1].eMode.ToString());
                mSB.AppendLine("Total Depth=" + a_tWhl[0].aUsedP[1].dTotalDep);
                mSB.AppendLine("Cycle Depth=" + a_tWhl[0].aUsedP[1].dCycleDep);
                mSB.AppendLine("Table Speed=" + a_tWhl[0].aUsedP[1].dTblSpd);
                mSB.AppendLine("Spindle Speed=" + a_tWhl[0].aUsedP[1].iSplSpd);
                mSB.AppendLine("Direction=" + a_tWhl[0].aUsedP[1].eDir.ToString());
                mSB.AppendLine();
                mSB.AppendLine("[L - New Parameter 01]");
                mSB.AppendLine("Mode=" + a_tWhl[0].aNewP[0].eMode.ToString());
                mSB.AppendLine("Total Depth=" + a_tWhl[0].aNewP[0].dTotalDep);
                mSB.AppendLine("Cycle Depth=" + a_tWhl[0].aNewP[0].dCycleDep);
                mSB.AppendLine("Table Speed=" + a_tWhl[0].aNewP[0].dTblSpd);
                mSB.AppendLine("Spindle Speed=" + a_tWhl[0].aNewP[0].iSplSpd);
                mSB.AppendLine("Direction=" + a_tWhl[0].aNewP[0].eDir.ToString());
                mSB.AppendLine();
                mSB.AppendLine("[L - New Parameter 02]");
                mSB.AppendLine("Mode=" + a_tWhl[0].aNewP[1].eMode.ToString());
                mSB.AppendLine("Total Depth=" + a_tWhl[0].aNewP[1].dTotalDep);
                mSB.AppendLine("Cycle Depth=" + a_tWhl[0].aNewP[1].dCycleDep);
                mSB.AppendLine("Table Speed=" + a_tWhl[0].aNewP[1].dTblSpd);
                mSB.AppendLine("Spindle Speed=" + a_tWhl[0].aNewP[1].iSplSpd);
                mSB.AppendLine("Direction=" + a_tWhl[0].aNewP[1].eDir.ToString());
                mSB.AppendLine();
                //211210 pjh : Skyworks Wheel&Dresser Limit Device에서 편집
                mSB.AppendLine("[L - Limit]");
                mSB.AppendLine("Wheel Limit=" + a_tWhl[0].dWhlLimit);
                mSB.AppendLine("Dresser Limit=" + a_tWhl[0].dDrsLimit);
                mSB.AppendLine();
                //
                // 2021.10.22 SungTae Start : [수정] (Skyworks VOC) Wheel Name은 기록하지 말라는 고객사 요청으로 수정
                if (CData.CurCompany != ECompany.SkyWorks)
                {
                    mSB.AppendLine("Right=" + Dev.aData[(int)EWay.R].sSelWhl);
                }
                // 2021.10.22 SungTae End

                mSB.AppendLine("[R - Dressing Air Cut]");
                mSB.AppendLine("Air Cut=" + a_tWhl[1].dDair);
                mSB.AppendLine();
                //211013 pjh : Replace Dressing Parameter
                if (CDataOption.IsDrsAirCutReplace)
                {
                    mSB.AppendLine("[R - Replace Dressing Air Cut]");
                    mSB.AppendLine("Air Cut=" + a_tWhl[1].dDairRep);
                    mSB.AppendLine();
                }
                mSB.AppendLine("[R - Used Parameter 01]");
                mSB.AppendLine("Mode=" + a_tWhl[1].aUsedP[0].eMode.ToString());
                mSB.AppendLine("Total Depth=" + a_tWhl[1].aUsedP[0].dTotalDep);
                mSB.AppendLine("Cycle Depth=" + a_tWhl[1].aUsedP[0].dCycleDep);
                mSB.AppendLine("Table Speed=" + a_tWhl[1].aUsedP[0].dTblSpd);
                mSB.AppendLine("Spindle Speed=" + a_tWhl[1].aUsedP[0].iSplSpd);
                mSB.AppendLine("Direction=" + a_tWhl[1].aUsedP[0].eDir.ToString());
                mSB.AppendLine();
                mSB.AppendLine("[R - Used Parameter 02]");
                mSB.AppendLine("Mode=" + a_tWhl[1].aUsedP[1].eMode.ToString());
                mSB.AppendLine("Total Depth=" + a_tWhl[1].aUsedP[1].dTotalDep);
                mSB.AppendLine("Cycle Depth=" + a_tWhl[1].aUsedP[1].dCycleDep);
                mSB.AppendLine("Table Speed=" + a_tWhl[1].aUsedP[1].dTblSpd);
                mSB.AppendLine("Spindle Speed=" + a_tWhl[1].aUsedP[1].iSplSpd);
                mSB.AppendLine("Direction=" + a_tWhl[1].aUsedP[1].eDir.ToString());
                mSB.AppendLine();
                mSB.AppendLine("[R - New Parameter 01]");
                mSB.AppendLine("Mode=" + a_tWhl[1].aNewP[0].eMode.ToString());
                mSB.AppendLine("Total Depth=" + a_tWhl[1].aNewP[0].dTotalDep);
                mSB.AppendLine("Cycle Depth=" + a_tWhl[1].aNewP[0].dCycleDep);
                mSB.AppendLine("Table Speed=" + a_tWhl[1].aNewP[0].dTblSpd);
                mSB.AppendLine("Spindle Speed=" + a_tWhl[1].aNewP[0].iSplSpd);
                mSB.AppendLine("Direction=" + a_tWhl[1].aNewP[0].eDir.ToString());
                mSB.AppendLine();
                mSB.AppendLine("[R - New Parameter 02]");
                mSB.AppendLine("Mode=" + a_tWhl[1].aNewP[1].eMode.ToString());
                mSB.AppendLine("Total Depth=" + a_tWhl[1].aNewP[1].dTotalDep);
                mSB.AppendLine("Cycle Depth=" + a_tWhl[1].aNewP[1].dCycleDep);
                mSB.AppendLine("Table Speed=" + a_tWhl[1].aNewP[1].dTblSpd);
                mSB.AppendLine("Spindle Speed=" + a_tWhl[1].aNewP[1].iSplSpd);
                mSB.AppendLine("Direction=" + a_tWhl[1].aNewP[1].eDir.ToString());
                mSB.AppendLine();
                //211210 pjh : Skyworks Wheel&Dresser Limit Device에서 편집
                mSB.AppendLine("[R - Limit]");
                mSB.AppendLine("Wheel Limit=" + a_tWhl[1].dWhlLimit);
                mSB.AppendLine("Dresser Limit=" + a_tWhl[1].dDrsLimit);
                mSB.AppendLine();
                //
            }

            mSB.AppendLine();

            //201203 jhc : 드레싱 후 N번째 그라인딩 시 휠 소모량 보정 기능
            mSB.AppendLine("[Wheel Loss Correct]");
            for (int i = 0; i < 2; i++)
            {
                string sLR = (i==0) ? "Left" : "Right";
                for (int j = 0; j < GV.WHEEL_LOSS_CORRECT_STRIP_MAX; j++)
                {
                    mSB.AppendLine( sLR + (j+1).ToString() + "=" + Dev.aData[i].dWheelLoss[j].ToString("0.0000") );
                }
                mSB.AppendLine( sLR + "_Total_Wheel_Loss_Limit=" + Dev.aData[i].dTotalWheelLossLimit.ToString("0.0000") );
            }
            mSB.AppendLine();
            //

            mSB.AppendLine("[Common Contact Parameter]");
            mSB.AppendLine("Column Count="     + Dev.iCol     );
            mSB.AppendLine("Row Count="        + Dev.iRow     );
            mSB.AppendLine("Chip Width="       + Dev.dChipW   );
            mSB.AppendLine("Chip Height="      + Dev.dChipH   );
            mSB.AppendLine("Chip Width Gap="   + Dev.dChipWGap);
            mSB.AppendLine("Chip Height Gap="  + Dev.dChipHGap);
            mSB.AppendLine("Window Count="     + Dev.iWinCnt  );

            // 2021.08.02 SungTae Start : [추가] Measure(Before/After/One-point) 시 측정 위치에 대한 Offset 설정 추가(ASE-KR VOC)
            if (Dev.dMeasOffsetX > Dev.dChipW / 2)          { Dev.dMeasOffsetX = Dev.dChipW / 2; }
            if (Dev.dMeasOffsetX < (-1) * Dev.dChipW / 2)   { Dev.dMeasOffsetX = (-1) * Dev.dChipW / 2; }
            mSB.AppendLine("Measure Offset X=" + Dev.dMeasOffsetX);
            
            if (Dev.dMeasOffsetY > Dev.dChipH / 2)          { Dev.dMeasOffsetY = Dev.dChipH / 2; }
            if (Dev.dMeasOffsetY < (-1) * Dev.dChipH / 2)   { Dev.dMeasOffsetY = (-1) * Dev.dChipH / 2; }           
            mSB.AppendLine("Measure Offset Y=" + Dev.dMeasOffsetY);
            // 2021.08.02 SungTae End

            mSB.AppendLine();

            for (int i = 0; i < 5; i++)
            {
                mSB.AppendLine("[Common Contact Window - Start Position " + (i + 1) + "]");
                mSB.AppendLine("X=" + Dev.aWinSt[i].X);
                mSB.AppendLine("Y=" + Dev.aWinSt[i].Y);
                mSB.AppendLine();
            }

            //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
            mSB.AppendLine("[Common Contact Leading-Strip]");
            mSB.AppendLine("Leading-Strip Count=" + Dev.i18PStripCount);
            //

            mSB.AppendLine("[Unit Center Distance]");
            for (int i = 0; i < CData.Dev.iUnitCnt; i++)
            {
                mSB.AppendLine(string.Format("Unit{0}={1}", i + 1, Dev.aUnitCen[i]));
            }
            mSB.AppendLine();

            //200310 ksg : Spindle 부하
            mSB.AppendLine("[Spindle Load Factor]");
            mSB.AppendLine("LSpd Auto=" + Dev.aData[(int)EWay.L].dSpdAuto );
            mSB.AppendLine("LSpd Err="  + Dev.aData[(int)EWay.L].dSpdError);
            mSB.AppendLine("RSpd Auto=" + Dev.aData[(int)EWay.R].dSpdAuto );
            mSB.AppendLine("RSpd Err="  + Dev.aData[(int)EWay.R].dSpdError);
            mSB.AppendLine();

            // 2022.08.30 lhs Start : Spindle Current High/Low Limit 설정 (SCK+ VOC)
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)  // Device는 해당사이트만 저장/로딩 되도록
            {
                //mSB.AppendLine("[Spindle Current Limit]");
                //mSB.AppendLine("Left High Limit="   + CData.Dev.aData[(int)EWay.L].nSpdCurrHL);
                //mSB.AppendLine("Left Low Limit="    + CData.Dev.aData[(int)EWay.L].nSpdCurrLL);
                //mSB.AppendLine("Right High Limit="  + CData.Dev.aData[(int)EWay.R].nSpdCurrHL);
                //mSB.AppendLine("Right Low Limit="   + CData.Dev.aData[(int)EWay.R].nSpdCurrLL);
                //mSB.AppendLine();
            }
            // 2022.08.30 lhs End

            //201225 jhc : 고급 Grind Condition 설정/체크 기능 (Spindle Current, Table Vacuum Low Limit 설정)
            mSB.AppendLine("[Advanced Grind Condition]");
            for (int i = 0; i < 2; i++)
            {
                string sLR = (i==0) ? "Left" : "Right";
                mSB.AppendLine(sLR + " Spindle Current Range Dress Lower=" + Dev.aData[i].nSpindleCurrentDressLow.ToString());  //Lower Limit of Spindle Current when Dressing
                mSB.AppendLine(sLR + " Spindle Current Range Dress Upper=" + Dev.aData[i].nSpindleCurrentDressHigh.ToString()); //Upper Limit of Spindle Current when Dressing
                mSB.AppendLine(sLR + " Spindle Current Range Grind Lower=" + Dev.aData[i].nSpindleCurrentGrindLow.ToString());  //Lower Limit of Spindle Current when Grinding
                mSB.AppendLine(sLR + " Spindle Current Range Grind Upper=" + Dev.aData[i].nSpindleCurrentGrindHigh.ToString()); //Upper Limit of Spindle Current when Grinding
                mSB.AppendLine(sLR + " Table Vacuum Grind Limit=" + Dev.aData[i].dTableVacuumGrindLimit.ToString("0.0"));       //Lower Limit of Table Vacuum when Grinding
                mSB.AppendLine(sLR + " Table Vacuum Low Hold Time=" + Dev.aData[i].dTableVacuumLowHoldTime.ToString("0.000"));  //201231 : Table Vacuum Lower Limit Holding Time (이 시간 이상 Lower Limit 유지 시 Alarm)
            }
            mSB.AppendLine();
            //..

            mSB.AppendLine("[ETC - Magazine]");
            mSB.AppendLine("Slot Count=" + Dev.iMgzCnt  );
            mSB.AppendLine("Slot Pitch=" + Dev.dMgzPitch);
            // 2020.10.26 SungTae Start : Add by Qorvo(Strip 배출 관련)
            mSB.AppendLine("Off Slot Count=" + Dev.iOffMgzCnt);
            mSB.AppendLine("Off Slot Pitch=" + Dev.dOffMgzPitch);
            // 2020.10.26 SungTae End
            mSB.AppendLine();

            mSB.AppendLine("[ETC - Dry Part]");
            mSB.AppendLine("Use Top Blow="                  + Dev.bDryTop.ToString());
            mSB.AppendLine("Dry Speed="                     + Dev.iDryRPM           );
            mSB.AppendLine("Direction="                     + Dev.eDryDir.ToString());
            mSB.AppendLine("Dry Count="                     + Dev.iDryCnt           );
            mSB.AppendLine("Bottom Cleaning Picker Count="  + Dev.iOffpClean        );
            mSB.AppendLine("Bottom Cleaning Strip Count="   + Dev.iOffpCleanStrip   );
            // 2022.03.09 lhs Start : 사용하지 않을 경우는 Device 파일 변화 없도록 옵션 처리
            if (CDataOption.UseSprayBtmCleaner)
            {
                mSB.AppendLine("Bottom Cleaning Picker Count (Air)=" + Dev.iOffpClean_Air);
                mSB.AppendLine("Bottom Cleaning Strip Count (Air)="  + Dev.iOffpCleanStrip_Air);
            }
            // 2022.03.09 lhs End
            // 2022.07.27 lhs Start : Brush 추가
            if (CDataOption.UseBrushBtmCleaner)
            {
                mSB.AppendLine("Bottom Cleaning Brush Count="           + Dev.iOffpCleanBrush);         // Brush Clean Count
                mSB.AppendLine("Bottom Cleaning Strip Count N2="        + Dev.iOffpCleanStrip_N2);      // Strip Clean Count #2
                mSB.AppendLine("Bottom Cleaning Strip Count (Air) N2="  + Dev.iOffpCleanStrip_Air_N2);  // Strip Clean Count (Air Blow) #2 
            }
            // 2022.07.27 lhs End
            mSB.AppendLine("Dry Water Nozzle Speed="        + Dev.iDryWtNozzleRPM    );  // 2021.03.30 lhs
            mSB.AppendLine("Dry Water Nozzle Count="        + Dev.iDryWtNozzleCnt    );  // 2021.03.30 lhs
            mSB.AppendLine();
            mSB.AppendLine("[ETC - Pusher]");
            mSB.AppendLine("Fast Speed=" + Dev.dPushF);
            mSB.AppendLine("Slow Speed=" + Dev.dPushS);
            mSB.AppendLine();
            mSB.AppendLine("[ETC - Picker Vacuum Delay]");
            mSB.AppendLine("On Loader Picker="  + Dev.iPickDelayOn );
            mSB.AppendLine("On Loader Picker Place=" + Dev.iPlaceDelayOn);
            mSB.AppendLine("Off Loader Picker=" + Dev.iPickDelayOff);
            mSB.AppendLine("Off Loader Picker Place=" + Dev.iPlaceDelayOff); //20200427 jhc : <<--- 200417 jym : Unit에서는 설정된 딜레이 값 사용
            // 2022.05.24 lhs Start : Device 파일이 변경되면 타사이트에서도 서버에 레시피 등록을 다시 하는 경우가 있어 해당사이트만 저장
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            {
                mSB.AppendLine("On Loader Picker Eject="  + Dev.iEjectDelayOnP);
                mSB.AppendLine("Off Loader Picker Eject=" + Dev.iEjectDelayOffP);
            }
            // 2022.05.24 lhs End
            mSB.AppendLine();
            //20190611 ghk_onpclean
            mSB.AppendLine("[ETC - OnLoaderPicker Clean Count]");
            mSB.AppendLine("Clean Count=" + Dev.iOnpCleanCnt);
            mSB.AppendLine();
            //200408 ksg :
            mSB.AppendLine("[ETC - ReDoMaxCnt]");
            mSB.AppendLine("ReDoNum Max=" + Dev.iReDoNumMax);
            mSB.AppendLine("ReDoCnt Max=" + Dev.iReDoCntMax);
            mSB.AppendLine();
            mSB.AppendLine("[ETC - Grinding Strip Start Limit]");
            if (Dev.dStripStartLimit < 0.3) Dev.dStripStartLimit = 0.3;     //200416 pjh : Grinding Strip Start Limit
            mSB.AppendLine("Strip Start Limit=" + Dev.dStripStartLimit);    //200416 pjh : Grinding Strip Start Limit
            mSB.AppendLine();
            mSB.AppendLine("[ETC - BCR Part]");
            mSB.AppendLine("BCR Skip="         + Dev.bBcrSkip      .ToString());
            mSB.AppendLine("Ori Skip="         + Dev.bOriSkip      .ToString());
            //201103 jhc : Orientation Check 1회 Skip 기능 추가 (ASE-Kr VOC)
            if (Dev.bOriSkip)
            {
                Dev.bOriOneTimeSkipUse = false; //Orientation 검사 Skip인 경우 Orientation One Time Skip 설정 무의미 (Device 옵션)
            }
            if (!Dev.bOriOneTimeSkipUse)
            {
                CData.bOriOneTimeSkipBtnView = false; //201117 jhc : Orientation Error발생 시에만 버튼 표시, 검사 완료 후 다시 감춤
                CData.bOriOneTimeSkip = false;  //Orientation One Time Skip 설정 초기화 (현재 설정 상태)
            }
            mSB.AppendLine("Ori One Time Skip Use=" + Dev.bOriOneTimeSkipUse.ToString());
            //
            mSB.AppendLine("Ocr Skip="         + Dev.bOcrSkip      .ToString()); //190309 ksg :
            mSB.AppendLine("BCR Key In Skip="  + Dev.bBcrKeyInSkip .ToString()); //190625 ghk_dfserver
            mSB.AppendLine("BCR Second="       + Dev.bBcrSecondSkip.ToString()); //190817 ksg :
            mSB.AppendLine("Marked Skip="      + Dev.bOriMarkedSkip.ToString()); //190817 ksg :
            mSB.AppendLine("Data Shift="       + Dev.bDataShift    .ToString()); //190610 ksg :
            mSB.AppendLine("DShiftPSkip="      + Dev.bDShiftPSkip  .ToString()); //200325 ksg : Data Shift Probe Skip
            mSB.AppendLine("OCR Digit Type=" + Dev.iDigitType.ToString()); // 2022-05-26, jhLee : OCR 자릿수 0:10자리 (기존),  1:14자리 (신규)
            mSB.AppendLine();

            mSB.AppendLine("[ETC - After Data Offset]");
            mSB.AppendLine("Left Offset=" + Dev.aFake[0]);
            mSB.AppendLine("Right Offset=" + Dev.aFake[1]);
            mSB.AppendLine();

            // 2021.03.02 SungTae Start : Device 별로 Table Cleaning Velocity 관리할 수 있도록 고객사 요청에 따른 추가(ASE-KR)
            if (CData.CurCompany == ECompany.ASE_KR)
            {
                mSB.AppendLine("[ETC - Table Clean Velocity]");
                mSB.AppendLine("Left Grind Y Velocity="  + Dev.aTblCleanVel[0]);
                mSB.AppendLine("Right Grind Y Velocity=" + Dev.aTblCleanVel[1]);
                mSB.AppendLine();
            }
            // 2021.03.02 SungTae Start : Device 별로 Table Cleaning Velocity 관리할 수 있도록 고객사 요청에 따른 추가(ASE-KR)

            // 2020.09.08 SungTae : 3 Step 추가
            if (CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)  iStepMaxCnt = GV.StepMaxCnt;
            else                                                    iStepMaxCnt = GV.StepMaxCnt_3;

            for (int i = 0; i < 2; i++)
            {
                string sDir = "Left";
                if (i == 1)
                { sDir = "Right"; }

                mSB.AppendLine("[" + sDir + " Data - Strip]");
                mSB.AppendLine("Total Thickness=" + Dev.aData[i].dTotalTh);
                mSB.AppendLine("PCB Thickness="   + Dev.aData[i].dPcbTh  );
                mSB.AppendLine("Mold Thickness="  + Dev.aData[i].dMoldTh );
                mSB.AppendLine("Air Cut Depth="   + Dev.aData[i].dAir    );
                mSB.AppendLine();
                mSB.AppendLine("[" + sDir + " Data - Step]");
                mSB.AppendLine("Grinding Mode="     + Dev.aData[i].eGrdMod   .ToString());
                mSB.AppendLine("TDStart Mode="      + Dev.aData[i].eStartMode.ToString());      //190502 ksg :
                mSB.AppendLine("BaseOn Thickness="  + Dev.aData[i].eBaseOnThick.ToString());    // 2021.07.27 lhs (SCK VOC)

                // 2022.09.23 lhs Start : Rough1, 첫번째 Griding시에 Z축 StartPos에 cycle depth 적용 
                if (CDataOption.UseDevR1Option)
                {
                    mSB.AppendLine("bAppCylDepOnFirst=" + Dev.aData[i].bAppCylDepOnFirst);
                    mSB.AppendLine("bAppLargeCylDep="   + Dev.aData[i].bAppLargeCylDep);
                }
                // 2022.09.23 lhs End

                // 2022.08.10 SungTae Start : [수정] (ASE-KR 개발건) 설정된 Step 수량을 제외한 나머지 Step UI 비활성화
                if (CData.CurCompany == ECompany.ASE_KR)
                {
                    mSB.AppendLine("Fixed Step Cnt=" + Dev.aData[i].nFixStepCnt);
                } 
                // 2022.08.16 SungTae End
                mSB.AppendLine();

                for (int j = 0; j < iStepMaxCnt; j++)       // 2020.09.08 SungTae : 3 Step  기능 추가
                {
                    mSB.AppendLine("[" + sDir + " Data - Step " + (j + 1) + "]");
                    mSB.AppendLine("Use="                  + Dev.aData[i].aSteps[j].bUse     .ToString());
                    mSB.AppendLine("Mode="                 + Dev.aData[i].aSteps[j].eMode    .ToString());
                    mSB.AppendLine("Total Depth="          + Dev.aData[i].aSteps[j].dTotalDep           );
                    mSB.AppendLine("Cycle Depth="          + Dev.aData[i].aSteps[j].dCycleDep           );
                    mSB.AppendLine("Table Speed="          + Dev.aData[i].aSteps[j].dTblSpd             );
                    mSB.AppendLine("Spindle Speed="        + Dev.aData[i].aSteps[j].iSplSpd             );
                    mSB.AppendLine("Start Direction="      + Dev.aData[i].aSteps[j].eDir     .ToString());
                    mSB.AppendLine("ReGrinding Skip="      + Dev.aData[i].aSteps[j].bReSkip             );
                    mSB.AppendLine("ReGrinding Judgement=" + Dev.aData[i].aSteps[j].dReJud              );
                    //201023 JSKim : Over Grinding Correction - Grinding Count Correction 기능
                    mSB.AppendLine("OverGrinding Correction Use=" + Dev.aData[i].aSteps[j].bOverGrdCorrectionUse.ToString());
                    //201125 jhc : Grinding Step별 Correct 기능
                    mSB.AppendLine("Grinding Correct Depth=" + Dev.aData[i].aSteps[j].dCorrectDepth.ToString());
                    //
                    mSB.AppendLine();
                }

                mSB.AppendLine("[" + sDir + " Data - Step Fine Setting]");
                mSB.AppendLine("Fine Start Mode="    + Dev.aData[i].eFine.ToString());
                mSB.AppendLine("Compensation Value=" + Dev.aData[i].dCpen           );
                mSB.AppendLine();
                mSB.AppendLine("[" + sDir + " Data - Measure Limit]");
                mSB.AppendLine("Before Limit=" + Dev.aData[i].dBfLimit);
                mSB.AppendLine("BeforeLower Limit=" + Dev.aData[i].dBfLimitLower);      // 2021-03-08, jhLee, for Skyworks
                mSB.AppendLine("After Limit="     + Dev.aData[i].dAfLimit );
                mSB.AppendLine("TTV Limit="       + Dev.aData[i].dTTV     );
                mSB.AppendLine("One Point Limit=" + Dev.aData[i].dOneLimit);
                mSB.AppendLine("One Point Over=" + Dev.aData[i].dOneOver);    // 2020.08.19 JSKim

                // 2022.08.10 SungTae Start : [추가] (ASE-KR 개조건)
                // 최종 Target 두께 별도 입력하여 Grinding 최종 Target과 일치하지 않을 경우 Alarm 발생 기능 추가 개발 요청건
                if (CData.CurCompany == ECompany.ASE_KR)
                {
                    mSB.AppendLine("Final Target Thickness=" + Dev.aData[i].dFinalTarget);
                }
                // 2022.08.10 SungTae End

                mSB.AppendLine();
                mSB.AppendLine("[" + sDir + " Data - Top Cleaner]");
                mSB.AppendLine("Bubble Speed=" + Dev.aData[i].dTpBubSpd );
                mSB.AppendLine("Bubble Count=" + Dev.aData[i].iTpBubCnt );
                mSB.AppendLine("Air Speed="    + Dev.aData[i].dTpAirSpd );
                mSB.AppendLine("Air Count="    + Dev.aData[i].iTpAirCnt );
                mSB.AppendLine("Sponge Speed=" + Dev.aData[i].dTpSpnSpd );
                mSB.AppendLine("Sponge Count=" + Dev.aData[i].iTpSpnCnt );
                mSB.AppendLine("Probe Offset=" + Dev.aData[i].dPrbOffset); //190502 ksg :
                mSB.AppendLine();
                // 2020.08.31 JSKim St
                mSB.AppendLine("[" + sDir + " Data - Measure Strip One Point]");
                mSB.AppendLine("X Position Fix=" + Dev.aData[i].bOnePointXPosFix.ToString());
                mSB.AppendLine("Chip Point Win=" + Dev.aData[i].iOnePointWin);
                mSB.AppendLine("Chip Point Col=" + Dev.aData[i].iOnePointCol);
                mSB.AppendLine("Chip Point Row=" + Dev.aData[i].iOnePointRow);
                mSB.AppendLine();
                // 2020.08.31 JSKim Ed

                int iC = Dev.iCol;
                int iR = Dev.iRow;
                //koo 191104 Speed Read/Write;
                mSB.AppendLine("[" + sDir + " Data - Before Contact Position List]");
                for (int c = 0; c < iC; c++)
                {
                    for (int r = 0; r < iR * Dev.iWinCnt; r++)
                    {
                        mSB.AppendLine(r.ToString() + "," + c.ToString() + " Use=" + Dev.aData[i].aPosBf[r, c].bUse.ToString());
                        mSB.AppendLine(r.ToString() + "," + c.ToString() + " Use18P=" + Dev.aData[i].aPosBf[r, c].bUse18P.ToString()); //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                        mSB.AppendLine(r.ToString() + "," + c.ToString() + " X=" + Dev.aData[i].aPosBf[r, c].dX.ToString());
                        mSB.AppendLine(r.ToString() + "," + c.ToString() + " Y=" + Dev.aData[i].aPosBf[r, c].dY.ToString());
                    }
                }
                mSB.AppendLine();


                mSB.AppendLine("[" + sDir + " Data - After Contact Position List]");
                for (int c = 0; c < iC; c++)
                {
                    for (int r = 0; r < iR * Dev.iWinCnt; r++)
                    {
                        mSB.AppendLine(r.ToString() + "," + c.ToString() + " Use=" + Dev.aData[i].aPosAf[r, c].bUse.ToString());
                        mSB.AppendLine(r.ToString() + "," + c.ToString() + " Use18P=" + Dev.aData[i].aPosAf[r, c].bUse18P.ToString()); //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                        mSB.AppendLine(r.ToString() + "," + c.ToString() + " X=" + Dev.aData[i].aPosAf[r, c].dX.ToString());
                        mSB.AppendLine(r.ToString() + "," + c.ToString() + " Y=" + Dev.aData[i].aPosAf[r, c].dY.ToString());
                    }
                }
                mSB.AppendLine();

                if (CDataOption.Package == ePkg.Unit)
                {
                    mSB.AppendLine("[" + sDir + " Data - Before Contact Unit Position List]");
                    for (int c = 0; c < iC; c++)
                    {
                        for (int r = 0; r < iR; r++)
                        {
                            mSB.AppendLine(r.ToString() + "," + c.ToString() + " Use=" + Dev.aData[i].aPosBf[r, c].bUse.ToString());
                            mSB.AppendLine(r.ToString() + "," + c.ToString() + " X=" + Dev.aData[i].aPosBf[r, c].dX.ToString());
                            mSB.AppendLine(r.ToString() + "," + c.ToString() + " Y=" + Dev.aData[i].aPosBf[r, c].dY.ToString());
                            mSB.AppendLine(r.ToString() + "," + c.ToString() + " Y1=" + Dev.aData[i].aPosBf[r, c].aY[0].ToString());
                            mSB.AppendLine(r.ToString() + "," + c.ToString() + " Y2=" + Dev.aData[i].aPosBf[r, c].aY[1].ToString());
                            mSB.AppendLine(r.ToString() + "," + c.ToString() + " Y3=" + Dev.aData[i].aPosBf[r, c].aY[2].ToString());
                            mSB.AppendLine(r.ToString() + "," + c.ToString() + " Y4=" + Dev.aData[i].aPosBf[r, c].aY[3].ToString());
                        }
                    }
                    mSB.AppendLine();


                    mSB.AppendLine("[" + sDir + " Data - After Contact Unit Position List]");
                    for (int c = 0; c < iC; c++)
                    {
                        for (int r = 0; r < iR; r++)
                        {
                            mSB.AppendLine(r.ToString() + "," + c.ToString() + " Use=" + Dev.aData[i].aPosAf[r, c].bUse.ToString());
                            mSB.AppendLine(r.ToString() + "," + c.ToString() + " X=" + Dev.aData[i].aPosAf[r, c].dX.ToString());
                            mSB.AppendLine(r.ToString() + "," + c.ToString() + " Y=" + Dev.aData[i].aPosAf[r, c].dY.ToString());
                            mSB.AppendLine(r.ToString() + "," + c.ToString() + " Y1=" + Dev.aData[i].aPosAf[r, c].aY[0].ToString());
                            mSB.AppendLine(r.ToString() + "," + c.ToString() + " Y2=" + Dev.aData[i].aPosAf[r, c].aY[1].ToString());
                            mSB.AppendLine(r.ToString() + "," + c.ToString() + " Y3=" + Dev.aData[i].aPosAf[r, c].aY[2].ToString());
                            mSB.AppendLine(r.ToString() + "," + c.ToString() + " Y4=" + Dev.aData[i].aPosAf[r, c].aY[3].ToString());
                        }
                    }
                    mSB.AppendLine();
                }
            }

            mSB.AppendLine("[Set Position - On Loader]");
            mSB.AppendLine("X - Align Position="            + Dev.dOnL_X_Algn    );
            mSB.AppendLine("Y - Wait Position="             + Dev.dOnL_Y_Wait    );
            mSB.AppendLine("Y - MGZ Barcode Position="      + Dev.dOnL_Y_MgzBcr  ); //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)
            mSB.AppendLine("Z - Strip Entry Down Position=" + Dev.dOnL_Z_Entry_Dn);
            mSB.AppendLine("Z - Strip Entry Up Position="   + Dev.dOnL_Z_Entry_Up);
            mSB.AppendLine("Z - MGZ Barcode Position="      + Dev.dOnL_Z_MgzBcr  ); //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)
            mSB.AppendLine();
            mSB.AppendLine("[Set Position - In Rail]");
            mSB.AppendLine("X - Pick Position="        + Dev.dInr_X_Pick  );
            mSB.AppendLine("X - Barcode Position="     + Dev.dInr_X_Bcr   );
            mSB.AppendLine("X - Orientation Position=" + Dev.dInr_X_Ori   ); //190211 ksg :
            mSB.AppendLine("X - Vision Position="      + Dev.dInr_X_Vision);
            mSB.AppendLine("X - Align Position="       + Dev.dInr_X_Align );
            //20190220 ghk_dynamicfunction
            mSB.AppendLine("X - Dynamic Pos1=" + Dev.dInr_X_DynamicPos1);
            mSB.AppendLine("X - Dynamic Pos2=" + Dev.dInr_X_DynamicPos2);
            mSB.AppendLine("X - Dynamic Pos3=" + Dev.dInr_X_DynamicPos3);
            //20200328 jhc : 다이나믹 펑션 측정 포인트 최대 5개로 확대
            mSB.AppendLine("X - Dynamic Pos4=" + Dev.dInr_X_DynamicPos4);
            mSB.AppendLine("X - Dynamic Pos5=" + Dev.dInr_X_DynamicPos5);
            //mSB.AppendLine("X - Dynamic Position Number=" + Dev.iDynamicPosNum); //DF 측정 포인트 수는 DF Position X 설정 값으로 판단함
            CheckDfInrXPos(); //다이나믹 펑션 사용하는 조건일 경우 InRail X축 DF Position 설정 상태 체크
            //

            //
            //190827 ksg : system 계산 안함.
            //Dev.dInr_Y_Align = CData.SPos.dINR_Y_Align - ((Dev.dShSide - GV.STRIP_DEF_SHORT) / 2);
            mSB.AppendLine("Y - Align Position=" + Dev.dInr_Y_Align);
            mSB.AppendLine();
            mSB.AppendLine("[Set Position - On Loader Picker]");
            mSB.AppendLine("X - Wait Position="  + Dev.dOnP_X_Wait );
            mSB.AppendLine("Z - Pick Position="  + Dev.dOnP_Z_Pick );
            mSB.AppendLine("Z - Place Position=" + Dev.dOnP_Z_Place);
            mSB.AppendLine("Z - Slow Position="  + Dev.dOnP_Z_Slow );
            //20190611 ghk_onpclean
            mSB.AppendLine("X - Clean Position=" + Dev.dOnp_X_Clean);
            mSB.AppendLine("Z - Clean Position=" + Dev.dOnp_Z_Clean);
            //20190604 ghk_onpbcr
            mSB.AppendLine("X - Barcode Position="           + Dev.dOnp_X_Bcr    );
            mSB.AppendLine("X - Orientation Position="       + Dev.dOnp_X_Ori    );
            mSB.AppendLine("Z - Barcode Position="           + Dev.dOnp_Z_Bcr    );
            mSB.AppendLine("Z - Orientation Position="       + Dev.dOnp_Z_Ori    );
            mSB.AppendLine("Y - Barcode Position="           + Dev.dOnp_Y_Bcr    );
            mSB.AppendLine("Y - Orientation Position="       + Dev.dOnp_Y_Ori    );
            mSB.AppendLine("X - Barcode Second Position="    + Dev.dOnp_X_BcrSecon);//190821 ksg :
            mSB.AppendLine("Y - Barcode Second Position="    + Dev.dOnp_Y_BcrSecon);
            //201230 jhc : 2D 코드 판독 실패 Alarm 발생 전 제3의 위치(OCR 위치)로 카메라 이동 기능
            mSB.AppendLine("X - Barcode Error Position="     + Dev.dOnp_X_BcrErr);
            mSB.AppendLine("Y - Barcode Error Position="     + Dev.dOnp_Y_BcrErr);
            mSB.AppendLine("Z - Barcode Error Position="     + Dev.dOnp_Z_BcrErr);
            //
            //20191010 ghk_manual_bcr
            mSB.AppendLine("X - Left Table Barcode Position="       + Dev.dOnp_X_Bcr_TbL);
            mSB.AppendLine("X - Left Table Orientation Position="   + Dev.dOnp_X_Ori_TbL);
            mSB.AppendLine("X - Right Table Barcode Position="      + Dev.dOnp_X_Bcr_TbR);
            mSB.AppendLine("X - Right Table Orientation Position="  + Dev.dOnp_X_Ori_TbR);
            mSB.AppendLine("Z - Left Table Barcode Position="       + Dev.dOnp_Z_Bcr_TbL);
            mSB.AppendLine("Z - Left Table Orientation Position="   + Dev.dOnp_Z_Ori_TbL);
            mSB.AppendLine("Z - Right Table Barcode Position="      + Dev.dOnp_Z_Bcr_TbR);
            mSB.AppendLine("Z - Right Table Orientation Position="  + Dev.dOnp_Z_Ori_TbR);
            mSB.AppendLine("Y - Left Table Barcode Position="       + Dev.dOnp_Y_Bcr_TbL);
            mSB.AppendLine("Y - Left Table Orientation Position="   + Dev.dOnp_Y_Ori_TbL);
            mSB.AppendLine("Y - Right Table Barcode Position="      + Dev.dOnp_Y_Bcr_TbR);
            mSB.AppendLine("Y - Right Table Orientation Position="  + Dev.dOnp_Y_Ori_TbR);
            //211022 syc : Onp Pick Up Offset
            mSB.AppendLine("Z - Pick Up Offset=" + Dev.dOnp_Z_PickOffset);
            //
            mSB.AppendLine();
            mSB.AppendLine("[Set Position - Grind Left]");
            mSB.AppendLine("Y - Grind Start Position=" + Dev.aGrd_Y_Start   [0]);
            mSB.AppendLine("Y - Grind End Position="   + Dev.aGrd_Y_End     [0]);
            //20191011 ghk_manual_bcr
            mSB.AppendLine("Y - Grind Ori Position="   + Dev.aGrd_Y_Ori     [0]);
            mSB.AppendLine();
            mSB.AppendLine("[Set Position - Grind Right]");
            mSB.AppendLine("Y - Grind Start Position="  + Dev.aGrd_Y_Start  [1]);
            mSB.AppendLine("Y - Grind End Position="    + Dev.aGrd_Y_End    [1]);
            //20191011 ghk_manual_bcr
            mSB.AppendLine("Y - Grind Ori Position="    + Dev.aGrd_Y_Ori    [1]);
            mSB.AppendLine();

            mSB.AppendLine("[Set Position - Off Loader Picker]");
            mSB.AppendLine("X - Clean Start Position="   + Dev.dOffP_X_ClnStart);
            mSB.AppendLine("X - Clean End Position="     + Dev.dOffP_X_ClnEnd  );
            // 2021.12.13 SungTae Start : [추가] 
            // 접수일 : 2021.12.12 (ASE-KR Maint팀 문현수 대리)
            // Device Change 시 Center Position이 "0" 표기되어 Auto Run 중 Strip Cleaning을 제대로 못하는 현상 발생
            if (Dev.dOffP_X_ClnCenter == 0)
            {
                Dev.dOffP_X_ClnCenter = Dev.dOffP_X_ClnStart - (Dev.dOffP_X_ClnStart - Dev.dOffP_X_ClnEnd) / 2;
            }
            mSB.AppendLine("X - Clean Center Position=" + Dev.dOffP_X_ClnCenter);
            // 2021.12.13 SungTae End
            // 2022.07.28 lhs Start : Brush 추가
            if (CDataOption.UseBrushBtmCleaner)
            {
                mSB.AppendLine("X - Brush Clean Start Position=" + Dev.dOffP_X_ClnStart_Brush);
                mSB.AppendLine("X - Brush Clean End Position="   + Dev.dOffP_X_ClnEnd_Brush);
            }
            // 2022.07.28 lhs End

            mSB.AppendLine("Z - Pick Position="          + Dev.dOffP_Z_Pick    );
            mSB.AppendLine("Z - Place Position="         + Dev.dOffP_Z_Place   );
            mSB.AppendLine("Z - Slow Position="          + Dev.dOffP_Z_Slow    );
            mSB.AppendLine("Z - Place Down Position="    + Dev.dOffP_Z_PlaceDn );
            // 2021.02.27 SungTae Start : 고객사(ASE-KR) 요청으로 Device에서 관리 위해 추가
            mSB.AppendLine("Z - Bottom Clean Start Position=" + Dev.dOffP_Z_ClnStart);
            mSB.AppendLine("Z - Strip Clean Start Position="  + Dev.dOffP_Z_StripClnStart);
            // 2021.02.27 SungTae End
            // 2022.07.28 lhs Start : Brush 추가
            if (CDataOption.UseBrushBtmCleaner)
            {
                mSB.AppendLine("Z - Strip Brush Clean Start Position=" + Dev.dOffP_Z_StripClnStart_Brush);
            }
            // 2022.07.28 lhs End
            mSB.AppendLine();

			mSB.AppendLine("[Set Position - Dry]");
            mSB.AppendLine("R - Check 1 Position="       + Dev.dDry_R_Check1   );
            mSB.AppendLine("R - Check 2 Position="       + Dev.dDry_R_Check2   );
            // 200312-jym : Unit 방식 포지션 추가
            mSB.AppendLine("Y - Check Carrier Position=" + Dev.dDry_Car_Check);
            mSB.AppendLine("Y - Check Unit Position=" + Dev.dDry_Unit_Check);
            mSB.AppendLine("Y - Start Position=" + Dev.dDry_Start);
            mSB.AppendLine("Y - End Position=" + Dev.dDry_End);
            mSB.AppendLine();
            mSB.AppendLine("[Set Position - Off Loader]");
            mSB.AppendLine("X - Align Position="                     + Dev.dOffL_X_Algn   );
            mSB.AppendLine("Y - Wait Position="                      + Dev.dOffL_Y_Wait   );
            mSB.AppendLine("Y - MGZ Barcode Position="               + Dev.dOffL_Y_MgzBcr ); //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)
            mSB.AppendLine("Z - Strip Top Receive Down Position="    + Dev.dOffL_Z_TRcv_Dn);
            mSB.AppendLine("Z - Strip Top Receive Up Position="      + Dev.dOffL_Z_TRcv_Up);
            mSB.AppendLine("Z - Strip Bottom Receive Down Position=" + Dev.dOffL_Z_BRcv_Dn);
            mSB.AppendLine("Z - Strip Bottom Receive Up Position="   + Dev.dOffL_Z_BRcv_Up);
            mSB.AppendLine("Z - MGZ Barcode Position="               + Dev.dOffL_Z_MgzBcr ); //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)

            //210824 syc : 2004U
            mSB.AppendLine();
            mSB.AppendLine("[IV2 - Parameter]");
            mSB.AppendLine("ONP1 Parameter=" + Dev.sIV2_ONP1_Para);
            mSB.AppendLine("ONP2 Parameter=" + Dev.sIV2_ONP2_Para);
            mSB.AppendLine("OFP1 Parameter=" + Dev.sIV2_OFP1_Para);
            mSB.AppendLine("OFP2 Parameter=" + Dev.sIV2_OFP2_Para);
            mSB.AppendLine("OFPCover Parameter=" + Dev.sIV2_OFPCover_Para);

            mSB.AppendLine("X ONP1=" + Dev.dIV2_ONP1_X);
            mSB.AppendLine("Y ONP1=" + Dev.dIV2_ONP1_Y);
            mSB.AppendLine("Z ONP1=" + Dev.dIV2_ONP1_Z);

            mSB.AppendLine("X ONP2=" + Dev.dIV2_ONP2_X);
            mSB.AppendLine("Y ONP2=" + Dev.dIV2_ONP2_Y);
            mSB.AppendLine("Z ONP2=" + Dev.dIV2_ONP2_Z);

            mSB.AppendLine("X OFP1=" + Dev.dIV2_OFP1_X);
            mSB.AppendLine("Z OFP1=" + Dev.dIV2_OFP1_Z);

            mSB.AppendLine("X OFP2=" + Dev.dIV2_OFP2_X);
            mSB.AppendLine("Z OFP2=" + Dev.dIV2_OFP2_Z);

            mSB.AppendLine("X OFP Cover=" + Dev.dIV2_OFPCover_X);
            mSB.AppendLine("Z OFP Cover=" + Dev.dIV2_OFPCover_Z);

            mSB.AppendLine("ONP2 Use=" + Dev.bIV2_ONP2_Use.ToString());
            mSB.AppendLine("OFP2 Use=" + Dev.bIV2_OFP2_Use.ToString());
            //



            //2020.07.11 lks
            CCheckChange.CheckChanged("DEVICE", sPath, oldData, mSB.ToString());

            //200117-maeng : 정리된 내용 파일 저장
            CLog.Check_File_Access(sPath, mSB.ToString(), false);

            // Last Status Save
            CLast.Save();

            return iRet;
        }

        public int Load(string sPath, bool Init = false)
        {
            int iRet = 0;
            string sSc = "";
            string Temp = "";
            //210928 pjh : 디바이스 변경 시 Log 추가
            string sLog = "";
            //

            if (File.Exists(sPath) == false)
            { return -1; } // Error code

            // 불러오기전 디바이스 구조체 초기화 (배열 메모리 할당)
            if (Init)
            {
                InitDev(out CData.Dev);
            }
            CIni mIni = new CIni(sPath);

            if (mIni.Read(sSc, "Dual Mode") == "Dual")
            {
                if (CData.Opt.aTblSkip[(int)EWay.L] || CData.Opt.aTblSkip[(int)EWay.R])
                {
                    CMsg.Show(eMsg.Error, "Error", "Can't Change Device Plese Check Option of Table Skip");
                    return 0;
                }
            }

            int FindDot = sPath.IndexOf(".");
            int Lastsp  = sPath.LastIndexOf("\\");

            Temp = sPath.Substring(Lastsp + 1, FindDot - Lastsp - 1);

            sSc = "Information";
            //190107 ksg : 파일명으로 수정함.
            //CData.Dev.sName = mIni.Read(sSc, "Name");
            CData.Dev.sName = Temp;
            CData.Dev.dtLast = File.GetLastWriteTime(sPath);

            sSc = "Unit Setting";
            CData.Dev.iUnitCnt = mIni.ReadI(sSc, "Unit Count");
            if (CData.Dev.iUnitCnt == 0)
            { CData.Dev.iUnitCnt = 4; }

            sSc = "Common Strip Information";
            double.TryParse(mIni.Read(sSc, "Short Side"),               out CData.Dev.dShSide);
            double.TryParse(mIni.Read(sSc, "Long Side"),                out CData.Dev.dLnSide);
            Enum.TryParse(  mIni.Read(sSc, "Dual Mode"),                out CData.Dev.bDual);
            bool.TryParse(  mIni.Read(sSc, "Measure Mode"),             out CData.Dev.bMeasureMode);    // 2020.09.11 JSKim Add
            double.TryParse(mIni.Read(sSc, "Left Dressing Period"),     out CData.Dev.aData[(int)EWay.L].dDrsPrid);
            double.TryParse(mIni.Read(sSc, "Right Dressing Period"),    out CData.Dev.aData[(int)EWay.R].dDrsPrid);
            int.TryParse(   mIni.Read(sSc, "Left Dressing End_Dir"),    out CData.Dev.aData[(int)EWay.L].iDrs_YEnd_Dir);
            int.TryParse(   mIni.Read(sSc, "Right Dressing End_Dir"),   out CData.Dev.aData[(int)EWay.R].iDrs_YEnd_Dir);

            //20190218 ghk_dynamicfunction
            int.TryParse(   mIni.Read(sSc, "Pcb Height Type"),          out CData.Dynamic.iHeightType);
            double.TryParse(mIni.Read(sSc, "Pcb Range"),                out CData.Dynamic.dPcbRange);
            double.TryParse(mIni.Read(sSc, "Pcb Mean Range"),           out CData.Dynamic.dPcbMeanRange);
            bool.TryParse(  mIni.Read(sSc, "Dynamic Skip"),             out CData.Dev.bDynamicSkip);
            double.TryParse(mIni.Read(sSc, "Pcb RNR"),                  out CData.Dynamic.dPcbRnr);     //190511 ksg :
            double.TryParse(mIni.Read(sSc, "Pcb Align"),                out CData.Dynamic.dYAlign);     //190516 ksg :
            Enum.TryParse(  mIni.Read(sSc, "Pcb Top_Btm"),              out CData.Dev.eMoldSide);       //190516 ksg :
            double.TryParse(mIni.Read(sSc, "Base Range"),               out CData.Dynamic.dBaseRange);  //20200402 jhc : DF InRail Base 측정 값 허용 범위(+/-)
            //20190618 ghk_dfserver
            bool.TryParse(  mIni.Read(sSc, "DfServer Skip"), out CData.Dev.bDfServerSkip);

            // 2021.03.16 SungTae : Qorvo_RT와 Qorvo_NC 추가에 따른 수정
            if (CData.CurCompany != ECompany.Qorvo      && 
                CData.CurCompany != ECompany.Qorvo_DZ   &&
                CData.CurCompany != ECompany.Qorvo_RT   &&
                CData.CurCompany != ECompany.Qorvo_NC   && 
                CData.CurCompany != ECompany.SkyWorks   &&
                CData.CurCompany != ECompany.ASE_K12    && 
                CData.CurCompany != ECompany.ASE_KR     &&
                CData.CurCompany != ECompany.SST        && 
                CData.CurCompany != ECompany.USI        &&
                CData.CurCompany != ECompany.SCK        && 
                CData.CurCompany != ECompany.JSCK       && 
                CData.CurCompany != ECompany.JCET       )
            {
                CData.Dev.bDynamicSkip = true; //190616 ksg :
                CData.Dev.bDfServerSkip = true;
            }
            //
            //20190618 ghk_dfserver
            //if((CData.CurCompany != eCompany.AseKr) && (CData.CurCompany != eCompany.AseK26))
            if (CDataOption.eDfserver == eDfserver.NotUse)
            {
                CData.Dev.bDfServerSkip = true;
            }
            //

            //20191105 ghk_dfserver_notuse_df
            if (CDataOption.MeasureDf == eDfServerType.NotMeasureDf)
            {
                CData.Dev.bDynamicSkip = CData.Dev.bDfServerSkip;
            }

            // 200803 jym : 휠파일 연동
            sSc = "Wheel File";
            if(CData.CurCompany == ECompany.ASE_KR)
            {
                CData.Dev.aData[(int)EWay.L].sWhl = mIni.Read(sSc, "Left");
                CData.Dev.aData[(int)EWay.R].sWhl = mIni.Read(sSc, "Right");
            }
            //else if(CData.CurCompany == ECompany.SkyWorks)
            //211011 pjh : Device에 Wheel 정보 관리하는 기능 License로 관리
            if (CDataOption.UseDeviceWheel)
            {
                // 2021.10.22 SungTae Start : [수정] (Skyworks VOC) Wheel Name은 기록하지 말라는 고객사 요청으로 수정
                if (CData.CurCompany != ECompany.SkyWorks)
                {
                    CData.Dev.aData[(int)EWay.L].sSelWhl = mIni.Read(sSc, "Left");
                }
                else
                {//211118 pjh : 추가
                    CData.Dev.aData[(int)EWay.L].sSelWhl = CData.Whls[(int)EWay.L].sWhlName;
                }
                // 2021.10.22 SungTae End

                if (a_tWhl[0].aUsedP != null)
                {
                    if (mIni.ReadD("L - Used Parameter 01", "Total Depth").ToString() != "0")
                    {
                        sSc = "L - Dressing Air Cut";
                        a_tWhl[0].dDair = mIni.ReadD(sSc, "Air Cut");
                        if (a_tWhl[0].dDair == 0) { a_tWhl[0].dDair = CData.Whls[0].dDair; }

                        //211013 pjh : Replace Dressing Parameter
                        if (CDataOption.IsDrsAirCutReplace)
                        {
                            sSc = "L - Replace Dressing Air Cut";
                            a_tWhl[0].dDairRep = mIni.ReadD(sSc, "Air Cut");
                            if (a_tWhl[0].dDairRep == 0) { a_tWhl[0].dDairRep = CData.Whls[0].dDairRep; }
                        }

                        sSc = "L - Used Parameter 01";
                        a_tWhl[0].aUsedP[0].eMode = (eStepMode)Enum.Parse(typeof(eStepMode), mIni.Read(sSc, "Mode"));
                        a_tWhl[0].aUsedP[0].dTotalDep = mIni.ReadD(sSc, "Total Depth");
                        a_tWhl[0].aUsedP[0].dCycleDep = mIni.ReadD(sSc, "Cycle Depth");
                        a_tWhl[0].aUsedP[0].dTblSpd = mIni.ReadD(sSc, "Table Speed");
                        a_tWhl[0].aUsedP[0].iSplSpd = mIni.ReadI(sSc, "Spindle Speed");
                        a_tWhl[0].aUsedP[0].eDir = (eStartDir)Enum.Parse(typeof(eStartDir), mIni.Read(sSc, "Direction"));

                        sSc = "L - Used Parameter 02";
                        a_tWhl[0].aUsedP[1].eMode = (eStepMode)Enum.Parse(typeof(eStepMode), mIni.Read(sSc, "Mode"));
                        a_tWhl[0].aUsedP[1].dTotalDep = mIni.ReadD(sSc, "Total Depth");
                        a_tWhl[0].aUsedP[1].dCycleDep = mIni.ReadD(sSc, "Cycle Depth");
                        a_tWhl[0].aUsedP[1].dTblSpd = mIni.ReadD(sSc, "Table Speed");
                        a_tWhl[0].aUsedP[1].iSplSpd = mIni.ReadI(sSc, "Spindle Speed");
                        a_tWhl[0].aUsedP[1].eDir = (eStartDir)Enum.Parse(typeof(eStartDir), mIni.Read(sSc, "Direction"));

                        sSc = "L - New Parameter 01";
                        a_tWhl[0].aNewP[0].eMode = (eStepMode)Enum.Parse(typeof(eStepMode), mIni.Read(sSc, "Mode"));
                        a_tWhl[0].aNewP[0].dTotalDep = mIni.ReadD(sSc, "Total Depth");
                        a_tWhl[0].aNewP[0].dCycleDep = mIni.ReadD(sSc, "Cycle Depth");
                        a_tWhl[0].aNewP[0].dTblSpd = mIni.ReadD(sSc, "Table Speed");
                        a_tWhl[0].aNewP[0].iSplSpd = mIni.ReadI(sSc, "Spindle Speed");
                        a_tWhl[0].aNewP[0].eDir = (eStartDir)Enum.Parse(typeof(eStartDir), mIni.Read(sSc, "Direction"));

                        sSc = "L - New Parameter 02";
                        a_tWhl[0].aNewP[1].eMode = (eStepMode)Enum.Parse(typeof(eStepMode), mIni.Read(sSc, "Mode"));
                        a_tWhl[0].aNewP[1].dTotalDep = mIni.ReadD(sSc, "Total Depth");
                        a_tWhl[0].aNewP[1].dCycleDep = mIni.ReadD(sSc, "Cycle Depth");
                        a_tWhl[0].aNewP[1].dTblSpd = mIni.ReadD(sSc, "Table Speed");
                        a_tWhl[0].aNewP[1].iSplSpd = mIni.ReadI(sSc, "Spindle Speed");
                        a_tWhl[0].aNewP[1].eDir = (eStartDir)Enum.Parse(typeof(eStartDir), mIni.Read(sSc, "Direction"));

                        //211210 pjh : Skyworks Wheel&Dresser Limit Device에서 편집
                        sSc = "L - Limit";
                        CData.Whls[0].dWhlLimit = a_tWhl[0].dWhlLimit = mIni.ReadD(sSc, "Wheel Limit");
                        CData.Whls[0].dDrsLimit = a_tWhl[0].dDrsLimit = mIni.ReadD(sSc, "Dresser Limit");
                        //
                    }
                }

                // 2021.10.22 SungTae Start : [수정] (Skyworks VOC) Wheel Name은 기록하지 말라는 고객사 요청으로 수정
                if (CData.CurCompany != ECompany.SkyWorks)
                {
                    CData.Dev.aData[(int)EWay.R].sSelWhl = mIni.Read(sSc, "Right");
                }
                else
                {//211118 pjh : 추가
                    CData.Dev.aData[(int)EWay.R].sSelWhl = CData.Whls[(int)EWay.R].sWhlName;
                }
                // 2021.10.22 SungTae End

                if (a_tWhl[1].aUsedP != null)
                {
                    if (mIni.ReadD("R - Used Parameter 01", "Total Depth").ToString() != "0")
                    {
                        sSc = "R - Dressing Air Cut";
                        a_tWhl[1].dDair = mIni.ReadD(sSc, "Air Cut");
                        if (a_tWhl[1].dDair == 0) { a_tWhl[1].dDair = CData.Whls[1].dDair; }

                        //211013 pjh : Replace Dressing Parameter
                        if (CDataOption.IsDrsAirCutReplace)
                        {
                            sSc = "R - Replace Dressing Air Cut";
                            a_tWhl[1].dDairRep = mIni.ReadD(sSc, "Air Cut");
                            if (a_tWhl[1].dDairRep == 0) { a_tWhl[1].dDairRep = CData.Whls[1].dDairRep; }
                        }

                        sSc = "R - Used Parameter 01";
                        a_tWhl[1].aUsedP[0].eMode = (eStepMode)Enum.Parse(typeof(eStepMode), mIni.Read(sSc, "Mode"));
                        a_tWhl[1].aUsedP[0].dTotalDep = mIni.ReadD(sSc, "Total Depth");
                        a_tWhl[1].aUsedP[0].dCycleDep = mIni.ReadD(sSc, "Cycle Depth");
                        a_tWhl[1].aUsedP[0].dTblSpd = mIni.ReadD(sSc, "Table Speed");
                        a_tWhl[1].aUsedP[0].iSplSpd = mIni.ReadI(sSc, "Spindle Speed");
                        a_tWhl[1].aUsedP[0].eDir = (eStartDir)Enum.Parse(typeof(eStartDir), mIni.Read(sSc, "Direction"));

                        sSc = "R - Used Parameter 02";
                        a_tWhl[1].aUsedP[1].dTotalDep = mIni.ReadD(sSc, "Total Depth");
                        a_tWhl[1].aUsedP[1].eMode = (eStepMode)Enum.Parse(typeof(eStepMode), mIni.Read(sSc, "Mode"));
                        a_tWhl[1].aUsedP[1].dCycleDep = mIni.ReadD(sSc, "Cycle Depth");
                        a_tWhl[1].aUsedP[1].dTblSpd = mIni.ReadD(sSc, "Table Speed");
                        a_tWhl[1].aUsedP[1].iSplSpd = mIni.ReadI(sSc, "Spindle Speed");
                        a_tWhl[1].aUsedP[1].eDir = (eStartDir)Enum.Parse(typeof(eStartDir), mIni.Read(sSc, "Direction"));

                        sSc = "R - New Parameter 01";
                        a_tWhl[1].aNewP[0].eMode = (eStepMode)Enum.Parse(typeof(eStepMode), mIni.Read(sSc, "Mode"));
                        a_tWhl[1].aNewP[0].dTotalDep = mIni.ReadD(sSc, "Total Depth");
                        a_tWhl[1].aNewP[0].dCycleDep = mIni.ReadD(sSc, "Cycle Depth");
                        a_tWhl[1].aNewP[0].dTblSpd = mIni.ReadD(sSc, "Table Speed");
                        a_tWhl[1].aNewP[0].iSplSpd = mIni.ReadI(sSc, "Spindle Speed");
                        a_tWhl[1].aNewP[0].eDir = (eStartDir)Enum.Parse(typeof(eStartDir), mIni.Read(sSc, "Direction"));

                        //211118 pjh : 버그 수정(기존 : a_tWhl[0] > 수정 : a_tWhl[1])
                        sSc = "R - New Parameter 02";
                        a_tWhl[1].aNewP[1].eMode = (eStepMode)Enum.Parse(typeof(eStepMode), mIni.Read(sSc, "Mode"));
                        a_tWhl[1].aNewP[1].dTotalDep = mIni.ReadD(sSc, "Total Depth");
                        a_tWhl[1].aNewP[1].dCycleDep = mIni.ReadD(sSc, "Cycle Depth");
                        a_tWhl[1].aNewP[1].dTblSpd = mIni.ReadD(sSc, "Table Speed");
                        a_tWhl[1].aNewP[1].iSplSpd = mIni.ReadI(sSc, "Spindle Speed");
                        a_tWhl[1].aNewP[1].eDir = (eStartDir)Enum.Parse(typeof(eStartDir), mIni.Read(sSc, "Direction"));
                        //

                        //211210 pjh : Skyworks Wheel&Dresser Limit Device에서 편집
                        sSc = "R - Limit";
                        CData.Whls[1].dWhlLimit = a_tWhl[1].dWhlLimit = mIni.ReadD(sSc, "Wheel Limit");
                        CData.Whls[1].dDrsLimit = a_tWhl[1].dDrsLimit = mIni.ReadD(sSc, "Dresser Limit");
                        //
                    }
                }
                //210831 pjh : 이전 Recipe의 휠과 현재 Recipe의 휠을 비교 후 휠이 다르면 자동 Dressing 진행
                if ((sPreWhlL != CData.Dev.aData[(int)EWay.L].sSelWhl && !CData.DrData[0].bDrs) &&
                    (sPreWhlR != CData.Dev.aData[(int)EWay.R].sSelWhl && !CData.DrData[1].bDrs))
                {
                    CData.DrData[0].bDrs = true;
                    CData.DrData[1].bDrs = true;
                }
                else if(sPreWhlL != CData.Dev.aData[(int)EWay.L].sSelWhl && !CData.DrData[0].bDrs)
                { CData.DrData[0].bDrs = true; }
                else if (sPreWhlR != CData.Dev.aData[(int)EWay.R].sSelWhl && !CData.DrData[1].bDrs)
                { CData.DrData[1].bDrs = true; }
                //
            }

            //201203 jhc : 드레싱 후 N번째 그라인딩 시 휠 소모량 보정 기능
            sSc = "Wheel Loss Correct";
            for (int i = 0; i < 2; i++)
            {
                string sLR = (i==0) ? "Left" : "Right";
                for (int j = 0; j < GV.WHEEL_LOSS_CORRECT_STRIP_MAX; j++)
                {
                    double.TryParse( mIni.Read(sSc, sLR+(j+1)), out CData.Dev.aData[i].dWheelLoss[j] );
                    if (CDataOption.UseDeviceAdvancedOption && CDataOption.UseWheelLossCorrect)
                    {
                        CData.Dev.aData[i].dWheelLoss[j] = GU.Truncate(CData.Dev.aData[i].dWheelLoss[j],4);
                    }
                    else
                    {
                        CData.Dev.aData[i].dWheelLoss[j] = 0.0;
                    }
                }
                double.TryParse( mIni.Read(sSc, sLR+"_Total_Wheel_Loss_Limit"), out CData.Dev.aData[i].dTotalWheelLossLimit );
                if (CDataOption.UseDeviceAdvancedOption && CDataOption.UseWheelLossCorrect)
                {
                    CData.Dev.aData[i].dTotalWheelLossLimit = GU.Truncate(CData.Dev.aData[i].dTotalWheelLossLimit,4);
                }
                else
                {
                    CData.Dev.aData[i].dTotalWheelLossLimit = 0.0;
                }
            }
            //

            sSc = "Common Contact Parameter";
            int.TryParse(   mIni.Read(sSc, "Column Count"),     out CData.Dev.iCol);
            int.TryParse(   mIni.Read(sSc, "Row Count"),        out CData.Dev.iRow);
            double.TryParse(mIni.Read(sSc, "Chip Width"),       out CData.Dev.dChipW);
            double.TryParse(mIni.Read(sSc, "Chip Height"),      out CData.Dev.dChipH);
            double.TryParse(mIni.Read(sSc, "Chip Width Gap"),   out CData.Dev.dChipWGap);
            double.TryParse(mIni.Read(sSc, "Chip Height Gap"),  out CData.Dev.dChipHGap);
            int.TryParse(   mIni.Read(sSc, "Window Count"),     out CData.Dev.iWinCnt);

            // 2021.08.02 SungTae Start : [추가] Measure(Before/After/One-point) 시 측정 위치에 대한 Offset 설정 추가(ASE-KR VOC)
            double.TryParse(mIni.Read(sSc, "Measure Offset X"), out CData.Dev.dMeasOffsetX);
            double.TryParse(mIni.Read(sSc, "Measure Offset Y"), out CData.Dev.dMeasOffsetY);
            // 2021.08.02 SungTae End

            for (int i = 0; i < 5; i++)
            {
                sSc = "Common Contact Window - Start Position " + (i + 1);
                CData.Dev.aWinSt[i].X = (float)mIni.ReadD(sSc, "X");
                CData.Dev.aWinSt[i].Y = (float)mIni.ReadD(sSc, "Y");
            }

            //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
            sSc = "Common Contact Leading-Strip";
            int.TryParse(mIni.Read(sSc, "Leading-Strip Count"), out CData.Dev.i18PStripCount);
            //

            sSc = "Unit Center Distance";
            CData.Dev.aUnitCen = new double[GV.PKG_MAX_UNIT];
            for (int i = 0; i < CData.Dev.iUnitCnt; i++)
            {
                CData.Dev.aUnitCen[i] = mIni.ReadD(sSc, string.Format("Unit{0}", i + 1));
            }

            //200310 ksg : Spindle 부하
            sSc = "Spindle Load Factor";
            double.TryParse(mIni.Read(sSc, "LSpd Auto"),    out CData.Dev.aData[(int)EWay.L].dSpdAuto);
            double.TryParse(mIni.Read(sSc, "LSpd Err"),     out CData.Dev.aData[(int)EWay.L].dSpdError);
            double.TryParse(mIni.Read(sSc, "RSpd Auto"),    out CData.Dev.aData[(int)EWay.R].dSpdAuto);
            double.TryParse(mIni.Read(sSc, "RSpd Err"),     out CData.Dev.aData[(int)EWay.R].dSpdError);

            // 2022.08.30 lhs Start : Spindle Current High/Low Limit 설정 (SCK+ VOC)
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)  // Device는 해당사이트만 저장/로딩 되도록
            {
                //sSc = "Spindle Current Limit";
                //int.TryParse(mIni.Read(sSc, "Left High Limit"),  out CData.Dev.aData[(int)EWay.L].nSpdCurrHL);
                //int.TryParse(mIni.Read(sSc, "Left Low Limit"),   out CData.Dev.aData[(int)EWay.L].nSpdCurrLL);
                //int.TryParse(mIni.Read(sSc, "Right High Limit"), out CData.Dev.aData[(int)EWay.R].nSpdCurrHL);
                //int.TryParse(mIni.Read(sSc, "Right Low Limit"),  out CData.Dev.aData[(int)EWay.R].nSpdCurrLL);
            }
            // 2022.08.30 lhs End

            //201225 jhc : 고급 Grind Condition 설정/체크 기능 (Spindle Current, Table Vacuum Low Limit 설정)
            sSc = "Advanced Grind Condition";
            for (int i = 0; i < 2; i++)
            {
                string sLR = (i==0) ? "Left" : "Right";
                int.TryParse(   mIni.Read(sSc, sLR + " Spindle Current Range Dress Lower"), out CData.Dev.aData[i].nSpindleCurrentDressLow);   //Lower Limit of Spindle Current when Dressing
                int.TryParse(   mIni.Read(sSc, sLR + " Spindle Current Range Dress Upper"), out CData.Dev.aData[i].nSpindleCurrentDressHigh);  //Upper Limit of Spindle Current when Dressing
                int.TryParse(   mIni.Read(sSc, sLR + " Spindle Current Range Grind Lower"), out CData.Dev.aData[i].nSpindleCurrentGrindLow);   //Lower Limit of Spindle Current when Grinding
                int.TryParse(   mIni.Read(sSc, sLR + " Spindle Current Range Grind Upper"), out CData.Dev.aData[i].nSpindleCurrentGrindHigh);  //Upper Limit of Spindle Current when Grinding
                double.TryParse(mIni.Read(sSc, sLR + " Table Vacuum Grind Limit"),          out CData.Dev.aData[i].dTableVacuumGrindLimit);    //Lower Limit of Table Vacuum when Grinding
                double.TryParse(mIni.Read(sSc, sLR + " Table Vacuum Low Hold Time"),        out CData.Dev.aData[i].dTableVacuumLowHoldTime);   //201231 : Table Vacuum Lower Limit Holding Time (이 시간 이상 Lower Limit 유지 시 Alarm)

                if (CDataOption.UseDeviceAdvancedOption && CDataOption.UseAdvancedGrindCondition)
                {
                    CData.Dev.aData[i].dTableVacuumGrindLimit  = GU.Truncate(CData.Dev.aData[i].dTableVacuumGrindLimit,  1);
                    CData.Dev.aData[i].dTableVacuumLowHoldTime = GU.Truncate(CData.Dev.aData[i].dTableVacuumLowHoldTime, 3); //201231
                }
                else
                {
                    CData.Dev.aData[i].nSpindleCurrentDressLow  = 0;
                    CData.Dev.aData[i].nSpindleCurrentDressHigh = 0;
                    CData.Dev.aData[i].nSpindleCurrentGrindLow  = 0;
                    CData.Dev.aData[i].nSpindleCurrentGrindHigh = 0;
                    CData.Dev.aData[i].dTableVacuumGrindLimit   = 0;
                    CData.Dev.aData[i].dTableVacuumLowHoldTime  = 0; //201231
                }
            }
            //..

            sSc = "ETC - Magazine";
            int.TryParse(   mIni.Read(sSc, "Slot Count"),       out CData.Dev.iMgzCnt);
            double.TryParse(mIni.Read(sSc, "Slot Pitch"),       out CData.Dev.dMgzPitch);
            // 2020.10.26 SungTae Start : Add by Qorvo(Strip 배출 관련)
            int.TryParse(   mIni.Read(sSc, "Off Slot Count"),   out CData.Dev.iOffMgzCnt);
            double.TryParse(mIni.Read(sSc, "Off Slot Pitch"),   out CData.Dev.dOffMgzPitch);
            // 2020.10.26 SungTae End

            sSc = "ETC - Dry Part";
            bool.TryParse(  mIni.Read(sSc, "Use Top Blow"),                 out CData.Dev.bDryTop);
            int.TryParse(   mIni.Read(sSc, "Dry Speed"),                    out CData.Dev.iDryRPM);
            Enum.TryParse(  mIni.Read(sSc, "Direction"),                    out CData.Dev.eDryDir);
            int.TryParse(   mIni.Read(sSc, "Dry Count"),                    out CData.Dev.iDryCnt);
            int.TryParse(   mIni.Read(sSc, "Bottom Cleaning Picker Count"), out CData.Dev.iOffpClean);
            int.TryParse(   mIni.Read(sSc, "Bottom Cleaning Strip Count"),  out CData.Dev.iOffpCleanStrip);
            // 2022.03.09 lhs Start : 사용하지 않을 경우는 Device 파일 변화 없도록 옵션 처리
            if (CDataOption.UseSprayBtmCleaner)
            {
                int.TryParse(mIni.Read(sSc, "Bottom Cleaning Picker Count (Air)"), out CData.Dev.iOffpClean_Air);
                int.TryParse(mIni.Read(sSc, "Bottom Cleaning Strip Count (Air)"),  out CData.Dev.iOffpCleanStrip_Air);
            }
            // 2022.03.09 lhs End
            // 2022.07.27 lhs Start : Brush 추가
            if (CDataOption.UseBrushBtmCleaner)
            {
                int.TryParse(mIni.Read(sSc, "Bottom Cleaning Brush Count"),            out CData.Dev.iOffpCleanBrush);         // Brush Clean Count
                int.TryParse(mIni.Read(sSc, "Bottom Cleaning Strip Count N2"),         out CData.Dev.iOffpCleanStrip_N2);      // Strip Clean Count #2
                int.TryParse(mIni.Read(sSc, "Bottom Cleaning Strip Count (Air) N2"),   out CData.Dev.iOffpCleanStrip_Air_N2);  // Strip Clean Count (Air Blow) #2 
            }
            // 2022.07.27 lhs End
            int.TryParse(   mIni.Read(sSc, "Dry Water Nozzle Speed"),       out CData.Dev.iDryWtNozzleRPM);  // 2021.03.30 lhs
            int.TryParse(   mIni.Read(sSc, "Dry Water Nozzle Count"),       out CData.Dev.iDryWtNozzleCnt);  // 2021.03.30 lhs
            sSc = "ETC - Pusher";
            double.TryParse(mIni.Read(sSc, "Fast Speed"),                   out CData.Dev.dPushF);
            double.TryParse(mIni.Read(sSc, "Slow Speed"),                   out CData.Dev.dPushS);
            sSc = "ETC - Picker Vacuum Delay";
            int.TryParse(   mIni.Read(sSc, "On Loader Picker"),             out CData.Dev.iPickDelayOn);
            int.TryParse(   mIni.Read(sSc, "On Loader Picker Place"),       out CData.Dev.iPlaceDelayOn);
            int.TryParse(   mIni.Read(sSc, "Off Loader Picker"),            out CData.Dev.iPickDelayOff);
            int.TryParse(   mIni.Read(sSc, "Off Loader Picker Place"),      out CData.Dev.iPlaceDelayOff); //20200427 jhc : <<--- 200417 jym : Unit에서는 설정된 딜레이 값 사용
            // 2022.05.24 lhs Start
            int.TryParse(   mIni.Read(sSc, "On Loader Picker Eject"),       out CData.Dev.iEjectDelayOnP);   // 2022.05.24 lhs
            int.TryParse(   mIni.Read(sSc, "Off Loader Picker Eject"),      out CData.Dev.iEjectDelayOffP);  // 2022.05.24 lhs
            if (CData.Dev.iEjectDelayOnP  <= 0) CData.Dev.iEjectDelayOnP  = 2000;  // 초기값
            if (CData.Dev.iEjectDelayOffP <= 0) CData.Dev.iEjectDelayOffP = 2000;  // 초기값
            // 2022.05.24 lhs End

         //20190611 ghk_onpclean
         sSc = "ETC - OnLoaderPicker Clean Count";
            int.TryParse(   mIni.Read(sSc, "Clean Count"),          out CData.Dev.iOnpCleanCnt);
            //
            //200408 ksg :
            sSc = "ETC - ReDoMaxCnt";
            int.TryParse(   mIni.Read(sSc, "ReDoNum Max"),          out CData.Dev.iReDoNumMax);
            int.TryParse(   mIni.Read(sSc, "ReDoCnt Max"),          out CData.Dev.iReDoCntMax);
            //200416 pjh : 
            sSc = "ETC - Grinding Strip Start Limit";
            double.TryParse(mIni.Read(sSc, "Strip Start Limit"),    out CData.Dev.dStripStartLimit);
            if (CData.Dev.dStripStartLimit < 0.3) CData.Dev.dStripStartLimit = 0.3;
            if (CData.CurCompany != ECompany.SCK && CData.CurCompany != ECompany.JSCK && CData.CurCompany != ECompany.Suhwoo) CData.Dev.bStripStartLimit = false;
            // 
            //190211 ksg :
            sSc = "ETC - BCR Part";
            bool.TryParse(  mIni.Read(sSc, "BCR Skip"),             out CData.Dev.bBcrSkip);
            bool.TryParse(  mIni.Read(sSc, "Ori Skip"),             out CData.Dev.bOriSkip);
            //201103 jhc : Orientation Check 1회 Skip 기능 추가 (ASE-Kr VOC)
            bool.TryParse(  mIni.Read(sSc, "Ori One Time Skip Use"), out CData.Dev.bOriOneTimeSkipUse);
            if (CData.Dev.bOriSkip)
            {
                CData.Dev.bOriOneTimeSkipUse = false;   //Orientation 검사 Skip인 경우 Orientation One Time Skip 설정 무의미 (Device 옵션)
            }
            if (!CData.Dev.bOriOneTimeSkipUse)
            {
                CData.bOriOneTimeSkipBtnView = false; //201117 jhc : Orientation Error발생 시에만 버튼 표시, 검사 완료 후 다시 감춤
                CData.bOriOneTimeSkip = false;  //Orientation One Time Skip 설정 초기화 (현재 설정 상태)
            }
            //
            bool.TryParse(mIni.Read(sSc, "Ocr Skip"),       out CData.Dev.bOcrSkip);    //190309 ksg :
            if (CData.CurCompany != ECompany.SkyWorks) CData.Dev.bOcrSkip = true;    //190309 ksg :
            
            bool.TryParse(mIni.Read(sSc, "Data Shift"),     out CData.Dev.bDataShift);    //190610 ksg :
            if (CData.CurCompany != ECompany.SCK && CData.CurCompany != ECompany.JSCK) CData.Dev.bDataShift = false; //200121 ksg :
            
            bool.TryParse(mIni.Read(sSc, "DShiftPSkip"),    out CData.Dev.bDShiftPSkip);    //200325 ksg : Data Shift Probe Skip
            if (CData.CurCompany != ECompany.ASE_KR) CData.Dev.bDShiftPSkip = false; //200325 ksg : Data Shift Probe Skip
            //20190625 ghk_dfserver
            
            bool.TryParse(mIni.Read(sSc, "BCR Key In Skip"), out CData.Dev.bBcrKeyInSkip);
            //
            bool.TryParse(mIni.Read(sSc, "BCR Second"),     out CData.Dev.bBcrSecondSkip); //190817 ksg :

            bool.TryParse(mIni.Read(sSc, "Marked Skip"),    out CData.Dev.bOriMarkedSkip); //190817 ksg :

            Int32.TryParse(mIni.Read(sSc, "OCR Digit Type"), out CData.Dev.iDigitType); // 2022-05-26, jhLee : OCR 자릿수 0:10자리 (기존),  1:14자리 (신규)

            if (CData.CurCompany != ECompany.Qorvo && CData.CurCompany != ECompany.Qorvo_DZ &&
                CData.CurCompany != ECompany.Qorvo_RT && CData.CurCompany != ECompany.Qorvo_NC &&
                CData.CurCompany != ECompany.SST)
            {
                CData.Dev.bOriMarkedSkip = true;
            }

            sSc = "ETC - After Data Offset";
            CData.Dev.aFake = new double[2];
            double.TryParse(mIni.Read(sSc, "Left Offset"),  out CData.Dev.aFake[0]);
            double.TryParse(mIni.Read(sSc, "Right Offset"), out CData.Dev.aFake[1]);

            // 2021.03.02 SungTae Start : Device 별로 Table Cleaning Velocity 관리할 수 있도록 고객사 요청에 따른 추가(ASE-KR)
            if (CData.CurCompany == ECompany.ASE_KR)
            {
                sSc = "ETC - Table Clean Velocity";
                CData.Dev.aTblCleanVel = new double[2];
                double.TryParse(mIni.Read(sSc, "Left Grind Y Velocity"), out CData.Dev.aTblCleanVel[0]);
                double.TryParse(mIni.Read(sSc, "Right Grind Y Velocity"), out CData.Dev.aTblCleanVel[1]);
            }
            // 2021.03.02 SungTae Start : Device 별로 Table Cleaning Velocity 관리할 수 있도록 고객사 요청에 따른 추가(ASE-KR)

            // 2020.09.08 SungTae : 3 Step 기능 추가
            if (CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)  { iStepMaxCnt = GV.StepMaxCnt;      }
            else                                                    { iStepMaxCnt = GV.StepMaxCnt_3;    }

            for (int i = 0; i < 2; i++)
            {
                string sDir = "Left";
                if (i == 1)
                { sDir = "Right"; }

                sSc = sDir + " Data - Strip";
                double.TryParse(mIni.Read(sSc, "Total Thickness"),  out CData.Dev.aData[i].dTotalTh);
                double.TryParse(mIni.Read(sSc, "PCB Thickness"),    out CData.Dev.aData[i].dPcbTh);
                double.TryParse(mIni.Read(sSc, "Mold Thickness"),   out CData.Dev.aData[i].dMoldTh);
                double.TryParse(mIni.Read(sSc, "Air Cut Depth"),    out CData.Dev.aData[i].dAir);

                sSc = sDir + " Data - Step";
                // 2021.06.30 lhs Start : 로딩시 Grinding Mode 바뀌는 오류 수정 
                //Enum.TryParse(mIni.Read(sSc, "Grinding Mode"), out CData.Dev.aData[i].eGrdMod);
                string sGrdMode = mIni.Read(sSc, "Grinding Mode");
                if      (sGrdMode == "Normal" || sGrdMode == "Target")  {   CData.Dev.aData[i].eGrdMod = eGrdMode.Target;   }
                else if (sGrdMode == "Top"    || sGrdMode == "TopDown") {   CData.Dev.aData[i].eGrdMod = eGrdMode.TopDown;  }
                else                                                    {   CData.Dev.aData[i].eGrdMod = eGrdMode.Target;   }
                // 2021.06.30 lhs End

                //210928 pjh : 디바이스 변경 시 Log 추가
                sLog += sDir.ToString() + " : " + aPreGrd[i] + " -> " + CData.Dev.aData[i].eGrdMod.ToString() + "\t";
                //

                Enum.TryParse(mIni.Read(sSc, "TDStart Mode"), out CData.Dev.aData[i].eStartMode);  //190502 ksg :
                
                Enum.TryParse(mIni.Read(sSc, "BaseOn Thickness"), out CData.Dev.aData[i].eBaseOnThick);  // 2021.07.27 lhs (SCK VOC)

                // 2022.09.26 lhs Start : Rough1, 첫번째 Griding시에 Z축 StartPos에 cycle depth 적용 
                if (CDataOption.UseDevR1Option)
                {
                    bool.TryParse(mIni.Read(sSc, "bAppCylDepOnFirst"), out CData.Dev.aData[i].bAppCylDepOnFirst);
                    bool.TryParse(mIni.Read(sSc, "bAppLargeCylDep"),   out CData.Dev.aData[i].bAppLargeCylDep);
                }
                // 2022.09.26 lhs End


                // 2022.08.10 SungTae Start : [수정] (ASE-KR 개발건) 설정된 Step 수량을 제외한 나머지 Step UI 비활성화
                if (CData.CurCompany == ECompany.ASE_KR)
                {
                    int.TryParse(mIni.Read(sSc, "Fixed Step Cnt"), out CData.Dev.aData[i].nFixStepCnt);
                }
                // 2022.08.10 SungTae End

                for (int j = 0; j < iStepMaxCnt; j++)       // 2020.09.08 SungTae : 3 Step  기능 추가
                {
                    sSc = sDir + " Data - Step " + (j + 1);
                    bool.TryParse(  mIni.Read(sSc, "Use"),                  out CData.Dev.aData[i].aSteps[j].bUse);
                    Enum.TryParse(  mIni.Read(sSc, "Mode"),                 out CData.Dev.aData[i].aSteps[j].eMode);
                    double.TryParse(mIni.Read(sSc, "Total Depth"),          out CData.Dev.aData[i].aSteps[j].dTotalDep);
                    double.TryParse(mIni.Read(sSc, "Cycle Depth"),          out CData.Dev.aData[i].aSteps[j].dCycleDep);
                    double.TryParse(mIni.Read(sSc, "Table Speed"),          out CData.Dev.aData[i].aSteps[j].dTblSpd);
                    int.TryParse(   mIni.Read(sSc, "Spindle Speed"),        out CData.Dev.aData[i].aSteps[j].iSplSpd);
                    Enum.TryParse(  mIni.Read(sSc, "Start Direction"),      out CData.Dev.aData[i].aSteps[j].eDir);
                    bool.TryParse(  mIni.Read(sSc, "ReGrinding Skip"),      out CData.Dev.aData[i].aSteps[j].bReSkip);
                    double.TryParse(mIni.Read(sSc, "ReGrinding Judgement"), out CData.Dev.aData[i].aSteps[j].dReJud);
                    //201023 JSKim : Over Grinding Correction - Grinding Count Correction 기능
                    bool.TryParse(  mIni.Read(sSc, "OverGrinding Correction Use"), out CData.Dev.aData[i].aSteps[j].bOverGrdCorrectionUse);
                    //201025 jhc : Over Grinding Correction - Grinding Count Correction 기능 제공/감춤 용
                    if (!CDataOption.UseGrindingCorrect)
                    { CData.Dev.aData[i].aSteps[j].bOverGrdCorrectionUse = false; }
                    //201125 jhc : Grinding Step별 Correct 기능
                    double.TryParse(mIni.Read(sSc, "Grinding Correct Depth"), out CData.Dev.aData[i].aSteps[j].dCorrectDepth);
                    if (!CDataOption.UseGrindingStepCorrect)
                    { CData.Dev.aData[i].aSteps[j].dCorrectDepth = 0.0; }
                    //
                }

                sSc = sDir + " Data - Step Fine Setting";
                Enum.TryParse(  mIni.Read(sSc, "Fine Start Mode"),      out CData.Dev.aData[i].eFine);
                double.TryParse(mIni.Read(sSc, "Compensation Value"),   out CData.Dev.aData[i].dCpen);

                sSc = sDir + " Data - Measure Limit";
                double.TryParse(mIni.Read(sSc, "Before Limit"),         out CData.Dev.aData[i].dBfLimit);

                // 2021-03-10, jhLee, for Skyworks VOC, Before limit값을 +/- 범위를 달리적용하도록 한다.
                Temp = mIni.Read(sSc, "BeforeLower Limit");
                if (string.IsNullOrEmpty(Temp) == false)                                // 파일에 저장된 값이 존재한다면
                {
                    double.TryParse(Temp, out CData.Dev.aData[i].dBfLimitLower);        // 읽어들인 값을 사용한다.
                    if (CData.Dev.aData[i].dBfLimitLower < 0.0)                         // 음수라면 양수로 바꾸어준다.
                    {
                        CData.Dev.aData[i].dBfLimitLower = Math.Abs(CData.Dev.aData[i].dBfLimitLower);     // 양수로 변경
                    }
                }
                else
                {
                    // 저장되어진 값이 존재하지 않는다면 기존의 + 방향 범위의 Limit 값을 그대로 사용하도록 한다. (이전 버전 호환용)
                    CData.Dev.aData[i].dBfLimitLower = CData.Dev.aData[i].dBfLimit;
                }


                double.TryParse(mIni.Read(sSc, "After Limit"),      out CData.Dev.aData[i].dAfLimit);
                double.TryParse(mIni.Read(sSc, "TTV Limit"),        out CData.Dev.aData[i].dTTV);
                double.TryParse(mIni.Read(sSc, "One Point Limit"),  out CData.Dev.aData[i].dOneLimit);
                double.TryParse(mIni.Read(sSc, "One Point Over"),   out CData.Dev.aData[i].dOneOver); // 2020.08.19 JSKim

                // 2022.08.10 SungTae Start : [추가] (ASE-KR 개조건)
                // 최종 Target 두께 별도 입력하여 Grinding 최종 Target과 일치하지 않을 경우 Alarm 발생 기능 추가 개발 요청건
                if (CData.CurCompany == ECompany.ASE_KR)
                {
                    double.TryParse(mIni.Read(sSc, "Final Target Thickness"), out CData.Dev.aData[i].dFinalTarget);
                }
                // 2022.08.10 SungTae End

                sSc = sDir + " Data - Top Cleaner";
                //syc : new cleaner val
                double.TryParse(mIni.Read(sSc, "Bubble Speed"), out CData.Dev.aData[i].dTpBubSpd);
                if      (CData.Dev.aData[i].dTpBubSpd <  10) { CData.Dev.aData[i].dTpBubSpd =  10; }
                else if (CData.Dev.aData[i].dTpBubSpd > 200) { CData.Dev.aData[i].dTpBubSpd = 200; }
                //
                int.TryParse(   mIni.Read(sSc, "Bubble Count"), out CData.Dev.aData[i].iTpBubCnt);
                //syc : new cleaner val
                double.TryParse(mIni.Read(sSc, "Air Speed"),    out CData.Dev.aData[i].dTpAirSpd);
                if      (CData.Dev.aData[i].dTpAirSpd <  10) { CData.Dev.aData[i].dTpAirSpd =  10; }
                else if (CData.Dev.aData[i].dTpAirSpd > 200) { CData.Dev.aData[i].dTpAirSpd = 200; }
                //
                int.TryParse(   mIni.Read(sSc, "Air Count"),    out CData.Dev.aData[i].iTpAirCnt);
                //syc : new cleaner val
                double.TryParse(mIni.Read(sSc, "Sponge Speed"), out CData.Dev.aData[i].dTpSpnSpd);
                if      (CData.Dev.aData[i].dTpSpnSpd <  10) { CData.Dev.aData[i].dTpSpnSpd =  10; }
                else if (CData.Dev.aData[i].dTpSpnSpd > 200) { CData.Dev.aData[i].dTpSpnSpd = 200; }
                //
                int.TryParse(   mIni.Read(sSc, "Sponge Count"), out CData.Dev.aData[i].iTpSpnCnt);
                double.TryParse(mIni.Read(sSc, "Probe Offset"), out CData.Dev.aData[i].dPrbOffset); //190502 ksg :

                // 2020.08.31 JSKim St
                sSc = sDir + " Data - Measure Strip One Point";
                bool.TryParse(  mIni.Read(sSc, "X Position Fix"), out CData.Dev.aData[i].bOnePointXPosFix);
                int.TryParse(   mIni.Read(sSc, "Chip Point Win"), out CData.Dev.aData[i].iOnePointWin);
                int.TryParse(   mIni.Read(sSc, "Chip Point Col"), out CData.Dev.aData[i].iOnePointCol);
                int.TryParse(   mIni.Read(sSc, "Chip Point Row"), out CData.Dev.aData[i].iOnePointRow);

                if (CDataOption.Is1Point == false)
                {
                    CData.Dev.aData[i].bOnePointXPosFix = false;
                    CData.Dev.aData[i].iOnePointWin = 0;
                    CData.Dev.aData[i].iOnePointCol = 0;
                    CData.Dev.aData[i].iOnePointRow = 0;
                }
                // 2020.08.31 JSKim Ed

                int iC   = CData.Dev.iCol   ;
                int iR   = CData.Dev.iRow   ;
                int iCnt = iR * CData.Dev.iWinCnt;

                CData.Dev.aData[i].aPosBf = new tCont[iCnt, iC];
                CData.Dev.aData[i].aPosAf = new tCont[iCnt, iC];

                // 2022.01.24 lhs Start : 2004U, Carrier내 Dummy 설정
                if (CDataOption.Use2004U)
				{
                    CData.Parts[(int)EPart.GRDL].bCarrierWithDummy = false; // 초기화
                    CData.Parts[(int)EPart.GRDR].bCarrierWithDummy = false; // 초기화
                    CData.Dev.aData[i].bDummy = new bool[iCnt, iC];         // 생성
					if (i == 0) // 한번만
					{
						CData.Dev.bCopyDummy = new bool[iCnt, iC];
					}
				}
				// 2022.01.24 lhs End 

				if (Init)
                {
                    for (int iSq = 0; iSq < 10; iSq++)
                    {
                        CData.Parts[iSq].iSlot_info = new int[CData.Dev.iMgzCnt];
                        //20200328 jhc : 다이나믹 펑션 측정 포인트 최대 5개로 확대
                        //CData.Parts[iSq].dPcb = new double[3];
                        CData.Parts[iSq].dPcb = new double[GV.DFPOS_MAX];
                        //

                        // 200316 mjy : Unit일때 초기화
                        if (CDataOption.Package == ePkg.Unit)
                        {
                            CData.Parts[iSq].aUnitEx = new bool[GV.PKG_MAX_UNIT];
                        }
                    }
                }

                for (int c = 0; c < iC; c++)
                {
                    for (int r = 0; r < iCnt; r++)
                    {
                        sSc = sDir + " Data - Before Contact Position List";
                        bool.TryParse(  mIni.Read(sSc, r + "," + c + " Use"),       out CData.Dev.aData[i].aPosBf[r, c].bUse);
                        bool.TryParse(  mIni.Read(sSc, r + "," + c + " Use18P"),    out CData.Dev.aData[i].aPosBf[r, c].bUse18P); //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                        double.TryParse(mIni.Read(sSc, r + "," + c + " X"),         out CData.Dev.aData[i].aPosBf[r, c].dX);
                        double.TryParse(mIni.Read(sSc, r + "," + c + " Y"),         out CData.Dev.aData[i].aPosBf[r, c].dY);

                        sSc = sDir + " Data - After Contact Position List";
                        bool.TryParse(  mIni.Read(sSc, r + "," + c + " Use"),       out CData.Dev.aData[i].aPosAf[r, c].bUse);
                        bool.TryParse(  mIni.Read(sSc, r + "," + c + " Use18P"),    out CData.Dev.aData[i].aPosAf[r, c].bUse18P); //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                        double.TryParse(mIni.Read(sSc, r + "," + c + " X"),         out CData.Dev.aData[i].aPosAf[r, c].dX);
                        double.TryParse(mIni.Read(sSc, r + "," + c + " Y"),         out CData.Dev.aData[i].aPosAf[r, c].dY);
                    }
                }

                if (CDataOption.Package == ePkg.Unit)
                {
                    for (int c = 0; c < iC; c++)
                    {
                        for (int r = 0; r < iR; r++)
                        {
                            sSc = sDir + " Data - Before Contact Unit Position List";
                            bool.TryParse(mIni.Read(sSc, r + "," + c + " Use"), out CData.Dev.aData[i].aPosBf[r, c].bUse);
                            double.TryParse(mIni.Read(sSc, r + "," + c + " X"), out CData.Dev.aData[i].aPosBf[r, c].dX);
                            double.TryParse(mIni.Read(sSc, r + "," + c + " Y"), out CData.Dev.aData[i].aPosBf[r, c].dY);
                            CData.Dev.aData[i].aPosBf[r, c].aY = new double[GV.PKG_MAX_UNIT];
                            double.TryParse(mIni.Read(sSc, r + "," + c + " Y1"), out CData.Dev.aData[i].aPosBf[r, c].aY[0]);
                            double.TryParse(mIni.Read(sSc, r + "," + c + " Y2"), out CData.Dev.aData[i].aPosBf[r, c].aY[1]);
                            double.TryParse(mIni.Read(sSc, r + "," + c + " Y3"), out CData.Dev.aData[i].aPosBf[r, c].aY[2]);
                            double.TryParse(mIni.Read(sSc, r + "," + c + " Y4"), out CData.Dev.aData[i].aPosBf[r, c].aY[3]);

                            sSc = sDir + " Data - After Contact Unit Position List";
                            bool.TryParse(mIni.Read(sSc, r + "," + c + " Use"), out CData.Dev.aData[i].aPosAf[r, c].bUse);
                            double.TryParse(mIni.Read(sSc, r + "," + c + " X"), out CData.Dev.aData[i].aPosAf[r, c].dX);
                            double.TryParse(mIni.Read(sSc, r + "," + c + " Y"), out CData.Dev.aData[i].aPosAf[r, c].dY);
                            CData.Dev.aData[i].aPosAf[r, c].aY = new double[GV.PKG_MAX_UNIT];
                            double.TryParse(mIni.Read(sSc, r + "," + c + " Y1"), out CData.Dev.aData[i].aPosAf[r, c].aY[0]);
                            double.TryParse(mIni.Read(sSc, r + "," + c + " Y2"), out CData.Dev.aData[i].aPosAf[r, c].aY[1]);
                            double.TryParse(mIni.Read(sSc, r + "," + c + " Y3"), out CData.Dev.aData[i].aPosAf[r, c].aY[2]);
                            double.TryParse(mIni.Read(sSc, r + "," + c + " Y4"), out CData.Dev.aData[i].aPosAf[r, c].aY[3]);
                        }
                    }
                }
            }


            sSc = "Set Position - On Loader";
            double.TryParse(mIni.Read(sSc, "X - Align Position"), out CData.Dev.dOnL_X_Algn);
            double.TryParse(mIni.Read(sSc, "Y - Wait Position"), out CData.Dev.dOnL_Y_Wait);
            double.TryParse(mIni.Read(sSc, "Y - MGZ Barcode Position"), out CData.Dev.dOnL_Y_MgzBcr); //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)
            double.TryParse(mIni.Read(sSc, "Z - Strip Entry Down Position"), out CData.Dev.dOnL_Z_Entry_Dn);
            double.TryParse(mIni.Read(sSc, "Z - Strip Entry Up Position"), out CData.Dev.dOnL_Z_Entry_Up);
            double.TryParse(mIni.Read(sSc, "Z - MGZ Barcode Position"), out CData.Dev.dOnL_Z_MgzBcr); //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)

            sSc = "Set Position - In Rail";
            double.TryParse(mIni.Read(sSc, "X - Pick Position"), out CData.Dev.dInr_X_Pick);
            double.TryParse(mIni.Read(sSc, "X - Barcode Position"), out CData.Dev.dInr_X_Bcr);
            double.TryParse(mIni.Read(sSc, "X - Orientation Position"), out CData.Dev.dInr_X_Ori);
            double.TryParse(mIni.Read(sSc, "X - Vision Position"), out CData.Dev.dInr_X_Vision);
            double.TryParse(mIni.Read(sSc, "X - Align Position"), out CData.Dev.dInr_X_Align);
            double.TryParse(mIni.Read(sSc, "Y - Align Position"), out CData.Dev.dInr_Y_Align);
            //20190220 ghk_dynamicfunction
            double.TryParse(mIni.Read(sSc, "X - Dynamic Pos1"), out CData.Dev.dInr_X_DynamicPos1);
            double.TryParse(mIni.Read(sSc, "X - Dynamic Pos2"), out CData.Dev.dInr_X_DynamicPos2);
            double.TryParse(mIni.Read(sSc, "X - Dynamic Pos3"), out CData.Dev.dInr_X_DynamicPos3);
            //20200328 jhc : 다이나믹 펑션 측정 포인트 최대 5개로 확대
            double.TryParse(mIni.Read(sSc, "X - Dynamic Pos4"), out CData.Dev.dInr_X_DynamicPos4);
            double.TryParse(mIni.Read(sSc, "X - Dynamic Pos5"), out CData.Dev.dInr_X_DynamicPos5);
            //int   .TryParse(mIni.Read(sSc, "X - Dynamic Position Number"), out CData.Dev.iDynamicPosNum); //DF 측정 포인트 수는 DF Position X 설정 값으로 판단함
            CheckDfInrXPos(); //다이나믹 펑션 사용하는 조건일 경우 InRail X축 DF Position 설정 상태 체크
            //

            sSc = "Set Position - On Loader Picker";
            double.TryParse(mIni.Read(sSc, "X - Wait Position"), out CData.Dev.dOnP_X_Wait);
            double.TryParse(mIni.Read(sSc, "Z - Pick Position"), out CData.Dev.dOnP_Z_Pick);
            double.TryParse(mIni.Read(sSc, "Z - Place Position"), out CData.Dev.dOnP_Z_Place);
            double.TryParse(mIni.Read(sSc, "Z - Slow Position"), out CData.Dev.dOnP_Z_Slow);
            //20190611 ghk_onpclean
            double.TryParse(mIni.Read(sSc, "X - Clean Position"), out CData.Dev.dOnp_X_Clean);
            double.TryParse(mIni.Read(sSc, "Z - Clean Position"), out CData.Dev.dOnp_Z_Clean);
            //
            //20190604 ghk_onpbcr
            double.TryParse(mIni.Read(sSc, "X - Barcode Position"),         out CData.Dev.dOnp_X_Bcr);
            double.TryParse(mIni.Read(sSc, "X - Orientation Position"),     out CData.Dev.dOnp_X_Ori);
            double.TryParse(mIni.Read(sSc, "Z - Barcode Position"),         out CData.Dev.dOnp_Z_Bcr);
            double.TryParse(mIni.Read(sSc, "Z - Orientation Position"),     out CData.Dev.dOnp_Z_Ori);
            double.TryParse(mIni.Read(sSc, "Y - Barcode Position"),         out CData.Dev.dOnp_Y_Bcr);
            double.TryParse(mIni.Read(sSc, "Y - Orientation Position"),     out CData.Dev.dOnp_Y_Ori);
            double.TryParse(mIni.Read(sSc, "X - Barcode Second Position"),  out CData.Dev.dOnp_X_BcrSecon);
            double.TryParse(mIni.Read(sSc, "Y - Barcode Second Position"),  out CData.Dev.dOnp_Y_BcrSecon);
            //201230 jhc : 2D 코드 판독 실패 Alarm 발생 전 제3의 위치(OCR 위치)로 카메라 이동 기능
            double.TryParse(mIni.Read(sSc, "X - Barcode Error Position"),   out CData.Dev.dOnp_X_BcrErr);
            double.TryParse(mIni.Read(sSc, "Y - Barcode Error Position"),   out CData.Dev.dOnp_Y_BcrErr);
            double.TryParse(mIni.Read(sSc, "Z - Barcode Error Position"),   out CData.Dev.dOnp_Z_BcrErr);
            //
            //20191010 ghk_manual_bcr
            double.TryParse(mIni.Read(sSc, "X - Left Table Barcode Position"),      out CData.Dev.dOnp_X_Bcr_TbL);
            double.TryParse(mIni.Read(sSc, "X - Left Table Orientation Position"),  out CData.Dev.dOnp_X_Ori_TbL);
            double.TryParse(mIni.Read(sSc, "X - Right Table Barcode Position"),     out CData.Dev.dOnp_X_Bcr_TbR);
            double.TryParse(mIni.Read(sSc, "X - Right Table Orientation Position"), out CData.Dev.dOnp_X_Ori_TbR);

            double.TryParse(mIni.Read(sSc, "Z - Left Table Barcode Position"),      out CData.Dev.dOnp_Z_Bcr_TbL);
            double.TryParse(mIni.Read(sSc, "Z - Left Table Orientation Position"),  out CData.Dev.dOnp_Z_Ori_TbL);
            double.TryParse(mIni.Read(sSc, "Z - Right Table Barcode Position"),     out CData.Dev.dOnp_Z_Bcr_TbR);
            double.TryParse(mIni.Read(sSc, "Z - Right Table Orientation Position"), out CData.Dev.dOnp_Z_Ori_TbR);

            double.TryParse(mIni.Read(sSc, "Y - Left Table Barcode Position"),      out CData.Dev.dOnp_Y_Bcr_TbL);
            double.TryParse(mIni.Read(sSc, "Y - Left Table Orientation Position"),  out CData.Dev.dOnp_Y_Ori_TbL);
            double.TryParse(mIni.Read(sSc, "Y - Right Table Barcode Position"),     out CData.Dev.dOnp_Y_Bcr_TbR);
            double.TryParse(mIni.Read(sSc, "Y - Right Table Orientation Position"), out CData.Dev.dOnp_Y_Ori_TbR);
            //211022 syc : Onp Pick Up Offset
            double.TryParse(mIni.Read(sSc, "Z - Pick Up Offset"), out CData.Dev.dOnp_Z_PickOffset);
            //
            sSc = "Set Position - Grind Left";
            double.TryParse(mIni.Read(sSc, "Y - Grind Start Position"), out CData.Dev.aGrd_Y_Start[0]);
            double.TryParse(mIni.Read(sSc, "Y - Grind End Position"),   out CData.Dev.aGrd_Y_End[0]);
            //20191011 ghk_manual_bcr
            double.TryParse(mIni.Read(sSc, "Y - Grind Ori Position"),   out CData.Dev.aGrd_Y_Ori[0]);

            sSc = "Set Position - Grind Right";
            double.TryParse(mIni.Read(sSc, "Y - Grind Start Position"), out CData.Dev.aGrd_Y_Start[1]);
            double.TryParse(mIni.Read(sSc, "Y - Grind End Position"),   out CData.Dev.aGrd_Y_End[1]);
            //20191011 ghk_manual_bcr
            double.TryParse(mIni.Read(sSc, "Y - Grind Ori Position"),   out CData.Dev.aGrd_Y_Ori[1]);

            sSc = "Set Position - Off Loader Picker";
            double.TryParse(mIni.Read(sSc, "X - Clean Start Position"), out CData.Dev.dOffP_X_ClnStart);
            double.TryParse(mIni.Read(sSc, "X - Clean End Position"),   out CData.Dev.dOffP_X_ClnEnd);

            // 2021.12.13 SungTae Start : [추가] (ASE-KR VOC)
            // 접수일 : 2021.12.12 (ASE-KR Maint팀 문현수 대리)
            // Device Change 시 Center Position이 "0" 표기되어 Auto Run 중 Strip Cleaning을 제대로 못하는 현상 발생
            double.TryParse(mIni.Read(sSc, "X - Clean End Position"), out CData.Dev.dOffP_X_ClnCenter);

            if (CData.Dev.dOffP_X_ClnCenter == 0)
            {
                CData.Dev.dOffP_X_ClnCenter = CData.Dev.dOffP_X_ClnStart - (CData.Dev.dOffP_X_ClnStart - CData.Dev.dOffP_X_ClnEnd) / 2;
            }
            // 2021.12.13 SungTae End

            // 2022.07.28 lhs Start : Brush X Pos
            if (CDataOption.UseBrushBtmCleaner)
            {
                double.TryParse(mIni.Read(sSc, "X - Brush Clean Start Position"),  out CData.Dev.dOffP_X_ClnStart_Brush);
                double.TryParse(mIni.Read(sSc, "X - Brush Clean End Position"),    out CData.Dev.dOffP_X_ClnEnd_Brush);
            }
            // 2022.07.28 lhs End

            double.TryParse(mIni.Read(sSc, "Z - Pick Position"),        out CData.Dev.dOffP_Z_Pick);
            double.TryParse(mIni.Read(sSc, "Z - Place Position"),       out CData.Dev.dOffP_Z_Place);
            double.TryParse(mIni.Read(sSc, "Z - Slow Position"),        out CData.Dev.dOffP_Z_Slow);
            double.TryParse(mIni.Read(sSc, "Z - Place Down Position"),  out CData.Dev.dOffP_Z_PlaceDn);
            // 2021.02.27 SungTae Start : 고객사(ASE-KR) 요청으로 Device에서 관리 위해 추가
            if (CData.CurCompany == ECompany.ASE_KR     ||
                CData.CurCompany == ECompany.SCK        ||  // 2021.07.14 lhs  SCK, JSCK 추가
                CData.CurCompany == ECompany.JSCK       ||
                CData.CurCompany == ECompany.SkyWorks   ||  // 220216 pjh : Skyworks 추가
                CDataOption.UseSprayBtmCleaner          )   // 2022.03.08 lhs Spray Nozzle Bottom Cleaner 추가
            {
                double.TryParse(mIni.Read(sSc, "Z - Bottom Clean Start Position"),  out CData.Dev.dOffP_Z_ClnStart);
                double.TryParse(mIni.Read(sSc, "Z - Strip Clean Start Position"),   out CData.Dev.dOffP_Z_StripClnStart);

                // 2021.07.14 lhs Start : Device의 Z값이 0이면 이전의 Option의 값 대입. (초기)
                if (CData.Dev.dOffP_Z_ClnStart      <= 0)   {   CData.Dev.dOffP_Z_ClnStart      = CData.SPos.dOFP_Z_ClnStart;       }   //Option보다 Dev가 먼저 로딩되어 Option이 처음에는 0 일 수 있음.
                if (CData.Dev.dOffP_Z_StripClnStart <= 0)   {   CData.Dev.dOffP_Z_StripClnStart = CData.Dev.dOffP_Z_ClnStart - 1;   }
                // 2021.07.14 lhs End

                // 2022.02.22 SungTae Start : [추가] (ASE-KR VOC) Device Change 시 Strip Cleanning Issue 관련 Off-picker Z축 Position 확인 위해 Log 추가
                if (CData.CurCompany == ECompany.ASE_KR)
                {
                    string sState = (CData.Opt.bSecsUse == true) ? "Using" : "Not Use";
                
                    _SetLog($"============[Changed Device Information]===========");
                    _SetLog($"Device Name           : {CData.Dev.sName}");
                    _SetLog($"SECS/GEM Mode         : {sState}");
                    _SetLog($"Bottom Clean Position : {CData.Dev.dOffP_Z_ClnStart} mm");
                    _SetLog($"Strip Clean Position  : {CData.Dev.dOffP_Z_StripClnStart} mm");
                    _SetLog($"===================================================");
                }
                // 2022.02.22 SungTae End
            }
            // 2021.02.27 SungTae End

            // 2022.07.28 lhs Start : Brush Z Pos
            if (CDataOption.UseBrushBtmCleaner)
            {
                double.TryParse(mIni.Read(sSc, "Z - Strip Brush Clean Start Position"), out CData.Dev.dOffP_Z_StripClnStart_Brush);
            }
            // 2022.07.28 lhs End

            sSc = "Set Position - Dry";
            double.TryParse(mIni.Read(sSc, "R - Check 1 Position"),         out CData.Dev.dDry_R_Check1);
            double.TryParse(mIni.Read(sSc, "R - Check 2 Position"),         out CData.Dev.dDry_R_Check2);
            // 200312-jym : Unit 방식 포지션 추가
            double.TryParse(mIni.Read(sSc, "Y - Check Carrier Position"),   out CData.Dev.dDry_Car_Check);
            double.TryParse(mIni.Read(sSc, "Y - Check Unit Position"),      out CData.Dev.dDry_Unit_Check);
            double.TryParse(mIni.Read(sSc, "Y - Start Position"),           out CData.Dev.dDry_Start);
            double.TryParse(mIni.Read(sSc, "Y - End Position"),             out CData.Dev.dDry_End);

            sSc = "Set Position - Off Loader";
            double.TryParse(mIni.Read(sSc, "X - Align Position"),                       out CData.Dev.dOffL_X_Algn);
            double.TryParse(mIni.Read(sSc, "Y - Wait Position"),                        out CData.Dev.dOffL_Y_Wait);
            double.TryParse(mIni.Read(sSc, "Y - MGZ Barcode Position"),                 out CData.Dev.dOffL_Y_MgzBcr); //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)
            double.TryParse(mIni.Read(sSc, "Z - Strip Top Receive Down Position"),      out CData.Dev.dOffL_Z_TRcv_Dn);
            double.TryParse(mIni.Read(sSc, "Z - Strip Top Receive Up Position"),        out CData.Dev.dOffL_Z_TRcv_Up);
            double.TryParse(mIni.Read(sSc, "Z - Strip Bottom Receive Down Position"),   out CData.Dev.dOffL_Z_BRcv_Dn);
            double.TryParse(mIni.Read(sSc, "Z - Strip Bottom Receive Up Position"),     out CData.Dev.dOffL_Z_BRcv_Up);
            double.TryParse(mIni.Read(sSc, "Z - MGZ Barcode Position"),                 out CData.Dev.dOffL_Z_MgzBcr); //20200608 jhc : KEYENCE BCR (On/Off-로더 매거진 바코드)

            //210928 pjh : 디바이스 변경 시 Log 추가
            CLog.Save_Log(eLog.None, eLog.DSL, "[Device] " + sPreDev + "->" + CData.Dev.sName);
            CLog.Save_Log(eLog.None, eLog.DSL, "[Grinding Mode] " + sLog + "\n");
            //210824 syc : 2004U
            sSc = "IV2 - Parameter";
            CData.Dev.sIV2_ONP1_Para     = mIni.Read(sSc, "ONP1 Parameter");
            CData.Dev.sIV2_ONP2_Para     = mIni.Read(sSc, "ONP2 Parameter");
            CData.Dev.sIV2_OFP1_Para     = mIni.Read(sSc, "OFP1 Parameter");
            CData.Dev.sIV2_OFP2_Para     = mIni.Read(sSc, "OFP2 Parameter");
            CData.Dev.sIV2_OFPCover_Para = mIni.Read(sSc, "OFPCover Parameter");

            double.TryParse(mIni.Read(sSc, "X ONP1"), out CData.Dev.dIV2_ONP1_X);
            double.TryParse(mIni.Read(sSc, "Y ONP1"), out CData.Dev.dIV2_ONP1_Y);
            double.TryParse(mIni.Read(sSc, "Z ONP1"), out CData.Dev.dIV2_ONP1_Z);

            double.TryParse(mIni.Read(sSc, "X ONP2"), out CData.Dev.dIV2_ONP2_X);
            double.TryParse(mIni.Read(sSc, "Y ONP2"), out CData.Dev.dIV2_ONP2_Y);
            double.TryParse(mIni.Read(sSc, "Z ONP2"), out CData.Dev.dIV2_ONP2_Z);

            double.TryParse(mIni.Read(sSc, "X OFP1"), out CData.Dev.dIV2_OFP1_X);
            double.TryParse(mIni.Read(sSc, "Z OFP1"), out CData.Dev.dIV2_OFP1_Z);

            double.TryParse(mIni.Read(sSc, "X OFP2"), out CData.Dev.dIV2_OFP2_X);
            double.TryParse(mIni.Read(sSc, "Z OFP2"), out CData.Dev.dIV2_OFP2_Z);

            double.TryParse(mIni.Read(sSc, "X OFP Cover"), out CData.Dev.dIV2_OFPCover_X);
            double.TryParse(mIni.Read(sSc, "Z OFP Cover"), out CData.Dev.dIV2_OFPCover_Z);

            bool.TryParse(mIni.Read(sSc, "ONP2 Use"), out CData.Dev.bIV2_ONP2_Use);
            bool.TryParse(mIni.Read(sSc, "OFP2 Use"), out CData.Dev.bIV2_OFP2_Use);
            //

            return iRet;
        }

        //20200328 jhc : 다이나믹 펑션 측정 포인트 최대 5개로 확대
        /// <summary>
        /// 다이나믹 펑션 사용하는 조건일 경우 InRail X축 DF Position 설정 상태 체크,
        /// 
        /// Device > SET POSITION > InRail > Dynamic Function 1~5 Position에 설정된 값에 따라 실제 DF 포지션 수가 결정됨
        /// DynamicPos1~3 입력 필수 (즉, DF 측정 포지션 수는 최소 3개 이상이어야 함),
        /// DF 사용 활성화 옵션(!CData.Dev.bDynamicSkip && CDataOption.MeasureDf == eDfServerType.MeasureDf)일 때,
        /// (DynamicPos1 == 0) || (DynamicPos2 == 0) || (DynamicPos3 == 0) => Error, DF 포지션 수: 3개로 설정됨,
        /// DynamicPos4 == 0, DynamicPos5 == 0 => DF 포지션 수: 3개로 설정됨,
        /// DynamicPos4 != 0, DynamicPos5 == 0 => DF 포지션 수: 4개로 설정됨,
        /// DynamicPos4 == 0, DynamicPos5 != 0 => DF 포지션 수: 4개로 설정됨, DynamicPos5의 값이 DynamicPos4에 할당됨
        /// DynamicPos4 != 0, DynamicPos5 != 0 => DF 포지션 수: 5개로 설정됨,
        /// Default = 3개
        /// </summary>
        /// <returns>true: 정상, false: 위치 설정값 부족</returns>
        private bool CheckDfInrXPos()
        {
            bool bResult = true;

            if(!CData.Dev.bDynamicSkip && (CDataOption.MeasureDf == eDfServerType.MeasureDf))
            {
                if( CData.Dev.dInr_X_DynamicPos1 == 0 || CData.Dev.dInr_X_DynamicPos2 == 0 || CData.Dev.dInr_X_DynamicPos3 == 0 )
                {
                    CMsg.Show(eMsg.Error, "Notice", "At least three DF measurement positions must be set!!");
                    CData.Dev.iDynamicPosNum = 3; //DF 측정 포인트 수 = 3
                    bResult = false;
                }
                else
                {
                    if( CData.Dev.dInr_X_DynamicPos4 == 0 && CData.Dev.dInr_X_DynamicPos5 == 0 )
                    {
                        CData.Dev.iDynamicPosNum = 3; //DF 측정 포인트 수 = 3
                    }
                    else if( CData.Dev.dInr_X_DynamicPos4 == 0 && CData.Dev.dInr_X_DynamicPos5 != 0 )
                    {
                        //5번째 위치에 입력한 값을 4번째 위치로 이동시킴
                        CData.Dev.dInr_X_DynamicPos4 = CData.Dev.dInr_X_DynamicPos5;
                        CData.Dev.dInr_X_DynamicPos5 = 0;
                        CData.Dev.iDynamicPosNum = 4; //DF 측정 포인트 수 = 4
                    }
                    else if( CData.Dev.dInr_X_DynamicPos4 != 0 && CData.Dev.dInr_X_DynamicPos5 == 0 )
                    {
                        CData.Dev.iDynamicPosNum = 4; //DF 측정 포인트 수 = 4
                    }
                    else
                    {
                        CData.Dev.iDynamicPosNum = 5; //DF 측정 포인트 수 = 5
                    }
                }
            }
            else
            {
                CData.Dev.iDynamicPosNum = 3; //Default DF 측정 포인트 수 = 3
            }

            return bResult;
        }

        /*
        private void CheckChanged(string path, string oldData, string newData)
        {
            try
            {
                Console.WriteLine("==================================================================================");                
                Dictionary<string, Hashtable> dicOld = ConvertDicToStr(oldData);
                Dictionary<string, Hashtable> dicNew = ConvertDicToStr(newData);
                string changeLog = string.Empty;

                foreach (KeyValuePair<string, Hashtable> valDic in dicNew)
                {
                    foreach (DictionaryEntry valHt in valDic.Value)
                    {
                        if (dicOld.ContainsKey(valDic.Key))
                        {
                            if (dicOld[valDic.Key].ContainsKey(valHt.Key))
                            {
                                if (!dicOld[valDic.Key][valHt.Key].Equals(valHt.Value))
                                {
                                    changeLog += valDic.Key + " " + valHt.Key + "=" + dicOld[valDic.Key][valHt.Key].ToString() + " -> " + valHt.Value + "\r\n";

                                }
                            }

                        }
                    }
                }

                if (!string.IsNullOrEmpty(changeLog))
                {
                    Console.WriteLine(changeLog);
                    CLog.Save_Log(eLog.None, eLog.DSL, "DEVICE : PARAMETER Changed log ("+path+")\r\n" + changeLog);
                }
            }
            catch (Exception err)
            {
                CLog.Save_Log(eLog.None, eLog.DSL, "DEVICE : PARAMETER Changed log (" + path + ") ERROR : " + err.Message);
            }
        }

        private Dictionary<string, Hashtable> ConvertDicToStr(string str)
        {
            Dictionary<string, Hashtable> dicRet = new Dictionary<string, Hashtable>();
            Hashtable htTemp = new Hashtable();
            string tempKey = string.Empty;

            string[] newStrLines = str.Split('\n');
            for (int i = 0; i < newStrLines.Length; i++)
            {
                newStrLines[i] = newStrLines[i].Replace("\r", "").Trim();

                if (newStrLines[i].Contains("[") && !tempKey.Equals(newStrLines[i]))
                {
                    tempKey = newStrLines[i];
                    if (htTemp.Count > 0)
                    {
                        dicRet.Add(tempKey, htTemp);
                    }
                    htTemp = new Hashtable();
                }
                else if (!string.IsNullOrEmpty(newStrLines[i]))
                {
                    string[] convString = newStrLines[i].Split('=');
                    htTemp.Add(convString[0], convString[1]);
                }
            }

            return dicRet;
        }

        public string ReadAllSetupPosition(string path)
        {
            string ret = "";
            string sPath = path;

            if (File.Exists(sPath))
            {
                try
                {
                    ret = File.ReadAllText(sPath);
                }
                catch
                {
                    ret = "ERR";
                }
            }
            else
            {
                ret = "EPT";
            }

            return ret;
        }*/

        //201103 jhc : Orientation Check 1회 Skip 기능 추가 (ASE-Kr VOC)
        public void ReLoadEtcBcrOption()
        {
            string sPath = CData.DevCur;
            string sSc = "";

            if (File.Exists(sPath) == false)
            { return; } // Error

            CIni mIni = new CIni(sPath);

            sSc = "ETC - BCR Part";
            bool.TryParse(mIni.Read(sSc, "BCR Skip"), out CData.Dev.bBcrSkip);
            bool.TryParse(mIni.Read(sSc, "Ori Skip"), out CData.Dev.bOriSkip);
            //201103 jhc : Orientation Check 1회 Skip 기능 추가 (ASE-Kr VOC)
            bool.TryParse(mIni.Read(sSc, "Ori One Time Skip Use"), out CData.Dev.bOriOneTimeSkipUse);
            if (CData.Dev.bOriSkip)
            {
                CData.Dev.bOriOneTimeSkipUse = false;    //Orientation 검사 Skip인 경우 Orientation One Time Skip 설정 무의미 (Device 옵션)
            }
            if (!CData.Dev.bOriOneTimeSkipUse)
            {
                CData.bOriOneTimeSkipBtnView = false;    //201117 jhc : Orientation Error발생 시에만 버튼 표시, 검사 완료 후 다시 감춤
                CData.bOriOneTimeSkip        = false;    //Orientation One Time Skip 설정 초기화 (현재 설정 상태)
            }
            //
            bool.TryParse(mIni.Read(sSc, "Ocr Skip"),       out CData.Dev.bOcrSkip);        //190309 ksg :
            bool.TryParse(mIni.Read(sSc, "Data Shift"),     out CData.Dev.bDataShift);      //190610 ksg :
            bool.TryParse(mIni.Read(sSc, "DShiftPSkip"),    out CData.Dev.bDShiftPSkip);    //200325 ksg : Data Shift Probe Skip
            
            if (CData.CurCompany != ECompany.SkyWorks)                                  CData.Dev.bOcrSkip      = true;     //190309 ksg :
            if (CData.CurCompany != ECompany.SCK && CData.CurCompany != ECompany.JSCK)  CData.Dev.bDataShift    = false;    //200121 ksg :
            if (CData.CurCompany != ECompany.ASE_KR)                                    CData.Dev.bDShiftPSkip  = false;    //200325 ksg : Data Shift Probe Skip

            bool.TryParse(mIni.Read(sSc, "BCR Key In Skip"),    out CData.Dev.bBcrKeyInSkip);  
            bool.TryParse(mIni.Read(sSc, "BCR Second"),         out CData.Dev.bBcrSecondSkip);  //190817 ksg :
            bool.TryParse(mIni.Read(sSc, "Marked Skip"),        out CData.Dev.bOriMarkedSkip);  //190817 ksg :

            Int32.TryParse(mIni.Read(sSc, "OCR Digit Type"), out CData.Dev.iDigitType); // 2022-05-26, jhLee : OCR 자릿수 0:10자리 (기존),  1:14자리 (신규)


            // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
            if (CData.CurCompany != ECompany.Qorvo      && CData.CurCompany != ECompany.Qorvo_DZ &&
                CData.CurCompany != ECompany.Qorvo_RT   && CData.CurCompany != ECompany.Qorvo_NC &&
                CData.CurCompany != ECompany.SST)
                CData.Dev.bOriMarkedSkip = true;
        }
        //

        //201203 jhc : 드레싱 후 N번째 그라인딩 시 휠 소모량 보정 기능
        public void ReLoadWheelLossCorrect()
        {
            string sPath = CData.DevCur;
            string sSc = "";

            if (File.Exists(sPath) == false)
            { return; } // Error

            CIni mIni = new CIni(sPath);

            sSc = "Wheel Loss Correct";
            for (int i = 0; i < 2; i++)
            {
                string sLR = (i==0) ? "Left" : "Right";
                for (int j = 0; j < GV.WHEEL_LOSS_CORRECT_STRIP_MAX; j++)
                {
                    double.TryParse( mIni.Read(sSc, sLR+(j+1)), out CData.Dev.aData[i].dWheelLoss[j] );
                    if (CDataOption.UseDeviceAdvancedOption && CDataOption.UseWheelLossCorrect)
                    {
                        CData.Dev.aData[i].dWheelLoss[j] = GU.Truncate(CData.Dev.aData[i].dWheelLoss[j],4);
                    }
                    else
                    {
                        CData.Dev.aData[i].dWheelLoss[j] = 0.0;
                    }
                }
                double.TryParse( mIni.Read(sSc, sLR+"_Total_Wheel_Loss_Limit"), out CData.Dev.aData[i].dTotalWheelLossLimit );
                if (CDataOption.UseDeviceAdvancedOption && CDataOption.UseWheelLossCorrect)
                {
                    CData.Dev.aData[i].dTotalWheelLossLimit = GU.Truncate(CData.Dev.aData[i].dTotalWheelLossLimit,4);
                }
                else
                {
                    CData.Dev.aData[i].dTotalWheelLossLimit = 0.0;
                }
            }
        }
        //

        //210928 pjh : Device 변경 시 Grinding Mode 비교를 위한 함수
        public bool CompareGrdMod(string sPath)
        {
            bool bRet = true;
            bool bL = false;
            bool bR = false;
            sChangedMode = "";
            
            for (int i = 0; i<2; i++)
            {
                aNowGrd[i] = CData.Dev.aData[i].eGrdMod.ToString();

                if(i == 0)
                {
                    if(aNowGrd[0] == aPreGrd[0])
                    { bL = true; }
                    else
                    { sChangedMode += "\n" + "Left : " + aPreGrd[0].ToString() + "->" + aNowGrd[0].ToString(); }
                }
                else
                {
                    if (aNowGrd[1] == aPreGrd[1])
                    { bR = true; }
                    else
                    { sChangedMode += "\n" + "Right : " + aPreGrd[1].ToString() + "->" + aNowGrd[1].ToString(); }
                }
            }
            if(!bL || !bR)
            { bRet = false; }

            return bRet;
        }
        //210928 pjh : 이전 Device의 정보를 저장하기 위한 함수
        public void SavePreData()
        {
            sPreDev = CData.Dev.sName;

            for (int i = 0; i < 2; i++)
            {
                aPreGrd[i] = "";
                aPreGrd[i] = CData.Dev.aData[i].eGrdMod.ToString();
            }
        }
        //

        // 2022.02.22 SungTae Start : [추가] (ASE-KR VOC) Device Change 시 Strip Cleanning Issue 관련 Off-picker Z축 Position 확인 위해 Log 추가
        private void _SetLog(string sMsg)
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            string sMth = sf.GetMethod().Name.PadRight(20);

            CLog.Save_Log(eLog.None, eLog.DSL, string.Format("[{0}]\t{1}", sMth, sMsg));
        }
        // 2022.02.22 SungTae End
    }
}
