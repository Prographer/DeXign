using System;
using System.Windows;
using System.Collections.Generic;

using DeXign.Core;
using DeXign.Core.Logic;
using DeXign.Editor.Renderer;

namespace DeXign.Editor
{
    public interface IRenderer : IBinderHostProvider
    {
        event EventHandler ElementAttached;

        RendererMetadata Metadata { get; }
        IRenderer RendererParent { get; }
        IList<IRenderer> RendererChildren { get; }

        FrameworkElement Element { get; }
        PObject Model { get; set; }
        
        void AddChild(IRenderer child, Point position);
        void RemoveChild(IRenderer child);
        bool Equals(object outputRenderer);
    }

    public interface IRenderer<TModel, TElement> : IRenderer
        where TModel : PObject
        where TElement : FrameworkElement
    {
        new TElement Element { get; }
        new TModel Model { get; set; }
    }
}