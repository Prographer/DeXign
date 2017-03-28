using System.Windows.Media;
using DeXign.Core.Logic;
using DeXign.Editor;
using DeXign.Editor.Logic;
using DeXign.Editor.Renderer;

[assembly: ExportRenderer(typeof(PBranch), typeof(Branch), typeof(BranchRenderer))]

namespace DeXign.Editor.Renderer
{
    public class BranchRenderer : ComponentRenderer<PBranch, Branch>
    {
        public BranchRenderer(Branch adornedElement, PBranch model) : base(adornedElement, model)
        {
        }

        protected override void OnDrawOutSightText(DrawingContext drawingContext)
        {
            DrawSightText(drawingContext, "IF");
        }
    }
}
