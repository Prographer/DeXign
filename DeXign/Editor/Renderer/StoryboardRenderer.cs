using System.Windows;
using DeXign.Editor.Layer;
using DeXign.Core.Controls;

namespace DeXign.Editor.Renderer
{
    class StoryboardRenderer : StoryboardLayer, IRenderer, IStoryboard
    {
        public FrameworkElement Element => (FrameworkElement)AdornedElement;

        public PObject Model { get; set; }

        public StoryboardRenderer(UIElement adornedElement) : base(adornedElement)
        {
        }

        public void OnAddedChild(IRenderer child)
        {
        }

        public void OnRemovedChild(IRenderer child)
        {
        }
    }
}
