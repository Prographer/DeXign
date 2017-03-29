using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace DeXign.Core.Compiler
{
    internal class WPFDebugger : BaseDebugger
    {
        public Process AppProcess { get; private set; }

        public string FileName { get; }

        public bool IsBusy => this.AppProcess != null;

        public WPFDebugger(string exePath)
        {
            this.FileName = exePath;
        }
        
        public override async Task Run()
        {
            if (IsBusy)
                return;
            
            var startInfo = new ProcessStartInfo(this.FileName);

            this.AppProcess = Process.Start(startInfo);

            // Stop으로 AppProcess가 삭제될 수 있음
            while (this.AppProcess != null && !this.AppProcess.HasExited)
            {
                await Task.Delay(1000);

                this.AppProcess.Refresh();
            }

            await this.Stop();
        }

        public override async Task Stop()
        {
            if (this.AppProcess != null)
            {
                this.AppProcess.Refresh();

                if (!this.AppProcess.HasExited)
                {
                    this.AppProcess.Kill();
                }

                this.AppProcess.Dispose();

                this.AppProcess = null;
            }
        }
    }
}