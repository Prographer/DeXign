using System.Windows;
using System.Windows.Controls;

using DeXign.Editor;
using DeXign.Editor.Renderer;
using DeXign.Core.Logic.Component;

[assembly: ExportRenderer(typeof(PTrigger), typeof(Button), typeof(TriggerRenderer))]

namespace DeXign.Editor.Renderer
{
    public class TriggerRenderer : ComponentRenderer<PTrigger, Button>
    {
        public TriggerRenderer(Button adornedElement, PTrigger model) : base(adornedElement, model)
        {
        }
    }
}