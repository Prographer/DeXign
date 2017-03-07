using System;
using System.Collections.Generic;
using System.Windows;

using DeXign.Core;
using DeXign.Core.Logic;
using DeXign.Editor.Layer;
using DeXign.Extension;

namespace DeXign.Editor.Renderer
{
    public class ComponentRenderer<TModel, TElement> : StoryboardLayer, IRenderer<TModel, TElement>, IUISupport
        where TModel : PComponent
        where TElement : FrameworkElement
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

            this.RendererChildren = new List<IRenderer>();
        }

        protected override void OnLoaded(FrameworkElement adornedElement)
        {
            RendererParent = adornedElement.Parent.GetRenderer();

            ElementAttached?.Invoke(this, EventArgs.Empty);
        }

        public virtual void OnAddedChild(IRenderer child, Point position)
        {
            this.RendererChildren.Add(child);
        }

        public virtual void OnRemovedChild(IRenderer child)
        {
            this.RendererChildren.Remove(child);
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
    }
}