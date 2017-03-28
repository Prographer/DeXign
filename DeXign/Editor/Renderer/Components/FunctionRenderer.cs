using DeXign.Core.Logic;
using DeXign.Editor;
using DeXign.Editor.Logic;
using DeXign.Editor.Renderer;
using System.Windows.Media;

[assembly: ExportRenderer(typeof(PFunction), typeof(Function), typeof(FunctionRenderer))]

namespace DeXign.Editor.Renderer
{
    public class FunctionRenderer : ComponentRenderer<PFunction, Function>
    {
        public FunctionRenderer(Function adornedElement, PFunction model) : base(adornedElement, model)
        {
        }

        protected override void OnDrawOutSightText(DrawingContext drawingContext)
        {
            DrawSightText(drawingContext, this.Model.FunctionName);
        }
    }
}