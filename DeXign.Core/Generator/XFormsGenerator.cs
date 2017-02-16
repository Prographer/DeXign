using DeXign.Core.Controls;
using DeXign.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Linq;
using System.IO;
using System.Text;
using DeXign.Extension;

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
            CodeComponent<XFormsAttribute> root = items[0];
            string pageName = null;

            if (root.Element is PPage)
                pageName = (root.Element as PPage).GetPageName();

            // 루트페이지는 항상 이름(파일, 클래스 이름)이 설정되어있어야 함
            if (pageName == null)
                throw new ArgumentException();

            var doc = new XmlDocument();
            var dec = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            
            // 노드
            XmlElement rootElement = doc.CreateElement(root.Attribute.Name);
            rootElement.SetAttribute("xmlns", XMLNS);
            rootElement.SetAttribute("xmlns:x", XMLNSX);
            rootElement.SetAttribute("xmlns:local", $"clr-namespace:{this.Manifest.NamespaceName}");
            rootElement.SetAttribute("Class", XMLNSX, $"{this.Manifest.NamespaceName}.{pageName}");

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

                        if (pi.CanCastingTo<IEnumerable<PObject>>())
                        {
                            var lstXml = doc.CreateElement(
                                $"{child.Parent.Attribute.Name}.{child.Attribute.Name}");

                            xml.AppendChild(lstXml);

                            if (child.HasChildren)
                            {
                                foreach (var item in child.Children?.Reverse())
                                {
                                    var childXml = doc.CreateElement(item.Attribute.Name);
                                    lstXml.AppendChild(childXml);

                                    SetXamlName(childXml, item);

                                    comQueue.Enqueue(item);
                                    xmlQueue.Enqueue(childXml);
                                }
                            }
                        }
                        else
                        {
                            xml.SetAttribute(
                                child.Attribute.Name,
                                ValueToXamlInline(value));
                        }
                    }

                    // Content
                    if (content != null)
                    {
                        var pi = content.Element as PropertyInfo;

                        if (pi.CanCastingTo<IEnumerable<PObject>>())
                        {
                            if (content.HasChildren)
                            {
                                foreach (var item in content.Children?.Reverse())
                                {
                                    var contentXml = doc.CreateElement(item.Attribute.Name);
                                    xml.AppendChild(contentXml);

                                    SetXamlName(contentXml, item);

                                    comQueue.Enqueue(item);
                                    xmlQueue.Enqueue(contentXml);
                                }
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

        private string XamlCodeGenerate()
        {
            throw new NotImplementedException();
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
            if (value == null)
                return "";

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
