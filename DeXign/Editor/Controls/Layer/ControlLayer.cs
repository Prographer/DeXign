using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

using WPFExtension;

namespace DeXign.Editor.Layer
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

        protected VisualCollection InternalChildren;

        protected override int VisualChildrenCount
        {
            get { return InternalChildren.Count; }
        }

        public ControlLayer(UIElement targetElement) : base(targetElement)
        {
            InternalChildren = new VisualCollection(this);

            SetValue(ChildrenPropertyKey, InternalChildren);
        }

        public void Add(FrameworkElement element)
        {
            InternalChildren.Add(element);
        }

        public void Remove(FrameworkElement element)
        {
            InternalChildren.Remove(element);
        }

        protected override Visual GetVisualChild(int index)
        {
            return InternalChildren[index];
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (FrameworkElement element in InternalChildren)
                element.Arrange(
                    new Rect(0, 0, AdornedElement.RenderSize.Width, AdornedElement.RenderSize.Height));

            return finalSize;
        }
    }

}
