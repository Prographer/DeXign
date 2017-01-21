using Phlet.Core.Controls;

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Phlet.Core
{
    public enum XFormsGenerateType
    {
        Xaml,
        Code
    }

    public class XFormsGenerator : Generator<XFormsAttribute, PObject>
    {

        public XFormsGenerateType GenerateType { get; set; } = XFormsGenerateType.Code;

        public XFormsGenerator(
            XFormsGenerateType generateType,
            CodeGeneratorUnit<PObject> cgUnit,
            CodeGeneratorManifest cgManifest, 
            CodeGeneratorAssemblyInfo cgAssmInfo) : base(cgUnit, cgManifest, cgAssmInfo)
        {
        }

        protected override IEnumerable<string> OnGenerate(IEnumerable<CodeComponent<XFormsAttribute>> components)
        {
            switch (GenerateType)
            {
                case XFormsGenerateType.Code:
                    yield return CodeGenerate(components);
                    break;

                case XFormsGenerateType.Xaml:
                    yield return XamlGenerate(components);
                    break;
            }

#if DEBUG
            foreach (var c in components)
            {
                string space = new string('\t', c.Depth);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"{space}{c.Attribute.Name}");
                Console.ForegroundColor = ConsoleColor.Gray;

                if (c.ElementType == ComponentType.Instance)
                {
                    Console.Write($" ({c.Element.GetType().Name})");
                }
                else
                {
                    var pi = (PropertyInfo)c.Element;

                    Console.Write($" ({pi.Name})");
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine();// $" - {c.ElementType}");
            }
#endif

            yield return "Not Implemented";
        }

        private string XamlGenerate(IEnumerable<CodeComponent<XFormsAttribute>> components)
        {
            throw new NotImplementedException();
        }

        private string CodeGenerate(IEnumerable<CodeComponent<XFormsAttribute>> components)
        {
            throw new NotImplementedException();
        }
    }
}
