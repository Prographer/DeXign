using System;
using System.CodeDom;
using System.Collections;
using System.Diagnostics;
using System.Windows;

using DeXign.Core;
using DeXign.Core.Collections;
using DeXign.Core.Controls;
using DeXign.Core.Designer;
using DeXign.Extension;

namespace DeXign.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Test2();
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
                    VerticalAlignment = LayoutAlignment.Center,
                    HorizontalAlignment = LayoutAlignment.End,
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