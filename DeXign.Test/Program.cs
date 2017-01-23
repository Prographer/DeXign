using System.Windows;

using DeXign.Core;
using DeXign.Core.Controls;
using System.CodeDom;
using DeXign.Core.Collections;
using System.Collections;
using System;
using System.Diagnostics;

namespace DeXign.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            PStackLayout stack;

            // Memory
            var content = new PContentPage
            {
                Name = "rootPage",
                Title = "DeXign",
                Content = (stack = new PStackLayout
                {
                    VerticalAlignment = LayoutAlignment.Center,
                    HorizontalAlignment = LayoutAlignment.End,
                    Children =
                    {
                        new PLabel
                        {
                            Name = "lbl1",
                            HorizontalTextAlignment = TextAlignment.Center,
                            Text = "Hello World"
                        },
                        new PLabel
                        {
                            HorizontalTextAlignment = TextAlignment.Center,
                            Text = "Welcome to DeXign!",
                            Rotation = 2
                        }
                    }
                })
            };
            
            var sw = new Stopwatch();
            sw.Start();

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
                XFormsGenerateType.Xaml,
                codeUnit,
                manifest,
                assemblyInfo);
            
            foreach (string code in xGenerator.Generate())
            {
                sw.Stop();

                Console.WriteLine(code);
                Console.WriteLine($"Elapsed: {sw.ElapsedMilliseconds}ms");
            }
        }
    }
}