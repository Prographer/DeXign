using System;
using System.Collections.Generic;
using DeXign.Core.Controls;
using DeXign.Core.Logic;
using System.Linq;

namespace DeXign.Core.Compiler
{
    public class WPFCompiler : BaseCompilerService
    {
        public WPFCompiler()
        {
            this.Platform = Platform.Window;
        }

        public override IEnumerable<Exception> Compile(DXCompileOption option, PContentPage[] screens, PBinderHost[] components)
        {
            var codeUnit = new CodeGeneratorUnit<PObject>()
            {
                NodeIterating = true
            };

            codeUnit.Items.AddRange(screens);

            var assemblyInfo = new CodeGeneratorAssemblyInfo();
            var manifest = new CodeGeneratorManifest();

            var wpfLayoutGenerator = new WPFLayoutGenerator(
                codeUnit,
                manifest,
                assemblyInfo);

            foreach (string code in wpfLayoutGenerator.Generate())
            {
                
            }

            return Enumerable.Empty<Exception>();
        }
    }
}
