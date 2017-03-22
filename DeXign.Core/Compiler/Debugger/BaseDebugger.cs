using System.Threading.Tasks;

namespace DeXign.Core.Compiler
{
    internal abstract class BaseDebugger
    {
        public abstract Task Run();
        public abstract Task Stop();
    }
}
