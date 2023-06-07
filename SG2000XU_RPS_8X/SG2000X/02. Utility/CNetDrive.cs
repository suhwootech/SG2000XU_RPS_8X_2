using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SG2000X
{
    public enum ENetError
    {
        NO_ERROR = 0,
        ERROR_NO_NET_OR_BAD_SERVER = 53,
        ERROR_BAD_USER_OR_PASSWORD = 1326,
        ERROR_ACCESS_DENIED = 5,
        ERROR_ALREADY_ASSIGNED = 85,
        ERROR_BAD_DEV_TYPE = 66,
        ERROR_BAD_DEVICE = 1200,
        ERROR_BAD_NET_NAME = 67,
        ERROR_BAD_PROFILE = 1206,
        ERROR_BAD_PROVIDER = 1204,
        ERROR_BUSY = 170,
        ERROR_CANCELLED = 1223,
        ERROR_CANNOT_OPEN_PROFILE = 1205,
        ERROR_DEVICE_ALREADY_REMEMBERED = 1202,
        ERROR_EXTENDED_ERROR = 1208,
        ERROR_INVALID_PASSWORD = 86,
        ERROR_NO_NET_OR_BAD_PATH = 1203,
        ERROR_INVALID_ADDRESS = 487,
        ERROR_NETWORK_BUSY = 54,
        ERROR_UNEXP_NET_ERR = 59,
        ERROR_INVALID_PARAMETER = 87,
        ERROR_MULTIPLE_CONNECTION = 1219
    }


        
    public static class CNetDrive
    {
        private const int RESOURCE_CONNECTED = 0x00000001;
        private const int RESOURCE_GLOBALNET = 0x00000002;
        private const int RESOURCE_REMEMBERED = 0x00000003;
        private const int RESOURCETYPE_ANY = 0x00000000;
        private const int RESOURCETYPE_DISK = 0x00000001;
        private const int RESOURCETYPE_PRINT = 0x00000002;
        private const int RESOURCEDISPLAYTYPE_GENERIC = 0x00000000;
        private const int RESOURCEDISPLAYTYPE_DOMAIN = 0x00000001;
        private const int RESOURCEDISPLAYTYPE_SERVER = 0x00000002;
        private const int RESOURCEDISPLAYTYPE_SHARE = 0x00000003;
        private const int RESOURCEDISPLAYTYPE_FILE = 0x00000004;
        private const int RESOURCEDISPLAYTYPE_GROUP = 0x00000005;
        private const int RESOURCEUSAGE_CONNECTABLE = 0x00000001;
        private const int RESOURCEUSAGE_CONTAINER = 0x00000002;

        private const int CONNECT_UPDATE_PROFILE = 0x00000001;
        private const int CONNECT_UPDATE_RECENT = 0x00000002;
        private const int CONNECT_TEMPORARY = 0x00000004;
        private const int CONNECT_INTERACTIVE = 0x00000008;
        private const int CONNECT_PROMPT = 0x00000010;
        private const int CONNECT_REDIRECT = 0x00000080;
        private const int CONNECT_CURRENT_MEDIA = 0x00000200;
        private const int CONNECT_COMMANDLINE = 0x00000800;
        private const int CONNECT_CMD_SAVECRED = 0x00001000;
        private const int CONNECT_CRED_RESET = 0x00002000;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct NETRESOURCE
        {
            public uint dwScope;
            public uint dwType;
            public uint dwDisplayType;
            public uint dwUsage;
            public string lpLocalName;
            public string lpRemoteName;
            public string lpComment;
            public string lpProvider;
        }

        private static NETRESOURCE m_tNet = new NETRESOURCE();

        // Net Drive 연결
        [DllImport("mpr.dll", CharSet = CharSet.Auto)]
        private static extern int WNetUseConnection(
            IntPtr hwndOwner,
            [MarshalAs(UnmanagedType.Struct)] ref NETRESOURCE lpNetResource,
            string lpPassword,
            string lpUserID,
            uint dwFlags,
            StringBuilder lpAccessName,
            ref int lpBufferSize,
            out uint lpResult);

        // Net Drive 연결
        public static int Connect(string sPath, string sId, string sPw)
        {
            int iCapacity = 64;
            uint nResultFlags = 0;
            uint nFlags = 0;

            try
            {
                int iRet = 0;
                StringBuilder mSB = new StringBuilder(iCapacity);
                m_tNet.dwType = RESOURCETYPE_DISK;
                m_tNet.lpLocalName = null;                 // 로컬 드라이브명(null 이면 자동)
                m_tNet.lpRemoteName = sPath;
                m_tNet.lpProvider = null;

                if ((string.IsNullOrEmpty(sId)) && (string.IsNullOrEmpty(sPw)))
                { nFlags = CONNECT_INTERACTIVE | CONNECT_PROMPT; }

                if ((!string.IsNullOrEmpty(sPath)))
                {
                    iRet = WNetUseConnection(IntPtr.Zero, ref m_tNet, sPw, sId, nFlags,
                                        mSB, ref iCapacity, out nResultFlags);
                    if (iRet == (int)ENetError.ERROR_MULTIPLE_CONNECTION)
                    { Disconnect(); }

                    return iRet;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        // Net Drive 해제
        [DllImport("mpr.dll", EntryPoint = "WNetCancelConnection2", CharSet = CharSet.Auto)]
        private static extern int WNetCancelConnection2(string lpName, int dwFlags, bool fForce);

        // Net Drive 해제
        public static int Disconnect()
        {
            int iRet;

            try
            {
                iRet = WNetCancelConnection2(m_tNet.lpRemoteName, CONNECT_UPDATE_PROFILE, false);
            }
            catch (Exception ex)
            {
                iRet = 0;
            }

            return iRet;
        }
    }
}
