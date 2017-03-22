using DeXign.Core.Controls;
using DeXign.Core.Logic;
using System;

namespace DeXign.Core.Compiler
{
    internal class XFormsCompiler : BaseCompilerService
    {
        public XFormsCompiler()
        {
            this.Platform = Platform.XForms;
        }

        public override DXCompileResult Compile(DXCompileOption option, PContentPage[] screens, PBinderHost[] components)
        {
            return new DXCompileResult(option)
            {
                Errors =
                {
                    new NotImplementedException("Coming Soon! (Xamarin Forms Compiler)")
                }
            };
        }
    }
}
