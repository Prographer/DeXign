using System;
using System.Linq;
using System.Collections.Generic;

using DeXign.Extension;
using DeXign.Core.Logic;
using DeXign.Core.Controls;
using System.Threading.Tasks;

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

        public static async Task<DXCompileResult> Compile(DXCompileParameter parameter)
        {
            foreach (BaseCompilerService service in GetCompilerService(parameter.Option.TargetPlatform))
            {
                return await service.Compile(parameter);
            }

            // 컴파일 오류
            return new DXCompileResult(parameter.Option)
            {
                IsSuccess = false,
                Errors =
                {
                    new Exception($"{parameter.Option.TargetPlatform.ToString()}에 해당하는 컴파일러를 찾을 수 없습니다.")
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
