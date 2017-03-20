using System;
using System.Reflection;

using DeXign.Core;
using DeXign.Core.Logic;
using DeXign.Core.Designer;
using DeXign.Editor;
using DeXign.Editor.Logic;
using DeXign.Extension;

namespace DeXign.Models
{
    public class ComponentBoxItemModel
    {
        public string Title { get; }

        public string Category { get; }

        public ComponentType ComponentType { get; }

        public Type DataModelType { get; } // PVisual

        public Type ItemModelType { get; } // PComponent or PVisual

        public object Data { get; }

        internal ComponentBoxItemModel(DesignElementAttribute attr)
        {
            this.Title = attr.DisplayName;
            this.Category = attr.Category;
        }

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
            this.ItemModelType = typeof(PSelector);
        }

        // 함수
        internal ComponentBoxItemModel(AttributeTuple<DesignElementAttribute, MethodInfo> data)
            : this(data.Attribute)
        {
            this.ComponentType = ComponentType.Function;
            this.Data = data.Element;
            this.ItemModelType = typeof(PFunction);
            this.DataModelType = typeof(PFunction);
        }

        // 트리거
        internal ComponentBoxItemModel(AttributeTuple<DesignElementAttribute, EventInfo> data) 
            : this(data.Attribute)
        {
            this.ComponentType = ComponentType.Event;
            this.Data = data.Element;
            this.ItemModelType = typeof(PTrigger);
            this.DataModelType = typeof(PTrigger);
        }

        // 도구상자 -> 줌패널
        internal ComponentBoxItemModel(ItemDropRequest request)
        {
            this.ComponentType = ComponentType.Component;
            this.Data = request.Data;
            this.ItemModelType = request.ItemType;
            this.DataModelType = request.ItemType;

            if (request.ItemType.CanCastingTo<PFunction>())
                this.ComponentType = ComponentType.Function;

            if (request.ItemType.CanCastingTo<PComponent>())
            {
                var attr = request.ItemType.GetAttribute<DesignElementAttribute>();

                this.Title = attr.DisplayName;
                this.Category = attr.Category;
            }
        }
    }
}
