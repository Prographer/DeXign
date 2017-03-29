using System;
using System.Threading.Tasks;

namespace DeXign.Core.Compiler
{
    public abstract class BaseCompilerService : IProgress<double>
    {
        public event EventHandler<double> ProgressChanged;

        public Platform Platform { get; set; }

        public abstract Task<DXCompileResult> Compile(DXCompileParameter parameter);

        public void Report(double value)
        {
            ProgressChanged?.Invoke(this, value);
        }

        public async Task OnReport(double value)
        {
            this.Report(value);

            await Task.Delay(10);
        }
    }
}