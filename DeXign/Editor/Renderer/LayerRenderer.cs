using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;

using DeXign.Core;
using DeXign.Converter;
using DeXign.Core.Controls;
using DeXign.Extension;
using DeXign.Editor.Layer;

using WPFExtension;

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

        #region [ Local Variable ]
        private bool showModelName = false;
        private string displayTypeName = "";
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

            // �̸� ����
            var attr = model.GetAttribute<DesignElementAttribute>();
            if (attr != null)
                displayTypeName = attr.DisplayName;
        }
        
        protected override void OnDisposed()
        {
            VisualContentHelper.GetContent(
                AdornedElement,
                null,
                list =>
                {
                    foreach (FrameworkElement element in list)
                    {
                        IRenderer renderer = element.GetRenderer();
                        if (renderer is IDisposable)
                            (renderer as IDisposable).Dispose();
                    }
                });

            base.OnDisposed();
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
        
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (showModelName)
            {
                double blank = 4 / ScaleX;
                double opacity = 8;
                string name = Model.Name;
                SolidColorBrush brush = Brushes.Black;

                if (string.IsNullOrEmpty(name))
                {
                    brush = Brushes.LightSlateGray;
                    name = "<�̸� ����>";
                    opacity = 0.56;
                }

                name = $"{name} ({displayTypeName})";

                FormattedText text = CreateFormattedText(name, 11, "���� ����", brush);

                var position = new Point(blank, -text.Height - blank);
                var bound = new Rect(position, new Size(text.Width, text.Height));

                Inflate(ref bound, blank, blank);

                dc.PushOpacity(opacity);
                dc.DrawRectangle(Brushes.White, null, bound);
                dc.Pop();

                dc.PushOpacity(0.8);
                dc.DrawText(text, position);
                dc.Pop();
            }
        }

        protected override void OnDesignModeChanged()
        {
            showModelName &= (DesignMode == DesignMode.None);

            base.OnDesignModeChanged();
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            if (DesignMode == DesignMode.None)
                showModelName = true;
        
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            showModelName = false;

            base.OnMouseLeave(e);
        }
    }
}