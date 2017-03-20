using System;
using System.Collections.Generic;

using DeXign.Core;
using DeXign.Core.Controls;
using DeXign.Editor.Renderer;
using DeXign.Extension;
using DeXign.Utilities;
using DeXign.Editor.Layer;
using DeXign.IO;
using DeXign.Core.Logic;
using DeXign.Editor.Logic;
using System.Windows.Threading;
using System.Windows.Controls;

namespace DeXign.Editor.Controls
{
    public partial class Storyboard
    {
        internal void InitializeProject()
        {
            foreach (PContentPage screen in Model.Project.Screens)
            {
                // Load Renderer
                LoadScreenRenderer(screen);

                DispatcherEx.WaitFor(DispatcherPriority.Loaded);

                // 모든 트리 탐색
                foreach (var node in screen.FindContentChildrens<PObject, PObject>())
                {
                    // Load Renderer
                    LoadElementRenderer(node.Parent, node.Child);
                }
            }

            // 로직
            foreach (PComponent component in Model.Project.Components)
            {
                LoadComponentRenderer(component);
            }

            // 연결 정보
            foreach (var expression in Model.Project.GetBindExpressions())
            {
                PBinder output = Model.Project.GetComponentBinder(expression.Output);
                PBinder input = Model.Project.GetComponentBinder(expression.Input);

                var outputRenderer = (output.Host as PBinderHost).GetRenderer();

                outputRenderer.ElementAttached += Attached;
                
                void Attached(object sender, EventArgs e)
                {
                    outputRenderer.ElementAttached -= Attached;

                    BindThumb outputThumb = output.GetView<BindThumb>();
                    BindThumb inputThumb = input.GetView<BindThumb>();

                    ConnectComponent(outputThumb, inputThumb);
                }
            }
        }
        
        private void LoadScreenRenderer(PContentPage screen)
        {
            var visual = RendererManager.CreateVisualRendererFromModel(screen);

            if (visual == null)
                return;

            IRenderer modelRenderer = visual.GetRenderer();

            // Create Renderer
            LoadRendererCore(modelRenderer);

            // Add to storyboard
            AddElement(this, visual);
        }

        private void LoadComponentRenderer(PComponent componentModel)
        {
            var visual = RendererManager.CreateVisualRendererFromModel(componentModel);

            if (visual == null)
                return;

            IRenderer modelRenderer = componentModel.GetRenderer();

            // Create Renderer
            LoadRendererCore(modelRenderer);

            // Add to storyboard
            AddElement(this, visual);
        }

        private void LoadElementRenderer(PObject parent, PObject model)
        {
            var visual = RendererManager.CreateVisualRendererFromModel(model);

            if (visual == null)
                return;

            IRenderer parentRenderer = parent.GetRenderer();
            IRenderer modelRenderer = model.GetRenderer();

            // Create Renderer
            LoadRendererCore(modelRenderer);
            
            // Add to storyboard
            AddElement(parentRenderer.Element, visual);
        }

        private void LoadRendererCore(IRenderer renderer)
        {
            RendererSurface surface = Model.Project.GetRendererSurface(renderer.Model.Guid);

            // * Lock
            // 렌더러를 모델에 의해 생성하는경우 초기화를 방지하기 위해 잠굼
            DesignTime.Lock(renderer as ControlLayer);

            // Metadata mapping
            renderer.Metadata.CreatedPosition = surface.Metadata.CreatedPosition;
            renderer.Metadata.CreatedTime = surface.Metadata.CreatedTime;

            // * Pending rollback
            // 렌더러가 생성되고 WPF 컨트롤이 Load 될 때 바인딩을 진행하기 때문에
            // 모델의 속성에 영향을 주지 않음 (덤프 가능 상태)
            var dump = new DumpDependencyObject(renderer.Model);

            renderer.ElementAttached += ElementAttached;

            void ElementAttached(object sender, EventArgs e)
            {
                renderer.ElementAttached -= ElementAttached;

                // Property Rollback
                dump.CopyTo(renderer.Model);

                // Unlock
                DesignTime.Unlock(renderer as ControlLayer);

                Canvas.SetLeft(renderer.Element, surface.Location.X);
                Canvas.SetTop(renderer.Element, surface.Location.Y);
            }
        }
    }
}