using System;
using System.Diagnostics;
using System.Windows.Controls;

using DeXign.Core;
using DeXign.Core.Controls;
using DeXign.Editor.Renderer;
using DeXign.Extension;
using DeXign.Task;
using DeXign.Utilities;
using System.Collections.Generic;
using System.Windows;
using DeXign.Editor.Layer;

namespace DeXign.Editor.Controls
{
    public partial class Storyboard
    {
        private Dictionary<IRenderer, DumpDependencyObject> pendingDumps = 
            new Dictionary<IRenderer, DumpDependencyObject>();

        internal void InitializeProject()
        {
            var sw = new Stopwatch();
            sw.Start();

            foreach (PContentPage screen in Model.Project.Screens)
            {
                var screenDump = new DumpDependencyObject(screen);
                LoadScreenRenderer(screen); // Load Renderer
                
                // Roll Back
                screenDump.RollBack();
                
                DispatcherEx.WaitForContextIdle();

                // 모든 트리 탐색
                foreach (var node in screen.FindContentChildrens<PObject, PObject>())
                {
                    var elementDump = new DumpDependencyObject(node.Child);

                    LoadElementRenderer(node.Parent, node.Child); // Load Renderer

                    IRenderer renderer = node.Child.GetRenderer();
                    
                    pendingDumps.Add(renderer, elementDump);
                    renderer.ElementAttached += Storyboard_ElementAttached;
                }
            }

            sw.Stop();
        }

        private void Storyboard_ElementAttached(object sender, EventArgs e)
        {
            var renderer = sender as IRenderer;

            renderer.ElementAttached -= Storyboard_ElementAttached;

            if (pendingDumps.ContainsKey(renderer))
            {
                // Property Rollback
                pendingDumps[renderer].CopyTo(renderer.Model);

                // Remove At Pending List
                pendingDumps.Remove(renderer);
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

        private void LoadElementRenderer(PObject parent, PObject model)
        {
            var visual = RendererManager.CreateVisualRendererFromModel(model);

            if (visual == null)
                return;

            IRenderer parentRenderer = parent.GetRenderer();
            IRenderer modelRenderer = model.GetRenderer();

            AddElement(parentRenderer.Element, visual);
        }
    }
}