using System;
using System.Linq;
using System.Diagnostics;

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
            var binder1 = new PBinderHost();
            var binder2 = new PBinderHost();
            var binder3 = new PBinderHost();

            binder1.AddNewBinder(BindOptions.Output);
            binder1.AddNewBinder(BindOptions.Input);
            binder1.AddNewBinder(BindOptions.Return);

            binder2.AddNewBinder(BindOptions.Output);
            binder2.AddNewBinder(BindOptions.Input);
            binder2.AddNewBinder(BindOptions.Parameter);

            binder3.AddNewBinder(BindOptions.Output);
            binder3.AddNewBinder(BindOptions.Input);
            binder3.AddNewBinder(BindOptions.Parameter);

            // Binder1 -> Binder2
            binder1[BindOptions.Output].First()
                .Bind(binder2[BindOptions.Input].First());

            binder1[BindOptions.Return].First()
                .Bind(binder2[BindOptions.Parameter].First());

            binder1[BindOptions.Return].First()
                .Bind(binder3[BindOptions.Parameter].First());

            // Binder2 -> Binder3
            binder2[BindOptions.Output].First()
                .Bind(binder3[BindOptions.Input].First());

            var print = new Action<string, PBinderHost>(
                (name, host) =>
                {
                    Console.WriteLine($" # {name} # ");

                    foreach (var binder in host.Items)
                    {
                        foreach (var expression in binder.Items.GetExpressions())
                        {
                            bool n1 = expression.Output.Equals(binder);
                            bool n2 = expression.Input.Equals(binder);

                            Console.WriteLine($"{(n1 ? name : "Output")}({expression.Output.BindOption.ToString()}) -> {(n2 ? name : "Input")}({expression.Input.BindOption.ToString()})");
                        }
                    }

                    Console.WriteLine();
                });

            print("Binder1", binder1);
            print("Binder2", binder2);
            print("Binder3", binder3);

            binder1.ReleaseAll();
            binder2.ReleaseAll();

            Console.WriteLine(" ** Released ** \r\n");

            print("Binder1", binder1);
            print("Binder2", binder2);
            print("Binder3", binder3);
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