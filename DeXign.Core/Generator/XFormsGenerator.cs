using DeXign.Core.Controls;
using DeXign.Core;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DeXign.Core
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
#if DEBUG
            foreach (var c in components)
            {
                string space = new string('\t', c.Depth);
                
                if (c.ElementType == ComponentType.Instance)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write($"{space}{c.Attribute.Name}");
                    Console.ForegroundColor = ConsoleColor.Gray;

                    Console.Write($" ({c.Element.GetType().Name})");
                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine($" - {c.ElementType}");
                }
                else
                {
                    var pi = (PropertyInfo)c.Element;
                    var parent = (PObject)c.Parent.Element;

                    if (!pi.IsDefaultDependencyProperty(parent) ||
                        pi.CanCastingTo<IEnumerable<PObject>>() ||
                        pi.CanCastingTo<PObject>())
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write($"{space}{c.Attribute.Name}");
                        Console.ForegroundColor = ConsoleColor.Gray;

                        Console.Write($" ({pi.Name})");
                        Console.ForegroundColor = ConsoleColor.Green;
                        
                        Console.WriteLine($" - {c.ElementType}");
                    }
                }
            }
#endif

            switch (GenerateType)
            {
                case XFormsGenerateType.Code:
                    yield return CodeGenerate(components);
                    break;

                case XFormsGenerateType.Xaml:
                    yield return XamlGenerate(components);
                    break;
            }
        }

        private string XamlGenerate(IEnumerable<CodeComponent<XFormsAttribute>> components)
        {
            return "Not Implemented";
        }

        private string CodeGenerate(IEnumerable<CodeComponent<XFormsAttribute>> components)
        {
            return "Not Implemented";
        }
    }
}
