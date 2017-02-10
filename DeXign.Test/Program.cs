using System.Windows;

using DeXign.Core;
using DeXign.Core.Controls;
using System.CodeDom;
using DeXign.Core.Collections;
using System.Collections;
using System;
using System.Diagnostics;
using DeXign.Core.Designer;

namespace DeXign.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Test1();
            //DesignerTest();
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
                    VerticalAlignment = LayoutAlignment.FillAndExpand,
                    HorizontalAlignment = LayoutAlignment.FillAndExpand,

                    ColumnDefinitions =
                    {
                        new PColumnDefinition()
                        {
                            Width = new PGridLength(160, GridUnitType.Absolute)
                        },
                        new PColumnDefinition()
                        {
                            Width = new PGridLength(1, GridUnitType.Star)
                        }
                    },
                    RowDefinitions =
                    {
                        new PRowDefinition()
                        {
                            Height = new PGridLength(1, GridUnitType.Star)
                        },
                        new PRowDefinition()
                        {
                            Height = new PGridLength(1, GridUnitType.Star)
                        }
                    },

                    Children =
                    {
                        (lbl1 = new PLabel
                        {
                            Name = "lbl1",
                            HorizontalTextAlignment = TextAlignment.Center,
                            Text = "Hello World"
                        }),
                        (lbl2 = new PLabel
                        {
                            HorizontalTextAlignment = TextAlignment.Center,
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