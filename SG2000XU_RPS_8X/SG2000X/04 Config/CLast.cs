using System.IO;
using System.Text;
using System; /// Max 2020103 : SCK+ Rader & Error Text Modify

namespace SG2000X
{
    public class CLast
    {
        public static void Save()
        {
            string sPath = GV.PATH_CONFIG + "LastStatus.cfg";
            StringBuilder mSB = new StringBuilder();

            mSB.AppendLine("[Device File]");
            mSB.AppendLine("Group=" + CDev.It.m_sGrp );
            mSB.AppendLine("Path="  + CData.DevCur   );
            mSB.AppendLine();
            mSB.AppendLine("[Wheel Left]");
            //mIni.Write(sSec, "Path", GV.PATH_WHEEL +"Left\\" + CData.Whls[0].sWhlName + "\\" + CData.Whls[0].sWhlName + ".whl");
            mSB.AppendLine("Path="             + GV.PATH_WHEEL + "Left\\" + CData.Whls[0].sWhlName + "\\" + "WheelInfo.whl");
            mSB.AppendLine("Before Thickness=" + CData.WhlBf[0]);
            mSB.AppendLine("After Thickness="  + CData.WhlAf[0]);
            //201104 jhc : Grinding 시작 높이 계산 시 (휠팁 두께 값) 대신 (휠 측정 시 Z축 높이 값) 이용
            mSB.AppendLine("Before_Z_Axis_Pos=" + CData.WheelTipBeforeZPos[0]); //Wheel 측정 시 Z축 위치(가장 높은 값 = 가장 두꺼운 휠팁)
            mSB.AppendLine("After_Z_Axis_Pos=" + CData.WheelTipAfterZPos[0]); //Wheel 측정 시 Z축 위치(가장 높은 값 = 가장 두꺼운 휠팁)
            //

            mSB.AppendLine();
            mSB.AppendLine("[Wheel Right]");
            //mIni.Write(sSec, "Path", GV.PATH_WHEEL + "Right\\" + CData.Whls[1].sWhlName + "\\" + CData.Whls[1].sWhlName + ".whl");
            mSB.AppendLine("Path="             + GV.PATH_WHEEL + "Right\\" + CData.Whls[1].sWhlName + "\\" + "WheelInfo.whl");
            mSB.AppendLine("Before Thickness=" + CData.WhlBf[1]);
            mSB.AppendLine("After Thickness="  + CData.WhlAf[1]);
            //201104 jhc : Grinding 시작 높이 계산 시 (휠팁 두께 값) 대신 (휠 측정 시 Z축 높이 값) 이용
            mSB.AppendLine("Before_Z_Axis_Pos=" + CData.WheelTipBeforeZPos[1]); //Wheel 측정 시 Z축 위치(가장 높은 값 = 가장 두꺼운 휠팁)
            mSB.AppendLine("After_Z_Axis_Pos=" + CData.WheelTipAfterZPos[1]); //Wheel 측정 시 Z축 위치(가장 높은 값 = 가장 두꺼운 휠팁)
            //
            mSB.AppendLine();
            mSB.AppendLine("[Dresser Left]");
            mSB.AppendLine("Before Thickness=" + CData.DrsBf[0]);
            mSB.AppendLine("After Thickness="  + CData.DrsAf[0]);
            mSB.AppendLine();
            mSB.AppendLine("[Dresser Right]");
            mSB.AppendLine("Before Thickness=" + CData.DrsBf[1]);
            mSB.AppendLine("After Thickness="  + CData.DrsAf[1]);
            mSB.AppendLine();
            //190522 ksg :
            mSB.AppendLine("[Wheel Change]");
            mSB.AppendLine("Complete="    + CData.WhlChgSeq.bStart.ToString());
            mSB.AppendLine("CompleteDrs=" + CData.DrsChgSeq.bStart.ToString());

            //211122 pjh : Dresser 별도 관리 기능 사용 시 Data Save
            if(CDataOption.UseSeperateDresser)
            {
                mSB.AppendLine();
                mSB.AppendLine("[Seperated Dresser Left]");
                mSB.AppendLine("Path=" + GV.PATH_DRESSER + "Left\\" + CData.Drs[0].sDrsName + "\\" + "DresserInfo.txt");
                mSB.AppendLine("Name=" + CData.Drs[0].sDrsName);
                mSB.AppendLine("Outer=" + CData.Drs[0].dDrsOuter.ToString());
                mSB.AppendLine("Height=" + CData.Drs[0].dDrsH.ToString());

                mSB.AppendLine();
                mSB.AppendLine("[Seperated Dresser Right]");
                mSB.AppendLine("Path=" + GV.PATH_DRESSER + "Right\\" + CData.Drs[1].sDrsName + "\\" + "DresserInfo.txt");
                mSB.AppendLine("Name=" + CData.Drs[1].sDrsName);
                mSB.AppendLine("Outer=" + CData.Drs[1].dDrsOuter.ToString());
                mSB.AppendLine("Height=" + CData.Drs[1].dDrsH.ToString());
            }
            //
            // 2020.12.09 JSKim St
            ///// <summary>
            ///// Max 2020103 : SCK+ Rader & Error Text Modify
            ///// </summary>            
            //string Temp,sSec;
            //int nHour = 0;
            //int nMin = 0;
            //int nSec = 0;            

            //DateTime tmpDateTime;
            //CData.SwTotalRunTim_MC.Stop();
            //CData.SwTotalStopTim_MC.Stop();
            //CData.SwTotalJamTim_MC.Stop();

            //mSB.AppendLine("[SPC Data]");
            //Temp = CData.SwTotalRunTim_MC.Elapsed.ToString(@"hh\:mm\:ss");
            //tmpDateTime = Convert.ToDateTime(Temp);
            //nHour = tmpDateTime.Hour + CData.dtTotalRunTim_MC.Hour;
            //nMin = tmpDateTime.Minute + CData.dtTotalRunTim_MC.Minute;
            //if (nMin >= 60) { nHour++; nMin = nMin - 60; }
            //nSec = tmpDateTime.Second + CData.dtTotalRunTim_MC.Second;
            //if (nSec >= 60) { nMin++; nSec = nSec - 60; }
            //Temp = string.Format("{0:00}:{1:00}:{2:00}", nHour, nMin, nSec);
            //mSB.AppendLine("Total RUN Time=" + Temp);

            //Temp = CData.SwTotalStopTim_MC.Elapsed.ToString(@"hh\:mm\:ss");
            //tmpDateTime = Convert.ToDateTime(Temp);
            //nHour = tmpDateTime.Hour + CData.dtTotalStopTim_MC.Hour;
            //nMin = tmpDateTime.Minute + CData.dtTotalStopTim_MC.Minute;
            //if (nMin >= 60) { nHour++; nMin = nMin - 60; }
            //nSec = tmpDateTime.Second + CData.dtTotalStopTim_MC.Second;
            //if (nSec >= 60) { nMin++; nSec = nSec - 60; }
            //Temp = string.Format("{0:00}:{1:00}:{2:00}", nHour, nMin, nSec);
            //mSB.AppendLine("Total STOP Time=" + Temp);

            //Temp = CData.SwTotalJamTim_MC.Elapsed.ToString(@"hh\:mm\:ss");
            //tmpDateTime = Convert.ToDateTime(Temp);
            //nHour = tmpDateTime.Hour + CData.dtTotalJamTim_MC.Hour;
            //nMin = tmpDateTime.Minute + CData.dtTotalJamTim_MC.Minute;
            //if (nMin >= 60) { nHour++; nMin = nMin - 60; }
            //nSec = tmpDateTime.Second + CData.dtTotalJamTim_MC.Second;
            //if (nSec >= 60) { nMin++; nSec = nSec - 60; }
            //Temp = string.Format("{0:00}:{1:00}:{2:00}", nHour, nMin, nSec);
            //mSB.AppendLine("Total JAM Time=" + Temp);
            // 2020.12.09 JSKim Ed

            CLog.Check_File_Access(sPath, mSB.ToString(), false);

            SaveStatus(); //191104 ksg :
        }

        public static void Load()
        {
            string sSec = "";
            string sVal = "";
            string sPath = GV.PATH_CONFIG + "LastStatus.cfg";

            if (File.Exists(sPath) == false)
            {
                Save();
                return;
            }

			if ((CData.Opt.bSecsUse == true) && (CData.GemForm != null)) // 191125 ksg 
			{
				CData.GemForm.LastLoad();
			}

			CIni mIni = new CIni(sPath);
            sSec = "Device File";
            CDev.It.m_sGrp = mIni.Read(sSec, "Group");
            sVal = mIni.Read(sSec, "Path");
            CData.DevCur = sVal;
            CData.DevGr  = CDev.It.m_sGrp; //20200619 jhc : 그룹명 변수 값이 바뀌어, 디바이스 파일 저장 폴더가 바뀌는 현상 개선
            if (sVal != "") 
            {
                // 마지막 디바이스 파일 실제 존재 유무 판단
                // 없으면 디바이스 파일 로딩 안함 
                // maeng-190104
                if (File.Exists(sVal))  { CDev.It.Load(sVal, true); }
                else                    { sVal = "";                }
            }

            sSec = "Wheel Left";
            sVal = mIni.Read(sSec, "Path");
            if (sVal != "")
            {
                // 마지막 휠 파일 실제 존재 유무 판단
                // 없으면 휠 파일 로딩 안함 
                // maeng-190104
                //if (File.Exists(sVal))  {   CWhl.It.Load(sVal, out CData.Whls[0]);  }
                //else                    {   sVal = "";                              }
                if (File.Exists(sVal))
                {
                    CWhl.It.Load(sVal, out CData.Whls[0]);
                    //211011 pjh : Device에 Wheel 정보 관리하는 기능 License로 관리
                    if (CDataOption.UseDeviceWheel) { CWhl.It.Load(sVal, out CDev.It.a_tWhl[0]); }
                }
                else { sVal = ""; }
            }
            CData.WhlAf[0] = mIni.ReadD(sSec, "After Thickness" );
            CData.WhlBf[0] = mIni.ReadD(sSec, "Before Thickness");
            //201104 jhc : Grinding 시작 높이 계산 시 (휠팁 두께 값) 대신 (휠 측정 시 Z축 높이 값) 이용
            CData.WheelTipBeforeZPos[0] = mIni.ReadD(sSec, "Before_Z_Axis_Pos");    //Wheel 측정 시 Z축 위치
            if (CData.WheelTipBeforeZPos[0] < 1)    { CData.WheelTipBeforeZPos[0] = Math.Round((CData.MPos[0].dZ_WHL_BASE - CData.WhlBf[0]), 6); } //Last Data 값이 비정상적인 경우 또는 S/W 신규 적용 시 => (휠베이스 - 휠두께) 값으로 계산
            CData.WheelTipAfterZPos[0]  = mIni.ReadD(sSec, "After_Z_Axis_Pos");     //Wheel 측정 시 Z축 위치
            if (CData.WheelTipAfterZPos[0]  < 1)    { CData.WheelTipAfterZPos[0]  = Math.Round((CData.MPos[0].dZ_WHL_BASE - CData.WhlAf[0]), 6); } //Last Data 값이 비정상적인 경우 또는 S/W 신규 적용 시 => (휠베이스 - 휠두께) 값으로 계산
            //

            sSec = "Wheel Right";
            sVal = mIni.Read(sSec, "Path");

            if (sVal != "")
            {
                // 마지막 휠 파일 실제 존재 유무 판단
                // 없으면 휠 파일 로딩 안함 
                // maeng-190104
                if (File.Exists(sVal))
                {
                    CWhl.It.Load(sVal, out CData.Whls[1]);
                    //211011 pjh : Device에 Wheel 정보 관리하는 기능 License로 관리
                    if (CDataOption.UseDeviceWheel) { CWhl.It.Load(sVal, out CDev.It.a_tWhl[1]); }

                    if ((CData.GemForm != null) && (CData.Opt.bSecsUse))
                    {
                        //CEID 999701
                        CData.GemForm.ToolSerial_Number_Set(CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Serial_Num[0],    (uint)SECSGEM.JSCK.eTableLoc.N1);
                        CData.GemForm.OnToolVerifyRequest(  CData.LotInfo.sLToolId,                             (uint)SECSGEM.JSCK.eTableLoc.N1); // 2021.06.10 lhs sRToolId->sLToolId 오타수정.

                        CData.GemForm.ToolSerial_Number_Set(CData.JSCK_Gem_Data[(int)EDataShift.EQ_READY/*0*/].sCurr_Tool_Serial_Num[1],    (uint)SECSGEM.JSCK.eTableLoc.N2);
                        CData.GemForm.OnToolVerifyRequest(  CData.LotInfo.sRToolId,                             (uint)SECSGEM.JSCK.eTableLoc.N2);
                        
                        //CEID 999601
                        CData.GemForm.OnMatVerifyRequest(CData.LotInfo.sLDrsId, (uint)SECSGEM.JSCK.eTableLoc.N1);   // 2021.06.10 lhs Left 추가
                        CData.GemForm.OnMatVerifyRequest(CData.LotInfo.sRDrsId, (uint)SECSGEM.JSCK.eTableLoc.N2);
                    }
                }
                else
                { sVal = ""; }
            }
            CData.WhlAf[1] = mIni.ReadD(sSec, "After Thickness" );
            CData.WhlBf[1] = mIni.ReadD(sSec, "Before Thickness");
            //201104 jhc : Grinding 시작 높이 계산 시 (휠팁 두께 값) 대신 (휠 측정 시 Z축 높이 값) 이용
            CData.WheelTipBeforeZPos[1] = mIni.ReadD(sSec, "Before_Z_Axis_Pos"); //Wheel 측정 시 Z축 위치
            if (CData.WheelTipBeforeZPos[1] < 1)    { CData.WheelTipBeforeZPos[1] = Math.Round((CData.MPos[1].dZ_WHL_BASE - CData.WhlBf[1]), 6); } //Last Data 값이 비정상적인 경우 또는 S/W 신규 적용 시 => (휠베이스 - 휠두께) 값으로 계산
            CData.WheelTipAfterZPos[1]  = mIni.ReadD(sSec, "After_Z_Axis_Pos");  //Wheel 측정 시 Z축 위치
            if (CData.WheelTipAfterZPos[1]  < 1)    { CData.WheelTipAfterZPos[1]  = Math.Round((CData.MPos[1].dZ_WHL_BASE - CData.WhlAf[1]), 6); } //Last Data 값이 비정상적인 경우 또는 S/W 신규 적용 시 => (휠베이스 - 휠두께) 값으로 계산
            //

            sSec = "Dresser Left";
            CData.DrsAf[0] = mIni.ReadD(sSec, "After Thickness" );
            CData.DrsBf[0] = mIni.ReadD(sSec, "Before Thickness");

            sSec = "Dresser Right";
            CData.DrsAf[1] = mIni.ReadD(sSec, "After Thickness" );
            CData.DrsBf[1] = mIni.ReadD(sSec, "Before Thickness");

            //190522 ksg :
            sSec = "Wheel Change";
            bool.TryParse(mIni.Read(sSec, "Complete"   ), out CData.WhlChgSeq.bStart); 
            bool.TryParse(mIni.Read(sSec, "CompleteDrs"), out CData.DrsChgSeq.bStart);

            //211122 pjh : Dresser 별도 관리 기능 사용 시 Data Load
            if (CDataOption.UseSeperateDresser)
            {
                sSec = "Seperated Dresser Left";
                sVal = mIni.Read(sSec, "Path");
                if(sVal != "")
                {
                    CWhl.It.LoadDrs(sVal, out CData.Drs[0]);
                    CData.WhlsLog[0].sDrsName = CData.DrsLog[0].sDrsName = CData.Drs[0].sDrsName; //220106 pjh : History에 사용 될 Dresser Name 저장
                }

                sSec = "Seperated Dresser Right";
                sVal = mIni.Read(sSec, "Path");
                if (sVal != "")
                {
                    CWhl.It.LoadDrs(sVal, out CData.Drs[1]);
                    CData.WhlsLog[1].sDrsName = CData.DrsLog[0].sDrsName = CData.Drs[1].sDrsName; //220106 pjh : History에 사용 될 Dresser Name 저장
                }

            }
            //
            //191023 ksg :
            sSec = "SECSGEM ID";
            CData.LotInfo.sLDrsId = mIni.Read(sSec, "LDrsID");
            CData.LotInfo.sRDrsId = mIni.Read(sSec, "RDrsID");
            CData.LotInfo.sLToolId= mIni.Read(sSec, "LWhlID");
            CData.LotInfo.sRToolId= mIni.Read(sSec, "RWhlID");

            CData.LotInfo.sGMgzId = mIni.Read(sSec, "GMgzID");
            CData.LotInfo.sNMgzId = mIni.Read(sSec, "NMgzID");

			//// 2021.07.23 lhs Start : SCK, JSCK 파일삭제 후 LoadStatus() 추후 검증 후 적용
			//if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
			//{
			//	string sFile = GV.PATH_CONFIG + "LastStatusFile.cfg";
			//	File.Delete(sFile);
			//}
			//// 2021.07.23 lhs End

			LoadStatus(); //191104 ksg :
            //
            if(CDataOption.UseDeviceWheel) CDev.It.Load(CData.DevCur);

		}
		//191104 ksg :
		public static void SaveStatus()
        {
            int iMax;
            string sPath = GV.PATH_CONFIG + "LastStatusFile.cfg";
            StringBuilder mSB = new StringBuilder();

            mSB.AppendLine("[Lot Info]");
            mSB.AppendLine("Lot Name="          + CData.LotInfo.sLotName);
            mSB.AppendLine("Tool Id="           + CData.LotInfo.sToolId);
            mSB.AppendLine("TotalMgz="          + CData.LotInfo.iTotalMgz);
            mSB.AppendLine("Total Strip="       + CData.LotInfo.iTotalStrip);
            mSB.AppendLine("Lot Open="          + CData.LotInfo.bLotOpen.ToString());
            mSB.AppendLine("Lot End="           + CData.LotInfo.bLotEnd.ToString());
            mSB.AppendLine("TotalInCnt="        + CData.LotInfo.iTInCnt);
            mSB.AppendLine("TotalOutCnt="       + CData.LotInfo.iTOutCnt);
            mSB.AppendLine("Left Tool Id="      + CData.LotInfo.sLToolId);
            mSB.AppendLine("Right Tool Id="     + CData.LotInfo.sRToolId);
            mSB.AppendLine("Left Drs Id="       + CData.LotInfo.sLDrsId);
            mSB.AppendLine("Right Drs Id="      + CData.LotInfo.sRDrsId);

            mSB.AppendLine("LWhl_Serial_Num="   + CData.LotInfo.sLTool_Serial_Num);
            mSB.AppendLine("RWhl_Serial_Num="   + CData.LotInfo.sRTool_Serial_Num);

            //mIni.Write(sSec, "Good Mgz Id"  , CData.LotInfo.sGMgzId            );
            //mIni.Write(sSec, "NG Mgz Id"    , CData.LotInfo.sNMgzId            );

            //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
            mSB.AppendLine("Lot 18P Measured Num="  + CData.LotInfo.i18PMeasuredCount ); //18 Point 측정 완료(또는 진행중) 스트립 수량
            //

            if (CData.CurCompany == ECompany.SkyWorks)  iMax = (int)EPart.OFR;
            else                                        iMax = (int)EPart.OFL;

            for(int i = 0; i <= iMax; i++)
            {
                mSB.AppendLine("Part Status_"  + i.ToString() + "=" + (int)CData.Parts[i].iStat   );
                mSB.AppendLine("Part LotName_" + i.ToString() + "=" +      CData.Parts[i].sLotName);
                mSB.AppendLine("Part MgzNum_"  + i.ToString() + "=" +      CData.Parts[i].iMGZ_No );
                mSB.AppendLine("Part MgzId_"   + i.ToString() + "=" +      CData.Parts[i].sMGZ_ID );
                mSB.AppendLine("Part SlotNum_" + i.ToString() + "=" +      CData.Parts[i].iSlot_No);

				//200710 jhc : DEBUG - 매거진 슬롯 수 증가시키고 -> Device Save하면 -> Last Status 저장 시 매거진 슬롯 배열 수 overflow 되어 Exception 발생 
				//for(int j = 0; j < CData.Dev.iMgzCnt; j++)
				for (int j = 0; j < CData.Parts[i].iSlot_info.Length; j++)
					mSB.AppendLine("Part SlotInfo_" + i.ToString() + "=" + CData.Parts[i].iSlot_info[j]);

				mSB.AppendLine("Part Bcr_"          + i.ToString() + "=" + CData.Parts[i].sBcr);
				mSB.AppendLine("Part BcrStatus_"    + i.ToString() + "=" + CData.Parts[i].bBcr          .ToString());
                mSB.AppendLine("Part Ori_"          + i.ToString() + "=" + CData.Parts[i].bOri          .ToString());
                mSB.AppendLine("Part StripStatus_"  + i.ToString() + "=" + CData.Parts[i].bExistStrip   .ToString());
                mSB.AppendLine("Part LotMsgShow_"   + i.ToString() + "=" + CData.bPreLotEndMsgShow      .ToString()); //koo : Qorvo Lot Rework
                //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                mSB.AppendLine("Part 18PMeasure_" + i.ToString() + "=" + CData.Parts[i].b18PMeasure.ToString());    // Lot Open 후 첫 n장 여부
                mSB.AppendLine("Part LoadMgzSN_"  + i.ToString() + "=" + CData.Parts[i].nLoadMgzSN.ToString());     // for Multi-LOT

                //
            }
            string Temp;
            Temp = CData.SwIdle.Elapsed.ToString(@"hh\:mm\:ss");    mSB.AppendLine("Lot SwIdle="    + Temp); 
            Temp = CData.SwErr.Elapsed.ToString(@"hh\:mm\:ss");     mSB.AppendLine("Lot SwError="   + Temp);
            Temp = CData.SwRun.Elapsed.ToString(@"hh\:mm\:ss");     mSB.AppendLine("Lot SwRun="     + Temp); 

            CLog.Check_File_Access(sPath, mSB.ToString(), false);
        }

        // 2021-05-25, jhLee : Multi-LOT 입력/관리중인 LOT 정보 저장
        public static void SaveMultiLotInfo()
        {
            string sPath = GV.PATH_CONFIG + "MultiLotInfo.cfg";
            StringBuilder mSB = new StringBuilder();

            int nCount = CData.LotMgr.GetCount();               // 등록된 LOT 정보 수량

            mSB.AppendLine("[Total]");
            mSB.AppendLine($"LotCount = {nCount}");             // 수량

            TLotInfo rInfo;

            for (int i = 0; i < nCount; i++)
            {
                rInfo = CData.LotMgr.GetLotInfo(i);             // LOT 정보를 조회한다.

                mSB.AppendLine($"[LotInfo_{i}]");
                mSB.AppendLine($"Lot Name = {rInfo.sLotName}");
                mSB.AppendLine($"Tool Id = {rInfo.sToolId}");
                mSB.AppendLine($"TotalMgz = {rInfo.iTotalMgz}");
                mSB.AppendLine($"Total Strip = {rInfo.iTotalStrip}");
                mSB.AppendLine($"Lot Open = {rInfo.bLotOpen.ToString()}");
                mSB.AppendLine($"Lot End = {rInfo.bLotEnd.ToString()}");
                mSB.AppendLine($"TotalInCnt = {rInfo.iTInCnt}");
                mSB.AppendLine($"TotalOutCnt = {rInfo.iTOutCnt}");
                mSB.AppendLine($"Left Tool Id = {rInfo.sLToolId}");
                mSB.AppendLine($"Right Tool Id = {rInfo.sRToolId}");
                mSB.AppendLine($"Left Drs Id = {rInfo.sLDrsId}");
                mSB.AppendLine($"Right Drs Id = {rInfo.sRDrsId}");

                mSB.AppendLine($"LWhl_Serial_Num = {rInfo.sLTool_Serial_Num}");
                mSB.AppendLine($"RWhl_Serial_Num = {rInfo.sRTool_Serial_Num}");

                mSB.AppendLine($"Lot 18P Measured Num = {rInfo.i18PMeasuredCount}"); //18 Point 측정 완료(또는 진행중) 스트립 수량

                mSB.AppendLine($"Good Mgz Id = {rInfo.sGMgzId}");
                mSB.AppendLine($"NG Mgz Id = {rInfo.sNMgzId}");
            }

            CLog.Check_File_Access(sPath, mSB.ToString(), false);           // File로 저장
        }


        // 2021-05-25, jhLee : Multi-LOT 입력/관리용으로 저장된 LOT 정보 읽어오기 
        public static void LoadMultiLotInfo()
        {
            string sPath = GV.PATH_CONFIG + "MultiLotInfo.cfg";

            int nCount = 0;
            string sSec = "";
            CIni mIni = new CIni(sPath);
            TLotInfo rInfo;

            CData.LotMgr.Clear();               // 기존 내용은 삭제한다.

            int.TryParse(mIni.Read("Total", "LotCount"), out nCount);   // 등록된 LOT 정보 수량

            for (int i = 0; i < nCount; i++)
            {
                rInfo = new TLotInfo();                      // LOT 정보를 생성 한다.

                sSec = $"[LotInfo_{i}]";                    // section name

                rInfo.sLotName      = mIni.Read(sSec, "Lot Name");
                rInfo.sToolId       = mIni.Read(sSec, "Tool Id");
                rInfo.iTotalMgz     = mIni.ReadI(sSec, "TotalMgz");
                rInfo.iTotalStrip   = mIni.ReadI(sSec, "Total Strip");

                bool.TryParse(mIni.Read(sSec, "Lot Open"), out rInfo.bLotOpen);
                bool.TryParse(mIni.Read(sSec, "Lot End"), out rInfo.bLotEnd);

                rInfo.iTInCnt       = mIni.ReadI(sSec, "TotalInCnt");
                rInfo.iTOutCnt      = mIni.ReadI(sSec, "TotalOutCnt");

                int.TryParse(mIni.Read(sSec, "Lot 18P Measured Num"), out rInfo.i18PMeasuredCount);

                rInfo.sGMgzId       = mIni.Read(sSec, "Good Mgz Id");
                rInfo.sNMgzId       = mIni.Read(sSec, "NG Mgz Id");

                CData.LotMgr.AddLotInfo(ref rInfo);                 // 읽어들어 조립된 Lot Info 정보를 List에 추가한다.
            }

        }

        //191104 ksg :
        public static void LoadStatus()
        {
            int iMax;
            string sSec = "";
            string sPath = GV.PATH_CONFIG + "LastStatusFile.cfg";
            CIni mIni = new CIni(sPath);
            sSec = "Lot Info";
            bool.TryParse(mIni.Read(sSec, "Lot Open"), out CData.LotInfo.bLotOpen);

            if(CData.LotInfo.bLotOpen)
            { 
                CData.LotInfo.sLotName   = mIni.Read (sSec, "Lot Name"   );
                CData.LotInfo.sToolId    = mIni.Read (sSec, "Tool Id"    );
                CData.LotInfo.iTotalMgz  = mIni.ReadI(sSec, "TotalMgz"   );
                CData.LotInfo.iTotalStrip= mIni.ReadI(sSec, "Total Strip");
                bool.TryParse(mIni.Read(sSec, "Lot End"), out CData.LotInfo.bLotEnd );
				//bool.TryParse(mIni.Read(sSec, "Lot Open"), out CData.LotInfo.bLotEnd ); 어느것이 진짜?
                CData.LotInfo.iTInCnt    = mIni.ReadI(sSec, "TotalInCnt" );
                CData.LotInfo.iTOutCnt   = mIni.ReadI(sSec, "TotalOutCnt");

                //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                int.TryParse(mIni.Read(sSec, "Lot 18P Measured Num"), out CData.LotInfo.i18PMeasuredCount ); //18 Point 측정 완료(또는 진행중) 스트립 수량
                //

                if(CData.CurCompany == ECompany.SkyWorks) iMax = (int)EPart.OFR;
                else                                      iMax = (int)EPart.OFL;

                for(int i = 0; i <= iMax; i++)
                {
                    CData.Parts[i].iStat    = (ESeq)mIni.ReadI(sSec, "Part Status_" + i.ToString());
                    mIni.Write(sSec, "Part LotName_" + i.ToString(),      CData.Parts[i].sLotName);
                    CData.Parts[i].iMGZ_No  = mIni.ReadI(sSec, "Part MgzNum" + i.ToString());
                    mIni.Write(sSec, "Part MgzId_" + i.ToString()  ,      CData.Parts[i].sMGZ_ID );
                    CData.Parts[i].iSlot_No = mIni.ReadI(sSec, "Part SlotNum_" + i.ToString());
                    for(int j = 0; j < CData.Dev.iMgzCnt; j++) CData.Parts[i].iSlot_info[j] = mIni.ReadI(sSec, "Part SlotInfo_" + i.ToString());
                    CData.Parts[i].sBcr = mIni.Read(sSec, "Part Bcr_" + i.ToString());
                    bool.TryParse(mIni.Read(sSec, "Part BcrStatus_"   + i.ToString()), out CData.Parts[i].bBcr       );
                    bool.TryParse(mIni.Read(sSec, "Part Ori_"         + i.ToString()), out CData.Parts[i].bOri       );
                    bool.TryParse(mIni.Read(sSec, "Part StripStatus_" + i.ToString()), out CData.Parts[i].bExistStrip);
                    bool.TryParse(mIni.Read(sSec, "Part LotMsgShow_"  + i.ToString()), out CData.bPreLotEndMsgShow   );
                    //200712 jhc : 18 포인트 측정 (ASE-KR VOC)
                    bool.TryParse(mIni.Read(sSec, "Part 18PMeasure_" + i.ToString()), out CData.Parts[i].b18PMeasure); //Lot Open 후 첫 n장 여부

                    CData.Parts[i].LotColor = System.Drawing.Color.Lime;
                    int.TryParse(mIni.Read(sSec, "Part LoadMgzSN_" + i.ToString()), out CData.Parts[i].nLoadMgzSN);
                }
                /* 
                //나중에 반드시 해야 됨     
                TimeSpan Date;
                string   Temp;
                Temp = mIni.Read(sSec, "Lot SwIdle" ); 
                //Date = Temp.
                CData.SwIdle.ElapsedTicks = sTime;
                CData.SwErr  = mIni.Read(sSec, "Lot SwError"); 
                CData.SwRun  = mIni.Read(sSec, "Lot SwRun"  ); 
                */
                CData.bSecLotOpen = true;
            }
            //Lot과 상관 없이 Loading 되어야 함
            CData.LotInfo.sLToolId = mIni.Read(sSec, "Left Tool Id" );
            CData.LotInfo.sRToolId = mIni.Read(sSec, "Right Tool Id");
            CData.LotInfo.sLDrsId  = mIni.Read(sSec, "Left Drs Id"  );
            CData.LotInfo.sRDrsId  = mIni.Read(sSec, "Right Drs Id" );
            CData.LotInfo.sLTool_Serial_Num = mIni.Read(sSec, "LWhl_Serial_Num");
            CData.LotInfo.sRTool_Serial_Num = mIni.Read(sSec, "RWhl_Serial_Num");

            CData.LotInfo.sGMgzId  = mIni.Read(sSec, "Good Mgz Id"  );
            CData.LotInfo.sNMgzId  = mIni.Read(sSec, "NG Mgz Id"    );
        }
    }
}
