using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeXign.Core
{
    class WPFLayoutGenerator : Generator<WPFAttribute, PObject>
    {
        const string XMLNS = "http://xamarin.com/schemas/2014/forms";
        const string XMLNSX = "http://schemas.microsoft.com/winfx/2009/xaml";

        public WPFLayoutGenerator(
            CodeGeneratorUnit<PObject> cgUnit,
            CodeGeneratorManifest cgManifest,
            CodeGeneratorAssemblyInfo cgAssmInfo) : base(cgUnit, cgManifest, cgAssmInfo)
        {
        }

        protected override IEnumerable<string> OnGenerate(IEnumerable<CodeComponent<WPFAttribute>> components)
        {
            throw new NotImplementedException();
        }
    }
}