using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using DeXign.Core;
using DeXign.Core.Logic;
using DeXign.Editor.Layer;
using DeXign.Extension;
using DeXign.Editor.Logic;

namespace DeXign.Editor.Renderer
{
    public class StoryboardRenderer : StoryboardLayer, IRenderer, IStoryboard
    {
        public event EventHandler ElementAttached;

        public FrameworkElement Element => (FrameworkElement)AdornedElement;

        public PObject Model { get; set; }

        public IRenderer RendererParent { get; }

        public IList<IRenderer> RendererChildren { get; }

        public RendererMetadata Metadata { get; }

        public StoryboardRenderer(UIElement adornedElement) : base(adornedElement)
        {
            this.Metadata = new RendererMetadata();

            this.RendererChildren = new List<IRenderer>();
        }

        public void AddChild(IRenderer child, Point position)
        {
            this.RendererChildren.SafeAdd(child);

            OnAddedChild(child, position);
        }
        
        public void RemoveChild(IRenderer child)
        {
            this.RendererChildren.SafeRemove(child);

            OnRemovedChild(child);
        }

        protected virtual void OnAddedChild(IRenderer child, Point position)
        {
            if (child is ScreenRenderer)
            {
                child.Element.Margin = new Thickness(0);
                child.Element.VerticalAlignment = VerticalAlignment.Top;
                child.Element.HorizontalAlignment = HorizontalAlignment.Left;

                child.Element.Width = 360;
                child.Element.Height = 615;

                var screenSize = (Element.Parent as FrameworkElement).RenderSize;

                Canvas.SetTop(child.Element, screenSize.Height / 2 - child.Element.Height / 2);
                Canvas.SetLeft(child.Element, screenSize.Width / 2 - child.Element.Width / 2);
                Canvas.SetZIndex(child.Element, 10);
            }

            if (child.Model is PComponent)
            {
                var element = child.Element as ComponentElement;

                // Add Visual
                element.Margin = new Thickness(0);
                element.VerticalAlignment = VerticalAlignment.Top;
                element.HorizontalAlignment = HorizontalAlignment.Left;

                Canvas.SetLeft(element, element.SnapToGrid(position.X));
                Canvas.SetTop(element, element.SnapToGrid(position.Y));
                Canvas.SetZIndex(element, 0);
            }
        }

        protected virtual void OnRemovedChild(IRenderer child)
        {
        }

        protected override void OnLoaded(FrameworkElement adornedElement)
        {
            base.OnLoaded(adornedElement);

            ElementAttached?.Invoke(this, EventArgs.Empty);
        }

        #region [ IBinderProvider ]
        public IBinderHost ProvideValue()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
