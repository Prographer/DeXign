using System.Windows;
using DeXign.Core;
using DeXign.Core.Logic;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DeXign.Editor
{
    public interface IRenderer : IBinderProvider
    {
        IRenderer RendererParent { get; }
        IList<IRenderer> RendererChildren { get; }

        FrameworkElement Element { get; }
        PObject Model { get; set; }

        void OnAddedChild(IRenderer child);
        void OnRemovedChild(IRenderer child);
    }

    public interface IRenderer<TModel, TElement> : IRenderer
        where TModel : PObject
        where TElement : FrameworkElement
    {
        new TElement Element { get; }
        new TModel Model { get; set; }
    }
}
