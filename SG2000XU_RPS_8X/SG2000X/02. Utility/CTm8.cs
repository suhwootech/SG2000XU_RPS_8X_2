using System;
using System.Collections;
using System.IO;
using System.IO.Ports;
using System.Text;

namespace SG2000X
{
    public class CTableInfo
    {
        public string sName;
        public string sComment;
        public double dShortLength;
        public double dLongLength;
        public int nDivCol;
        public int nDivRow;

        // Table GridView 내용
        public int[] aTbl_RepeatCnt = new int[2];    // 반복횟수 0=Left, 1=Right
        public int[] aTbl_MeasCnt = new int[2];      // 측정갯수 0=Left, 1=Right

        public ArrayList alLTbl_Colidx = new ArrayList();
        public ArrayList alLTbl_Rowidx = new ArrayList();
        public ArrayList alRTbl_Colidx = new ArrayList();
        public ArrayList alRTbl_Rowidx = new ArrayList();

        public ArrayList alLTbl_X = new ArrayList();
        public ArrayList alLTbl_Y = new ArrayList();
        public ArrayList alRTbl_X = new ArrayList();
        public ArrayList alRTbl_Y = new ArrayList();

    }


    public class CTm8 : CStn<CTm8>
    {
        /// <summary>
        /// Tm8 : Table Measure 8 points
        /// </summary>
        
        public string sTm8_File;

        public int m_nSelectedTable = -1;
        public ArrayList m_alTableInfo = new ArrayList();

        public CTableInfo mTable = new CTableInfo();

        // Table 측정값 저장 변수
        public const int m_nMaxRept = 50;
        public const int m_nMaxPt = 50;
        public double[,] m_aLTbl_RltZ = new double[m_nMaxRept, m_nMaxPt]; // [반복, #Point]
        public double[,] m_aRTbl_RltZ = new double[m_nMaxRept, m_nMaxPt]; // [반복, #Point]

        public int[] m_nAcqReptCnt = new int[2]; // Left, Right  // 반복측정 취득 갯수
        public int[] m_nAcqPtCnt = new int[2]; // Left, Right  // 측정포인트 취득 갯수
        public bool[] m_bAcqed = new bool[2];    // Left, Right  // 측정포인트 데이터 취득 여부

        private CTm8()
        {
            Initial();
        }

        public void Initial()
        {
            sTm8_File = GV.PATH_EQ_TM8 + "TableInfo.txt";

            //Load();
            //Initiallize();
        }

        public void Initiallize()
        {
        }

        public void Release()
        {
        }

        public int Save()
        {
            string sPath = sTm8_File;
            if (!Directory.Exists(GV.PATH_EQ_TM8)) { Directory.CreateDirectory(GV.PATH_EQ_TM8); }

            StringBuilder mSB = new StringBuilder();

            int nTblCnt = m_alTableInfo.Count;

            mSB.AppendLine("[Info]");
            mSB.AppendLine("Table Count=" + nTblCnt);
            mSB.AppendLine("Selected Table=" + m_nSelectedTable);

            string sNum = "";
            string sSec = "";
            for (int i = 0; i < nTblCnt; i++)
            {
                CTableInfo ti = m_alTableInfo[i] as CTableInfo;

                sNum = String.Format("{0:D2}", i + 1);
                sSec = "[Table_" + sNum + "]";
                mSB.AppendLine(); // 빈라인
                mSB.AppendLine(sSec);

                mSB.AppendLine("Name=" + ti.sName);
                mSB.AppendLine("Comment=" + ti.sComment);
                mSB.AppendLine("Short Length=" + ti.dShortLength);
                mSB.AppendLine("Long Length=" + ti.dLongLength);
                mSB.AppendLine("Divide Column=" + ti.nDivCol);
                mSB.AppendLine("Divide Row=" + ti.nDivRow);

                string sIdx = "";
                string sPtCol = "";
                string sPtRow = "";
                
                // Left Table GridView 내용
                mSB.AppendLine();
                mSB.AppendLine("LT Repeat Count=" + ti.aTbl_RepeatCnt[0]);
                mSB.AppendLine("LT MeasPoint Count=" + ti.aTbl_MeasCnt[0]);
                for (int nIdx = 0; nIdx < ti.aTbl_MeasCnt[0]; nIdx++)
                {
                    sIdx = String.Format("{0:D2}", nIdx + 1);

                    sPtCol = "LT Point_" + sIdx + "_Colidx=";
                    sPtRow = "LT Point_" + sIdx + "_Rowidx=";
                    mSB.AppendLine(sPtCol + ti.alLTbl_Colidx[nIdx]);
                    mSB.AppendLine(sPtRow + ti.alLTbl_Rowidx[nIdx]);

                    sPtCol = "LT Point_" + sIdx + "_X=";
                    sPtRow = "LT Point_" + sIdx + "_Y=";
                    mSB.AppendLine(sPtCol + ti.alLTbl_X[nIdx]);
                    mSB.AppendLine(sPtRow + ti.alLTbl_Y[nIdx]);
                }

                // Right Table GridView 내용
                mSB.AppendLine();
                mSB.AppendLine("RT Repeat Count=" + ti.aTbl_RepeatCnt[1]);
                mSB.AppendLine("RT MeasPoint Count=" + ti.aTbl_MeasCnt[1]);
                for (int nIdx = 0; nIdx < ti.aTbl_MeasCnt[1]; nIdx++)
                {
                    sIdx = String.Format("{0:D2}", nIdx + 1);

                    sPtCol = "RT Point_" + sIdx + "_Colidx=";
                    sPtRow = "RT Point_" + sIdx + "_Rowidx=";
                    mSB.AppendLine(sPtCol + ti.alRTbl_Colidx[nIdx]);
                    mSB.AppendLine(sPtRow + ti.alRTbl_Rowidx[nIdx]);

                    sPtCol = "RT Point_" + sIdx + "_X=";
                    sPtRow = "RT Point_" + sIdx + "_Y=";
                    mSB.AppendLine(sPtCol + ti.alRTbl_X[nIdx]);
                    mSB.AppendLine(sPtRow + ti.alRTbl_Y[nIdx]);
                }
            }

            //200213 ksg :
            CLog.Check_File_Access(sPath, mSB.ToString(), false);
            return 0;
        }

        public CTableInfo MakeDefaultTable()
        {
            CTableInfo ti = new CTableInfo();

            ti.sName = "Table Name 01";
            ti.sComment = "Table Comment 01";
            ti.dShortLength = 200.0;    //mm
            ti.dLongLength = 400.0;     //mm
            ti.nDivCol = 13;            //ea    // 홀수만
            ti.nDivRow = 25;            //ea    // 홀수만
            ti.aTbl_RepeatCnt[0] = 5;
            ti.aTbl_RepeatCnt[1] = 5;
            ti.aTbl_MeasCnt[0] = 8;
            ti.aTbl_MeasCnt[1] = 8;

            if (ti.nDivCol % 2 == 0) ti.nDivCol++;  // 홀수만
            if (ti.nDivRow % 2 == 0) ti.nDivRow++;
            if (ti.nDivCol < 7) ti.nDivCol = 7;     // 최소 7개 이상으로
            if (ti.nDivRow < 7) ti.nDivRow = 7;

            for (int nIdx = 0; nIdx < ti.aTbl_MeasCnt[0]; nIdx++)
            {
                ti.alLTbl_Colidx.Add(DefaultColIdx(nIdx, ti.nDivCol));
                ti.alLTbl_Rowidx.Add(DefaultRowIdx(nIdx, ti.nDivRow));
                ti.alLTbl_X.Add(0.0);
                ti.alLTbl_Y.Add(0.0);
            }
            for (int nIdx = 0; nIdx < ti.aTbl_MeasCnt[1]; nIdx++)
            {
                ti.alRTbl_Colidx.Add(DefaultColIdx(nIdx, ti.nDivCol));
                ti.alRTbl_Rowidx.Add(DefaultRowIdx(nIdx, ti.nDivRow));
                ti.alRTbl_X.Add(0.0);
                ti.alRTbl_Y.Add(0.0);
            }
            CalcMeasPoints(ref ti);

            return ti;
        }

        public int SaveDefault()
        {
            string sPath = sTm8_File;
            if (!Directory.Exists(GV.PATH_EQ_TM8)) { Directory.CreateDirectory(GV.PATH_EQ_TM8); }

            StringBuilder mSB = new StringBuilder();

            CTableInfo ti = MakeDefaultTable();

            int nTblCnt = 1;    // Table 1개만...
            //------------------------------------

            mSB.AppendLine("[Info]");
            mSB.AppendLine("Table Count=" + nTblCnt);
            mSB.AppendLine("Selected Table=" + (nTblCnt-1));

            string sNum = "";
            string sSec = "";
            for (int i = 0; i < nTblCnt; i++)
            {
                // Section
                sNum = String.Format("{0:D2}", i + 1);
                sSec = "[Table_" + sNum + "]";
                mSB.AppendLine(); // 빈라인
                mSB.AppendLine(sSec);

                mSB.AppendLine("Name=" + ti.sName);
                mSB.AppendLine("Comment=" + ti.sComment);
                mSB.AppendLine("Short Length=" + ti.dShortLength);
                mSB.AppendLine("Long Length=" + ti.dLongLength);
                
                mSB.AppendLine("Divide Column=" + ti.nDivCol);
                mSB.AppendLine("Divide Row=" + ti.nDivRow);

                string sIdx = "";
                string sPtCol = "";
                string sPtRow = "";

                // Left Table GridView 내용
                mSB.AppendLine();
                mSB.AppendLine("LT Repeat Count=" + ti.aTbl_RepeatCnt[0]);
                mSB.AppendLine("LT MeasPoint Count=" + ti.aTbl_MeasCnt[0]);
                for (int nIdx = 0; nIdx < ti.aTbl_MeasCnt[0]; nIdx++)
                {
                    sIdx = String.Format("{0:D2}", nIdx + 1);

                    sPtCol = "LT Point_" + sIdx + "_Colidx=";
                    sPtRow = "LT Point_" + sIdx + "_Rowidx=";
                    mSB.AppendLine(sPtCol + ti.alLTbl_Colidx[nIdx]);
                    mSB.AppendLine(sPtRow + ti.alLTbl_Rowidx[nIdx]);

                    sPtCol = "LT Point_" + sIdx + "_X=";
                    sPtRow = "LT Point_" + sIdx + "_Y=";
                    mSB.AppendLine(sPtCol + ti.alLTbl_X[nIdx]);
                    mSB.AppendLine(sPtRow + ti.alLTbl_Y[nIdx]);
                }

                // Right Table GridView 내용
                mSB.AppendLine();
                mSB.AppendLine("RT Repeat Count=" + ti.aTbl_RepeatCnt[1]);
                mSB.AppendLine("RT MeasPoint Count=" + ti.aTbl_MeasCnt[1]);
                for (int nIdx = 0; nIdx < ti.aTbl_MeasCnt[1]; nIdx++)
                {
                    sIdx = String.Format("{0:D2}", nIdx + 1);

                    sPtCol = "RT Point_" + sIdx + "_Colidx=";
                    sPtRow = "RT Point_" + sIdx + "_Rowidx=";
                    mSB.AppendLine(sPtCol + ti.alRTbl_Colidx[nIdx]);
                    mSB.AppendLine(sPtRow + ti.alRTbl_Rowidx[nIdx]);

                    sPtCol = "RT Point_" + sIdx + "_X=";
                    sPtRow = "RT Point_" + sIdx + "_Y=";
                    mSB.AppendLine(sPtCol + ti.alRTbl_X[nIdx]);
                    mSB.AppendLine(sPtRow + ti.alRTbl_Y[nIdx]);
                }
            }
            //200213 ksg :
            CLog.Check_File_Access(sPath, mSB.ToString(), false);
            return 0;
        }


        public int SaveResult(string TableName, string sText)
        {
            DateTime dtNow = DateTime.Now;
            string sTime = dtNow.ToString("yyyy.MM.dd_HHmmss");
            string sPath = GV.PATH_EQ_TM8 + "Result\\";
            string sFile = sPath + TableName + "_" + sTime + ".txt";
            StringBuilder mSB = new StringBuilder();
            if (!Directory.Exists(sPath)) { Directory.CreateDirectory(sPath); }
            mSB.AppendLine(sText);

            CLog.Check_File_Access(sFile, mSB.ToString(), false);
            return 0;
        }

        private int DefaultColIdx(int nIdx, int nDivCol)
        {
            int nColIdx = 0;
            switch (nIdx)
            {
                //----------- Left Table : 왼쪽 3점
                case 0:
                case 1:
                case 2: nColIdx = 2;            break;
                //----------- Left Table 가운데 2점
                case 3:
                case 4: nColIdx = nDivCol / 2;  break;
                //----------- Left Table 오른쪽 3점
                case 5:
                case 6:
                case 7: nColIdx = nDivCol-3;    break;
                //----------- 
            }
            return nColIdx;
        }

        private int DefaultRowIdx(int nIdx, int nDivRow)
        {
            int nRowIdx = 0;
            switch (nIdx)
            {
                //----------- Left Table 왼쪽 3점
                case 0: nRowIdx = 2;            break;
                case 1: nRowIdx = nDivRow / 2;  break;
                case 2: nRowIdx = nDivRow-3;    break;
                //----------- Left Table 가운데 2점
                case 3: nRowIdx = 2;            break;
                case 4: nRowIdx = nDivRow - 3;  break;
                //----------- Left Table 오른쪽 3점
                case 5: nRowIdx = 2;            break;
                case 6: nRowIdx = nDivRow / 2;  break;
                case 7: nRowIdx = nDivRow - 3;  break;
                //----------- 
            }
            return nRowIdx;
        }

        public void CalcMeasPoints(ref CTableInfo ti)
        {
            double dShortLen = ti.dShortLength;
            double dLongLen = ti.dLongLength;

            int nDivCol = ti.nDivCol;
            int nDivRow = ti.nDivRow;

            double dColW = dShortLen / nDivCol;  // 1개의 가로길이
            double dRowH = dLongLen / nDivRow;   // 1개의 세로길이

            int nCen_Colidx = nDivCol / 2;   // 중심
            int nCen_Rowidx = nDivRow / 2;   // 중심

            for (int n = 0; n < ti.aTbl_MeasCnt[0]; n++)
            {
                int nDiffCol = (int)ti.alLTbl_Colidx[n] - nCen_Colidx;
                int nDiffRow = (int)ti.alLTbl_Rowidx[n] - nCen_Rowidx;
                ti.alLTbl_X[n] = CData.SPos.dGRD_X_Zero[0] + (double)nDiffCol * dColW;
                ti.alLTbl_Y[n] = CData.MPos[0].dY_PRB_TBL_CENTER + (double)nDiffRow * dRowH;
            }
            for (int n = 0; n < ti.aTbl_MeasCnt[1]; n++)
            {
                int nDiffCol = (int)ti.alRTbl_Colidx[n] - nCen_Colidx;
                int nDiffRow = (int)ti.alRTbl_Rowidx[n] - nCen_Rowidx;
                ti.alRTbl_X[n] = CData.SPos.dGRD_X_Zero[1] - (double)nDiffCol * dColW;  // 좌우 대칭
                ti.alRTbl_Y[n] = CData.MPos[1].dY_PRB_TBL_CENTER + (double)nDiffRow * dRowH;
            }
        }


        public int Load()
        {
            if (!File.Exists(sTm8_File))
            {
                // 디폴트 파일생성
                SaveDefault();
            }

            CIni mIni = new CIni(sTm8_File);
            string sSec = "";
            //-----------------
            sSec = "Info";
            int nTableCnt = mIni.ReadI(sSec, "Table Count");
            if (nTableCnt < 1)
            {
                // 파일 지우고 다시 Load()
                File.Delete(sTm8_File);
                Load();
                return 0;
            }
            m_nSelectedTable = mIni.ReadI(sSec, "Selected Table");
            
            //-----------------
            m_alTableInfo.Clear();
            string sNum = "";

            for (int i=0; i< nTableCnt; i++)
            {
                sNum = String.Format("{0:D2}", i + 1);
                sSec = "Table_" + sNum;
                ReadSection(ref mIni, sSec);
            }
            int nCnt = m_alTableInfo.Count;
            //-----------------
            return 0;
        }

        private void ReadSection(ref CIni Ini, string sSec)
        {
            CTableInfo ti = new CTableInfo();

            ti.sName = Ini.Read(sSec, "Name").Trim();
            ti.sComment = Ini.Read(sSec, "Comment").Trim();
            ti.dShortLength = Ini.ReadD(sSec, "Short Length");
            ti.dLongLength = Ini.ReadD(sSec, "Long Length");
            ti.nDivCol = Ini.ReadI(sSec, "Divide Column");
            ti.nDivRow = Ini.ReadI(sSec, "Divide Row");

            string sIdx = "";
            string sPtCol = "";
            string sPtRow = "";

            // Left Table GridView 내용
            ti.aTbl_RepeatCnt[0] = Ini.ReadI(sSec, "LT Repeat Count");
            ti.aTbl_MeasCnt[0] = Ini.ReadI(sSec, "LT MeasPoint Count");
            for (int nIdx = 0; nIdx < ti.aTbl_MeasCnt[0]; nIdx++)
            {
                sIdx = String.Format("{0:D2}", nIdx + 1);

                sPtCol = "LT Point_" + sIdx + "_Colidx";
                sPtRow = "LT Point_" + sIdx + "_Rowidx";
                ti.alLTbl_Colidx.Add(Ini.ReadI(sSec, sPtCol));
                ti.alLTbl_Rowidx.Add(Ini.ReadI(sSec, sPtRow));

                sPtCol = "LT Point_" + sIdx + "_X";
                sPtRow = "LT Point_" + sIdx + "_Y";
                ti.alLTbl_X.Add(Ini.ReadD(sSec, sPtCol));
                ti.alLTbl_Y.Add(Ini.ReadD(sSec, sPtRow));
            }

            // Right Table GridView 내용
            ti.aTbl_RepeatCnt[1] = Ini.ReadI(sSec, "RT Repeat Count");
            ti.aTbl_MeasCnt[1] = Ini.ReadI(sSec, "RT MeasPoint Count");
            for (int nIdx = 0; nIdx < ti.aTbl_MeasCnt[1]; nIdx++)
            {
                sIdx = String.Format("{0:D2}", nIdx + 1);

                sPtCol = "RT Point_" + sIdx + "_Colidx";
                sPtRow = "RT Point_" + sIdx + "_Rowidx";
                ti.alRTbl_Colidx.Add(Ini.ReadI(sSec, sPtCol));
                ti.alRTbl_Rowidx.Add(Ini.ReadI(sSec, sPtRow));

                sPtCol = "RT Point_" + sIdx + "_X";
                sPtRow = "RT Point_" + sIdx + "_Y";
                ti.alRTbl_X.Add(Ini.ReadD(sSec, sPtCol));
                ti.alRTbl_Y.Add(Ini.ReadD(sSec, sPtRow));
            }

            m_alTableInfo.Add(ti);
        }



    }
}



