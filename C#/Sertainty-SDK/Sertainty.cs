using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SertaintySDK
{
    static class Sertainty
    {
        public static bool HasError(IntPtr handle)
        {
            return SertaintyCore.uxpsys_hasError(handle);
        }

        public static IntPtr GetErrorMessage(IntPtr handle)
        {
            return SertaintyCore.uxpsys_getErrorMessage(handle);
        }

        public static void SetLogFile(string prefix, string version)
        {
            SertaintyCore.uxpsys_setLogFile(prefix, version);
        }

        public static void FileReadAll(CallStatus status, string filespec, ByteArray outbuf)
        {
            SertaintyCore.uxpsys_fileReadAll(status, filespec, outbuf);
        }

        public static long InitializeLibrary(ByteArray buffer, long argc, string[] argv, string licenseFile, string appKey, string prefix, string version)
        {
            return SertaintyCore.uxpsys_initLibrary(buffer, argc, argv, licenseFile, appKey, prefix, version);
        }

        public class CallStatus
        {
            public CallStatus()
            {
                _handle = SertaintyCore.uxpsys_newCallStatusHandle();
            }

            public bool HasError
            {
                get
                {
                    return Sertainty.HasError(_handle);
                }
            }

            public string ErrorMessage
            {
                get
                {
                    ByteArray errorBa = new ByteArray(Sertainty.GetErrorMessage(_handle));
                    return errorBa.ToString();
                }
            }

            public static implicit operator IntPtr(CallStatus cs)
            {
                return cs._handle;
            }

            ~CallStatus()
            {
                SertaintyCore.uxpsys_freeCallStatusHandle(_handle);
            }

            private IntPtr _handle;
        }
    }

    class ByteArray
    {
        public ByteArray()
        {
            _handle = SertaintyCore.uxpba_newHandle();
        }

        public ByteArray(IntPtr srcHandle)
        {
            _handle = srcHandle;
        }

        public long Size
        {
            get
            {
                return SertaintyCore.uxpba_getSize(_handle);
            }
        }

        public void Clear()
        {
            SertaintyCore.uxpba_clearData(_handle);
        }

        public byte[] GetBytes(int len)
        {
            IntPtr ptr = SertaintyCore.uxpba_getData(_handle);
            if (ptr == IntPtr.Zero)
                return new byte[0];
            if (len == 0)
                return new byte[0];
            byte[] array = new byte[len];
            Marshal.Copy(ptr, array, 0, len);
            return array;
        }

        public byte[] GetAllBytes()
        {
            return GetBytes((int)this.Size);
        }

        public void SetData(string data)
        {
            SertaintyCore.uxpba_setData(_handle, data, data.Length);
        }

        public override string ToString()
        {
            IntPtr ptr = SertaintyCore.uxpba_getData(_handle);
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

        public static implicit operator IntPtr(ByteArray byteArray)
        {
            return byteArray._handle;
        }

        ~ByteArray()
        {
            SertaintyCore.uxpba_freeHandle(_handle);
        }

        private IntPtr _handle;
    }

    class UxpFile
    {
        public UxpFile()
        {
            _handle = SertaintyCore.uxpfile_newHandle();
        }

        public void OpenNewFile(string data, string governance, long govtype, long mods, long flags)
        {
            SertaintyCore.uxpfile_openNewFile(_handle, data, governance, govtype, mods, flags);
        }

        public void AddVirtualFile(string virName, string filespec, long pageSize, long cacheSize, long mods)
        {
            SertaintyCore.uxpfile_addVirtualFromFile(_handle, virName, filespec, pageSize, cacheSize, mods);
        }

        public VirtualFile OpenVirtualFile(string fileSpec, Mode mode)
        {
            return new VirtualFile(this, fileSpec, mode);
        }

        public bool CompareExternalFle(string vf_name, string ext_file_name)
        {
            return SertaintyCore.uxpfile_compareExternalFile(_handle, vf_name, ext_file_name);
        }

        public void Close()
        {
            SertaintyCore.uxpfile_close(_handle);
        }

        public void Open(string source, Mode mode)
        {
            SertaintyCore.uxpfile_openFile(_handle, source, mode);
        }

        public AuthorizationStatus Authenticate()
        {
            return SertaintyCore.uxpfile_authenticate(_handle);
        }

        public int ChallengeCount
        {
            get
            {
                return SertaintyCore.uxpfile_getChallengeCount(_handle);
            }
        }

        public Challenge GetChallenge(int ch_idx)
        {
            IntPtr ch_handle = SertaintyCore.uxpfile_getChallenge(_handle, ch_idx);
            return Challenge.FromHandle(ch_handle);
        }

        public void AddResponse(Challenge ch)
        {
            SertaintyCore.uxpfile_addResponse(_handle, ch);
        }

        public bool HasError
        {
            get
            {
                return Sertainty.HasError(_handle);
            }
        }

        public string ErrorMessage
        {
            get
            {
                ByteArray errorBa = new ByteArray(Sertainty.GetErrorMessage(_handle));
                return errorBa.ToString();
            }
        }


        public static implicit operator IntPtr(UxpFile uxp)
        {
            return uxp._handle;
        }

        ~UxpFile()
        {
            SertaintyCore.uxpfile_freeHandle(_handle);
        }

        private IntPtr _handle;
    }

    class Challenge
    {
        private Challenge(IntPtr ch_handle)
        {
            _handle = ch_handle;
        }

        public static Challenge FromHandle(IntPtr ch_handle)
        {
            return new Challenge(ch_handle);
        }

        public void StartTimer()
        {
            SertaintyCore.uxpch_startTimer(_handle);
        }

        public void EndTimer()
        {
            SertaintyCore.uxpch_endTimer(_handle);
        }

        public ByteArray GetPrompt()
        {
            ByteArray prompt = new ByteArray();
            SertaintyCore.uxpch_getPrompt(_handle, prompt);
            return prompt;
        }

        public void SetValue(string value)
        {
            SertaintyCore.uxpch_setValueString(_handle, value);
        }

        public static implicit operator IntPtr(Challenge ch)
        {
            return ch._handle;
        }

        ~Challenge()
        {
            SertaintyCore.uxpch_freeHandle(_handle);
        }

        private IntPtr _handle;
    }

    class VirtualFile
    {
        public VirtualFile(UxpFile uxp, string fileSpec, Mode mode)
        {
            _uxp = uxp;
            _handle = SertaintyCore.uxpfile_openVirtualFile(uxp, fileSpec, mode);
        }

        public long Read(ByteArray buffer, long max_size)
        {
            return SertaintyCore.uxpfile_readVirtualFile(_uxp, this, buffer, max_size);
        }

        public void Close()
        {
            SertaintyCore.uxpfile_closeVirtualFile(_uxp, this);
        }

        public static implicit operator IntPtr(VirtualFile vf)
        {
            return vf._handle;
        }

        ~VirtualFile()
        {

        }

        private IntPtr _handle;
        private UxpFile _uxp;
    }

    static class Id
    {
        public static void PublishIdToFile(Sertainty.CallStatus status, string id, string doc, long mods)
        {
            SertaintyCore.uxpid_publishToFile(status, id, doc, mods);
        }
    }
}
