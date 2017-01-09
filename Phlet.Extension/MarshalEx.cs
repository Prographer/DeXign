using System;
using System.Runtime.InteropServices;

namespace Phlet.Extension
{
    public static class MarshalEx
    {
        public static void ToPtr(this object obj, IntPtr lParam, bool fDeleteOld)
        {
            Marshal.StructureToPtr(obj, lParam, fDeleteOld);
        }

        public static T PtrToStructure<T>(this IntPtr ptr)
        {
            return (T)Marshal.PtrToStructure(ptr, typeof(T));
        }

        public static int SizeOf<T>()
        {
            return Marshal.SizeOf(typeof(T));
        }
    }
}