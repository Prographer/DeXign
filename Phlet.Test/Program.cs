using System.Windows;

using Phlet.Core;
using Phlet.Core.Controls;
using System.CodeDom;
using Phlet.Core.Collections;
using System.Collections;

namespace Phlet.Test
{
    class Program
    {
        static void Main(string[] args)
        {   
            // Memory
            var content = new PContentPage
            {
                Title = "Phlet",
                Content = new PStackLayout
                {
                    VerticalAlignment = LayoutAlignment.Center,
                    HorizontalAlignment = LayoutAlignment.End,
                    Children =
                    {
                        new PLabel
                        {
                            HorizontalTextAlignment = TextAlignment.Center,
                            Text = "Welcome to Phlet!"
                        }
                    }
                }
            };
            
            // Generate
            var codeUnit = new CodeGeneratorUnit<PObject>()
            {
                NodeIterating = true,
                Items =
                {
                    content
                }
            };

            var assemblyInfo = new CodeGeneratorAssemblyInfo();
            var manifest = new CodeGeneratorManifest();

            var xGenerator = new XFormsGenerator(
                XFormsGenerateType.Code,
                codeUnit,
                manifest,
                assemblyInfo);

            foreach (string code in xGenerator.Generate())
            {
                // TODO: 
            }
        }
    }
}