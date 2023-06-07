using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics; //190328 : ksg

namespace SG2000X
{
    public class CSpc : CStn<CSpc>
    {
        /// <summary>
        /// 3 Step / 12 Step 구분하기 위한 Max step count
        /// </summary>
        public static int iStepMaxCnt = 0;

        /// <summary>
        /// 211213 pjh : Lot Name 자동 변경 시 Lot Name 뒤에 붙을 Number
        /// </summary>
        public static int iAddNum = 0;

        private CSpc() { }
        /// <summary>
        /// 매뉴얼/오토러닝 그라인딩 자재 측정 데이터 Csv 파일로 저장
        /// 
        /// Measure => BEFORE / AFTER
        /// TABLE => LEFT / RIGHT
        /// MGZ ID => XX
        /// SLOT ID => XX
        /// TARGET => X.XXX
        /// Finish Time => yyyy_MM_dd HH:mm:ss
        /// Wheel Serial => xxxxxxxxx    Wheel Height => x.xxxx
        /// 자재 측정 값
        /// 측정 안할 경우 셀 '.' / 측정 할 경우 'x.xxx' (소수점 3자리 표시)     
        /// 
        /// 경로 : D:\Spc\GrdData\GrindLeft of GrindRight\yyyy\MM\dd.csv
        /// </summary>
        /// <param name="eWy"> 테이블 왼쪽 or 오른쪽</param>
        /// <param name="sBfAf"> BEFORE or AFTER</param>
        /// <param name="dData"> 측정 데이터 </param>
        /// <returns></returns>
        public bool SaveDataCsv(EWay eWy, string sBfAf, double[,] dData, double[] dPcb)
        {
            StringBuilder mPath = new StringBuilder(GV.PATH_SPC);
            StringBuilder mMsg = new StringBuilder();
            double dPcbVal   = 0.0;

            DateTime dtNow = DateTime.Now;

            mPath.Append("GrdData\\");
            mPath.Append((eWy == EWay.L) ? "GrindLeft\\" : "GrindRight\\");
            mPath.Append(dtNow.ToString("yyyy"));
            mPath.Append("\\");
            mPath.Append(dtNow.ToString("MM"));
            mPath.Append("\\");

            if (!Directory.Exists(mPath.ToString()))
            {
                Directory.CreateDirectory(mPath.ToString());
            }

            mPath.Append(dtNow.ToString("dd"));
            mPath.Append(".csv");

            if (!File.Exists(mPath.ToString()))
            {
                File.Create(mPath.ToString()).Close();
            }

            // 200326 mjy : CSV Header 함수 제작 후 적용
            mMsg.Append(_CsvHeader(eWy, sBfAf, dtNow));

            //20191029 ghk_dfserver_notuse_df
            //if (!CData.Dev.bDynamicSkip && CDataOption.MeasureDf == eDfServerType.MeasureDf)
            if ((!CData.Dev.bDynamicSkip && CDataOption.MeasureDf == eDfServerType.MeasureDf) ||
                (CDataOption.UseDFDataServer && CData.Dev.eMoldSide == ESide.Btm))//211227 pjh : D/F Server 사용 시 Total Data도 저장 되게끔 추가
            {//다이나믹 펑션 사용
                //211227 pjh : D/F Server Bottom Grinding 시에는 Pcb Data가 없기 때문에 불러온 Data 사용
                if (CDataOption.UseDFDataServer && CData.Dev.eMoldSide == ESide.Btm && !CData.Opt.bSecsUse)//211227 pjh : D/F Server 사용 시 Total Data도 저장 되게끔 추가
                {
                    if (CData.Dynamic.iHeightType == 0)
                    {
                        //220111 pjh : DF Server 사용 시 Top Mold Thickness 저장 변수 변경
                        if (eWy == EWay.L)
                        {
                            dPcbVal = CData.Parts[(int)EPart.GRDL].dPcbMax + CData.Parts[(int)EPart.GRDL].dTopMoldMax;
                            _SetLog("Top PCB Thickness = " + CData.Parts[(int)EPart.GRDL].dPcbMax.ToString() + ", Top Mold Thickness = " + CData.Parts[(int)EPart.GRDL].dTopMoldMax.ToString());//220113 pjh : Data 확인용 Log
                        }
                        else
                        {
                            dPcbVal = CData.Parts[(int)EPart.GRDR].dPcbMax + CData.Parts[(int)EPart.GRDR].dTopMoldMax;
                            _SetLog("Top PCB Thickness = " + CData.Parts[(int)EPart.GRDR].dPcbMax.ToString() + ", Top Mold Thickness = " + CData.Parts[(int)EPart.GRDR].dTopMoldMax.ToString());//220113 pjh : Data 확인용 Log
                        }
                        //

                        mMsg.Append("PCB MAX,,");
                        mMsg.Append(dPcbVal);
                    }
                    else
                    {
                        //220111 pjh : DF Server 사용 시 Top Mold Thickness 저장 변수 변경
                        if (eWy == EWay.L)
                        {
                            dPcbVal = CData.Parts[(int)EPart.GRDL].dPcbMean + CData.Parts[(int)EPart.GRDL].dTopMoldAvg;
                            _SetLog("Top PCB Thickness = " + CData.Parts[(int)EPart.GRDL].dPcbMean.ToString() + ", Top Mold Thickness = " + CData.Parts[(int)EPart.GRDL].dTopMoldAvg.ToString());//220113 pjh : Data 확인용 Log
                        }
                        else
                        {
                            dPcbVal = CData.Parts[(int)EPart.GRDR].dPcbMean + CData.Parts[(int)EPart.GRDR].dTopMoldAvg;
                            _SetLog("Top PCB Thickness = " + CData.Parts[(int)EPart.GRDR].dPcbMean.ToString() + ", Top Mold Thickness = " + CData.Parts[(int)EPart.GRDR].dTopMoldAvg.ToString());//220113 pjh : Data 확인용 Log
                        }
                        //
                        mMsg.Append("PCB MEAN,,");
                        mMsg.Append(dPcbVal);
                    }
                    mMsg.AppendLine();
                }
                //
                else
                {
                    //20200328 jhc : 다이나믹 펑션 측정 포인트 최대 5개로 확대
                    double[] dTmpPcb = new double[CData.Dev.iDynamicPosNum];
                    for (int i = 0; i < CData.Dev.iDynamicPosNum; i++)
                    {
                        dTmpPcb[i] = dPcb[i]; //배열 아이템 수는 최대 5개지만 실제 측정 포인트 수는 3~5로 유동적이므로 별도 배열 할당 후 Max, Avg 값을 구함
                    }

                    mMsg.Append("PCB Height");
                    for (int i = 0; i < CData.Dev.iDynamicPosNum; i++)
                    {
                        mMsg.Append(",,");
                        mMsg.Append(dTmpPcb[i]);
                    }
                    mMsg.AppendLine();

                    if (CData.Dynamic.iHeightType == 0)
                    {
                        dPcbVal = dTmpPcb.Max();
                        mMsg.Append("PCB MAX,,");
                        mMsg.Append(dPcbVal);
                    }
                    else
                    {
                        dPcbVal = Math.Round(dTmpPcb.Average(), 4);
                        mMsg.Append("PCB MEAN,,");
                        mMsg.Append(dPcbVal);
                    }
                    mMsg.AppendLine();
                }
            }

            int iRowCnt = dData.GetLength(0);
            int iColCnt = dData.GetLength(1);
            double dVal = 0;
            for (int iRow = 0; iRow < iRowCnt; iRow++)
            {
                for (int iCol = 0; iCol < iColCnt; iCol++)
                {
                    dVal = dData[iRow, iCol];
                    if (dVal == 0 || dVal == 999.999)   { mMsg.Append(".");                 }
                    else                                { mMsg.Append(dVal.ToString("F3")); }

                    if ((iCol + 1) < iColCnt)           { mMsg.Append(","); }
                }

                if (!CData.Dev.bDynamicSkip || (CDataOption.UseDFDataServer && CData.Dev.eMoldSide == ESide.Btm && !CData.Opt.bSecsUse))
                {
                    mMsg.Append(",,,,");

                    for (int iCol = 0; iCol < iColCnt; iCol++)
                    {
                        dVal = dData[iRow, iCol];
                        if (dVal == 0 || dVal == 999.999)   { mMsg.Append(".");                                     }
                        else                                { dVal += dPcbVal;  mMsg.Append(dVal.ToString("F3"));   }

                        if ((iCol + 1) < iColCnt)           { mMsg.Append(","); }
                    }
                }

                mMsg.AppendLine();
            }

            CLog.Check_File_Access(mPath.ToString(), mMsg.ToString(), true);

            // 200828 jym : Network drive 설정
            // 210601 jym : 네트워크 드라이브 스레드 추가
            if (CData.Opt.bNetUse)
            {
                mPath = new StringBuilder(@"\\");
                mPath.Append(CData.Opt.sNetIP);
                mPath.Append(CData.Opt.sNetPath);
                mPath.Append(@"SPC\GrdData\");
                mPath.Append((eWy == EWay.L) ? "GrindLeft\\" : "GrindRight\\");
                mPath.Append(dtNow.ToString("yyyy"));
                mPath.Append("\\");
                mPath.Append(dtNow.ToString("MM"));
                mPath.Append("\\");
                mPath.Append(dtNow.ToString("dd"));
                mPath.Append(".csv");

                CData.QueNet.Enqueue(new TNetDrive(mPath.ToString(), mMsg.ToString()));
            }
            return true;
        }

        public bool SaveDataCsvU(EWay eWy, string sBfAf)
        {
            int iWy = (int)eWy;
            double dVal = 0;

            StringBuilder mPath;
            StringBuilder mNote;

            DateTime dtNow = DateTime.Now;

            DirectoryInfo mDI;
            FileInfo mFI;

            mPath = new StringBuilder(GV.PATH_SPC);
            mPath.Append("GrdData\\");

            if (eWy == EWay.L)
            {
                mPath.Append("GrindLeft\\");
            }
            else
            {
                mPath.Append("GrindRight\\");
            }

            mPath.Append(dtNow.ToString("yyyy"));
            mPath.Append("\\");
            mPath.Append(dtNow.ToString("MM"));
            mPath.Append("\\");

            mDI = new DirectoryInfo(mPath.ToString());
            if (!mDI.Exists)
            { mDI.Create(); }

            mPath.Append(dtNow.ToString("dd"));
            mPath.Append(".csv");

            mFI = new FileInfo(mPath.ToString());
            if (mFI.Exists == false)
            {
                mFI.Create().Close();
            }

            // 200326 mjy : CSV Header 함수 제작 후 적용
            mNote = new StringBuilder(_CsvHeader(eWy, sBfAf, dtNow));

            for (int iRow = 0; iRow < CData.Dev.iRow; iRow++)
            {
                string sRow = "";

                for (int iU = 0; iU < CData.Dev.iUnitCnt; iU++)
                {
                    for (int iCol = 0; iCol < CData.Dev.iCol; iCol++)
                    {
                        if (sBfAf == "BEFORE")
                        { dVal = CData.GrData[iWy].aUnit[iU].aMeaBf[iRow, iCol]; }
                        else
                        { dVal = CData.GrData[iWy].aUnit[iU].aMeaAf[iRow, iCol]; }

                        if (dVal == 0 || dVal == 999.999) //190813 ksg : 변경
                        { sRow += "."; }
                        else
                        {
                            sRow += dVal.ToString("F3");
                        }

                        if ((iU + 1 == CData.Dev.iUnitCnt) && iCol + 1 == CData.Dev.iCol)
                        { }// sData += "\n"; }
                        else
                        { sRow += ","; }
                    }

                    if (iU + 1 != CData.Dev.iUnitCnt)
                    { sRow += "l,"; }
                }

                mNote.AppendLine(sRow);
            }

            CLog.Check_File_Access(mPath.ToString(), mNote.ToString(), true);

            // 200828 jym : Network drive 설정
            // 210601 jym : 네트워크 드라이브 스레드 추가
            if (CData.Opt.bNetUse)
            {
                mPath = new StringBuilder(@"\\");
                mPath.Append(CData.Opt.sNetIP);
                mPath.Append(CData.Opt.sNetPath);
                mPath.Append(@"SPC\GrdData\");
                mPath.Append((eWy == EWay.L) ? "GrindLeft\\" : "GrindRight\\");
                mPath.Append(dtNow.ToString("yyyy"));
                mPath.Append("\\");
                mPath.Append(dtNow.ToString("MM"));
                mPath.Append("\\");
                mPath.Append(dtNow.ToString("dd"));
                mPath.Append(".csv");

                CData.QueNet.Enqueue(new TNetDrive(mPath.ToString(), mNote.ToString()));
            }

            return true;
        }
        
        /// <summary>
        /// CSV 파일의 헤더 부분 
        /// </summary>
        /// <param name="eWy"></param>
        /// <param name="sBfAf"></param>
        /// <param name="dtNow"></param>
        /// <returns></returns>
        private string _CsvHeader(EWay eWy, string sBfAf, DateTime dtNow)
        {
            int    iWy          = (int)eWy;
            int    iPart        = (eWy == EWay.L) ? (int)EPart.GRDL : (int)EPart.GRDR;

            string sTable       = (eWy == EWay.L) ? "LEFT" : "RIGHT";
            string sMgzId       = CData.Parts[iPart].iMGZ_No.ToString();
            string sSlotId      = CData.Parts[iPart].iSlot_No.ToString();
            string sWheel       = CData.Whls[iWy].sWhlName;
            string sWhHeight    = CData.WhlAf[iWy].ToString();
            string sBcr         = CData.Parts[iPart].sBcr;
            string sGroup       = new DirectoryInfo(Path.GetDirectoryName(CData.DevCur)).Name; //190704 myk_GrpDevName
            string sDevice      = CData.Dev.sName;
            string sMaxLoad     = CData.GrData[iWy].dSplMaxLoad.ToString();
            //201225-1 jhc : 고급 Grind Condition 설정/체크 기능 (Spindle Current, Table Vacuum Low Limit 설정)
            string sMaxSpindleCurrent   = CData.GrData[iWy].nSpindleCurrentMax.ToString();
            string sMinTableVacuum      = CData.GrData[iWy].dTableVacuumMin.ToString("0.0");
            //..
            string sAfterDrs = (CData.Whls[iWy].iGdc + 1).ToString(); //201216 jhc : 카운트 1부터, OnCarrierGrindEnd_Dataset() 통한 SECS/GEM 데이터 수정 필요하여 여기서 수정함 //CData.Whls[iWy].iGdc.ToString();
            string sTarget   = "";

            // 2020.09.08 SungTae : 3 Step 기능 추가
            if (CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)  { iStepMaxCnt = GV.StepMaxCnt;      }            
            else                                                    { iStepMaxCnt = GV.StepMaxCnt_3;    }

            if (sBfAf.Equals("BEFORE"))
            {
                sTarget = "0";
            }
            else
            {
                for (int i = iStepMaxCnt - 1; i >= 0; i--)
                {
                    if (CData.Dev.aData[iWy].aSteps[i].bUse)
                    {
                        sTarget = CData.GrData[iWy].aTar[i].ToString();
                        break;
                    }
                }
            }

            StringBuilder mNote = new StringBuilder();

            mNote.AppendLine("==================================================================================================");
            mNote.AppendLine("Measure,"             + sBfAf);
            mNote.AppendLine("TABLE,"               + sTable);
            mNote.AppendLine("GROUP,"               + sGroup);
            mNote.AppendLine("DEVICE,"              + sDevice);
            mNote.AppendLine("MGZ ID,"              + sMgzId);
            mNote.AppendLine("SLOT ID,"             + sSlotId);
            mNote.AppendLine("TARGET,"              + sTarget);
            mNote.AppendLine("BARCODE,"             + sBcr);
            mNote.AppendLine("Finish Time,"         + dtNow.ToString("yyyy_MM_dd HH:mm:ss"));
            mNote.AppendLine("Wheel Serial,,"       + sWheel + ",," + sWhHeight);
            mNote.AppendLine("Max Spindle Load,"    + sMaxLoad);

            //201225-1 jhc : 고급 Grind Condition 설정/체크 기능 (Spindle Current, Table Vacuum Low Limit 설정)
            //if (CDataOption.UseDeviceAdvancedOption && CDataOption.UseAdvancedGrindCondition && (CDataOption.SplType == eSpindleType.EtherCat))
            // 2023.03.15 Max
            if (CDataOption.UseDeviceAdvancedOption && CDataOption.UseAdvancedGrindCondition)
            {
                //Advanced Grind Condition 체크 사용 조건(라이선스)에서 추가 정보 남김
                mNote.AppendLine("Max Spindle Current," + sMaxSpindleCurrent);
                mNote.AppendLine("Min Table Vacuum,"    + sMinTableVacuum);
            }
            //..

            // 200326 mjy : 드레싱 이후 몇번째 그라인딩 자재인지 카운트 표시
            mNote.AppendLine("After Dressing Count," + sAfterDrs);

            //190718 ksg :
            if (sBfAf.Equals("BEFORE"))
            {
                mNote.AppendLine("Grind Cycle Time,,0");
            }
            else
            {
                mNote.AppendLine("Grind Cycle Time,," + CData.GrdElp[iWy].tsEls.ToString());

                // regrinding log
                if (CDataOption.ReGrdLog == eReGrdLog.Use)
                {
                    mNote.AppendLine("StepNo, ReGrinding No, ReGrinding Cnt");
                    for (int iStep = 0; iStep < iStepMaxCnt - 1; iStep++)
                    {
                        //if (iStepMaxCnt == 12)
                        if (CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)      // 2020.10.09 SungTae : Modify
                        {
                            if (CData.RedGrdCnt[iWy, iStep].m_lRedCnt.Count > 0)
                            {
                                for (int iReCnt = 0; iReCnt < CData.RedGrdCnt[iWy, iStep].m_lRedCnt.Count; iReCnt++)
                                {
                                    mNote.AppendLine((iStep + 1).ToString() + "," + (iReCnt + 1).ToString() + "," + CData.RedGrdCnt[iWy, iStep].m_lRedCnt[iReCnt].ToString());
                                }
                            }
                        }
                        else
                        {
                            if (CData.RedGrdCnt3[iWy, iStep].m_lRedCnt.Count > 0)
                            {
                                for (int iReCnt = 0; iReCnt < CData.RedGrdCnt3[iWy, iStep].m_lRedCnt.Count; iReCnt++)
                                {
                                    mNote.AppendLine((iStep + 1).ToString() + "," + (iReCnt + 1).ToString() + "," + CData.RedGrdCnt3[iWy, iStep].m_lRedCnt[iReCnt].ToString());
                                }
                            }
                        }
                    }
                }
            }

            if (CData.Opt.bSecsUse == true)
            {// 2020.11.23 SECS/GEM 1.0.18 SVID 추가
                if (CData.GemForm != null) CData.GemForm.OnCarrierGrindEnd_Dataset(iPart, sMaxLoad, sAfterDrs);
            }

            return mNote.ToString();
        }

        /// <summary>
        /// CSV 파일의 헤더 부분 
        /// </summary>
        /// <param name="eWy"></param>
        /// <param name="sBfAf"></param>
        /// <param name="dtNow"></param>
        /// <returns></returns>
        private string _CsvHeaderLot(EWay eWy, string sBfAf, DateTime dtNow)
        {
            int    iWy      = (int)eWy;
            int    iPart    = (eWy == EWay.L) ? (int)EPart.GRDL : (int)EPart.GRDR;

            string sTable       = (eWy == EWay.L) ? "LEFT" : "RIGHT";
            string sLotName     = CData.LotInfo.sLotName;
            string sMgzId       = CData.Parts[iPart].iMGZ_No.ToString();
            string sSlotId      = CData.Parts[iPart].iSlot_No.ToString();
            string sWheel       = CData.Whls[iWy].sWhlName;
            string sWhHeight    = CData.WhlAf[iWy].ToString();
            string sBcr         = CData.Parts[iPart].sBcr;
            string sGroup       = new DirectoryInfo(Path.GetDirectoryName(CData.DevCur)).Name; //190704 myk_GrpDevName
            string sDevice      = CData.Dev.sName;
            string sMaxLoad     = CData.GrData[iWy].dSplMaxLoad.ToString();
            string sAfterDrs    = CData.Whls[iWy].iGdc.ToString();
            string sTarget      = "";

            // 2020.09.08 SungTae : 3 Step 기능 추가
            if (CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)  { iStepMaxCnt = GV.StepMaxCnt;      }
            else                                                    { iStepMaxCnt = GV.StepMaxCnt_3;    }

            if (sBfAf.Equals("BEFORE"))
            {
                sTarget = "0";
            }
            else
            {
                //for (int i = GV.StepMaxCnt - 1; i >= 0; i--)
                for (int i = iStepMaxCnt - 1; i >= 0; i--)      // 2020.09.08 SungTae : Modify
                {
                    if (CData.Dev.aData[iWy].aSteps[i].bUse)
                    {
                        sTarget = CData.GrData[iWy].aTar[i].ToString();
                        break;
                    }
                }
            }

            StringBuilder mNote = new StringBuilder();

            mNote.AppendLine("==================================================================================================");
            mNote.AppendLine("Measure," + sBfAf);
            mNote.AppendLine("TABLE," + sTable);
            mNote.AppendLine("LOT ID," + sLotName);
            mNote.AppendLine("GROUP," + sGroup);
            mNote.AppendLine("DEVICE," + sDevice);
            mNote.AppendLine("MGZ ID," + sMgzId);
            mNote.AppendLine("SLOT ID," + sSlotId);
            mNote.AppendLine("TARGET," + sTarget);
            mNote.AppendLine("BARCODE," + sBcr);
            mNote.AppendLine("Finish Time," + dtNow.ToString("yyyy_MM_dd HH:mm:ss"));
            mNote.AppendLine("Wheel Serial,," + sWheel + ",," + sWhHeight);
            mNote.AppendLine("Max Spindle Load," + sMaxLoad);

            // 200326 mjy : 드레싱 이후 몇번째 그라인딩 자재인지 카운트 표시
            mNote.AppendLine("After Dressing Count," + sAfterDrs);

            //190718 ksg :
            if (sBfAf.Equals("BEFORE"))
            {
                mNote.AppendLine("Grind Cycle Time,,0");
            }
            else
            {
                mNote.AppendLine("Grind Cycle Time,," + CData.GrdElp[iWy].tsEls.ToString());

                // regrinding log
                if (CDataOption.ReGrdLog == eReGrdLog.Use)
                {
                    mNote.AppendLine("StepNo, ReGrinding No, ReGrinding Cnt");

                    //for (int iStep = 0; iStep < GV.StepMaxCnt - 1; iStep++)
                    for (int iStep = 0; iStep < iStepMaxCnt - 1; iStep++)       // 2020.09.08 SungTae : Modify
                    {
                        //if (iStepMaxCnt == 12)
                        if (CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)      // 2020.10.09 SungTae : Modify
                        {
                            if (CData.RedGrdCnt[iWy, iStep].m_lRedCnt.Count > 0)
                            {
                                for (int iReCnt = 0; iReCnt < CData.RedGrdCnt[iWy, iStep].m_lRedCnt.Count; iReCnt++)
                                {
                                    mNote.AppendLine((iStep + 1).ToString() + "," + (iReCnt + 1).ToString() + "," + CData.RedGrdCnt[iWy, iStep].m_lRedCnt[iReCnt].ToString());
                                }
                            }
                        }
                        else
                        {
                            if (CData.RedGrdCnt3[iWy, iStep].m_lRedCnt.Count > 0)
                            {
                                for (int iReCnt = 0; iReCnt < CData.RedGrdCnt3[iWy, iStep].m_lRedCnt.Count; iReCnt++)
                                {
                                    mNote.AppendLine((iStep + 1).ToString() + "," + (iReCnt + 1).ToString() + "," + CData.RedGrdCnt3[iWy, iStep].m_lRedCnt[iReCnt].ToString());
                                }
                            }
                        }
                    }
                }
            }

            return mNote.ToString();
        }

        /// <summary>
        /// 오토러닝 그라인딩 자재 랏기준 측정 데이터 Csv 파일로 저장
        /// 
        /// Measure => BEFORE / AFTER
        /// TABLE => LEFT / RIGHT
        /// MGZ ID => XX
        /// SLOT ID => XX
        /// TARGET => X.XXX
        /// Finish Time => yyyy_MM_dd HH:mm:ss
        /// Wheel Serial => xxxxxxxxx    Wheel Height => x.xxxx
        /// 자재 측정 값
        /// 측정 안할 경우 셀 '.' / 측정 할 경우 'x.xxx' (소수점 3자리 표시)     
        /// 
        /// 경로 : D:\Spc\GrdData\GrindLeft of GrindRight\yyyy\MM\dd.csv
        /// </summary>
        /// <param name="eWy"> 테이블 왼쪽 or 오른쪽</param>
        /// <param name="sBfAf"> BEFORE or AFTER</param>
        /// <param name="dData"> 측정 데이터 </param>
        /// <returns></returns>
        public bool SaveLotDataCsv(ref TLotInfo pLotInfo, EWay eWy, string sBfAf, double[,] dData, double[] dPcb)
        {
            string sPath = GV.PATH_SPC + "LotLog\\";
            string sData     = "";
            string sTable    = "";
            string sMgzId    = "";
            string sSlotId   = "";
            string sTarget   = "";
            string sWheel    = "";
            string sWhHeight = "";
            string sBcr      = "";
            string sLotName  = "";

            string sGroup    = "";
            string sDevice   = "";

            //201225-1 jhc : 고급 Grind Condition 설정/체크 기능 (Spindle Current, Table Vacuum Low Limit 설정)
            int iWy = (int)eWy;
            string sMaxSpindleCurrent   = CData.GrData[iWy].nSpindleCurrentMax.ToString();
            string sMinTableVacuum      = CData.GrData[iWy].dTableVacuumMin.ToString("0.0");
            //..

            double dPcbVal   = 0.0; //190625 ksg :
            string sMsgWrite = "";

            DirectoryInfo di;
            FileInfo fi;

            if (CData.CurCompany == ECompany.ASE_K12)   sLotName = CData.LotInfo.sNewLotName;//211213 pjh : 저장하는 Lot Name 변경
            else                                        sLotName = pLotInfo.sLotName;    //  CData.LotInfo.sLotName;

            sPath += DateTime.Now.ToString("yyyyMMdd") + "\\";
            sPath += sLotName + "\\";

            // 2020.09.08 SungTae : 3 Step 기능 추가
            if (CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)  { iStepMaxCnt = GV.StepMaxCnt;      }
            else                                                    { iStepMaxCnt = GV.StepMaxCnt_3;    }

            di = new DirectoryInfo(sPath);

            if (di.Exists == false)
            {
                di.Create();
            }

            if (eWy == EWay.L)
            {
                sPath  += "GrindLeft.csv";
                sTable  = "LEFT";
                sMgzId  = CData.Parts[(int)EPart.GRDL].iMGZ_No .ToString();
                sSlotId = CData.Parts[(int)EPart.GRDL].iSlot_No.ToString();

                for (int i = iStepMaxCnt - 1; i >= 0; i--) 
                {
                    if (CData.Dev.aData[0].aSteps[i].bUse)
                    {
                        sTarget = CData.GrData[(int)eWy].aTar[i].ToString();
                        break;
                    }
                }
                sWheel    = CData.Whls [0].sWhlName  ;
                sWhHeight = CData.WhlAf[0].ToString();
                sBcr      = CData.Parts[(int)EPart.GRDL].sBcr;
                //190704 myk_GrpDevName
                sGroup = new DirectoryInfo(Path.GetDirectoryName(CData.DevCur)).Name;
                sDevice = CData.Dev.sName;
            }
            else
            {
                sPath  += "GrindRight.csv";
                sTable  = "RIGHT";
                sMgzId  = CData.Parts[(int)EPart.GRDR].iMGZ_No .ToString();
                sSlotId = CData.Parts[(int)EPart.GRDR].iSlot_No.ToString();

                for (int i = iStepMaxCnt - 1; i >= 0; i--)
                {
                    if (CData.Dev.bDual == eDual.Dual)
                    {
                        if (CData.Dev.aData[1].aSteps[i].bUse)
                        {
                            sTarget = CData.GrData[(int)eWy].aTar[i].ToString();
                            break;
                        }
                    }
                    else
                    {
                        if (CData.Dev.aData[0].aSteps[i].bUse)
                        {
                            sTarget = CData.GrData[(int)eWy].aTar[i].ToString();
                            break;
                        }
                    }
                }

                sWheel      = CData.Whls[1].sWhlName;
                sWhHeight   = CData.WhlAf[1].ToString();

                sBcr = CData.Parts[(int)EPart.GRDR].sBcr;
                //190704 myk_GrpDevName
                sGroup = new DirectoryInfo(Path.GetDirectoryName(CData.DevCur)).Name;
                sDevice = CData.Dev.sName;
            }

            //190718 ksg :
            if(sBfAf == "BEFORE")
            {
                sTarget = "0";
            }
            else
            {
                for (int i = iStepMaxCnt - 1; i >= 0; i--)
                {
                    if (CData.Dev.aData[(int)eWy].aSteps[i].bUse)
                    {
                        sTarget = CData.GrData[(int)eWy].aTar[i].ToString();
                        break;
                    }
                }
            }

            fi = new FileInfo(sPath);

            if (fi.Exists == false)
            {
                fi.Create().Close();
            }

            sMsgWrite += "==================================================================================================\n";
            sData   = "Measure,"    + sBfAf;                                            sMsgWrite += sData +"\n";
            sData   = "TABLE,"      + sTable;                                           sMsgWrite += sData +"\n";
            sData   = "LotId,"      + pLotInfo.sLotName;                                sMsgWrite += sData +"\n";

            //190704 myk_GrpDevName
            sData   = "GROUP,"      + sGroup;                                           sMsgWrite += sData +"\n";
            sData   = "DEVICE,"     + sDevice;                                          sMsgWrite += sData +"\n";
            sData   = "MGZ ID,"     + sMgzId;                                           sMsgWrite += sData +"\n";
            sData   = "SLOT ID,"    + sSlotId;                                          sMsgWrite += sData +"\n";
            sData   = "TARGET,"     + sTarget;                                          sMsgWrite += sData +"\n";
            sData   = "BARCODE,"    + sBcr;                                             sMsgWrite += sData +"\n";
            sData   = "Finish Time," + DateTime.Now.ToString("yyyy_MM_dd HH:mm:ss");    sMsgWrite += sData +"\n";
            sData   = "Wheel Serial,," + sWheel + ",," + sWhHeight;                     sMsgWrite += sData +"\n";

            //201225-1 jhc : 고급 Grind Condition 설정/체크 기능 (Spindle Current, Table Vacuum Low Limit 설정)
            //if (CDataOption.UseDeviceAdvancedOption && CDataOption.UseAdvancedGrindCondition && (CDataOption.SplType == eSpindleType.EtherCat))
            // 2023.03.15 Max
            if (CDataOption.UseDeviceAdvancedOption && CDataOption.UseAdvancedGrindCondition)
            {
                //Advanced Grind Condition 체크 사용 조건(라이선스)에서 추가 정보 남김
                sData   = "Max Spindle Current,"    + sMaxSpindleCurrent;               sMsgWrite += sData +"\n";
                sData   = "Min Table Vacuum,"       + sMinTableVacuum;                  sMsgWrite += sData +"\n";
            }
            //..

            if(sBfAf == "BEFORE")
            {
                sData   = "Grind Cycle Time,," + "0";                                   sMsgWrite += sData +"\n";
            }
            else
            {
                sData = "Grind Cycle Time,," + CData.GrdElp[(int)eWy].tsEls.ToString(); sMsgWrite += sData +"\n";

                //20191111 ghk_regrindinglog
                if (CDataOption.ReGrdLog == eReGrdLog.Use)
                {
                    sData = "StepNo, ReGrinding No, ReGrinding Cnt" + "\n";
                    
                    for (int iStep = 0; iStep < iStepMaxCnt - 1; iStep++)     
                    {
                        if (CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)
                        {
                            if (CData.RedGrdCnt[(int)eWy, iStep].m_lRedCnt.Count > 0)
                            {
                                for (int iReCnt = 0; iReCnt < CData.RedGrdCnt[(int)eWy, iStep].m_lRedCnt.Count; iReCnt++)
                                {
                                    sData += (iStep + 1).ToString() + "," + (iReCnt + 1).ToString() + "," + CData.RedGrdCnt[(int)eWy, iStep].m_lRedCnt[iReCnt].ToString() + "\n";
                                }
                            }
                        }
                        else
                        {
                            if (CData.RedGrdCnt3[(int)eWy, iStep].m_lRedCnt.Count > 0)
                            {
                                for (int iReCnt = 0; iReCnt < CData.RedGrdCnt3[(int)eWy, iStep].m_lRedCnt.Count; iReCnt++)
                                {
                                    sData += (iStep + 1).ToString() + "," + (iReCnt + 1).ToString() + "," + CData.RedGrdCnt3[(int)eWy, iStep].m_lRedCnt[iReCnt].ToString() + "\n";
                                }
                            }
                        }
                    }
                    sMsgWrite += sData;
                }
                // 
            }

            //if (!CData.Dev.bDynamicSkip && CDataOption.MeasureDf == eDfServerType.MeasureDf)
            if ((!CData.Dev.bDynamicSkip && CDataOption.MeasureDf == eDfServerType.MeasureDf) || 
                (CDataOption.UseDFDataServer && CData.Dev.eMoldSide == ESide.Btm && !CData.Opt.bSecsUse))//211227 pjh : D/F Server 사용 시 Total Data도 저장 되게끔 추가
            {//다이나믹 펑션 사용
#if true //20200328 jhc : 다이나믹 펑션 측정 포인트 최대 5개로 확대
                
                //211227 pjh : D/F Server Bottom Grinding 시에는 Pcb Data가 없기 때문에 불러온 Data 사용
                if (CDataOption.UseDFDataServer && CData.Dev.eMoldSide == ESide.Btm)//211227 pjh : D/F Server 사용 시 Total Data도 저장 되게끔 추가
                {
                    if (CData.Dynamic.iHeightType == 0)
                    {
                        if (eWy == EWay.L)
                        {
                            dPcbVal = CData.Parts[(int)EPart.GRDL].dPcbMax;
                            //sData = "PCB MAX,," + (dPcbVal + CSq_OnP.It.m_tTarget.dAfMax).ToString();
                            sData = "PCB MAX,," + (dPcbVal + CData.Parts[(int)EPart.GRDL].dTopMoldMax).ToString();//220111 pjh : DF Server 사용 시 Top Mold Thickness 저장 변수 변경
                            _SetLog("Top PCB Thickness = " + dPcbVal.ToString() + ", Top Mold Thickness = " + CData.Parts[(int)EPart.GRDL].dTopMoldMax.ToString());//220113 pjh : Data 확인용 Log
                        }
                        else
                        {
                            dPcbVal = CData.Parts[(int)EPart.GRDR].dPcbMax;
                            //sData = "PCB MAX,," + (dPcbVal + CSq_OnP.It.m_tTarget.dAfMax).ToString();
                            sData = "PCB MAX,," + (dPcbVal + CData.Parts[(int)EPart.GRDR].dTopMoldMax).ToString();//220111 pjh : DF Server 사용 시 Top Mold Thickness 저장 변수 변경
                            _SetLog("Top PCB Thickness = " + dPcbVal.ToString() + ", Top Mold Thickness = " + CData.Parts[(int)EPart.GRDR].dTopMoldMax.ToString());//220113 pjh : Data 확인용 Log
                        }
                    }
                    else
                    {
                        if (eWy == EWay.L)
                        {
                            dPcbVal = CData.Parts[(int)EPart.GRDL].dPcbMean;
                            //sData = "PCB MEAN,," + (dPcbVal + CSq_OnP.It.m_tTarget.dAfAvg).ToString();
                            sData = "PCB MAX,," + (dPcbVal + CData.Parts[(int)EPart.GRDL].dTopMoldAvg).ToString();//220111 pjh : DF Server 사용 시 Top Mold Thickness 저장 변수 변경
                            _SetLog("Top PCB Thickness = " + dPcbVal.ToString() + ", Top Mold Thickness = " + CData.Parts[(int)EPart.GRDL].dTopMoldAvg.ToString());//220113 pjh : Data 확인용 Log
                        }
                        else
                        {
                            dPcbVal = CData.Parts[(int)EPart.GRDR].dPcbMean;
                            //sData = "PCB MEAN,," + (dPcbVal + CSq_OnP.It.m_tTarget.dAfAvg).ToString();
                            sData = "PCB MAX,," + (dPcbVal + CData.Parts[(int)EPart.GRDR].dTopMoldAvg).ToString();//220111 pjh : DF Server 사용 시 Top Mold Thickness 저장 변수 변경
                            _SetLog("Top PCB Thickness = " + dPcbVal.ToString() + ", Top Mold Thickness = " + CData.Parts[(int)EPart.GRDR].dTopMoldAvg.ToString());//220113 pjh : Data 확인용 Log
                        }
                    }
                }
                //
                else
                {
                    double[] dTmpPcb = new double[CData.Dev.iDynamicPosNum];
                    for (int i = 0; i < CData.Dev.iDynamicPosNum; i++)
                    {
                        dTmpPcb[i] = dPcb[i]; //배열 아이템 수는 최대 5개지만 실제 측정 포인트 수는 3~5로 유동적이므로 별도 배열 할당 후 Max, Avg 값을 구함
                    }

                    sData = "PCB Height";
                    for (int i = 0; i < CData.Dev.iDynamicPosNum; i++)
                    {
                        sData += ",," + dTmpPcb[i].ToString();
                    }
                    sMsgWrite += sData + "\n";
                    if (CData.Dynamic.iHeightType == 0)
                    {
                        sData = "PCB MAX,," + dTmpPcb.Max().ToString();
                        dPcbVal = dTmpPcb.Max();
                    }
                    else
                    {
                        sData = "PCB MEAN,," + Math.Round(dTmpPcb.Average(), 4).ToString();
                        dPcbVal = Math.Round(dTmpPcb.Average(), 4);
                    }
                }
#else
                sData = "PCB Height,," + dPcb[0] + ",," + dPcb[1] + ",," + dPcb[2];
                sMsgWrite += sData +"\n";

                if (CData.Dynamic.iHeightType == 0)
                {
                    sData = "PCB MAX,," + dPcb.Max(); 
                    dPcbVal = dPcb.Max(); //190625 ksg :
                }
                else
                {
                    sData = "PCB MEAN,," + Math.Round(dPcb.Average(), 4);
                    dPcbVal = Math.Round(dPcb.Average(), 4);
                }
#endif
                sMsgWrite += sData +"\n";
            }

            for (int iRow = 0; iRow < dData.GetLength(0); iRow++)
            {
                sData = "";
                for (int iCol = 0; iCol < dData.GetLength(1); iCol++)
                {
                    if (dData[iRow, iCol] == 0 || dData[iRow, iCol] == 999.999)
                    { sData += "."; }
                    else
                    { sData += dData[iRow, iCol].ToString("F3"); }

                    if ((iCol + 1) < dData.GetLength(1))
                    { sData += ","; }
                }

                //190709 ksg :
                if(!CData.Dev.bDynamicSkip || (CDataOption.UseDFDataServer && CData.Dev.eMoldSide == ESide.Btm && !CData.Opt.bSecsUse))
                {
                    sData += ",,,,";

                    for (int iCol = 0; iCol < dData.GetLength(1); iCol++)
                    {
                        if (dData[iRow, iCol] == 0 || dData[iRow, iCol] == 999.999)
                        { sData += "."; }
                        else
                        {
                            double Temp = dData[iRow, iCol] + dPcbVal;
                            sData += Temp.ToString("F3");
                        }

                        if ((iCol + 1) < dData.GetLength(1))
                        { sData += ","; }
                    }
                }
                sMsgWrite += sData +"\n";
            }
            
            CLog.Check_File_Access(sPath,sMsgWrite,true);//koo 191002 : Speed Write

            // 200820 jym : Network drive 설정
            // 210601 jym : 네트워크 드라이브 스레드 추가
            if (CData.Opt.bNetUse)
            {
                StringBuilder mPath = new StringBuilder(@"\\");
                mPath.Append(CData.Opt.sNetIP);
                mPath.Append(CData.Opt.sNetPath);
                mPath.Append(@"SPC\LotLog\");
                mPath.Append(DateTime.Now.ToString("yyyyMMdd"));
                mPath.Append("\\");
                mPath.Append(sLotName);
                mPath.Append("\\");
                mPath.Append(string.Format("Grind{0}.csv", (eWy == EWay.L) ? "Left" : "Right"));

                CData.QueNet.Enqueue(new TNetDrive(mPath.ToString(), sMsgWrite));
            }

            return true;
        }

        // 2020.09.14 JSKim St - 측정한 자재 총 높이로 남겨달라는 요청
        /// <summary>
        /// 오토러닝 그라인딩 자재 랏기준 측정 데이터 Csv 파일로 저장
        /// 
        /// Measure => BEFORE / AFTER
        /// TABLE => LEFT / RIGHT
        /// MGZ ID => XX
        /// SLOT ID => XX
        /// TARGET => X.XXX
        /// Finish Time => yyyy_MM_dd HH:mm:ss
        /// Wheel Serial => xxxxxxxxx    Wheel Height => x.xxxx
        /// 자재 측정 값
        /// 측정 안할 경우 셀 '.' / 측정 할 경우 'x.xxx' (소수점 3자리 표시)     
        /// 
        /// 경로 : D:\Spc\GrdData\GrindLeft of GrindRight\yyyy\MM\dd.csv
        /// </summary>
        /// <param name="eWy"> 테이블 왼쪽 or 오른쪽</param>
        /// <param name="sBfAf"> BEFORE or AFTER</param>
        /// <param name="dData"> 측정 데이터 </param>
        /// <param name="dTotalData"> 자제 총 높이 측정 데이터 </param>
        /// <returns></returns>
        public bool SaveLotDataCsv(ref TLotInfo pLotInfo, EWay eWy, string sBfAf, double[,] dData, double[] dPcb, double[,] dTotalData)
        {
            string sPath        = GV.PATH_SPC + "LotLog\\";
            string sData        = "";
            string sTable       = "";
            string sMgzId       = "";
            string sSlotId      = "";
            string sTarget      = "";
            string sWheel       = "";
            string sWhHeight    = "";
            string sBcr         = "";
            string sLotName     = "";
            
            string sGroup       = "";
            string sDevice      = "";

            //201225-1 jhc : 고급 Grind Condition 설정/체크 기능 (Spindle Current, Table Vacuum Low Limit 설정)
            int iWy = (int)eWy;
            string sMaxSpindleCurrent = CData.GrData[iWy].nSpindleCurrentMax.ToString();
            string sMinTableVacuum = CData.GrData[iWy].dTableVacuumMin.ToString("0.0");
            //..

            double dPcbVal  = 0.0; //190625 ksg :
            string sMsgWrite = "";

            DirectoryInfo di;
            FileInfo fi;

            if (CData.CurCompany == ECompany.ASE_K12)   sLotName = CData.LotInfo.sNewLotName;   //211213 pjh : 저장하는 Lot Name 변경
            else                                        sLotName = pLotInfo.sLotName;
            sLotName += "_" + pLotInfo.dtOpen.ToString("HH");

            sPath += DateTime.Now.ToString("yyyyMMdd") + "\\";
            sPath += sLotName + "\\";

            di = new DirectoryInfo(sPath);
            if (di.Exists == false)
            {
                di.Create();
            }

            if (eWy == EWay.L)
            {
                sPath   += "GrindLeft.csv";
                sTable  = "LEFT";
                sMgzId  = CData.Parts[(int)EPart.GRDL].iMGZ_No.ToString();
                sSlotId = CData.Parts[(int)EPart.GRDL].iSlot_No.ToString();

                for (int i = iStepMaxCnt - 1; i >= 0; i--)    
                {
                    if (CData.Dev.aData[0].aSteps[i].bUse)
                    {
                        sTarget = CData.GrData[(int)eWy].aTar[i].ToString();
                        break;
                    }
                }
                sWheel      = CData.Whls[0].sWhlName;
                sWhHeight   = CData.WhlAf[0].ToString();
                sBcr        = CData.Parts[(int)EPart.GRDL].sBcr;
                //190704 myk_GrpDevName
                sGroup      = new DirectoryInfo(Path.GetDirectoryName(CData.DevCur)).Name;
                sDevice     = CData.Dev.sName;
            }
            else    // Right 
            {
                sPath   += "GrindRight.csv";
                sTable  = "RIGHT";
                sMgzId  = CData.Parts[(int)EPart.GRDR].iMGZ_No.ToString();
                sSlotId = CData.Parts[(int)EPart.GRDR].iSlot_No.ToString();

                for (int i = iStepMaxCnt - 1; i >= 0; i--)
                {
                    if (CData.Dev.bDual == eDual.Dual)
                    {
                        if (CData.Dev.aData[1].aSteps[i].bUse)
                        {
                            sTarget = CData.GrData[(int)eWy].aTar[i].ToString();
                            break;
                        }
                    }
                    else
                    {
                        if (CData.Dev.aData[0].aSteps[i].bUse)
                        {
                            sTarget = CData.GrData[(int)eWy].aTar[i].ToString();
                            break;
                        }
                    }
                }

                sWheel      = CData.Whls[1].sWhlName;
                sWhHeight   = CData.WhlAf[1].ToString();

                sBcr        = CData.Parts[(int)EPart.GRDR].sBcr;
                //190704 myk_GrpDevName
                sGroup = new DirectoryInfo(Path.GetDirectoryName(CData.DevCur)).Name;
                sDevice = CData.Dev.sName;
            }
            //190718 ksg :
            if (sBfAf == "BEFORE")  {   sTarget = "0";  }

            fi = new FileInfo(sPath);
            if (fi.Exists == false)
            {
                fi.Create().Close();
            }

            sMsgWrite += "==================================================================================================\n";
            sData = "Measure,"      + sBfAf;                                        sMsgWrite += sData + "\n";
            sData = "TABLE,"        + sTable;                                       sMsgWrite += sData + "\n";
            sData = "LotId,"        + pLotInfo.sLotName;                            sMsgWrite += sData + "\n";
            sData = "GROUP,"        + sGroup;                                       sMsgWrite += sData + "\n";
            sData = "DEVICE,"       + sDevice;                                      sMsgWrite += sData + "\n";
            sData = "MGZ ID,"       + sMgzId;                                       sMsgWrite += sData + "\n";
            sData = "SLOT ID,"      + sSlotId;                                      sMsgWrite += sData + "\n";
            sData = "TARGET,"       + sTarget;                                      sMsgWrite += sData + "\n";
            sData = "BARCODE,"      + sBcr;                                         sMsgWrite += sData + "\n";
            sData = "Finish Time,"  + DateTime.Now.ToString("yyyy_MM_dd HH:mm:ss"); sMsgWrite += sData + "\n";
            sData = "Wheel Serial,," + sWheel + ",," + sWhHeight;                   sMsgWrite += sData + "\n";

            //201225-1 jhc : 고급 Grind Condition 설정/체크 기능 (Spindle Current, Table Vacuum Low Limit 설정)
            //if (CDataOption.UseDeviceAdvancedOption && CDataOption.UseAdvancedGrindCondition && (CDataOption.SplType == eSpindleType.EtherCat))
            // 2023.03.15 Max
            if (CDataOption.UseDeviceAdvancedOption && CDataOption.UseAdvancedGrindCondition)
            {
                //Advanced Grind Condition 체크 사용 조건(라이선스)에서 추가 정보 남김
                sData = "Max Spindle Current,"  + sMaxSpindleCurrent;                   sMsgWrite += sData +"\n";
                sData = "Min Table Vacuum,"     + sMinTableVacuum;                      sMsgWrite += sData +"\n";
            }
            //..
            if (sBfAf == "BEFORE")
            {
                sData = "Grind Cycle Time,," + "0";                                     sMsgWrite += sData + "\n";
            }
            else
            {
                sData = "Grind Cycle Time,," + CData.GrdElp[(int)eWy].tsEls.ToString(); sMsgWrite += sData + "\n";

                //20191111 ghk_regrindinglog
                if (CDataOption.ReGrdLog == eReGrdLog.Use)
                {
                    sData = "StepNo, ReGrinding No, ReGrinding Cnt" + "\n";
                    
                    for (int iStep = 0; iStep < iStepMaxCnt - 1; iStep++)
                    {
                        if (CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)  
                        {
                            if (CData.RedGrdCnt[(int)eWy, iStep].m_lRedCnt.Count > 0)
                            {
                                for (int iReCnt = 0; iReCnt < CData.RedGrdCnt[(int)eWy, iStep].m_lRedCnt.Count; iReCnt++)
                                {
                                    sData += (iStep + 1).ToString() + "," + (iReCnt + 1).ToString() + "," + CData.RedGrdCnt[(int)eWy, iStep].m_lRedCnt[iReCnt].ToString() + "\n";
                                }
                            }
                        }
                        else
                        {
                            if (CData.RedGrdCnt3[(int)eWy, iStep].m_lRedCnt.Count > 0)
                            {
                                for (int iReCnt = 0; iReCnt < CData.RedGrdCnt3[(int)eWy, iStep].m_lRedCnt.Count; iReCnt++)
                                {
                                    sData += (iStep + 1).ToString() + "," + (iReCnt + 1).ToString() + "," + CData.RedGrdCnt3[(int)eWy, iStep].m_lRedCnt[iReCnt].ToString() + "\n";
                                }
                            }
                        }
                    }
                    sMsgWrite += sData;
                }
                // 
            }

            if (!CData.Dev.bDynamicSkip && CDataOption.MeasureDf == eDfServerType.MeasureDf)
            {//다이나믹 펑션 사용
#if true //20200328 jhc : 다이나믹 펑션 측정 포인트 최대 5개로 확대
                double[] dTmpPcb = new double[CData.Dev.iDynamicPosNum];
                for (int i = 0; i < CData.Dev.iDynamicPosNum; i++)
                {
                    dTmpPcb[i] = dPcb[i]; //배열 아이템 수는 최대 5개지만 실제 측정 포인트 수는 3~5로 유동적이므로 별도 배열 할당 후 Max, Avg 값을 구함
                }

                sData = "PCB Height";
                for (int i = 0; i < CData.Dev.iDynamicPosNum; i++)
                {
                    sData += ",," + dTmpPcb[i].ToString();
                }
                sMsgWrite += sData + "\n";

                if (CData.Dynamic.iHeightType == 0)
                {
                    sData = "PCB MAX,," + dTmpPcb.Max().ToString();
                    dPcbVal = dTmpPcb.Max();
                }
                else
                {
                    sData = "PCB MEAN,," + Math.Round(dTmpPcb.Average(), 4).ToString();
                    dPcbVal = Math.Round(dTmpPcb.Average(), 4);
                }

#else
                sData = "PCB Height,," + dPcb[0] + ",," + dPcb[1] + ",," + dPcb[2];
                sMsgWrite += sData +"\n";

                if (CData.Dynamic.iHeightType == 0)
                {
                    sData = "PCB MAX,," + dPcb.Max(); 
                    dPcbVal = dPcb.Max(); //190625 ksg :
                }
                else
                {
                    sData = "PCB MEAN,," + Math.Round(dPcb.Average(), 4);
                    dPcbVal = Math.Round(dPcb.Average(), 4);
                }
#endif
                sMsgWrite += sData + "\n";
            }

            for (int iRow = 0; iRow < dData.GetLength(0); iRow++)
            {
                sData = "";
                for (int iCol = 0; iCol < dData.GetLength(1); iCol++)
                {
                    if (dData[iRow, iCol] == 0 || dData[iRow, iCol] == 999.999)
                    { sData += "."; }
                    else
                    { sData += dData[iRow, iCol].ToString("F3"); }

                    if ((iCol + 1) < dData.GetLength(1))
                    { sData += ","; }
                }

                sData += ",,,,";

                for (int iCol = 0; iCol < dData.GetLength(1); iCol++)
                {
                    if (dTotalData[iRow, iCol] == 0 || dTotalData[iRow, iCol] == 999.999)
                    { sData += "."; }
                    else
                    {
                        double Temp = dTotalData[iRow, iCol];
                        sData += Temp.ToString("F3");
                    }

                    if ((iCol + 1) < dData.GetLength(1))
                    { sData += ","; }
                }
                sMsgWrite += sData + "\n";
            }

            CLog.Check_File_Access(sPath, sMsgWrite, true);//koo 191002 : Speed Write

            // 200820 jym : Network drive 설정
            // 210601 jym : 네트워크 드라이브 스레드 추가
            if (CData.Opt.bNetUse)
            {
                StringBuilder mPath = new StringBuilder(@"\\");
                mPath.Append(CData.Opt.sNetIP);
                mPath.Append(CData.Opt.sNetPath);
                mPath.Append(@"SPC\LotLog\");
                mPath.Append(DateTime.Now.ToString("yyyyMMdd"));
                mPath.Append("\\");
                mPath.Append(sLotName);
                mPath.Append("\\");
                mPath.Append(string.Format("Grind{0}.csv", (eWy == EWay.L) ? "Left" : "Right"));

                CData.QueNet.Enqueue(new TNetDrive(mPath.ToString(), sMsgWrite));
            }

            return true;
        }
        // 2020.09.14 JSKim Ed

        public bool SaveLotDataCsvU(ref TLotInfo pLotInfo, EWay eWy, string sBfAf)
        {
            int iWy = (int)eWy;
            double dVal  =0;
            string sPath = GV.PATH_SPC + "LotLog\\";
            string sData     = "";
            string sTable    = "";
            string sMgzId    = "";
            string sSlotId   = "";
            string sTarget   = "";
            string sWheel    = "";
            string sWhHeight = "";
            string sBcr      = "";
            string sLotName  = "";
            //190704 myk_GrpDevName
            string sGroup = "";
            string sDevice = "";

            //201225-1 jhc : 고급 Grind Condition 설정/체크 기능 (Spindle Current, Table Vacuum Low Limit 설정)
            string sMaxSpindleCurrent = CData.GrData[iWy].nSpindleCurrentMax.ToString();
            string sMinTableVacuum = CData.GrData[iWy].dTableVacuumMin.ToString("0.0");
            //..

            string sMsgWrite = "";//koo 191002 : Speed Write

            DirectoryInfo di;
            FileInfo fi;

            if (CData.CurCompany == ECompany.ASE_K12)   sLotName = CData.LotInfo.sNewLotName;//211213 pjh : 저장하는 Lot Name 변경
            else                                        sLotName = pLotInfo.sLotName;
            
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            {
                sLotName += "_" + pLotInfo.dtOpen.ToString("HH");
            }
            
            sPath += DateTime.Now.ToString("yyyyMMdd") + "\\";
            sPath += sLotName + "\\";

            // 2020.09.08 SungTae : 3 Step 기능 추가
            if (CDataOption.StepCnt == (int)EStepCnt.DEFAULT_STEP)  { iStepMaxCnt = GV.StepMaxCnt;      }
            else                                                    { iStepMaxCnt = GV.StepMaxCnt_3;    }

            di = new DirectoryInfo(sPath);

            if (di.Exists == false)
            {
                di.Create();
            }

            if (eWy == EWay.L)
            {
                sPath += "GrindLeft.csv";
                sTable = "LEFT";
                sMgzId = CData.Parts[(int)EPart.GRDL].iMGZ_No.ToString();
                sSlotId = CData.Parts[(int)EPart.GRDL].iSlot_No.ToString();

                //for (int i = GV.StepMaxCnt - 1; i >= 0; i--)
                for (int i = iStepMaxCnt - 1; i >= 0; i--)      // 2020.09.08 SungTae : Modify
                {
                    if (CData.Dev.aData[0].aSteps[i].bUse)
                    {
                        sTarget = CData.GrData[(int)eWy].aTar[i].ToString();
                        break;
                    }
                }
                sWheel = CData.Whls[0].sWhlName;
                sWhHeight = CData.WhlAf[0].ToString();
                sBcr = CData.Parts[(int)EPart.GRDL].sBcr;
                //190704 myk_GrpDevName
                sGroup = new DirectoryInfo(Path.GetDirectoryName(CData.DevCur)).Name;
                sDevice = CData.Dev.sName;
            }
            else
            {
                sPath += "GrindRight.csv";
                sTable = "RIGHT";
                sMgzId = CData.Parts[(int)EPart.GRDR].iMGZ_No.ToString();
                sSlotId = CData.Parts[(int)EPart.GRDR].iSlot_No.ToString();

                //for (int i = GV.StepMaxCnt - 1; i >= 0; i--)
                for (int i = iStepMaxCnt - 1; i >= 0; i--)      // 2020.09.08 SungTae : Modify
                {
                    if (CData.Dev.bDual == eDual.Dual)
                    {
                        if (CData.Dev.aData[1].aSteps[i].bUse)
                        {
                            sTarget = CData.GrData[(int)eWy].aTar[i].ToString();
                            break;
                        }
                    }
                    else
                    {
                        if (CData.Dev.aData[0].aSteps[i].bUse)
                        {
                            sTarget = CData.GrData[(int)eWy].aTar[i].ToString();
                            break;
                        }
                    }
                }

                sWheel = CData.Whls[1].sWhlName;
                sWhHeight = CData.WhlAf[1].ToString();

                sBcr = CData.Parts[(int)EPart.GRDR].sBcr;
                //190704 myk_GrpDevName
                sGroup = new DirectoryInfo(Path.GetDirectoryName(CData.DevCur)).Name;
                sDevice = CData.Dev.sName;
            }
            //190718 ksg :
            if (sBfAf == "BEFORE")
            {
                sTarget = "0";
            }
            else
            {
                for (int i = iStepMaxCnt - 1; i >= 0; i--)
                {
                    if (CData.Dev.aData[iWy].aSteps[i].bUse)
                    {
                        sTarget = CData.GrData[iWy].aTar[i].ToString();
                        break;
                    }
                }
            }

            fi = new FileInfo(sPath);

            if (fi.Exists == false)
            {
                fi.Create().Close();
            }
           
            sMsgWrite += "==================================================================================================\n";
            sData = "Measure," + sBfAf;
            sMsgWrite += sData + "\n";

            sData = "TABLE," + sTable;
            sMsgWrite += sData + "\n";

            sData = "LotId," + pLotInfo.sLotName;    //old  CData.LotInfo.sLotName;
            sMsgWrite += sData + "\n";

            //190704 myk_GrpDevName
            sData = "GROUP," + sGroup;
            sMsgWrite += sData + "\n";

            sData = "DEVICE," + sDevice;
            sMsgWrite += sData + "\n";

            sData = "MGZ ID," + sMgzId;
            sMsgWrite += sData + "\n";

            sData = "SLOT ID," + sSlotId;
            sMsgWrite += sData + "\n";

            sData = "TARGET," + sTarget;
            sMsgWrite += sData + "\n";

            sData = "BARCODE," + sBcr;
            sMsgWrite += sData + "\n";

            sData = "Finish Time," + DateTime.Now.ToString("yyyy_MM_dd HH:mm:ss");
            sMsgWrite += sData + "\n";

            sData = "Wheel Serial,," + sWheel + ",," + sWhHeight;
            sMsgWrite += sData + "\n";

            //201225-1 jhc : 고급 Grind Condition 설정/체크 기능 (Spindle Current, Table Vacuum Low Limit 설정)
            //if (CDataOption.UseDeviceAdvancedOption && CDataOption.UseAdvancedGrindCondition && (CDataOption.SplType == eSpindleType.EtherCat))
            // 2023.03.15 Max
            if (CDataOption.UseDeviceAdvancedOption && CDataOption.UseAdvancedGrindCondition)
            {
                //Advanced Grind Condition 체크 사용 조건(라이선스)에서 추가 정보 남김
                sData = "Max Spindle Current," + sMaxSpindleCurrent;
                sMsgWrite += sData +"\n";
                sData = "Min Table Vacuum," + sMinTableVacuum;
                sMsgWrite += sData +"\n";
            }
            //..

            //190718 ksg :
            if (sBfAf == "BEFORE")
            {
                sData = "Grind Cycle Time,," + "0";
                sMsgWrite += sData + "\n";
            }
            else
            {
                sData = "Grind Cycle Time,," + CData.GrdElp[iWy].tsEls.ToString();
                sMsgWrite += sData + "\n";

                //20191111 ghk_regrindinglog
                if (CDataOption.ReGrdLog == eReGrdLog.Use)
                {
                    sData = "StepNo, ReGrinding No, ReGrinding Cnt" + "\n";
                    //200414 ksg : 12 Step  기능 추가
                    //for (int iStep = 0; iStep < 4; iStep++)
                    //for (int iStep = 0; iStep < GV.StepMaxCnt - 1; iStep++)
                    for (int iStep = 0; iStep < iStepMaxCnt - 1; iStep++)       // 2020.09.08 SungTae : Modify
                    {
                        if (CData.RedGrdCnt[iWy, iStep].m_lRedCnt.Count > 0)
                        {
                            for (int iReCnt = 0; iReCnt < CData.RedGrdCnt[iWy, iStep].m_lRedCnt.Count; iReCnt++)
                            {
                                sData += (iStep + 1).ToString() + "," + (iReCnt + 1).ToString() + "," + CData.RedGrdCnt[iWy, iStep].m_lRedCnt[iReCnt].ToString() + "\n";
                            }
                        }
                    }
                    sMsgWrite += sData;
                }
            }

            for (int iRow = 0; iRow < CData.Dev.iRow; iRow++)
            {
                sData = "";

                for (int iU = 0; iU < CData.Dev.iUnitCnt; iU++)
                {
                    for (int iCol = 0; iCol < CData.Dev.iCol; iCol++)
                    {
                        if (sBfAf == "BEFORE")
                        { dVal = CData.GrData[iWy].aUnit[iU].aMeaBf[iRow, iCol]; }
                        else
                        { dVal = CData.GrData[iWy].aUnit[iU].aMeaAf[iRow, iCol]; }

                        if (dVal == 0 || dVal == 999.999) //190813 ksg : 변경
                        { sData += "."; }
                        else
                        {
                            sData += dVal.ToString("F3");
                        }

                        if ((iU + 1 == CData.Dev.iUnitCnt) && iCol + 1 == CData.Dev.iCol)
                        { }//sData += "\n"; }
                        else
                        { sData += ","; }
                    }

                    if (iU + 1 != CData.Dev.iUnitCnt)
                    { sData += "l,"; }
                }

                sMsgWrite += sData + "\n";
            }

            CLog.Check_File_Access(sPath, sMsgWrite, true);//koo 191002 : Speed Write

            // 200820 jym : Network drive 설정
            // 210601 jym : 네트워크 드라이브 스레드 추가
            if (CData.Opt.bNetUse)
            {
                StringBuilder mPath = new StringBuilder(@"\\");
                mPath.Append(CData.Opt.sNetIP);
                mPath.Append(CData.Opt.sNetPath);
                mPath.Append(@"SPC\LotLog\");
                mPath.Append(DateTime.Now.ToString("yyyyMMdd"));
                mPath.Append("\\");
                mPath.Append(sLotName);
                mPath.Append("\\");
                mPath.Append(string.Format("Grind{0}.csv", (eWy == EWay.L) ? "Left" : "Right"));

                CData.QueNet.Enqueue(new TNetDrive(mPath.ToString(), sMsgWrite));
            }

            return true;
        }

        /// <summary>
        /// 오토러닝 그라인딩 자재 측정 데이터 ini 파일로 저장
        /// [DATA]
        /// R00= _ 0.XXX_ 0.XXX_ (해당 위치에 데이터 없을 경우 '_'로 표시 측정 값은 소수점 3자리까지 표시)
        /// R01= _ 0.XXX_ 0.XXX_
        /// [ETC]
        /// Row=XX  (유저가 설정한 측정 max Row 값)
        /// Col=XX  (유저가 설정한 측정 max Column 값)
        /// 경로 : D:\Spc\LotLog\yyyyMMdd\LotName\L or R\01_01_Grind_Before.ini or 01_01_Grind_After.ini    (MgzNo_SlotNo_Grind_Before.ini)
        /// </summary>
        /// <param name="eWy"> 테이블 왼쪽 or 오른쪽</param>
        /// <param name="sBfAf"> BEFORE or AFTER</param>
        /// <param name="dData"> 측정 데이터</param>
        /// <returns></returns>
        public bool SaveDataIni(ref TLotInfo pLotInfo, EWay eWy, string sBfAf, double[,] dData)
        {
            int iRow    = 0;
            int iCol    = 0;
            int iPart   = (eWy == EWay.L) ? (int)EPart.GRDL : (int)EPart.GRDR;
            string sMgzNo   = "";
            string sSlotNo  = "";
            string sLotName = "";
            string sSec     = "";
            string sKey     = "";
            string sData    = "";
            string sBcr     = "";
            StringBuilder mPath = new StringBuilder(GV.PATH_SPC);


            if (CData.CurCompany == ECompany.ASE_K12)   sLotName = CData.LotInfo.sNewLotName;//211213 pjh : 저장하는 Lot Name 변경
            else                                        sLotName = pLotInfo.sLotName;
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            {
                sLotName += "_" + pLotInfo.dtOpen.ToString("HH"); //20200827 lks
            }
			
            mPath.Append("LotLog\\");
            mPath.Append(DateTime.Now.ToString("yyyyMMdd"));
            mPath.Append("\\");
            mPath.Append(sLotName);
            mPath.Append("\\");
            mPath.Append((eWy == EWay.L) ? "L\\" : "R\\");

            sMgzNo  = string.Format("{0:00}", CData.Parts[iPart].iMGZ_No); 
			sSlotNo = string.Format("{0:00}", CData.Parts[iPart].iSlot_No);
			sBcr    = CData.Parts[iPart].sBcr;

			if (!Directory.Exists(mPath.ToString()))
            { Directory.CreateDirectory(mPath.ToString()); }
            /// 01_01_GRIND_BEFORE.ini
            mPath.Append(sMgzNo);
            mPath.Append("_");
            mPath.Append(sSlotNo);
            mPath.Append("_GRIND_");
            mPath.Append(sBfAf);
            mPath.Append(".ini");

            sSec = "[Data]" + Environment.NewLine; 
            string temp ="";
            temp += sSec;

            int iRowCnt = dData.GetLength(0);
            int iColCnt = dData.GetLength(1);
            for (iRow = 0; iRow < iRowCnt; iRow++)
            {
                sKey = "R" + string.Format("{0:00}", iRow);

                for (iCol = 0; iCol < iColCnt; iCol++)
                {
                    if (dData[iRow, iCol] == 0 || dData[iRow, iCol] == 999.999) { sData += " "; }
                    else                                                        { sData += dData[iRow, iCol].ToString("F3"); }
                    sData += "_";
                }
                temp += sKey + "=" + sData + Environment.NewLine;
                sData = "";
            }

            sSec = "[ETC]";
            temp += sSec + Environment.NewLine;
            temp += "Row=" + iRow.ToString() +Environment.NewLine;
            temp += "Col=" + iCol.ToString() +Environment.NewLine;
            temp += "BARCODE=" + sBcr +Environment.NewLine;
            CLog.Check_File_Access(mPath.ToString(),temp,false);

            // 200820 jym : Network drive 설정
            // 210601 jym : 네트워크 드라이브 스레드 추가
            if (CData.Opt.bNetUse)
            {
                mPath = new StringBuilder(@"\\");
                mPath.Append(CData.Opt.sNetIP);
                mPath.Append(CData.Opt.sNetPath);
                mPath.Append(@"SPC\LotLog\");
                mPath.Append(DateTime.Now.ToString("yyyyMMdd"));
                mPath.Append("\\");
                mPath.Append(sLotName);
                mPath.Append("\\");
                mPath.Append((eWy == EWay.L) ? "L" : "R");
                mPath.Append("\\");
                mPath.Append(sMgzNo + "_" + sSlotNo + "_GRIND_" + sBfAf + ".ini");

                CData.QueNet.Enqueue(new TNetDrive(mPath.ToString(), temp));
            }

            return true;
        }

        public bool SaveDataIniU(ref TLotInfo pLotInfo, EWay eWy, string sBfAf)
        {
            int iRow = 0;
            int iCol = 0;
            int iWy = (int)eWy;
            int iPart = (eWy == EWay.L) ? (int)EPart.GRDL : (int)EPart.GRDR;
            double dVal = 0;
            string sPath = "";
            string sMgzNo = "";
            string sSlotNo = "";
            string sLot = "";
            string sBcr = "";
            string sData = "";

            DirectoryInfo mDir;
            FileInfo mFile;
            StringBuilder mSB = new StringBuilder();

            sLot = pLotInfo.sLotName;   // old CData.LotInfo.sLotName;
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            {
                sLot += "_" + pLotInfo.dtOpen.ToString("HH"); //20200827 lks
            }

            sPath   = GV.PATH_SPC + "LotLog\\";
            sPath   += DateTime.Now.ToString("yyyyMMdd") + "\\";
            sPath   += sLot + "\\";
            sPath   += (eWy == EWay.L) ? "L\\" : "R\\";
            sMgzNo  = string.Format("{0:00}", CData.Parts[iPart].iMGZ_No);
            sSlotNo = string.Format("{0:00}", CData.Parts[iPart].iSlot_No);
            sBcr    = CData.Parts[iPart].sBcr;

            mDir = new DirectoryInfo(sPath);

            if (!mDir.Exists)
            { mDir.Create(); }
            //01_01_Grind_Before.ini
            sPath += sMgzNo + "_" + sSlotNo + "_GRIND_" + sBfAf + ".ini";

            mFile = new FileInfo(sPath);

            if (!mFile.Exists)
            { mFile.Create().Close(); }

            for (int iU = 0; iU < CData.Dev.iUnitCnt; iU++)
            {
                mSB.AppendLine("[Unit " + (iU + 1) + " Data]");

                for (iRow = 0; iRow < CData.Dev.iRow; iRow++)
                {
                    mSB.Append("R" + string.Format("{0:00}", iRow));

                    for (iCol = 0; iCol < CData.Dev.iCol; iCol++)
                    {
                        if (sBfAf == "BEFORE")
                        { dVal = CData.GrData[iWy].aUnit[iU].aMeaBf[iRow, iCol]; }
                        else
                        { dVal = CData.GrData[iWy].aUnit[iU].aMeaAf[iRow, iCol]; }

                        if (dVal == 0 || dVal == 999.999) //190813 ksg : 변경
                        { sData += " "; }
                        else
                        { sData += dVal.ToString("F3"); }
                        sData += "_";
                    }

                    mSB.Append("=");
                    mSB.AppendLine(sData);
                    sData = "";
                }

                mSB.AppendLine();
            }

            mSB.AppendLine();
            mSB.AppendLine("[ETC]");
            mSB.AppendLine("Row=" + iRow.ToString());
            mSB.AppendLine("Col=" + iCol.ToString());
            mSB.AppendLine("BARCODE=" + sBcr);
            CLog.Check_File_Access(sPath, mSB.ToString(), false);

            // 200820 jym : Network drive 설정
            // 210601 jym : 네트워크 드라이브 스레드 추가
            if (CData.Opt.bNetUse)
            {
                StringBuilder mPath = new StringBuilder(@"\\");
                mPath.Append(CData.Opt.sNetIP);
                mPath.Append(CData.Opt.sNetPath);
                mPath.Append(@"SPC\LotLog\");
                mPath.Append(DateTime.Now.ToString("yyyyMMdd"));
                mPath.Append("\\");
                mPath.Append(sLot);
                mPath.Append("\\");
                mPath.Append((eWy == EWay.L) ? "L" : "R");
                mPath.Append("\\");
                mPath.Append(sMgzNo + "_" + sSlotNo + "_GRIND_" + sBfAf + ".ini");

                CData.QueNet.Enqueue(new TNetDrive(mPath.ToString(), mSB.ToString()));
            }

            mSB.Clear();

            return true;
        }

        /// <summary>
        /// 오토러닝 LotInfo csv 파일로 저장 (LotEnd 시에 저장)
        /// Day, LotName, GrdMode, Device, Target, StartTime, EndTime, RunTime, IdleTime, JamTime, TotalTime, StripUPH, WorkStrip, WhLSerial, WhRSerial
        /// 경로 : D:\Spc\LotLog\yyyyMMdd\LotName\LotInfo.csv
        /// </summary>
        /// <returns></returns>
        public bool SaveLotInfo()
        {
            string sPath        = "";
            string sDay         = "";
            string sLotName     = "";
            string sGrdMode     = "";
            string sDevice      = "";
            string sTarget      = "";
            string sStartTime   = "";
            string sEndTime     = "";
            string sRunTime     = "";
            string sIdleTime    = "";
            string sJamTime     = "";
            string sTotalTime   = "";
            string sUph         = "";
            string sWorkStrip   = "";
            string sWhlSerial_L = "";
            string sWhlSerial_R = "";
            string sWriteLine   = "";
            // 2020.12.10 JSKim St
            string sErrorCount  = "";
            // 2020.12.10 JSKim Ed

            DirectoryInfo dI;
            FileInfo fI;
            //StreamWriter sw;

            //Lot Name
            if(CData.CurCompany == ECompany.ASE_K12)    sLotName = CData.LotInfo.sNewLotName;//211213 pjh : 저장하는 Lot Name 변경
            else                                        sLotName = CData.SpcInfo.sLotName;
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            {
                sLotName += "_" + CData.LotInfo.iLotOpenHour.ToString("00");
            }

            sPath = GV.PATH_SPC;
            sPath += "LotLog\\";
            sPath += DateTime.Now.ToString("yyyyMMdd") + "\\";
            sPath += sLotName + "\\";

            dI = new DirectoryInfo(sPath);
            if (!dI.Exists) { dI.Create(); }

            sPath += "LotInfo.csv";

            fI = new FileInfo(sPath);
            if (!fI.Exists)
            {
                fI.Create().Close();

                sWriteLine = "Day,";
                sWriteLine += "LotName,";
                sWriteLine += "GrdMode,";
                sWriteLine += "Device,";
                sWriteLine += "Target,";
                sWriteLine += "StarTime,";
                sWriteLine += "EndTime,";
                sWriteLine += "RunTime,";
                sWriteLine += "IdleTime,";
                sWriteLine += "JamTime,";
                sWriteLine += "TotalTime,";
                sWriteLine += "StripUPH,";
                sWriteLine += "WorkStrip,";
                sWriteLine += "WhL Serial,";
                sWriteLine += "WhR Serial,";
                // 2020.12.10 JSKim St
                if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                {
                    sWriteLine += "Error Count";
                }
                // 2020.12.10 JSKim Ed

                CLog.Check_File_Access(sPath, sWriteLine, true);

                // 200820 jym : Network drive 설정
                // 210601 jym : 네트워크 드라이브 스레드 추가
                if (CData.Opt.bNetUse)
                {
                    StringBuilder mPath = new StringBuilder(@"\\");
                    mPath.Append(CData.Opt.sNetIP);
                    mPath.Append(CData.Opt.sNetPath);
                    mPath.Append(@"SPC\LotLog\");
                    mPath.Append(DateTime.Now.ToString("yyyyMMdd"));
                    mPath.Append("\\");
                    mPath.Append(sLotName);
                    mPath.Append("\\");
                    mPath.Append("LotInfo.csv");

                    CData.QueNet.Enqueue(new TNetDrive(mPath.ToString(), sWriteLine));
                }
            }

            //현재 날짜
            sDay = DateTime.Now.ToString("yyMMdd");

            //그라인딩 모드
            if (CData.Dev.bDual == eDual.Normal)    { CData.SpcInfo.sGrdMode = "NOMAL"; }
            else                                    { CData.SpcInfo.sGrdMode = "STEP"; }
            sGrdMode = CData.SpcInfo.sGrdMode;

            //Device Name
            sDevice = CData.SpcInfo.sDevice;

            sTarget = CData.SpcInfo.sTarget;
            if (sTarget == "")  { sTarget = "0.0"; }

            //LotOpen 후 Start 시간, 현재 테스트 버젼으로 현재 시간 입력
            sStartTime = CData.SpcInfo.sStartTime;
            if (sStartTime  == "")  { sStartTime = "00:00:00";  }

            //LotEnd 시간, 현재 테스트 버젼으로 현재 시간 입력
            sEndTime = CData.SpcInfo.sEndTime;
            if (sEndTime    == "")  { sEndTime  = "00:00:00";   }

            //LotEnd 시간 - LotOpen 후 Start 시간
            CalTotalTime(sStartTime, sEndTime);
            sTotalTime = CData.SpcInfo.sTotalTime;
            if (sTotalTime  == "")  { sTotalTime = "00:00:00";  }

            //LotOpen 후 Start 시간 부터 LotEnd 시간 사이에서 총 STOP 시간, 현재 테스트 버젼으로 현재 시간 입력
            CData.SpcInfo.sIdleTime = CData.SwIdle.Elapsed.ToString(@"hh\:mm\:ss"); //190110 ksg : 수정
            sIdleTime = CData.SpcInfo.sIdleTime;
            if (sIdleTime   == "")  { sIdleTime = "00:00:00"; }

            //LotOpen 후 Start 시간 부터 LotEnd 시간 사이에서 총 error 시간, 현재 테스트 버젼으로 현재 시간 입력
            CData.SpcInfo.sJamTime = CData.SwErr.Elapsed.ToString(@"hh\:mm\:ss"); //190110 ksg : 수정
            sJamTime = CData.SpcInfo.sJamTime;
            if (sJamTime == "")     { sJamTime = "00:00:00"; }

            //LotOpen 후 Start 시간 부터 LotEnd 시간 사이에서 총 정상 동작 시간 : 총 시간 - Idle 시간 - Jam 시간
            CalRunTime();
            sRunTime = CData.SpcInfo.sRunTime;
            if (sRunTime == "")     { sRunTime = "00:00:00"; }

            //LotOpen 후 Start 부터 LotEnd 까지 작업한 Strip 장수, 현재 테스트 버젼으로 임의값 입력
            sWorkStrip  = CData.SpcInfo.iWorkStrip.ToString();

            //UPH WorkStrip / TotalTime(hour) = UPH 소수점 버리기
            CalUph();
            sUph        = CData.SpcInfo.sUph;

            //Wheel Left 시리얼
            sWhlSerial_L = CData.SpcInfo.sWhlSerial_L;

            //Wheel Right 시리얼
            sWhlSerial_R = CData.SpcInfo.sWhlSerial_R;

            // 2020.12.10 JSKim St
            // Lot Open ~ Lot End 사이 Error Count
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            {
                sErrorCount = CData.SpcInfo.iErrCnt.ToString();
            }
            // 2020.12.10 JSKim Ed

            sWriteLine = sDay           + ",";
            sWriteLine += sLotName      + ",";
            sWriteLine += sGrdMode      + ",";
            sWriteLine += sDevice       + ",";
            sWriteLine += sTarget       + ",";
            sWriteLine += sStartTime    + ",";
            sWriteLine += sEndTime      + ",";
            sWriteLine += sRunTime      + ",";
            sWriteLine += sIdleTime     + ",";
            sWriteLine += sJamTime      + ",";
            sWriteLine += sTotalTime    + ",";
            sWriteLine += sUph          + ",";
            sWriteLine += sWorkStrip    + ",";
            sWriteLine += sWhlSerial_L  + ",";
            sWriteLine += sWhlSerial_R  + ",";
            // 2020.12.10 JSKim St
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            {
                sWriteLine += sErrorCount;
            }
            // 2020.12.10 JSKim Ed

            CLog.Check_File_Access(sPath, sWriteLine, true);

            // 200820 jym : Network drive
            // 210601 jym : 네트워크 드라이브 스레드 추가
            if (CData.Opt.bNetUse)
            {
                StringBuilder mPath = new StringBuilder(@"\\");
                mPath.Append(CData.Opt.sNetIP);
                mPath.Append(CData.Opt.sNetPath);
                mPath.Append(@"SPC\LotLog\");
                mPath.Append(DateTime.Now.ToString("yyyyMMdd"));
                mPath.Append("\\");
                mPath.Append(sLotName);
                mPath.Append("\\");
                mPath.Append("LotInfo.csv");

                CData.QueNet.Enqueue(new TNetDrive(mPath.ToString(), sWriteLine));
            }

            ClearLotInfo();

            return true;
        }



        /// <summary>
        /// 2021-05-24, jhLee : Multi-LOT 사용시 
        /// 
        /// 지정 LOT의 SPC data를  LotInfo csv 파일로 저장 (LotEnd 시에 저장)
        /// Day, LotName, GrdMode, Device, Target, StartTime, EndTime, RunTime, IdleTime, JamTime, TotalTime, StripUPH, WorkStrip, WhLSerial, WhRSerial
        /// 경로 : D:\Spc\LotLog\yyyyMMdd\LotName\LotInfo.csv
        /// </summary>
        /// <returns></returns>
        public bool SaveLotInfo(ref TLotInfo pLotInfo)
        {
            string sPath = "";
            string sDay = "";
            string sLotName = "";
            string sGrdMode = "";
            string sDevice = "";
            string sTarget = "";
            string sStartTime = "";
            string sEndTime = "";
            string sRunTime = "";
            string sIdleTime = "";
            string sJamTime = "";
            string sTotalTime = "";
            string sUph = "";
            string sWorkStrip = "";
            string sWhlSerial_L = "";
            string sWhlSerial_R = "";
            string sWriteLine = "";
            string sErrorCount = "";

            DirectoryInfo dI;
            FileInfo fI;

            //Lot Name
            if (CData.CurCompany == ECompany.ASE_K12)   sLotName = CData.LotInfo.sNewLotName;   //211213 pjh : 저장하는 Lot Name 변경
            else                                        sLotName = pLotInfo.sLotName;
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            {
                sLotName += "_" + CData.LotInfo.dtOpen.ToString("HH");  // 舊 .iLotOpenHour.ToString("00"); //20200827 lks
            }

            sPath = GV.PATH_SPC;
            sPath += "LotLog\\";
            sPath += DateTime.Now.ToString("yyyyMMdd") + "\\";
            sPath += sLotName + "\\";

            dI = new DirectoryInfo(sPath);

            if (!dI.Exists)
            { dI.Create(); }

            sPath += "LotInfo.csv";

            fI = new FileInfo(sPath);
            if (!fI.Exists)
            {
                fI.Create().Close();

                sWriteLine = "Day,";
                sWriteLine += "LotName,";
                sWriteLine += "GrdMode,";
                sWriteLine += "Device,";
                sWriteLine += "Target,";
                sWriteLine += "StarTime,";
                sWriteLine += "EndTime,";
                sWriteLine += "RunTime,";
                sWriteLine += "IdleTime,";
                sWriteLine += "JamTime,";
                sWriteLine += "TotalTime,";
                sWriteLine += "StripUPH,";
                sWriteLine += "WorkStrip,";
                sWriteLine += "WhL Serial,";
                sWriteLine += "WhR Serial,";

                if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
                {
                    sWriteLine += "Error Count";
                }

                CLog.Check_File_Access(sPath, sWriteLine, true);

                // Network drive 설정
                if (CData.Opt.bNetUse)
                {
                    StringBuilder mPath = new StringBuilder(@"\\");
                    mPath.Append(CData.Opt.sNetIP);
                    int iRet = CNetDrive.Connect(mPath.ToString(), CData.Opt.sNetID, CData.Opt.sNetPw);
                    if (iRet == (int)ENetError.NO_ERROR)
                    {
                        mPath.Append(CData.Opt.sNetPath);
                        mPath.Append(@"SPC\LotLog\");
                        mPath.Append(DateTime.Now.ToString("yyyyMMdd"));
                        mPath.Append("\\");
                        mPath.Append(sLotName);
                        mPath.Append("\\");
                        if (!Directory.Exists(mPath.ToString()))
                        { Directory.CreateDirectory(mPath.ToString()); }
                        mPath.Append("LotInfo.csv");
                        CLog.Check_File_Access(mPath.ToString(), sWriteLine, true);

                        CNetDrive.Disconnect();
                    }
                }
            }

            //현재 날짜
            sDay = DateTime.Now.ToString("yyMMdd");

            //그라인딩 모드
            if (CData.Dev.bDual == eDual.Normal)        { pLotInfo.rSpcInfo.sGrdMode = "NOMAL"; }
            else                                        { pLotInfo.rSpcInfo.sGrdMode = "STEP";  }
            sGrdMode = pLotInfo.rSpcInfo.sGrdMode;

            //Device Name
            sDevice = pLotInfo.rSpcInfo.sDevice;

            sTarget = pLotInfo.rSpcInfo.sTarget;
            if (sTarget == "")      { sTarget = "0.0"; }

            //LotOpen 후 Start 시간, 현재 테스트 버젼으로 현재 시간 입력
            sStartTime = pLotInfo.rSpcInfo.sStartTime;
            if (sStartTime == "")   { sStartTime = "00:00:00"; }

            //LotEnd 시간, 현재 테스트 버젼으로 현재 시간 입력
            sEndTime = pLotInfo.rSpcInfo.sEndTime;
            if (sEndTime == "")     { sEndTime = "00:00:00"; }

            //LotEnd 시간 - LotOpen 후 Start 시간
            CalTotalTime(sStartTime, sEndTime);
            sTotalTime = pLotInfo.rSpcInfo.sTotalTime;
            if (sTotalTime == "")   { sTotalTime = "00:00:00"; }

            //LotOpen 후 Start 시간 부터 LotEnd 시간 사이에서 총 STOP 시간, 현재 테스트 버젼으로 현재 시간 입력
            pLotInfo.rSpcInfo.sIdleTime = CData.SwIdle.Elapsed.ToString(@"hh\:mm\:ss"); //190110 ksg : 수정
            sIdleTime = pLotInfo.rSpcInfo.sIdleTime;
            if (sIdleTime == "")    { sIdleTime = "00:00:00"; }

            //LotOpen 후 Start 시간 부터 LotEnd 시간 사이에서 총 error 시간, 현재 테스트 버젼으로 현재 시간 입력
            pLotInfo.rSpcInfo.sJamTime = CData.SwErr.Elapsed.ToString(@"hh\:mm\:ss"); //190110 ksg : 수정
            sJamTime = pLotInfo.rSpcInfo.sJamTime;
            if (sJamTime == "") { sJamTime = "00:00:00"; }

            //LotOpen 후 Start 시간 부터 LotEnd 시간 사이에서 총 정상 동작 시간 : 총 시간 - Idle 시간 - Jam 시간
            CalRunTime();

            sRunTime = pLotInfo.rSpcInfo.sRunTime;
            if (sRunTime == "")
            { sRunTime = "00:00:00"; }

            //LotOpen 후 Start 부터 LotEnd 까지 작업한 Strip 장수, 현재 테스트 버젼으로 임의값 입력
            sWorkStrip = pLotInfo.rSpcInfo.iWorkStrip.ToString();

            //UPH WorkStrip / TotalTime(hour) = UPH 소수점 버리기
            CalUph();

            sUph = pLotInfo.rSpcInfo.sUph;

            //Wheel Left 시리얼
            sWhlSerial_L = pLotInfo.rSpcInfo.sWhlSerial_L;

            //Wheel Right 시리얼
            sWhlSerial_R = pLotInfo.rSpcInfo.sWhlSerial_R;

            // Lot Open ~ Lot End 사이 Error Count
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            {
                sErrorCount = pLotInfo.rSpcInfo.iErrCnt.ToString();
            }

            sWriteLine  = sDay          + ",";
            sWriteLine += sLotName      + ",";
            sWriteLine += sGrdMode      + ",";
            sWriteLine += sDevice       + ",";
            sWriteLine += sTarget       + ",";
            sWriteLine += sStartTime    + ",";
            sWriteLine += sEndTime      + ",";
            sWriteLine += sRunTime      + ",";
            sWriteLine += sIdleTime     + ",";
            sWriteLine += sJamTime      + ",";
            sWriteLine += sTotalTime    + ",";
            sWriteLine += sUph          + ",";
            sWriteLine += sWorkStrip    + ",";
            sWriteLine += sWhlSerial_L  + ",";
            sWriteLine += sWhlSerial_R  + ",";

            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            {
                sWriteLine += sErrorCount;
            }

            CLog.Check_File_Access(sPath, sWriteLine, true);

            // Network drive 설정
            if (CData.Opt.bNetUse)
            {
                StringBuilder mPath = new StringBuilder(@"\\");
                mPath.Append(CData.Opt.sNetIP);
                int iRet = CNetDrive.Connect(mPath.ToString(), CData.Opt.sNetID, CData.Opt.sNetPw);
                if (iRet == (int)ENetError.NO_ERROR)
                {
                    mPath.Append(CData.Opt.sNetPath);
                    mPath.Append(@"SPC\LotLog\");
                    mPath.Append(DateTime.Now.ToString("yyyyMMdd"));
                    mPath.Append("\\");
                    mPath.Append(sLotName);
                    mPath.Append("\\");
                    if (!Directory.Exists(mPath.ToString()))
                    { Directory.CreateDirectory(mPath.ToString()); }
                    mPath.Append("LotInfo.csv");
                    CLog.Check_File_Access(mPath.ToString(), sWriteLine, true);

                    CNetDrive.Disconnect();
                }
            }

            //d ClearLotInfo();  // LOT마다 개별적으로 데이터를 가져가므로 특별히 Clear 해 줄 필요가 없다.

            return true;
        }

        /// <summary>
        /// 오토러닝 LotData csv 파일로 저장
        /// Day, LotName, MGZNo, SlotNo, Table, Max, Min, Mean, Ttv, Target, TargetLimit, TtvLimit, GrdMode
        /// 경로 : D:\Spc\LotLog\yyyyMMdd\LotName\LotName.csv
        /// </summary>
        /// <returns></returns>
        public bool SaveLotData()
        {
            string sPath        = "";
            string sDay         = "";
            string sLotName     = "";
            string sMgzNo       = "";
            string sSlotNo      = "";
            string sBcr         = "";
            string sTable       = "";
            string sMax         = "";
            string sMin         = "";
            string sMean        = "";
            string sTtv         = "";
            string sTarget      = "";
            string sTargetLimit = "";
            string sTtvLimit    = "";
            string sWriteLine   = "";
            string sGrdMode     = "";
            // 200730 myk : LotData 파일에 그룹/디바이스 명 추가
            string sGroup       = "";
            string sDevice      = "";

            DirectoryInfo dtInfo;
            FileInfo fiInfo;
            //StreamWriter sw;
            if (CData.CurCompany == ECompany.ASE_K12)   sLotName = CData.LotInfo.sNewLotName;   //211213 pjh : 저장하는 Lot Name 변경
            else                                        sLotName = CData.SpcData.sLotName;
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            {
                sLotName += "_" + CData.LotInfo.iLotOpenHour.ToString("00"); //20200827 lks
            }

            sPath = GV.PATH_SPC;
            sPath += "LotLog\\";
            sPath += DateTime.Now.ToString("yyyyMMdd") + "\\";
            sPath += sLotName + "\\";

            dtInfo = new DirectoryInfo(sPath);
            if (!dtInfo.Exists)
            { dtInfo.Create(); }

            sPath += CData.SpcData.sLotName + ".csv";

            fiInfo = new FileInfo(sPath);
            if (!fiInfo.Exists)
            {
                fiInfo.Create().Close();
                sWriteLine = "Day,"         ;
                sWriteLine += "LotName,"    ;
                sWriteLine += "MGZNo,"      ;
                sWriteLine += "SlotNo,"     ;
                sWriteLine += "Barcode,"    ;
                sWriteLine += "Table,"      ;
                sWriteLine += "Max,"        ;
                sWriteLine += "Min,"        ;
                sWriteLine += "Mean,"       ;
                sWriteLine += "Ttv,"        ;
                sWriteLine += "Target,"     ;
                sWriteLine += "TargetLimit,";
                sWriteLine += "TtvLimit,"   ;
                sWriteLine += "GrdMode,"    ;
                // 200730 myk : LotData 파일에 그룹/디바이스 명 추가
                sWriteLine += "Group,"      ;
                sWriteLine += "Device,"     ;
                CLog.Check_File_Access(sPath,sWriteLine,true);

                // 200820 jym : Network drive 설정
                // 210601 jym : 네트워크 드라이브 스레드 추가
                if (CData.Opt.bNetUse)
                {
                    StringBuilder mPath2 = new StringBuilder(@"\\");
                    mPath2.Append(CData.Opt.sNetIP);
                    mPath2.Append(CData.Opt.sNetPath);
                    mPath2.Append(@"SPC\LotLog\");
                    mPath2.Append(DateTime.Now.ToString("yyyyMMdd"));
                    mPath2.Append("\\");
                    mPath2.Append(CData.LotInfo.sLotName);
                    mPath2.Append("\\");
                    mPath2.Append(CData.SpcData.sLotName + ".csv");

                    CData.QueNet.Enqueue(new TNetDrive(mPath2.ToString(), sWriteLine));
                }
            }

            //현재 날짜
            // 200730 myk : LotData 파일에 시간 추가
            sDay    = DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss");

            //Lot에서 매거진 번호
            sMgzNo  = CData.SpcData.sMgzNo;

            //Lot에서 해당 매거진 슬롯 번호
            sSlotNo = CData.SpcData.sSlotNo;

            //바코드 번호
            sBcr    = CData.SpcData.sBcr;

            //테이블 왼쪽 = L, 오른쪽 = R
            sTable  = CData.SpcData.sTable;

            //그라인딩 후 측정 데이터에서 Max값
            sMax    = CData.SpcData.sMax;

            //그라인딩 후 측정 데이터에서 Min값
            sMin    = CData.SpcData.sMin;

            //그라인딩 후 측정 데이터 평균 값
            sMean   = CData.SpcData.sMean;

            //그라인딩 후 측정 데이터 Ttv값
            sTtv    = CData.SpcData.sTtv;

            //그라인딩 타켓 값
            sTarget = CData.SpcData.sTarget;

            //그라인딩 타켓 오차
            sTargetLimit = CData.SpcData.sTargetLim;
            if (sTargetLimit == "") { sTargetLimit = "0";   }

            //그라인딩 ttv 오차
            sTtvLimit = CData.SpcData.sTtvLim;
            if (sTtvLimit    == "") { sTtvLimit = "0";      }

            //그라인딩 모드
            if (CData.Dev.bDual == eDual.Normal)    { CData.SpcInfo.sGrdMode = "NOMAL"; }
            else                                    { CData.SpcInfo.sGrdMode = "STEP";  }

            sGrdMode = CData.SpcInfo.sGrdMode;

            // 200730 myk : LotData 파일에 그룹/디바이스 명 추가
            sGroup = new DirectoryInfo(Path.GetDirectoryName(CData.DevCur)).Name;

            sDevice = CData.Dev.sName;
            sWriteLine = sDay          + ",";
            sWriteLine += sLotName     + ",";
            sWriteLine += sMgzNo       + ",";
            sWriteLine += sSlotNo      + ",";
            sWriteLine += sBcr         + ",";
            sWriteLine += sTable       + ",";
            sWriteLine += sMax         + ",";
            sWriteLine += sMin         + ",";
            sWriteLine += sMean        + ",";
            sWriteLine += sTtv         + ",";
            sWriteLine += sTarget      + ",";
            sWriteLine += sTargetLimit + ",";
            sWriteLine += sTtvLimit    + ",";
            sWriteLine += sGrdMode     + ",";
            // 200730 myk : LotData 파일에 그룹/디바이스 명 추가
            sWriteLine += sGroup       + ",";
            sWriteLine += sDevice      + ",";
            CLog.Check_File_Access(sPath,sWriteLine,true);

            // 200820 jym : Network drive 설정
            // 210601 jym : 네트워크 드라이브 스레드 추가
            if (CData.Opt.bNetUse)
            {
                StringBuilder mPath2 = new StringBuilder(@"\\");
                mPath2.Append(CData.Opt.sNetIP);
                mPath2.Append(CData.Opt.sNetPath);
                mPath2.Append(@"SPC\LotLog\");
                mPath2.Append(DateTime.Now.ToString("yyyyMMdd"));
                mPath2.Append("\\");
                mPath2.Append(CData.LotInfo.sLotName);
                mPath2.Append("\\");
                mPath2.Append(CData.SpcData.sLotName + ".csv");

                CData.QueNet.Enqueue(new TNetDrive(mPath2.ToString(), sWriteLine));
            }

            if (CData.CurCompany == ECompany.Qorvo || 
                CData.CurCompany == ECompany.Qorvo_DZ ||
                CData.CurCompany == ECompany.Qorvo_RT ||        // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                CData.CurCompany == ECompany.Qorvo_NC ||        // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                CData.CurCompany == ECompany.SST) //191202 ksg :
            {
                SaveLotData_Probe(); //191120 ksg :
            }
            ClearLotData();

            return true;
        }


        /// <summary>
        /// Multi-LOT 지원, 지정된 LOT Info의 SPC 데이터를 LotData csv 파일로 저장
        /// Day, LotName, MGZNo, SlotNo, Table, Max, Min, Mean, Ttv, Target, TargetLimit, TtvLimit, GrdMode
        /// 경로 : D:\Spc\LotLog\yyyyMMdd\LotName\LotName.csv
        /// </summary>
        /// <returns></returns>
        public bool SaveLotData(TLotInfo pLotInfo)
        {
            string sPath        = "";
            string sDay         = "";
            string sLotName     = "";
            string sMgzNo       = "";
            string sSlotNo      = "";
            string sBcr         = "";
            string sTable       = "";
            string sMax         = "";
            string sMin         = "";
            string sMean        = "";
            string sTtv         = "";
            string sTarget      = "";
            string sTargetLimit = "";
            string sTtvLimit    = "";
            string sWriteLine   = "";
            string sGrdMode     = "";
            string sGroup       = "";
            string sDevice      = "";

            DirectoryInfo dtInfo;
            FileInfo fiInfo;

            if (CData.CurCompany == ECompany.ASE_K12)   sLotName = CData.LotInfo.sNewLotName;   //211213 pjh : 저장하는 Lot Name 변경
            else                                        sLotName = pLotInfo.sLotName;
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            {
                sLotName += "_" + pLotInfo.dtOpen.ToString("HH");                       // LOT Open 시각
            }

            sPath = GV.PATH_SPC;
            sPath += "LotLog\\";
            sPath += DateTime.Now.ToString("yyyyMMdd") + "\\";
            sPath += sLotName + "\\";

            dtInfo = new DirectoryInfo(sPath);
            if (!dtInfo.Exists) {   dtInfo.Create();    }

            //sPath += pLotInfo.sLotName + ".csv";
            sPath += sLotName + ".csv";  // 2021.12.29 lhs : sLotName으로 수정

            fiInfo = new FileInfo(sPath);
            if (!fiInfo.Exists)
            {
                fiInfo.Create().Close();
                sWriteLine = "Day,";
                sWriteLine += "LotName,";
                sWriteLine += "MGZNo,";
                sWriteLine += "SlotNo,";
                sWriteLine += "Barcode,";
                sWriteLine += "Table,";
                sWriteLine += "Max,";
                sWriteLine += "Min,";
                sWriteLine += "Mean,";
                sWriteLine += "Ttv,";
                sWriteLine += "Target,";
                sWriteLine += "TargetLimit,";
                sWriteLine += "TtvLimit,";
                sWriteLine += "GrdMode,";
                // 200730 myk : LotData 파일에 그룹/디바이스 명 추가
                sWriteLine += "Group,";
                sWriteLine += "Device,";
                CLog.Check_File_Access(sPath, sWriteLine, true);

                // 200820 jym : Network drive 설정
                if (CData.Opt.bNetUse)
                {
                    StringBuilder mPath2 = new StringBuilder(@"\\");
                    mPath2.Append(CData.Opt.sNetIP);
                    int iRet = CNetDrive.Connect(mPath2.ToString(), CData.Opt.sNetID, CData.Opt.sNetPw);
                    if (iRet == (int)ENetError.NO_ERROR)
                    {
                        mPath2.Append(CData.Opt.sNetPath);
                        mPath2.Append(@"SPC\LotLog\");
                        mPath2.Append(DateTime.Now.ToString("yyyyMMdd"));
                        mPath2.Append("\\");
                        mPath2.Append(sLotName);
                        mPath2.Append("\\");
                        if (!Directory.Exists(mPath2.ToString()))
                        { Directory.CreateDirectory(mPath2.ToString()); }
                        mPath2.Append(sLotName + ".csv");
                        CLog.Check_File_Access(mPath2.ToString(), sWriteLine, true);

                        CNetDrive.Disconnect();
                    }
                }
            }

            //현재 날짜
            // 200730 myk : LotData 파일에 시간 추가
            sDay    = DateTime.Now.ToString("yyyy.MM.dd.HH.mm.ss");

            //Lot에서 매거진 번호
            sMgzNo  = pLotInfo.rSpcData.sMgzNo;

            //Lot에서 해당 매거진 슬롯 번호
            sSlotNo = pLotInfo.rSpcData.sSlotNo;

            //바코드 번호
            sBcr    = pLotInfo.rSpcData.sBcr;

            //테이블 왼쪽 = L, 오른쪽 = R
            sTable  = pLotInfo.rSpcData.sTable;

            //그라인딩 후 측정 데이터에서 Max값
            sMax    = pLotInfo.rSpcData.sMax;

            //그라인딩 후 측정 데이터에서 Min값
            sMin    = pLotInfo.rSpcData.sMin;

            //그라인딩 후 측정 데이터 평균 값
            sMean   = pLotInfo.rSpcData.sMean;

            //그라인딩 후 측정 데이터 Ttv값
            sTtv    = pLotInfo.rSpcData.sTtv;

            //그라인딩 타켓 값
            sTarget = pLotInfo.rSpcData.sTarget;

            //그라인딩 타켓 오차
            sTargetLimit = pLotInfo.rSpcData.sTargetLim;
            if (sTargetLimit == "") { sTargetLimit = "0"; }

            //그라인딩 ttv 오차
            sTtvLimit = pLotInfo.rSpcData.sTtvLim;
            if (sTtvLimit == "")    { sTtvLimit = "0"; }

            //그라인딩 모드
            if (CData.Dev.bDual == eDual.Normal)    { pLotInfo.rSpcInfo.sGrdMode = "NOMAL"; }
            else                                    { pLotInfo.rSpcInfo.sGrdMode = "STEP";  }
            sGrdMode = pLotInfo.rSpcInfo.sGrdMode;

            // 200730 myk : LotData 파일에 그룹/디바이스 명 추가
            sGroup = new DirectoryInfo(Path.GetDirectoryName(CData.DevCur)).Name;

            sDevice     = CData.Dev.sName;
            sWriteLine  = sDay          + ",";
            sWriteLine += sLotName      + ",";
            sWriteLine += sMgzNo        + ",";
            sWriteLine += sSlotNo       + ",";
            sWriteLine += sBcr          + ",";
            sWriteLine += sTable        + ",";
            sWriteLine += sMax          + ",";
            sWriteLine += sMin          + ",";
            sWriteLine += sMean         + ",";
            sWriteLine += sTtv          + ",";
            sWriteLine += sTarget       + ",";
            sWriteLine += sTargetLimit  + ",";
            sWriteLine += sTtvLimit     + ",";
            sWriteLine += sGrdMode      + ",";
            // 200730 myk : LotData 파일에 그룹/디바이스 명 추가
            sWriteLine += sGroup        + ",";
            sWriteLine += sDevice       + ",";
            CLog.Check_File_Access(sPath, sWriteLine, true);

            // 200820 jym : Network drive 설정
            if (CData.Opt.bNetUse)
            {
                StringBuilder mPath2 = new StringBuilder(@"\\");
                mPath2.Append(CData.Opt.sNetIP);
                int iRet = CNetDrive.Connect(mPath2.ToString(), CData.Opt.sNetID, CData.Opt.sNetPw);
                if (iRet == (int)ENetError.NO_ERROR)
                {
                    mPath2.Append(CData.Opt.sNetPath);
                    mPath2.Append(@"SPC\LotLog\");
                    mPath2.Append(DateTime.Now.ToString("yyyyMMdd"));
                    mPath2.Append("\\");
                    mPath2.Append(sLotName);
                    mPath2.Append("\\");
                    if (!Directory.Exists(mPath2.ToString()))
                    { Directory.CreateDirectory(mPath2.ToString()); }
                    mPath2.Append(sLotName + ".csv");
                    CLog.Check_File_Access(mPath2.ToString(), sWriteLine, true);

                    CNetDrive.Disconnect();
                }
            }

            if (CData.CurCompany == ECompany.Qorvo ||
                CData.CurCompany == ECompany.Qorvo_DZ ||
                CData.CurCompany == ECompany.Qorvo_RT ||        // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                CData.CurCompany == ECompany.Qorvo_NC ||        // 2021.03.29 SungTae : Qorvo_RT & NC 조건 추가
                CData.CurCompany == ECompany.SST) //191202 ksg :
            {
                SaveLotData_Probe(ref pLotInfo); //191120 ksg :
            }

            // 미사용 ClearLotData();

            return true;
        }
        // end of SaveLotData(TLotInfo pLotInfo)


        /// <summary>
        /// 오토러닝 LotData csv 파일로 저장
        /// Ase Kr에서 추가 요청 사항
        /// Day, LotName, MGZNo, SlotNo, Table, Max, Min, Mean, Ttv, Target, TargetLimit, TtvLimit
        /// 경로 : D:\Spc\LotLog\yyyyMMdd\LotName\LotName.csv
        /// </summary>
        /// <returns></returns>
        public bool SaveLotData_Add()
        {
            string sPath        = "";
            string sDay         = "";
            string sLotName     = "";
            string sMgzNo       = "";
            string sSlotNo      = "";
            string sBcr         = "";
            string sTable       = "";
            string sBMax        = "";
            string sBMin        = "";
            string sBMean       = "";
            string sMax         = "";
            string sMin         = "";
            string sMean        = "";
            string sTtv         = "";
            string sTarget      = "";
            string sTargetLimit = "";
            string sTtvLimit    = "";
            string sWriteLine   = "";

            DirectoryInfo dtInfo;
            FileInfo fiInfo;
            //koo 191002 : Speed Write StreamWriter sw;

            if (CData.CurCompany == ECompany.ASE_K12)   sLotName = CData.LotInfo.sNewLotName;//211213 pjh : 저장하는 Lot Name 변경
            else                                        sLotName = CData.SpcData.sLotName;
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            {
                sLotName += "_" + CData.LotInfo.iLotOpenHour.ToString("00"); //20200827 lks
            }

            sPath = GV.PATH_SPC;
            sPath += "LotLog\\";
            sPath += DateTime.Now.ToString("yyyyMMdd") + "\\";
            sPath += sLotName + "\\";

            dtInfo = new DirectoryInfo(sPath);
            if (!dtInfo.Exists)
            { dtInfo.Create(); }

            sPath += CData.SpcData.sLotName + ".csv";

            fiInfo = new FileInfo(sPath);
            if (!fiInfo.Exists)
            {
                fiInfo.Create().Close();
                sWriteLine = "Day,"         ;
                sWriteLine += "LotName,"    ;
                sWriteLine += "MGZNo,"      ;
                sWriteLine += "SlotNo,"     ;
                sWriteLine += "Barcode,"    ;
                sWriteLine += "Table,"      ;
                sWriteLine += "BeforeMax,"  ;
                sWriteLine += "BeforeMin,"  ;
                sWriteLine += "BeforeMean," ;
                sWriteLine += "AfterMax,"   ;
                sWriteLine += "AfterMin,"   ;
                sWriteLine += "AfterMean,"  ;
                sWriteLine += "Ttv,"        ;
                sWriteLine += "Target,"     ;
                sWriteLine += "TargetLimit,";
                sWriteLine += "TtvLimit,"   ;
                CLog.Check_File_Access(sPath,sWriteLine,true);
            }

            //현재 날짜
            sDay    = DateTime.Now.ToString("yyyy.MM.dd");

            //Lot에서 매거진 번호
            sMgzNo  = CData.SpcData.sMgzNo;

            //Lot에서 해당 매거진 슬롯 번호
            sSlotNo = CData.SpcData.sSlotNo;

            //바코드 번호
            sBcr    = CData.SpcData.sBcr;

            //테이블 왼쪽 = L, 오른쪽 = R
            sTable  = CData.SpcData.sTable;

            //그라인딩 전 측정 데이터에서 Max값
            sBMax   = CData.SpcData.sBMax;

            //그라인딩 전 측정 데이터에서 Min값
            sBMin   = CData.SpcData.sBMin;

            //그라인딩 후 측정 데이터 평균 값
            sBMean  = CData.SpcData.sBAvg;

            //그라인딩 후 측정 데이터에서 Max값
            sMax    = CData.SpcData.sMax;

            //그라인딩 후 측정 데이터에서 Min값
            sMin    = CData.SpcData.sMin;

            //그라인딩 후 측정 데이터 평균 값
            sMean   = CData.SpcData.sMean;

            //그라인딩 후 측정 데이터 Ttv값
            sTtv    = CData.SpcData.sTtv;

            //그라인딩 타켓 값
            sTarget = CData.SpcData.sTarget;

            //그라인딩 타켓 오차
            sTargetLimit = CData.SpcData.sTargetLim;
            if (sTargetLimit == "") { sTargetLimit = "0"; }

            //그라인딩 ttv 오차
            sTtvLimit = CData.SpcData.sTtvLim;
            if (sTtvLimit == "")    { sTtvLimit = "0"; }

            sWriteLine  = sDay         + ",";
            sWriteLine += sLotName     + ",";
            sWriteLine += sMgzNo       + ",";
            sWriteLine += sSlotNo      + ",";
            sWriteLine += sBcr         + ",";
            sWriteLine += sTable       + ",";
            sWriteLine += sBMax        + ",";
            sWriteLine += sBMin        + ",";
            sWriteLine += sBMean       + ",";
            sWriteLine += sMax         + ",";
            sWriteLine += sMin         + ",";
            sWriteLine += sMean        + ",";
            sWriteLine += sTtv         + ",";
            sWriteLine += sTarget      + ",";
            sWriteLine += sTargetLimit + ",";
            sWriteLine += sTtvLimit    + ",";
            CLog.Check_File_Access(sPath,sWriteLine,true);
            ClearLotData();

            return true;
        }

        /// <summary>
        /// 오토러닝 LotData csv 파일로 저장
        /// Ase Kr에서 추가 요청 사항
        /// Day, LotName, MGZNo, SlotNo, Table, Max, Min, Mean, Ttv, Target, TargetLimit, TtvLimit
        /// 경로 : D:\Spc\LotLog\yyyyMMdd\LotName\LotName.csv
        /// </summary>
        /// <returns></returns>
        public bool SaveLotData_Add(ref TLotInfo pLotInfo)
        {
            string sPath        = "";
            string sDay         = "";
            string sLotName     = "";
            string sMgzNo       = "";
            string sSlotNo      = "";
            string sBcr         = "";
            string sTable       = "";
            string sBMax        = "";
            string sBMin        = "";
            string sBMean       = "";
            string sMax         = "";
            string sMin         = "";
            string sMean        = "";
            string sTtv         = "";
            string sTarget      = "";
            string sTargetLimit = "";
            string sTtvLimit    = "";
            string sWriteLine   = "";

            DirectoryInfo dtInfo;
            FileInfo fiInfo;
            //koo 191002 : Speed Write StreamWriter sw;

            if (CData.CurCompany == ECompany.ASE_K12)   sLotName = CData.LotInfo.sNewLotName;   //211213 pjh : 저장하는 Lot Name 변경
            else                                        sLotName = pLotInfo.sLotName;
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            {
                sLotName += "_" + pLotInfo.dtOpen.ToString("HH");
            }

            sPath = GV.PATH_SPC;
            sPath += "LotLog\\";
            sPath += DateTime.Now.ToString("yyyyMMdd") + "\\";
            sPath += sLotName + "\\";

            dtInfo = new DirectoryInfo(sPath);
            if (!dtInfo.Exists)
            { dtInfo.Create(); }

            //sPath += pLotInfo.sLotName + ".csv";
            sPath += sLotName + ".csv";  // 2021.12.29 lhs : sLotName으로 수정

            fiInfo = new FileInfo(sPath);
            if (!fiInfo.Exists)
            {
                fiInfo.Create().Close();
                sWriteLine = "Day,";
                sWriteLine += "LotName,";
                sWriteLine += "MGZNo,";
                sWriteLine += "SlotNo,";
                sWriteLine += "Barcode,";
                sWriteLine += "Table,";
                sWriteLine += "BeforeMax,";
                sWriteLine += "BeforeMin,";
                sWriteLine += "BeforeMean,";
                sWriteLine += "AfterMax,";
                sWriteLine += "AfterMin,";
                sWriteLine += "AfterMean,";
                sWriteLine += "Ttv,";
                sWriteLine += "Target,";
                sWriteLine += "TargetLimit,";
                sWriteLine += "TtvLimit,";
                CLog.Check_File_Access(sPath, sWriteLine, true);
            }

            //현재 날짜
            sDay    = DateTime.Now.ToString("yyyy.MM.dd");

            //Lot에서 매거진 번호
            sMgzNo  = pLotInfo.rSpcData.sMgzNo;

            //Lot에서 해당 매거진 슬롯 번호
            sSlotNo = pLotInfo.rSpcData.sSlotNo;

            //바코드 번호
            sBcr    = pLotInfo.rSpcData.sBcr;

            //테이블 왼쪽 = L, 오른쪽 = R
            sTable  = pLotInfo.rSpcData.sTable;

            //그라인딩 전 측정 데이터에서 Max값
            sBMax   = pLotInfo.rSpcData.sBMax;

            //그라인딩 전 측정 데이터에서 Min값
            sBMin   = pLotInfo.rSpcData.sBMin;

            //그라인딩 후 측정 데이터 평균 값
            sBMean  = pLotInfo.rSpcData.sBAvg;

            //그라인딩 후 측정 데이터에서 Max값
            sMax    = pLotInfo.rSpcData.sMax;

            //그라인딩 후 측정 데이터에서 Min값
            sMin    = pLotInfo.rSpcData.sMin;

            //그라인딩 후 측정 데이터 평균 값
            sMean   = pLotInfo.rSpcData.sMean;

            //그라인딩 후 측정 데이터 Ttv값
            sTtv    = pLotInfo.rSpcData.sTtv;

            //그라인딩 타켓 값
            sTarget = pLotInfo.rSpcData.sTarget;

            //그라인딩 타켓 오차
            sTargetLimit = pLotInfo.rSpcData.sTargetLim;
            if (sTargetLimit == "") { sTargetLimit = "0"; }

            //그라인딩 ttv 오차
            sTtvLimit = pLotInfo.rSpcData.sTtvLim;
            if (sTtvLimit == "")    { sTtvLimit = "0"; }

            sWriteLine  = sDay          + ",";
            sWriteLine += sLotName      + ",";
            sWriteLine += sMgzNo        + ",";
            sWriteLine += sSlotNo       + ",";
            sWriteLine += sBcr          + ",";
            sWriteLine += sTable        + ",";
            sWriteLine += sBMax         + ",";
            sWriteLine += sBMin         + ",";
            sWriteLine += sBMean        + ",";
            sWriteLine += sMax          + ",";
            sWriteLine += sMin          + ",";
            sWriteLine += sMean         + ",";
            sWriteLine += sTtv          + ",";
            sWriteLine += sTarget       + ",";
            sWriteLine += sTargetLimit  + ",";
            sWriteLine += sTtvLimit     + ",";
            CLog.Check_File_Access(sPath, sWriteLine, true);

            // ClearLotData();

            return true;
        }

        /// <summary>
        /// 오토러닝 LotProbeData csv 파일로 저장
        /// Ase Kr에서 추가 요청 사항
        /// Day, LotName, MGZNo, SlotNo, Table, Max, Min, Mean, Ttv, Target, TargetLimit, TtvLimit
        /// 경로 : D:\Spc\LotLog\yyyyMMdd\LotName\LotName.csv
        /// </summary>
        /// <returns></returns>
        public bool SaveLotData_Probe()
        {
            string sPath        = "";
            string sDay         = "";
            string sLotName     = "";
            string sMgzNo       = "";
            string sSlotNo      = "";
            string sBcr         = "";
            string sTable       = "";
            string sBMax        = "";
            string sBMin        = "";
            string sBMean       = "";
            string sMax         = "";
            string sMin         = "";
            string sMean        = "";
            string sBfTtv       = "";
            string sTtv         = "";
            string sTarget      = "";
            string sTargetLimit = "";
            string sTtvLimit    = "";
            string sWriteLine   = "";
            string sMode        = "";

            int SelTb;
            double dBfTtv;
            
            DirectoryInfo dtInfo;
            FileInfo fiInfo;

            if (CData.CurCompany == ECompany.ASE_K12)   sLotName = CData.LotInfo.sNewLotName;   //211213 pjh : 저장하는 Lot Name 변경
            else                                        sLotName = CData.SpcData.sLotName;
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            {
                sLotName += "_" + CData.LotInfo.iLotOpenHour.ToString("00"); //20200827 lks
            }
            
            sPath = GV.PATH_SPC;
            sPath += "LotLog\\";
            sPath += DateTime.Now.ToString("yyyyMMdd") + "\\";
            sPath += sLotName + "\\";

            dtInfo = new DirectoryInfo(sPath);
            if (!dtInfo.Exists)
            { dtInfo.Create(); }

            sPath += CData.SpcData.sLotName + "_Probe.csv";

            if(CData.SpcData.sTable == "L") SelTb = 0;
            else                            SelTb = 1;

            fiInfo = new FileInfo(sPath);
            if (!fiInfo.Exists)
            {
                fiInfo.Create().Close();
                sWriteLine = "Day,"          ;
                sWriteLine += "LotName,"     ;
                sWriteLine += "MGZNo,"       ;
                sWriteLine += "SlotNo,"      ;
                sWriteLine += "Barcode,"     ;
                sWriteLine += "Table,"       ;
                sWriteLine += "Before/After,";
                for(int i = 0; i < CData.m_dPbVal[SelTb].m_BFVal.Count; i++)
                {
                    sWriteLine += "Data_"+ i.ToString()+ ",";
                }
                sWriteLine += "Max,"        ;
                sWriteLine += "Min,"        ;
                sWriteLine += "Mean,"       ;
                sWriteLine += "Ttv,"        ;
                sWriteLine += "Target,"     ;
                sWriteLine += "TargetLimit,";
                sWriteLine += "TtvLimit,"   ;
                sWriteLine += "GrdMode,"    ;

                CLog.Check_File_Access(sPath,sWriteLine,true);

                // 200820 jym : Network drive 설정
                // 210601 jym : 네트워크 드라이브 스레드 추가
                if (CData.Opt.bNetUse)
                {
                    StringBuilder mPath2 = new StringBuilder(@"\\");
                    mPath2.Append(CData.Opt.sNetIP);
                    mPath2.Append(CData.Opt.sNetPath);
                    mPath2.Append(@"SPC\LotLog\");
                    mPath2.Append(DateTime.Now.ToString("yyyyMMdd"));
                    mPath2.Append("\\");
                    mPath2.Append(CData.LotInfo.sLotName);
                    mPath2.Append("\\");
                    mPath2.Append(CData.SpcData.sLotName + "_Probe.csv");

                    CData.QueNet.Enqueue(new TNetDrive(mPath2.ToString(), sWriteLine));
                }
            }

            sWriteLine   = string.Empty;

            sDay         = DateTime.Now.ToString("yyyy.MM.dd"); //현재 날짜
            sMgzNo       = CData.SpcData.sMgzNo ;               //Lot에서 매거진 번호
            sSlotNo      = CData.SpcData.sSlotNo;               //Lot에서 해당 매거진 슬롯 번호
            sBcr         = CData.SpcData.sBcr   ;               //바코드 번호
            sTable       = CData.SpcData.sTable ;               //테이블 왼쪽 = L, 오른쪽 = R
            sBMax        = CData.SpcData.sBMax  ;               //그라인딩 전 측정 데이터에서 Max값
            sBMin        = CData.SpcData.sBMin  ;               //그라인딩 전 측정 데이터에서 Min값
            sBMean       = CData.SpcData.sBAvg  ;               //그라인딩 후 측정 데이터 평균 값
            sMax         = CData.SpcData.sMax   ;               //그라인딩 후 측정 데이터에서 Max값
            sMin         = CData.SpcData.sMin   ;               //그라인딩 후 측정 데이터에서 Min값
            sMean        = CData.SpcData.sMean  ;               //그라인딩 후 측정 데이터 평균 값
            dBfTtv       = Convert.ToDouble(CData.SpcData.sBMax) - Convert.ToDouble(CData.SpcData.sBMin); //Before Ttv
            sBfTtv       = dBfTtv.ToString();
            sTtv         = CData.SpcData.sTtv   ;           //그라인딩 후 측정 데이터 Ttv값
            sTarget      = CData.SpcData.sTarget;           //그라인딩 타켓 값
            sTargetLimit = CData.SpcData.sTargetLim;        //그라인딩 타켓 오차
            if (sTargetLimit == "") { sTargetLimit = "0"; }
            sTtvLimit    = CData.SpcData.sTtvLim;           //그라인딩 ttv 오차
            if (sTtvLimit    == "") { sTtvLimit    = "0"; }
            sMode        = CData.SpcData.sMode  ;           //Grd Mode
            
            for (int i = 0; i < 2; i++)
            {
                sWriteLine += sDay     + ",";
                sWriteLine += sLotName + ",";
                sWriteLine += sMgzNo   + ",";
                sWriteLine += sSlotNo  + ",";
                sWriteLine += sBcr     + ",";
                sWriteLine += sTable   + ",";
                if(i == 0)
                {
                    sWriteLine += "B,";
                    for(int j = 0; j < CData.m_dPbVal[SelTb].m_BFVal.Count; j++)
                    {
                        sWriteLine += CData.m_dPbVal[SelTb].m_BFVal[j].ToString() + ",";
                    }
                }
                else
                {
                    sWriteLine += "A,";
                    for(int j = 0; j < CData.m_dPbVal[SelTb].m_AFVal.Count; j++)
                    {
                        sWriteLine += CData.m_dPbVal[SelTb].m_AFVal[j].ToString() + ",";
                    }
                }
                if(i == 0)
                {
                    sWriteLine += sBMax  + ",";
                    sWriteLine += sBMin  + ",";
                    sWriteLine += sBMean + ",";
                }
                else
                {
                    sWriteLine += sMax  + ",";
                    sWriteLine += sMin  + ",";
                    sWriteLine += sMean + ",";
                }
                if(i == 0)
                {
                    sWriteLine += sBfTtv + ",";
                }
                else
                {
                    sWriteLine += sTtv + ",";
                }
                
                sWriteLine += sTarget      + ",";
                sWriteLine += sTargetLimit + ",";
                sWriteLine += sTtvLimit    + ",";
                sWriteLine += sMode        + ",";
                if(i == 0) sWriteLine += "\r\n";
            }
            CLog.Check_File_Access(sPath,sWriteLine,true);

            // 200820 jym : Network drive 설정
            // 210601 jym : 네트워크 드라이브 스레드 추가
            if (CData.Opt.bNetUse)
            {
                StringBuilder mPath2 = new StringBuilder(@"\\");
                mPath2.Append(CData.Opt.sNetIP);
                mPath2.Append(CData.Opt.sNetPath);
                mPath2.Append(@"SPC\LotLog\");
                mPath2.Append(DateTime.Now.ToString("yyyyMMdd"));
                mPath2.Append("\\");
                mPath2.Append(CData.LotInfo.sLotName);
                mPath2.Append("\\");
                mPath2.Append(CData.SpcData.sLotName + "_Probe.csv");

                CData.QueNet.Enqueue(new TNetDrive(mPath2.ToString(), sWriteLine));
            }

            ClearLotInfo();

            return true;
        }


        /// <summary>
        /// 오토러닝 LotProbeData csv 파일로 저장
        /// Ase Kr에서 추가 요청 사항
        /// Day, LotName, MGZNo, SlotNo, Table, Max, Min, Mean, Ttv, Target, TargetLimit, TtvLimit
        /// 경로 : D:\Spc\LotLog\yyyyMMdd\LotName\LotName.csv
        /// </summary>
        /// <returns></returns>
        public bool SaveLotData_Probe(ref TLotInfo pLotInfo)
        {
            string sPath        = "";
            string sDay         = "";
            string sLotName     = "";
            string sMgzNo       = "";
            string sSlotNo      = "";
            string sBcr         = "";
            string sTable       = "";
            string sBMax        = "";
            string sBMin        = "";
            string sBMean       = "";
            string sMax         = "";
            string sMin         = "";
            string sMean        = "";
            string sBfTtv       = "";
            string sTtv         = "";
            string sTarget      = "";
            string sTargetLimit = "";
            string sTtvLimit    = "";
            string sWriteLine   = "";
            string sMode        = "";

            int SelTb;
            double dBfTtv;
            DirectoryInfo dtInfo;
            FileInfo fiInfo;

            if (CData.CurCompany == ECompany.ASE_K12)   sLotName = CData.LotInfo.sNewLotName;   //211213 pjh : 저장하는 Lot Name 변경
            else                                        sLotName = pLotInfo.sLotName;
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            {
                sLotName += "_" + pLotInfo.dtOpen.ToString("HH");
            }

            sPath = GV.PATH_SPC;
            sPath += "LotLog\\";
            sPath += DateTime.Now.ToString("yyyyMMdd") + "\\";
            sPath += sLotName + "\\";

            dtInfo = new DirectoryInfo(sPath);
            if (!dtInfo.Exists)
            { dtInfo.Create(); }

            //sPath += pLotInfo.sLotName + "_Probe.csv";
            sPath += sLotName + "_Probe.csv";  // 2021.12.29 lhs : sLotName으로 수정

            //if (CData.SpcData.sTable == "L")    SelTb = 0;
            if (pLotInfo.rSpcData.sTable == "L")    SelTb = 0; // 2021.12.29 lhs : 오류수정 CData.SpcData.sTable -> pLotInfo.rSpcData.sTable
            else                                    SelTb = 1;

            fiInfo = new FileInfo(sPath);
            if (!fiInfo.Exists)
            {
                fiInfo.Create().Close();
                sWriteLine = "Day,";
                sWriteLine += "LotName,";
                sWriteLine += "MGZNo,";
                sWriteLine += "SlotNo,";
                sWriteLine += "Barcode,";
                sWriteLine += "Table,";
                sWriteLine += "Before/After,";
                for (int i = 0; i < CData.m_dPbVal[SelTb].m_BFVal.Count; i++)
                {
                    sWriteLine += "Data_" + i.ToString() + ",";
                }
                sWriteLine += "Max,";
                sWriteLine += "Min,";
                sWriteLine += "Mean,";
                sWriteLine += "Ttv,";
                sWriteLine += "Target,";
                sWriteLine += "TargetLimit,";
                sWriteLine += "TtvLimit,";
                sWriteLine += "GrdMode,";
                CLog.Check_File_Access(sPath, sWriteLine, true);

                // 200820 jym : Network drive 설정
                // 210601 jym : 네트워크 드라이브 스레드 추가
                if (CData.Opt.bNetUse)
                {
                    StringBuilder mPath2 = new StringBuilder(@"\\");
                    mPath2.Append(CData.Opt.sNetIP);
                    mPath2.Append(CData.Opt.sNetPath);
                    mPath2.Append(@"SPC\LotLog\");
                    mPath2.Append(DateTime.Now.ToString("yyyyMMdd"));
                    mPath2.Append("\\");
                    //mPath2.Append(CData.LotInfo.sLotName);
                    mPath2.Append(sLotName);                // 2021.12.29 lhs : sLotName으로 수정
                    mPath2.Append("\\");
                    //mPath2.Append(CData.SpcData.sLotName + "_Probe.csv");
                    mPath2.Append(sLotName + "_Probe.csv"); // 2021.12.29 lhs : sLotName으로 수정

                    CData.QueNet.Enqueue(new TNetDrive(mPath2.ToString(), sWriteLine));
                }
            }

            sWriteLine  = string.Empty;

            sDay        = DateTime.Now.ToString("yyyy.MM.dd");  //현재 날짜
            sMgzNo      = pLotInfo.rSpcData.sMgzNo;             //Lot에서 매거진 번호
            sSlotNo     = pLotInfo.rSpcData.sSlotNo;            //Lot에서 해당 매거진 슬롯 번호
            sBcr        = pLotInfo.rSpcData.sBcr;               //바코드 번호
            sTable      = pLotInfo.rSpcData.sTable;             //테이블 왼쪽 = L, 오른쪽 = R
            sBMax       = pLotInfo.rSpcData.sBMax;              //그라인딩 전 측정 데이터에서 Max값
            sBMin       = pLotInfo.rSpcData.sBMin;              //그라인딩 전 측정 데이터에서 Min값
            sBMean      = pLotInfo.rSpcData.sBAvg;              //그라인딩 후 측정 데이터 평균 값
            sMax        = pLotInfo.rSpcData.sMax;               //그라인딩 후 측정 데이터에서 Max값
            sMin        = pLotInfo.rSpcData.sMin;               //그라인딩 후 측정 데이터에서 Min값
            sMean       = pLotInfo.rSpcData.sMean;              //그라인딩 후 측정 데이터 평균 값
            dBfTtv      = Convert.ToDouble(pLotInfo.rSpcData.sBMax) - Convert.ToDouble(pLotInfo.rSpcData.sBMin); //Before Ttv
            sBfTtv      = dBfTtv.ToString();
            sTtv        = pLotInfo.rSpcData.sTtv;               //그라인딩 후 측정 데이터 Ttv값
            sTarget     = pLotInfo.rSpcData.sTarget;            //그라인딩 타켓 값
            sTargetLimit = pLotInfo.rSpcData.sTargetLim;    if (sTargetLimit == "") { sTargetLimit = "0"; } //그라인딩 타켓 오차
            sTtvLimit   = pLotInfo.rSpcData.sTtvLim;        if (sTtvLimit    == "") { sTtvLimit    = "0"; } //그라인딩 ttv 오차
            sMode       = pLotInfo.rSpcData.sMode;              //Grd Mode

            for (int i = 0; i < 2; i++)
            {
                sWriteLine += sDay      + ",";
                sWriteLine += sLotName  + ",";
                sWriteLine += sMgzNo    + ",";
                sWriteLine += sSlotNo   + ",";
                sWriteLine += sBcr      + ",";
                sWriteLine += sTable    + ",";
                if (i == 0)
                {
                    sWriteLine += "B,";
                    for (int j = 0; j < CData.m_dPbVal[SelTb].m_BFVal.Count; j++)
                    {
                        sWriteLine += CData.m_dPbVal[SelTb].m_BFVal[j].ToString() + ",";
                    }
                }
                else
                {
                    sWriteLine += "A,";
                    for (int j = 0; j < CData.m_dPbVal[SelTb].m_AFVal.Count; j++)
                    {
                        sWriteLine += CData.m_dPbVal[SelTb].m_AFVal[j].ToString() + ",";
                    }
                }
                if (i == 0)
                {
                    sWriteLine += sBMax  + ",";
                    sWriteLine += sBMin  + ",";
                    sWriteLine += sBMean + ",";
                }
                else
                {
                    sWriteLine += sMax  + ",";
                    sWriteLine += sMin  + ",";
                    sWriteLine += sMean + ",";
                }
                if (i == 0)
                {
                    sWriteLine += sBfTtv + ",";
                }
                else
                {
                    sWriteLine += sTtv  + ",";
                }

                sWriteLine += sTarget       + ",";
                sWriteLine += sTargetLimit  + ",";
                sWriteLine += sTtvLimit     + ",";
                sWriteLine += sMode         + ",";
                if (i == 0) sWriteLine      += "\r\n";
            }
            CLog.Check_File_Access(sPath, sWriteLine, true);

            // 200820 jym : Network drive 설정
            // 210601 jym : 네트워크 드라이브 스레드 추가
            if (CData.Opt.bNetUse)
            {
                StringBuilder mPath2 = new StringBuilder(@"\\");
                mPath2.Append(CData.Opt.sNetIP);
                mPath2.Append(CData.Opt.sNetPath);
                mPath2.Append(@"SPC\LotLog\");
                mPath2.Append(DateTime.Now.ToString("yyyyMMdd"));
                mPath2.Append("\\");
                //mPath2.Append(CData.LotInfo.sLotName);
                mPath2.Append(sLotName);                // 2021.12.29 lhs : sLotName으로 수정
                mPath2.Append("\\");
                //mPath2.Append(CData.SpcData.sLotName + "_Probe.csv");
                mPath2.Append(sLotName + "_Probe.csv"); // 2021.12.29 lhs : sLotName으로 수정

                CData.QueNet.Enqueue(new TNetDrive(mPath2.ToString(), sWriteLine));
            }

            return true;
        }
        //end of SaveLotData_Probe(TLotInfo pLotInfo)


        /// <summary>
        /// Total시간 계산 : Lot End Time - Lot Start Time
        /// </summary>
        /// <param name="sStartTime">Lot Start 시간</param>
        /// <param name="sEndTime">Lot End 시간</param>
        public void CalTotalTime(string sStartTime, string sEndTime)
        {
            DateTime dtSt;
            DateTime dtEnd;
            DateTime dtTotal;

            dtSt    = Convert.ToDateTime(sStartTime);
            dtEnd   = Convert.ToDateTime(sEndTime  );
            dtTotal = dtEnd - dtSt.TimeOfDay;

            CData.SpcInfo.sTotalTime = dtTotal.ToString("HH:mm:ss");
        }

        /// <summary>
        /// Total시간 계산 : Lot End Time - Lot Start Time
        /// </summary>
        /// <param name="sStartTime">Lot Start 시간</param>
        /// <param name="sEndTime">Lot End 시간</param>
        public void CalTotalTime(ref TLotInfo pLotInfo, string sStartTime, string sEndTime)
        {
            DateTime dtSt;
            DateTime dtEnd;
            DateTime dtTotal;

            dtSt    = Convert.ToDateTime(sStartTime);
            dtEnd   = Convert.ToDateTime(sEndTime  );
            dtTotal = dtEnd - dtSt.TimeOfDay;

            pLotInfo.rSpcInfo.sTotalTime = dtTotal.ToString("HH:mm:ss");
        }


        /// <summary>
        /// RunTime 계산 : TotalTime - JamTime - IdleTime
        /// </summary>
        public void CalRunTime()
        {
            if(CData.SpcInfo.sIdleTime == "") {CData.SpcInfo.sIdleTime = "00:00:00"; }
            if(CData.SpcInfo.sJamTime  == "") {CData.SpcInfo.sJamTime  = "00:00:00"; }
            CData.SpcInfo.sRunTime = (Convert.ToDateTime(CData.SpcInfo.sTotalTime) - Convert.ToDateTime(CData.SpcInfo.sIdleTime).TimeOfDay - Convert.ToDateTime(CData.SpcInfo.sJamTime).TimeOfDay).ToString("HH:mm:ss");
            //190103 ksg : View에 작동이 잘 되서 Timer 변경 함.
            //CData.SpcInfo.sRunTime = (Convert.ToDateTime(CData.SpcInfo.sTotalTime) - Convert.ToDateTime(CData.IdleS).TimeOfDay - Convert.ToDateTime(CData.JamS).TimeOfDay).ToString("HH:mm:ss");
        }


        /// <summary>
        /// 지정 LOT Info에 대한 RunTime 계산 : TotalTime - JamTime - IdleTime
        /// </summary>
        public void CalRunTime(ref TLotInfo pLotInfo)
        {
            if (pLotInfo.rSpcInfo.sIdleTime == "") { pLotInfo.rSpcInfo.sIdleTime = "00:00:00"; }
            if (pLotInfo.rSpcInfo.sJamTime == "") { pLotInfo.rSpcInfo.sJamTime = "00:00:00"; }

            pLotInfo.rSpcInfo.sRunTime = (Convert.ToDateTime(pLotInfo.rSpcInfo.sTotalTime) - Convert.ToDateTime(pLotInfo.rSpcInfo.sIdleTime).TimeOfDay - Convert.ToDateTime(pLotInfo.rSpcInfo.sJamTime).TimeOfDay).ToString("HH:mm:ss");
        }


        public void CalUph()
        {
            double dTotalTime = 0.0;
            DateTime dtTotal = Convert.ToDateTime(CData.SpcInfo.sTotalTime);

            dTotalTime = dtTotal.Hour;
            dTotalTime += Convert.ToDouble(dtTotal.Minute) / 60.0;
            dTotalTime += Convert.ToDouble(dtTotal.Second) / 60.0 / 60.0;
            dTotalTime = Math.Round(dTotalTime, 2);

            CData.SpcInfo.sUph = Math.Round((Convert.ToDouble(CData.SpcInfo.iWorkStrip) / dTotalTime), 0).ToString();
        }

        public void CalUph(ref TLotInfo pLotInfo)
        {
            double dTotalTime = 0.0;
            DateTime dtTotal = Convert.ToDateTime(pLotInfo.rSpcInfo.sTotalTime);

            dTotalTime = dtTotal.Hour;
            dTotalTime += Convert.ToDouble(dtTotal.Minute) / 60.0;
            dTotalTime += Convert.ToDouble(dtTotal.Second) / 60.0 / 60.0;
            dTotalTime = Math.Round(dTotalTime, 2);

            pLotInfo.rSpcInfo.sUph = Math.Round((Convert.ToDouble(pLotInfo.rSpcInfo.iWorkStrip) / dTotalTime), 0).ToString();
        }


        public void ClearLotInfo()
        {
            CData.SpcInfo.sLotName     = "";
            CData.SpcInfo.sGrdMode     = "";
            CData.SpcInfo.sDevice      = "";
            CData.SpcInfo.sTarget      = "";
            CData.SpcInfo.sStartTime   = "";
            CData.SpcInfo.sEndTime     = "";
            CData.SpcInfo.sRunTime     = "";
            CData.SpcInfo.sIdleTime    = "";
            CData.SpcInfo.sJamTime     = "";
            CData.SpcInfo.sTotalTime   = "";
            CData.SpcInfo.sUph         = "";
            CData.SpcInfo.iWorkStrip   = 0 ;
            CData.SpcInfo.sWhlSerial_L = "";
            CData.SpcInfo.sWhlSerial_R = "";
            // 2020.12.09 JSKim St
            CData.SpcInfo.iErrCnt      = 0 ;
            // 2020.12.09 JSKim Ed
        }

        public void ClearLotData()
        {
            CData.SpcData.sLotName   = "";
            CData.SpcData.sMgzNo     = "";
            CData.SpcData.sSlotNo    = "";
            CData.SpcData.sBcr       = "";
            CData.SpcData.sTable     = "";
            CData.SpcData.sMax       = "";
            CData.SpcData.sMin       = "";
            CData.SpcData.sMean      = "";
            CData.SpcData.sTtv       = "";
            CData.SpcData.sTarget    = "";
            CData.SpcData.sTargetLim = "";
            CData.SpcData.sTtvLim    = "";
        }

        //190329 ksg :
        public bool CheckAllOpenCsv()
        {
            bool Open = true;

            if(!ChkOpenSaveDataCsv(EWay.L)) Open = false;
            if(!ChkOpenSaveDataCsv(EWay.R)) Open = false;
            return Open;
        }

        //190328 ksg :
        public bool ChkOpenSaveDataCsv(EWay eWy)
        {
            bool Open = true;

            DirectoryInfo di;
            FileInfo      fi;
            string sPath = GV.PATH_SPC + "GrdData\\";
            string sFileName;
            string sFullFileName;

            if (eWy == EWay.L)  {   sPath += "GrindLeft\\";     }
            else                {   sPath += "GrindRight\\";    }

            sPath += DateTime.Now.ToString("yyyy") + "\\";
            sPath += DateTime.Now.ToString("MM") + "\\";

            di = new DirectoryInfo(sPath);

            if (di.Exists == false)
            {
                Open = false;
                return Open;
            }
            sPath    += DateTime.Now.ToString("dd") + ".csv";
            sFileName = DateTime.Now.ToString("dd") + ".csv";

            fi = new FileInfo(sPath);

            if (fi.Exists == false)
            {
                Open = false;
                return Open;
            }

            sFullFileName = sFileName + " - Excel";
            var excelProcesses = Process.GetProcessesByName("excel");
            foreach (var process in excelProcesses)
            {
                if (process.MainWindowTitle == $"Microsoft Excel - {sFileName}") // String.Format for pre-C# 6.0 
                {
                    process.Kill();
                    Open = true;
                    return Open;
                }
                else if (process.MainWindowTitle == sFullFileName) //190501 ksg : Excel 파일 Close 조건 추가
                {
                    process.Kill();
                    Open = true;
                    return Open;
                }
                else 
                {
                    Open = false;
                    return Open;
                }
            }
            return Open;
        }

        public bool ChkOpenLotDataCsv(EWay eWy)
        {
            bool Open = true;

            string sPath = GV.PATH_SPC + "LotLog\\";
            string sLotName = "";

            DirectoryInfo di;
            FileInfo fi;
            //koo 191002 : Speed Write 사용하지 않아 삭제 StreamWriter sw;

            sLotName = CData.LotInfo.sLotName;

            sPath += DateTime.Now.ToString("yyyyMMdd") + "\\";
            sPath += sLotName + "\\";

            di = new DirectoryInfo(sPath);

            if (di.Exists == false)
            {
                Open = false;
            }

            if (eWy == EWay.L) sPath += "GrindLeft.csv";
            else               sPath += "GrindRight.csv";

            fi = new FileInfo(sPath);

            if (fi.Exists == false)
            {
                Open = false;
            }


            return Open;
        }

        //190328 ksg :
        public bool ChkOpenLotInfoCsv(EWay eWy)
        {
            bool Open = true;

            string sPath        = "";
            string sLotName     = "";

            DirectoryInfo dI;
            FileInfo fI;
            //koo 191002 : Speed Write 사용하지 않아 삭제 StreamWriter sw;
            
            //Lot Name
            sLotName = CData.SpcInfo.sLotName;

            sPath = GV.PATH_SPC;
            sPath += "LotLog\\";
            sPath += DateTime.Now.ToString("yyyyMMdd") + "\\";
            sPath += sLotName + "\\";

            dI = new DirectoryInfo(sPath);

            if (!dI.Exists)
            { 
                Open = false;
                return Open;
            }

            sPath += "LotInfo.csv";

            fI = new FileInfo(sPath);
            if (!fI.Exists)
            {
                Open = false;
                return Open;
            }            

            return Open;
        }

        // 2021.12.28 lhs Start : 사용하는 곳이 없어 주석처리
        //public bool ChkOpenLotData()
        //{
        //    bool Open = true;

        //    string sPath        = "";
        //    string sLotName     = "";

        //    DirectoryInfo dtInfo;
        //    FileInfo fiInfo;

        //    sLotName = CData.SpcData.sLotName;

        //    sPath = GV.PATH_SPC;
        //    sPath += "LotLog\\";
        //    sPath += DateTime.Now.ToString("yyyyMMdd") + "\\";
        //    sPath += sLotName + "\\";

        //    dtInfo = new DirectoryInfo(sPath);
        //    if (!dtInfo.Exists)
        //    { 
        //        Open = false;
        //        return Open;
        //    }

        //    sPath += CData.SpcData.sLotName + ".csv";

        //    fiInfo = new FileInfo(sPath);
            
        //    if (!fiInfo.Exists)
        //    {
        //        Open = false;
        //        return Open;
        //    }

        //    return Open;
        //}
        // 2021.12.28 lhs End : 사용하는 곳이 없어 주석처리

        //200213 ksg :
        public bool ChkFindSameLot(string LotName)
        {
            bool bFind = false;
            string sPath;
            DirectoryInfo dI;

            sPath = GV.PATH_SPC;
            sPath += "LotLog\\";
            sPath += DateTime.Now.ToString("yyyyMMdd") + "\\";

            // 2020.09.11 JSKim St
            // 2020.10.21 JSKim St
            //if (CData.CurCompany == ECompany.SCK)
            if (CData.CurCompany == ECompany.SCK || CData.CurCompany == ECompany.JSCK)
            // 2020.10.21 JSKim Ed
            {
                LotName += "_" + DateTime.Now.Hour.ToString("00"); //20200827 lks
            }
            // 2020.09.11 JSKim Ed
            sPath += LotName + "\\";

            // 2021.06.22 lhs Start : 예외 발생시 true를 리턴하여 에러 발생 유도
            //dI = new DirectoryInfo(sPath);
            try
            {
                dI = new DirectoryInfo(sPath);
            }
            catch(Exception e)
            {
                return true;
			}
            // 2021.06.22 lhs End

            if (dI.Exists)
            {
                bFind = true;
            }

            return bFind;
        }

        private void _SetLog(string sMsg)
        {
            StackTrace mTrace = new StackTrace();
            StackFrame mFrame = mTrace.GetFrame(1);

            string sMth = mFrame.GetMethod().Name.PadRight(20);

            //200824 jhc : 
            string sStat = CSQ_Main.It.m_iStat.ToString();
            if (CSQ_Main.It.m_iStat == EStatus.Error)
            {
                sStat = string.Format("{0}[{1}]", sStat, (CData.iErrNo + 1).ToString("000"));
            }
            sStat.PadRight(20);

            string sLog = string.Format("{0}\t[{1}()]\t{2}", sStat, sMth, sMsg);
            CLog.Save_Log(eLog.None, eLog.DSL, sLog);
        }

        //211210 pjh : 중복 Lot Name 발생 시 별도 Lot Name 만드는 함수
        public void SaveNewLotName(string sNewLotName, string sLotName)
        {
            string sNewName = "";
            string sPath = GV.PATH_SPC;
            DirectoryInfo di;
            string[] aNum = sNewLotName.Split('_');

            if (aNum.Length < 2) iAddNum = 0;
            else iAddNum = int.Parse(aNum[1]);

            sPath += "LotLog\\";
            sPath += DateTime.Now.ToString("yyyyMMdd") + "\\";
            sPath += sNewLotName + "\\";

            di = new DirectoryInfo(sPath);

            if(di.Exists)
            {
                iAddNum++;
                sNewName = sLotName + "_" + iAddNum.ToString();
                
                CData.LotInfo.sNewLotName = sNewName;
            }
        }
        //
    }
}
