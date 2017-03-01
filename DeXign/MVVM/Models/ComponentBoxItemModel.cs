using System;
using System.Reflection;

using DeXign.Core;
using DeXign.Core.Designer;

namespace DeXign.Models
{
    public class ComponentBoxItemModel
    {
        public string Title { get; }

        public string Category { get; }

        public ComponentType ComponentType { get; }

        public object Data { get; }

        internal ComponentBoxItemModel(DesignElementAttribute attr)
        {
            this.Title = attr.DisplayName;
            this.Category = attr.Category;
        }

        public ComponentBoxItemModel(AttributeTuple<DesignElementAttribute, EventInfo> data) 
            : this(data.Attribute)
        {
            this.ComponentType = ComponentType.Event;
            Data = data.Element;
        }

        public ComponentBoxItemModel(AttributeTuple<DesignElementAttribute, Type> data)
            : this(data.Attribute)
        {
            ComponentType = ComponentType.Instance;
            Data = data.Element;
        }
    }
}
