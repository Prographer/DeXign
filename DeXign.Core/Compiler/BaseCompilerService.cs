using System;
using System.Collections.Generic;

using DeXign.Core.Logic;
using DeXign.Core.Controls;

namespace DeXign.Core.Compiler
{
    public abstract class BaseCompilerService
    {
        public Platform Platform { get; set; }

        public abstract DXCompileResult Compile(DXCompileOption option, PContentPage[] screens, PBinderHost[] components);
    }
}