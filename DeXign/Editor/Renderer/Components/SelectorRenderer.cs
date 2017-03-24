using System.Windows;
using DeXign.Core.Logic;
using DeXign.Editor;
using DeXign.Editor.Logic;
using DeXign.Editor.Renderer;
using System.Windows.Media;

[assembly: ExportRenderer(typeof(PSelector), typeof(ObjectSelector), typeof(SelectorRenderer))]

namespace DeXign.Editor.Renderer
{
    public class SelectorRenderer : ComponentRenderer<PSelector, ObjectSelector>
    {
        public SelectorRenderer(ObjectSelector adornedElement, PSelector model) : base(adornedElement, model)
        {
        }

        protected override void OnLoaded(FrameworkElement adornedElement)
        {
            base.OnLoaded(adornedElement);
        }
    }
}
