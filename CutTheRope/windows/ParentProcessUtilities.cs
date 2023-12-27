using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CutTheRope.windows
{
    public struct ParentProcessUtilities
    {
        internal IntPtr Reserved1;

        internal IntPtr PebBaseAddress;

        internal IntPtr Reserved2_0;

        internal IntPtr Reserved2_1;

        internal IntPtr UniqueProcessId;

        internal IntPtr InheritedFromUniqueProcessId;

        [DllImport("ntdll.dll")]
        private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref ParentProcessUtilities processInformation, int processInformationLength, out int returnLength);

        public static Process GetParentProcess()
        {
            return GetParentProcess(Process.GetCurrentProcess().Handle);
        }

        public static Process GetParentProcess(int id)
        {
            Process processById = Process.GetProcessById(id);
            return GetParentProcess(processById.Handle);
        }

        public static Process GetParentProcess(IntPtr handle)
        {
            ParentProcessUtilities processInformation = default(ParentProcessUtilities);
            int returnLength;
            int num = NtQueryInformationProcess(handle, 0, ref processInformation, Marshal.SizeOf((object)processInformation), out returnLength);
            if (num != 0)
            {
                throw new Win32Exception(num);
            }
            try
            {
                return Process.GetProcessById(processInformation.InheritedFromUniqueProcessId.ToInt32());
            }
            catch (ArgumentException)
            {
                return null;
            }
        }
    }
}
