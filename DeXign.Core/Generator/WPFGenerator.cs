using DeXign.Core.Controls;
using DeXign.Extension;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System;
using DeXign.SDK;

namespace DeXign.Core
{
    public class NameContainer : Dictionary<string, object>
    {
        public new object this[string key]
        {
            get
            {
                return base[key];
            }
            set
            {
                base[key] = value;
            }
        }

        public string this[object obj]
        {
            get
            {
                return this.FirstOrDefault(kv => object.ReferenceEquals(kv.Value, obj)).Key;
            }
            set
            {
                this.Remove(this[obj]);
                this[value] = obj;
            }
        }

        public void Add(object obj)
        {
            string name = GetName(obj);

            this[name] = obj;
        }
        
        public string GetName(object obj)
        {
            string prefix = obj.GetType().Name;
            string name = null;

            prefix = $"{prefix[0].ToString().ToLower()}{prefix.Substring(1)}";

            if (obj is PObject pObj)
                name = pObj.Name;

            if (string.IsNullOrWhiteSpace(name))
            {
                int idx = 1;

                do
                {
                    name = $"__{prefix}{idx++}__";
                } while (this.ContainsKey(name));
            }
            else
            {
                name = $"__{name}__";
            }

            return name;
        }
    }

    class WPFLayoutGenerator : Generator<WPFAttribute, PObject>
    {
        const string XMLNS = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

        public NameContainer NameContainer { get; }
        public Dictionary<string, string> NamespaceContainer { get; }

        public List<string> Images { get; }

        public WPFLayoutGenerator(
            CodeGeneratorUnit<PObject> cgUnit,
            CodeGeneratorManifest cgManifest,
            CodeGeneratorAssemblyInfo cgAssmInfo) : base(cgUnit, cgManifest, cgAssmInfo)
        {
            this.Images = new List<string>();
            this.NameContainer = new NameContainer();
            this.NamespaceContainer = new Dictionary<string, string>();
        }

        protected override IEnumerable<string> OnGenerate(IEnumerable<CodeComponent<WPFAttribute>> components)
        {
            var items = components.ToArray();
            CodeComponent<WPFAttribute> root = items.FirstOrDefault();
            
            // �̸��� �������� ������� ��������
            ImagePatch(items);
            NamingPatch(items);
            NamespacePatch(items);

            if (root.Element is PPage page)
            {
                this.NameContainer[page] = page.GetPageName();
            }

            var doc = new XmlDocument();

            XmlElement rootElement = CreateElement(doc, root);
            rootElement.SetAttribute("xmlns", XMLNS);

            // ���ӽ����̽� ����
            foreach (var namespaceKv in this.NamespaceContainer)
            {
                rootElement.SetAttribute($"xmlns:{namespaceKv.Value}", namespaceKv.Key);
            }

            SetXamlName(rootElement, root);

            var comQueue = new Queue<CodeComponent<WPFAttribute>>(new[] { root });
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
                        .Where(c => c.ElementType == CodeComponentType.Property))
                    {
                        if (content == child)
                            continue;

                        var pi = child.Element as PropertyInfo;

                        //if (pi.IsDefaultDependencyProperty(child.Parent.Element as PObject))
                        //    continue;

                        object value = pi.GetValue(child.Parent.Element);

                        if (pi.CanCastingTo<IEnumerable<PObject>>())
                        {
                            XmlElement lstXml = CreateElement(doc,
                                $"{child.Parent.Attribute.Name}.{child.Attribute.Name}", child.Parent);

                            xml.AppendChild(lstXml);

                            if (child.HasChildren)
                            {
                                foreach (var item in child.Children?.Reverse())
                                {
                                    XmlElement childXml = CreateElement(doc, item);
                                    lstXml.AppendChild(childXml);

                                    SetXamlName(childXml, item);

                                    comQueue.Enqueue(item);
                                    xmlQueue.Enqueue(childXml);
                                }
                            }
                        }
                        else
                        {
                            bool isContinue = false;

                            // Null Reference
                            isContinue |= (value == null);

                            // Auto Size Pass
                            isContinue |= (value is double && double.IsNaN((double)value));
                            
                            if (!isContinue)
                            {
                                if (child.HasResource)
                                {
                                    xml.SetAttribute(
                                        child.Attribute.Name,
                                        GetResourceName(child.ResourceType.Value, value.ToString()));
                                }
                                else
                                {
                                    xml.SetAttribute(
                                        child.Attribute.Name,
                                        ValueToXamlInline(value));
                                }
                            }
                        }
                    }

                    // Content
                    if (content != null && content.HasChildren)
                    {
                        var pi = content.Element as PropertyInfo;

                        if (pi.CanCastingTo<IEnumerable<PObject>>())
                        {
                            foreach (var item in content.Children?.Reverse())
                            {
                                XmlElement contentXml = CreateElement(doc, item);
                                xml.AppendChild(contentXml);

                                SetXamlName(contentXml, item);

                                comQueue.Enqueue(item);
                                xmlQueue.Enqueue(contentXml);
                            }
                        }
                        else if (pi.CanCastingTo<PObject>())
                        {
                            var contentItem = content.Children[0];
                            XmlElement contentXml = CreateElement(doc, contentItem);
                            xml.AppendChild(contentXml);

                            SetXamlName(contentXml, contentItem);

                            comQueue.Enqueue(contentItem);
                            xmlQueue.Enqueue(contentXml);
                        }
                    }
                }
            }

            doc.AppendChild(rootElement);

            // �ε�Ʈ ó��
            using (var ms = new MemoryStream())
            {
                using (var writer = new XmlTextWriter(ms, new UTF8Encoding(false)))
                {
                    writer.Formatting = Formatting.Indented;
                    doc.Save(writer);

                    yield return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }

        private void ImagePatch(CodeComponent<WPFAttribute>[] items)
        {
            // �Ӽ��� �˻�
            foreach (var item in items.Where(c => c.ElementType == CodeComponentType.Property))
            {
                // DX ���ҽ� Ư���� �ִ°��
                if (item.HasResource && item.ResourceType == ResourceType.Image)
                {
                    var pi = item.Element as PropertyInfo;

                    string value = (string)pi.GetValue(item.Parent.Element);

                    if (File.Exists(value))
                    {
                        // ���ҽ� �߰�
                        this.Images.Add(value);
                    }
                }
            }
        }

        private void NamespacePatch(CodeComponent<WPFAttribute>[] items)
        {
            int idx = 1;

            foreach (string n in items
                .Select(item => item.Attribute.Namespace)
                .Where(ns => ns != null && ns.StartsWith("clr-namespace"))
                .Distinct())
            {
                this.NamespaceContainer[n] = $"cns{idx++}";
            }
        }

        private void NamingPatch(IEnumerable<CodeComponent<WPFAttribute>> components)
        {
            foreach (var component in components
                .Where(c => c.ElementType == CodeComponentType.Instance))
            {
                this.NameContainer.Add(component.Element);
            }
        }

        private string GetResourceName(ResourceType resourceType, string resourceFileName)
        {
            if (resourceType == ResourceType.Image)
                return $"pack://application:,,,/{Manifest.ApplicationName};component/{Path.GetFileName(resourceFileName)}";

            return null;
        }

        private XmlElement CreateElement(XmlDocument doc, CodeComponent<WPFAttribute> component)
        {
            return CreateElement(doc, component.Attribute.Name, component);
        }

        private XmlElement CreateElement(XmlDocument doc, string name, CodeComponent<WPFAttribute> component)
        {
            if (this.NamespaceContainer.ContainsKey(component.Attribute.Namespace))
            {
                string prefix = this.NamespaceContainer[component.Attribute.Namespace];

                return doc.CreateElement(prefix, name, component.Attribute.Namespace);
            }
            else
            {
                return doc.CreateElement(name, XMLNS);
            }
        }

        private void SetXamlName(XmlElement element, CodeComponent<WPFAttribute> component)
        {
            if (component.ElementType == CodeComponentType.Instance)
            {
                var pObj = (PObject)component.Element;

                if (this.NameContainer[pObj] == null)
                {

                }
                element.SetAttribute("Name", this.NameContainer[pObj]);
            }
        }

        private string ValueToXamlInline(object value)
        {
            if (value == null)
                return "{x:Null}";
            
            if (value.HasAttribute<WPFAttribute>())
            {
                var attr = value.GetAttribute<WPFAttribute>();

                return attr.Name;
            }

            return value.ToString();
        }
    }
}