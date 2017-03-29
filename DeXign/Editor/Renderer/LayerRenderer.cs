using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Collections.Generic;

using DeXign.Core;
using DeXign.Core.Logic;
using DeXign.Core.Controls;
using DeXign.Editor.Layer;
using DeXign.Editor.Logic;
using DeXign.Extension;
using DeXign.Converter;

using WPFExtension;
using System.Windows.Controls;
using DeXign.Task;

namespace DeXign.Editor.Renderer
{
    public class LayerRenderer<TModel, TElement> : DropSelectionLayer,
        IRenderer<TModel, TElement>, IRendererElement, IUISupport, IMovable
        where TModel : PVisual
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

        #region [ Event ]
        public event EventHandler ElementAttached;
        public event EventHandler Moved;
        #endregion

        #region [ Property ]
        public IList<IRenderer> RendererChildren { get; }

        public IRenderer RendererParent => this.Parent;

        public bool IsElementAttached { get; private set; }

        public RendererMetadata Metadata { get; private set; }
        #endregion

        #region [ Local Variable ]
        private Thickness beginMargin;
        private Point beginPosition;
        private Size beginSize;

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
            this.Metadata = new RendererMetadata();

            this.Model = model;
            this.Element = adornedElement;

            // 렌더러 설정
            this.Model.Binder.SetRenderer(this);

            this.RendererChildren = new List<IRenderer>();
            
            // 이름 설정
            var attr = model.GetAttribute<DesignElementAttribute>();
            if (attr != null)
                displayTypeName = attr.DisplayName;

            // Binder
            RendererManager.ResolveBinder(this).Released += LayerRenderer_Released;
        }

        private void LayerRenderer_Released(object sender, BinderBindedEventArgs e)
        {
            var outputBinder = e.Expression.Output as PBinder;
            var inputBinder = e.Expression.Input as PBinder;
            
            Storyboard.DisconnectComponentLine(
                outputBinder.GetView<BindThumb>(), inputBinder.GetView<BindThumb>());
        }

        protected override void OnDisposed()
        {
            ObjectContentHelper.GetContent(
                AdornedElement,
                null,
                list =>
                {
                    foreach (FrameworkElement element in list)
                    {
                        IRenderer renderer = element.GetRenderer();
                        if (renderer is IDisposable disposable)
                            disposable.Dispose();
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

            IsElementAttached = true;
            ElementAttached?.Invoke(this, EventArgs.Empty);
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

        protected virtual void OnElementDettached(TElement element)
        {
        }

        protected virtual void OnElementAttached(TElement element)
        {
            var visual = element as FrameworkElement;

            // Default Binding
            #region < PVisual >
            if (Model is PVisual model)
            {
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
                    Model, PVisual.BackgroundProperty);

                // Size Binding
                BindingEx.SetBinding(
                    visual, FrameworkElement.WidthProperty,
                    Model, PVisual.WidthProperty);

                BindingEx.SetBinding(
                    visual, FrameworkElement.HeightProperty,
                    Model, PVisual.HeightProperty);

                // Min Size Binding
                BindingEx.SetBinding(
                    visual, FrameworkElement.MinWidthProperty,
                    Model, PVisual.MinWidthProperty);

                BindingEx.SetBinding(
                    Model, PVisual.MinHeightProperty,
                    visual, FrameworkElement.MinHeightProperty);

                // Opacity Binding
                BindingEx.SetBinding(
                    visual, FrameworkElement.OpacityProperty,
                    Model, PVisual.OpacityProperty);

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
                    Model, PVisual.XProperty);

                BindingEx.SetBinding(
                    translate, TranslateTransform.YProperty,
                    Model, PVisual.YProperty);

                // TODO: Rotation
                // TODO: RotationX
                // TODO: RotationY
            }
            #endregion

            #region < PControl >
            if (Model is PControl)
            {
                BindingEx.SetBinding(
                    visual, FrameworkElement.MarginProperty,
                    Model, PControl.MarginProperty);

                BindingEx.SetBinding(
                    visual, FrameworkElement.VerticalAlignmentProperty,
                    Model, PControl.VerticalAlignmentProperty,
                    converter: vConverter);

                BindingEx.SetBinding(
                    visual, FrameworkElement.HorizontalAlignmentProperty,
                    Model, PControl.HorizontalAlignmentProperty,
                    converter: hConverter);
            }
            #endregion

            #region < PLayout >
            if (Model is PLayout)
            {
                BindingEx.TryBinding(
                    Model, PLayout.PaddingProperty,
                    visual, "Padding");
            }
            #endregion

            #region < PPage >
            if (Model is PPage)
            {
                BindingEx.TryBinding(
                    Model, PPage.PaddingProperty,
                    visual, "Padding");
            }
            #endregion
        }

        public void AddChild(IRenderer child, Point position)
        {
            this.RendererChildren.SafeAdd(child);

            if (!DesignTime.IsLocked(this))
                OnAddedChild(child, position);
        }

        public void RemoveChild(IRenderer child)
        {
            this.RendererChildren.SafeRemove(child);

            if (!DesignTime.IsLocked(this))
                OnRemovedChild(child);
        }

        protected virtual void OnAddedChild(IRenderer child, Point position)
        { 
        }

        protected virtual void OnRemovedChild(IRenderer child)
        {
        }

        protected override void OnMoved()
        {
            base.OnMoved();

            Moved?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnDragStarted()
        {
            beginMargin = this.Element.Margin;
            beginPosition = new Point(Canvas.GetLeft(this.Element), Canvas.GetTop(this.Element));
        }

        protected override void OnDragCompleted()
        {
            Thickness undoMargin = beginMargin;
            Thickness doMargin = this.Element.Margin;

            Point undoPosition = beginPosition;
            Point doPosition = new Point(Canvas.GetLeft(this.Element), Canvas.GetTop(this.Element));
            
            // 아무 작업 없음
            if (undoPosition.Equals(doPosition) && 
                undoMargin.Equals(doMargin))
                return;

            // 이동 태스크
            this.Storyboard.TaskManager.Push(
                new TaskData(this,
                () =>
                {
                    this.Element.Margin = doMargin;
                    Canvas.SetLeft(this.Element, doPosition.X);
                    Canvas.SetTop(this.Element, doPosition.Y);
                },
                () =>
                {
                    this.Element.Margin = undoMargin;
                    Canvas.SetLeft(this.Element, undoPosition.X);
                    Canvas.SetTop(this.Element, undoPosition.Y);
                }));

            foreach (IRenderer componentRenderer in this.Storyboard.Components.Select(c => c.GetRenderer()))
            {
                (componentRenderer as StoryboardLayer).InvalidateVisual();
            }
        }

        protected override void OnSizingStarted()
        {
            beginSize = new Size(this.Model.Width, this.Model.Height);
            
            OnDragStarted();
        }

        protected override void OnSizingCompleted()
        {
            Size undoSize = beginSize;
            Size doSize = new Size(this.Model.Width, this.Model.Height);

            Thickness undoMargin = beginMargin;
            Thickness doMargin = this.Element.Margin;

            Point undoPosition = beginPosition;
            Point doPosition = new Point(Canvas.GetLeft(this.Element), Canvas.GetTop(this.Element));

            // 아무 작업 없음
            if (undoSize.Equals(doSize) && 
                undoMargin.Equals(doMargin) && 
                undoPosition.Equals(doPosition))
                return;

            // 이동 태스크
            this.Storyboard.TaskManager.Push(
                new TaskData(this,
                () =>
                {
                    this.SetSize(doSize.Width, doSize.Height);

                    this.Element.Margin = doMargin;
                    Canvas.SetLeft(this.Element, doPosition.X);
                    Canvas.SetTop(this.Element, doPosition.Y);
                },
                () =>
                {
                    this.SetSize(undoSize.Width, undoSize.Height);

                    this.Element.Margin = undoMargin;
                    Canvas.SetLeft(this.Element, undoPosition.X);
                    Canvas.SetTop(this.Element, undoPosition.Y);
                }));
        }

        private IEnumerable<IRenderer> GetOverlappedComponentRenderers()
        {
            Rect bound = GetBound();

            foreach (IRenderer r in this.Storyboard.Components.Select(c => c.GetRenderer()))
            {
                if (r is IUISupport support)
                {
                    if (bound.IntersectsWith(support.GetBound()))
                    {
                        yield return r;
                    }
                }
            }
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            foreach (IRenderer componentRenderer in GetOverlappedComponentRenderers())
            {
                (componentRenderer as StoryboardLayer).InvalidateVisual();
            }

            if (showModelName)
            {
                double blank = 4;
                double opacity = 8;
                string name = Model.Name;
                var brush = Brushes.Black;

                if (string.IsNullOrEmpty(name))
                {
                    brush = Brushes.LightSlateGray;
                    name = "<이름 없음>";
                    opacity = 0.56;
                }

                name = $"{name} ({displayTypeName})";

                FormattedText text = CreateFormattedText(name, 11, "맑은 고딕", brush);

                var position = new Point(this.Fit(blank), -text.Height - this.Fit(blank));
                var bound = new Rect(position, new Size(text.Width, text.Height));

                this.InflateFit(ref bound, blank, blank);

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

        #region [ IBinderProvider Interface ]
        public IBinderHost ProvideValue()
        {
            return Model.Binder;
        }
        #endregion

        #region [ IUISupport ]
        public Point GetLocation()
        {
            FrameworkElement element = this.Element;

            if (DesignMode == DesignMode.Trigger)
                element = TriggerButton;

            return element.TranslatePoint(
                new Point(
                    element.RenderSize.Width,
                    element.RenderSize.Height / 2),
                Storyboard);
        }

        public Rect GetBound()
        {
            var point = Element.TranslatePoint(new Point(), Storyboard);

            return new Rect(
                point,
                Element.RenderSize);
        }
        #endregion
    }
}