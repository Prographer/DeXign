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

        private TextBlock descriptionBlock;

        public ComponentRenderer(TElement adornedElement, TModel model) : base(adornedElement)
        {
            InitializeComponent();
            InitializeResources();
            
            this.SetAdornerIndex(10);

            this.Metadata = new RendererMetadata();

            this.Model = model;
            this.Element = adornedElement;

            this.Element.SetComponentModel(this.Model);
            
            this.RendererChildren = new List<IRenderer>();

            InitializeSelector();

            // Pending Visual Line Queue
            pendingConnectLine = new Queue<(BindThumb Output, BindThumb Input)>();
            pendingDisconnectLine = new Queue<(BindThumb Output, BindThumb Input)>();

            // Binder
            RendererManager.ResolveBinder(this).Released += ComponentRenderer_Released;
            RendererManager.ResolveBinder(this).Binded += ComponentRenderer_Binded;
        }

        private void InitializeComponent()
        {
            this.Add(descriptionBlock = new TextBlock()
            {
                Visibility = Visibility.Collapsed,
                IsHitTestVisible = false,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontFamily = ResourceManager.GetFont("NotoSans.Light"),
                FontSize = 12
            });
        }

        private void InitializeSelector()
        {
            this.Element.AddSelectedHandler(Selected);
            this.Element.AddUnselectedHandler(UnSelected);

            if (!DesignTime.IsLocked(this))
                GroupSelector.Select(this.Element, true);
        }

        private void InitializeResources()
        {
            selectionBrush = ResourceManager.GetBrush("Flat.Accent.DeepDark");
        }

        private void Selected(object sender, SelectionChangedEventArgs e)
        {
            Keyboard.Focus(this.Storyboard);
            this.SetAdornerIndex(10);

            this.InvalidateVisual();

            ShowAllNodes();

            OnSelected();
        }

        private void UnSelected(object sender, SelectionChangedEventArgs e)
        {
            this.InvalidateVisual();

            HideAllNodes();

            OnUnSelected();
        }

        public void InvalidateNodes()
        {
            if (GroupSelector.IsSelected(this.Element))
                ShowAllNodes();
            else
                HideAllNodes();
        }

        private void ShowAllNodes()
        {
            SetNodeOpacity(BindOptions.Output | BindOptions.Return, 1);
            SetNodeOpacity(BindOptions.Input | BindOptions.Parameter, 1);
        }

        private void HideAllNodes()
        {
            SetNodeOpacity(BindOptions.Output | BindOptions.Return, 0.3);
            SetNodeOpacity(BindOptions.Input | BindOptions.Parameter, 0.3);
        }

        protected virtual void OnSelected()
        {
        }

        protected virtual void OnUnSelected()
        {
        }

        private void SetNodeOpacity(BindOptions option, double opacity)
        {
            this.Element.Opacity = opacity;

            foreach (var node in BinderHelper.FindHostNodes(this.Model, option))
            {
                var element = node.GetView<FrameworkElement>();

                if (element != null)
                    element.Opacity = opacity;
            }
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

            if (DesignTime.IsLocked(this))
                return;
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

            this.Storyboard.SetUnscaledProperty(descriptionBlock, TextBlock.FontSizeProperty);
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

            this.Element.RemoveSelectedHandler(Selected);
            this.Element.RemoveUnselectedHandler(UnSelected);
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
            descriptionBlock.Visibility = Visibility.Collapsed;

            if (this.Zoom.Scale < MaxZoom)
            {
                DrawOutLine(drawingContext, this.Element.AccentBrush, 1);
                OnDrawOutSight(drawingContext);

                descriptionBlock.Visibility = Visibility.Visible;
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

            descriptionBlock.Foreground = textBrush;
            descriptionBlock.Text = text;

            //var rect = new Rect(new Point(0, 0), this.RenderSize);
            //var run = gFactory.CreateGlyphRun(text, this.Fit(12), rect, AlignmentX.Center, AlignmentY.Center);

            //drawingContext.DrawGlyphRun(textBrush, run);
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