using System;
using System.Windows;

using DeXign.Core;
using DeXign.Core.Designer;
using DeXign.Controls;
using System.Reflection;

namespace DeXign.Models
{
    internal class PropertyGridItemModel
    {
        internal AttributeTuple<DesignElementAttribute, PropertyInfo> Metadata { get; }

        public string Title => Metadata.Attribute.DisplayName;

        public string Category => Metadata.Attribute.Category;

        public ISetter Setter { get; }

        public PropertyGridItemModel(AttributeTuple<DesignElementAttribute, PropertyInfo> data, ISetter setter)
        {
            this.Metadata = data;
            this.Setter = setter;
        }
    }
}
