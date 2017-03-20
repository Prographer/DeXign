using DeXign.Core.Controls;
using DeXign.Core.Logic;
using DeXign.Extension;
using System.Collections.Generic;

namespace DeXign.Core.Compiler
{
    public static class DXCompiler
    {
        private static List<BaseCompilerService> compilers;

        static DXCompiler()
        {
            compilers = new List<BaseCompilerService>();

            AddCompiler(new WPFCompiler());
            AddCompiler(new XFormsCompiler());
        }

        public static void Compile(DXCompileOption option, PContentPage[] screens, PComponent[] components)
        {
            foreach (BaseCompilerService service in GetCompilerService(option.TargetPlatform))
            {
                // TODO: Compile
            }
        }

        public static IEnumerable<BaseCompilerService> GetCompilerService(Platform platform)
        {
            foreach (var service in compilers)
                if (platform.HasFlag(service.Platform))
                    yield return service;
        }

        public static void AddCompiler(BaseCompilerService compiler)
        {
            compilers.SafeAdd(compiler);
        }

        public static void RemoveCompiler(BaseCompilerService compiler)
        {
            compilers.SafeRemove(compiler);
        }
    }
}
