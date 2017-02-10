using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using WPFExtension;

namespace DeXign.Designer.Layer
{
    [ContentProperty("Children")]
    public class ControlLayer : Adorner
    {
        private static readonly DependencyPropertyKey ChildrenPropertyKey =
            DependencyHelper.RegisterReadOnly();

        public VisualCollection Children
        {
            get { return (VisualCollection)GetValue(ChildrenPropertyKey.DependencyProperty); }
        }

        VisualCollection visualChildren;

        protected override int VisualChildrenCount
        {
            get { return visualChildren.Count; }
        }

        public ControlLayer(UIElement targetElement) : base(targetElement)
        {
            visualChildren = new VisualCollection(this);

            SetValue(ChildrenPropertyKey, visualChildren);
        }

        public void Add(FrameworkElement element)
        {
            visualChildren.Add(element);
        }

        public void Remove(FrameworkElement element)
        {
            visualChildren.Remove(element);
        }

        protected override Visual GetVisualChild(int index)
        {
            return visualChildren[index];
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (FrameworkElement element in visualChildren)
                element.Arrange(
                    new Rect(0, 0, AdornedElement.DesiredSize.Width, AdornedElement.DesiredSize.Height));

            return finalSize;
        }
    }

}
