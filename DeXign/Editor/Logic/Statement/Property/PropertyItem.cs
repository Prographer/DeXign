using DeXign.Core;
using DeXign.Core.Designer;
using DeXign.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
