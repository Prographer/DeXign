using DeXign.Core.Controls;
using DeXign.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Linq;
using System.IO;
using System.Text;

namespace DeXign.Core
{
    public enum XFormsGenerateType
    {
        Xaml,
        Code
    }

    public class XFormsGenerator : Generator<XFormsAttribute, PObject>
    {
        const string XMLNS = "http://xamarin.com/schemas/2014/forms";
        const string XMLNSX = "http://schemas.microsoft.com/winfx/2009/xaml";

        public XFormsGenerateType GenerateType { get; set; } = XFormsGenerateType.Code;

        public XFormsGenerator(
            XFormsGenerateType generateType,
            CodeGeneratorUnit<PObject> cgUnit,
            CodeGeneratorManifest cgManifest, 
            CodeGeneratorAssemblyInfo cgAssmInfo) : base(cgUnit, cgManifest, cgAssmInfo)
        {
            this.GenerateType = generateType;
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
            var items = components.ToArray();
            var root = items[0];

            var doc = new XmlDocument();
            var dec = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            
            // 노드
            XmlElement rootElement = doc.CreateElement(root.Attribute.Name);
            rootElement.SetAttribute("xmlns", XMLNS);
            rootElement.SetAttribute("xmlns:x", XMLNSX);
            SetXamlName(rootElement, root);

            var comQueue = new Queue<CodeComponent<XFormsAttribute>>(new[] { root });
            var xmlQueue = new Queue<XmlElement>(new[] { rootElement });

            while (comQueue.Count > 0)
            {
                var com = comQueue.Dequeue();
                var xml = xmlQueue.Dequeue();

                if (com.HasChildren)
                {
                    var content = com.GetContentComponent();

                    // property
                    foreach (var child in com.Children
                        .Where(c => c.ElementType == ComponentType.Property))
                    {
                        if (content == child)
                            continue;

                        var pi = child.Element as PropertyInfo;

                        if (pi.IsDefaultDependencyProperty(child.Parent.Element as PObject))
                            continue;

                        object value = pi.GetValue(child.Parent.Element);
                        
                        xml.SetAttribute(
                            child.Attribute.Name, 
                            ValueToXamlInline(value));
                    }

                    // Content
                    if (content != null)
                    {
                        var pi = content.Element as PropertyInfo;

                        if (pi.CanCastingTo<IEnumerable<PObject>>())
                        {
                            foreach (var item in content.Children.Reverse())
                            {
                                var contentXml = doc.CreateElement(item.Attribute.Name);
                                xml.AppendChild(contentXml);

                                SetXamlName(contentXml, item);

                                comQueue.Enqueue(item);
                                xmlQueue.Enqueue(contentXml);
                            }
                        }
                        else if (pi.CanCastingTo<PObject>())
                        {
                            var contentItem = content.Children[0];
                            var contentXml = doc.CreateElement(contentItem.Attribute.Name);
                            xml.AppendChild(contentXml);

                            SetXamlName(contentXml, contentItem);

                            comQueue.Enqueue(contentItem);
                            xmlQueue.Enqueue(contentXml);
                        }
                    }
                }
            }

            doc.AppendChild(dec);
            doc.AppendChild(rootElement);

            // 인덴트 처리
            using (var ms = new MemoryStream())
            {
                using (var writer = new XmlTextWriter(ms, new UTF8Encoding(false)))
                {
                    writer.Formatting = Formatting.Indented;
                    doc.Save(writer);

                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }

        private string CodeGenerate(IEnumerable<CodeComponent<XFormsAttribute>> components)
        {
            return "Not Implemented";
        }

        private void SetXamlName(XmlElement element, CodeComponent<XFormsAttribute> component)
        {
            if (component.ElementType == ComponentType.Instance)
            {
                var pObj = (PObject)component.Element;

                if (!string.IsNullOrWhiteSpace(pObj.Name))
                    element.SetAttribute("Name", XMLNSX, pObj.Name);
            }
        }

        private string ValueToXamlInline(object value)
        {
            if (value.HasAttribute<XFormsAttribute>())
            {
                var attr = value.GetAttribute<XFormsAttribute>();
                
                return value.ToString();
            }

            return value.ToString();
        }

        private string ValueToCodeInline(object value)
        {
            throw new NotImplementedException();
        }
    }
}
