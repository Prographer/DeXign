using System;
using System.Runtime.InteropServices;

namespace DeXign.Interop
{
    internal static class UnsafeNativeMethods
    {
        #region [ User32 ]
        [DllImport(ExternDll.User32)]
        public static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        [DllImport(ExternDll.User32)]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, ref NativeMethods.MONITORINFO lpmi);

        [DllImport(ExternDll.User32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetCursorPos(out NativeMethods.Point lpPoint);
        #endregion

        #region [ Kernel32 ]
        [DllImport(ExternDll.Kernel32)]
        public static extern bool SetProcessWorkingSetSize(IntPtr hProcess, IntPtr dwMinimumWorkingSetSize, IntPtr dwMaximumWorkingSetSize);

        [DllImport(ExternDll.Kernel32)]
        public static extern IntPtr GetCurrentProcess();
        #endregion
    }
}
