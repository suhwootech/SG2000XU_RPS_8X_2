using System;
using System.IO;
using System.Text;

namespace SG2000X
{
    public class COpt : CStn<COpt>
    {
        private COpt()
        {
            _Init();
        }

        private void _Init()
        {
            CData.Opt = new tOpt();
            CData.Opt.aTbGrd = new tStep[2];
            CData.Opt.dTbGrdAir = 0;
            CData.Opt.aTblSkip = new bool[2];
            CData.Opt.aTC_Cnt = new int[2];
            CData.Opt.aOnpRinseTime = new int[2];
            // 201022 jym : 신규추가
            CData.Opt.aAtoLimit = new double[2];
            CData.Opt.aWhlTTV = new double[2];

            // 2020-11-16, jhLee : 신규추가
            CData.Opt.iTC_Cycle = 0;        // Table cleaning 주기 

            // 2021.04.05 SungTae
            CData.Opt.aLTblMeasPos = new double[2];
            CData.Opt.aRTblMeasPos = new double[2];

            CData.Opt.aWheelMax = new double[2];
            CData.Opt.aDresserMax = new double[2];
        }

        public void Save()
        {
            string sPath = GV.PATH_CONFIG + "Option.cfg";
            StringBuilder mSB = new StringBuilder();

            mSB.AppendLine("[Warm Up]");
            mSB.AppendLine("Time=" + CData.Opt.iWmUT);
            mSB.AppendLine("Spindle Speed=" + CData.Opt.iWmUS);
            mSB.AppendLine("Idle Time=" + CData.Opt.iWmUI); //syc : Warm up 유휴 시간
            mSB.AppendLine("WarmUpWithStrip=" + CData.Opt.bWarmUpWithStrip);  // 2022.06.10 lhs : (SCK+)자재가 있어도 워밍업
            mSB.AppendLine("Skip=" + CData.Opt.bWarmUpSkip.ToString());
            // 201023 jym : 추가
            mSB.AppendLine("Auto time=" + CData.Opt.iAWT);
            mSB.AppendLine("Auto period=" + CData.Opt.iAWP);
            // 201029 jym : Add
            mSB.AppendLine("Auto skip=" + CData.Opt.bAwSkip);
            // 201029 jym End
            mSB.AppendLine();
            mSB.AppendLine("[Wheel Measure]");
            mSB.AppendLine("Spindle Speed=" + CData.Opt.iMeaS);
            mSB.AppendLine("Time=" + CData.Opt.iMeaT);
            mSB.AppendLine();
            mSB.AppendLine("[Table Grinding]");
            mSB.AppendLine("Air Cut Depth=" + CData.Opt.dTbGrdAir);
            mSB.AppendLine("Left Last Grinding Date=" + CData.Opt.dtLast_L.ToString());
            mSB.AppendLine("Right Last Grinding Date=" + CData.Opt.dtLast_R.ToString());
            mSB.AppendLine();
            mSB.AppendLine("[Table Grinding - Step 01]");
            mSB.AppendLine("Total Depth=" + CData.Opt.aTbGrd[0].dTotalDep);
            mSB.AppendLine("Cycle Depth=" + CData.Opt.aTbGrd[0].dCycleDep);
            mSB.AppendLine("Table Speed=" + CData.Opt.aTbGrd[0].dTblSpd);
            mSB.AppendLine("Spindle Speed=" + CData.Opt.aTbGrd[0].iSplSpd);
            mSB.AppendLine();
            mSB.AppendLine("[Table Grinding - Step 02]");
            mSB.AppendLine("Total Depth=" + CData.Opt.aTbGrd[1].dTotalDep);
            mSB.AppendLine("Cycle Depth=" + CData.Opt.aTbGrd[1].dCycleDep);
            mSB.AppendLine("Table Speed=" + CData.Opt.aTbGrd[1].dTblSpd);
            mSB.AppendLine("Spindle Speed=" + CData.Opt.aTbGrd[1].iSplSpd);

            //191203 ksg : Table Grd Manual Set Pos
            if (CDataOption.TblSetPos == eTblSetPos.NotUse)
            {
                CData.Opt.LeftTopPos    = 0;
                CData.Opt.LeftBtmPos    = 0;
                CData.Opt.RightTopPos   = 0;
                CData.Opt.RightBtmPos   = 0;
                CData.Opt.LeftXPos      = 0;
                CData.Opt.RightXPos     = 0;

                // 2021.10.08 SungTae Start : [추가] 5um로 Fix 해서 사용하는 것에서 Limit을 설정해서 사용할 수 있도록 변경 요청(ASE-KR VOC)
                CData.Opt.LeftLimitTtoB  = 0;
                CData.Opt.LeftLimitLtoR  = 0;
                CData.Opt.RightLimitTtoB = 0;
                CData.Opt.RightLimitLtoR = 0;
                // 2021.10.08 SungTae End
            }
            // 2021.10.05 SungTae Start : [추가]
            else
            {
                if (CData.Opt.LeftXPos < CData.Axes[(int)EAx.LeftGrindZone_X].dSWMin)
                {
                    CData.Opt.LeftXPos = CData.Axes[(int)EAx.LeftGrindZone_X].dSWMin;
                }

                if (CData.Opt.RightXPos > CData.Axes[(int)EAx.RightGrindZone_X].dSWMax)
                {
                    CData.Opt.RightXPos = CData.Axes[(int)EAx.RightGrindZone_X].dSWMax;
                }
            }
            // 2021.10.05 SungTae End

            mSB.AppendLine("Table Left Top="        + CData.Opt.LeftTopPos);
            mSB.AppendLine("Table Left Btm="        + CData.Opt.LeftBtmPos);
            mSB.AppendLine("Table Right Top="       + CData.Opt.RightTopPos);
            mSB.AppendLine("Table Right Btm="       + CData.Opt.RightBtmPos);
            mSB.AppendLine("Table Left  X Top="     + CData.Opt.LeftXPos);
            mSB.AppendLine("Table Right X Top="     + CData.Opt.RightXPos);

            // 2021.10.08 SungTae Start : [추가] 5um로 Fix 해서 사용하는 것에서 Limit을 설정해서 사용할 수 있도록 변경 요청(ASE-KR VOC)
            mSB.AppendLine("Left  Limit T to B="    + CData.Opt.LeftLimitTtoB);
            mSB.AppendLine("Left  Limit L to R="    + CData.Opt.LeftLimitLtoR);
            mSB.AppendLine("Right Limit T to B="    + CData.Opt.RightLimitTtoB);
            mSB.AppendLine("Right Limit L to R="    + CData.Opt.RightLimitLtoR);
            // 2021.10.08 SungTae End

            // 2021.04.05 SungTae Start : Table Grinding 허용 가능 잔여량 표시 위해 추가(ASE-KR)
            mSB.AppendLine("L-Table Measure Top="   + CData.Opt.aLTblMeasPos[0]);
            mSB.AppendLine("L-Table Measure Btm="   + CData.Opt.aLTblMeasPos[1]);
            mSB.AppendLine("R-Table Measure Top="   + CData.Opt.aRTblMeasPos[0]);
            mSB.AppendLine("R-Table Measure Btm="   + CData.Opt.aRTblMeasPos[1]);
            // 2021.04.05 SungTae End
			
            mSB.AppendLine();
            //20190404 ghk_피커 대기 시간
            mSB.AppendLine("[OnLoaderPicker Option]");
            mSB.AppendLine("Left Rinse Time="       + CData.Opt.aOnpRinseTime[(int)EWay.L]);
            mSB.AppendLine("Right Rinse Time="      + CData.Opt.aOnpRinseTime[(int)EWay.R]);
            mSB.AppendLine();
            //20190926 ghk_ofppadclean
            mSB.AppendLine("[OffLoaderPicker Option]");
            mSB.AppendLine("Pad Clean Time=" + CData.Opt.iOfpPadCleanTime);
            mSB.AppendLine();
            mSB.AppendLine("[LOT]");
            mSB.AppendLine("Max Magazine Count=" + CData.Opt.iLotCnt);
            mSB.AppendLine("Start Table Clean=" + CData.Opt.bLotTblClean);  //200401 jym
            mSB.AppendLine("End Dressing=" + CData.Opt.bLotDrs);  //200401 jym
            mSB.AppendLine();
            //190103 ksg :  연속 빈슬롯 일때 자동 배출
            mSB.AppendLine("[MAGAZIN]");
            mSB.AppendLine("Empty Slot Cnt=" + CData.Opt.iEmptySlotCnt);
            mSB.AppendLine("Emit=" + CData.Opt.eEmitMgz); //20200513 jym : 매거진 배출 위치 변경 추가
            mSB.AppendLine();
            if (CData.Opt.aTblSkip[(int)EWay.L] && CData.Opt.aTblSkip[(int)EWay.R])
            {
                CMsg.Show(eMsg.Warning, "Warning", "Can't Both Select Skip Table");
                return;
            }
            mSB.AppendLine("[Talbe Skip]");
            mSB.AppendLine("LTableSkip=" + CData.Opt.aTblSkip[(int)EWay.L].ToString());
            mSB.AppendLine("RTableSkip=" + CData.Opt.aTblSkip[(int)EWay.R].ToString());
            mSB.AppendLine("LeakSkip=" + CData.Opt.bLeakSkip.ToString()); //190109 ksg :
            mSB.AppendLine();
            mSB.AppendLine("[Door]");
            mSB.AppendLine("Skip=" + CData.Opt.bDoorSkip.ToString());
            mSB.AppendLine("CoverSkip=" + CData.Opt.bCoverSkip.ToString()); //190319 ksg :
            mSB.AppendLine();
            //ksg 추가
            mSB.AppendLine("[Dry]");
            mSB.AppendLine("Skip="                  + CData.Opt.bDryStickSkip.ToString());
            mSB.AppendLine("Delay="                 + CData.Opt.iDryDelay.ToString());          //200715 lks : 중복 값 처리
            mSB.AppendLine("Sponge Water Auto OnOff=" + CData.Opt.bSpgWtAutoOnOff.ToString());    //2022.06.14 lhs : Sponge water auto on/off 사용 여부
            mSB.AppendLine("Sponge Water On Time="  + CData.Opt.nSpgWtOnMin.ToString());        //2022.06.14 lhs : Sponge water auto on time
            mSB.AppendLine("Sponge Water Off Time=" + CData.Opt.nSpgWtOffMin.ToString());       //2022.06.14 lhs : Sponge water auto off time
            mSB.AppendLine("ZUpStableDelay="        + CData.Opt.nDryZUpStableDelay.ToString()); // 2022.01.26 lhs Start : Dryer Z축 Up 후에 안정화 시간 갖기, Pusher 걸리지 않고 동작시키기 위해
            mSB.AppendLine();

            mSB.AppendLine("[DryRun]");
            mSB.AppendLine("Skip=" + CData.Opt.bDryAuto.ToString());
            mSB.AppendLine();
            //20190703 ghk_automeasure
            mSB.AppendLine("[DryRunMeasure]");
            mSB.AppendLine("Skip=" + CData.Opt.bDryAutoMeaStripSkip.ToString());
            mSB.AppendLine();
            //190109 ksg :
            mSB.AppendLine("[OffLoadPicker]");
            mSB.AppendLine("Skip=" + CData.Opt.bOfpBtmCleanSkip.ToString());
            mSB.AppendLine("Use=" + CData.Opt.bOfpUseCenterPos.ToString());                 // 2021.03.18 SungTae Start : 고객사(ASE-KR) 요청으로 Center Position Option 추가
            mSB.AppendLine("CleaningMode=" + CData.Opt.iOfpCleaningMode.ToString());        // 2021.07.02 SungTae : [추가] 고객사(ASE-KR) 요청으로 Strip Cleaning Mode 추가
            mSB.AppendLine("CoverDryerSkip=" + CData.Opt.bOfpCoverDryerSkip.ToString());    // 2021.04.09 lhs
            mSB.AppendLine();

            //190213 ksg :
            mSB.AppendLine("[WheelJig]");
            mSB.AppendLine("Skip=" + CData.Opt.bWhlJigSkip.ToString());
            mSB.AppendLine();
            //190214 ksg :
            mSB.AppendLine("[OfLMgzOption]");
            mSB.AppendLine("On="          + CData.Opt.bOfLMgzMatchingOn.ToString()); 
            mSB.AppendLine("Dir="         + CData.Opt.bFirstTopSlot    .ToString()); //190503 ksg :
            mSB.AppendLine("Dir1="        + CData.Opt.bFirstTopSlotOff .ToString()); //190511 ksg :
            mSB.AppendLine("Unload Type=" + CData.Opt.bOfLMgzUnloadType.ToString());    // 2021.12.08 SungTae : [추가] (Qorvo향 VOC) 
            mSB.AppendLine();
            //190404 ksg : Qc
            mSB.AppendLine("[QcVision]");
            mSB.AppendLine("Use="    + CData.Opt.bQcUse   .ToString());
            mSB.AppendLine("ByPass=" + CData.Opt.bQcByPass.ToString()); //190406 ksg : Qc
            // 201116 jym : QC와 오프로더 도어 센서 연동
            mSB.AppendLine("Door=" + CData.Opt.bQcDoor.ToString());
            mSB.AppendLine("SIMULATION=" + CData.Opt.bQcSimulation.ToString());     // 2021-01-13, jhLee : 시뮬레이션 모드 저장 (유지)

            mSB.AppendLine();
            //MTBA MTBF
            mSB.AppendLine("[MTBA MTBF]");
            if (CData.Opt.dDayWorking == 0)
            { CData.Opt.dDayWorking = 24; }
            mSB.AppendLine("Day Working="        + CData.Opt.dDayWorking.ToString());
            mSB.AppendLine("Calculation Period=" + CData.Opt.iPeriod    .ToString());
            mSB.AppendLine("Time for Renewal="   + CData.Opt.iRenewal   .ToString());
            mSB.AppendLine("MTBA Set Time="      + CData.Opt.iSetTime   .ToString());
            mSB.AppendLine();
            // 201022 jym : TTV검사 좌우 추가 -> 190405 maeng - Wheel TTV 저장 추가
            mSB.AppendLine("[Wheel TTV]");
            mSB.AppendLine("TTV=" + CData.Opt.aWhlTTV[(int)EWay.L]);
            mSB.AppendLine("TTV Right=" + CData.Opt.aWhlTTV[(int)EWay.R]);
            mSB.AppendLine();
            //Auto Level Change Time 저장 추가
            mSB.AppendLine("[Lv Change]");
            mSB.AppendLine("Minute=" + CData.Opt.iChangeLevel); //190509 ksg :
            mSB.AppendLine();
            //Select Language 저장 추가
            mSB.AppendLine("[Language]");
            mSB.AppendLine("Select=" + CData.Opt.iSelLan); //190516 ksg :
            mSB.AppendLine();
            //Probe Measure Type
            mSB.AppendLine("[Probe]");
            mSB.AppendLine("Type=" + CData.Opt.bPbType.ToString()); //190628 ksg :
           // 2020-12-14, jhLee : 기존 MeaStrip_T1, T2를 통합한 함수를 이용할것인지 여부
            mSB.AppendLine("MeasureFunctionType=" + CData.Opt.bMeasureFunctionType.ToString()); //190628 ksg :

            mSB.AppendLine();

            //20191010 ghk_manual_bcr
            mSB.AppendLine("[Manual Bcr]");
            mSB.AppendLine("ManBcr=" + CData.Opt.bManBcrSkip.ToString());
            mSB.AppendLine();
			//20191010 Secs Use
            mSB.AppendLine("[Secs Use]");
            mSB.AppendLine("Use=" + CData.Opt.bSecsUse.ToString());
            // 2020.10.19, jhLee : HOST의 START RCMD 회신을 기다리는 시간 ms, 0이면 기다리지 않는다.
            mSB.AppendLine("Host RCMD Start Timeout=" + CData.Opt.iRCMDStartTimeout.ToString());
            mSB.AppendLine();
			//200203 ksg :
            mSB.AppendLine("[RFID Skip]");
            mSB.AppendLine("OnlRFIDSkip=" + CData.Opt.bOnlRfidSkip.ToString());
            mSB.AppendLine("OflRFIDSkip=" + CData.Opt.bOflRfidSkip.ToString());
            mSB.AppendLine();
			//200318 ksg : Wheel One Point Check
            mSB.AppendLine("[Wheel One Check]");
            mSB.AppendLine("WhlOneCheck=" + CData.Opt.bWhlOneCheck.ToString());
            mSB.AppendLine();
            // 200728 jym : 휠 클린 노즐 추가
            mSB.AppendLine("[Wheel Clean Nozzle]");
            mSB.AppendLine("Skip=" + CData.Opt.bWhlClnSkip);
            mSB.AppendLine();
            // 200820 jym : Network drive 설정 
            mSB.AppendLine("[Network Drive]");
            mSB.AppendLine("Use=" + CData.Opt.bNetUse);
            mSB.AppendLine("Ip=" + CData.Opt.sNetIP);
            mSB.AppendLine("Path=" + CData.Opt.sNetPath);
            mSB.AppendLine("Id=" + CData.Opt.sNetID);
            mSB.AppendLine("Pw=" + CData.Opt.sNetPw);
            //210811 pjh : D/F Server Data Save 기능 Net Drive 추가
            mSB.AppendLine("D/F Use =" + CData.Opt.bDFNetUse);
            //
            mSB.AppendLine();
            // 2020.08.21 JSKim St
            mSB.AppendLine("[Display Probe Value]");
            mSB.AppendLine("ProbeFontSize=" + CData.Opt.iProbeFontSize.ToString());
            mSB.AppendLine("ProbeFloationPoint=" + CData.Opt.iProbeFloationPoint.ToString());
            // 2020.08.21 JSKim Ed
            // 200917 jym
            mSB.AppendLine("[General Skip]");
            mSB.AppendLine("DiChiller=" + CData.Opt.bDiChillerSkip);

            // 201006 jym start
            mSB.AppendLine("[Check DI]");
            mSB.AppendLine("Delay time=" + CData.Opt.iChkDiTime);
            // 201006 jym end

            // 201022 jym : 추가
            mSB.AppendLine("[Auto Offset Limit]");
            mSB.AppendLine("Left limit=" + CData.Opt.aAtoLimit[(int)EWay.L]);
            mSB.AppendLine("Right limit=" + CData.Opt.aAtoLimit[(int)EWay.R]);
            mSB.AppendLine("Max limit=" + CData.Opt.dToolMax);

            //201023 JSKim : Over Grinding Correction - Grinding Count Correction 기능
            mSB.AppendLine("[Over Grinding Correction]");
            mSB.AppendLine("GrindingCountCorrectionUse=" + CData.Opt.bOverGrdCountCorrectionUse.ToString());

            // 2021.02.20 lhs Start
            mSB.AppendLine("[Probe Measure]");
            mSB.AppendLine("WheelStopWaitSkip=" + CData.Opt.bWheelStopWaitSkip.ToString());
            // 2021.02.20 lhs End

            // 210727 pjh : Wheel 및 Dresser 최대 높이 설정
            mSB.AppendLine("[Wheel/Dresser Maximum Thickness Use]");
            mSB.AppendLine("Limit Use=" + CData.Opt.bVarLimitUse);
            mSB.AppendLine();

            mSB.AppendLine("[Wheel/Dresser Maximum Thickness]");
            mSB.AppendLine("Left Wheel limit=" + CData.Opt.aWheelMax[(int)EWay.L]);
            mSB.AppendLine("Right Wheel limit=" + CData.Opt.aWheelMax[(int)EWay.R]);
            mSB.AppendLine("Left Dresser limit=" + CData.Opt.aDresserMax[(int)EWay.L]);
            mSB.AppendLine("Right Dresser limit=" + CData.Opt.aDresserMax[(int)EWay.R]);
            mSB.AppendLine();
            //

            //20190309 ghk_level
            #region Auto
            mSB.AppendLine("[Lv Auto]");
            mSB.AppendLine("Manual=" + CData.Opt.iLvManual);
            mSB.AppendLine("Device=" + CData.Opt.iLvDevice);
            mSB.AppendLine("Wheel="  + CData.Opt.iLvWheel );
            mSB.AppendLine("Spc="    + CData.Opt.iLvSpc   );
            mSB.AppendLine("Option=" + CData.Opt.iLvOption);
            mSB.AppendLine("Util="   + CData.Opt.iLvUtil  );
            mSB.AppendLine("Exit="   + CData.Opt.iLvExit  );
            mSB.AppendLine();
            #endregion
            #region Manual
            mSB.AppendLine("[Lv Manual]");
            mSB.AppendLine("Warm Set="      + CData.Opt.iLvWarmSet);
            mSB.AppendLine("OnL="           + CData.Opt.iLvOnL);
            mSB.AppendLine("Inr="           + CData.Opt.iLvInr);
            mSB.AppendLine("Onp="           + CData.Opt.iLvOnp);
            mSB.AppendLine("GrL="           + CData.Opt.iLvGrL);
            mSB.AppendLine("Grd="           + CData.Opt.iLvGrd);
            mSB.AppendLine("Grr="           + CData.Opt.iLvGrr);
            mSB.AppendLine("Ofp="           + CData.Opt.iLvOfp);
            mSB.AppendLine("Dry="           + CData.Opt.iLvDry);
            mSB.AppendLine("OFL="           + CData.Opt.iLvOfL);
            mSB.AppendLine("All Servo On="  + CData.Opt.iLvAllSvOn);
            mSB.AppendLine("All Servo Off=" + CData.Opt.iLvAllSvOff);
            mSB.AppendLine();
            #endregion
            #region Device
            mSB.AppendLine("[Lv Device]");
            mSB.AppendLine("Group New="               + CData.Opt.iLvGpNew);
            mSB.AppendLine("Group Save As="           + CData.Opt.iLvGpSaveAs);
            mSB.AppendLine("Group Delete="            + CData.Opt.iLvGpDel);
            mSB.AppendLine("Device New="              + CData.Opt.iLvDvNew);
            mSB.AppendLine("Device Save As="          + CData.Opt.iLvDvSaveAs);
            mSB.AppendLine("Device Delete="           + CData.Opt.iLvDvDel);
            mSB.AppendLine("Device Load="             + CData.Opt.iLvDvLoad);
            mSB.AppendLine("Device Current="          + CData.Opt.iLvDvCurrent);
            mSB.AppendLine("Device Save="             + CData.Opt.iLvDvSave);
            mSB.AppendLine("Device Parameter Enable=" + CData.Opt.iLvDvParaEnable);
            mSB.AppendLine("Device Position View="    + CData.Opt.iLvDvPosView);
            mSB.AppendLine();
            #endregion
            #region Wheel
            mSB.AppendLine("[Lv Wheel]");
            mSB.AppendLine("Wheel New="       + CData.Opt.iLvWhlNew      );
            mSB.AppendLine("Wheel Save As="   + CData.Opt.iLvWhlSaveAs   );
            mSB.AppendLine("Wheel Delete="    + CData.Opt.iLvWhlDel      );
            mSB.AppendLine("Wheel Save="      + CData.Opt.iLvWhlSave     );
            mSB.AppendLine("Wheel Change="    + CData.Opt.iLvWhlChange   ); //190717 ksg : 추가
            mSB.AppendLine("Wheel DrsChange=" + CData.Opt.iLvWhlDrsChange); //190717 ksg : 추가
            mSB.AppendLine();
            #endregion
            #region Spc
            mSB.AppendLine("[Lv Spc]");
            mSB.AppendLine("Spc Graph Save="       + CData.Opt.iLvSpcGpSave);
            mSB.AppendLine("Spc Err List="         + CData.Opt.iLvSpcErrList);
            mSB.AppendLine("Spc Err History="      + CData.Opt.iLvSpcErrHis);
            mSB.AppendLine("Spc Err History View=" + CData.Opt.iLvSpcErrHisView);
            mSB.AppendLine("Spc Err HIstory Save=" + CData.Opt.iLvSpcErrHisSave);
            mSB.AppendLine();
            #endregion
            #region Option
            mSB.AppendLine("[Lv Option]");
            mSB.AppendLine("Option System Position="    + CData.Opt.iLvOptSysPos);
            mSB.AppendLine("Option Table Grinding="     + CData.Opt.iLvOptTbGrd);
            // 2020.10.09 SungTae Start : Add only Qorvo
            mSB.AppendLine("Option On/Off Loader="      + CData.Opt.iLvOptLoader);
            mSB.AppendLine("Option InRail/Dry="         + CData.Opt.iLvOptRailDry);
            mSB.AppendLine("Option On/Off Picker="      + CData.Opt.iLvOptPicker);
            mSB.AppendLine("Option Left/Right Grind="   + CData.Opt.iLvOptGrind);
            // 2020.10.09 SungTae End
            // 2021.07.15 lhs Start
            mSB.AppendLine("Option General="            + CData.Opt.iLvOptGen); 
            mSB.AppendLine("Option Maintenance="        + CData.Opt.iLvOptMnt);
            // 2021.07.15 lhs End
            mSB.AppendLine();
            #endregion
            #region Util
            mSB.AppendLine("[Lv Util]");
            mSB.AppendLine("Util Motion="     + CData.Opt.iLvUtilMot);
            mSB.AppendLine("Util Spindle="    + CData.Opt.iLvUtilSpd);
            mSB.AppendLine("Util Input="      + CData.Opt.iLvUtilIn);
            mSB.AppendLine("Util OutPut="     + CData.Opt.iLvUtilOut);
            mSB.AppendLine("Util Probe="      + CData.Opt.iLvUtilPrb);
            mSB.AppendLine("Util Tower Lamp=" + CData.Opt.iLvUtilTw);
            mSB.AppendLine("Util Barcode="    + CData.Opt.iLvUtilBcr);
            mSB.AppendLine("Util Repeat="     + CData.Opt.iLvUtilRepeat);
            mSB.AppendLine();
            #endregion
            //20190809 ghk_tableclean
            mSB.AppendLine("[Table Clean Count]");
            mSB.AppendLine("Left Table="  + CData.Opt.aTC_Cnt[(int)EWay.L].ToString());
            mSB.AppendLine("Right Table=" + CData.Opt.aTC_Cnt[(int)EWay.R].ToString());
            mSB.AppendLine("Cleaning Cycle=" + CData.Opt.iTC_Cycle.ToString());             // 2020-11-16, jhLee 추가
            mSB.AppendLine();
            #region OpManual
            //20191204 ghk_level
            mSB.AppendLine("[Lv OpManual]");
            mSB.AppendLine("OpManual Dresser Position=" + CData.Opt.iLvOpDrsPos);
            mSB.AppendLine("OpManual Strip Existence Edit=" + CData.Opt.iLvOpStripExistEdit);
            mSB.AppendLine();
            #endregion

            // 200330 mjy : 추가
            mSB.AppendLine("[Pick/Place Water Clean]");
            mSB.AppendLine("Pick Water Gap=" + CData.Opt.dPickGap);
            mSB.AppendLine("Pick Water Delay=" + CData.Opt.iPickDelay);
            mSB.AppendLine("Place Water Gap=" + CData.Opt.dPlaceGap);
            mSB.AppendLine("Place Water Delay=" + CData.Opt.iPlaceDelay);
            mSB.AppendLine();

			mSB.AppendLine("[QC Client]");
            mSB.AppendLine("QcVisionIp=" + CData.Opt.sQcServerIp);
            mSB.AppendLine("QcVisionPort=" + CData.Opt.sQcServerPort);
            mSB.AppendLine();

            // 2020.10.13 SungTae Start : Dresser Five Point Check
            mSB.AppendLine("[Dresser Five Check]");
            mSB.AppendLine("DrsFiveCheck=" + CData.Opt.bDrsFiveCheck.ToString());
            mSB.AppendLine();
            // 2020.10.13 SungTae End

            // 2020.10.22 JSKim St
            mSB.AppendLine("[Dual Pump Use Check]");
            mSB.AppendLine("DualPumpUse=" + CData.Opt.bDualPumpUse.ToString());
            mSB.AppendLine();
            // 2020.10.22 JSKim Ed

            //201005 pjh : Log Delete Period
            mSB.AppendLine("[Log Delete Period]");
            mSB.AppendLine("Log Delete Period=" + CData.Opt.iDelPeriod);
            mSB.AppendLine();
            //

            // 2020-11-17, jhLee : Skyworks 일정매수 투입 후 자동으로 Loading stop 되기위한 투입 수량
            mSB.AppendLine("[LoadingStop]");
            mSB.AppendLine("Auto Loading Stop Count=" + CData.Opt.iAutoLoadingStopCount);

            // 2021.10.25 SungTae : [수정] Code 확인 용이하도록 변경 (0 -> (int)eTypeLDStop.Inrail)
            //syc : ase kr loading stop
            if (!CDataOption.UseAutoLoadingStop) CData.Opt.iLoadingStopType = (int)eTypeLDStop.Inrail/*0*/;

            mSB.AppendLine("Loding Stop Type=" + CData.Opt.iLoadingStopType);
            mSB.AppendLine();

            // 2020-10-26, jhLee : 측정 정확도를 높이기 위해 Probe를 더 눌러주는 양
            mSB.AppendLine("[Motorize Probe Measure]");
            mSB.AppendLine("Probe Overdrive=" + CData.Opt.dProbeOD);
            mSB.AppendLine("Probe Stable Delay=" + CData.Opt.iProbeStableDelay);    // Probe 안정화 시간
            mSB.AppendLine("Safety Top Offset=" + CData.Opt.dSafetyTopOffset.ToString());
            mSB.AppendLine("ZAxis MoveUp Speed=" + CData.Opt.dZAxisMoveUpSpeed.ToString());
            mSB.AppendLine();
            //end of 2020-11-03, jhLee

            // 2021.04.09 SungTae Start : Wheel History Auto Delete Period
            mSB.AppendLine("[Wheel History Auto Delete Period]");
            if (CData.Opt.iDelPeriodHistory < 1)
                CData.Opt.iDelPeriodHistory = 1;
            mSB.AppendLine("Delete Period=" + CData.Opt.iDelPeriodHistory);
            mSB.AppendLine();
            // 2021.04.09 SungTae End


            // 2021.05.17 jhLee : 연속LOT (Multi-LOT) 기능 사용 여부, CDataOption.UseMultiLOT 이 활성화 된 상태에서 사용 가능
            mSB.AppendLine("[Multi LOT]");
            mSB.AppendLine("Multi LOT Use=" + CData.Opt.bMultiLOTUse);
            mSB.AppendLine("Empty Slot Count=" + CData.Opt.nLOTEndEmptySlot.ToString());   // // 연속 LOT에서 LOT의 구분을 위한 연속된 빈 Slot의 수량 3 ~ 5
            mSB.AppendLine();

            // 2021.09.14 Start : 이오나이저 Sol/V Off 설정 (SCK전용)
            mSB.AppendLine("[Ionizer]");
            mSB.AppendLine("IOZT SolValve Off Use="         + CData.Opt.bUseIOZTSolOff.ToString());
            mSB.AppendLine("IOZT SolValve Off Delay Sec="   + CData.Opt.nIOZTSolOffDelaySec.ToString());
            mSB.AppendLine();
            // 2021.09.14 End

            // 2022.03.22 Start : Dummy Thickness (2004U)
            if (CDataOption.Use2004U)
            {
                mSB.AppendLine("[Dummy]");
                mSB.AppendLine("Dummy Thickness=" + CData.Opt.dDummyThick.ToString());
                mSB.AppendLine();
            }
            // 2022.03.22 End


            //2020.07.11 lks
            CCheckChange.CheckChanged("OPTION", sPath, CCheckChange.ReadOldFile(sPath), mSB.ToString());

            CLog.Check_File_Access(sPath, mSB.ToString(), false);
        }

   //     public void Load_()
   //     {
   //         string sPath = GV.PATH_CONFIG + "Option.cfg";
   //         string sSec = "";
   //         string sVal = "";

   //         if (!File.Exists(sPath))
   //         {
   //             return;
   //         }

   //         _Init();

   //         CIni mIni = new CIni(sPath);
   //         sSec = "Warm Up";
   //         int.TryParse(mIni.Read(sSec, "Time"), out CData.Opt.iWmUT);
   //         int.TryParse(mIni.Read(sSec, "Spindle Speed"), out CData.Opt.iWmUS);
   //         int.TryParse(mIni.Read(sSec, "Idle Time"), out CData.Opt.iWmUI); //syc : Warm up 유휴 시간
   //         if (CData.Opt.iWmUI < 30) { CData.Opt.iWmUI = 30; }//syc : Warm up 유휴 시간
   //         bool.TryParse(mIni.Read(sSec, "Skip"), out CData.Opt.bWarmUpSkip);
   //         if (CData.CurCompany != ECompany.ASE_KR) CData.Opt.bWarmUpSkip = false; //190228 ksg :

   //         sSec = "Wheel Measure";
   //         int.TryParse(mIni.Read(sSec, "Spindle Speed"), out CData.Opt.iMeaS);
   //         int.TryParse(mIni.Read(sSec, "Time"), out CData.Opt.iMeaT);

   //         sSec = "Table Grinding";
   //         CData.Opt.dTbGrdAir = mIni.ReadD(sSec, "Air Cut Depth");
   //         DateTime.TryParse(mIni.Read(sSec, "Left Last Grinding Date"), out CData.Opt.dtLast_L);
   //         DateTime.TryParse(mIni.Read(sSec, "Right Last Grinding Date"), out CData.Opt.dtLast_R);

   //         sSec = "Table Grinding - Step 01";
   //         CData.Opt.aTbGrd[0].dTotalDep = mIni.ReadD(sSec, "Total Depth");
   //         CData.Opt.aTbGrd[0].dCycleDep = mIni.ReadD(sSec, "Cycle Depth");
   //         CData.Opt.aTbGrd[0].dTblSpd = mIni.ReadD(sSec, "Table Speed");
   //         CData.Opt.aTbGrd[0].iSplSpd = mIni.ReadI(sSec, "Spindle Speed");

   //         sSec = "Table Grinding - Step 02";
   //         CData.Opt.aTbGrd[1].dTotalDep = mIni.ReadD(sSec, "Total Depth");
   //         CData.Opt.aTbGrd[1].dCycleDep = mIni.ReadD(sSec, "Cycle Depth");
   //         CData.Opt.aTbGrd[1].dTblSpd = mIni.ReadD(sSec, "Table Speed");
   //         CData.Opt.aTbGrd[1].iSplSpd = mIni.ReadI(sSec, "Spindle Speed");

   //         //191203 ksg : Table Grd Manual Set Pos
   //         CData.Opt.LeftTopPos = mIni.ReadD(sSec, "Table Left Top");
   //         CData.Opt.LeftBtmPos = mIni.ReadD(sSec, "Table Left Btm");
   //         CData.Opt.RightTopPos = mIni.ReadD(sSec, "Table Right Top");
   //         CData.Opt.RightBtmPos = mIni.ReadD(sSec, "Table Right Btm");
   //         CData.Opt.LeftXPos = mIni.ReadD(sSec, "Table Left  X Top");
   //         CData.Opt.RightXPos = mIni.ReadD(sSec, "Table Right X Top");
   //         if (CDataOption.TblSetPos == eTblSetPos.NotUse)
   //         {
   //             CData.Opt.LeftTopPos = 0;
   //             CData.Opt.LeftBtmPos = 0;
   //             CData.Opt.RightTopPos = 0;
   //             CData.Opt.RightBtmPos = 0;
   //             CData.Opt.LeftXPos = 0;
   //             CData.Opt.RightXPos = 0;
   //         }

   //         // 피커 대기 시간
   //         sSec = "OnLoaderPicker Option";
   //         CData.Opt.aOnpRinseTime[(int)EWay.L] = mIni.ReadI(sSec, "Left Rinse Time");
   //         CData.Opt.aOnpRinseTime[(int)EWay.R] = mIni.ReadI(sSec, "Right Rinse Time");

   //         // ofppadclean
   //         sSec = "OffLoaderPicker Option";
   //         CData.Opt.iOfpPadCleanTime = mIni.ReadI(sSec, "Pad Clean Time");
   //         //

   //         //ksg 변수명 변경 해야 됨(i)
   //         sSec = "LOT";
   //         CData.Opt.iLotCnt = mIni.ReadI(sSec, "Max Magazine Count");
   //         bool.TryParse(mIni.Read(sSec, "Start Table Clean"), out CData.Opt.bLotTblClean); //200401 jym
   //         bool.TryParse(mIni.Read(sSec, "End Dressing"), out CData.Opt.bLotDrs); //200401 jym

   //         sSec = "MAGAZIN";
   //         CData.Opt.iEmptySlotCnt = mIni.ReadI(sSec, "Empty Slot Cnt");
   //         Enum.TryParse(mIni.Read(sSec, "Emit"), out CData.Opt.eEmitMgz); //20200513 jym : 매거진 배출 위치 변경 추가
   //         if ((int)CData.Opt.eEmitMgz == -1)
   //         { CData.Opt.eEmitMgz = EMgzWay.Btm; }

   //         //sSec = "Barcode";
   //         //bool.TryParse(mIni.Read(sSec, "Skip"), out CData.Opt.bBcrSkip);

   //         sSec = "Talbe Skip";
   //         bool.TryParse(mIni.Read(sSec, "LTableSkip"), out CData.Opt.aTblSkip[(int)EWay.L]);
   //         bool.TryParse(mIni.Read(sSec, "RTableSkip"), out CData.Opt.aTblSkip[(int)EWay.R]);
   //         bool.TryParse(mIni.Read(sSec, "LeakSkip"  ), out CData.Opt.bLeakSkip  ); //190109 ksg :

   //         sSec = "Door";
   //         bool.TryParse(mIni.Read(sSec, "Skip"     ), out CData.Opt.bDoorSkip );
   //         if(CData.CurCompany == ECompany.SkyWorks) CData.Opt.bDoorSkip = false; //190429 ksg : Sky Works는 옵션 안씀
   //         bool.TryParse(mIni.Read(sSec, "CoverSkip"), out CData.Opt.bCoverSkip); //190319 ksg :
   //         //ksg 추가
   //         sSec = "Dry";
   //         bool.TryParse(mIni.Read(sSec, "Skip"), out CData.Opt.bDryStickSkip);
   //         int.TryParse(mIni.Read(sSec, "Spg Idle Time"), out CData.Opt.iSWIdT); //syc : Sponge water auto on
   //         int.TryParse(mIni.Read(sSec, "Spg On Time"), out CData.Opt.iSWOnT); //syc : Sponge water auto on

   //         sSec = "DryRun";
   //         bool.TryParse(mIni.Read(sSec, "Skip"), out CData.Opt.bDryAuto);

   //         //20190703 ghk_automeasure
   //         sSec = "DryRunMeasure";
   //         bool.TryParse(mIni.Read(sSec, "Skip"), out CData.Opt.bDryAutoMeaStripSkip);
   //         if (CData.CurCompany != ECompany.SkyWorks)
   //         { CData.Opt.bDryAutoMeaStripSkip = true; }//191107 ksg : 

   //         //190109 ksg :
   //         sSec = "OffLoadPicker";
   //         bool.TryParse(mIni.Read(sSec, "Skip"), out CData.Opt.bOfpBtmCleanSkip);

   //         //190109 ksg :
   //         sSec = "WheelJig";
   //         bool.TryParse(mIni.Read(sSec, "Skip"), out CData.Opt.bWhlJigSkip);

   //         if (CData.CurCompany == ECompany.JSCK || CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JCET)
   //         { CData.Opt.bWhlJigSkip = false; } //200121 ksg : JSCK는 이 옵션 안씀 , 200625 lks

   //         //190109 ksg :
   //         sSec = "OfLMgzOption";
   //         bool.TryParse(mIni.Read(sSec, "On"  ), out CData.Opt.bOfLMgzMatchingOn);
   //         bool.TryParse(mIni.Read(sSec, "Dir" ), out CData.Opt.bFirstTopSlot    ); //190503 ksg :
   //         bool.TryParse(mIni.Read(sSec, "Dir1"), out CData.Opt.bFirstTopSlotOff ); //190511 ksg :

   //         //190404 ksg : Qc
   //         sSec = "QcVision";
   //         bool.TryParse(mIni.Read(sSec, "Use"   ), out CData.Opt.bQcUse   );
   //         bool.TryParse(mIni.Read(sSec, "ByPass"), out CData.Opt.bQcByPass); //190406 ksg : Qc
   //         if(CData.CurCompany != ECompany.SkyWorks)
   //         {
   //             CData.Opt.bQcUse = false;
   //         }

   //         //MTBA MTBF
   //         sSec = "MTBA MTBF";
   //         double.TryParse(mIni.Read(sSec, "Day Working"       ), out CData.Opt.dDayWorking);
   //         int   .TryParse(mIni.Read(sSec, "Calculation Period"), out CData.Opt.iPeriod    );
   //         int   .TryParse(mIni.Read(sSec, "Time for Renewal"  ), out CData.Opt.iRenewal   );
   //         int   .TryParse(mIni.Read(sSec, "MTBA Set Time"     ), out CData.Opt.iSetTime   );
   //         //

   //         sSec = "Wheel TTV";
   //         CData.Opt.dWhlTTV = mIni.ReadD(sSec, "TTV");

   //         //Auto Level Change Time 저장 추가
   //         sSec = "Lv Change";
   //         CData.Opt.iChangeLevel = mIni.ReadI(sSec, "Minute");//190509 ksg :

   //         //Select Language 저장 추가
   //         sSec = "Language";
   //         CData.Opt.iSelLan = mIni.ReadI(sSec, "Select");//190516 ksg :

   //         //190628 ksg :
   //         sSec = "Probe";
   //         bool.TryParse(mIni.Read(sSec, "Type"   ), out CData.Opt.bPbType   );

   //         //20191010 ghk_manual_bcr
   //         sSec = "Manual Bcr";
   //         if (mIni.Read(sSec, "ManBcr") == "")
   //         { CData.Opt.bManBcrSkip = true; }
   //         else
   //         { bool.TryParse(mIni.Read(sSec, "ManBcr"), out CData.Opt.bManBcrSkip); }

   //         sSec = "Secs Use";
   //         if (mIni.Read(sSec, "Use") == "")
   //         { CData.Opt.bSecsUse = false; }
   //         else
   //         { bool.TryParse(mIni.Read(sSec, "Use"), out CData.Opt.bSecsUse); }

   //         if (CDataOption.ManualBcr == eManualBcr.NotUse)
   //         { CData.Opt.bManBcrSkip = true; }
   //         //
   //         //200203 ksg :
   //         sSec = "RFID Skip";
   //         if (mIni.Read(sSec, "OnlRFIDSkip") == "")
   //         { CData.Opt.bOnlRfidSkip = true; }
   //         else
   //         { bool.TryParse(mIni.Read(sSec, "OnlRFIDSkip"), out CData.Opt.bOnlRfidSkip); }

   //         if (mIni.Read(sSec, "OflRFIDSkip") == "")
   //         { CData.Opt.bOflRfidSkip = true; }
   //         else
   //         { bool.TryParse(mIni.Read(sSec, "OflRFIDSkip"), out CData.Opt.bOflRfidSkip); }

   //         if (CDataOption.OnlRfid == eOnlRFID.NotUse)
   //         { CData.Opt.bOnlRfidSkip = true; }
   //         if (CDataOption.OflRfid == eOflRFID.NotUse)
   //         { CData.Opt.bOflRfidSkip = true; }
            
   //         // 200318 mjy : 신규추가
   //         CData.Opt.iDryDelay = mIni.ReadI("Dry", "Delay");
            
			////200318 ksg : Wheel One Point Check
   //         sSec = "Wheel One Check";
   //         if (mIni.Read(sSec, "WhlOneCheck") == "")
   //         { CData.Opt.bWhlOneCheck = false; }
   //         else
   //         { bool.TryParse(mIni.Read(sSec, "WhlOneCheck"), out CData.Opt.bWhlOneCheck); }
   //         // 200728 jym : 휠 클린 노즐 추가
   //         sSec = "Wheel Clean Nozzle";
   //         if ( mIni.Read(sSec, "Skip") == "")
   //         { CData.Opt.bWhlClnSkip = true; }
   //         else
   //         { bool.TryParse(mIni.Read(sSec, "Skip"), out CData.Opt.bWhlClnSkip); }
   //         // 200820 jym : Network drive 설정 
   //         sSec = "Network Drive";
   //         bool.TryParse(mIni.Read(sSec, "Use"), out CData.Opt.bNetUse);
   //         CData.Opt.sNetIP = mIni.Read(sSec, "Ip");
   //         CData.Opt.sNetPath = mIni.Read(sSec, "Path");
   //         CData.Opt.sNetID = mIni.Read(sSec, "Id");
   //         CData.Opt.sNetPw = mIni.Read(sSec, "Pw");
   //         // 2020.08.21 JSKim St : Probe Value 표시 Font Size, 소수점 자리수
   //         sSec = "Display Probe Value";
   //         int.TryParse(mIni.Read(sSec, "ProbeFontSize"), out CData.Opt.iProbeFontSize);
   //         int.TryParse(mIni.Read(sSec, "ProbeFloationPoint"), out CData.Opt.iProbeFloationPoint);
   //         // 2020.08.21 JSKim Ed

   //         // 200917 jym
   //         sSec = "General Skip";
   //         sVal = mIni.Read(sSec, "DiChiller");
   //         if (sVal.Equals(""))
   //         { CData.Opt.bDiChillerSkip = true; }
   //         else
   //         { bool.TryParse(mIni.Read(sSec, "DiChiller"), out CData.Opt.bDiChillerSkip); }

   //         //20190309 ghk_level
   //         #region Auto
   //         sSec = "Lv Auto";
   //         int.TryParse(mIni.Read(sSec, "Manual"), out CData.Opt.iLvManual);
   //         int.TryParse(mIni.Read(sSec, "Device"), out CData.Opt.iLvDevice);
   //         int.TryParse(mIni.Read(sSec, "Wheel" ), out CData.Opt.iLvWheel );
   //         int.TryParse(mIni.Read(sSec, "Spc"   ), out CData.Opt.iLvSpc   );
   //         int.TryParse(mIni.Read(sSec, "Option"), out CData.Opt.iLvOption);
   //         int.TryParse(mIni.Read(sSec, "Util"  ), out CData.Opt.iLvUtil  );
   //         int.TryParse(mIni.Read(sSec, "Exit"  ), out CData.Opt.iLvExit  );
   //         #endregion

   //         #region Manual
   //         sSec = "Lv Manual";
   //         int.TryParse(mIni.Read(sSec, "Warm Set"), out CData.Opt.iLvWarmSet);
   //         int.TryParse(mIni.Read(sSec, "OnL"), out CData.Opt.iLvOnL);
   //         int.TryParse(mIni.Read(sSec, "Inr"), out CData.Opt.iLvInr);
   //         int.TryParse(mIni.Read(sSec, "Onp"), out CData.Opt.iLvOnp);
   //         int.TryParse(mIni.Read(sSec, "GrL"), out CData.Opt.iLvGrL);
   //         int.TryParse(mIni.Read(sSec, "Grd"), out CData.Opt.iLvGrd);
   //         int.TryParse(mIni.Read(sSec, "Grr"), out CData.Opt.iLvGrr);
   //         int.TryParse(mIni.Read(sSec, "Ofp"), out CData.Opt.iLvOfp);
   //         int.TryParse(mIni.Read(sSec, "Dry"), out CData.Opt.iLvDry);
   //         int.TryParse(mIni.Read(sSec, "OFL"), out CData.Opt.iLvOfL);
   //         //20191204 ghk_level
   //         int.TryParse(mIni.Read(sSec, "All Servo On"), out CData.Opt.iLvAllSvOn);
   //         int.TryParse(mIni.Read(sSec, "All Servo Off"), out CData.Opt.iLvAllSvOff);
   //         //
   //         #endregion

   //         #region Device
   //         sSec = "Lv Device";
   //         int.TryParse(mIni.Read(sSec, "Group New"), out CData.Opt.iLvGpNew);
   //         int.TryParse(mIni.Read(sSec, "Group Save As"), out CData.Opt.iLvGpSaveAs);
   //         int.TryParse(mIni.Read(sSec, "Group Delete"), out CData.Opt.iLvGpDel);
   //         int.TryParse(mIni.Read(sSec, "Device New"), out CData.Opt.iLvDvNew);
   //         int.TryParse(mIni.Read(sSec, "Device Save As"), out CData.Opt.iLvDvSaveAs);
   //         int.TryParse(mIni.Read(sSec, "Device Delete"), out CData.Opt.iLvDvDel);
   //         int.TryParse(mIni.Read(sSec, "Device Load"), out CData.Opt.iLvDvLoad);
   //         int.TryParse(mIni.Read(sSec, "Device Current"), out CData.Opt.iLvDvCurrent);
   //         int.TryParse(mIni.Read(sSec, "Device Save"), out CData.Opt.iLvDvSave);
   //         //20191203 ghk_level
   //         int.TryParse(mIni.Read(sSec, "Device Parameter Enable"), out CData.Opt.iLvDvParaEnable);
   //         int.TryParse(mIni.Read(sSec, "Device Position View"), out CData.Opt.iLvDvPosView);
   //         //
   //         #endregion

   //         #region Wheel
   //         sSec = "Lv Wheel";
   //         int.TryParse(mIni.Read(sSec, "Wheel New"      ), out CData.Opt.iLvWhlNew      );
   //         int.TryParse(mIni.Read(sSec, "Wheel Save As"  ), out CData.Opt.iLvWhlSaveAs   );
   //         int.TryParse(mIni.Read(sSec, "Wheel Delete"   ), out CData.Opt.iLvWhlDel      );
   //         int.TryParse(mIni.Read(sSec, "Wheel Save"     ), out CData.Opt.iLvWhlSave     );
   //         int.TryParse(mIni.Read(sSec, "Wheel Change"   ), out CData.Opt.iLvWhlChange   ); //190717 ksg : 추가
   //         int.TryParse(mIni.Read(sSec, "Wheel DrsChange"), out CData.Opt.iLvWhlDrsChange); //190717 ksg : 추가
   //         #endregion

   //         #region Spc
   //         sSec = "Lv Spc";
   //         int.TryParse(mIni.Read(sSec, "Spc Graph Save"), out CData.Opt.iLvSpcGpSave);
   //         int.TryParse(mIni.Read(sSec, "Spc Err List"), out CData.Opt.iLvSpcErrList);
   //         int.TryParse(mIni.Read(sSec, "Spc Err History"), out CData.Opt.iLvSpcErrHis);
   //         int.TryParse(mIni.Read(sSec, "Spc Err History View"), out CData.Opt.iLvSpcErrHisView);
   //         int.TryParse(mIni.Read(sSec, "Spc Err HIstory Save"), out CData.Opt.iLvSpcErrHisSave);
   //         #endregion

   //         #region Option
   //         sSec = "Lv Option";
   //         int.TryParse(mIni.Read(sSec, "Option System Position"), out CData.Opt.iLvOptSysPos);
   //         int.TryParse(mIni.Read(sSec, "Option Table Grinding"), out CData.Opt.iLvOptTbGrd);
   //         #endregion

   //         #region Util
   //         sSec = "Lv Util";
   //         int.TryParse(mIni.Read(sSec, "Util Motion"), out CData.Opt.iLvUtilMot);
   //         int.TryParse(mIni.Read(sSec, "Util Spindle"), out CData.Opt.iLvUtilSpd);
   //         int.TryParse(mIni.Read(sSec, "Util Input"), out CData.Opt.iLvUtilIn);
   //         int.TryParse(mIni.Read(sSec, "Util OutPut"), out CData.Opt.iLvUtilOut);
   //         int.TryParse(mIni.Read(sSec, "Util Probe"), out CData.Opt.iLvUtilPrb);
   //         int.TryParse(mIni.Read(sSec, "Util Tower Lamp"), out CData.Opt.iLvUtilTw);
   //         int.TryParse(mIni.Read(sSec, "Util Barcode"), out CData.Opt.iLvUtilBcr);
   //         int.TryParse(mIni.Read(sSec, "Util Repeat"), out CData.Opt.iLvUtilRepeat);
   //         #endregion

   //         #region OpManual
   //         //20191204 ghk_level
   //         sSec = "Lv OpManual";
   //         int.TryParse(mIni.Read(sSec, "OpManual Dresser Position"), out CData.Opt.iLvOpDrsPos);
   //         #endregion
   //         //
   //         //20190809 ghk_tableclean
   //         sSec = "Table Clean Count";
   //         int.TryParse(mIni.Read(sSec, "Left Table"), out CData.Opt.aTC_Cnt[(int)EWay.L]);
   //         int.TryParse(mIni.Read(sSec, "Right Table"), out CData.Opt.aTC_Cnt[(int)EWay.R]);

   //         // 200330 mjy : 추가
   //         sSec = "Pick/Place Water Clean";
   //         CData.Opt.dPickGap = mIni.ReadD(sSec, "Pick Water Gap");
   //         CData.Opt.iPickDelay = mIni.ReadI(sSec, "Pick Water Delay");
   //         CData.Opt.dPlaceGap = mIni.ReadD(sSec, "Place Water Gap");
   //         CData.Opt.iPlaceDelay = mIni.ReadI(sSec, "Place Water Delay");

   //         // 200619 lks : 추가
   //         sSec = "QC Client";
   //         string qcip = mIni.Read(sSec, "QcVisionIp");
   //         if (string.IsNullOrEmpty(qcip)) qcip = "10.0.0.1";
   //         string qcport = mIni.Read(sSec, "QcVisionPort");
   //         if (string.IsNullOrEmpty(qcport)) qcport = "5500";
   //         CData.Opt.sQcServerIp = qcip;
   //         CData.Opt.sQcServerPort = qcport;
   //     }

        public void Load()
        {
            string sPath = GV.PATH_CONFIG + "Option.cfg";
            if (!File.Exists(sPath))
            { return; }

            _Init();
            IniFile mIni = new IniFile();
            mIni.Load(sPath);
            CData.Opt.iWmUT = mIni["Warm Up"]["Time"].ToInt();
            CData.Opt.iWmUS = mIni["Warm Up"]["Spindle Speed"].ToInt();
            CData.Opt.iWmUI = mIni["Warm Up"]["Idle Time"].ToInt();
            //if (CData.Opt.iWmUI < 30)                   {   CData.Opt.iWmUI = 30;   }   //syc : Warm up 유휴 시간
            CData.Opt.bWarmUpWithStrip = mIni["Warm Up"]["WarmUpWithStrip"].ToBool(false);   // 2022.06.10 lhs : (SCK+)자재가 있어도 워밍업
            CData.Opt.bWarmUpSkip = mIni["Warm Up"]["Skip"].ToBool(false);
            if (CData.CurCompany != ECompany.ASE_KR)    {   CData.Opt.bWarmUpSkip = false;  }            
            CData.Opt.iAWT      = mIni["Warm Up"]["Auto time"].ToInt(15);       // 201023 jym : 추가
            CData.Opt.iAWP      = mIni["Warm Up"]["Auto period"].ToInt(15);
            CData.Opt.bAwSkip   = mIni["Warm Up"]["Auto skip"].ToBool(false);   // 201029 jym : Add

            CData.Opt.iMeaS = mIni["Wheel Measure"]["Spindle Speed"].ToInt();
            CData.Opt.iMeaT = mIni["Wheel Measure"]["Time"].ToInt();

            CData.Opt.dTbGrdAir = mIni["Table Grinding"]["Air Cut Depth"].ToDouble(0);
            DateTime.TryParse(mIni["Table Grinding"]["Left Last Grinding Date"].GetString(), out CData.Opt.dtLast_L);
            DateTime.TryParse(mIni["Table Grinding"]["Right Last Grinding Date"].GetString(), out CData.Opt.dtLast_R);

            CData.Opt.aTbGrd[0].dTotalDep = mIni["Table Grinding - Step 01"]["Total Depth"  ].ToDouble();
            CData.Opt.aTbGrd[0].dCycleDep = mIni["Table Grinding - Step 01"]["Cycle Depth"  ].ToDouble();
            CData.Opt.aTbGrd[0].dTblSpd   = mIni["Table Grinding - Step 01"]["Table Speed"  ].ToDouble();
            CData.Opt.aTbGrd[0].iSplSpd   = mIni["Table Grinding - Step 01"]["Spindle Speed"].ToInt();

            CData.Opt.aTbGrd[1].dTotalDep = mIni["Table Grinding - Step 02"]["Total Depth"  ].ToDouble();
            CData.Opt.aTbGrd[1].dCycleDep = mIni["Table Grinding - Step 02"]["Cycle Depth"  ].ToDouble();
            CData.Opt.aTbGrd[1].dTblSpd   = mIni["Table Grinding - Step 02"]["Table Speed"  ].ToDouble();
            CData.Opt.aTbGrd[1].iSplSpd   = mIni["Table Grinding - Step 02"]["Spindle Speed"].ToInt();

            // Table Grd Manual Set Pos
            CData.Opt.LeftTopPos    = mIni["Table Grinding - Step 02"]["Table Left Top"     ].ToDouble();
            CData.Opt.LeftBtmPos    = mIni["Table Grinding - Step 02"]["Table Left Btm"     ].ToDouble();
            CData.Opt.RightTopPos   = mIni["Table Grinding - Step 02"]["Table Right Top"    ].ToDouble();
            CData.Opt.RightBtmPos   = mIni["Table Grinding - Step 02"]["Table Right Btm"    ].ToDouble();
            CData.Opt.LeftXPos      = mIni["Table Grinding - Step 02"]["Table Left  X Top"  ].ToDouble();
            CData.Opt.RightXPos     = mIni["Table Grinding - Step 02"]["Table Right X Top"  ].ToDouble();

            // 2021.10.08 SungTae Start : [추가] 5um로 Fix 해서 사용하는 것에서 Limit을 설정해서 사용할 수 있도록 변경 요청(ASE-KR VOC)
            CData.Opt.LeftLimitTtoB  = mIni["Table Grinding - Step 02"]["Left  Limit T to B"].ToDouble();
            CData.Opt.LeftLimitLtoR  = mIni["Table Grinding - Step 02"]["Left  Limit L to R"].ToDouble();
            CData.Opt.RightLimitTtoB = mIni["Table Grinding - Step 02"]["Right Limit T to B"].ToDouble();
            CData.Opt.RightLimitLtoR = mIni["Table Grinding - Step 02"]["Right Limit L to R"].ToDouble();
            // 2021.10.08 SungTae End

            if (CDataOption.TblSetPos == eTblSetPos.NotUse)
            {
                CData.Opt.LeftTopPos     = 0;
                CData.Opt.LeftBtmPos     = 0;
                CData.Opt.RightTopPos    = 0;
                CData.Opt.RightBtmPos    = 0;
                CData.Opt.LeftXPos       = 0;
                CData.Opt.RightXPos      = 0;

                // 2021.10.08 SungTae Start : [추가] 5um로 Fix 해서 사용하는 것에서 Limit을 설정해서 사용할 수 있도록 변경 요청(ASE-KR VOC)
                CData.Opt.LeftLimitTtoB  = 0;
                CData.Opt.LeftLimitLtoR  = 0;
                CData.Opt.RightLimitTtoB = 0;
                CData.Opt.RightLimitLtoR = 0;
                // 2021.10.08 SungTae End
            }

            // 2021.04.05 SungTae : Table Grinding 허용 가능 잔여량 표시 위해 추가(ASE-KR)
            CData.Opt.aLTblMeasPos[0] = mIni["Table Grinding - Step 02"]["L-Table Measure Top"].ToDouble();
            CData.Opt.aLTblMeasPos[1] = mIni["Table Grinding - Step 02"]["L-Table Measure Btm"].ToDouble();
            CData.Opt.aRTblMeasPos[0] = mIni["Table Grinding - Step 02"]["R-Table Measure Top"].ToDouble();
            CData.Opt.aRTblMeasPos[1] = mIni["Table Grinding - Step 02"]["R-Table Measure Btm"].ToDouble();
            // 2021.04.05 SungTae

            // 피커 대기 시간
            CData.Opt.aOnpRinseTime[(int)EWay.L] = mIni["OnLoaderPicker Option"]["Left Rinse Time" ].ToInt() ;
            CData.Opt.aOnpRinseTime[(int)EWay.R] = mIni["OnLoaderPicker Option"]["Right Rinse Time"].ToInt();

            // ofppadclean
            CData.Opt.iOfpPadCleanTime = mIni["OffLoaderPicker Option"]["Pad Clean Time"].ToInt();

            //ksg 변수명 변경 해야 됨(i)
            CData.Opt.iLotCnt = mIni["LOT"]["Max Magazine Count"].ToInt();
             CData.Opt.bLotTblClean =mIni["LOT"]["Start Table Clean"].ToBool(); //200401 jym
            CData.Opt.bLotDrs       =mIni["LOT"]["End Dressing"].ToBool(); //200401 jym

            CData.Opt.iEmptySlotCnt = mIni["MAGAZIN"]["Empty Slot Cnt"].ToInt();
            Enum.TryParse(mIni["MAGAZIN"]["Emit"].ToString(), out CData.Opt.eEmitMgz); //20200513 jym : 매거진 배출 위치 변경 추가

            if ((int)CData.Opt.eEmitMgz == -1)
            { CData.Opt.eEmitMgz = EMgzWay.Btm; }

            CData.Opt.aTblSkip[(int)EWay.L] = mIni["Talbe Skip"]["LTableSkip"].ToBool();
            CData.Opt.aTblSkip[(int)EWay.R] = mIni["Talbe Skip"]["RTableSkip"].ToBool(); 
            CData.Opt.bLeakSkip = mIni["Talbe Skip"]["LeakSkip"].ToBool();

            CData.Opt.bDoorSkip = mIni["Door"]["Skip"].ToBool();
            if (CData.CurCompany == ECompany.SkyWorks)
            { CData.Opt.bDoorSkip = false; }
            CData.Opt.bCoverSkip = mIni["Door"]["CoverSkip"].ToBool();

            // Dryer
            CData.Opt.bDryStickSkip      = mIni["Dry"]["Skip"].ToBool();
            CData.Opt.bSpgWtAutoOnOff    = mIni["Dry"]["Sponge Water Auto OnOff"].ToBool(true);  //2022.06.14 lhs : Sponge water auto on/off 사용 여부
            CData.Opt.nSpgWtOnMin        = mIni["Dry"]["Sponge Water On Time"].ToInt(5);      //2022.06.14 lhs : Sponge water auto on time
            CData.Opt.nSpgWtOffMin       = mIni["Dry"]["Sponge Water Off Time"].ToInt(10);     //2022.06.14 lhs : Sponge water auto off time
            CData.Opt.nDryZUpStableDelay = mIni["Dry"]["ZUpStableDelay"].ToInt();   // 2022.01.26 lhs Start : Dryer Z축 Up 후에 안정화 시간 갖기, Pusher 걸리지 않고 동작시키기 위해

            CData.Opt.bDryAuto = mIni["DryRun"]["Skip"].ToBool();

            CData.Opt.bDryAutoMeaStripSkip = mIni["DryRunMeasure"]["Skip"].ToBool();
            if (CData.CurCompany != ECompany.SkyWorks)
            { CData.Opt.bDryAutoMeaStripSkip = true; }

            CData.Opt.bOfpBtmCleanSkip = mIni["OffLoadPicker"]["Skip"].ToBool();

            // 2021.03.18 SungTae Start : 고객사(ASE-KR) 요청으로 Center Position Option 추가
            if (CData.CurCompany == ECompany.ASE_KR)
            {
                CData.Opt.bOfpUseCenterPos = mIni["OffLoadPicker"]["Use"].ToBool();
                CData.Opt.iOfpCleaningMode = mIni["OffLoadPicker"]["CleaningMode"].ToInt();         // 2021.07.02 SungTae : [추가] 고객사(ASE-KR) 요청으로 Strip Cleaning Mode 추가
            }
            else
            {
                CData.Opt.bOfpUseCenterPos = false;
                CData.Opt.iOfpCleaningMode = (int)eCleanMode.BASIC;
            }

            CData.Opt.bOfpCoverDryerSkip = mIni["OffLoadPicker"]["CoverDryerSkip"].ToBool();    // 2021.04.09 lhs

            CData.Opt.bWhlJigSkip = mIni["WheelJig"]["Skip"].ToBool();

            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK || CData.CurCompany == ECompany.JCET)
            { CData.Opt.bWhlJigSkip = false; } //200121 ksg : JSCK는 이 옵션 안씀 , 200625 lks

            CData.Opt.bOfLMgzMatchingOn = mIni["OfLMgzOption"]["On"].ToBool();
            CData.Opt.bFirstTopSlot     = mIni["OfLMgzOption"]["Dir"].ToBool();
            CData.Opt.bFirstTopSlotOff  = mIni["OfLMgzOption"]["Dir1"].ToBool();
            CData.Opt.bOfLMgzUnloadType = mIni["OfLMgzOption"]["Unload Type"].ToBool();     // 2021.12.08 SungTae : [추가] (Qorvo향 VOC)

            CData.Opt.bQcUse = mIni["QcVision"]["Use"].ToBool(false);
            CData.Opt.bQcByPass = mIni["QcVision"]["ByPass"].ToBool(true);
            CData.Opt.bQcSimulation = mIni["QcVision"]["SIMULATION"].ToBool(false);
            // 201116 jym : QC와 오프로더 도어 센서 연동
            CData.Opt.bQcDoor = mIni["QcVision"]["Door"].ToBool(true);

            // MTBA MTBF
            CData.Opt.dDayWorking = mIni["MTBA MTBF"]["Day Working"].ToDouble();
            CData.Opt.iPeriod = mIni["MTBA MTBF"] ["Calculation Period"].ToInt();
            CData.Opt.iRenewal = mIni["MTBA MTBF"]["Time for Renewal"].ToInt();
            CData.Opt.iSetTime = mIni["MTBA MTBF"]["MTBA Set Time"].ToInt();

            CData.Opt.aWhlTTV[(int)EWay.L] = mIni["Wheel TTV"]["TTV"].ToDouble();
            CData.Opt.aWhlTTV[(int)EWay.R] = mIni["Wheel TTV"]["TTV Right"].ToDouble(CData.Opt.aWhlTTV[(int)EWay.L]);
            //Auto Level Change Time 저장 추가
            CData.Opt.iChangeLevel = mIni["Lv Change"]["Minute"].ToInt();

            //Select Language 저장 추가
            CData.Opt.iSelLan = mIni["Language"]["Select"].ToInt();
            CData.Opt.bPbType = mIni["Probe"]["Type"].ToBool();
            // 2020-12-14, jhLee : 기존 MeaStrip_T1, T2를 통합한 함수를 이용할것인지 여부
            CData.Opt.bMeasureFunctionType = mIni["Probe"]["MeasureFunctionType"].ToBool();

            CData.Opt.bManBcrSkip = mIni["Manual Bcr"]["ManBcr"].ToBool(true);
            if (CDataOption.ManualBcr == eManualBcr.NotUse)
            { CData.Opt.bManBcrSkip = true; }

            CData.Opt.bSecsUse = mIni["Secs Use"]["Use"].ToBool();
            // 2020.10.19, jhLee : HOST의 START RCMD 회신을 기다리는 시간 ms, 0이면 묻지도 않고, 기다리지 않는다.
            CData.Opt.iRCMDStartTimeout = mIni["Secs Use"]["Host RCMD Start Timeout"].ToInt();

            //201005 pjh : SECS/GEM Not Use 일 때 Option값 False 
            if (CDataOption.SecsUse == eSecsGem.NotUse)
            {
                CData.Opt.bSecsUse = false;
                CData.Opt.iRCMDStartTimeout = 0;                    // HOST에게 START 여부를 묻는 기능 미사용
            }

            CData.Opt.bOnlRfidSkip = mIni["RFID Skip"]["OnlRFIDSkip"].ToBool(true);
            CData.Opt.bOflRfidSkip = mIni["RFID Skip"]["OflRFIDSkip"].ToBool(true);
            if (CDataOption.OnlRfid == eOnlRFID.NotUse)
            { CData.Opt.bOnlRfidSkip = true; }
            if (CDataOption.OflRfid == eOflRFID.NotUse)
            { CData.Opt.bOflRfidSkip = true; }

            // 200318 mjy : 신규추가
            CData.Opt.iDryDelay = mIni["Dry"]["Delay"].ToInt();
            CData.Opt.bWhlOneCheck = mIni["Wheel One Check"]["WhlOneCheck"].ToBool();

            // 200728 jym : 휠 클린 노즐 추가
            CData.Opt.bWhlClnSkip = mIni["Wheel Clean Nozzle"]["Skip"].ToBool(true);

            // 200820 jym : Network drive 설정 
            CData.Opt.bNetUse = mIni["Network Drive"]["Use"].ToBool();
            CData.Opt.sNetIP = mIni["Network Drive"]["Ip"].GetString();
            CData.Opt.sNetPath = mIni["Network Drive"]["Path"].GetString();
            CData.Opt.sNetID =   mIni["Network Drive"]["Id"].GetString();
            CData.Opt.sNetPw = mIni["Network Drive"]["Pw"].GetString();
            //210811 pjh : D/F Server Data Save 기능 Net Drive 추가
            CData.Opt.bDFNetUse = mIni["Network Drive"]["D/F Use"].ToBool();
            //

            // 2020.08.21 JSKim St : Probe Value 표시 Font Size, 소수점 자리수
            CData.Opt.iProbeFontSize = mIni["Display Probe Value"]["ProbeFontSize"].ToInt();
            CData.Opt.iProbeFloationPoint = mIni["Display Probe Value"]["ProbeFloationPoint"].ToInt();
            // 2020.08.21 JSKim Ed

            // 200917 jym
            CData.Opt.bDiChillerSkip = mIni["General Skip"]["DiChiller"].ToBool(true);

            // 201006 jym st
            CData.Opt.iChkDiTime = mIni["Check DI"]["Delay time"].ToInt(3000);
            // 201006 jym ed

            // 201022 jym : 추가
            CData.Opt.aAtoLimit[(int)EWay.L] = mIni["Auto Offset Limit"]["Left limit"].ToDouble(0);
            CData.Opt.aAtoLimit[(int)EWay.R] = mIni["Auto Offset Limit"]["Right limit"].ToDouble(0);
            // 210804 pjh : Tool Setter Gap 변경 최대 값(Tool Setter Gap이 해당 설정값 이상으로 바뀌면 Tool Setter Gap 값 갱신하지 않음)
            CData.Opt.dToolMax = mIni["Auto Offset Limit"]["Max limit"].ToDouble();
            //

            //201023 JSKim : Over Grinding Correction - Grinding Count Correction 기능
            CData.Opt.bOverGrdCountCorrectionUse = mIni["Over Grinding Correction"]["GrindingCountCorrectionUse"].ToBool();
            //
            //201025 jhc : Over Grinding Correction - Grinding Count Correction 기능 제공/감춤 용
            if (!CDataOption.UseGrindingCorrect)
            { CData.Opt.bOverGrdCountCorrectionUse = false; }
            //

            // 2021.02.20 lhs Start
            CData.Opt.bWheelStopWaitSkip = mIni["Probe Measure"]["WheelStopWaitSkip"].ToBool();
            if (CData.CurCompany != ECompany.JCET)
            {
                CData.Opt.bWheelStopWaitSkip = false;
            }
            // 2021.02.20 lhs End

            // 210727 pjh : Wheel 및 Dresser 최대 높이 설정
            CData.Opt.bVarLimitUse = mIni["Wheel/Dresser Maximum Thickness Use"]["Limit Use"].ToBool();

            CData.Opt.aWheelMax[(int)EWay.L] = mIni["Wheel/Dresser Maximum Thickness"]["Left Wheel limit"].ToDouble();
            CData.Opt.aWheelMax[(int)EWay.R] = mIni["Wheel/Dresser Maximum Thickness"]["Right Wheel limit"].ToDouble();
            CData.Opt.aDresserMax[(int)EWay.L] = mIni["Wheel/Dresser Maximum Thickness"]["Left Dresser limit"].ToDouble();
            CData.Opt.aDresserMax[(int)EWay.R] = mIni["Wheel/Dresser Maximum Thickness"]["Right Dresser limit"].ToDouble();

            if (CData.Opt.aWheelMax[(int)EWay.L] <= 0 || CData.Opt.aWheelMax[(int)EWay.L] > GV.WHEEL_DEF_TIP_T)
            {
                CData.Opt.aWheelMax[(int)EWay.L] = GV.WHEEL_DEF_TIP_T;
            }
            if(CData.Opt.aWheelMax[(int)EWay.R] <= 0 || CData.Opt.aWheelMax[(int)EWay.R] > GV.WHEEL_DEF_TIP_T)
            {
                CData.Opt.aWheelMax[(int)EWay.R] = GV.WHEEL_DEF_TIP_T;
            }
            if (CData.Opt.aDresserMax[(int)EWay.L] <= 0 || CData.Opt.aDresserMax[(int)EWay.L] > GV.EQP_DRESSER_BLOCK_HEIGHT)
            {
                CData.Opt.aDresserMax[(int)EWay.L] = GV.EQP_DRESSER_BLOCK_HEIGHT;
            }
            if (CData.Opt.aDresserMax[(int)EWay.R] <= 0 || CData.Opt.aDresserMax[(int)EWay.R] > GV.EQP_DRESSER_BLOCK_HEIGHT)
            {
                CData.Opt.aDresserMax[(int)EWay.R] = GV.EQP_DRESSER_BLOCK_HEIGHT;
            }
            //
            //20190309 ghk_level
            #region Auto
            CData.Opt.iLvManual = mIni["Lv Auto"]["Manual"].ToInt();
            CData.Opt.iLvDevice = mIni["Lv Auto"]["Device"].ToInt(); 
            CData.Opt.iLvWheel=    mIni["Lv Auto"]["Wheel"].ToInt();
            CData.Opt.iLvSpc=      mIni["Lv Auto"]["Spc"].ToInt();
            CData.Opt.iLvOption=   mIni["Lv Auto"]["Option"].ToInt();
            CData.Opt.iLvUtil=     mIni["Lv Auto"]["Util"].ToInt();
            CData.Opt.iLvExit=     mIni["Lv Auto"]["Exit"].ToInt();   
            #endregion

            #region Manual
            CData.Opt.iLvWarmSet = mIni["Lv Manual"]["Warm Set"].ToInt();
            CData.Opt.iLvOnL     = mIni["Lv Manual"]["OnL"     ].ToInt();
            CData.Opt.iLvInr     = mIni["Lv Manual"]["Inr"     ].ToInt();
            CData.Opt.iLvOnp     = mIni["Lv Manual"]["Onp"     ].ToInt();
            CData.Opt.iLvGrL     = mIni["Lv Manual"]["GrL"     ].ToInt();
            CData.Opt.iLvGrd     = mIni["Lv Manual"]["Grd"     ].ToInt();
            CData.Opt.iLvGrr     = mIni["Lv Manual"]["Grr"     ].ToInt();
            CData.Opt.iLvOfp     = mIni["Lv Manual"]["Ofp"     ].ToInt();
            CData.Opt.iLvDry     = mIni["Lv Manual"]["Dry"     ].ToInt();
            CData.Opt.iLvOfL = mIni["Lv Manual"]["OFL"].ToInt();
            CData.Opt.iLvAllSvOn   = mIni["Lv Manual"]["All Servo On" ].ToInt();
            CData.Opt.iLvAllSvOff = mIni["Lv Manual"]["All Servo Off"].ToInt(); 
            #endregion

            #region Device
            CData.Opt.iLvGpNew          =mIni["Lv Device"]["Group New"              ].ToInt();                  
            CData.Opt.iLvGpSaveAs       =mIni["Lv Device"]["Group Save As"          ].ToInt();              
            CData.Opt.iLvGpDel          =mIni["Lv Device"]["Group Delete"           ].ToInt();               
            CData.Opt.iLvDvNew          =mIni["Lv Device"]["Device New"             ].ToInt();                 
            CData.Opt.iLvDvSaveAs       =mIni["Lv Device"]["Device Save As"         ].ToInt();             
            CData.Opt.iLvDvDel          =mIni["Lv Device"]["Device Delete"          ].ToInt();              
            CData.Opt.iLvDvLoad         =mIni["Lv Device"]["Device Load"            ].ToInt();                
            CData.Opt.iLvDvCurrent      =mIni["Lv Device"]["Device Current"         ].ToInt();             
            CData.Opt.iLvDvSave         =mIni["Lv Device"]["Device Save"            ].ToInt();                
            CData.Opt.iLvDvParaEnable   =mIni["Lv Device"]["Device Parameter Enable"].ToInt();
            CData.Opt.iLvDvPosView = mIni["Lv Device"]["Device Position View"].ToInt();       
            #endregion

            #region Wheel
            CData.Opt.iLvWhlNew        =mIni["Lv Wheel"]["Wheel New"            ].ToInt();  
            CData.Opt.iLvWhlSaveAs     =mIni["Lv Wheel"]["Wheel Save As"        ].ToInt();  
            CData.Opt.iLvWhlDel        =mIni["Lv Wheel"]["Wheel Delete"         ].ToInt();  
            CData.Opt.iLvWhlSave       =mIni["Lv Wheel"]["Wheel Save"           ].ToInt();  
            CData.Opt.iLvWhlChange     =mIni["Lv Wheel"]["Wheel Change"         ].ToInt();
            CData.Opt.iLvWhlDrsChange  =mIni["Lv Wheel"]["Wheel DrsChange"      ].ToInt();     
            #endregion

            #region Spc
            CData.Opt.iLvSpcGpSave       =mIni["Lv Spc"]["Spc Graph Save"       ].ToInt(); 
            CData.Opt.iLvSpcErrList      =mIni["Lv Spc"]["Spc Err List"         ].ToInt(); 
            CData.Opt.iLvSpcErrHis       =mIni["Lv Spc"]["Spc Err History"      ].ToInt(); 
            CData.Opt.iLvSpcErrHisView   =mIni["Lv Spc"]["Spc Err History View" ].ToInt();
            CData.Opt.iLvSpcErrHisSave  = mIni["Lv Spc"]["Spc Err HIstory Save" ].ToInt(); 
            #endregion

            #region Option
            CData.Opt.iLvOptSysPos  = mIni["Lv Option"]["Option System Position"].ToInt();
            CData.Opt.iLvOptTbGrd   = mIni["Lv Option"]["Option Table Grinding"].ToInt();
            // 2020.10.09 SungTae Start : Add only Qorvo
            CData.Opt.iLvOptLoader  = mIni["Lv Option"]["Option On/Off Loader"].ToInt();
            CData.Opt.iLvOptRailDry = mIni["Lv Option"]["Option InRail/Dry"].ToInt();
            CData.Opt.iLvOptPicker  = mIni["Lv Option"]["Option On/Off Picker"].ToInt();
            CData.Opt.iLvOptGrind   = mIni["Lv Option"]["Option Left/Right Grind"].ToInt();
            // 2020.10.09 SungTae End
            // 2021.07.15 lhs Start
            CData.Opt.iLvOptGen     = mIni["Lv Option"]["Option General"].ToInt(); 
            CData.Opt.iLvOptMnt     = mIni["Lv Option"]["Option Maintenance"].ToInt(); 
            // 2021.07.15 lhs End
            #endregion

            #region Util
            CData.Opt.iLvUtilMot    =mIni["Lv Util"]["Util Motion"      ].ToInt(); 
            CData.Opt.iLvUtilSpd    =mIni["Lv Util"]["Util Spindle"     ].ToInt(); 
            CData.Opt.iLvUtilIn     =mIni["Lv Util"]["Util Input"       ].ToInt(); 
            CData.Opt.iLvUtilOut    =mIni["Lv Util"]["Util OutPut"      ].ToInt(); 
            CData.Opt.iLvUtilPrb    =mIni["Lv Util"]["Util Probe"       ].ToInt(); 
            CData.Opt.iLvUtilTw     =mIni["Lv Util"]["Util Tower Lamp"  ].ToInt(); 
            CData.Opt.iLvUtilBcr    =mIni["Lv Util"]["Util Barcode"     ].ToInt();
            CData.Opt.iLvUtilRepeat =mIni["Lv Util"]["Util Repeat"      ].ToInt(); 
            #endregion

            #region OpManual
            CData.Opt.iLvOpDrsPos = mIni["Lv OpManual"]["OpManual Dresser Position"].ToInt();
            CData.Opt.iLvOpStripExistEdit = mIni["Lv OpManual"]["OpManual Strip Existence Edit"].ToInt();
            #endregion

            CData.Opt.aTC_Cnt[(int)EWay.L]= mIni["Table Clean Count"]["Left Table" ].ToInt();
            CData.Opt.aTC_Cnt[(int)EWay.R]= mIni["Table Clean Count"]["Right Table"].ToInt();
            CData.Opt.iTC_Cycle = mIni["Table Clean Count"]["Cleaning Cycle"].ToInt();              // 2020-11-16, jhLee 추가


            // 200330 mjy : 추가
            CData.Opt.dPickGap    = mIni["Pick/Place Water Clean"]["Pick Water Gap"   ].ToDouble();
            CData.Opt.iPickDelay  = mIni["Pick/Place Water Clean"]["Pick Water Delay" ].ToInt();
            CData.Opt.dPlaceGap   = mIni["Pick/Place Water Clean"]["Place Water Gap"  ].ToDouble();
            CData.Opt.iPlaceDelay = mIni["Pick/Place Water Clean"]["Place Water Delay"].ToInt();

            // 200619 lks : 추가
            string qcip = mIni["QC Client"][ "QcVisionIp"].GetString();
            if (string.IsNullOrEmpty(qcip)) qcip = "10.0.0.1";
            string qcport = mIni["QC Client"]["QcVisionPort"].GetString();
            if (string.IsNullOrEmpty(qcport)) qcport = "5500";
            CData.Opt.sQcServerIp = qcip;
            CData.Opt.sQcServerPort = qcport;

            // 2020.10.13 SungTae : Add
            CData.Opt.bDrsFiveCheck = mIni["Dresser Five Check"]["DrsFiveCheck"].ToBool();

            // 2020.10.22 JSKim St
            CData.Opt.bDualPumpUse = mIni["Dual Pump Use Check"]["DualPumpUse"].ToBool();
            // 2020.10.22 JSKim Ed

            //201005 pjh : Log Delete Period
            CData.Opt.iDelPeriod = mIni["Log Delete Period"]["Log Delete Period"].ToInt();
            //

            // 2021.04.09 SungTae Start : Wheel History Auto Delete Period
            CData.Opt.iDelPeriodHistory = mIni["Wheel History Auto Delete Period"]["Delete Period"].ToInt();
            // 2021.04.09 SungTae End

            // 2020-11-17, jhLee : Skyworks 일정매수 투입 후 자동으로 Loading stop 되기위한 투입 수량
            CData.Opt.iAutoLoadingStopCount = mIni["LoadingStop"]["Auto Loading Stop Count"].ToInt();
            CData.Opt.iAutoLoadingStopCount = (CData.Opt.iAutoLoadingStopCount <= 1) ? 1 : CData.Opt.iAutoLoadingStopCount;     // 최소/최대 범위 체크
            CData.Opt.iAutoLoadingStopCount = (CData.Opt.iAutoLoadingStopCount > 10) ? 10 : CData.Opt.iAutoLoadingStopCount;

            // 2021.10.25 SungTae Start : [수정] Code 확인 용이하도록 변경 (1 -> (int)eTypeLDStop.Table)
            //syc : ase kr loading stop
            CData.Opt.iLoadingStopType = mIni["LoadingStop"]["Loding Stop Type"].ToInt();
            if (CData.Opt.iLoadingStopType != (int)eTypeLDStop.Table/*1*/ || !CDataOption.UseAutoLoadingStop) //1과0이 아닌값 혹은 옵션 사용이 아니라면 0 //Option 파일 수정으로 인한 1또는 0이아닌 값이 들어왔을경우 0으로 설정
            {
                CData.Opt.iLoadingStopType = (int)eTypeLDStop.Inrail/*0*/;
            }
            // 2021.10.25 SungTae End

            #region Motorize probe measure, 2020-11-03, jhLee
            // 2020-10-26, jhLee : 측정 정확도를 높이기 위해 Probe를 더 눌러주는 양
            CData.Opt.dProbeOD = mIni["Motorize Probe Measure"]["Probe Overdrive"].ToDouble();
            CData.Opt.iProbeStableDelay = mIni["Motorize Probe Measure"]["Probe Stable Delay"].ToInt();   // Probe 안정화 시간

            // 2020-11-03, jhLee : 속도 단축을 위해 사용되는 위치/속도 값
            CData.Opt.dSafetyTopOffset = mIni["Motorize Probe Measure"]["Safety Top Offset"].ToDouble();
            CData.Opt.dZAxisMoveUpSpeed = mIni["Motorize Probe Measure"]["ZAxis MoveUp Speed"].ToDouble();

            // 값의 정합성을 검사하여 범위를 벗어나면 지정 값으로 대체한다.
            if ( CData.Opt.dProbeOD < 1.0 )     // 눌러주는 양 최소값 처리
            {
                CData.Opt.dProbeOD = 1.0;
            }
            else if (CData.Opt.dProbeOD > 4.0)     // 최대값 처리
            {
                CData.Opt.dProbeOD = 4.0;
            }

            if (CData.Opt.iProbeStableDelay < 500)     // 안정화 시간 최소값 처리
            {
                CData.Opt.iProbeStableDelay = 500;
            }
            else if (CData.Opt.iProbeStableDelay > 2000)     // 최대값 처리
            {
                CData.Opt.iProbeStableDelay = 2000;
            }

            // Probe를 Down상태로 안전하게 이동할 수 있는 Table 상단에서의 Offset
            if (CData.Opt.dSafetyTopOffset < 8.0)           // 최소 안전위치 이하라면
            {
                CData.Opt.dSafetyTopOffset = 8.0;
            }
            else if (CData.Opt.dSafetyTopOffset > 20.0)      // 최대 안전위치 이상이라면
            {
                CData.Opt.dSafetyTopOffset = 20.0;
            }

            // 측정 후 Z축을 Up 이동시 이동속도
            if (CData.Opt.dZAxisMoveUpSpeed < 10.0)           // Normal 이동속도 이하라면
            {
                CData.Opt.dZAxisMoveUpSpeed = 10.0;
            }
            else if (CData.Opt.dZAxisMoveUpSpeed > 60.0 )      // 최대 속도 이상 이라면
            {
                CData.Opt.dZAxisMoveUpSpeed = 60;               // 최대 속도로
            }
            //end of 2020-11-03, jhLee
            #endregion // of #region Motorize probe measure, 2020-11-03, jhLee

            #region Multi-LOT
            // 2021.05.17 jhLee : 연속LOT (Multi-LOT) 기능 사용 여부, CDataOption.UseMultiLOT 이 활성화 된 상태에서 사용 가능
            CData.Opt.bMultiLOTUse      = mIni["Multi LOT"]["Multi LOT Use"].ToBool();
            CData.Opt.nLOTEndEmptySlot  = mIni["Multi LOT"]["Empty Slot Count"].ToInt();// 연속 LOT에서 LOT의 구분을 위한 연속된 빈 Slot의 수량 3 ~ 5
			CData.LotMgr.MaxEmptySlot   = CData.Opt.nLOTEndEmptySlot;     // 실제 사용하는 Class의 Max count도 Update한다
            #endregion // of #region Multi-LOT

            // 2021.09.14 Start : 이오나이저 Sol/V Off 설정 (SCK전용)
            CData.Opt.bUseIOZTSolOff        = mIni["Ionizer"]["IOZT SolValve Off Use"].ToBool();
            CData.Opt.nIOZTSolOffDelaySec   = mIni["Ionizer"]["IOZT SolValve Off Delay Sec"].ToInt();
            // 2021.09.14 End


            // 2022.03.22 Start : Dummy Thickness (2004U)
            if (CDataOption.Use2004U)
            {
                CData.Opt.dDummyThick       = mIni["Dummy"]["Dummy Thickness"].ToDouble();
            }
            // 2022.03.22 End

        }
    }
}
