using System.Windows;
using DeXign.Core;
using DeXign.Core.Logic;
using System;

namespace DeXign.Editor
{
    public interface IRenderer : IBinderProvider
    {
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
