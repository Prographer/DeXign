using Microsoft.Windows.Shell;

using System;
using System.Windows;
using System.Windows.Interop;

using Phlet.Interop;
using Phlet.Extension;

using Monitor = Phlet.Interop.NativeMethods.Monitor;
using MINMAXINFO = Phlet.Interop.NativeMethods.MINMAXINFO;
using MONITORINFO = Phlet.Interop.NativeMethods.MONITORINFO;

namespace Phlet.Controls
{
    public class ChromeWindow : Window
    {
        #region [ Dependency Property ]
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyHelper.Register(new PropertyMetadata(false));

        public static readonly DependencyProperty CaptionHeightProperty =
            DependencyHelper.Register(new PropertyMetadata(30));

        private static readonly DependencyPropertyKey HandlePropertyKey =
            DependencyHelper.RegisterReadonly();

        public static readonly DependencyProperty HandleProperty =
            HandlePropertyKey.DependencyProperty;
        #endregion

        #region [ Property ]
        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        public int CaptionHeight
        {
            get { return (int)GetValue(CaptionHeightProperty); }
            set { SetValue(CaptionHeightProperty, value); }
        }

        public IntPtr Handle
        {
            get { return (IntPtr)GetValue(HandlePropertyKey.DependencyProperty); }
        }
        #endregion

        #region [ Constructor ]
        public ChromeWindow()
        {
            this.Style = FindResource("ChromeWindowStyle") as Style;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var helper = new WindowInteropHelper(this);
            var hwndSource = HwndSource.FromHwnd(helper.Handle);

            SetValue(HandlePropertyKey, helper.Handle);
            hwndSource.AddHook(WndProc);
        }
        
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);

            if (this.Content != null)
                WindowChrome.SetIsHitTestVisibleInChrome(this.Content as IInputElement, true);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // TODO:s
        }
        #endregion

        #region [ Window Handling ]
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            var nMsg = (NativeMethods.WM)msg;

            switch (nMsg)
            {
                case NativeMethods.WM.GETMINMAXINFO:
                    WmGetMinmaxInfo(hwnd, lParam);
                    break;
            }

            return IntPtr.Zero;
        }

        private void WmGetMinmaxInfo(IntPtr hwnd, IntPtr lParam)
        {
            var mmi = MarshalEx.PtrToStructure<MINMAXINFO>(lParam);
            IntPtr monitor = UnsafeNativeMethods.MonitorFromWindow(
                hwnd, (uint)Monitor.DEFAULTTONEAREST);

            if (monitor != IntPtr.Zero)
            {
                var mInfo = new MONITORINFO();
                mInfo.cbSize = MarshalEx.SizeOf<MONITORINFO>();
                UnsafeNativeMethods.GetMonitorInfo(monitor, ref mInfo);

                mmi.ptMaxPosition.X = Math.Abs(mInfo.rcWork.left - mInfo.rcMonitor.left);
                mmi.ptMaxPosition.Y = Math.Abs(mInfo.rcWork.top - mInfo.rcMonitor.top);
                mmi.ptMaxSize.X = Math.Abs(mInfo.rcWork.right - mInfo.rcWork.left);
                mmi.ptMaxSize.Y = Math.Abs(mInfo.rcWork.bottom - mInfo.rcWork.top);
            }

            mmi.ToPtr(lParam, true);
        }
        #endregion
    }
}
