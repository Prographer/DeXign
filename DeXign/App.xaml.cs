using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

using DeXign.Interop;

namespace DeXign
{
    public partial class App : Application
    {
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            // 메모리 사용량 최소화
            UnsafeNativeMethods.SetProcessWorkingSetSize(
                UnsafeNativeMethods.GetCurrentProcess(), 
                new IntPtr(-1), new IntPtr(-1));
        }
    }
}