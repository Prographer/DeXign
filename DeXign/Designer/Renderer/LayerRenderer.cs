using System;
using System.Windows;
using DeXign.Core.Controls;
using DeXign.Designer.Layer;

namespace DeXign.Designer.Renderer
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

            Model = Activator.CreateInstance<TModel>();

            this.Element = (TElement)adornedElement;
            OnElementAttached(this.Element);
        }

        protected virtual void OnElementAttached(TElement element)
        {
        }
    }
}