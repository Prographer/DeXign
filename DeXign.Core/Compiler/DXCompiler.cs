using System;
using System.Linq;
using System.Collections.Generic;

using DeXign.Extension;
using DeXign.Core.Logic;
using DeXign.Core.Controls;

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

        public static DXCompileResult Compile(DXCompileOption option, PContentPage[] screens, PBinderHost[] components)
        {
            foreach (BaseCompilerService service in GetCompilerService(option.TargetPlatform))
            {
                return service.Compile(option, screens, components);
            }

            // 컴파일 오류
            return new DXCompileResult(option)
            {
                IsSuccess = false,
                Errors =
                {
                    new Exception($"{option.ToString()}에 해당하는 컴파일러를 찾을 수 없습니다.")
                }
            };
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
