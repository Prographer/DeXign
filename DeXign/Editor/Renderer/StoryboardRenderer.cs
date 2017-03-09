using System;
using System.Collections.Generic;
using System.Windows;

using DeXign.Core;
using DeXign.Core.Logic;
using DeXign.Editor.Layer;
using DeXign.Extension;
using System.Windows.Controls;

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

                Canvas.SetTop(child.Element, 80);
                Canvas.SetLeft(child.Element, 80);
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
        public bool CanBind(BaseBinder outputBinder, BinderOptions options)
        {
            throw new NotImplementedException();
        }

        public void Bind(BaseBinder outputBinder, BinderOptions options)
        {
            throw new NotImplementedException();
        }

        public void ReleaseInput(BaseBinder outputBinder)
        {
            throw new NotImplementedException();
        }

        public void ReleaseOutput(BaseBinder inputBinder)
        {
            throw new NotImplementedException();
        }

        public void ReleaseAll()
        {
            throw new NotImplementedException();
        }

        public BaseBinder ProvideValue()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
