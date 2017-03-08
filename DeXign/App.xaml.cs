using System;
using System.Windows;

using DeXign.Interop;
using DeXign.Database;
using System.Windows.Navigation;

namespace DeXign
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Local Database
            RecentDB.Open();

            base.OnStartup(e);
        }

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