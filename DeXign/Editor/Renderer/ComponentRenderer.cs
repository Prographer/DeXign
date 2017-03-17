using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

using DeXign.Core;
using DeXign.Core.Logic;
using DeXign.Editor.Layer;
using DeXign.Extension;
using DeXign.Editor.Logic;

namespace DeXign.Editor.Renderer
{
    public class ComponentRenderer<TModel, TElement> : StoryboardLayer, IRenderer<TModel, TElement>, IRendererComponent, IUISupport
        where TModel : PComponent
        where TElement : ComponentElement
    {
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

        public ComponentRenderer(TElement adornedElement, TModel model) : base(adornedElement)
        {
            this.Metadata = new RendererMetadata();
            
            this.Model = model;
            this.Element = adornedElement;

            this.Element.SetComponentModel(this.Model);

            this.RendererChildren = new List<IRenderer>();

            // Pending Visual Line Queue
            pendingConnectLine = new Queue<(BindThumb Output, BindThumb Input)>();
            pendingDisconnectLine = new Queue<(BindThumb Output, BindThumb Input)>();

            // Binder
            RendererManager.ResolveBinder(this).Released += ComponentRenderer_Released;
            RendererManager.ResolveBinder(this).Binded += ComponentRenderer_Binded;
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

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            drawingContext.PushOpacity(0.5);

            drawingContext.DrawRoundedRectangle(
                null, 
                new Pen(Brushes.LightGray, 1 / Zoom.Scale),
                new Rect(0, 0, this.RenderSize.Width, this.RenderSize.Height),
                2, 2);
        }
    }
}