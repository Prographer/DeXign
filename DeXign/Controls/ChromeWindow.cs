using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Controls;
using Microsoft.Windows.Shell;

using DeXign.Interop;
using DeXign.Extension;

using WinSystemCommands = System.Windows.SystemCommands;

using Monitor = DeXign.Interop.NativeMethods.Monitor;
using MINMAXINFO = DeXign.Interop.NativeMethods.MINMAXINFO;
using MONITORINFO = DeXign.Interop.NativeMethods.MONITORINFO;

namespace DeXign.Controls
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

        public static readonly DependencyProperty MenuProperty =
            DependencyHelper.Register();
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

        public Menu Menu
        {
            get { return (Menu)GetValue(MenuProperty); }
            set { SetValue(MenuProperty, value); }
        }

        public IntPtr Handle
        {
            get { return (IntPtr)GetValue(HandlePropertyKey.DependencyProperty); }
        }
        #endregion

        #region [ Constructor ]
        public ChromeWindow()
        {
            InitializeWindow();
            InitializeCommands();
        }

        private void InitializeWindow()
        {
            var chrome = new WindowChrome()
            {
                ResizeBorderThickness = SystemParameters.WindowResizeBorderThickness,
                CaptionHeight = this.CaptionHeight,
                CornerRadius = new CornerRadius(0),
                GlassFrameThickness = new Thickness(1),
                UseAeroCaptionButtons = false,
                UseNoneWindowStyle = false
            };

            WindowChrome.SetWindowChrome(this, chrome);

            this.WindowStyle = WindowStyle.SingleBorderWindow;
        }

        private void InitializeCommands()
        {
            this.CommandBindings.Add(
                new CommandBinding(
                    WinSystemCommands.MinimizeWindowCommand, WindowMinimize_Execute));

            this.CommandBindings.Add(
                new CommandBinding(
                    WinSystemCommands.MaximizeWindowCommand, WindowMaximize_Execute));

            this.CommandBindings.Add(
                new CommandBinding(
                    WinSystemCommands.RestoreWindowCommand, WindowRestore_Execute));

            this.CommandBindings.Add(
                new CommandBinding(
                    WinSystemCommands.CloseWindowCommand, WindowClose_Execute));
        }

        #region [ Commands ]
        private void WindowClose_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            WinSystemCommands.CloseWindow(this);
        }

        private void WindowMaximize_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            WinSystemCommands.MaximizeWindow(this);
        }

        private void WindowRestore_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            WinSystemCommands.RestoreWindow(this);
        }
        
        private void WindowMinimize_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            WinSystemCommands.MinimizeWindow(this);
        }
        #endregion

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var helper = new WindowInteropHelper(this);
            var hwndSource = HwndSource.FromHwnd(helper.Handle);

            SetValue(HandlePropertyKey, helper.Handle);
            hwndSource.AddHook(WndProc);
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
