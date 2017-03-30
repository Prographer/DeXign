using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

using DeXign.Core;
using DeXign.Core.Logic;
using DeXign.Editor.Layer;
using DeXign.Extension;
using DeXign.Editor.Logic;
using System.Windows.Controls;
using System.Linq;
using DeXign.Resources;
using System.Windows.Input;
using DeXign.OS;
using System.Windows.Documents;
using System.Diagnostics;
using DeXign.Core.Controls;
using DeXign.Render;

namespace DeXign.Editor.Renderer
{
    public class ComponentRenderer<TModel, TElement> : StoryboardLayer, IRenderer<TModel, TElement>, IRendererComponent, IUISupport
        where TModel : PComponent
        where TElement : ComponentElement
    {
        public const double MinZoom = 0.2;
        public const double MaxZoom = 0.5;

        public event EventHandler ElementAttached;

        public TElement Element { get; }

        public TModel Model { get; set; }

        FrameworkElement IRenderer.Element => Element;

        PObject IRenderer.Model
        {
            get { return Model; }
            set { Model = (TModel)value; }
        }

        public IRenderer RendererParent { get; private set; }

        public IList<IRenderer> RendererChildren { get; }

        public RendererMetadata Metadata { get; }

        // 렌더러가 로드되기전 선을 생성 및 삭제하려는 경우 큐에 쌓아둠
        private Queue<(BindThumb Output, BindThumb Input)> pendingConnectLine;
        private Queue<(BindThumb Output, BindThumb Input)> pendingDisconnectLine;

        private Border elementBorder;
        private Brush selectionBrush;

        private GlyphRunFactory gFactory;

        public ComponentRenderer(TElement adornedElement, TModel model) : base(adornedElement)
        {
            InitializeResources();

            this.SetAdornerIndex(10);

            this.Metadata = new RendererMetadata();

            this.Model = model;
            this.Element = adornedElement;

            this.Element.SetComponentModel(this.Model);

            this.Element.AddSelectedHandler(OnSelected);
            this.Element.AddUnselectedHandler(OnUnSelected);

            this.RendererChildren = new List<IRenderer>();

            // Pending Visual Line Queue
            pendingConnectLine = new Queue<(BindThumb Output, BindThumb Input)>();
            pendingDisconnectLine = new Queue<(BindThumb Output, BindThumb Input)>();

            // Binder
            RendererManager.ResolveBinder(this).Released += ComponentRenderer_Released;
            RendererManager.ResolveBinder(this).Binded += ComponentRenderer_Binded;
        }

        private void InitializeResources()
        {
            selectionBrush = ResourceManager.GetBrush("Flat.Accent.DeepDark");

            gFactory = GlyphRunFactory.Create(
                new Typeface(
                    ResourceManager.GetFont("NotoSans.Light"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal));
        }

        private void OnSelected(object sender, SelectionChangedEventArgs e)
        {
            this.SetAdornerIndex(10);

            this.InvalidateVisual();
        }

        private void OnUnSelected(object sender, SelectionChangedEventArgs e)
        {
            this.InvalidateVisual();
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);

            this.Element.DragMove();
        }

        protected void ConnectVisualLine(BindThumb outputThumb, BindThumb inputThumb)
        {
            if (this.IsLoaded)
            {
                Storyboard.ConnectComponentLine(outputThumb, inputThumb);
            }
            else
            {
                pendingConnectLine.Enqueue((outputThumb, inputThumb));
            }
        }

        protected void DisconnectVisualLine(BindThumb outputThumb, BindThumb inputThumb)
        {
            if (this.IsLoaded)
            {
                Storyboard.DisconnectComponentLine(outputThumb, inputThumb);
            }
            else
            {
                pendingDisconnectLine.Enqueue((outputThumb, inputThumb));
            }
        }

        private void ComponentRenderer_Binded(object sender, BinderBindedEventArgs e)
        {
            var outputBinder = e.Expression.Output as PBinder;
            var inputBinder = e.Expression.Input as PBinder;

            var outputThumb = outputBinder.GetView<BindThumb>();
            var inputThumb = inputBinder.GetView<BindThumb>();

            ConnectVisualLine(outputThumb, inputThumb);
        }

        private void ComponentRenderer_Released(object sender, BinderBindedEventArgs e)
        {
            var outputBinder = e.Expression.Output as PBinder;
            var inputBinder = e.Expression.Input as PBinder;

            var outputThumb = outputBinder.GetView<BindThumb>();
            var inputThumb = inputBinder.GetView<BindThumb>();

            DisconnectVisualLine(outputThumb, inputThumb);
        }

        protected override void OnLoaded(FrameworkElement adornedElement)
        {
            RendererParent = adornedElement.Parent.GetRenderer();

            ElementAttached?.Invoke(this, EventArgs.Empty);

            // 대기중인 연결 처리
            while (pendingConnectLine.Count > 0)
            {
                var expression = pendingConnectLine.Dequeue();

                Storyboard.ConnectComponentLine(expression.Output, expression.Input);
            }

            // 대기중인 연결 해제 처리
            while (pendingDisconnectLine.Count > 0)
            {
                var expression = pendingDisconnectLine.Dequeue();

                Storyboard.DisconnectComponentLine(expression.Output, expression.Input);
            }

            // 보더 가져옴
            elementBorder = this.Element.FindVisualChildrens<Border>()
                .FirstOrDefault(b => b.Name == "border");
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

        #region [ IBinderProvider Interface ]
        public virtual IBinderHost ProvideValue()
        {
            return Model;
        }
        #endregion

        #region [ IUISupport ]
        public Rect GetBound()
        {
            Point position = Element.TranslatePoint(new Point(), Storyboard);

            return new Rect(
                position,
                Element.DesiredSize);
        }

        public Point GetLocation()
        {
            return Element.TranslatePoint(
                new Point(
                    0,
                    Element.DesiredSize.Height / 2),
                Storyboard);
        }
        #endregion

        #region [ Dispose ]
        protected override void OnDisposed()
        {
            base.OnDisposed();
        }
        #endregion

        protected IEnumerable<Rect> GetOverlappedBounds()
        {
            AdornerLayer layer = this.Storyboard.GetAdornerLayer();
            Rect bound = GetBound();

            foreach (IRenderer r in RendererTreeHelper.FindChildrens<IRenderer>(this.Storyboard.GetRenderer(), true, true))
            {
                if (r.Model is PVisual && r is IUISupport support)
                {
                    Rect iRect = Rect.Intersect(bound, support.GetBound());

                    if (iRect == Rect.Empty)
                        continue;

                    iRect.X -= bound.X;
                    iRect.Y -= bound.Y;

                    yield return iRect;
                }
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            foreach (Rect r in GetOverlappedBounds())
            {
                drawingContext.DrawRectangle(Brushes.Transparent, null, r);
            }

            DrawOutLine(drawingContext, Brushes.LightGray, 1, 0.5);

            if (this.Element.GetIsSelected())
            {
                DrawOutLine(drawingContext, selectionBrush, 4);
            }

            DrawOutSight(drawingContext);
        }

        private void DrawOutSight(DrawingContext drawingContext)
        {
            if (this.Zoom.Scale < MaxZoom)
            {
                DrawOutLine(drawingContext, this.Element.AccentBrush, 1);
                OnDrawOutSight(drawingContext);
            }
        }

        protected virtual void OnDrawOutSight(DrawingContext drawingContext)
        {
            Rect rect = new Rect(0, 0, this.RenderSize.Width, this.RenderSize.Height);
            double opacity = 1 - (this.Zoom.Scale - MinZoom) / (MaxZoom - MinZoom);

            drawingContext.PushOpacity(opacity);

            // Fill
            drawingContext.DrawRoundedRectangle(
                new SolidColorBrush(Color.FromRgb(67, 67, 67)),
                null,
                rect,
                elementBorder.CornerRadius);

            drawingContext.Pop();

            OnDrawOutSightText(drawingContext);
        }

        protected virtual void OnDrawOutSightText(DrawingContext drawingContext)
        {
            DrawSightText(drawingContext, this.Element.Header);
        }

        protected void DrawSightText(DrawingContext drawingContext, string text)
        {
            Brush textBrush = Brushes.White;

            if (!string.IsNullOrEmpty(this.Model.Description))
            {
                text = this.Model.Description;
                textBrush = Brushes.Lime;
            }

            var run = gFactory.CreateGlyphRun(text, this.Fit(12), new Rect(new Point(0, 0), this.RenderSize), AlignmentX.Center, AlignmentY.Center);

            drawingContext.DrawGlyphRun(textBrush, run);
        }

        private void DrawOutLine(DrawingContext drawingContext, Brush penBrush, double strokeWidth, double opacity = 1)
        {
            Rect rect = new Rect(0, 0, this.RenderSize.Width, this.RenderSize.Height);

            this.InflateFit(ref rect, strokeWidth / 2, strokeWidth / 2);

            drawingContext.PushOpacity(opacity);
            drawingContext.DrawRoundedRectangle(
                null,
                new Pen(penBrush, this.Fit(strokeWidth)),
                rect,
                TranslateCornerRadius(elementBorder.CornerRadius, strokeWidth));
            drawingContext.Pop();
        }

        private CornerRadius TranslateCornerRadius(CornerRadius corner, double strokeWidth)
        {
            if (corner.TopLeft > 0)
                corner.TopLeft = corner.TopLeft + this.Fit(strokeWidth / 2);

            if (corner.TopRight > 0)
                corner.TopRight = corner.TopRight + this.Fit(strokeWidth / 2);

            if (corner.BottomLeft > 0)
                corner.BottomLeft = corner.BottomLeft + this.Fit(strokeWidth / 2);

            if (corner.BottomRight > 0)
                corner.BottomRight = corner.BottomRight + this.Fit(strokeWidth / 2);

            return corner;
        }
    }
}