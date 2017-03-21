using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeXign.Core
{
    public class XMLGenerator : Generator<WPFAttribute, PObject>
    {
        public XMLGenerator(CodeGeneratorUnit<PObject> cgUnit, CodeGeneratorManifest cgManifest, CodeGeneratorAssemblyInfo cgAssmInfo) : base(cgUnit, cgManifest, cgAssmInfo)
        {
        }

        protected override IEnumerable<string> OnGenerate(IEnumerable<CodeComponent<WPFAttribute>> components)
        {
            throw new NotImplementedException();
        }
    }
}
