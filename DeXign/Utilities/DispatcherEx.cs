using System.Windows;
using System.Windows.Threading;

namespace DeXign.Utilities
{
    public static class DispatcherEx
    {
        static Dispatcher dispatcher;

        static DispatcherEx()
        {
            dispatcher = Application.Current.Dispatcher;
        }

        public static void WaitForContextIdle()
        {
            WaitFor(DispatcherPriority.ContextIdle);
        }

        public static void WaitForRender()
        {
            WaitFor(DispatcherPriority.Render);
        }

        public static void WaitFor(DispatcherPriority priority)
        {
            dispatcher.Invoke(FakeMethod, priority);
        }

        private static void FakeMethod()
        {
        }
    }
}
