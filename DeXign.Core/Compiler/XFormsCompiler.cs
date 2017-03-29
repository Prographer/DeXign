using DeXign.Core.Controls;
using DeXign.Core.Logic;
using System;
using System.Threading.Tasks;

namespace DeXign.Core.Compiler
{
    internal class XFormsCompiler : BaseCompilerService
    {
        public XFormsCompiler()
        {
            this.Platform = Platform.XForms;
        }

        public override async Task<DXCompileResult> Compile(DXCompileParameter parameter)
        {
            return new DXCompileResult(parameter.Option)
            {
                Errors =
                {
                    new NotImplementedException("Coming Soon! (Xamarin Forms Compiler)")
                }
            };
        }
    }
}
