﻿using System;
using System.Reflection;

using DeXign.Core;
using DeXign.Core.Designer;
using DeXign.Editor;
using DeXign.Extension;
using DeXign.Editor.Logic;
using DeXign.Core.Logic;

namespace DeXign.Models
{
    public class ComponentBoxItemModel
    {
        public string Title { get; }

        public string Category { get; }

        public ComponentType ComponentType { get; }

        public Type DataModelType { get; }
        public object Data { get; }

        // 객체 선택기
        internal ComponentBoxItemModel(IRenderer renderer)
        {
            var modelAttr = renderer.Model.GetAttribute<DesignElementAttribute>();

            string name = renderer.Model.Name;

            if (string.IsNullOrEmpty(name))
                name = "<이름 없음>";

            this.Title = $"{name} ({modelAttr.DisplayName})";
            this.ComponentType = ComponentType.Instance;
            this.Category = "레이아웃 객체";

            this.Data = renderer.Model;
            this.DataModelType = renderer.Model.GetType();
        }

        internal ComponentBoxItemModel(DesignElementAttribute attr)
        {
            this.Title = attr.DisplayName;
            this.Category = attr.Category;
        }

        internal ComponentBoxItemModel(AttributeTuple<DesignElementAttribute, EventInfo> data) 
            : this(data.Attribute)
        {
            this.ComponentType = ComponentType.Event;
            this.Data = data.Element;
            this.DataModelType = typeof(PTrigger);
        }

        internal ComponentBoxItemModel(AttributeTuple<DesignElementAttribute, Type> data)
            : this(data.Attribute)
        {
            this.ComponentType = ComponentType.Component;
            this.Data = data.Element;
            this.DataModelType = data.Element;
        }
    }
}
