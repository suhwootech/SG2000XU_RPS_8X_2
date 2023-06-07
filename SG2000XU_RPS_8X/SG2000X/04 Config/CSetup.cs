using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SG2000X
{
    /// <summary>
    /// 메인 포지션
    /// 장비 셋업시 사용되는 포지션의 데이터 관리 클래스
    /// </summary>
    public class CSetup : CStn<CSetup>
    {
        private CSetup()
        {
            // Config 폴더 없을 시 폴더 생성
            if (Directory.Exists(GV.PATH_CONFIG) == false)
            { Directory.CreateDirectory(GV.PATH_CONFIG); }
        }

        public int Save()
        {
            int iRet = 0;            
            string sPath = GV.PATH_CONFIG + "SetupPosition.cfg";
            StringBuilder mSB = new StringBuilder();

            mSB.AppendLine("[Main Position Left - Manual Position]");
            mSB.AppendLine("Z Axis Table Base="                 + CData.MPos[0].dZ_TBL_BASE       );
            mSB.AppendLine("Z Axis Wheel Base="                 + CData.MPos[0].dZ_WHL_BASE       );
            mSB.AppendLine("Z Axis Dresser Base="               + CData.MPos[0].dZ_DRS_BASE       );
            mSB.AppendLine("Z Axis Probe Measure Tool Setter="  + CData.MPos[0].dZ_PRB_TOOL_SETTER);
            mSB.AppendLine("Y Axis Probe Measure Table Center=" + CData.MPos[0].dY_PRB_TBL_CENTER );
            mSB.AppendLine("Y Axis Wheel Center Table Center="  + CData.MPos[0].dY_WHL_TBL_CENTER );
            mSB.AppendLine("Tool Setter Gap="                   + CData.MPos[0].dTOOL_SETTER_GAP  );
            mSB.AppendLine("PCB Thickness="                     + CData.MPos[0].dPCBThickness     ); //191031 ksg :
            mSB.AppendLine();
            mSB.AppendLine("[Main Position Left - Calculate Position]");
            mSB.AppendLine("Z Axis Table Measure Start Position="   + CData.MPos[0].dZ_TBL_MEA_POS    );
            mSB.AppendLine("Z Axis Dresser Measure Start Position=" + CData.MPos[0].dZ_DRS_MEA_POS    );
            mSB.AppendLine("Y Axis Wheel Center Tool Setter="       + CData.MPos[0].dY_WHL_TOOL_SETTER);
            mSB.AppendLine("Y Axis Wheel Center Dresser Center="    + CData.MPos[0].dY_WHL_DRS        );
            mSB.AppendLine("Y Axis Probe Dresser Center="           + CData.MPos[0].dY_PRB_DRS        );
            mSB.AppendLine("Probe To Wheel Base="                   + CData.MPos[0].dPRB_TO_WHL_BASE  );
            mSB.AppendLine();
            mSB.AppendLine("[Main Position Left - Offset]");
            mSB.AppendLine("After Offset=" + CData.MPos[0].fake);
            mSB.AppendLine();

            mSB.AppendLine("[Main Position Right - Manual Position]");
            mSB.AppendLine("Z Axis Table Base="                 + CData.MPos[1].dZ_TBL_BASE       );
            mSB.AppendLine("Z Axis Wheel Base="                 + CData.MPos[1].dZ_WHL_BASE       );
            mSB.AppendLine("Z Axis Dresser Base="               + CData.MPos[1].dZ_DRS_BASE       );
            mSB.AppendLine("Z Axis Probe Measure Tool Setter="  + CData.MPos[1].dZ_PRB_TOOL_SETTER);
            mSB.AppendLine("Y Axis Probe Measure Table Center=" + CData.MPos[1].dY_PRB_TBL_CENTER );
            mSB.AppendLine("Y Axis Wheel Center Table Center="  + CData.MPos[1].dY_WHL_TBL_CENTER );
            mSB.AppendLine("Tool Setter Gap="                   + CData.MPos[1].dTOOL_SETTER_GAP  );
            mSB.AppendLine();
            mSB.AppendLine("[Main Position Right - Calculate Position]");
            mSB.AppendLine("Z Axis Table Measure Start Position="   + CData.MPos[1].dZ_TBL_MEA_POS    );
            mSB.AppendLine("Z Axis Dresser Measure Start Position=" + CData.MPos[1].dZ_DRS_MEA_POS    );
            mSB.AppendLine("Y Axis Wheel Center Tool Setter="       + CData.MPos[1].dY_WHL_TOOL_SETTER);
            mSB.AppendLine("Y Axis Wheel Center Dresser Center="    + CData.MPos[1].dY_WHL_DRS        );
            mSB.AppendLine("Y Axis Probe Dresser Center="           + CData.MPos[1].dY_PRB_DRS        );
            mSB.AppendLine("Probe To Wheel Base="                   + CData.MPos[1].dPRB_TO_WHL_BASE  );
            mSB.AppendLine();
            mSB.AppendLine("[Main Position Right - Offset]");
            mSB.AppendLine("After Offset=" + CData.MPos[1].fake);
            mSB.AppendLine();
            mSB.AppendLine("[System Position - On Loader X]");
            mSB.AppendLine("Wait Position="    + CData.SPos.dONL_X_Wait    );
            mSB.AppendLine("Default Position=" + CData.SPos.dONL_X_DefAlign);
            mSB.AppendLine();
            mSB.AppendLine("[System Position - On Loader Y]");
            mSB.AppendLine("Pick Position="  + CData.SPos.dONL_Y_Pick );
            mSB.AppendLine("Place Position=" + CData.SPos.dONL_Y_Place);
            mSB.AppendLine();
            mSB.AppendLine("[System Position - On Loader Z]");
            mSB.AppendLine("Wait Position="       + CData.SPos.dONL_Z_Wait   );
            mSB.AppendLine("Pick Go Position="    + CData.SPos.dONL_Z_PickGo );
            mSB.AppendLine("Clamp Position="      + CData.SPos.dONL_Z_Clamp  );
            mSB.AppendLine("Pick Up Position="    + CData.SPos.dONL_Z_PickUp );
            mSB.AppendLine("Place Go Position="   + CData.SPos.dONL_Z_PlaceGo);
            mSB.AppendLine("Unclamp Position="    + CData.SPos.dONL_Z_UnClamp);
            mSB.AppendLine("Place Down Position=" + CData.SPos.dONL_Z_PlaceDn);
            mSB.AppendLine();
            mSB.AppendLine("[System Position - Inrail X]");
            mSB.AppendLine("Wait Position=" + CData.SPos.dINR_X_Wait);
            mSB.AppendLine("Sensor Check Position=" + CData.SPos.dINR_X_ChkUnit);
            mSB.AppendLine();
            mSB.AppendLine("[System Position - Inrail Y]");
            mSB.AppendLine("Wait Position="  + CData.SPos.dINR_Y_Wait);
            mSB.AppendLine("Align Position=" + CData.SPos.dINR_Y_Align);
            mSB.AppendLine();
            mSB.AppendLine("[System Position - On Loader Picker X]");
            mSB.AppendLine("Place Left Position="  + CData.SPos.dONP_X_PlaceL);
            mSB.AppendLine("Place Right Position=" + CData.SPos.dONP_X_PlaceR);
            mSB.AppendLine("Conversion Position="  + CData.SPos.dONP_X_Con   ); //190321 ksg :
            mSB.AppendLine("Pick Left Wait Position="  + CData.SPos.dONP_X_WaitPickL); //20200406 jhc : OnPicker L-Table Pickup 대기 위치
            mSB.AppendLine();
            mSB.AppendLine("[System Position - On Loader Picker Z]");
            mSB.AppendLine("Wait Position=" + CData.SPos.dONP_Z_Wait);
            mSB.AppendLine();
            //20190604 ghk_onpbcr
            mSB.AppendLine("[System Position - On Loader Picker Y]");
            mSB.AppendLine("Wait Position=" + CData.SPos.dONP_Y_Wait);
            mSB.AppendLine();
            mSB.AppendLine("[System Position - Grind Left X]");
            mSB.AppendLine("Wait Position=" + CData.SPos.dGRD_X_Wait[0]);
            mSB.AppendLine("Zero Position=" + CData.SPos.dGRD_X_Zero[0]);
            mSB.AppendLine();
            mSB.AppendLine("[System Position - Grind Left Y]");
            mSB.AppendLine("Wait Position=" + CData.SPos.dGRD_Y_Wait[0]);
            mSB.AppendLine("Cleaning Start Position="    + CData.SPos.dGRD_Y_ClnStart [0]);
            mSB.AppendLine("Cleaning End Position="      + CData.SPos.dGRD_Y_ClnEnd   [0]);
            mSB.AppendLine("Dressing Start Position="    + CData.SPos.dGRD_Y_DrsStart [0]);
            mSB.AppendLine("Dressing End Position="      + CData.SPos.dGRD_Y_DrsEnd   [0]);
            mSB.AppendLine("Table Inspection Position="  + CData.SPos.dGRD_Y_TblInsp  [0]);
            mSB.AppendLine("Dresser Change Position="    + CData.SPos.dGRD_Y_DrsChange[0]);
            // 2021.04.13 SungTae Start : Table Grinding 시 조작 실수 방지 위해 추가
            mSB.AppendLine("Table Grind Start Position=" + CData.SPos.dGRD_Y_TblGrdSt [0]);
            mSB.AppendLine("Table Grind End Position="   + CData.SPos.dGRD_Y_TblGrdEd [0]);
            // 2021.04.13 SungTae End
            mSB.AppendLine();

            mSB.AppendLine("[System Position - Grind Left Z]");
            mSB.AppendLine("Wait Position=" + CData.SPos.dGRD_Z_Wait[0]);
            mSB.AppendLine("Zero Position=" + CData.SPos.dGRD_Z_Able[0]);
            mSB.AppendLine();
            mSB.AppendLine("[System Position - Grind Right X]");
            mSB.AppendLine("Wait Position=" + CData.SPos.dGRD_X_Wait[1]);
            mSB.AppendLine("Zero Position=" + CData.SPos.dGRD_X_Zero[1]);
            mSB.AppendLine();
            mSB.AppendLine("[System Position - Grind Right Y]");
            mSB.AppendLine("Wait Position="              + CData.SPos.dGRD_Y_Wait     [1]);
            mSB.AppendLine("Cleaning Start Position="    + CData.SPos.dGRD_Y_ClnStart [1]);
            mSB.AppendLine("Cleaning End Position="      + CData.SPos.dGRD_Y_ClnEnd   [1]);
            mSB.AppendLine("Dressing Start Position="    + CData.SPos.dGRD_Y_DrsStart [1]);
            mSB.AppendLine("Dressing End Position="      + CData.SPos.dGRD_Y_DrsEnd   [1]);
            mSB.AppendLine("Table Inspection Position="  + CData.SPos.dGRD_Y_TblInsp  [1]);
            mSB.AppendLine("Dresser Change Position="    + CData.SPos.dGRD_Y_DrsChange[1]);
            // 2021.04.13 SungTae Start : Table Grinding 시 조작 실수 방지 위해 추가
            mSB.AppendLine("Table Grind Start Position=" + CData.SPos.dGRD_Y_TblGrdSt [1]);
            mSB.AppendLine("Table Grind End Position="   + CData.SPos.dGRD_Y_TblGrdEd [1]);
            // 2021.04.13 SungTae End
            mSB.AppendLine();
            mSB.AppendLine("[System Position - Grind Right Z]");
            mSB.AppendLine("Wait Position=" + CData.SPos.dGRD_Z_Wait[1]);
            mSB.AppendLine("Zero Position=" + CData.SPos.dGRD_Z_Able[1]);
            mSB.AppendLine();
            mSB.AppendLine("[System Position - Off Loader Picker X]");
            mSB.AppendLine("Wait Position="       + CData.SPos.dOFP_X_Wait );
            mSB.AppendLine("Pick Left Position="  + CData.SPos.dOFP_X_PickL);
            mSB.AppendLine("Pick Right Position=" + CData.SPos.dOFP_X_PickR);
            mSB.AppendLine("Place Position="      + CData.SPos.dOFP_X_Place);
            mSB.AppendLine("Conversion Position=" + CData.SPos.dOFP_X_Conv ); //1903221 ksg :
            mSB.AppendLine("Picture1 Position=" + CData.SPos.dOFP_X_Picture1);  // 2021.03.30 lhs
            mSB.AppendLine("Picture2 Position=" + CData.SPos.dOFP_X_Picture2);  // 2021.03.30 lhs
            mSB.AppendLine("IV2 Cover Position=" + CData.SPos.dOFP_X_IV2Cover); //210831 syc : 2004U IV2
            mSB.AppendLine();
            mSB.AppendLine("[System Position - Off Loader Picker Z]");
            mSB.AppendLine("Wait Position="     + CData.SPos.dOFP_Z_Wait    );
            mSB.AppendLine("Picture Position="  + CData.SPos.dOFP_Z_Picture);  // 2021.03.30 lhs
            mSB.AppendLine("IV2 Cover Position=" + CData.SPos.dOFP_Z_IV2Cover);  //210831 syc : 2004U IV2

            // 2021.02.27 SungTae Start : 고객사(ASE-KR) 요청으로 Device에서 관리 위해 변경
            if (CData.CurCompany != ECompany.ASE_KR     &&
                CData.CurCompany != ECompany.SCK        &&  // 2021.07.14 lhs  SCK, JSCK 추가
                CData.CurCompany != ECompany.JSCK       &&
                CData.CurCompany != ECompany.SkyWorks   &&  // 220216 pjh : Skyworks 추가
                CDataOption.UseSprayBtmCleaner == false )   // 2022.03.08 lhs Spray Nozzle Bottom Cleaner 추가 
			{
				mSB.AppendLine("Cleaning Start Position="       + CData.SPos.dOFP_Z_ClnStart);
				//mSB.AppendLine("Strip Clean Start Position="    + CData.SPos.dOFP_Z_StripClnStart);
			}
			// 2021.02.27 SungTae End

            mSB.AppendLine();
            mSB.AppendLine("[System Position - Dry X]");
            mSB.AppendLine("Wait Position=" + CData.SPos.dDRY_X_Wait);
            mSB.AppendLine("Out Position="  + CData.SPos.dDRY_X_Out );
            mSB.AppendLine();
            mSB.AppendLine("[System Position - Dry Z]");
            mSB.AppendLine("Wait Position="         + CData.SPos.dDRY_Z_Wait );
            mSB.AppendLine("Up Position="           + CData.SPos.dDRY_Z_Up   );
            mSB.AppendLine("Sensor Check Position=" + CData.SPos.dDRY_Z_Check);
            CData.SPos.dDRY_Z_TipClamped = CData.SPos.dDRY_Z_Up - CData.Dev.dOffP_Z_PlaceDn; // 2021.04.22 lhs
            
			// 2022.01.17 SungTae Start : [추가] (ASE-KR VOC) Strip 유무 체크 시작 위치를 별도로 설정할 있도록 고객사 요청 사항
			if (CData.CurCompany == ECompany.ASE_KR)
			{
				mSB.AppendLine("Strip Check Position="  + CData.SPos.dDRY_Z_ChkStrip);
			}
			// 2022.01.17 SungTae End
			
            mSB.AppendLine();
            mSB.AppendLine("[System Position - Dry R]");
            mSB.AppendLine("Wait Position=" + CData.SPos.dDRY_R_Wait);
            mSB.AppendLine();
            mSB.AppendLine("[System Position - Off Loader X]");
            mSB.AppendLine("Wait Position="    + CData.SPos.dOFL_X_Wait    );
            mSB.AppendLine("Default Position=" + CData.SPos.dOFL_X_DefAlign);
            mSB.AppendLine();
            mSB.AppendLine("[System Position - Off Loader Y]");
            mSB.AppendLine("Pick Position="  + CData.SPos.dOFL_Y_Pick );
            mSB.AppendLine("Place Position=" + CData.SPos.dOFL_Y_Place);
            mSB.AppendLine();
            mSB.AppendLine("[System Position - Off Loader Z]");
            mSB.AppendLine("Wait Position="              + CData.SPos.dOFL_Z_Wait    );
            mSB.AppendLine("Top Pick Go Position="       + CData.SPos.dOFL_Z_TPickGo );
            mSB.AppendLine("Top Clamp Position="         + CData.SPos.dOFL_Z_TClamp  );
            mSB.AppendLine("Top Pick Up Position="       + CData.SPos.dOFL_Z_TPickUp );
            mSB.AppendLine("Top Place Go Position="      + CData.SPos.dOFL_Z_TPlaceGo);
            mSB.AppendLine("Top Unclamp Position="       + CData.SPos.dOFL_Z_TUnClamp);
            mSB.AppendLine("Top Place Down Position="    + CData.SPos.dOFL_Z_TPlaceDn);
            mSB.AppendLine("Bottom Pick Go Position="    + CData.SPos.dOFL_Z_BPickGo );
            mSB.AppendLine("Bottom Clamp Position="      + CData.SPos.dOFL_Z_BClamp  );
            mSB.AppendLine("Bottom Pick Up Position="    + CData.SPos.dOFL_Z_BPickUp );
            mSB.AppendLine("Bottom Place Go Position="   + CData.SPos.dOFL_Z_BPlaceGo);
            mSB.AppendLine("Bottom Unclamp Position="    + CData.SPos.dOFL_Z_BUnClamp);
            mSB.AppendLine("Bottom Place Down Position=" + CData.SPos.dOFL_Z_BPlaceDn);
            mSB.AppendLine();
            mSB.AppendLine("[Option]");
            mSB.AppendLine("Regrinding Skip Count=" + CData.ReSkipCnt);
            //201104 jhc : Grinding 시작 높이 계산 시 (휠팁 두께 값) 대신 (휠 측정 시 Z축 높이 값) 이용
            mSB.AppendLine("Use Wheel Z-Axis Based Calculation=" + CData.bUseWheelZAxisAfterMeasureWheel.ToString());
            //210830 syc : 2004U IV2
            mSB.AppendLine("[IV2 - Parameter]");
            mSB.AppendLine("ONP IV2 IP="   + CData.sIV2OnpIP  );
            mSB.AppendLine("ONP IV2 Port=" + CData.iIV2OnpPort);
            mSB.AppendLine("OFP IV2 IP="   + CData.sIV2OfpIP  );
            mSB.AppendLine("OFP IV2 Port=" + CData.iIV2OfpPort );
            mSB.AppendLine();
            //syc end

            //

            //2020.07.11 lks
            CCheckChange.CheckChanged("MASTER", sPath, CCheckChange.ReadOldFile(sPath), mSB.ToString());

            CLog.Check_File_Access(sPath, mSB.ToString(), false);

            return iRet;
        }

        public int Load()
        {
            int iRet = 0;

            string sPath = GV.PATH_CONFIG + "SetupPosition.cfg";
            string sSec = "";

            if (File.Exists(sPath) == false)
            {
                iRet = -1;
            }
            else
            {
                CData.SPos = new tSyP();
                CData.SPos.dGRD_X_Wait     = new double[2];
                CData.SPos.dGRD_X_Zero     = new double[2];
                CData.SPos.dGRD_Y_Wait     = new double[2];
                CData.SPos.dGRD_Y_ClnStart = new double[2];
                CData.SPos.dGRD_Y_ClnEnd   = new double[2];
                CData.SPos.dGRD_Y_DrsStart = new double[2];
                CData.SPos.dGRD_Y_DrsEnd   = new double[2];
                CData.SPos.dGRD_Y_DrsChange = new double[2];
                CData.SPos.dGRD_Y_TblInsp  = new double[2];
                CData.SPos.dGRD_Z_Wait     = new double[2];
                CData.SPos.dGRD_Z_Able     = new double[2];
                CData.SPos.dGRD_Y_DrsChange = new double[2];

                // 2021.04.13 SungTae Start : Table Grinding 시 조작 실수 방지 위해 추가
                CData.SPos.dGRD_Y_TblGrdSt = new double[2];
                CData.SPos.dGRD_Y_TblGrdEd = new double[2];
                // 2021.04.13 SungTae 

                CIni mIni = new CIni(sPath);
                sSec = "Main Position Left - Manual Position";
                CData.MPos[0].dZ_TBL_BASE        = mIni.ReadD(sSec, "Z Axis Table Base"                );
                CData.MPos[0].dZ_WHL_BASE        = mIni.ReadD(sSec, "Z Axis Wheel Base"                );
                CData.MPos[0].dZ_DRS_BASE        = mIni.ReadD(sSec, "Z Axis Dresser Base"              );
                CData.MPos[0].dZ_PRB_TOOL_SETTER = mIni.ReadD(sSec, "Z Axis Probe Measure Tool Setter" );
                CData.MPos[0].dY_PRB_TBL_CENTER  = mIni.ReadD(sSec, "Y Axis Probe Measure Table Center");
                CData.MPos[0].dY_WHL_TBL_CENTER  = mIni.ReadD(sSec, "Y Axis Wheel Center Table Center" );
                CData.MPos[0].dTOOL_SETTER_GAP   = mIni.ReadD(sSec, "Tool Setter Gap"                  );
                //CData.MPos[0].dPRB_OFFSET        = mIni.ReadDouble(sSec, "Probe Offset"                     );
                CData.MPos[0].dPCBThickness      = mIni.ReadD(sSec, "PCB Thickness"                    ); //191031 ksg :
                if(CData.MPos[0].dPCBThickness < 0) CData.MPos[0].dPCBThickness = 0;
                sSec = "Main Position Left - Calculate Position";
                CData.MPos[0].dZ_TBL_MEA_POS     = mIni.ReadD(sSec, "Z Axis Table Measure Start Position"  );
                CData.MPos[0].dZ_DRS_MEA_POS     = mIni.ReadD(sSec, "Z Axis Dresser Measure Start Position");
                CData.MPos[0].dY_WHL_TOOL_SETTER = mIni.ReadD(sSec, "Y Axis Wheel Center Tool Setter"      );
                CData.MPos[0].dY_WHL_DRS         = mIni.ReadD(sSec, "Y Axis Wheel Center Dresser Center"   );
                CData.MPos[0].dY_PRB_DRS         = mIni.ReadD(sSec, "Y Axis Probe Dresser Center"          );
                CData.MPos[0].dPRB_TO_WHL_BASE   = mIni.ReadD(sSec, "Probe To Wheel Base"                  );
                // 210426 jym
                sSec = "Main Position Left - Offset";
                CData.MPos[0].fake = mIni.ReadD(sSec, "After Offset");

                sSec = "Main Position Right - Manual Position";
                CData.MPos[1].dZ_TBL_BASE        = mIni.ReadD(sSec, "Z Axis Table Base"                );
                CData.MPos[1].dZ_WHL_BASE        = mIni.ReadD(sSec, "Z Axis Wheel Base"                );
                CData.MPos[1].dZ_DRS_BASE        = mIni.ReadD(sSec, "Z Axis Dresser Base"              );
                CData.MPos[1].dZ_PRB_TOOL_SETTER = mIni.ReadD(sSec, "Z Axis Probe Measure Tool Setter" );
                CData.MPos[1].dY_PRB_TBL_CENTER  = mIni.ReadD(sSec, "Y Axis Probe Measure Table Center");
                CData.MPos[1].dY_WHL_TBL_CENTER  = mIni.ReadD(sSec, "Y Axis Wheel Center Table Center" );
                CData.MPos[1].dTOOL_SETTER_GAP   = mIni.ReadD(sSec, "Tool Setter Gap"                  );
                //CData.MPos[1].dPRB_OFFSET        = mIni.ReadDouble(sSec, "Probe Offset"                     );

                sSec = "Main Position Right - Calculate Position";
                CData.MPos[1].dZ_TBL_MEA_POS     = mIni.ReadD(sSec, "Z Axis Table Measure Start Position"  );
                CData.MPos[1].dZ_DRS_MEA_POS     = mIni.ReadD(sSec, "Z Axis Dresser Measure Start Position");
                CData.MPos[1].dY_WHL_TOOL_SETTER = mIni.ReadD(sSec, "Y Axis Wheel Center Tool Setter"      );
                CData.MPos[1].dY_WHL_DRS         = mIni.ReadD(sSec, "Y Axis Wheel Center Dresser Center"   );
                CData.MPos[1].dY_PRB_DRS         = mIni.ReadD(sSec, "Y Axis Probe Dresser Center"          );
                CData.MPos[1].dPRB_TO_WHL_BASE   = mIni.ReadD(sSec, "Probe To Wheel Base"                  );

                // 210426 jym
                sSec = "Main Position Right - Offset";
                CData.MPos[1].fake = mIni.ReadD(sSec, "After Offset");

                sSec = "System Position - On Loader X";
                CData.SPos.dONL_X_Wait     = mIni.ReadD(sSec, "Wait Position"   );
                CData.SPos.dONL_X_DefAlign = mIni.ReadD(sSec, "Default Position");

                sSec = "System Position - On Loader Y";
                CData.SPos.dONL_Y_Pick  = mIni.ReadD(sSec, "Pick Position" );
                CData.SPos.dONL_Y_Place = mIni.ReadD(sSec, "Place Position");

                sSec = "System Position - On Loader Z";
                CData.SPos.dONL_Z_Wait    = mIni.ReadD(sSec, "Wait Position"      );
                CData.SPos.dONL_Z_PickGo  = mIni.ReadD(sSec, "Pick Go Position"   );
                CData.SPos.dONL_Z_Clamp   = mIni.ReadD(sSec, "Clamp Position"     );
                CData.SPos.dONL_Z_PickUp  = mIni.ReadD(sSec, "Pick Up Position"   );
                CData.SPos.dONL_Z_PlaceGo = mIni.ReadD(sSec, "Place Go Position"  );
                CData.SPos.dONL_Z_UnClamp = mIni.ReadD(sSec, "Unclamp Position"   );
                CData.SPos.dONL_Z_PlaceDn = mIni.ReadD(sSec, "Place Down Position");

                sSec = "System Position - Inrail X";
                CData.SPos.dINR_X_Wait = mIni.ReadD(sSec, "Wait Position");
                CData.SPos.dINR_X_ChkUnit = mIni.ReadD(sSec, "Sensor Check Position");

                sSec = "System Position - Inrail Y";
                CData.SPos.dINR_Y_Wait  = mIni.ReadD(sSec, "Wait Position" );
                CData.SPos.dINR_Y_Align = mIni.ReadD(sSec, "Align Position");

                sSec = "System Position - On Loader Picker X";
                CData.SPos.dONP_X_PlaceL = mIni.ReadD(sSec, "Place Left Position" );
                CData.SPos.dONP_X_PlaceR = mIni.ReadD(sSec, "Place Right Position");
                CData.SPos.dONP_X_Con    = mIni.ReadD(sSec, "Conversion Position" ); //190321 ksg :
                CData.SPos.dONP_X_WaitPickL = mIni.ReadD(sSec, "Pick Left Wait Position"); //20200406 jhc : OnPicker L-Table Pickup 대기 위치

                sSec = "System Position - On Loader Picker Z";
                CData.SPos.dONP_Z_Wait = mIni.ReadD(sSec, "Wait Position");

                //20190604 ghk_onpbcr
                sSec = "System Position - On Loader Picker Y";
                CData.SPos.dONP_Y_Wait = mIni.ReadD(sSec, "Wait Position");
                //

                sSec = "System Position - Grind Left X";
                CData.SPos.dGRD_X_Wait[0] = mIni.ReadD(sSec, "Wait Position");
                CData.SPos.dGRD_X_Zero[0] = mIni.ReadD(sSec, "Zero Position");

                sSec = "System Position - Grind Left Y";
                CData.SPos.dGRD_Y_Wait    [0] = mIni.ReadD(sSec, "Wait Position"            );
                CData.SPos.dGRD_Y_ClnStart[0] = mIni.ReadD(sSec, "Cleaning Start Position"  );
                CData.SPos.dGRD_Y_ClnEnd  [0] = mIni.ReadD(sSec, "Cleaning End Position"    );
                CData.SPos.dGRD_Y_DrsStart[0] = mIni.ReadD(sSec, "Dressing Start Position"  );
                CData.SPos.dGRD_Y_DrsEnd  [0] = mIni.ReadD(sSec, "Dressing End Position"    );
                CData.SPos.dGRD_Y_TblInsp [0] = mIni.ReadD(sSec, "Table Inspection Position");
                CData.SPos.dGRD_Y_DrsChange[0] = mIni.ReadD(sSec, "Dresser Change Position");
                // 2021.04.13 SungTae Start : Table Grinding 시 조작 실수 방지 위해 추가
                CData.SPos.dGRD_Y_TblGrdSt[0] = mIni.ReadD(sSec, "Table Grind Start Position");
                CData.SPos.dGRD_Y_TblGrdEd[0] = mIni.ReadD(sSec, "Table Grind End Position");
                // 2021.04.13 SungTae End

                sSec = "System Position - Grind Left Z";
                CData.SPos.dGRD_Z_Wait[0] = mIni.ReadD(sSec, "Wait Position");
                CData.SPos.dGRD_Z_Able[0] = mIni.ReadD(sSec, "Zero Position");

                sSec = "System Position - Grind Right X";
                CData.SPos.dGRD_X_Wait[1] = mIni.ReadD(sSec, "Wait Position");
                CData.SPos.dGRD_X_Zero[1] = mIni.ReadD(sSec, "Zero Position");

                sSec = "System Position - Grind Right Y";
                CData.SPos.dGRD_Y_Wait    [1] = mIni.ReadD(sSec, "Wait Position"            );
                CData.SPos.dGRD_Y_ClnStart[1] = mIni.ReadD(sSec, "Cleaning Start Position"  );
                CData.SPos.dGRD_Y_ClnEnd  [1] = mIni.ReadD(sSec, "Cleaning End Position"    );
                CData.SPos.dGRD_Y_DrsStart[1] = mIni.ReadD(sSec, "Dressing Start Position"  );
                CData.SPos.dGRD_Y_DrsEnd  [1] = mIni.ReadD(sSec, "Dressing End Position"    );
                CData.SPos.dGRD_Y_TblInsp [1] = mIni.ReadD(sSec, "Table Inspection Position");
                CData.SPos.dGRD_Y_DrsChange[1] = mIni.ReadD(sSec, "Dresser Change Position");
                // 2021.04.13 SungTae Start : Table Grinding 시 조작 실수 방지 위해 추가
                CData.SPos.dGRD_Y_TblGrdSt[1] = mIni.ReadD(sSec, "Table Grind Start Position");
                CData.SPos.dGRD_Y_TblGrdEd[1] = mIni.ReadD(sSec, "Table Grind End Position");
                // 2021.04.13 SungTae End

                sSec = "System Position - Grind Right Z";
                CData.SPos.dGRD_Z_Wait[1] = mIni.ReadD(sSec, "Wait Position");
                CData.SPos.dGRD_Z_Able[1] = mIni.ReadD(sSec, "Zero Position");

                sSec = "System Position - Off Loader Picker X";
                CData.SPos.dOFP_X_Wait  = mIni.ReadD(sSec, "Wait Position"      );
                CData.SPos.dOFP_X_PickL = mIni.ReadD(sSec, "Pick Left Position" );
                CData.SPos.dOFP_X_PickR = mIni.ReadD(sSec, "Pick Right Position");
                CData.SPos.dOFP_X_Place = mIni.ReadD(sSec, "Place Position"     );
                CData.SPos.dOFP_X_Conv  = mIni.ReadD(sSec, "Conversion Position");  //190321 ksg :
                CData.SPos.dOFP_X_Picture1 = mIni.ReadD(sSec, "Picture1 Position"); // 2021.03.30 lhs
                CData.SPos.dOFP_X_Picture2 = mIni.ReadD(sSec, "Picture2 Position"); // 2021.03.30 lhs
                CData.SPos.dOFP_X_IV2Cover = mIni.ReadD(sSec, "IV2 Cover Position"); //210831 syc : 2004U IV2

                sSec = "System Position - Off Loader Picker Z";
                CData.SPos.dOFP_Z_Wait      = mIni.ReadD(sSec, "Wait Position"          );
                CData.SPos.dOFP_Z_Picture   = mIni.ReadD(sSec, "Picture Position"); // 2021.03.30 lhs
                CData.SPos.dOFP_Z_IV2Cover  = mIni.ReadD(sSec, "IV2 Cover Position"); //210831 syc : 2004U IV2

                // 2022.03.08 lhs  : 기본값, Company 옵션 삭제, Device에 설정이 안된 경우 기초값으로 사용
                CData.SPos.dOFP_Z_ClnStart  = mIni.ReadD(sSec, "Cleaning Start Position");

                // 2021.07.14 lhs Start : Device의 Z값이 0이면 이전의 Option의 값 대입. (초기)
                if (CData.Dev.dOffP_Z_ClnStart      <= 0)   {   CData.Dev.dOffP_Z_ClnStart      = CData.SPos.dOFP_Z_ClnStart;       }
                if (CData.Dev.dOffP_Z_StripClnStart <= 0)   {   CData.Dev.dOffP_Z_StripClnStart = CData.Dev.dOffP_Z_ClnStart - 1;   }
                // 2021.07.14 lhs End

                sSec = "System Position - Dry X";
                CData.SPos.dDRY_X_Wait = mIni.ReadD(sSec, "Wait Position");
                CData.SPos.dDRY_X_Out  = mIni.ReadD(sSec, "Out Position" );

                sSec = "System Position - Dry Z";
                CData.SPos.dDRY_Z_Wait  = mIni.ReadD(sSec, "Wait Position"        );
                CData.SPos.dDRY_Z_Up    = mIni.ReadD(sSec, "Up Position"          );
                CData.SPos.dDRY_Z_Check = mIni.ReadD(sSec, "Sensor Check Position");
                CData.SPos.dDRY_Z_TipClamped = CData.SPos.dDRY_Z_Up - CData.Dev.dOffP_Z_PlaceDn; // 2021.04.22 lhs
                
				// 2022.01.17 SungTae Start : [추가] (ASE-KR VOC) Strip 유무 체크 시작 위치를 별도로 설정할 있도록 고객사 요청 사항
				if (CData.CurCompany == ECompany.ASE_KR)
				{
					CData.SPos.dDRY_Z_ChkStrip = mIni.ReadD(sSec, "Strip Check Position");
				}
				// 2022.01.17 SungTae End
				
                sSec = "System Position - Dry R";
                CData.SPos.dDRY_R_Wait = mIni.ReadD(sSec, "Wait Position");

                sSec = "System Position - Off Loader X";
                CData.SPos.dOFL_X_Wait     = mIni.ReadD(sSec, "Wait Position"   );
                CData.SPos.dOFL_X_DefAlign = mIni.ReadD(sSec, "Default Position");

                sSec = "System Position - Off Loader Y";
                CData.SPos.dOFL_Y_Pick  = mIni.ReadD(sSec, "Pick Position");
                CData.SPos.dOFL_Y_Place = mIni.ReadD(sSec, "Place Position");

                sSec = "System Position - Off Loader Z";
                CData.SPos.dOFL_Z_Wait     = mIni.ReadD(sSec, "Wait Position"             );
                CData.SPos.dOFL_Z_TPickGo  = mIni.ReadD(sSec, "Top Pick Go Position"      );
                CData.SPos.dOFL_Z_TClamp   = mIni.ReadD(sSec, "Top Clamp Position"        );
                CData.SPos.dOFL_Z_TPickUp  = mIni.ReadD(sSec, "Top Pick Up Position"      );
                CData.SPos.dOFL_Z_TPlaceGo = mIni.ReadD(sSec, "Top Place Go Position"     );
                CData.SPos.dOFL_Z_TUnClamp = mIni.ReadD(sSec, "Top Unclamp Position"      );
                CData.SPos.dOFL_Z_TPlaceDn = mIni.ReadD(sSec, "Top Place Down Position"   );
                CData.SPos.dOFL_Z_BPickGo  = mIni.ReadD(sSec, "Bottom Pick Go Position"   );
                CData.SPos.dOFL_Z_BClamp   = mIni.ReadD(sSec, "Bottom Clamp Position"     );
                CData.SPos.dOFL_Z_BPickUp  = mIni.ReadD(sSec, "Bottom Pick Up Position"   );
                CData.SPos.dOFL_Z_BPlaceGo = mIni.ReadD(sSec, "Bottom Place Go Position"  );
                CData.SPos.dOFL_Z_BUnClamp = mIni.ReadD(sSec, "Bottom Unclamp Position"   );
                CData.SPos.dOFL_Z_BPlaceDn = mIni.ReadD(sSec, "Bottom Place Down Position");

                sSec = "Option";
                CData.ReSkipCnt = mIni.ReadI(sSec, "Regrinding Skip Count");
                //201104 jhc : Grinding 시작 높이 계산 시 (휠팁 두께 값) 대신 (휠 측정 시 Z축 높이 값) 이용
                bool.TryParse(mIni.Read(sSec, "Use Wheel Z-Axis Based Calculation"), out CData.bUseWheelZAxisAfterMeasureWheel);
                //

                // 2020.10.19 SungTae : Delete(Skip Count 0 추가)
                //if (CData.ReSkipCnt == 0)
                //{ CData.ReSkipCnt = 1; }


                sSec = "IV2 - Parameter";
                //210830 syc : 2004U IV2
                CData.sIV2OnpIP   = mIni.Read(sSec, "ONP IV2 IP"   );
                CData.iIV2OnpPort = mIni.ReadI(sSec, "ONP IV2 Port" );
                CData.sIV2OfpIP   = mIni.Read(sSec, "OFP IV2 IP  " );
                CData.iIV2OfpPort  = mIni.ReadI(sSec, "OFP IV2 Port" );
                //
            }

            return iRet;
        }

        //20191028 ghk_auto_tool_offset
        public int Save_Offset()
        {
            int iRet = 0;
            string sPath = GV.PATH_CONFIG + "SetupPosition.cfg";
            string sSec = "";

            CIni mIni = new CIni(sPath);
            sSec = "Main Position Left - Manual Position";
            mIni.Write(sSec, "Z Axis Wheel Base", CData.MPos[0].dZ_WHL_BASE);
            mIni.Write(sSec, "Z Axis Probe Measure Tool Setter", CData.MPos[0].dZ_PRB_TOOL_SETTER);
            mIni.Write(sSec, "Tool Setter Gap", CData.MPos[0].dTOOL_SETTER_GAP);

            sSec = "Main Position Left - Calculate Position";

            CData.MPos[0].dPRB_TO_WHL_BASE = Math.Abs(Math.Round((CData.MPos[0].dZ_WHL_BASE - CData.MPos[0].dZ_PRB_TOOL_SETTER) - CData.MPos[0].dTOOL_SETTER_GAP, 6));

            mIni.Write(sSec, "Probe To Wheel Base", CData.MPos[0].dPRB_TO_WHL_BASE);

            sSec = "Main Position Right - Manual Position";
            mIni.Write(sSec, "Z Axis Wheel Base", CData.MPos[1].dZ_WHL_BASE);
            mIni.Write(sSec, "Z Axis Probe Measure Tool Setter", CData.MPos[1].dZ_PRB_TOOL_SETTER);
            mIni.Write(sSec, "Tool Setter Gap", CData.MPos[1].dTOOL_SETTER_GAP);

            sSec = "Main Position Right - Calculate Position";

            CData.MPos[1].dPRB_TO_WHL_BASE = Math.Abs(Math.Round((CData.MPos[1].dZ_WHL_BASE - CData.MPos[1].dZ_PRB_TOOL_SETTER) - CData.MPos[1].dTOOL_SETTER_GAP, 6));

            mIni.Write(sSec, "Probe To Wheel Base", CData.MPos[1].dPRB_TO_WHL_BASE);

            return iRet;
        }

        //20191028 ghk_auto_tool_offset
        public int Load_Offset()
        {
            int iRet = 0;

            string sPath = GV.PATH_CONFIG + "SetupPosition.cfg";
            string sSec = "";

            if (File.Exists(sPath) == false)
            {
                iRet = -1;
            }
            else
            {
                CIni mIni = new CIni(sPath);
                sSec = "Main Position Left - Manual Position";
                CData.MPos[0].dZ_WHL_BASE = mIni.ReadD(sSec, "Z Axis Wheel Base");
                CData.MPos[0].dZ_PRB_TOOL_SETTER = mIni.ReadD(sSec, "Z Axis Probe Measure Tool Setter");
                CData.MPos[0].dTOOL_SETTER_GAP = mIni.ReadD(sSec, "Tool Setter Gap");

                sSec = "Main Position Left - Calculate Position";
                CData.MPos[0].dPRB_TO_WHL_BASE = mIni.ReadD(sSec, "Probe To Wheel Base");

                sSec = "Main Position Right - Manual Position";
                CData.MPos[1].dZ_WHL_BASE = mIni.ReadD(sSec, "Z Axis Wheel Base");
                CData.MPos[1].dZ_PRB_TOOL_SETTER = mIni.ReadD(sSec, "Z Axis Probe Measure Tool Setter");
                CData.MPos[1].dTOOL_SETTER_GAP = mIni.ReadD(sSec, "Tool Setter Gap");

                sSec = "Main Position Right - Calculate Position";
                CData.MPos[1].dPRB_TO_WHL_BASE = mIni.ReadD(sSec, "Probe To Wheel Base");
            }

            return iRet;
        }

        /*
        private void CheckChanged(string path, string newData)
        {
            try
            {
                Console.WriteLine("==================================================================================");
                string oldData = ReadAllSetupPosition(path);
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
                    CLog.Save_Log(eLog.None, eLog.DSL, "MASTER : Manual Setup Position Changed log \r\n" + changeLog);
                }
            }
            catch (Exception err)
            {
                CLog.Save_Log(eLog.None, eLog.DSL, "MASTER : Manual Setup Position Changed log White ERROR : " + err.Message);
            }
        }

        private Dictionary<string, Hashtable> ConvertDicToStr(string str)
        {
            Dictionary<string, Hashtable> dicRet = new Dictionary<string, Hashtable>();
            Hashtable htTemp = new Hashtable();
            string tempKey = string.Empty;

            string[] newStrLines = str.Split('\n');
            for(int i=0;i<newStrLines.Length;i++)
            {
                newStrLines[i] = newStrLines[i].Replace("\r", "").Trim();

                if (newStrLines[i].Contains("[") && !tempKey.Equals(newStrLines[i]))
                {
                    tempKey = newStrLines[i];
                    if(htTemp.Count > 0)
                    {
                        dicRet.Add(tempKey, htTemp);
                    }
                    htTemp = new Hashtable();
                }
                else if(!string.IsNullOrEmpty(newStrLines[i]))
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
            string sPath = path;// GV.PATH_CONFIG + "SetupPosition.cfg";

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
        }
        */
    }
}
