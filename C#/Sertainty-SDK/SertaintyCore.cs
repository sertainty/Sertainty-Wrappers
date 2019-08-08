using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SertaintySDK
{
    struct SertaintyCore2
    {
        public const string dll = "SertaintyCore2.dll";
    }

    enum Modifiers
    {
        Replace = 0x00001,
        Merge = 0x00002,
        Recurse = 0x00004,
        Compress = 0x00008,
        IncHidden = 0x00010,
        Shred = 0x00020,
        IncRealities = 0x00040,
        NoOptimize = 0x00080,
        DeleteSrc = 0x00100,
        MinSize = 0x00200,
        Reclaim = 0x00400,
        RecurseVirtual = 0x00800,
        Create = 0x01000,
        Load = 0x02000,
        ReadWrite = 0x04000,
        Protect = 0x08000,
        IncScript = 0x10000,
        Formatted = 0x20000,
        TokenReplace = 0x40000
    }

    enum Mode
    {
        ReadOnly = 1,
        WriteOnly = 2,
        ReadWrite = 3
    }

    enum AuthorizationStatus
    {
        NotAuthorized = 0x00001,  /*!< Access to UXP is not authorized */
        InvalidUsername = 0x00002,  /*!< Invalid username was provided */
        FileMoved = 0x00004,  /*!< File has been moved */
        ScheduleViolation = 0x00008,  /*!< Schedule violation */
        Authorized = 0x00010,  /*!< User has been authorized */
        ConfigNotFound = 0x00020,  /*!< Address configuration not reconized */
        LocationNotFound = 0x00040,  /*!< Address location not recognized */
        DeviceNotFound = 0x00080,  /*!< Address device not recognized */
        DeviceLocationFound = 0x00100,  /*!< Address location and device pair not recognized */
        Challenged = 0x00200,  /*!< User is challenged */
        Panic = 0x00400,  /*!< User has indicated a panic situation */
        GlobalSchedViolation = 0x00800,  /*!< Global schedule violation */
        Threat = 0x01000,  /*!< Unauthoried access threat detected */
        Canceled = 0x02000,  /*!< User canceled access attempt */
        LdapViolation = 0x04000,  /*!< LDAP approval violation */
        ConfigFound = 0x08000,  /*!< Address configuration was recognized */
        NoSingleSignOn = 0x10000  /*!< Single sign-on attempt failed */
    }

    static class SertaintyCore
    {
        public static string ReadString(this IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                return "";
            int len = 0;
            while (Marshal.ReadByte(ptr, len) != 0)
                len++;
            if (len == 0)
                return "";
            byte[] array = new byte[len];

            Marshal.Copy(ptr, array, 0, len);

            return Encoding.UTF8.GetString(array);
        }

        public static byte[] ReadBytes(this IntPtr ptr, int len)
        {
            if (ptr == IntPtr.Zero)
                return new byte[0];
            if (len == 0)
                return new byte[0];
            byte[] array = new byte[len];

            Marshal.Copy(ptr, array, 0, len);

            return array;
        }

        #region uxpsys
        [DllImport(SertaintyCore2.dll)]
        public static extern void uxpsys_setLogFile(string prefix, string version);

        [DllImport(SertaintyCore2.dll, CharSet = CharSet.Ansi)]
        public static extern long uxpsys_installLicense(IntPtr error, string licenseFile);

        [DllImport(SertaintyCore2.dll)]
        public static extern bool uxpsys_hasError(IntPtr handle);

        [DllImport(SertaintyCore2.dll)]
        public static extern IntPtr uxpsys_getErrorMessage(IntPtr handle);

        [DllImport(SertaintyCore2.dll, CharSet = CharSet.Ansi)]
        public static extern long uxpsys_initLibrary(IntPtr error, long argc, string[] argv, string licenseFile, string appKey, string prefix, string version);

        [DllImport(SertaintyCore2.dll)]
        public static extern IntPtr uxpsys_newCallStatusHandle();

        [DllImport(SertaintyCore2.dll)]
        public static extern void uxpsys_freeCallStatusHandle(IntPtr cs_handle);

        [DllImport(SertaintyCore2.dll, CharSet = CharSet.Ansi)]
        public static extern void uxpsys_fileReadAll(IntPtr status, string filespec, IntPtr outbuf);

        #endregion

        #region uxpba
        [DllImport(SertaintyCore2.dll)]
        public static extern IntPtr uxpba_newHandle();

        [DllImport(SertaintyCore2.dll)]
        //[return: MarshalAs(UnmanagedType.LPStr)]
        public static extern IntPtr uxpba_getData(IntPtr handle);

        [DllImport(SertaintyCore2.dll)]
        public static extern long uxpba_getSize(IntPtr handle);

        [DllImport(SertaintyCore2.dll, CharSet = CharSet.Ansi)]
        public static extern void uxpba_setData(IntPtr handle, [MarshalAs(UnmanagedType.LPStr)]string data, long len);

        [DllImport(SertaintyCore2.dll)]
        public static extern void uxpba_clearData(IntPtr handle);

        [DllImport(SertaintyCore2.dll)]
        public static extern void uxpba_freeHandle(IntPtr handle);
        #endregion

        #region uxpfile

        [DllImport(SertaintyCore2.dll)]
        public static extern IntPtr uxpfile_newHandle();

        [DllImport(SertaintyCore2.dll)]
        public static extern void uxpfile_openNewFile(IntPtr handle, string data, string governance, long govtype, long mods, long flags);

        [DllImport(SertaintyCore2.dll)]
        public static extern void uxpfile_addVirtualFromFile(IntPtr uxp_handle, string virName, string filespec, long pageSize, long cacheSize, long mods);

        [DllImport(SertaintyCore2.dll)]
        public static extern IntPtr uxpfile_openVirtualFile(IntPtr uxp_handle, string fileSpec, Mode mode);

        [DllImport(SertaintyCore2.dll)]
        public static extern long uxpfile_readVirtualFile(IntPtr uxp_handle, IntPtr vf_handle, IntPtr buffer, long mx);

        [DllImport(SertaintyCore2.dll)]
        public static extern void uxpfile_closeVirtualFile(IntPtr uxp_handle, IntPtr vf_handle);

        [DllImport(SertaintyCore2.dll)]
        public static extern bool uxpfile_compareExternalFile(IntPtr uxp_handle, string vf_name, string ext_file_name);

        [DllImport(SertaintyCore2.dll)]
        public static extern void uxpfile_close(IntPtr uxp_handle);

        [DllImport(SertaintyCore2.dll)]
        public static extern void uxpfile_openFile(IntPtr uxp_handle, string source, Mode mode);

        [DllImport(SertaintyCore2.dll)]
        public static extern AuthorizationStatus uxpfile_authenticate(IntPtr uxp_handle);

        [DllImport(SertaintyCore2.dll)]
        public static extern int uxpfile_getChallengeCount(IntPtr uxp_handle);

        [DllImport(SertaintyCore2.dll)]
        public static extern IntPtr uxpfile_getChallenge(IntPtr uxp_handle, int idx);

        [DllImport(SertaintyCore2.dll)]
        public static extern void uxpfile_addResponse(IntPtr uxp_handle, IntPtr ch_handle);

        [DllImport(SertaintyCore2.dll)]
        public static extern void uxpfile_freeHandle(IntPtr uxp_handle);


        #endregion

        #region uxpid
        [DllImport(SertaintyCore2.dll)]
        public static extern void uxpid_publishToFile(IntPtr status, string id, string doc, long mods);

        #endregion

        #region uxpch
        [DllImport(SertaintyCore2.dll)]
        public static extern void uxpch_freeHandle(IntPtr ch_handle);

        [DllImport(SertaintyCore2.dll)]
        public static extern void uxpch_getPrompt(IntPtr ch_handle, IntPtr ba_handle);

        [DllImport(SertaintyCore2.dll)]
        public static extern void uxpch_setValueString(IntPtr ch_handle, string value);

        [DllImport(SertaintyCore2.dll)]
        public static extern void uxpch_startTimer(IntPtr ch_handle);

        [DllImport(SertaintyCore2.dll)]
        public static extern void uxpch_endTimer(IntPtr ch_handle);


        #endregion

    }
}
