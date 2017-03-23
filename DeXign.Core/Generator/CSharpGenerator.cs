using DeXign.Core.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeXign.Core
{
    public class CSharpGenerator : Generator<CSharpAttribute, PBinderHost>
    {
        public NameContainer LayoutContainer { get; }

        public CSharpGenerator(
            NameContainer nameContainer,
            CodeGeneratorUnit<PBinderHost> cgUnit,
            CodeGeneratorManifest cgManifest,
            CodeGeneratorAssemblyInfo cgAssmInfo) : base(cgUnit, cgManifest, cgAssmInfo)
        {
            this.LayoutContainer = nameContainer;
        }

        protected override IEnumerable<string> OnGenerate(IEnumerable<CodeComponent<CSharpAttribute>> components)
        {
            var items = components.ToArray();
            CodeComponent<CSharpAttribute> root = items.FirstOrDefault();

            yield return "";
        }
    }
}
