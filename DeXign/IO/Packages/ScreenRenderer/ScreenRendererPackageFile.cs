using DeXign.Core;
using DeXign.Editor;
using DeXign.Editor.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DeXign.IO
{ 
    internal class ScreenRendererPackageFile : ModelPackageFile<RendererContainer>
    {
        public const string Path = "Renderers";

        public ScreenRendererPackageFile(ScreenRenderer screenRenderer)
        {
            this.Name = $"{Path}\\{LayoutExtension.GetPageName(screenRenderer.Model)}.xml";

            List<IRenderer> childrens = RendererTreeHelper
                .FindChildrens<IRenderer>(screenRenderer)
                .ToList();

            childrens.Insert(0, screenRenderer);

            this.Model = new RendererContainer()
            {
                Items = childrens
                    .Select(c => new RendererSurface()
                    {
                        Guid = c.Model.Guid,
                        Metadata = c.Metadata
                    }).ToList()
            };

            base.CreateStream();
        }
    }
}
