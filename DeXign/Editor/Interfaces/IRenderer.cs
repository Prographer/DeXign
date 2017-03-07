using System.Windows;
using System.Collections.Generic;

using DeXign.Core;
using DeXign.Core.Logic;
using DeXign.Core.Controls;
using DeXign.Editor.Renderer;
using System;

namespace DeXign.Editor
{
    public interface IRenderer : IBinderProvider
    {
        event EventHandler ElementAttached;

        RendererMetadata Metadata { get; }
        IRenderer RendererParent { get; }
        IList<IRenderer> RendererChildren { get; }

        FrameworkElement Element { get; }
        PObject Model { get; set; }

        void OnAddedChild(IRenderer child, Point position);
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