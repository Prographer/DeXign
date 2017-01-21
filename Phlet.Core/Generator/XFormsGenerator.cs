using Phlet.Core.Controls;

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Phlet.Core
{
    public class XFormsGenerator : Generator<XFormsAttribute, PObject>
    {
        public XFormsGenerator(
            CodeGeneratorUnit<PObject> cgUnit,
            CodeGeneratorManifest cgManifest, 
            CodeGeneratorAssemblyInfo cgAssmInfo) : base(cgUnit, cgManifest, cgAssmInfo)
        {
        }

        protected override IEnumerable<string> OnGenerate(IEnumerable<CodeComponent<XFormsAttribute>> components)
        {
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
            
            yield return "Not Implemented";
        }
    }
}
