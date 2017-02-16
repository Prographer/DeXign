using System;
using System.Windows;
using System.Windows.Media;

using DeXign.Editor.Layer;
using DeXign.Editor.Interfaces;
using DeXign.Extension;
using DeXign.Core.Controls;

using WPFExtension;
using System.Windows.Controls;

namespace DeXign.Editor.Renderer
{
    public class LayerRenderer<TModel, TElement> : DropSelectionLayer, IRenderer<TModel, TElement>
        where TModel : PObject
        where TElement : FrameworkElement
    {
        #region [ IRenderer Interface ]
        FrameworkElement IRenderer.Element => Element;

        PObject IRenderer.Model
        {
            get { return Model; }
            set { Model = (TModel)value; }
        }
        #endregion

        #region [ IRenderer<> Interface ]
        public TElement Element { get; }

        public TModel Model { get; set; }
        #endregion

        public LayerRenderer(UIElement adornedElement) : base(adornedElement)
        {
            if (!(adornedElement is TElement))
                throw new ArgumentException();
            
            this.Model = (TModel)AdornedElement.DataContext;
            this.Element = (TElement)adornedElement;

            // Platform Style
            string styleName = OnLoadPlatformStyleName();

            if (!string.IsNullOrWhiteSpace(styleName))
                this.Element.SetResourceReference(StyleProperty, styleName);

            OnElementAttached(this.Element);
        }
        
        public LayerRenderer(TElement adornedElement, TModel model) : base(adornedElement)
        {
            this.Model = model;
            this.Element = adornedElement;

            OnElementAttached(this.Element);
        }

        public bool IsContentParent()
        {
            var parent = (FrameworkElement)Element.Parent;

            return (parent.DataContext is PPage);
        }

        protected virtual string OnLoadPlatformStyleName()
        {
            return null;
        }

        protected virtual void OnElementAttached(TElement element)
        {
            // Default Binding
            if (Model is PVisual && element is FrameworkElement)
            {
                var model = Model as PVisual;
                var visual = element as FrameworkElement;

                // RenderTransformOrigin Binding
                FrameworkElement.RenderTransformOriginProperty.AddValueChanged(
                    visual, (s, e) =>
                    {
                        model.AnchorX = visual.RenderTransformOrigin.X;
                        model.AnchorY = visual.RenderTransformOrigin.Y;
                    });

                // Background Binding
                var backgroundProperty = visual.FindDependencyProperty("Background");

                if (backgroundProperty != null)
                    BindingEx.SetBinding(
                        visual, backgroundProperty,
                        model, PVisual.BackgroundProperty);

                // Size Binding
                BindingEx.SetBinding(
                    visual, FrameworkElement.WidthProperty,
                    model, PVisual.WidthProperty);

                BindingEx.SetBinding(
                    visual, FrameworkElement.HeightProperty,
                    model, PVisual.HeightProperty);

                // Min Size Binding
                BindingEx.SetBinding(
                    visual, FrameworkElement.MinWidthProperty,
                    model, PVisual.MinWidthProperty);

                BindingEx.SetBinding(
                    visual, FrameworkElement.MinHeightProperty,
                    model, PVisual.MinHeightProperty);

                // Opacity Binding
                BindingEx.SetBinding(
                    visual, FrameworkElement.OpacityProperty,
                    model, PVisual.OpacityProperty);

                // * Transform Binding
                // X, Y Binding
                TranslateTransform translate;
                visual.RenderTransform = new TransformGroup()
                {
                    Children =
                    {
                        (translate = new TranslateTransform())
                    }
                };

                BindingEx.SetBinding(
                    translate, TranslateTransform.XProperty,
                    model, PVisual.XProperty);

                BindingEx.SetBinding(
                    translate, TranslateTransform.YProperty,
                    model, PVisual.YProperty);

                // TODO: Rotation
                // TODO: RotationX
                // TODO: RotationY
            }
        }
    }
}