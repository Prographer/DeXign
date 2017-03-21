using DeXign.Core.Controls;
using DeXign.Core.Logic;
using System;
using System.Collections.Generic;

namespace DeXign.Core.Compiler
{
    public class XFormsCompiler : BaseCompilerService
    {
        public XFormsCompiler()
        {
            this.Platform = Platform.XForms;
        }

        public override IEnumerable<Exception> Compile(DXCompileOption option, PContentPage[] screens, PBinderHost[] components)
        {
            yield return new NotImplementedException("Coming Soon! (Xamarin Forms Compiler)");
        }
    }
}
