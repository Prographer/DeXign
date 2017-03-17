using System;
using System.Collections.Generic;
using System.Reflection;

namespace DeXign.Core
{
    public class CodeComponent<TAttribute> 
        where TAttribute : Attribute
    {
        // Element의 정의된 Attribute
        public TAttribute Attribute { get; }

        // 대상
        public object Element { get; }

        // Element Type
        public CodeComponentType ElementType { get; }

        // 노드 부모
        public CodeComponent<TAttribute> Parent { get; set; }

        // 자식
        public IList<CodeComponent<TAttribute>> Children { get; set; }

        // 노드 깊이 (인덴트에 주로 많이쓸듯)
        public int Depth { get; set; }

        public bool HasChildren
        {
            get { return Children != null && Children.Count > 0; }
        }

        public CodeComponent(object element, TAttribute attribute)
        {
            this.Element = element;
            this.Attribute = attribute;

            if (Attribute == null)
                throw new ArgumentException();

            if (this.Element is PropertyInfo)
                this.ElementType = CodeComponentType.Property;
            else
                this.ElementType = CodeComponentType.Instance;
        }

        public void Add(CodeComponent<TAttribute> component)
        {
            if (Children == null)
                Children = new List<CodeComponent<TAttribute>>();

            component.Parent = this;
            
            Children.Add(component);
        }

        public void Remove(CodeComponent<TAttribute> component)
        {
            if (Children == null)
                return;

            Children.Remove(component);
        }
    }
}