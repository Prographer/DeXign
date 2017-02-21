using System;
using System.Windows;
using System.Windows.Media;

using DeXign.Core.Controls;
using DeXign.Editor;
using DeXign.Editor.Layer;
using DeXign.Extension;
using DeXign.Resources;

using WPFExtension;
using DeXign.Converter;
using DeXign.Core;
using System.Windows.Controls;

namespace DeXign.Editor.Renderer
{
    public class LayerRenderer<TModel, TElement> : DropSelectionLayer, IRenderer<TModel, TElement>
        where TModel : PObject
        where TElement : FrameworkElement
    {
        static EnumToEnumConverter<HorizontalAlignment, PHorizontalAlignment> hConverter;
        static EnumToEnumConverter<VerticalAlignment, PVerticalAlignment> vConverter;

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
        
        static LayerRenderer()
        {
            hConverter = new EnumToEnumConverter<HorizontalAlignment, PHorizontalAlignment>();
            vConverter = new EnumToEnumConverter<VerticalAlignment, PVerticalAlignment>();
        }

        public LayerRenderer(TElement adornedElement, TModel model) : base(adornedElement)
        {
            this.Model = model;
            this.Element = adornedElement;
        }

        protected override void OnLoaded(FrameworkElement adornedElement)
        {
            base.OnLoaded(adornedElement);

            // Platform Style
            string styleName = OnLoadPlatformStyleName();

            if (!string.IsNullOrWhiteSpace(styleName))
                this.Element.SetResourceReference(StyleProperty, styleName);

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
            var visual = element as FrameworkElement;

            // Default Binding
            #region < PVisual >
            if (Model is PVisual)
            {
                var model = Model as PVisual;

                // RenderTransformOrigin Binding
                FrameworkElement.RenderTransformOriginProperty.AddValueChanged(
                    visual, (s, e) =>
                    {
                        model.AnchorX = visual.RenderTransformOrigin.X;
                        model.AnchorY = visual.RenderTransformOrigin.Y;
                    });

                // Background Binding
                BindingEx.TryBinding(
                    visual, "Background",
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
                    model, PVisual.MinHeightProperty,
                    visual, FrameworkElement.MinHeightProperty);

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
            #endregion

            #region < PControl >
            if (Model is PControl)
            {
                var model = Model as PControl;

                BindingEx.SetBinding(
                    visual, FrameworkElement.MarginProperty,
                    model, PControl.MarginProperty);

                BindingEx.SetBinding(
                    visual, FrameworkElement.VerticalAlignmentProperty,
                    model, PControl.VerticalAlignmentProperty,
                    converter: vConverter);

                BindingEx.SetBinding(
                    visual, FrameworkElement.HorizontalAlignmentProperty,
                    model, PControl.HorizontalAlignmentProperty,
                    converter: hConverter);
            }
            #endregion

            #region < PLayout >
            if (Model is PLayout)
            {
                var model = Model as PLayout;

                BindingEx.TryBinding(
                    model, PLayout.PaddingProperty,
                    visual, "Padding");
            }
            #endregion
        }

        public virtual void OnAddedChild(IRenderer child)
        {
        }

        public virtual void OnRemovedChild(IRenderer child)
        {
        }
    }
}