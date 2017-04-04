using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

using DeXign.Extension;
using DeXign.Interop;

using WPFExtension;

using Microsoft.Windows.Shell;

using MINMAXINFO = DeXign.Interop.NativeMethods.MINMAXINFO;
using Monitor = DeXign.Interop.NativeMethods.Monitor;
using MONITORINFO = DeXign.Interop.NativeMethods.MONITORINFO;
using WinSystemCommands = System.Windows.SystemCommands;

namespace DeXign.Controls
{
    [TemplatePart(Name = "PART_taskNavigationBox", Type = typeof(TaskNavigationBox))]
    public class ChromeWindow : Window
    {
        #region [ Dependency Property ]
        public static readonly DependencyProperty IsLoadingProperty =
            DependencyHelper.Register(new PropertyMetadata(false));

        public static readonly DependencyProperty CaptionHeightProperty =
            DependencyHelper.Register(new PropertyMetadata(30));

        private static readonly DependencyPropertyKey HandlePropertyKey =
            DependencyHelper.RegisterReadOnly();

        public static readonly DependencyProperty HandleProperty =
            HandlePropertyKey.DependencyProperty;

        public static readonly DependencyProperty MenuProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty SubMenuProperty =
            DependencyHelper.Register();
        #endregion

        #region [ Property ]
        public TaskNavigationBox TaskNavigator { get; private set; }

        public bool IsLoading
        {
            get { return this.GetValue<bool>(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        public int CaptionHeight
        {
            get { return this.GetValue<int>(CaptionHeightProperty); }
            set { SetValue(CaptionHeightProperty, value); }
        }

        public Menu Menu
        {
            get { return this.GetValue<Menu>(MenuProperty); }
            set { SetValue(MenuProperty, value); }
        }

        public object SubMenu
        {
            get { return GetValue(SubMenuProperty); }
            set { SetValue(SubMenuProperty, value); }
        }

        public IntPtr Handle => this.GetValue<IntPtr>(HandlePropertyKey.DependencyProperty);

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

            CaptionHeightProperty.AddValueChanged(this, CaptionHeightChanged);
        }

        private void CaptionHeightChanged(object sender, EventArgs e)
        {
            WindowChrome.GetWindowChrome(this).CaptionHeight = this.CaptionHeight;
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

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            TaskNavigator = (TaskNavigationBox)GetTemplateChild("PART_taskNavigationBox");
        }
    }
}