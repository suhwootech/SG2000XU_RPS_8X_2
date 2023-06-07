using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using wmxCLRLibrary;
using wmxCLRLibrary.common;
using wmxCLRLibrary.common.jerkMotionEvent;

namespace SG2000X
{
    public class CMot : CStn<CMot>
    {
        /// <summary>
        /// Axis start index
        /// </summary>
        public  int AxS = 4;
        /// <summary>
        /// Axis last index
        /// </summary>
        public readonly int AxL = 24;

        /// <summary>
        /// 툴세터 눌렸을 때 왼쪽 Z축 멈추게 하려고 만든 이벤트 변수
        /// </summary>
        private uint m_iLZ_IoID = 0;

        /// <summary>
        /// 왼쪽 Z축 퀵스탑을 하기 위해 만든 이벤트 변수
        /// </summary>
        private uint m_iLZ_AxID = 0;

        /// <summary>
        /// 툴세터 눌렸을 때 오른쪽 Z축 멈추게 하려고 만든 이벤트 변수
        /// </summary>
        private uint m_iRZ_IoID = 0;

        /// <summary>
        /// 오른쪽 Z축 퀵스탑을 하기 위해 만든 이벤트 변수
        /// </summary>
        private uint m_iRZ_AxID = 0;

        //Temporary variables for internal test
        AxisSelection axes = new AxisSelection();
        LogOptions option = new LogOptions();
        IOAddress[] ioAddr = new IOAddress[1];
        public int[] nSts_Axis = new int[2];
        private static double[] bufEncode = new double[50]; //koo WriteLog
        public double GetBufEnc(int axis)
        {
            return bufEncode[axis];
        }
        private CMot()
        {
            if(CDataOption.CurEqu == eEquType.Nomal) AxS = 4;
            else                                     AxS = 3;
            // 축 갯수 가져오기 (eAx의 갯수)
            int iCnt = Enum.GetNames(typeof(EAx)).Length;
            // 배열 초기화
            CData.Axes = new tAx[AxL + 1];
            //This variable is also for internal test.
            //Should be removed.
            ioAddr[0] = new IOAddress();
        }

        #region Private method
        public int WmxEvt()
        {
            int iRet = 0;
            Event tIoEvt = new Event();
            tIoEvt.Enabled = 0;
            tIoEvt.InputFunction = EventInputFunction.NotIOBit;
            tIoEvt.InputByteAddress = 10;
            tIoEvt.InputBitAddress = 7;
            tIoEvt.InputInvert = 0;
            tIoEvt.InputUseOutput = 0;
            tIoEvt.Priority = 0;
            iRet = CWmx.It.WLib.eventControl.SetEvent(ref m_iLZ_IoID, tIoEvt);
            if (iRet != 0)
            {
                _SetLog("Error : Left IO event fail.  " + Get_Err());
                return iRet;
            }

            PosBlockEvent tAxEvt = new PosBlockEvent();
            tAxEvt.Enabled = 0;
            tAxEvt.InputEventID = m_iLZ_IoID;
            tAxEvt.OutputDisableAfterActivate = 0;
            tAxEvt.OutputFunction = OutputFunc.ExecQuickStopSingleAxis;
            tAxEvt.OutputBlock.Axis = (short)EAx.LeftGrindZone_Z;
            iRet = CWmx.It.WLib.eventControl.SetEvent(ref m_iLZ_AxID, tAxEvt);
            if (iRet != 0)
            {
                _SetLog("Error : Left Z axis event fail.  " + Get_Err());
                return iRet;
            }
            Event tIoEvt2 = new Event();
            tIoEvt2.Enabled = 0;
            tIoEvt2.InputFunction = EventInputFunction.NotIOBit;
            //20191022 ghk_spindle_type
            //if (CSpl.It.bUse232)
            //if(CDataOption.SplType == eSpindleType.Rs232)
            //{
            //    if (CDataOption.CurEqu == eEquType.Nomal)
            //    { tIoEvt2.InputByteAddress = 85; }
            //    else
            //    { tIoEvt2.InputByteAddress = 89; }
            //}
            //else
            //{
            //    if(CDataOption.CurEqu == eEquType.Nomal) tIoEvt2.InputByteAddress = 97 ;//190612 ksg :
            //    else                                     tIoEvt2.InputByteAddress = 101;//190612 ksg :
            //}
            // 2023.03.15 Max
            if (CDataOption.CurEqu == eEquType.Nomal) tIoEvt2.InputByteAddress = 97;
            else                                      tIoEvt2.InputByteAddress = 101;


            tIoEvt2.InputBitAddress = 7;
            tIoEvt2.InputInvert = 0;
            tIoEvt2.InputUseOutput = 0;
            tIoEvt2.Priority = 0;
            iRet = CWmx.It.WLib.eventControl.SetEvent(ref m_iRZ_IoID, tIoEvt2);
            if (iRet != 0)
            {
                _SetLog("Error : Right IO event fail.  " + Get_Err());
                return iRet;
            }

            PosBlockEvent tAxEvt2 = new PosBlockEvent();
            tAxEvt2.Enabled = 0;
            tAxEvt2.InputEventID = m_iRZ_IoID;
            tAxEvt2.OutputDisableAfterActivate = 0;
            tAxEvt2.OutputFunction = OutputFunc.ExecQuickStopSingleAxis;
            tAxEvt2.OutputBlock.Axis = (short)EAx.RightGrindZone_Z;
            iRet = CWmx.It.WLib.eventControl.SetEvent(ref m_iRZ_AxID, tAxEvt2);
            if (iRet != 0)
            {
                _SetLog("Error : Right Z axis event fail.  " + Get_Err());
                return iRet;
            }

            return iRet;
        }

        private void _SetLog(string sMsg)
        {
            StackTrace mTrace = new StackTrace();
            StackFrame mFrame = mTrace.GetFrame(1);

            string sMth = mFrame.GetMethod().Name;
            string sLog = string.Format("[{0}()]\t{1}", sMth, sMsg);

            CLog.Save_Log(eLog.None, eLog.MOT, sLog);
        }

        private void _SetLog(string sMsg, int iAx)
        {
            EAx eAx = (EAx)iAx;
            StackTrace mTrace = new StackTrace();
            StackFrame mFrame = mTrace.GetFrame(1);

            string sMth = mFrame.GetMethod().Name;
            string sLog = string.Format("[{0}()]\tAx:{1}\t{2}", sMth, eAx.ToString(), sMsg);

            CLog.Save_Log(eLog.None, eLog.MOT, sLog);
        }
        #endregion

        #region Check method
        /// <summary>
        /// Check SERVO ON Signal - return bool
        /// </summary>
        /// <param name="iAx">Axis index</param>
        /// <returns>false : Off,    true : On</returns>
        public bool Chk_Srv(int iAx)
        {
            bool bRet = Convert.ToBoolean(CWmx.It.Stat.ServoOn[iAx]);

            return bRet;
        }

        //20190930 ghk_autowarmup
        /// <summary>
        /// Check All Servo On
        /// servo on = true, servo off = false;
        /// </summary>
        /// <returns></returns>
        public bool Chk_AllSrv()
        {
            bool bRet = true;

            foreach (int iAx in Enum.GetValues(typeof(EAx)))
            {
                if (CDataOption.CurEqu == eEquType.Nomal)
                {
                    if (iAx == (int)EAx.OnLoaderPicker_Y)
                    { continue; }
                }
                
                //20191022 ghk_spindle_type
                //if (CSpl.It.bUse232)
                //if(CDataOption.SplType == eSpindleType.Rs232)
                //{
                //    if (iAx == (int)EAx.LeftSpindle || iAx == (int)EAx.RightSpindle)
                //    { continue; }
                //}

                // 2023.03.15 Max
                if (iAx == (int)EAx.LeftSpindle || iAx == (int)EAx.RightSpindle)  { continue; }

                bRet = Convert.ToBoolean(CWmx.It.Stat.ServoOn[iAx]);

                if (!bRet)
                { break; }
            }

            return bRet;
        }
        //

        /// <summary>
        /// Check SERVO ON Signal - return int
        /// </summary>
        /// <param name="iAx">Axis index</param>
        /// <returns>0 : Off,    1 : On</returns>
        public int Chk_SrvI(int iAx)
        {
            int iRet = CWmx.It.Stat.ServoOn[iAx];

            return iRet;
        }

        /// <summary>
        /// Check Alarm Signal - return bool
        /// </summary>
        /// <param name="iAx">Axis index</param>
        /// <returns>false : No alarm,    true : Alarm</returns>
        public bool Chk_Alr(int iAx)
        {
            bool bRet = Convert.ToBoolean(CWmx.It.Stat.AmpAlarm[iAx]);

            return bRet;
        }
        /// <summary>
        /// Check Alarm Signal - return int
        /// </summary>
        /// <param name="iAx">Axis index</param>
        /// <returns>0 : No alarm,    1 : Alarm</returns>
        public int Chk_AlrI(int iAx)
        {
            int iRet = CWmx.It.Stat.AmpAlarm[iAx];

            return iRet;
        }

        /// <summary>
        /// Check Inposition Signal - return bool
        /// </summary>
        /// <param name="iAx">Axis index</param>
        /// <returns>false : Off,    true : On</returns>
        public bool Chk_InP(int iAx)
        {
            bool bRet = Convert.ToBoolean(CWmx.It.Stat.InPos[iAx]);

            return bRet;
        }
        /// <summary>
        /// Check Inposition Signal - return int
        /// </summary>
        /// <param name="iAx">Axis index</param>
        /// <returns>0 : Off,    1 : On</returns>
        public int Chk_InPI(int iAx)
        {
            int iRet = CWmx.It.Stat.InPos[iAx];

            return iRet;
        }

        /// <summary>
        /// Check Home Done Signal - return bool
        /// </summary>
        /// <param name="iAx">Axis index</param>
        /// <returns>false : Off,    true : On</returns>
        public bool Chk_HD(int iAx)
        {
            bool bRet = Convert.ToBoolean(CWmx.It.Stat.HomeDone[iAx]);

            return bRet;
        }
        /// <summary>
        /// Check Home Done Signal - return int
        /// </summary>
        /// <param name="iAx">Axis index</param>
        /// <returns>0 : Off,    1 : On</returns>
        public int Chk_HDI(int iAx)
        {
            int iRet = CWmx.It.Stat.HomeDone[iAx];

            return iRet;
        }

        /// <summary>
        /// Check Home Sensor Status - return bool
        /// </summary>
        /// <param name="iAx">Axis index</param>
        /// <returns>false : Off,    true : On</returns>
        public bool Chk_Home(int iAx)
        {
            bool bRet = Convert.ToBoolean(CWmx.It.Stat.HomeSwitch[iAx]);

            return bRet;
        }
        /// <summary>
        /// Check Home Sensor Status - return int
        /// </summary>
        /// <param name="iAx">Axis index</param>
        /// <returns>0 : Off,    1 : On</returns
        public int Chk_HomeI(int iAx)
        {
            int iRet = CWmx.It.Stat.HomeSwitch[iAx];

            return iRet;
        }

        /// <summary>
        /// Check Positive Limit Sensor Status - return bool
        /// </summary>
        /// <param name="iAx">Axis index</param>
        /// <returns>false : Off,    true : On</returns>
        public bool Chk_POT(int iAx)
        {
            bool bRet = Convert.ToBoolean(CWmx.It.Stat.PositiveLS[iAx]);

            return bRet;
        }
        /// <summary>
        /// Check Positive Limit Sensor Status - return int
        /// </summary>
        /// <param name="iAx">Axis index</param>
        /// <returns>0 : Off,    1 : On</returns>
        public int Chk_POTI(int iAx)
        {
            int iRet = CWmx.It.Stat.PositiveLS[iAx];

            return iRet;
        }

        /// <summary>
        /// Check Negative Limit Sensor Status - return bool
        /// </summary>
        /// <param name="iAx">Axis index</param>
        /// <returns>false : Off,    true : On</returns>
        public bool Chk_NOT(int iAx)
        {
            bool bRet = Convert.ToBoolean(CWmx.It.Stat.NegativeLS[iAx]);

            return bRet;
        }
        /// <summary>
        /// Check Negative Limit Sensor Status - return int
        /// </summary>
        /// <param name="iAx">Axis index</param>
        /// <returns>0 : Off,    1 : On</returns>
        public int Chk_NOTI(int iAx)
        {
            int iRet = CWmx.It.Stat.NegativeLS[iAx];

            return iRet;
        }

        /// <summary>
        /// Check OP State Status
        /// </summary>
        /// <param name="iAx"></param>
        /// <returns>eAxOp</returns>
        public EAxOp Chk_OP(int iAx)
        {
            CWmx.It.Get_Stat();
            EAxOp tRet = (EAxOp)CWmx.It.Stat.OpState[iAx];

            return tRet;
        }

        /// <summary>
        /// Axis ready check 1.Used  2.Alarm  3.SERVO ON  4.Busy  5.Home done
        /// </summary>
        /// <param name="iAx">Axis index</param>
        /// <param name="bHD">Home done check 여부</param>
        /// <returns></returns>
        public int Chk_Rdy(int iAx, bool bHD = true)
        {
            int iRet = 0;

            if (!CData.Axes[iAx].bUse)
            {// 1. Used Check
                GV.bErr = true;
                iRet = 1;    // Error code
                _SetLog("Error : Not used.", iAx);

                return iRet;
            }

            if (Chk_Alr(iAx))
            {// 2. Alarm Check
                GV.bErr = true;
                iRet = 1;
                _SetLog("Error : Axis alarm.", iAx);

                return iRet;
            }

            if (!Chk_Srv(iAx))
            {// 3. Servo On Check
                GV.bErr = true;
                iRet = 1;
                _SetLog("Error : Servo off.", iAx);

                return iRet;
            }

            if (Chk_OP(iAx) != EAxOp.Idle)
            {// 4. Busy Check
                GV.bErr = true;
                iRet = 1;
                _SetLog("Error : Axis busy.", iAx);

                return iRet;
            }

            if (bHD == true)
            {

                if (!Chk_HD(iAx))
                {// 5. Home Done Check
                    GV.bErr = true;
                    iRet = 1;
                    _SetLog("Error : Not home done.", iAx);

                    return iRet;
                }
            }
            _SetLog("Axis ready.", iAx);

            return iRet;
        }
        #endregion

        #region Set
        /// <summary>
        /// Set SERVO ON
        /// </summary>
        /// <param name="iAx">Axis index</param>
        /// <param name="bVal">true:On    false:Off</param>
        /// <returns>0:Ok</returns>
        public int Set_SOn(int iAx, bool bVal)
        {
            int iRet = 0;

            if (Chk_Srv(iAx) == bVal)
            {
                // 현재 상태와 이전 상태가 동일함 - 동작 안함                
            }
            else
            {
                if (bVal == false)
                {// Home Done Off
                    iRet = CWmx.It.WLib.home.SetHomeDone((short)iAx, 0);
                }

                iRet = CWmx.It.WLib.axisControl.SetServoOn(iAx, Convert.ToInt32(bVal));
                if (iRet != 0)
                {
                    GV.bErr = true;
                    iRet = 1;
                    _SetLog("Error : Servo on fail -> " + Get_Err(), iAx);
                }
            }

            _SetLog("Servo on : " + bVal, iAx);

            return iRet;
        }

        public int Set_Clr(int iAx)
        {
            int iRet = 0;

            if (Chk_Alr(iAx))
            {
                iRet = CWmx.It.WLib.axisControl.ClearAmpAlarm((short)iAx);
                if (iRet != 0)
                {
                    GV.bErr = true;
                    //20190227 ghk_err
                    //iRet = (int)eErr.MOTION_SETCLR_FAIL;
                    iRet = 1;
                    _SetLog("Error : Alarm clear fail -> " + Get_Err(), iAx);
                }
            }
            _SetLog("Alarm clear.", iAx);

            return iRet;
        }

        /// <summary>
        /// 이벤트 동작 활성화 비활성화 함수    false:비활성화  true:활성화
        /// </summary>
        /// <param name="eWy"></param>
        /// <param name="bVal"></param>
        /// <returns></returns>
        public int Set_Evt(EWay eWy, bool bVal)
        {
            int iRet = 0;
            int iAx = 0;
            uint iAxID = 0;
            uint iIoID = 0;
            byte btVal = 0;

            if (bVal == true)
            { btVal = 1; }

            if (eWy == EWay.L)
            {
                iAx = (int)EAx.LeftGrindZone_Z;
                iAxID = m_iLZ_AxID;
                iIoID = m_iLZ_IoID;
            }
            else
            {
                iAx = (int)EAx.RightGrindZone_Z;
                iAxID = m_iRZ_AxID;
                iIoID = m_iRZ_IoID;
            }

            iRet = CWmx.It.WLib.eventControl.EnableEvent(iAxID, btVal);
            if (iRet != 0)
            {
                GV.bErr = true;
                iRet = 1;
                _SetLog("Error : IO event fail -> " + Get_Err(), iAx);

                return iRet;
            }

            GU.Delay(100);
            iRet = CWmx.It.WLib.eventControl.EnableEvent(iIoID, btVal);

            if (iRet != 0)
            {
                GV.bErr = true;
                iRet = 1;
                _SetLog("Error : Motion event fail -> " + Get_Err(), iAx);

                return iRet;
            }

            GU.Delay(100);
            _SetLog("Set event.", iAx);

            return iRet;
        }

        public int Set_Log(int iCh, int iAx, int iByteAddr, int iBitAddr)
        {
            int iRet = 0;
            string path = @"D:\LogStatus\LogData_" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString()
                                          + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString()
                                          + ".txt";
            //AxisSelection axes = new AxisSelection();
            //LogOptions option = new LogOptions();
            //IOAddress[] ioAddr = new IOAddress[1];
            //ioAddr[0] = new IOAddress();
            ioAddr[0].ByteAddress = iByteAddr;
            ioAddr[0].BitOffset = (short)iBitAddr;
            ioAddr[0].Size = 1;
            axes.AxisCount = 1;
            axes.Axis[0] = (short)iAx;
            option.CommandPos = 1;
            option.FeedbackPos = 1;
            option.OpState = 1;

            iRet = CWmx.It.WLib.log.SetLog((uint)iCh, path, 120, 1, axes, option, 0);
            if (iRet != 0)
            {
                GV.bErr = true;
                iRet = 1;
                CLog.Save_Log(eLog.None, eLog.MOT, "Set_Log E " + iRet.ToString() + string.Format(", Log Setting Fail -> {0}", CWmx.It.WLib.GetLastError()));
                return iRet;
            }
            iRet = CWmx.It.WLib.log.SetIOLog((uint)iCh, ioAddr, 1, ioAddr, 0);
            if (iRet != 0)
            {
                GV.bErr = true;
                iRet = 1;
                CLog.Save_Log(eLog.None, eLog.MOT, "Set_Log E " + iRet.ToString() + string.Format(", Log Setting Fail -> {0}", CWmx.It.WLib.GetLastError()));
                return iRet;
            }
            return iRet;
        }

        public int Start_Log(int iCh)
        {
            int iRet = 0;

            iRet = CWmx.It.WLib.log.StartLog((uint)iCh);
            if (iRet != 0)
            {
                GV.bErr = true;
                iRet = 1;
                CLog.Save_Log(eLog.None, eLog.MOT, "Start_Log E " + iRet.ToString() + string.Format(", Log Start Fail -> {0}", CWmx.It.WLib.GetLastError()));
                return iRet;
            }
            return iRet;
        }

        public int Stop_Log(int iCh)
        {
            int iRet = 0;

            iRet = CWmx.It.WLib.log.StopLog((uint)iCh);
            if (iRet != 0)
            {
                GV.bErr = true;
                iRet = 1;
                CLog.Save_Log(eLog.None, eLog.MOT, "Stop_Log E " + iRet.ToString() + string.Format(", Log Stop Fail -> {0}", CWmx.It.WLib.GetLastError()));
                return iRet;
            }
            return iRet;
        }

        /// <summary>
        /// 강제 홈둔
        /// </summary>
        /// <param name="iAx"></param>
        /// <param name="Done"></param>
        /// <returns></returns>
        public int Set_HomeDone(int iAx, bool Done)
        {
            CLog.Save_Log(eLog.None, eLog.MOT, "Set_HomeDone S " + iAx.ToString() + ", " + Done.ToString());

            //20190227 ghk_err
            //int iRet = (int)eErr.NONE;
            int iRet = 0;

            // 1. 준비 동작 체크
            iRet = Chk_Rdy(iAx, false);

            if (iRet != 0)
            {
                GV.bErr = true;
                //20190227 ghk_err
                //iRet = (int)eErr.MOTION_SETHOMEDONE_CHKRDY;
                iRet = 1;
                CLog.Save_Log(eLog.None, eLog.MOT, "Set_HomeDone E " + iRet.ToString() + ", Axis Not Ready");

                return iRet;
            }

            // Home done off
            if (Done)
            { iRet = CWmx.It.WLib.home.SetHomeDone((short)iAx, 1); }
            else
            { iRet = CWmx.It.WLib.home.SetHomeDone((short)iAx, 0); }

            if (iRet != 0)
            {
                GV.bErr = true;
                //20190227 ghk_err
                //iRet = (int)eErr.MOTION_SETHOMEDONE_SETHOMEDONE;
                iRet = 1;
                CLog.Save_Log(eLog.None, eLog.MOT, "Set_HomeDone E " + iRet.ToString());
            }

            CLog.Save_Log(eLog.None, eLog.MOT, "Set_HomeDone F " + iRet.ToString());

            return iRet;
        }

        public int Set_ZeroPos(int iAx)
        {
            int iRet = 0;

            iRet = CWmx.It.WLib.home.SetFeedbackPos((short)iAx, 0);
            if (iRet != 0)
            {
                GV.bErr = true;
                iRet = 1;
                CLog.Save_Log(eLog.None, eLog.MOT, "Set_ZeroPos E " + iRet.ToString());
                _SetLog("Error : Set zero feedback position fail -> " + Get_Err(), iAx);

                return iRet;
            }

            iRet = CWmx.It.WLib.home.SetCommandPos((short)iAx, 0);
            if (iRet != 0)
            {
                GV.bErr = true;
                iRet = 1;
                _SetLog("Error : Set zero command position fail -> " + Get_Err(), iAx);

                return iRet;
            }
            _SetLog("Set zero position.", iAx);

            return iRet;

        }

        #endregion

        #region Get
        /// <summary>
        /// Get Command Position
        /// </summary>
        /// <param name="iAx">Axis index</param>
        /// <returns>Command position [mm]</returns>
        public double Get_CP(int iAx)
        {
            double dRet = 0.0;

            CWmx.It.WLib.axisControl.GetPosCommand((short)iAx, ref dRet);

            if ((iAx == (int)EAx.LeftGrindZone_Z) || (iAx == (int)EAx.RightGrindZone_Z))
            { dRet = GU.Truncate(Math.Round(dRet / CData.Axes[iAx].dPP1, 6), 4); }
            else if (iAx == (int)EAx.DryZone_Air)
            { dRet = GU.Truncate(Math.Round(dRet / CData.Axes[iAx].dPP1, 6), 1); }
            else
            { dRet = GU.Truncate(Math.Round(dRet / CData.Axes[iAx].dPP1, 6), 3); }

            return dRet;
        }

        /// <summary>
        /// Get Feedback Position
        /// </summary>
        /// <param name="iAx">Axis index</param>
        /// <returns>Encorder position [mm]</returns>
        public double Get_FP(int iAx)
        {
            double dRet = 0.0;

            CWmx.It.WLib.axisControl.GetPosFeedback((short)iAx, ref dRet);

            // 201003 jym : 반올림 자리수 5 -> 6자리로 변경
            if ((iAx == (int)EAx.LeftGrindZone_Z) || (iAx == (int)EAx.RightGrindZone_Z))
            { dRet = GU.Truncate(Math.Round(dRet / CData.Axes[iAx].dPP1, 6), 4); }
            else if (iAx == (int)EAx.DryZone_Air)
            { dRet = GU.Truncate(Math.Round(dRet / CData.Axes[iAx].dPP1, 4), 1); }
            else
            {   // 2021.01.14 lhs Start  
                // dRet = GU.Truncate(Math.Round(dRet / CData.Axes[iAx].dPP1, 6), 3);
                if (CData.CurCompany == ECompany.JCET)      // JECT VOC : 자리수 떨림 최소화
                { dRet = GU.Truncate(Math.Round(dRet / CData.Axes[iAx].dPP1, 4), 3); }
                else
                { dRet = GU.Truncate(Math.Round(dRet / CData.Axes[iAx].dPP1, 6), 3); }
                // 2021.01.14 lhs Stop
            }
            bufEncode[iAx] = dRet;//koo WriteLog
            return dRet;
        }

        /// <summary>
        /// Get velosity
        /// </summary>
        /// <param name="iAx">Axis index</param>
        /// <returns>Velosity [mm/s]</returns>
        public double Get_Vel(int iAx)
        {
            double dRet = 0.0;
            CWmx.It.WLib.axisControl.GetVelFeedback((short)iAx, ref dRet);

            if ((iAx == (int)EAx.DryZone_Air) && (CDataOption.Dryer == eDryer.Rotate))
            {
                dRet /= 6.0; 
            }

            dRet = GU.Truncate(Math.Round(dRet / CData.Axes[iAx].dPP1, 6), 1);

            return dRet;
        }

        /// <summary>
        /// 이동 지령 후, 이동이 완료되었는지 확인하는 함수 - return bool
        /// </summary>
        /// <param name="iAx"></param>
        /// <returns></returns>
        public bool Get_Mv(int iAx)
        {//이동위치 이동 완료 확인 목적
            bool bRet = (Chk_InP(iAx)) && (Chk_OP(iAx) == EAxOp.Idle);

            return bRet;
        }


        // 지정 축이 정지해있는가 ?
        public bool Get_Stop(int iAx)
        {
            return (Chk_InP(iAx) && (Chk_OP(iAx) == EAxOp.Idle));
        }

        /// <summary>
        /// 이동 지령 후, 이동이 완료되었는지 확인하는 함수 도착 포지션도 체크 - return bool
        /// </summary>
        /// <param name="iAx"></param>
        /// <returns></returns>
        public bool Get_Mv(int iAx, double dPos)
        {//이동위치 이동 완료 확인 목적
            bool bRet = false;
            double dGap = 0.001;

            if (iAx == (int)EAx.LeftGrindZone_X || iAx == (int)EAx.RightGrindZone_X)
            { dGap = 0.003; }
            else if (iAx == (int)EAx.LeftGrindZone_Z || iAx == (int)EAx.RightGrindZone_Z)
            { dGap = 0.0001; }
            //else if (iAx == (int)eAx.Inrail_X)
            else if (iAx == (int)EAx.Inrail_X || iAx == (int)EAx.DryZone_X) //191109 ksg :
            { dGap = 0.01  ; }
            else if (iAx == (int)EAx.LeftGrindZone_Y || iAx == (int)EAx.RightGrindZone_Y)
            { dGap = 0.05  ; }

#if true //201010 jym : Encoder 값 미세 떨림 발생 경우를 고려, 계산 오류 방지를 위해 소수점 6번째자리에서 반올림 추가
            double dRet = Get_FP(iAx);
            double dUp = Math.Round(dPos + dGap, 6);
            double dDown = Math.Round(dPos - dGap, 6);

            bRet = (((dRet <= dUp) && (dRet >= dDown)) &&
                   (Chk_InP(iAx)) && (Chk_OP(iAx) == EAxOp.Idle));
#else
            bRet = (((Get_FP(iAx) <= dPos + dGap) && (Get_FP(iAx) >= dPos - dGap)) &&
                   (Chk_InP(iAx)) && (Chk_OP(iAx) == EAxOp.Idle));
#endif

            return bRet;
        }

        /// <summary>
        /// 이동 지령 후, 이동이 완료되었는지 확인하는 함수 도착 포지션도 체크 - return bool
        /// </summary>
        /// <param name="iAx">축 번호</param>
        /// <param name="dPos">타겟 포지션</param>
        /// <param name="dGap">인포지션 확인 값</param>
        /// <returns></returns>
        public bool Get_Mv(int iAx, double dPos, double dGap)
        {//이동위치 이동 완료 확인 목적
            bool bRet = false;

            bRet = (((Get_FP(iAx) <= dPos + dGap) && (Get_FP(iAx) >= dPos - dGap)) &&
                   (Chk_InP(iAx)) && (Chk_OP(iAx) == EAxOp.Idle));

            return bRet;
        }

        /// <summary>
        /// Home Sequence Complete Check
        /// 홈 지령 후, 호밍이 완료되었는지 확인하는 함수
        /// </summary>
        /// <param name="iAx"></param>
        /// <returns></returns>
        public bool Get_HD(int iAx)
        {//이동위치 이동 완료 확인 목적
            bool bRet = (Chk_HD(iAx)) && (Chk_OP(iAx) == EAxOp.Idle);

            return bRet;
        }

        /// <summary>
        /// Get WMX last error message
        /// </summary>
        /// <returns>Error message</returns>
        public string Get_Err()
        {
            string sRet = "";

            CWmx.It.WLib.GetLastErrorString(ref sRet, 255);

            return sRet;
        }


        public double Get_InPos(int iAx)
        {
            double dRet = 0.0;

            dRet = CWmx.It.SParm.FeedbackParam[iAx].InPosWidth / CData.Axes[iAx].dPP1;

            return dRet;
        }

        /// <summary>
        /// 축의 부하량
        /// </summary>
        /// <returns></returns>
        public double Get_Load(int iAx)
        {
            int iInd = 19753;    //index  0x4D29
            int iSub =0;    //sub index
            int iVal = 0;
            int iRet = 0;

            iRet = CWmx.It.Read_SDO(iAx, iInd, iSub, ref iVal);

            return iVal / 10.0;
        }
        #endregion

        #region File
        /// <summary>
        /// 모든 축의 정보 ini 파일 저장
        /// </summary>
        /// <returns>0:Ok</returns>
        public int Save()
        {
            int iRet = 0;
            int iCnt = Enum.GetNames(typeof(EAx)).Length;

            // 경로 없을 시 생성
            if (Directory.Exists(GV.PATH_EQ_MOTION) == false)
            { Directory.CreateDirectory(GV.PATH_EQ_MOTION); }

            for (int i = 0; i < iCnt; i++)
            {
                iRet = Save(i);
                if (iRet != 0)
                {
                    GV.bErr = true;
                    iRet = 1;
                } 
            }

            return iRet;
        }

        /// <summary>
        /// 단일 축의 정보 ini 파일 저장
        /// </summary>
        /// <param name="iAx">축 인덱스</param>
        /// <returns>0:Ok</returns>
        public int Save(int iAx)
        {
            int iRet = 0;
            string sName = "";    // 파일 경로 + 이름
            StringBuilder mSB = new StringBuilder();

            if (iAx > AxL)
            {
                GV.bErr = true;
                iRet = 1;
                _SetLog("Error : Motion index fail.", iAx);

                return -1;
            }

            // 경로 없을 시 생성
            if (Directory.Exists(GV.PATH_EQ_MOTION) == false)
            { Directory.CreateDirectory(GV.PATH_EQ_MOTION); }

            sName = GV.PATH_EQ_MOTION + string.Format("{0}_{1}.mot", iAx, ((EAx)iAx).ToString());

            if (File.Exists(sName))
            { File.Delete(sName); } //이미 파일 있을 시 지우고 다시 생성

            mSB.AppendLine("[INFORMATION]");
            mSB.AppendLine("NAME="               + ((EAx)iAx)          .ToString());
            mSB.AppendLine("USE="                + CData.Axes[iAx].bUse.ToString());
            mSB.AppendLine("BALL SCREW PITCH="   + CData.Axes[iAx].dBSP           );
            mSB.AppendLine("PP1="                + CData.Axes[iAx].dPP1           );
            mSB.AppendLine();
            mSB.AppendLine("[RUN NORMAL]");      
            mSB.AppendLine("VELOCITY="           + CData.Axes[iAx].tRN.iVel       );
            mSB.AppendLine("ACCELERATION="       + CData.Axes[iAx].tRN.iAcc       );
            mSB.AppendLine("DECELERATION="       + CData.Axes[iAx].tRN.iDec       );
            mSB.AppendLine();
            mSB.AppendLine("[RUN SLOW]");                                         
            mSB.AppendLine("VELOCITY="           + CData.Axes[iAx].tRS.iVel       );
            mSB.AppendLine("ACCELERATION="       + CData.Axes[iAx].tRS.iAcc       );
            mSB.AppendLine("DECELERATION="       + CData.Axes[iAx].tRS.iDec       );
            mSB.AppendLine();
            mSB.AppendLine("[JOG NORMAL]");                                       
            mSB.AppendLine("VELOCITY="           + CData.Axes[iAx].tJN.iVel       );
            mSB.AppendLine("ACCELERATION="       + CData.Axes[iAx].tJN.iAcc       );
            mSB.AppendLine("DECELERATION="       + CData.Axes[iAx].tJN.iDec       );
            mSB.AppendLine();
            mSB.AppendLine("[LIMIT]");
            mSB.AppendLine("MCData.Axes LIMIT POSITION=" + CData.Axes[iAx].dSWMax         );
            mSB.AppendLine("MIN LIMIT POSITION=" + CData.Axes[iAx].dSWMin         );
            mSB.AppendLine("NOT SENSOR USE="     + CData.Axes[iAx].bNOT.ToString());
            mSB.AppendLine("POT SENSOR USE="     + CData.Axes[iAx].bPOT.ToString());
            mSB.AppendLine("Acceleration Ratio=" + CData.Axes[iAx].iAccR          );
            mSB.AppendLine("Deceleration Ratio=" + CData.Axes[iAx].iDecR          );

            CLog.Check_File_Access(sName, mSB.ToString(), false);
            _SetLog("Save success.", iAx);

            return iRet;
        }

        /// <summary>
        /// 모든 축의 정보 ini 파일 불러오기
        /// </summary>
        /// <returns>0:Ok</returns>
        public int Load()
        {
            int iRet = 0;

            // 경로 없을 시 생성
            if (Directory.Exists(GV.PATH_EQ_MOTION) == false)
            { Directory.CreateDirectory(GV.PATH_EQ_MOTION); }

            for (int i = AxS; i <= AxL; i++)
            {
                iRet = Load(i);

                if (iRet != 0)
                {
                    GV.bErr = true;
                    iRet = 1;
                }
            }

            return iRet;
        }

        /// <summary>
        /// 단일 축의 정보 ini 파일 불러오기
        /// </summary>
        /// <param name="iAx">축의 인덱스</param>
        /// <returns>0:Ok</returns>
        public int Load(int iAx)
        {
            int iRet = 0;
            string sName = "";    // 파일 경로 + 이름
            string sSec = "";    // INI section
            CIni mIni = null;    // INI class

            if (iAx > AxL)
            {
                GV.bErr = true;
                iRet = 1;
                _SetLog("Error : Motion index fail.", iAx);

                return -1;
            }

            sName = GV.PATH_EQ_MOTION + string.Format("{0}_{1}.mot", iAx, ((EAx)iAx).ToString());

            // 경로 없을 시 생성
            if (Directory.Exists(GV.PATH_EQ_MOTION) == false)
            {
                GV.bErr = true;
                iRet = 1;
                _SetLog("Error : Motion folder not exist.", iAx);

                return -1;
            }
            else
            {
                if (File.Exists(sName) == false)
                {
                    GV.bErr = true;
                    iRet = 1;
                    _SetLog("Error : Motion file not exist.", iAx);

                    return -2;
                }
            }

            mIni = new CIni(sName);
            sSec = "INFORMATION";
            //mIni.Read(sSec, "NAME", ((eAx)iAx).ToString());
            bool.TryParse(mIni.Read(sSec, "USE"), out CData.Axes[iAx].bUse);
            CData.Axes[iAx].dBSP = mIni.ReadD(sSec, "BALL SCREW PITCH");
            CData.Axes[iAx].dPP1 = mIni.ReadD(sSec, "PP1");
            sSec = "RUN NORMAL";
            int.TryParse(mIni.Read(sSec, "VELOCITY"), out CData.Axes[iAx].tRN.iVel);
            int.TryParse(mIni.Read(sSec, "ACCELERATION"), out CData.Axes[iAx].tRN.iAcc);
            int.TryParse(mIni.Read(sSec, "DECELERATION"), out CData.Axes[iAx].tRN.iDec);
            sSec = "RUN SLOW";
            int.TryParse(mIni.Read(sSec, "VELOCITY"), out CData.Axes[iAx].tRS.iVel);
            int.TryParse(mIni.Read(sSec, "ACCELERATION"), out CData.Axes[iAx].tRS.iAcc);
            int.TryParse(mIni.Read(sSec, "DECELERATION"), out CData.Axes[iAx].tRS.iDec);
            sSec = "JOG NORMAL";
            int.TryParse(mIni.Read(sSec, "VELOCITY"), out CData.Axes[iAx].tJN.iVel);
            int.TryParse(mIni.Read(sSec, "ACCELERATION"), out CData.Axes[iAx].tJN.iAcc);
            int.TryParse(mIni.Read(sSec, "DECELERATION"), out CData.Axes[iAx].tJN.iDec);
            sSec = "LIMIT";
            CData.Axes[iAx].dSWMax = mIni.ReadD(sSec, "MCData.Axes LIMIT POSITION");
            CData.Axes[iAx].dSWMin = mIni.ReadD(sSec, "MIN LIMIT POSITION");
            bool.TryParse(mIni.Read(sSec, "NOT SENSOR USE"), out CData.Axes[iAx].bNOT);
            bool.TryParse(mIni.Read(sSec, "POT SENSOR USE"), out CData.Axes[iAx].bPOT);
            int.TryParse(mIni.Read(sSec, "Acceleration Ratio"), out CData.Axes[iAx].iAccR);
            int.TryParse(mIni.Read(sSec, "Deceleration Ratio"), out CData.Axes[iAx].iDecR);

            _SetLog("Load success.", iAx);

            return iRet;
        }

        #endregion

        #region Move method
        /// <summary>
        /// Homing 함수
        /// </summary>
        /// <param name="iAx"></param>
        /// <returns></returns>
        public int Mv_H(int iAx)
        {
            int iRet = 0;

            // 1. 준비 동작 체크
            iRet = Chk_Rdy(iAx, false);
            if (iRet != 0)
            {
                GV.bErr = true;
                iRet = 1;
                _SetLog("Error : Not ready.", iAx);

                return iRet;
            }

            // Home done off
            iRet = CWmx.It.WLib.home.SetHomeDone((short)iAx, 0);
            if (iRet != 0)
            {
                GV.bErr = true;
                iRet = 1;
                _SetLog("Error : Set home done fail -> " + Get_Err(), iAx);

                return -1;
            }

            iRet = CWmx.It.WLib.home.StartHome(iAx);
            if (iRet != 0)
            {
                // Homing 중단
                CWmx.It.WLib.home.Cancel(iAx);
                GV.bErr = true;
                iRet = 1;
                _SetLog("Error : Homing fail -> " + Get_Err(), iAx);

                return iRet;
            }
            _SetLog("Home done.", iAx);

            return iRet;
        }

        /// <summary>
        /// 조그 지령 함수
        /// false : NOT 방향
        /// true : POT 방향
        /// </summary>
        /// <param name="iAx"></param>
        /// <param name="bNP"></param>
        /// <returns></returns>
        public int Mv_J(int iAx, bool bNP)
        {
            int iRet = 0;
            double dVel = CData.Axes[iAx].tJN.iVel * CData.Axes[iAx].dPP1;
            double dAcc = CData.Axes[iAx].tJN.iAcc * CData.Axes[iAx].dPP1;

            if ((iAx == (int)EAx.DryZone_Air) && (CDataOption.Dryer == eDryer.Rotate))
            {
                dVel *= 6.0;
                dAcc *= 6.0;
            }

            // 1. 준비 동작 체크
            iRet = Chk_Rdy(iAx, false);
            if (iRet != 0)
            {
                GV.bErr = true;
                iRet = 1;
                _SetLog("Error : Not ready -> " + Get_Err(), iAx);

                return iRet;
            }

            // old
            //if (!bNP)
            //{ dVel *= -1; }

            // 2021-05-20, jhLee : Limit 센서가 감지중이면 해당 방향으로의 이동을 막는다.
            //
            if (!bNP)
            {
                if (Chk_NOT(iAx))                  // 이미 해당 방향의 Limit Sensor가 감지중이라면 이동을 할 수 없다.
                {
                    GV.bErr = true;
                    _SetLog($"Error : Mv_J()  Axis:{iAx} Move Fail,  NOT is ON");

                    return -1;
                }

                // Negative 방향인 경우 속도를 음수로 만들어서 동작 방향을 지정해준다.
                dVel *= -1;                         // 속도를 이용한 동작 방향 지정
            }
            else
            {
                // CW 방향
                if (Chk_POT(iAx))                  // 이미 해당 방향의 Limit Sensor가 감지중이라면 이동을 할 수 없다.
                {
                    GV.bErr = true;
                    _SetLog($"Error : Mv_J()  Axis:{iAx} Move Fail, POT is ON");            // Log  기록

                    return -1;                      // 이동 실패를 되돌린다.
                }
            }


            iRet = CWmx.It.WLib.basicMotion.StartJog((short)iAx, dVel, dAcc);
            if (iRet != 0)
            {
                GV.bErr = true;
                iRet = 1; 
                _SetLog("Error : Jog fail -> " + Get_Err(), iAx);

                return -1;
            }
            _SetLog("Jog.  Vel : " + dVel + "mm/s", iAx);

            return iRet;
        }

        /// <summary>
        /// 조그 지령 함수
        /// false : NOT 방향
        /// true : POT 방향
        /// </summary>
        /// <param name="iAx"></param>
        /// <param name="bNP"></param>
        /// <returns></returns>
        public int Mv_J(int iAx, bool bNP, double dVel, double dAcc)
        {
            CLog.Save_Log(eLog.None, eLog.MOT, "Mv_J S " + iAx.ToString() + ", " + bNP.ToString() + ", " + dVel.ToString() + ", " + dAcc.ToString());

            //20190227 ghk_err
            //int iRet = (int)eErr.NONE;
            int iRet = 0;

            dVel *= CData.Axes[iAx].dPP1;
            dAcc *= CData.Axes[iAx].dPP1;

            if ((iAx == (int)EAx.DryZone_Air) && (CDataOption.Dryer == eDryer.Rotate))
            {
                dVel *= 6.0;
                dAcc *= 6.0;
            }

            // 1. 준비 동작 체크
            iRet = Chk_Rdy(iAx, false);
            if (iRet != 0)
            {
                GV.bErr = true;
                iRet = 1;
                _SetLog("Error : Not ready -> " + Get_Err(), iAx);

                return iRet;
            }


            // 2021-05-20, jhLee : Limit 센서가 감지중이면 해당 방향으로의 이동을 막는다.
            //
            if ( !bNP )
            {
                if (Chk_NOT(iAx))                  // 이미 해당 방향의 Limit Sensor가 감지중이라면 이동을 할 수 없다.
                {
                    GV.bErr = true;
                    _SetLog($"Error : Mv_J()  Axis:{iAx} Move Fail,  NOT is ON");

                    return -1;
                }

                // Negative 방향인 경우 속도를 음수로 만들어서 동작 방향을 지정해준다.
                dVel *= -1;                         // 속도를 이용한 동작 방향 지정
            }
            else
            {
                // CW 방향
                if (Chk_POT(iAx))                  // 이미 해당 방향의 Limit Sensor가 감지중이라면 이동을 할 수 없다.
                {
                    GV.bErr = true;
                    _SetLog($"Error : Mv_J()  Axis:{iAx} Move Fail, POT is ON");            // Log  기록

                    return -1;                      // 이동 실패를 되돌린다.
                }
            }



            iRet = CWmx.It.WLib.basicMotion.StartJog((short)iAx, dVel, dAcc);
            if (iRet != 0)
            {
                GV.bErr = true;
                iRet = 1;
                _SetLog("Error : Jog fail -> " + Get_Err(), iAx);

                return -1;
            }
            _SetLog("Jog.  Vel : " + dVel + @"mm/s", iAx);

            return iRet;
        }

        /// <summary>
        /// Normal Move 지령 함수
        /// </summary>
        /// <param name="iAx"></param>
        /// <param name="dTarPos"></param>
        /// <returns></returns>
        public int Mv_N(int iAx, double dTarPos)
        {//Normal
            int iRet = 0;
            double dPos = dTarPos * CData.Axes[iAx].dPP1;

            double dSpd = CData.Axes[iAx].tRN.iVel * CData.Axes[iAx].dPP1;
            double dAcc = CData.Axes[iAx].tRN.iAcc * CData.Axes[iAx].dPP1;
            double dDec = CData.Axes[iAx].tRN.iDec * CData.Axes[iAx].dPP1;

            // 1. 준비 동작 체크
            iRet = Chk_Rdy(iAx);
            if (iRet != 0)
            {
                GV.bErr = true;
                _SetLog("Error : Not ready -> " + Get_Err(), iAx);

                return -1;
            }

            // 2. 이동 전 목표 이동 위치 - 이동 가능 여부 확인
            if (CData.Axes[iAx].dSWMin * CData.Axes[iAx].dPP1 > dPos || dPos > CData.Axes[iAx].dSWMax * CData.Axes[iAx].dPP1)
            {
                GV.bErr = true;
                CLog.Save_Log(eLog.None, eLog.MOT, "Mv_N E " + iRet.ToString());
                _SetLog(string.Format("Error : Not enable position.  Min : {0}mm  Max : {1}mm  Pos : {2}mm", CData.Axes[iAx].dSWMin, CData.Axes[iAx].dSWMax, dTarPos), iAx);

                return -1;
            }

            // 3. 이동전 이동하고자 하는 방향의 Limit Sensor가 감지중인지 확인 (2021-05-20, jhLee)
            double dCurrent = Get_FP(iAx);          // 현재 위치 조회
            if ( dCurrent <= dTarPos)                  // 현재 위치에서 양의 방향으로 이동하고자 한다면
            {
                if (Chk_POT(iAx) )                  // 이미 해당 방향의 Limit Sensor가 감지중이라면 이동을 할 수 없다.
                {
                    GV.bErr = true;
                    _SetLog($"Error : Mv_N()  Axis:{iAx} Move Fail, POT is ON, Current:{dCurrent}, Target:{dTarPos}");            // Log  기록

                    return -1;                      // 이동 실패를 되돌린다.
                }
            }
            else if (dCurrent > dTarPos)               // 현재 위치에서 음의 방향으로 이동하고자 한다면
            {
                if (Chk_NOT(iAx))                  // 이미 해당 방향의 Limit Sensor가 감지중이라면 이동을 할 수 없다.
                {
                    GV.bErr = true;
                    _SetLog($"Error : Mv_N()  Axis:{iAx} Move Fail,  NOT is ON, Current:{dCurrent}, Target:{dTarPos}");

                    return -1;
                }
            }



            // 이동 명령 수행
            iRet = CWmx.It.WLib.basicMotion.StartPos((short)iAx, dPos, dSpd, dAcc, dDec);

            // 4. 이동 명령 정상 확인
            if (iRet != 0)
            {
                GV.bErr = true;
                iRet = 1;
                _SetLog("Error : Move fail -> " + Get_Err(), iAx);

                return iRet;
            }

            _SetLog(string.Format("Move.  Pos : {0}mm  Vel : {1}mm/s  Acc : {2}mm/s^2  Dec : {3}mm/s^2", dTarPos, CData.Axes[iAx].tRN.iVel, CData.Axes[iAx].tRN.iAcc, CData.Axes[iAx].tRN.iDec), iAx);
            return iRet;
        }

        /// <summary>
        /// Slow Move 지령 함수
        /// </summary>
        /// <param name="iAx"></param>
        /// <param name="dTarPos"></param>
        /// <returns></returns>
        public int Mv_S(int iAx, double dTarPos)
        {//Slow
            int iRet = 0;
            double dPos = dTarPos * CData.Axes[iAx].dPP1;
            double dSpd = (double)CData.Axes[iAx].tRS.iVel * CData.Axes[iAx].dPP1;
            double dAcc = (double)CData.Axes[iAx].tRS.iAcc * CData.Axes[iAx].dPP1;
            double dDec = (double)CData.Axes[iAx].tRS.iDec * CData.Axes[iAx].dPP1;

            // 1. 준비 동작 체크
            iRet = Chk_Rdy(iAx);

            if (iRet != 0)
            {
                GV.bErr = true;
                iRet = 1;
                _SetLog("Error : Not ready -> " + Get_Err(), iAx);

                return iRet;
            }

            // 2. 이동 전 목표 이동 위치 - 이동 가능 여부 확인
            if (CData.Axes[iAx].dSWMin * CData.Axes[iAx].dPP1 > dPos || dPos > CData.Axes[iAx].dSWMax * CData.Axes[iAx].dPP1)
            {
                GV.bErr = true;
                iRet = 1;
                _SetLog(string.Format("Error : Not enable position.  Min : {0}mm  Max : {1}mm  Pos : {2}mm", CData.Axes[iAx].dSWMin, CData.Axes[iAx].dSWMax, dTarPos), iAx);

                return -1;
            }


            // 3. 이동전 이동하고자 하는 방향의 Limit Sensor가 감지중인지 확인 (2021-05-20, jhLee)
            double dCurrent = Get_FP(iAx);          // 현재 위치 조회
            if (dCurrent <= dTarPos)                  // 현재 위치에서 양의 방향으로 이동하고자 한다면
            {
                if (Chk_POT(iAx))                  // 이미 해당 방향의 Limit Sensor가 감지중이라면 이동을 할 수 없다.
                {
                    GV.bErr = true;
                    _SetLog($"Error : Mv_S() Axis:{iAx} Move Fail, POT is ON, Current:{dCurrent}, Target:{dTarPos}");            // Log  기록

                    return -1;                      // 이동 실패를 되돌린다.
                }
            }
            else if (dCurrent > dTarPos)               // 현재 위치에서 음의 방향으로 이동하고자 한다면
            {
                if (Chk_NOT(iAx))                  // 이미 해당 방향의 Limit Sensor가 감지중이라면 이동을 할 수 없다.
                {
                    GV.bErr = true;
                    _SetLog($"Error : Mv_S() Axis:{iAx} Move Fail,  NOT is ON, Current:{dCurrent}, Target:{dTarPos}");

                    return -1;
                }
            }



            iRet = CWmx.It.WLib.basicMotion.StartPos((short)iAx, dPos, dSpd, dAcc, dDec);

            // 4. 이동 명령 정상 확인
            if (iRet != 0)
            {
                GV.bErr = true;
                iRet = 1;
                _SetLog("Error : Move fail -> " + Get_Err(), iAx);

                return iRet;
            }
            _SetLog(string.Format("Move(slow).  Pos : {0}mm  Vel : {1}mm/s  Acc : {2}mm/s^2  Dec : {3}mm/s^2", dTarPos, CData.Axes[iAx].tRN.iVel, CData.Axes[iAx].tRN.iAcc, CData.Axes[iAx].tRN.iDec), iAx);

            return iRet;
        }

        /// <summary>
        /// 속도 지정 Move 지령 함수, 가/감속도는 Normal 속도대비 자동 지정
        /// </summary>
        /// <param name="iAx"></param>
        /// <param name="dTarPos"></param>
        /// <returns></returns>
        public int Mv_Speed(int iAx, double dTarPos, double dSpeed)
        {//Normal
            int iRet = 0;
            double dPos = dTarPos * CData.Axes[iAx].dPP1;
            double dSpd = dSpeed * CData.Axes[iAx].dPP1;
            double dRatio = dSpeed / CData.Axes[iAx].tRN.iVel;      // Normal 속도 대비 비율
            double dAcc = CData.Axes[iAx].tRN.iAcc * CData.Axes[iAx].dPP1 * dRatio;     // Normal 속도 대비 비율로 계산한다.
            double dDec = CData.Axes[iAx].tRN.iDec * CData.Axes[iAx].dPP1 * dRatio;


            // 1. 준비 동작 체크
            iRet = Chk_Rdy(iAx);
            if (iRet != 0)
            {
                GV.bErr = true;
                _SetLog("Error : Not ready -> " + Get_Err(), iAx);

                return -1;
            }

            // 2. 이동 전 목표 이동 위치 - 이동 가능 여부 확인
            if (CData.Axes[iAx].dSWMin * CData.Axes[iAx].dPP1 > dPos || dPos > CData.Axes[iAx].dSWMax * CData.Axes[iAx].dPP1)
            {
                GV.bErr = true;
                CLog.Save_Log(eLog.None, eLog.MOT, "Mv_N E " + iRet.ToString());
                _SetLog(string.Format("Error : Not enable position.  Min : {0}mm  Max : {1}mm  Pos : {2}mm", CData.Axes[iAx].dSWMin, CData.Axes[iAx].dSWMax, dTarPos), iAx);

                return -1;
            }

            // 3. 이동전 이동하고자 하는 방향의 Limit Sensor가 감지중인지 확인 (2021-05-20, jhLee)
            double dCurrent = Get_FP(iAx);          // 현재 위치 조회
            if (dCurrent <= dTarPos)                  // 현재 위치에서 양의 방향으로 이동하고자 한다면
            {
                if (Chk_POT(iAx))                  // 이미 해당 방향의 Limit Sensor가 감지중이라면 이동을 할 수 없다.
                {
                    GV.bErr = true;
                    _SetLog($"Error : Mv_Speed() Axis:{iAx} Move Fail, POT is ON, Current:{dCurrent}, Target:{dTarPos}");            // Log  기록

                    return -1;                      // 이동 실패를 되돌린다.
                }
            }
            else if (dCurrent > dTarPos)               // 현재 위치에서 음의 방향으로 이동하고자 한다면
            {
                if (Chk_NOT(iAx))                  // 이미 해당 방향의 Limit Sensor가 감지중이라면 이동을 할 수 없다.
                {
                    GV.bErr = true;
                    _SetLog($"Error : Mv_Speed() Axis:{iAx} Move Fail,  NOT is ON, Current:{dCurrent}, Target:{dTarPos}");

                    return -1;
                }
            }


            iRet = CWmx.It.WLib.basicMotion.StartPos((short)iAx, dPos, dSpd, dAcc, dDec);
            // 4. 이동 명령 정상 확인
            if (iRet != 0)
            {
                GV.bErr = true;
                iRet = 1;
                _SetLog("Error : Move fail -> " + Get_Err(), iAx);

                return iRet;
            }

            _SetLog(string.Format("Move.  Pos : {0}mm  Vel : {1}mm/s  Acc : {2}mm/s^2  Dec : {3}mm/s^2", dTarPos, CData.Axes[iAx].tRN.iVel, CData.Axes[iAx].tRN.iAcc, CData.Axes[iAx].tRN.iDec), iAx);
            return iRet;
        }



        //190614 koo : Pause
        /// <summary>
        /// Axis Pause 지령 함수
        /// </summary>
        /// <param name="iAx"></param>
        /// <returns></returns>
        public int Pause(int iAx)
        {//Slow
            int iRet = 0;
            //CLog.Save_Log(eLog.None, eLog.MOT, "Pause" + iAx.ToString() + ", " + "On");

            iRet = CWmx.It.WLib.basicMotion.Pause(iAx);
            if (iRet != 0)
            {
                string s = "";
                CWmx.It.WLib.GetLastErrorString(ref s, 1000);
                return 1;
            }
            //CLog.Save_Log(eLog.None, eLog.MOT, "Pause F " + iRet.ToString());
            return iRet;
        }
        //190614 koo : Pause
        /// <summary>
        /// Axis Resume 지령 함수
        /// </summary>
        /// <param name="iAx"></param>
        /// <returns></returns>
        public int Resume(int iAx)
        {//Slow
            int iRet = 0;
            //CLog.Save_Log(eLog.None, eLog.MOT, "Resume" + iAx.ToString() + ", " + "On");

            iRet = CWmx.It.WLib.basicMotion.Resume(iAx);
            if (iRet != 0)
            {
                /*
                GV.bErr = true;
                iRet = 1;
                CLog.Save_Log(eLog.None, eLog.MOT, "Resume E " + iRet.ToString());
                return iRet;
                */
                string s = "";
                CWmx.It.WLib.GetLastErrorString(ref s, 1000);
                return 1;
            }
            //CLog.Save_Log(eLog.None, eLog.MOT, "Resume F " + iRet.ToString());
            return iRet;
        }
        //190614 koo : Pause
        /// <summary>
        /// Axis Cancel 지령 함수
        /// 홈동작시 일시 정지 기능
        /// </summary>
        /// <param name="iAx"></param>
        /// <returns></returns>
        public int Cancel(int iAx)
        {//Slow
            int iRet = 0;
            //CLog.Save_Log(eLog.None, eLog.MOT, "Resume" + iAx.ToString() + ", " + "On");

            iRet = CWmx.It.WLib.home.Cancel(iAx);
            if (iRet != 0)
            {
                /*
                GV.bErr = true;
                iRet = 1;
                CLog.Save_Log(eLog.None, eLog.MOT, "Resume E " + iRet.ToString());
                return iRet;
                */
                string s = "";
                CWmx.It.WLib.GetLastErrorString(ref s, 1000);
                return 1;
            }
            //CLog.Save_Log(eLog.None, eLog.MOT, "Resume F " + iRet.ToString());
            return iRet;
        }
        //190614 koo : Pause
        /// <summary>
        /// Axis Continue 지령 함수
        /// 홈동작시 일시 정지된 축을 다시 홈동작 되게 하는 기능
        /// </summary>
        /// <param name="iAx"></param>
        /// <returns></returns>
        public int Continue(int iAx)
        {//Slow
            int iRet = 0;
            //CLog.Save_Log(eLog.None, eLog.MOT, "Resume" + iAx.ToString() + ", " + "On");

            iRet = CWmx.It.WLib.home.Continue(iAx);
            if (iRet != 0)
            {
                /*
                GV.bErr = true;
                iRet = 1;
                CLog.Save_Log(eLog.None, eLog.MOT, "Resume E " + iRet.ToString());
                return iRet;
                */
                string s = "";
                CWmx.It.WLib.GetLastErrorString(ref s, 1000);
                return 1;
            }
            //CLog.Save_Log(eLog.None, eLog.MOT, "Resume F " + iRet.ToString());
            return iRet;
        }
        /// <summary>
        /// Step 이동 시 지령 함수
        /// </summary>
        /// <param name="iAx"></param>
        /// <param name="dStp"></param>
        /// <returns></returns>
        public int Mv_I(int iAx, double dStp)
        {
            double dOld;    // 이전 포지션
            double dTar;    // Target pulse value
            double dSpd = (double)CData.Axes[iAx].tRS.iVel * CData.Axes[iAx].dPP1;
            double dAcc = (double)CData.Axes[iAx].tRS.iAcc * CData.Axes[iAx].dPP1;
            double dDec = (double)CData.Axes[iAx].tRS.iDec * CData.Axes[iAx].dPP1;
            int iRet = 0;

            // 1. 준비 동작 체크
            iRet = Chk_Rdy(iAx);
            if (iRet != 0)
            {
                GV.bErr = true;
                iRet = 1;
                _SetLog("Error : Not ready -> " + Get_Err(), iAx);

                return iRet;
            }


            // 2. 이동전 이동하고자 하는 방향의 Limit Sensor가 감지중인지 확인 (2021-05-20, jhLee)
            if (dStp >= 0.0)                       // 현재 위치에서 양의 방향으로 이동하고자 한다면
            {
                if (Chk_POT(iAx))                  // 이미 해당 방향의 Limit Sensor가 감지중이라면 이동을 할 수 없다.
                {
                    GV.bErr = true;
                    _SetLog($"Error : Mv_I() Axis:{iAx} Move Fail, POT is ON, Step:{dStp}");            // Log  기록

                    return -1;                      // 이동 실패를 되돌린다.
                }
            }
            else                                   // 현재 위치에서 음의 방향으로 이동하고자 한다면
            {
                if (Chk_NOT(iAx))                  // 이미 해당 방향의 Limit Sensor가 감지중이라면 이동을 할 수 없다.
                {
                    GV.bErr = true;
                    _SetLog($"Error : Mv_I() Axis:{iAx} Move Fail,  NOT is ON, Step:{dStp}");

                    return -1;
                }
            }


            // 3. 포지션 체크
            dOld = Get_CP(iAx);
            dOld += dStp;

            if (CData.Axes[iAx].dSWMin * CData.Axes[iAx].dPP1 > dOld || dOld > CData.Axes[iAx].dSWMax * CData.Axes[iAx].dPP1)
            {
                GV.bErr = true;
                iRet = 1;
                _SetLog(string.Format("Error : Not enable position.  Min : {0}mm  Max : {1}mm  Pos : {2}mm", CData.Axes[iAx].dSWMin, CData.Axes[iAx].dSWMax, dOld), iAx);

                return -1;
            }

            dTar = dStp * CData.Axes[iAx].dPP1;

            iRet = CWmx.It.WLib.basicMotion.StartMov((short)iAx, dTar, dSpd, dAcc, dDec);
            if (iRet != 0)
            {
                GV.bErr = true;
                iRet = 1;
                _SetLog("Error : Move fail -> " + Get_Err(), iAx);

                return iRet;
            }
            _SetLog(string.Format("Move(interval).  Step : {0}mm  Pos : {1}mm", dStp, dOld), iAx);

            return iRet;

        }

        /// <summary>
        /// 다축 동시 Normal 이동 함수
        /// </summary>
        /// <param name="iAx1"></param>
        /// <param name="dPos1"></param>
        /// <param name="iAx2"></param>
        /// <param name="dPos2"></param>
        /// <returns></returns>
        public int Mv_MN(int iAx1, double dPos1, int iAx2, double dPos2)
        {
            int iRet = 0;

            // 1. 제한 포지션 체크
            if (CData.Axes[iAx1].dSWMin > dPos1 || dPos1 > CData.Axes[iAx1].dSWMax)
            {
                GV.bErr = true;
                iRet = 1;
                _SetLog("Error : Limit position 1.");

                return iRet;
            }

            // 2. 제한 포지션 체크 2
            if (CData.Axes[iAx2].dSWMin > dPos2 || dPos2 > CData.Axes[iAx2].dSWMax)
            {
                GV.bErr = true;
                iRet = 1;
                _SetLog("Error : Limit position 2.");

                return iRet;
            }


            // 3. 이동전 이동하고자 하는 방향의 Limit Sensor가 감지중인지 확인 (2021-05-20, jhLee)
            double dCurrent = Get_FP(iAx1);          // 현재 위치 조회
            if (dCurrent < dPos1)                  // 현재 위치에서 양의 방향으로 이동하고자 한다면
            {
                if (Chk_POT(iAx1))                  // 이미 해당 방향의 Limit Sensor가 감지중이라면 이동을 할 수 없다.
                {
                    GV.bErr = true;
                    _SetLog($"Error : Mv_MN() Axis:{iAx1} Move Fail, POT is ON, Current:{dCurrent}, Target:{dPos1}");            // Log  기록

                    return -1;                      // 이동 실패를 되돌린다.
                }
            }
            else                                    // 현재 위치에서 음의 방향으로 이동하고자 한다면
            {
                if (Chk_NOT(iAx1))                  // 이미 해당 방향의 Limit Sensor가 감지중이라면 이동을 할 수 없다.
                {
                    GV.bErr = true;
                    _SetLog($"Error : Mv_MN() Axis:{iAx1} Move Fail,  NOT is ON, Current:{dCurrent}, Target:{dPos1}");

                    return -1;
                }
            }

            dCurrent = Get_FP(iAx2);                // 2번째 축 현재 위치 조회
            if (dCurrent < dPos2)                  // 현재 위치에서 양의 방향으로 이동하고자 한다면
            {
                if (Chk_POT(iAx2))                  // 이미 해당 방향의 Limit Sensor가 감지중이라면 이동을 할 수 없다.
                {
                    GV.bErr = true;
                    _SetLog($"Error : Mv_MN() Axis:{iAx2} Move Fail, POT is ON, Current:{dCurrent}, Target:{dPos2}");            // Log  기록

                    return -1;                      // 이동 실패를 되돌린다.
                }
            }
            else                                    // 현재 위치에서 음의 방향으로 이동하고자 한다면
            {
                if (Chk_NOT(iAx2))                  // 이미 해당 방향의 Limit Sensor가 감지중이라면 이동을 할 수 없다.
                {
                    GV.bErr = true;
                    _SetLog($"Error : Mv_MN() Axis:{iAx2} Move Fail,  NOT is ON, Current:{dCurrent}, Target:{dPos2}");

                    return -1;
                }
            }


            wmxCLRLibrary.basicMotion.PosBlock mBlk = new wmxCLRLibrary.basicMotion.PosBlock();
            mBlk.AxisCount = 2;
            mBlk.Block[0].Axis = (short)iAx1;
            mBlk.Block[0].Target = dPos1 * CData.Axes[iAx1].dPP1;
            mBlk.Block[0].Velocity = CData.Axes[iAx1].tRN.iVel * CData.Axes[iAx1].dPP1;
            mBlk.Block[0].Acc = CData.Axes[iAx1].tRN.iAcc * CData.Axes[iAx1].dPP1;
            mBlk.Block[0].Dec = CData.Axes[iAx1].tRN.iDec * CData.Axes[iAx1].dPP1;
            mBlk.Block[1].Axis = (short)iAx2;
            mBlk.Block[1].Target = dPos2 * CData.Axes[iAx2].dPP1;
            mBlk.Block[1].Velocity = CData.Axes[iAx2].tRN.iVel * CData.Axes[iAx2].dPP1;
            mBlk.Block[1].Acc = CData.Axes[iAx2].tRN.iAcc * CData.Axes[iAx2].dPP1;
            mBlk.Block[1].Dec = CData.Axes[iAx2].tRN.iDec * CData.Axes[iAx2].dPP1;


            iRet = CWmx.It.WLib.basicMotion.StartPos(mBlk);
            if (iRet != 0)
            {
                GV.bErr = true;
                iRet = 1;
                _SetLog("Error : Move fail -> " + Get_Err());

                return iRet;
            }
            _SetLog("Move axes.");

            return iRet;
        }

        /// <summary>
        /// 다축 동시 Slow 이동 함수
        /// </summary>
        /// <param name="iAx1"></param>
        /// <param name="dPos1"></param>
        /// <param name="iAx2"></param>
        /// <param name="dPos2"></param>
        /// <returns></returns>
        public int Mv_MS(int iAx1, double dPos1, int iAx2, double dPos2)
        {
            int iRet = 0;

            // 1. 제한 포지션 체크
            if (CData.Axes[iAx1].dSWMin > dPos1 || dPos1 > CData.Axes[iAx1].dSWMax)
            {
                GV.bErr = true;
                iRet = 1;
                _SetLog("Error : Limit position 1.");

                return iRet;
            }

            // 2. 제한 포지션 체크 2
            if (CData.Axes[iAx2].dSWMin > dPos2 || dPos2 > CData.Axes[iAx2].dSWMax)
            {
                GV.bErr = true;
                iRet = 1;
                _SetLog("Error : Limit position 2.");

                return iRet;
            }


            // 3. 이동전 이동하고자 하는 방향의 Limit Sensor가 감지중인지 확인 (2021-05-20, jhLee)
            double dCurrent = Get_FP(iAx1);          // 현재 위치 조회
            if (dCurrent < dPos1)                  // 현재 위치에서 양의 방향으로 이동하고자 한다면
            {
                if (Chk_POT(iAx1))                  // 이미 해당 방향의 Limit Sensor가 감지중이라면 이동을 할 수 없다.
                {
                    GV.bErr = true;
                    _SetLog($"Error : Mv_MS() Axis:{iAx1} Move Fail, POT is ON, Current:{dCurrent}, Target:{dPos1}");            // Log  기록

                    return -1;                      // 이동 실패를 되돌린다.
                }
            }
            else                                    // 현재 위치에서 음의 방향으로 이동하고자 한다면
            {
                if (Chk_NOT(iAx1))                  // 이미 해당 방향의 Limit Sensor가 감지중이라면 이동을 할 수 없다.
                {
                    GV.bErr = true;
                    _SetLog($"Error : Mv_MS() Axis:{iAx1} Move Fail,  NOT is ON, Current:{dCurrent}, Target:{dPos1}");

                    return -1;
                }
            }

            dCurrent = Get_FP(iAx2);                // 2번째 축 현재 위치 조회
            if (dCurrent < dPos2)                  // 현재 위치에서 양의 방향으로 이동하고자 한다면
            {
                if (Chk_POT(iAx2))                  // 이미 해당 방향의 Limit Sensor가 감지중이라면 이동을 할 수 없다.
                {
                    GV.bErr = true;
                    _SetLog($"Error : Mv_MS() Axis:{iAx2} Move Fail, POT is ON, Current:{dCurrent}, Target:{dPos2}");            // Log  기록

                    return -1;                      // 이동 실패를 되돌린다.
                }
            }
            else                                    // 현재 위치에서 음의 방향으로 이동하고자 한다면
            {
                if (Chk_NOT(iAx2))                  // 이미 해당 방향의 Limit Sensor가 감지중이라면 이동을 할 수 없다.
                {
                    GV.bErr = true;
                    _SetLog($"Error : Mv_MS() Axis:{iAx2} Move Fail,  NOT is ON, Current:{dCurrent}, Target:{dPos2}");

                    return -1;
                }
            }


            wmxCLRLibrary.basicMotion.PosBlock mBlk = new wmxCLRLibrary.basicMotion.PosBlock();
            mBlk.AxisCount = 2;
            mBlk.Block[0].Axis = (short)iAx1;
            mBlk.Block[0].Target = dPos1 * CData.Axes[iAx1].dPP1;
            mBlk.Block[0].Velocity = CData.Axes[iAx1].tRS.iVel * CData.Axes[iAx1].dPP1;
            mBlk.Block[0].Acc = CData.Axes[iAx1].tRS.iAcc * CData.Axes[iAx1].dPP1;
            mBlk.Block[0].Dec = CData.Axes[iAx1].tRS.iDec * CData.Axes[iAx1].dPP1;
            mBlk.Block[1].Axis = (short)iAx2;
            mBlk.Block[1].Target = dPos2 * CData.Axes[iAx2].dPP1;
            mBlk.Block[1].Velocity = CData.Axes[iAx2].tRS.iVel * CData.Axes[iAx2].dPP1;
            mBlk.Block[1].Acc = CData.Axes[iAx2].tRS.iAcc * CData.Axes[iAx2].dPP1;
            mBlk.Block[1].Dec = CData.Axes[iAx2].tRS.iDec * CData.Axes[iAx2].dPP1;

            iRet = CWmx.It.WLib.basicMotion.StartPos(mBlk);
            if (iRet != 0)
            {
                GV.bErr = true;
                iRet = 1;
                _SetLog("Error : Move fail -> " + Get_Err());

                return iRet;
            }
            _SetLog("Move axes(slow).");

            return iRet;
        }

        /// <summary>
        /// 이동 속도 지정 이동 함수
        /// </summary>
        /// <param name="iAx"></param>
        /// <param name="dPos"></param>
        /// <param name="dVel"></param>
        /// <returns></returns>
        public int Mv_V(int iAx, double dTarPos, double dVel)
        {
            int iRet = 0;
            double dPos = dTarPos * CData.Axes[iAx].dPP1;
            double dSpd = dVel * CData.Axes[iAx].dPP1;
            double dAcc = dSpd * (double)CData.Axes[iAx].iAccR;
            double dDec = dSpd * (double)CData.Axes[iAx].iDecR;


            // 1. 준비 동작 체크
            iRet = Chk_Rdy(iAx);
            if (iRet != 0)
            {
                GV.bErr = true;
                _SetLog("Error : Not ready -> " + Get_Err(), iAx);

                return -1;
            }

            // 2. 이동 전 목표 이동 위치 - 이동 가능 여부 확인
            if (CData.Axes[iAx].dSWMin * CData.Axes[iAx].dPP1 > dPos || dPos > CData.Axes[iAx].dSWMax * CData.Axes[iAx].dPP1)
            {
                GV.bErr = true;
                CLog.Save_Log(eLog.None, eLog.MOT, "Mv_V E " + iRet.ToString());
                _SetLog(string.Format("Error : Not enable position.  Min : {0}mm  Max : {1}mm  Pos : {2}mm", CData.Axes[iAx].dSWMin, CData.Axes[iAx].dSWMax, dTarPos), iAx);

                return -1;
            }

            //  이동전 이동하고자 하는 방향의 Limit Sensor가 감지중인지 확인 (2021-05-20, jhLee)
            double dCurrent = Get_FP(iAx);          // 현재 위치 조회
            if (dCurrent <= dTarPos)                  // 현재 위치에서 양의 방향으로 이동하고자 한다면
            {
                if (Chk_POT(iAx))                  // 이미 해당 방향의 Limit Sensor가 감지중이라면 이동을 할 수 없다.
                {
                    GV.bErr = true;
                    _SetLog($"Error : Mv_V() Axis:{iAx} Move Fail, POT is ON, Current:{dCurrent}, Target:{dTarPos}");            // Log  기록

                    return -1;                      // 이동 실패를 되돌린다.
                }
            }
            else                                    // 현재 위치에서 음의 방향으로 이동하고자 한다면
            {
                if (Chk_NOT(iAx))                  // 이미 해당 방향의 Limit Sensor가 감지중이라면 이동을 할 수 없다.
                {
                    GV.bErr = true;
                    _SetLog($"Error : Mv_V() Axis:{iAx} Move Fail,  NOT is ON, Current:{dCurrent}, Target:{dTarPos}");

                    return -1;
                }
            }


            iRet = CWmx.It.WLib.basicMotion.StartPos((short)iAx, dPos, dSpd, dAcc, dDec);
            if (iRet != 0)
            {
                GV.bErr = true;
                iRet = 1;
                _SetLog("Error : Move fail -> " + Get_Err(), iAx);

                return iRet;
            }
            _SetLog(string.Format("Move.  Pos : {0}mm  Vel : {1}mm/s", dTarPos, dVel), iAx);

            return iRet;
        }

        /// <summary>
        /// Dry R축 이동 지령 함수
        /// </summary>
        /// <param name="dTarPos"></param>
        /// <param name="iRPM"></param>
        /// <returns></returns>
        public int MV_R(double dTarPos, int iRPM)
        {
            int iAx = (int)EAx.DryZone_Air;
            int iRet = 0;
            double dPos = dTarPos * CData.Axes[iAx].dPP1;
            double dSpd = (double)iRPM * CData.Axes[iAx].dPP1 * 6.0;
            double dAcc = dSpd;
            double dDec = dSpd;

            iRet = CWmx.It.WLib.basicMotion.StartPos((short)iAx, dPos, dSpd, dAcc, dDec);
            if (iRet != 0)
            {
                GV.bErr = true;
                iRet = 1;
                _SetLog("Error : Move fail -> " + Get_Err(), iAx);

                return iRet;
            }
            _SetLog(string.Format("Move.  Pos : {0}mm  Vel : {1}rpm", dTarPos, iRPM), iAx);

            return iRet;
        }

        public int Stop(int iAx)
        {
            int iRet = 0;

			iRet = CWmx.It.WLib.basicMotion.Stop(iAx);
            if (iRet != 0)
            {
                GV.bErr = true;
                iRet = 1;
                _SetLog("Error : Stop fail -> " + Get_Err(), iAx);
                EStop(iAx);

                return iRet;
            }
            _SetLog("Stop.", iAx);

            return iRet;
        }

        public int EStop(int iAx)
        {
            int iRet = 0;
            iRet = CWmx.It.WLib.basicMotion.ExecQuickStop(iAx);
            if (iRet != 0)
            {
                GV.bErr = true;
                iRet = 1;
                _SetLog("Error : EStop fail -> " + Get_Err(), iAx);

                return iRet;
            }
            _SetLog("EStop.", iAx);

            return iRet;
        }
        #endregion
        #region
        /// Memory Log 획득 방법
        /// Max 40 [ms] 까지 Data 확보 가능
        /// 

        MemoryLogData memData = new MemoryLogData();
        MemoryLogStatus memStatus = new MemoryLogStatus();
        AxisSelection axisSel = new AxisSelection();
        private void InitMemoryLog()
        {
            CWmx.It.WLib.log.GetMemoryLogStatus(ref memStatus);

            if (memStatus.LogState != LogState.Idle)
            {
                CWmx.It.WLib.log.StopMemoryLog();
                if (CDataOption.MeasureType == eMeasureType.Step)
                { CWmx.It.WLib.eventControl.ClearAllEvent(); }
            }
        }
        public int SetMemoryLog(string sData)
        {
            CLog.mLogEncoder(sData);
            InitMemoryLog();

            axisSel.AxisCount = 6;
            axisSel.Axis[0 ] = (int)EAx.LeftGrindZone_X ;
            axisSel.Axis[1 ] = (int)EAx.LeftGrindZone_Y ;
            axisSel.Axis[2 ] = (int)EAx.LeftGrindZone_Z ;
            axisSel.Axis[3 ] = (int)EAx.RightGrindZone_X ;
            axisSel.Axis[4 ] = (int)EAx.RightGrindZone_Y ;
            axisSel.Axis[5 ] = (int)EAx.RightGrindZone_Z ;

            MemoryLogOptions memOpition = new MemoryLogOptions(); // Could be ignored
            memOpition.TriggerEventCount = 0;

            int ret = CWmx.It.WLib.log.SetMemoryLog(axisSel, memOpition);
            if (ret != 0)
            {
                return (int)CWmx.It.WLib.GetLastError();
            }
            else
            {
                ret = CWmx.It.WLib.log.StartMemoryLog();
                if (ret != 0)
                {
                    return (int)CWmx.It.WLib.GetLastError();
                }
            }
            return 0;
        }
        public int ReadMemoryLog_Data()
        {
            if (CSQ_Main.It.m_iStat != EStatus.Manual) return 0;

            int[] nAx = new int[6];
            nAx[0] = (int)EAx.LeftGrindZone_X ;
            nAx[1] = (int)EAx.LeftGrindZone_Y ;
            nAx[2] = (int)EAx.LeftGrindZone_Z ;
            nAx[3] = (int)EAx.RightGrindZone_X ;
            nAx[4] = (int)EAx.RightGrindZone_Y ;
            nAx[5] = (int)EAx.RightGrindZone_Z ;

            int[] nAx_PPI = new int[6];
            nAx_PPI[0] = 1000;
            nAx_PPI[1] = 1000;
            nAx_PPI[2] = 10000;
            nAx_PPI[3] = 1000;
            nAx_PPI[4] = 1000;
            nAx_PPI[5] = 10000;

            int nCnt0 = 0;
            int nCnt1 = 0;
            string sData = "";
            string sDay = DateTime.Now.ToString("[yyyyMMdd-HHmmss.fff]");
            CWmx.It.WLib.log.GetMemoryLogData(ref memData);
            //Console.WriteLine(sData);
            /*if (memData.OverflowFlag == 1)
            {
             // OverFlow 는 처리 하지 않음.
            }
            else*/
            {
                //sData = sDay;               

                for (nCnt0 = 0; nCnt0 < memData.Count; nCnt0++)
                {
                    sDay = DateTime.Now.ToString("\n[yyyyMMdd-HHmmss.fff]");
                    sData += sDay;
                    sData += "[Cnt = " + nCnt0 + "]";
                    for (nCnt1 = 0; nCnt1 < 6; nCnt1++)
                    {                        
                        sData += ",[Axis"+ nAx[nCnt1] +","+ Chk_OP(nAx[nCnt1]) + "],CmdPos = " + (memData.LogData[nCnt0].LogAxisData[nCnt1].CommandPos/nAx_PPI[nCnt1]).ToString("0.0000") + ",EncPos = "+ (memData.LogData[nCnt0].LogAxisData[nCnt1].FeedbackPos/nAx_PPI[nCnt1]).ToString("0.0000") + ",FedVec = " + (memData.LogData[nCnt0].LogAxisData[nCnt1].FeedbackVelocity/nAx_PPI[nCnt1]).ToString("0.0000");
                    }
                    // Console.WriteLine(sData);
                    //sData += "\n";
                }
                CLog.mLogEncoder(sData);
            }
            return 0;
        }

        public int StopMemoryLog()
        {
            int ret = 0;
            ret = CWmx.It.WLib.log.StopMemoryLog();
            if (ret != 0)
            {
                return (int)CWmx.It.WLib.GetLastError();
            }
            return 0;
        }
        #endregion


    }
}