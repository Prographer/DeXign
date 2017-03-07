using System;
using System.Diagnostics;

using DeXign.IO;
using DeXign.Core;
using DeXign.Core.Logic;
using DeXign.Core.Designer;
using DeXign.Core.Controls;

namespace DeXign.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var c1 = new PComponent() { Tag = "c1" };
            var c2 = new PComponent() { Tag = "c2" };
            var c3 = new PComponent() { Tag = "c3" };

            // c1 -> c2 -> c3

            c2.Bind(c1, BinderOptions.Trigger);
            c3.Bind(c2, BinderOptions.Trigger);

            var printAction = new Action<PComponent>((PComponent c) =>
            {
                Console.WriteLine($"# {c.Tag}.Outputs");
                foreach (PComponent binder in c.Outputs)
                    Console.WriteLine($"{c.Tag} -> {binder.Tag}");

                Console.WriteLine($"# {c.Tag}.Parameters");
                foreach (PComponent binder in c.Parameters)
                    Console.WriteLine($"{c.Tag} -> {binder.Tag}");

                Console.WriteLine($"# {c.Tag}.Inputs");
                foreach (PComponent binder in c.Inputs)
                    Console.WriteLine($"{c.Tag} -> {binder.Tag}");

                Console.WriteLine();
            });

            printAction(c1);
            printAction(c2);
            printAction(c3);

            c1.ReleaseAll();
            c2.ReleaseAll();

            Console.WriteLine(new string('-', 30) + "\r\n");
            
            printAction(c1);
            printAction(c2);
            printAction(c3);
        }

        private static void DesignerTest()
        {
            foreach (var aTuple in DesignerManager.GetElementTypes())
            {
                Console.WriteLine(aTuple.Attribute.DisplayName);
                Console.WriteLine(aTuple.Element.Name);
                Console.WriteLine();
            }
        }

        private static void Test2()
        {
            PGridLayout grid;
            PLabel lbl1;
            PLabel lbl2;

            // Memory
            var content = new PContentPage
            {
                Name = "rootPage",
                Title = "DeXign",
                Content = (grid = new PGridLayout
                {
                    VerticalAlignment = PVerticalAlignment.Stretch,
                    HorizontalAlignment = PHorizontalAlignment.Stretch,

                    ColumnDefinitions =
                    {
                        new PColumnDefinition()
                        {
                            Width = new PGridLength(160, PGridUnitType.Absolute)
                        },
                        new PColumnDefinition()
                        {
                            Width = new PGridLength(1, PGridUnitType.Star)
                        }
                    },
                    RowDefinitions =
                    {
                        new PRowDefinition()
                        {
                            Height = new PGridLength(1, PGridUnitType.Star)
                        },
                        new PRowDefinition()
                        {
                            Height = new PGridLength(1, PGridUnitType.Star)
                        }
                    },

                    Children =
                    {
                        (lbl1 = new PLabel
                        {
                            Name = "lbl1",
                            HorizontalTextAlignment = PHorizontalTextAlignment.Center,
                            Text = "Hello World"
                        }),
                        (lbl2 = new PLabel
                        {
                            HorizontalTextAlignment = PHorizontalTextAlignment.Center,
                            Text = "Welcome to DeXign!",
                            Rotation = 2
                        })
                    }
                })
            };
            
            PGridLayout.SetColumn(lbl1, 1);

            PGridLayout.SetColumn(lbl2, 0);
            PGridLayout.SetRow(lbl2, 1);

            LayoutExtension.SetPageName(content, "MainPage");

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

        private static void Test1()
        {
            PStackLayout stack;

            // Memory
            var content = new PContentPage
            {
                Name = "rootPage",
                Title = "DeXign",
                Content = (stack = new PStackLayout
                {
                    VerticalAlignment = PVerticalAlignment.Center,
                    HorizontalAlignment = PHorizontalAlignment.Right,
                    Children =
                    {
                        new PLabel
                        {
                            Name = "lbl1",
                            HorizontalTextAlignment = PHorizontalTextAlignment.Center,
                            Text = "Hello World"
                        },
                        new PLabel
                        {
                            HorizontalTextAlignment = PHorizontalTextAlignment.Center,
                            Text = "Welcome to DeXign!",
                            Rotation = 2
                        }
                    }
                })
            };

            LayoutExtension.SetPageName(content, "MainPage");

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