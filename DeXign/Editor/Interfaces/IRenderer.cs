using DeXign.Core.Controls;
using System.Windows;

namespace DeXign.Editor
{
    public interface IRenderer
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
