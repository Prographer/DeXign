using System;
using System.Collections.Generic;
using System.Windows;

using DeXign.Core;
using DeXign.Core.Logic;
using DeXign.Editor.Layer;
using DeXign.Extension;
using DeXign.Editor.Logic;
using System.Windows.Controls;

namespace DeXign.Editor.Renderer
{
    public class ComponentRenderer<TModel, TElement> : StoryboardLayer, IRenderer<TModel, TElement>, IUISupport
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

        public ComponentRenderer(TElement adornedElement, TModel model) : base(adornedElement)
        {
            Metadata = new RendererMetadata();

            this.Model = model;
            this.Element = adornedElement;

            this.Element.SetComponentModel(this.Model);
            this.Element.Binded += Element_Binded;

            this.RendererChildren = new List<IRenderer>();
        }

        private void Element_Binded(object sender, BindExpression e)
        {
            // 이미 바인딩된 후 이벤트가 발생하기 때문에,
            // 시각적으로 연결만 해줌
            RootParent.ConnectComponentLine(e.Output.Renderer, e.Input.Renderer, BinderOptions.Trigger);
        }

        protected override void OnLoaded(FrameworkElement adornedElement)
        {
            RendererParent = adornedElement.Parent.GetRenderer();

            ElementAttached?.Invoke(this, EventArgs.Empty);
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
        public virtual bool CanBind(BaseBinder outputBinder, BinderOptions options)
        {
            return Model.CanBind(outputBinder, options);
        }

        public virtual void Bind(BaseBinder outputBinder, BinderOptions options)
        {
            Model.Bind(outputBinder, options);
        }

        public virtual void ReleaseInput(BaseBinder outputBinder)
        {
            Model.ReleaseInput(outputBinder);
        }

        public virtual void ReleaseOutput(BaseBinder inputBinder)
        {
            Model.ReleaseOutput(inputBinder);
        }

        public virtual void ReleaseAll()
        {
            Model.ReleaseAll();
        }

        public virtual BaseBinder ProvideValue()
        {
            return Model;
        }
        #endregion

        #region [ IUISupport ]
        public Rect GetBound()
        {
            Point position = Element.TranslatePoint(new Point(), RootParent);

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
                RootParent);
        }
        #endregion

        #region [ Dispose ]
        protected override void OnDisposed()
        {
            this.Element.Binded -= Element_Binded;

            base.OnDisposed();
        }
        #endregion
    }
}