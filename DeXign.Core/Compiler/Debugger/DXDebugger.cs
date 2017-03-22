using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DeXign.Core.Compiler
{
    public static class DXDebugger
    {
        private static Stack<BaseDebugger> stableDebugger;

        static DXDebugger()
        {
            stableDebugger = new Stack<BaseDebugger>();
        }

        public static async Task<bool> RunWinApplication(string exePath)
        {
            if (!File.Exists(exePath))
                return false;

            await DXDebugger.PushDebugger(new WPFDebugger(exePath));

            return true;
        }

        private static async Task PushDebugger(BaseDebugger debugger)
        {
            stableDebugger.Push(debugger);

            await debugger.Run();

            await DXDebugger.Stop();
        }

        public static async Task Stop()
        {
            if (stableDebugger.Count > 0)
            {
                BaseDebugger debugger = stableDebugger.Pop();

                await debugger.Stop();
            }
        }
    }
}
