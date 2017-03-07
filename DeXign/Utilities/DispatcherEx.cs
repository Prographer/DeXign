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
            dispatcher.Invoke(FakeMethod, DispatcherPriority.ContextIdle);
        }

        private static void FakeMethod()
        {
        }
    }
}
