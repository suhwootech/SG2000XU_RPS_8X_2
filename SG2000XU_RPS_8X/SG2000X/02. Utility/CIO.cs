using System;
using System.IO;
using wmxCLRLibrary;

namespace SG2000X
{
    public class CIO : CStn<CIO>
    {
        /// <summary>
        /// Main input address
        /// </summary>
        public  int MAIN_IN_ADDR = 30;           // 메인 어드레스
                                                         /// <summary>
                                                         /// Main input size
                                                         /// </summary>
        public readonly int MAIN_IN_SIZE = 144;          // 메인 사이즈
                                                         /// <summary>
                                                         /// Loader input address
                                                         /// </summary>
        public  int LDR_IN_ADDR = 121;           // 로더 어드레스
                                                         /// <summary>
                                                         /// Loader input size
                                                         /// </summary>
        public readonly int LDR_IN_SIZE = 32;            // 로더 사이즈
                                                         /// <summary>
                                                         /// Off loader input address
                                                         /// </summary>
        public  int ODR_IN_ADDR = 143;           // 언로더 어드레스
                                                         /// <summary>
                                                         /// Off loader input size
                                                         /// </summary>
        public readonly int ODR_IN_SIZE = 32;            // 언로더 사이즈

        /// <summary>
        /// Main output address
        /// </summary>
        public int MAIN_OUT_ADDR = 2;           // 메인 어드레스
                                                /// <summary>
                                                /// Main output size
                                                /// </summary>
        public int MAIN_OUT_SIZE = 144;         // 메인 사이즈
                                                /// <summary>
                                                /// Loader output address
                                                /// </summary>
        public int LDR_OUT_ADDR = 22;           // 로더 어드레스
                                                /// <summary>
                                                /// Loader output size
                                                /// </summary>
        public int LDR_OUT_SIZE = 32;           // 로더 사이즈
                                                /// <summary>
                                                /// Off loader output address
                                                /// </summary>
        public int ODR_OUT_ADDR = 28;           // 언로더 어드레스
                                                /// <summary>
                                                /// Off loader output size
                                                /// </summary>
        public int ODR_OUT_SIZE = 32;           // 언로더 사이즈

        #region String Array
        /// <summary>
        /// String[] Array
        /// </summary>
        public string[] aMain_InTxt; //메인 인풋 텍스트
        public string[] aLdr_InTxt;  //로더 인풋 텍스트
        public string[] aOdr_InTxt;  //언로더 인풋 텍스트

        public string[] aMain_OutTxt; //메인 아웃풋 텍스트
        public string[] aLdr_OutTxt;  //로더 아웃풋 텍스트
        public string[] aOdr_OutTxt;  //언로더 아웃풋 텍스트
        #endregion
        #region
        /// <summary>
        /// int[] array
        /// </summary>
        public int[] aInput;
        //public int[] aMain_InVal;  //메인 데이터
        //public int[] aLdr_InVal;  //로더 데이터
        //public int[] aOdr_InVal;  //언로더 데이터

        public int[] aMain_OutVal;  //메인 데이터
        public int[] aLdr_OutVal;  //로더 데이터
        public int[] aOdr_OutVal;  //언로더 데이터
        #endregion
        private CIO()
        {
            //190524 ksg :
            //if(CDataOption.CurEqu == eEquType.Nomal) 
            if(CDataOption.CurEqu == eEquType.Nomal) 
            {
                MAIN_IN_ADDR = 30;           // 메인 어드레스
                LDR_IN_ADDR = 121;           // 로더 어드레스
                ODR_IN_ADDR = 143;           // 언로더 어드레스
            }
            else 
            {
                MAIN_IN_ADDR = 34;           // 메인 어드레스
                LDR_IN_ADDR = 125;           // 로더 어드레스
                ODR_IN_ADDR = 147;           // 언로더 어드레스
            }
            aInput      = new int[MAIN_IN_SIZE + LDR_IN_SIZE + ODR_IN_SIZE];
            //aMain_InVal = new int[MAIN_IN_SIZE                            ];
            //aLdr_InVal  = new int[LDR_IN_SIZE                             ];
            //aOdr_InVal  = new int[ODR_IN_SIZE                             ];

            aMain_OutVal = new int[MAIN_OUT_SIZE                          ];
            aLdr_OutVal  = new int[LDR_OUT_SIZE                           ];
            aOdr_OutVal  = new int[ODR_OUT_SIZE                           ];
        }

        /// <summary>
        /// Initial class
        /// </summary>
        public void Init()
        {
            Get_Txt();
        }

        /// <summary>
        /// Release class
        /// </summary>
        public void Rele()
        {

        }

        public void Get_IO()
        {
            Get_X(ref aInput);

            //Get_X(MAIN_IN_ADDR, MAIN_IN_SIZE, ref aMain_InVal);
            //Get_X(LDR_IN_ADDR, LDR_IN_SIZE, ref aLdr_InVal);
            //Get_X(ODR_IN_ADDR, ODR_IN_SIZE, ref aOdr_InVal);

            Get_Y(MAIN_OUT_ADDR, MAIN_OUT_SIZE, ref aMain_OutVal);
            Get_Y(LDR_OUT_ADDR, LDR_OUT_SIZE, ref aLdr_OutVal);
            Get_Y(ODR_OUT_ADDR, ODR_OUT_SIZE, ref aOdr_OutVal);
        }

        public void Get_Txt()
        {
            string sPath = GV.PATH_EQ_IO;  //실행 파일의 인풋, 아웃풋 등 데이터 경로 가져오기
            string sFullPath;
            string sVal;  //스트링 변수
            string[] aTmp;  //스트링 배열 변수
            string[] aIMain;
            string[] aIOn;
            string[] aIOff;
            string[] aOMain;
            string[] aOOn;
            string[] aOOff;

            // Input - Main
            //     if ((int)eLanguage.English == Constants.LANGUAGE) sFullPath = sPath + "Input_Main.txt"          ;
            //else if ((int)eLanguage.Korea   == Constants.LANGUAGE) sFullPath = sPath + "Input_Main.txt"          ;
            //else if ((int)eLanguage.China   == Constants.LANGUAGE) sFullPath = sPath + "Input_Main - Chinese.txt";

            //if ((int)ELang.English == CData.Opt.iSelLan) sFullPath = sPath + "Input_Main.txt"          ;
            //syc : I/O U,X 구분
            if ((int)ELang.English == CData.Opt.iSelLan)
            {
                if (CDataOption.Package == ePkg.Unit)//syc : I/O U,X 구분
                {
                    string sCheck = sPath + "Input_Main_U.txt";
                    FileInfo filecheck = new FileInfo(sCheck);                   
                    if (filecheck.Exists)
                    {
                        sFullPath = sPath + "Input_Main_U.txt";
                    }
                    else
                    {
                        sFullPath = sPath + "Input_Main.txt";
                    }
                }
                //210902 syc : 2004U
                else if (CDataOption.Use2004U)
                {
                    string sCheck = sPath + "Input_Main_4U.txt";
                    FileInfo filecheck = new FileInfo(sCheck);
                    if (filecheck.Exists)
                    {
                        sFullPath = sPath + "Input_Main_4U.txt";
                    }
                    else
                    {
                        sFullPath = sPath + "Input_Main.txt";
                    }
                }
                else
                {
                    sFullPath = sPath + "Input_Main.txt";
                }
            }
            else if ((int)ELang.Korea   == CData.Opt.iSelLan) sFullPath = sPath + "Input_Main.txt"          ;
            else                                                  sFullPath = sPath + "Input_Main - Chinese.txt";
            //StreamReader sReader = new StreamReader(sPath + "Input_Main.txt");  //메인 인풋 텍스트 읽어들이기
            StreamReader sReader = new StreamReader(sFullPath);  //메인 인풋 텍스트 읽어들이기
            sVal = sReader.ReadToEnd();  //텍스트 끝까지 읽기
            sVal = sVal.Replace("\t", "    ");  //텍스트 띄움(가로 탭)
            aTmp = sVal.Replace("\r", "").Split("\n".ToCharArray());  //텍스트박스 줄바꿈
            aMain_InTxt = new string[aTmp.Length - 1];  //메인 인풋 인덱스 길이
            Array.Copy(aTmp, aMain_InTxt, aTmp.Length - 1);  //배열 카피 
            sReader.Close();

            // Output - Main
            //     if ((int)eLanguage.English == Constants.LANGUAGE) sFullPath = sPath + "Output_Main.txt"          ;
            //else if ((int)eLanguage.Korea   == Constants.LANGUAGE) sFullPath = sPath + "Output_Main.txt"          ;
            //else if ((int)eLanguage.China   == Constants.LANGUAGE) sFullPath = sPath + "Output_Main - Chinese.txt";

            //if ((int)ELang.English == CData.Opt.iSelLan) sFullPath = sPath + "Output_Main.txt"          ;
            //syc : I/O U,X 구분
            if ((int)ELang.English == CData.Opt.iSelLan)
            {
                if (CDataOption.Package == ePkg.Unit)
                {
                    string sCheck = sPath + "Output_Main_U.txt";
                    FileInfo filecheck = new FileInfo(sCheck);
                    if (filecheck.Exists)
                    {
                        sFullPath = sPath + "Output_Main_U.txt";
                    }
                    else
                    {
                        sFullPath = sPath + "Output_Main.txt";
                    }
                }
                //210902 syc : 2004U
                else if (CDataOption.Use2004U)
                {
                    string sCheck = sPath + "Output_Main_4U.txt";
                    FileInfo filecheck = new FileInfo(sCheck);
                    if (filecheck.Exists)
                    {
                        sFullPath = sPath + "Output_Main_4U.txt"; 
                    }
                    else
                    {
                        sFullPath = sPath + "Output_Main.txt";
                    }
                }
                else
                {
                    sFullPath = sPath + "Output_Main.txt";
                }
            }
            else if ((int)ELang.Korea   == CData.Opt.iSelLan) sFullPath = sPath + "Output_Main.txt"          ;
            else if ((int)ELang.China   == CData.Opt.iSelLan) sFullPath = sPath + "Output_Main - Chinese.txt";
            //sReader = new StreamReader(sPath + "Output_Main.txt");  //메인 아웃풋 텍스트 읽어들이기
            sReader = new StreamReader(sFullPath);  //메인 아웃풋 텍스트 읽어들이기
            sVal = sReader.ReadToEnd();  //텍스트 끝까지 읽기
            sVal = sVal.Replace("\t", "    ");  //텍스트 띄움(가로 탭)
            aTmp = sVal.Replace("\r", "").Split("\n".ToCharArray());  //텍스트박스 줄바꿈 
            aMain_OutTxt = new string[aTmp.Length - 1];  //메인 아웃풋 인덱스 길이
            Array.Copy(aTmp, aMain_OutTxt, aTmp.Length - 1);  //배열 카피
            sReader.Close();

            // Input - On Loader
            //     if ((int)eLanguage.English == Constants.LANGUAGE) sFullPath = sPath + "Input_OnLoader.txt"          ;
            //else if ((int)eLanguage.Korea   == Constants.LANGUAGE) sFullPath = sPath + "Input_OnLoader.txt"          ;
            //else if ((int)eLanguage.China   == Constants.LANGUAGE) sFullPath = sPath + "Input_OnLoader - Chinese.txt";
                 if ((int)ELang.English == CData.Opt.iSelLan) sFullPath = sPath + "Input_OnLoader.txt"          ;
            else if ((int)ELang.Korea   == CData.Opt.iSelLan) sFullPath = sPath + "Input_OnLoader.txt"          ;
            else if ((int)ELang.China   == CData.Opt.iSelLan) sFullPath = sPath + "Input_OnLoader - Chinese.txt";
            //else if ((int)eLanguage.China   == Constants.LANGUAGE) sFullPath = sPath + "Input_OnLoader.txt"          ;//sFullPath = sPath + "Input_OnLoader - Chinese.txt";
            //sReader = new StreamReader(sPath + "Input_OnLoader.txt");
            sReader = new StreamReader(sFullPath);
            sVal = sReader.ReadToEnd();
            sVal = sVal.Replace("\t", "    ");
            aTmp = sVal.Replace("\r", "").Split("\n".ToCharArray());
            aLdr_InTxt = new string[aTmp.Length - 1];
            Array.Copy(aTmp, aLdr_InTxt, aTmp.Length - 1);
            sReader.Close();

            // Output - On Loader
            //     if ((int)eLanguage.English == Constants.LANGUAGE) sFullPath = sPath + "Output_OnLoader.txt"          ;
            //else if ((int)eLanguage.Korea   == Constants.LANGUAGE) sFullPath = sPath + "Output_OnLoader.txt"          ;
            //else if ((int)eLanguage.China   == Constants.LANGUAGE) sFullPath = sPath + "Output_OnLoader - Chinese.txt";
                 if ((int)ELang.English == CData.Opt.iSelLan) sFullPath = sPath + "Output_OnLoader.txt"          ;
            else if ((int)ELang.Korea   == CData.Opt.iSelLan) sFullPath = sPath + "Output_OnLoader.txt"          ;
            else if ((int)ELang.China   == CData.Opt.iSelLan) sFullPath = sPath + "Output_OnLoader - Chinese.txt";
            //else if ((int)eLanguage.China   == Constants.LANGUAGE) sFullPath = sPath + "Output_OnLoader.txt"          ;//sFullPath = sPath + "Output_OnLoader - Chinese.txt";
            //sReader = new StreamReader(sPath + "Output_OnLoader.txt");
            sReader = new StreamReader(sFullPath);
            sVal = sReader.ReadToEnd();
            sVal = sVal.Replace("\t", "    ");
            aTmp = sVal.Replace("\r", "").Split("\n".ToCharArray());
            aLdr_OutTxt = new string[aTmp.Length - 1];
            Array.Copy(aTmp, aLdr_OutTxt, aTmp.Length - 1);
            sReader.Close();

            // Input - Off Loader
            //     if ((int)eLanguage.English == Constants.LANGUAGE) sFullPath = sPath + "Input_OffLoader.txt"          ;
            //else if ((int)eLanguage.Korea   == Constants.LANGUAGE) sFullPath = sPath + "Input_OffLoader.txt"          ;
            //else if ((int)eLanguage.China   == Constants.LANGUAGE) sFullPath = sPath + "Input_OffLoader - Chinese.txt";
                 if ((int)ELang.English == CData.Opt.iSelLan) sFullPath = sPath + "Input_OffLoader.txt"          ;
            else if ((int)ELang.Korea   == CData.Opt.iSelLan) sFullPath = sPath + "Input_OffLoader.txt"          ;
            else if ((int)ELang.China   == CData.Opt.iSelLan) sFullPath = sPath + "Input_OffLoader - Chinese.txt";
            //sReader = new StreamReader(sPath + "Input_OffLoader.txt");
            sReader = new StreamReader(sFullPath);
            sVal = sReader.ReadToEnd();
            sVal = sVal.Replace("\t", "    ");
            aTmp = sVal.Replace("\r", "").Split("\n".ToCharArray());
            aOdr_InTxt = new string[aTmp.Length - 1];
            Array.Copy(aTmp, aOdr_InTxt, aTmp.Length - 1);
            sReader.Close();

            // Output - Off Loader
            //     if ((int)eLanguage.English == Constants.LANGUAGE) sFullPath = sPath + "Output_OffLoader.txt"          ;
            //else if ((int)eLanguage.Korea   == Constants.LANGUAGE) sFullPath = sPath + "Output_OffLoader.txt"          ;
            //else if ((int)eLanguage.China   == Constants.LANGUAGE) sFullPath = sPath + "Output_OffLoader - Chinese.txt";
                 if ((int)ELang.English == CData.Opt.iSelLan) sFullPath = sPath + "Output_OffLoader.txt"          ;
            else if ((int)ELang.Korea   == CData.Opt.iSelLan) sFullPath = sPath + "Output_OffLoader.txt"          ;
            else if ((int)ELang.China   == CData.Opt.iSelLan) sFullPath = sPath + "Output_OffLoader - Chinese.txt";
            //sReader = new StreamReader(sPath + "Output_OffLoader.txt");
            sReader = new StreamReader(sFullPath);
            sVal = sReader.ReadToEnd();
            sVal = sVal.Replace("\t", "    ");
            aTmp = sVal.Replace("\r", "").Split("\n".ToCharArray());
            aOdr_OutTxt = new string[aTmp.Length - 1];
            Array.Copy(aTmp, aOdr_OutTxt, aTmp.Length - 1);
            sReader.Close();
            sReader.Dispose();

            try
            {
                aIMain = new string[aMain_InTxt.Length];
                Array.Copy(aMain_InTxt, aIMain, aMain_InTxt.Length);
                aIOn = new string[aLdr_InTxt.Length];
                Array.Copy(aLdr_InTxt, aIOn, aLdr_InTxt.Length);
                aIOff = new string[aOdr_InTxt.Length];
                Array.Copy(aOdr_InTxt, aIOff, aOdr_InTxt.Length);

                aOMain = new string[aMain_OutTxt.Length];
                Array.Copy(aMain_OutTxt, aIMain, aMain_OutTxt.Length);
                aOOn = new string[aLdr_OutTxt.Length];
                Array.Copy(aLdr_OutTxt, aIOn, aLdr_OutTxt.Length);
                aOOff = new string[aOdr_OutTxt.Length];
                Array.Copy(aOdr_OutTxt, aIOff, aOdr_OutTxt.Length);
            }
            catch (Exception ex)
            {

                return;
            }
        }

        public bool Get_X(eX eIn)
        {
            return Convert.ToBoolean(aInput[(int)eIn]);
        }

        public int Get_X(short iAddr, short iBit)
        {
            byte btRet = 0;
            CWmx.It.WLib.io.GetInBit(iAddr, iBit, ref btRet);

            return Convert.ToInt32(btRet);
        }

        /// <summary>
        /// Input read
        /// </summary>
        /// <param name="iAddr">Address</param>
        /// <param name="iSz"></param>
        /// <param name="aRet"></param>
        /// <returns></returns>
        public int Get_X(int iAddr, int iSz, ref int[] aRet)
        {
            int iRet = -1;
            short nAddr = (short)iAddr;
            int iCnt = iSz / 16;
            short nSize = 2;
            byte[] aData = new byte[iSz];

            for (short i = 0; i < iCnt; i++)
            {
                byte[] aTmp = new byte[nSize];
                int iSt = nAddr + (i * 3);
                iRet = CWmx.It.WLib.io.GetInBytes((short)iSt, nSize, ref aTmp);
                if (iRet != (int)ErrorCode.None)
                {
                    return -1;
                }
                Array.Copy(aTmp, 0, aData, (i * nSize), nSize);
            }

            for (int i = 0; i < iSz / 8; i++)
            {
                byte btTemp = aData[i];

                for (int j = 0; j < 8; ++j)
                {
                    aRet[(i * 8) + j] = ((Convert.ToInt32(btTemp) >> (j)) & 0X1);
                }
            }

            return iRet;
        }

        public int Get_X(ref int[] aRet)
        {
            int iRet = -1;
            short nAddr = (short)MAIN_IN_ADDR;
            int iCnt = MAIN_IN_SIZE / 16;
            short nSize = 2;
            byte[] aData = new byte[MAIN_IN_SIZE];

            for (short i = 0; i < iCnt; i++)
            {
                byte[] aTmp = new byte[nSize];
                int iSt = nAddr + (i * 3);
                iRet = CWmx.It.WLib.io.GetInBytes((short)iSt, nSize, ref aTmp);
                if (iRet != (int)ErrorCode.None)
                {
                    return -1;
                }
                Array.Copy(aTmp, 0, aData, (i * nSize), nSize);
            }

            for (int i = 0; i < MAIN_IN_SIZE / 8; i++)
            {
                byte btTemp = aData[i];

                for (int j = 0; j < 8; ++j)
                {
                    aRet[(i * 8) + j] = ((Convert.ToInt32(btTemp) >> (j)) & 0X1);
                }
            }

            nAddr = (short)LDR_IN_ADDR;
            iCnt = LDR_IN_SIZE / 16;
            nSize = 2;
            aData = new byte[LDR_IN_SIZE];

            for (short i = 0; i < iCnt; i++)
            {
                byte[] aTmp = new byte[nSize];
                int iSt = nAddr + (i * 3);
                iRet = CWmx.It.WLib.io.GetInBytes((short)iSt, nSize, ref aTmp);
                if (iRet != (int)ErrorCode.None)
                {
                    return -1;
                }
                Array.Copy(aTmp, 0, aData, (i * nSize), nSize);
            }

            for (int i = 0; i < LDR_IN_SIZE / 8; i++)
            {
                byte btTemp = aData[i];

                for (int j = 0; j < 8; ++j)
                {
                    aRet[MAIN_IN_SIZE + (i * 8) + j] = ((Convert.ToInt32(btTemp) >> (j)) & 0X1);
                }
            }

            nAddr = (short)ODR_IN_ADDR;
            iCnt = LDR_IN_SIZE / 16;
            nSize = 2;
            aData = new byte[ODR_IN_SIZE];

            for (short i = 0; i < iCnt; i++)
            {
                byte[] aTmp = new byte[nSize];
                int iSt = nAddr + (i * 3);
                iRet = CWmx.It.WLib.io.GetInBytes((short)iSt, nSize, ref aTmp);
                if (iRet != (int)ErrorCode.None)
                {
                    return -1;
                }
                Array.Copy(aTmp, 0, aData, (i * nSize), nSize);
            }

            for (int i = 0; i < ODR_IN_SIZE / 8; i++)
            {
                byte btTemp = aData[i];

                for (int j = 0; j < 8; ++j)
                {
                    aRet[MAIN_IN_SIZE + LDR_IN_SIZE + (i * 8) + j] = ((Convert.ToInt32(btTemp) >> (j)) & 0X1);
                }
            }

            return iRet;
        }

        public void Get_X_Num(eX eIdx, ref int iByte, ref int iBit)
        {
            int iIdx = (int)eIdx;
            iByte = (iIdx / 8);
            iBit = iIdx % 8;

            if (iIdx < MAIN_IN_SIZE)
            {
                // Main output
                iIdx += (MAIN_IN_ADDR * 8);
            }
            else if (iIdx < MAIN_IN_SIZE + LDR_IN_SIZE)
            {
                // Loader output
                iIdx -= MAIN_IN_SIZE;
                iIdx += (LDR_IN_ADDR * 8);
            }
            else
            {
                // Offloader output
                iIdx -= MAIN_IN_SIZE;
                iIdx -= LDR_IN_SIZE;
                iIdx += (ODR_IN_ADDR * 8);
            }

            iByte = (iIdx / 8);
            iBit = iIdx % 8;
        }

        /// <summary>
        /// 현재 사용중인 eY를 기반으로 Byte와 Bit를 추출
        /// </summary>
        /// <param name="eIdx"></param>
        /// <param name="iByte"></param>
        /// <param name="iBit"></param>
        public void Get_Y_Num(eY eIdx, ref int iByte, ref int iBit)
        {
            int iIdx = (int)eIdx;
            iByte = (iIdx / 8);
            iBit = iIdx % 8;

            if (iIdx < MAIN_OUT_SIZE)
            {
                // Main output
                iIdx += (MAIN_OUT_ADDR * 8);
            }
            else if (iIdx < MAIN_OUT_SIZE + LDR_OUT_SIZE)
            {
                // Loader output
                iIdx -= MAIN_OUT_SIZE;
                iIdx += (LDR_OUT_ADDR * 8);
            }
            else
            {
                // Offloader output
                iIdx -= MAIN_OUT_SIZE;
                iIdx -= LDR_OUT_SIZE;
                iIdx += (ODR_OUT_ADDR * 8);
            }

            iByte = (iIdx / 8);
            iBit = iIdx % 8;
        }

        /// <summary>
        /// 해당 아웃풋 상태 반환
        /// </summary>
        /// <param name="eIdx"></param>
        /// <returns></returns>
        public bool Get_Y(eY eIdx)
        {
            return Get_Y((int)eIdx);
        }

        public bool Get_Y(int iIdx)
        {
            if (!CData.WMX)
            { return false; }

            int iRet = 0;
            byte btRet = 0;
            int nAddr = (iIdx / 8);
            int nOffset = iIdx % 8;

            if (iIdx < MAIN_OUT_SIZE)
            {
                // Main output
                iIdx += (MAIN_OUT_ADDR * 8);
            }
            else if (iIdx < MAIN_OUT_SIZE + LDR_OUT_SIZE)
            {
                // Loader output
                iIdx -= MAIN_OUT_SIZE;
                iIdx += (LDR_OUT_ADDR * 8);
            }
            else
            {
                // Offloader output
                iIdx -= MAIN_OUT_SIZE;
                iIdx -= LDR_OUT_SIZE;
                iIdx += (ODR_OUT_ADDR * 8);
            }

            nAddr = (iIdx / 8);
            nOffset = iIdx % 8;

            iRet = CWmx.It.WLib.io.GetOutBit((short)nAddr, (short)nOffset, ref btRet);

            return Convert.ToBoolean(btRet);
        }

        public int Get_Y(int iAddr, int iSz, ref int[] aRet)
        {
            int iRet = -1;
            short nAddr = (short)iAddr;
            short nSz = (short)(iSz / 8);
            byte[] aData = new byte[nSz];

            iRet = CWmx.It.WLib.io.GetOutBytes(nAddr, nSz, ref aData);

            for (int i = 0; i < nSz; i++)
            {
                byte btTemp = aData[i];

                for (int j = 0; j < 8; ++j)
                {
                    aRet[(i * 8) + j] = ((Convert.ToInt32(btTemp) >> (j)) & 0X1);
                }
            }

            return iRet;
        }

        /// <summary>
        /// 190709-maeng
        /// Tool Setter 상태 반환
        /// </summary>
        /// <param name="eWy"></param>
        /// <returns></returns>
        public int Get_TS(EWay eWy)
        {
            int iRet;

            if (eWy == EWay.L)
            { iRet = Get_X(10, 7); }
            else
            {
                /*
                if (CDataOption.SplType == eSpindleType.Rs232)
                {
                    if (CDataOption.CurEqu == eEquType.Nomal)
                    { iRet = Get_X(85, 7); }
                    else
                    { iRet = Get_X(89, 7); }
                }
                else
                {
                    if (CDataOption.CurEqu == eEquType.Nomal)
                    { iRet = Get_X(97, 7); }
                    else
                    { iRet = Get_X(101, 7); }
                }
                */
                // 2023.03.15 Max
                if (CDataOption.CurEqu == eEquType.Nomal)
                { iRet = Get_X(97, 7); }
                else
                { iRet = Get_X(101, 7); }
            }

            return iRet;
        }

        public int Set_Y(int iIdx)
        {
            int iRet = 0;
            byte btRet = 0;
            int nAddr = (iIdx / 8);
            int nOffset = iIdx % 8;

            if (iIdx < MAIN_OUT_SIZE)
            {
                // Main output
                iIdx += (MAIN_OUT_ADDR * 8);


            }
            else if (iIdx < MAIN_OUT_SIZE + LDR_IN_SIZE)
            {
                // Loader output
                iIdx -= MAIN_OUT_SIZE;
                iIdx += (LDR_OUT_ADDR * 8);
            }
            else
            {
                // Offloader output
                iIdx -= MAIN_OUT_SIZE;
                iIdx -= LDR_OUT_SIZE;
                iIdx += (ODR_OUT_ADDR * 8);
            }

            nAddr = (iIdx / 8);
            nOffset = iIdx % 8;

            CWmx.It.WLib.io.GetOutBit((short)nAddr, (short)nOffset, ref btRet);
            if (btRet == 0)
            { btRet = 1; }
            else
            { btRet = 0; }

            iRet = CWmx.It.WLib.io.SetOutBit((short)nAddr, (short)nOffset, btRet);
            if (iRet != (int)ErrorCode.None)
            {
                return -1;
            }

            return iRet;
        }


        public int Set_Y(int iIdx, bool bVal)
        {
            int iRet = 0;
            byte btRet = 0;
            int nAddr = (iIdx / 8);
            int nOffset = iIdx % 8;

            if (iIdx < MAIN_OUT_SIZE)
            {
                // Main output
                iIdx += (MAIN_OUT_ADDR * 8);
            }
            else if (iIdx < MAIN_OUT_SIZE + LDR_IN_SIZE)
            {
                // Loader output
                iIdx -= MAIN_OUT_SIZE;
                iIdx += (LDR_OUT_ADDR * 8);
            }
            else
            {
                // Offloader output
                iIdx -= MAIN_OUT_SIZE;
                iIdx -= LDR_OUT_SIZE;
                iIdx += (ODR_OUT_ADDR * 8);
            }

            nAddr = (iIdx / 8);
            nOffset = iIdx % 8;

            btRet = Convert.ToByte(bVal);

            iRet = CWmx.It.WLib.io.SetOutBit((short)nAddr, (short)nOffset, btRet);
            if (iRet != (int)ErrorCode.None)
            {
                return -1;
            }

            return iRet;
        }

        /// <summary>
        /// 리턴 값 반환
        /// </summary>
        /// <param name="eIdx"></param>
        /// <param name="bVal"></param>
        /// <returns></returns>
        public bool Set_Y(eY eIdx, bool bVal)
        {
            Set_Y((int)eIdx, bVal);

            if (Get_Y((int)eIdx) == bVal)
            { return true; }
            else
            { return false; }
        }
        /// <summary>
        /// Probe Up 명령 수정
        //  기존 : CIO.It.Set_Y(m_eOt1, false);
        //         m_Delay.Set_Delay(GV.PRB_DELAY); //191002 ksg :
        //  개선 : CIO.It.Probe_Up(m_eWy, m_eOt1, 1 * 1000);// 20200629 LCY
        /// </summary>
        /// <param name="eIdx"></param>
        /// <param name="bVal"></param>
        /// <returns></returns>
        private int nWait_ms;
        private CTim[] m_Check = new CTim[10];
        private CTim[] m_Check_0 = new CTim[10];
        public bool Probe_Up(EWay eWay,eY eIdx, int nWait)
        {
            Set_Y((int)eIdx, false);
            nWait_ms = nWait;
            m_Check[(int)eWay].Set_Delay(nWait_ms);  // Time out 시간 설정
            m_Check_0[(int)eWay].Set_Delay(1 * 1000);// 안정화 시간 설정
            return true;
        }

        /// <summary>
        /// Probe Up 동작 명령 후 완료 Check 함수
        /// 기존 : if(!CIO.It.Get_X(m_eIn1) && m_Delay.Chk_Delay())
        /// 개선 : Probe_Up_Check (EWay eWay,m_eIn1) 
        /// </summary>
        /// <param name="eWay"></param>
        /// <param name="eIdx"></param>
        /// <returns></returns>
        public int Probe_Up_Check(EWay eWay, eX eIdx)
        {
            int nret_val = 0;
            if (m_Check[(int)eWay].Chk_Delay())         nret_val = -1;// 10 s 경과시 Error
            else if (m_Check_0[(int)eWay].Chk_Delay())  nret_val = 1; // Up 감지 후 1초 경과시 동작 완료
            else
            {
                if (!Get_X(eIdx)) m_Check_0[(int)eWay].Set_Delay(1 * 1000); // Up 신호가 입력이 되지 않았으면 동작 완료 경과 시간 Reset
                nret_val = 0;
            }
            return nret_val;
        }

        //public bool Set_YR(int iIdx, bool bVal)
        //{
        //    byte btRet = 0;
        //    int nAddr = (iIdx / 8);
        //    int nOffset = iIdx % 8;

        //    if (iIdx < MAIN_OUT_SIZE)
        //    {
        //        // Main output
        //        iIdx += (MAIN_OUT_ADDR * 8);
        //    }
        //    else if (iIdx < MAIN_OUT_SIZE + LDR_IN_SIZE)
        //    {
        //        // Loader output
        //        iIdx -= MAIN_OUT_SIZE;
        //        iIdx += (LDR_OUT_ADDR * 8);
        //    }
        //    else
        //    {
        //        // Offloader output
        //        iIdx -= MAIN_OUT_SIZE;
        //        iIdx -= LDR_OUT_SIZE;
        //        iIdx += (ODR_OUT_ADDR * 8);
        //    }

        //    nAddr = (iIdx / 8);
        //    nOffset = iIdx % 8;

        //    btRet = Convert.ToByte(bVal);

        //    CWmx.It.WLib.io.SetOutBit((short)nAddr, (short)nOffset, btRet);

        //    CWmx.It.WLib.io.GetOutBit((short)nAddr, (short)nOffset, ref btRet);

        //    return Convert.ToBoolean(btRet);
        //}

        /// <summary>
        /// Chuck Table Vacuum 아날로그 값 획득

        public double Get_Chuck_Table_Vaccum(EWay eWy)
        {
            int iRet = 0;
            double dVal = 0.0;

            int iAX = (eWy == EWay.L) ? (int)EAx.LeftGrindZone_Y : (int)EAx.RightGrindZone_Y;
            iRet = CWmx.It.Get_Vac_Value(iAX, ref dVal);

            return dVal; //Vacuum 압력
        }
    }
}

