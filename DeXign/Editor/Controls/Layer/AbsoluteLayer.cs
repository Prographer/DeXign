using System.Windows;

using DeXign.Editor.Controls;

namespace DeXign.Editor.Layer
{
    public class AbsoluteLayer : ControlLayer
    {
        public AbsoluteLayer(UIElement targetElement) : base(targetElement)
        {
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (FrameworkElement element in InternalChildren)
            {
                double x = Storyboard.GetLeft(element);
                double y = Storyboard.GetTop(element);

                x = double.IsNaN(x) ? 0 : x;
                y = double.IsNaN(y) ? 0 : y;

                element.Arrange(
                    new Rect(x, y, element.Width, element.Height));
            }

            return finalSize;
        }
    }
}
