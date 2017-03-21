using DeXign.Core.Controls;
using DeXign.Core.Logic;
using DeXign.Extension;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public static IEnumerable<Exception> Compile(DXCompileOption option, PContentPage[] screens, PBinderHost[] components)
        {
            foreach (BaseCompilerService service in GetCompilerService(option.TargetPlatform))
            {
                // TODO: Compile
                return service.Compile(option, screens, components);
            }

            return Enumerable.Empty<Exception>();
        }

        public static IEnumerable<BaseCompilerService> GetCompilerService(Platform platform)
        {
            foreach (var service in compilers)
                if (service.Platform.HasFlag(platform))
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
