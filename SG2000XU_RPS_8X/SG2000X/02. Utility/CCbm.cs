using System;
using System.Collections;
using System.IO;
using System.IO.Ports;
using System.Text;

namespace SG2000X
{
    class CCylinder
    {
        public string sName;

        public string   sIn_01_Name;
        public string   sIn_01_Addr;
        public int      nIn_01_Addr;

        public string   sIn_02_Name;
        public string   sIn_02_Addr;
        public int      nIn_02_Addr;
        public bool     bDualIn;

        public string   sOut_01_Name;
        public string   sOut_01_Addr;
        public int      nOut_01_Addr;

        public string   sOut_02_Name;
        public string   sOut_02_Addr;
        public int      nOut_02_Addr;
        public bool     bDualOut;   //복동실린더
    }



    public class CCbm : CStn<CCbm>
    {
        /// <summary>
        /// CBM
        /// </summary>
        
        public string[] m_sCmd;
        public string[] m_sRec;
        public string sCBM_CYL_PATH;

        public ArrayList m_alCylinder = new ArrayList();


        private CCbm()
        {
            Initial();
        }

        public void Initial()
        {
            sCBM_CYL_PATH = GV.PATH_EQ_CBM + "CBM_Cylinder.txt";

            //Load();
            Initiallize();
        }

        public void Initiallize()
        {
            

        }

        public void Release()
        {
        }

        public int Save(string sCyldName, string sText)
        {
            DateTime dtNow = DateTime.Now;
            string sTime = dtNow.ToString("yyyy.MM.dd_HHmmss");
            string sPath = GV.PATH_EQ_CBM + "CBM_Cyldinder_" + sCyldName + "_" + sTime + ".txt";

            StringBuilder mSB = new StringBuilder();
            if (!Directory.Exists(GV.PATH_EQ_CBM)) { Directory.CreateDirectory(GV.PATH_EQ_CBM); }

            mSB.AppendLine(sText);

            //200213 ksg :
            CLog.Check_File_Access(sPath, mSB.ToString(), false);
            return 0;
        }

        public int Load()
        {

            if (!File.Exists(sCBM_CYL_PATH))
            {
                // 디폴트 파일생성
                SaveDefault();
            }
            CIni mIni = new CIni(sCBM_CYL_PATH);

            string sSec = "";
            m_alCylinder.Clear();

            sSec = "Onloader_MGZ_Pusher";           ReadSection(ref mIni, sSec); // 1
            sSec = "Onloader_MGZ_Clamp";            ReadSection(ref mIni, sSec); // 2
            sSec = "InRail_Gripper_Clamp_Rear";     ReadSection(ref mIni, sSec); // 3
            sSec = "InRail_Align_Guide";            ReadSection(ref mIni, sSec); // 4
            sSec = "InRail_Gripper_Clamp";          ReadSection(ref mIni, sSec); // 5
            sSec = "InRail_Lift";                   ReadSection(ref mIni, sSec); // 6
            sSec = "InRail_Probe";                  ReadSection(ref mIni, sSec); // 7
            sSec = "Onloader_Picker_Rotate";        ReadSection(ref mIni, sSec); // 8
            sSec = "Left_Table_Cleaner";            ReadSection(ref mIni, sSec); // 9
            sSec = "Left_Probe";                    ReadSection(ref mIni, sSec); // 10
            sSec = "Right_Table_Cleaner";           ReadSection(ref mIni, sSec); // 11
            sSec = "Right_Probe";                   ReadSection(ref mIni, sSec); // 12
            sSec = "Offloader_Picker_Rotate";       ReadSection(ref mIni, sSec); // 13
            sSec = "Offloader_MGZ1_UpDown";         ReadSection(ref mIni, sSec); // 14
            sSec = "Offloader_MGZ1_Clamp";          ReadSection(ref mIni, sSec); // 15
            sSec = "Offloader_MGZ2_UpDown";         ReadSection(ref mIni, sSec); // 16
            sSec = "Offloader_MGZ2_Clamp";          ReadSection(ref mIni, sSec); // 17

            int nCnt = m_alCylinder.Count;

            return 0;
        }

        private void ReadSection(ref CIni Ini, string sSec)
        {
            string sTemp = "";

            string  sIn_01_Name = "",   sIn_01_Addr = "",   sIn_02_Name = "",   sIn_02_Addr = "";
            string  sOut_01_Name = "",  sOut_01_Addr = "",  sOut_02_Name = "",  sOut_02_Addr = "";
            int     nIn_01_Addr = -1,   nIn_02_Addr = -1,   nOut_01_Addr = -1,  nOut_02_Addr = -1;
            bool    bDualIn = false,    bDualOut = false;
            int     nTemp = -1;

            //------- In_01
            sIn_01_Name = Ini.Read(sSec, "In_01_Name").Trim();
            sIn_01_Addr = Ini.Read(sSec, "In_01_Addr").Trim();
            sTemp = sIn_01_Addr.Replace("X", "");
            if (sTemp != "")
            {
                nTemp = int.Parse(sTemp, System.Globalization.NumberStyles.HexNumber);
                if (nTemp > 0)
                {
                    nIn_01_Addr = nTemp;
                }
            }
            //------- In_02
            sIn_02_Name = Ini.Read(sSec, "In_02_Name").Trim();
            sIn_02_Addr = Ini.Read(sSec, "In_02_Addr").Trim();
            sTemp = sIn_02_Addr.Replace("X", "");
            if (sTemp != "")
            {
                nTemp = int.Parse(sTemp, System.Globalization.NumberStyles.HexNumber);
                if (nTemp > 0)
                {
                    nIn_02_Addr = nTemp;
                    bDualIn = true;
                }
            }
            //------- Out_01
            sOut_01_Name = Ini.Read(sSec, "Out_01_Name").Trim();
            sOut_01_Addr = Ini.Read(sSec, "Out_01_Addr").Trim();
            sTemp = sOut_01_Addr.Replace("Y", "");
            if (sTemp != "")
            {
                nTemp = int.Parse(sTemp, System.Globalization.NumberStyles.HexNumber);
                if (nTemp > 0)
                {
                    nOut_01_Addr = nTemp;
                }
            }
            //------- Out_02
            sOut_02_Name = Ini.Read(sSec, "Out_02_Name").Trim();
            sOut_02_Addr = Ini.Read(sSec, "Out_02_Addr").Trim();
            sTemp = sOut_02_Addr.Replace("Y", "");
            if (sTemp != "")
            {
                nTemp = int.Parse(sTemp, System.Globalization.NumberStyles.HexNumber);
                if (nTemp > 0)
                {
                    nOut_02_Addr = nTemp;
                    bDualOut = true;
                }
            }
            //-------

            // 섹션이 없으면(다르면) 모두 ""이므로 In_01_Addr만 체크
            if (sIn_01_Addr == "")
                return;

            CCylinder Cyld = new CCylinder();

            Cyld.sName = sSec;
            Cyld.sIn_01_Name = sIn_01_Name;
            Cyld.sIn_01_Addr = sIn_01_Addr;
            Cyld.nIn_01_Addr = nIn_01_Addr;
            Cyld.sIn_02_Name = sIn_02_Name;
            Cyld.sIn_02_Addr = sIn_02_Addr;
            Cyld.nIn_02_Addr = nIn_02_Addr;
            Cyld.bDualIn = bDualIn;
            Cyld.sOut_01_Name = sOut_01_Name;
            Cyld.sOut_01_Addr = sOut_01_Addr;
            Cyld.nOut_01_Addr = nOut_01_Addr;
            Cyld.sOut_02_Name = sOut_02_Name;
            Cyld.sOut_02_Addr = sOut_02_Addr;
            Cyld.nOut_02_Addr = nOut_02_Addr;
            Cyld.bDualOut = bDualOut;

            m_alCylinder.Add(Cyld);
        }




        public int SaveDefault()
        {
            string sPath = sCBM_CYL_PATH;
            if (!Directory.Exists(GV.PATH_EQ_CBM)) { Directory.CreateDirectory(GV.PATH_EQ_CBM); }

            StringBuilder mSB = new StringBuilder();

            mSB.AppendLine("[Onloader_MGZ_Pusher]");            // 1
            mSB.AppendLine("In_01_Name = Cylinder - Forward");
            mSB.AppendLine("In_01_Addr = " + string.Format("X{0}", ((int)eX.ONL_PusherForward).ToString("X4"))); //X00A5
            mSB.AppendLine("In_02_Name = Cylinder - Backward");
            mSB.AppendLine("In_02_Addr = " + string.Format("X{0}", ((int)eX.ONL_PusherBackward).ToString("X4"))); //X00A6
            mSB.AppendLine("Out_01_Name = Pusher Backward / Forward");
            mSB.AppendLine("Out_01_Addr = " + string.Format("Y{0}", ((int)eY.ONL_PusherForward).ToString("X4"))); //Y00A5
            mSB.AppendLine("Out_02_Name =");
            mSB.AppendLine("Out_02_Addr =");
            mSB.AppendLine("");

            mSB.AppendLine("[Onloader_MGZ_Clamp]");
            mSB.AppendLine("In_01_Name = Cylinder - Clamp On");
            mSB.AppendLine("In_01_Addr = " + string.Format("X{0}", ((int)eX.ONL_ClampOn).ToString("X4"))); //X00AA
            mSB.AppendLine("In_02_Name = Cylinder - Clamp Off");
            mSB.AppendLine("In_02_Addr = " + string.Format("X{0}", ((int)eX.ONL_ClampOff).ToString("X4"))); //X00AB
            mSB.AppendLine("Out_01_Name = MGZ Clamp On Off / On");
            mSB.AppendLine("Out_01_Addr = " + string.Format("Y{0}", ((int)eY.ONL_ClampOn).ToString("X4"))); //Y00AA
            mSB.AppendLine("Out_02_Name = MGZ Clamp Off Off / On");
            mSB.AppendLine("Out_02_Addr = " + string.Format("Y{0}", ((int)eY.ONL_ClampOff).ToString("X4"))); //Y00AB
            mSB.AppendLine("");

            mSB.AppendLine("[InRail_Gripper_Clamp_Rear]");
            mSB.AppendLine("In_01_Name = Cylinder - Gripper Clamp On-Rear(only U)");
            mSB.AppendLine("In_01_Addr = " + string.Format("X{0}", ((int)eX.INR_GripperClampOn_Rear).ToString("X4"))); //X000B
            mSB.AppendLine("In_02_Name = Cylinder - Gripper Clamp Off-Rear(only U)");
            mSB.AppendLine("In_02_Addr = " + string.Format("X{0}", ((int)eX.INR_GripperClampOff_Rear).ToString("X4"))); //X000C
            mSB.AppendLine("Out_01_Name = Clamp Off / On");
            mSB.AppendLine("Out_01_Addr = " + string.Format("Y{0}", ((int)eY.INR_GripperClampOn).ToString("X4"))); //Y0015
            mSB.AppendLine("Out_02_Name =");
            mSB.AppendLine("Out_02_Addr =");
            mSB.AppendLine("");

            mSB.AppendLine("[InRail_Align_Guide]");
            mSB.AppendLine("In_01_Name = Cylinder - Align Guide Forward");
            mSB.AppendLine("In_01_Addr = " + string.Format("X{0}", ((int)eX.INR_GuideForward).ToString("X4"))); //X0012
            mSB.AppendLine("In_02_Name = Cylinder - Align Guide Backward");
            mSB.AppendLine("In_02_Addr = " + string.Format("X{0}", ((int)eX.INR_GuideBackward).ToString("X4"))); //X0013
            mSB.AppendLine("Out_01_Name = Align Guide Backward / Forward(A Port, 4 pi)");
            mSB.AppendLine("Out_01_Addr = " + string.Format("Y{0}", ((int)eY.INR_GuideForward).ToString("X4"))); //Y0012
            mSB.AppendLine("Out_02_Name =");
            mSB.AppendLine("Out_02_Addr =");
            mSB.AppendLine("");

            mSB.AppendLine("[InRail_Gripper_Clamp]");
            mSB.AppendLine("In_01_Name = Cylinder - Gripper Clamp On");
            mSB.AppendLine("In_01_Addr = " + string.Format("X{0}", ((int)eX.INR_GripperClampOn).ToString("X4"))); //X0015
            mSB.AppendLine("In_02_Name = Cylinder - Gripper Clamp Off");
            mSB.AppendLine("In_02_Addr = " + string.Format("X{0}", ((int)eX.INR_GripperClampOff).ToString("X4"))); //X0016
            mSB.AppendLine("Out_01_Name = [INR]SOL - Clamp Off / On(A PORT, 4 pi)");
            mSB.AppendLine("Out_01_Addr = " + string.Format("Y{0}", ((int)eY.INR_GripperClampOn).ToString("X4"))); //Y0015
            mSB.AppendLine("Out_02_Name =");
            mSB.AppendLine("Out_02_Addr =");
            mSB.AppendLine("");

            mSB.AppendLine("[InRail_Lift]");
            mSB.AppendLine("In_01_Name = Cylinder - Lift Up");
            mSB.AppendLine("In_01_Addr = " + string.Format("X{0}", ((int)eX.INR_LiftUp).ToString("X4"))); //X0018
            mSB.AppendLine("In_02_Name = Cylinder - Lift Down");
            mSB.AppendLine("In_02_Addr = " + string.Format("X{0}", ((int)eX.INR_LiftDn).ToString("X4"))); //X0019
            mSB.AppendLine("Out_01_Name = [INR]SOL - Lift Down / Up(A PORT, 4pi)");
            mSB.AppendLine("Out_01_Addr = " + string.Format("Y{0}", ((int)eY.INR_LiftUp).ToString("X4"))); //Y0018
            mSB.AppendLine("Out_02_Name =");
            mSB.AppendLine("Out_02_Addr =");
            mSB.AppendLine("");

            mSB.AppendLine("[InRail_Probe]");
            mSB.AppendLine("In_01_Name = Cylinder - Probe Up");
            mSB.AppendLine("In_01_Addr = " + string.Format("X{0}", ((int)eX.INR_ProbeUp).ToString("X4"))); //X001E
            mSB.AppendLine("In_02_Name = Cylinder - Probe Down");
            mSB.AppendLine("In_02_Addr = " + string.Format("X{0}", ((int)eX.INR_ProbeDn).ToString("X4"))); //X001F
            mSB.AppendLine("Out_01_Name = Probe Up / Down(A Port)");
            mSB.AppendLine("Out_01_Addr = " + string.Format("Y{0}", ((int)eY.INR_ProbeDn).ToString("X4"))); //Y001E
            mSB.AppendLine("Out_02_Name =");
            mSB.AppendLine("Out_02_Addr =");
            mSB.AppendLine("");

            mSB.AppendLine("[Onloader_Picker_Rotate]");
            mSB.AppendLine("In_01_Name = Cylinder - Rotate 0' (Table)");
            mSB.AppendLine("In_01_Addr = " + string.Format("X{0}", ((int)eX.ONP_Rotate0).ToString("X4"))); //X0020
            mSB.AppendLine("In_02_Name = Cylinder - Rotate 90' (Table)");
            mSB.AppendLine("In_02_Addr = " + string.Format("X{0}", ((int)eX.ONP_Rotate90).ToString("X4"))); //X0021
            mSB.AppendLine("Out_01_Name = Rotate 0' Off / On");
            mSB.AppendLine("Out_01_Addr = " + string.Format("Y{0}", ((int)eY.ONP_Rotate0).ToString("X4"))); //Y0020
            mSB.AppendLine("Out_02_Name = Rotate 90' Off / On");
            mSB.AppendLine("Out_02_Addr = " + string.Format("Y{0}", ((int)eY.ONP_Rotate90).ToString("X4"))); //Y0021
            mSB.AppendLine("");

            mSB.AppendLine("[Left_Table_Cleaner]");
            mSB.AppendLine("In_01_Name = Cylinder - Cleaner Up / Down");
            mSB.AppendLine("In_01_Addr = " + string.Format("X{0}", ((int)eX.GRDL_TopClnDn).ToString("X4"))); //X0033
            mSB.AppendLine("In_02_Name =");
            mSB.AppendLine("In_02_Addr =");
            mSB.AppendLine("Out_01_Name = SOL - Cleaner Up / Down(A PORT, 4pi)");
            mSB.AppendLine("Out_01_Addr = " + string.Format("Y{0}", ((int)eY.GRDL_TopClnDn).ToString("X4"))); //Y0033
            mSB.AppendLine("Out_02_Name =");
            mSB.AppendLine("Out_02_Addr =");
            mSB.AppendLine("");

            mSB.AppendLine("[Left_Probe]");
            mSB.AppendLine("In_01_Name = Sensor - Probe AMP Out3");
            mSB.AppendLine("In_01_Addr = " + string.Format("X{0}", ((int)eX.GRDL_ProbeAMP).ToString("X4"))); //X003B
            mSB.AppendLine("In_02_Name =");
            mSB.AppendLine("In_02_Addr =");
            mSB.AppendLine("Out_01_Name = [GRL]SOL - Probe Up / Down");
            mSB.AppendLine("Out_01_Addr = " + string.Format("Y{0}", ((int)eY.GRDL_ProbeDn).ToString("X4"))); //Y0038
            mSB.AppendLine("Out_02_Name = ");
            mSB.AppendLine("Out_02_Addr =");
            mSB.AppendLine("");

            mSB.AppendLine("[Right_Table_Cleaner]");
            mSB.AppendLine("In_01_Name = Cylinder - Cleaner Up / Down");
            mSB.AppendLine("In_01_Addr = " + string.Format("X{0}", ((int)eX.GRDR_TopClnDn).ToString("X4"))); //X004B
            mSB.AppendLine("In_02_Name = ");
            mSB.AppendLine("In_02_Addr = ");
            mSB.AppendLine("Out_01_Name = [GRR]SOL - Cleaner Up / Down(A PORT, 4pi)");
            mSB.AppendLine("Out_01_Addr = " + string.Format("Y{0}", ((int)eY.GRDR_TopClnDn).ToString("X4"))); //Y004B
            mSB.AppendLine("Out_02_Name =");
            mSB.AppendLine("Out_02_Addr =");
            mSB.AppendLine("");

            mSB.AppendLine("[Right_Probe]");
            mSB.AppendLine("In_01_Name = Sensor - Probe AMP Out3");
            mSB.AppendLine("In_01_Addr = " + string.Format("X{0}", ((int)eX.GRDR_ProbeAMP).ToString("X4"))); //X0053
            mSB.AppendLine("In_02_Name = ");
            mSB.AppendLine("In_02_Addr = ");
            mSB.AppendLine("Out_01_Name = [GRR]SOL - Probe Up / Down");
            mSB.AppendLine("Out_01_Addr = " + string.Format("Y{0}", ((int)eY.GRDR_ProbeDn).ToString("X4"))); //Y0050
            mSB.AppendLine("Out_02_Name =");
            mSB.AppendLine("Out_02_Addr =");
            mSB.AppendLine("");

            mSB.AppendLine("[Offloader_Picker_Rotate]");
            mSB.AppendLine("In_01_Name = Cylinder - Rotate 0' (Table)");
            mSB.AppendLine("In_01_Addr = " + string.Format("X{0}", ((int)eX.OFFP_Rotate0).ToString("X4"))); //X0060
            mSB.AppendLine("In_02_Name = Cylinder - Rotate 90' (Table)");
            mSB.AppendLine("In_02_Addr = " + string.Format("X{0}", ((int)eX.OFFP_Rotate90).ToString("X4"))); //X0061
            mSB.AppendLine("Out_01_Name = [OFP]SOL - Rotate 0' Off / On    (A PORT, 6pi)");
            mSB.AppendLine("Out_01_Addr = " + string.Format("Y{0}", ((int)eY.OFFP_Rotate0).ToString("X4"))); //Y0060
            mSB.AppendLine("Out_02_Name = [OFP]SOL - Rotate 90' Off / On    (B PORT, 6pi)");
            mSB.AppendLine("Out_02_Addr = " + string.Format("Y{0}", ((int)eY.OFFP_Rotate90).ToString("X4"))); //Y0061
            mSB.AppendLine("");

            mSB.AppendLine("[Offloader_MGZ1_UpDown]");
            mSB.AppendLine("In_01_Name = Cylinder - Up");
            mSB.AppendLine("In_01_Addr = " + string.Format("X{0}", ((int)eX.OFFL_TopMGZUp).ToString("X4"))); //X00C2
            mSB.AppendLine("In_02_Name = Cylinder - Down");
            mSB.AppendLine("In_02_Addr = " + string.Format("X{0}", ((int)eX.OFFL_TopMGZDn).ToString("X4"))); //X00C3
            mSB.AppendLine("Out_01_Name = [TOP]SOL - Up Off / On(A PORT, 4pi)");
            mSB.AppendLine("Out_01_Addr = " + string.Format("Y{0}", ((int)eY.OFFL_TopMGZUp).ToString("X4"))); //Y00C2
            mSB.AppendLine("Out_02_Name = [TOP]SOL - Down Off / On(B PORT, 4pi)");
            mSB.AppendLine("Out_02_Addr = " + string.Format("Y{0}", ((int)eY.OFFL_TopMGZDn).ToString("X4"))); //Y00C3
            mSB.AppendLine("");

            mSB.AppendLine("[Offloader_MGZ1_Clamp]");
            mSB.AppendLine("In_01_Name = Cylinder - Clamp On");
            mSB.AppendLine("In_01_Addr = " + string.Format("X{0}", ((int)eX.OFFL_TopClampOn).ToString("X4"))); //X00C4
            mSB.AppendLine("In_02_Name = Cylinder - Clamp Off");
            mSB.AppendLine("In_02_Addr = " + string.Format("X{0}", ((int)eX.OFFL_TopClampOff).ToString("X4"))); //X00C5
            mSB.AppendLine("Out_01_Name = [TOP]SOL - MGZ Clamp On Off / On(A PORT, 4pi)");
            mSB.AppendLine("Out_01_Addr = " + string.Format("Y{0}", ((int)eY.OFFL_TopClampOn).ToString("X4"))); //Y00C4
            mSB.AppendLine("Out_02_Name = [TOP]SOL - MGZ Clamp Off Off / On(B PORT, 4pi)");
            mSB.AppendLine("Out_02_Addr = " + string.Format("Y{0}", ((int)eY.OFFL_TopClampOff).ToString("X4"))); //Y00C5
            mSB.AppendLine("");

            mSB.AppendLine("[Offloader_MGZ2_UpDown]");
            mSB.AppendLine("In_01_Name = Cylinder - Up");
            mSB.AppendLine("In_01_Addr = " + string.Format("X{0}", ((int)eX.OFFL_BtmMGZUp).ToString("X4"))); //X00CA
            mSB.AppendLine("In_02_Name = Cylinder - Down");
            mSB.AppendLine("In_02_Addr = " + string.Format("X{0}", ((int)eX.OFFL_BtmMGZDn).ToString("X4"))); //X00CB
            mSB.AppendLine("Out_01_Name = [BTM]SOL - Up Off / On(A PORT, 4pi)");
            mSB.AppendLine("Out_01_Addr = " + string.Format("Y{0}", ((int)eY.OFFL_BtmMGZUp).ToString("X4"))); //Y00CA
            mSB.AppendLine("Out_02_Name = [BTM]SOL - Down Off / On(B PORT, 4pi)");
            mSB.AppendLine("Out_02_Addr = " + string.Format("Y{0}", ((int)eY.OFFL_BtmMGZDn).ToString("X4"))); //Y00CB
            mSB.AppendLine("");

            mSB.AppendLine("[Offloader_MGZ2_Clamp]");
            mSB.AppendLine("In_01_Name = Cylinder - Clamp On");
            mSB.AppendLine("In_01_Addr = " + string.Format("X{0}", ((int)eX.OFFL_BtmClampOn).ToString("X4"))); //X00CC
            mSB.AppendLine("In_02_Name = Cylinder - Clamp Off");
            mSB.AppendLine("In_02_Addr = " + string.Format("X{0}", ((int)eX.OFFL_BtmClampOff).ToString("X4"))); //X00CD
            mSB.AppendLine("Out_01_Name = [BTM]SOL - MGZ Clamp On Off / On(A PORT, 4pi)");
            mSB.AppendLine("Out_01_Addr = " + string.Format("Y{0}", ((int)eY.OFFL_BtmClampOn).ToString("X4"))); //Y00CC
            mSB.AppendLine("Out_02_Name = [BTM]SOL - MGZ Clamp Off Off / On(B PORT, 4pi)");
            mSB.AppendLine("Out_02_Addr = " + string.Format("Y{0}", ((int)eY.OFFL_BtmClampOff).ToString("X4"))); //Y00CD

            //200213 ksg :
            CLog.Check_File_Access(sPath, mSB.ToString(), false);
            return 0;
        }




    }
}



