using System;
using System.Runtime.InteropServices;

namespace DeXign.Interop
{
    public static class UnsafeNativeMethods
    {
        [DllImport(ExternDll.User32)]
        public static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        [DllImport(ExternDll.User32)]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, ref NativeMethods.MONITORINFO lpmi);
    }
}
