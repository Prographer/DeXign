using System;
using System.Windows;
using DeXign.Core.Controls;
using DeXign.Editor.Layer;

namespace DeXign.Editor.Renderer
{
    class LayerRenderer<TModel, TElement> : SelectionLayer
        where TModel : PObject
        where TElement : FrameworkElement
    {
        public TElement Element { get; }
        public TModel Model { get; set; }

        public LayerRenderer(UIElement adornedElement) : base(adornedElement)
        {
            if (!(adornedElement is TElement))
                throw new ArgumentException();

            this.Model = Activator.CreateInstance<TModel>();
            this.Element = (TElement)adornedElement;

            OnElementAttached(this.Element);
        }

        public LayerRenderer(TElement adornedElement, TModel model) : base(adornedElement)
        {
            this.Model = model;
            this.Element = adornedElement;

            OnElementAttached(this.Element);
        }

        protected virtual void OnElementAttached(TElement element)
        {
        }
    }
}