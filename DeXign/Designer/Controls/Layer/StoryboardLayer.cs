using System.Linq;
using System.Windows;
using System.Windows.Media;
using DeXign.Designer.Controls;
using DeXign.Extension;

namespace DeXign.Designer.Layer
{
    class StoryboardLayer : ControlLayer
    {
        internal new Storyboard Parent;
        internal ScaleTransform ParentScale;

        internal double ScaleX => ParentScale.ScaleX;
        internal double ScaleY => ParentScale.ScaleY;

        public StoryboardLayer(UIElement adornedElement) : base(adornedElement)
        {
            this.Parent = adornedElement
                .FindParents<Storyboard>()
                .FirstOrDefault();

            this.ParentScale = Parent?.RenderTransform as ScaleTransform;
        }
    }
}