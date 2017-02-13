using System.Linq;
using System.Windows;
using System.Windows.Media;
using DeXign.Editor.Controls;
using DeXign.Extension;

namespace DeXign.Editor.Layer
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
                .FindLogicalParents<Storyboard>()
                .FirstOrDefault();

            this.ParentScale = Parent?.RenderTransform as ScaleTransform;
        }
    }
}