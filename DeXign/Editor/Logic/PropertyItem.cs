using System.Windows;
using System.Reflection;

using DeXign.Core;
using DeXign.Core.Designer;
using DeXign.Extension;

namespace DeXign.Editor.Logic
{
    public class PropertyItem
    {
        public DesignElementAttribute Metadata { get; }
        public DependencyProperty Property { get; }

        public string Title => Metadata.DisplayName;

        public PropertyItem(AttributeTuple<DesignElementAttribute, PropertyInfo> data)
        {
            this.Metadata = data.Attribute;
            this.Property = data.Element.GetDependencyProperty();
        }
    }
}
