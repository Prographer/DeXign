using DeXign.Core;
using DeXign.Core.Controls;
using DeXign.Editor.Renderer;
using DeXign.Extension;
using System;
using System.Windows.Controls;

namespace DeXign.Editor.Controls
{
    public partial class Storyboard
    {
        internal void InitializeProject()
        {
            foreach (PContentPage screen in Model.Project.Screens)
            {
                LoadScreenRenderer(screen);

                foreach (PObject item in screen.FindContentChildrens<PObject>())
                {
                    Console.WriteLine(item.GetType().Name);
                }
            }
        }

        private void LoadScreenRenderer(PContentPage screen)
        {
            var visual = RendererManager.CreateVisualRendererFromModel(screen);

            if (visual == null)
                return;

            AddElement(this, visual);
            AddScreenCore(visual as ContentControl);
        }
    }
}