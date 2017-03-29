using System.Linq;
using System.Collections.Generic;

using DeXign.Core;
using DeXign.Editor;
using DeXign.Editor.Renderer;
using System.Windows.Controls;
using System.Windows;

namespace DeXign.IO
{
    internal class ScreenRendererPackageFile : ModelPackageFile<ObjectContainer<RendererSurface>>
    {
        public const string Path = "Renderers";

        public ScreenRendererPackageFile(ScreenRenderer screenRenderer)
        {
            this.Name = $"{Path}/{LayoutExtension.GetPageName(screenRenderer.Model)}.xml";

            List<IRenderer> childrens = RendererTreeHelper
                .FindChildrens<IRenderer>(screenRenderer)
                .ToList();

            childrens.Insert(0, screenRenderer);

            this.Model = new ObjectContainer<RendererSurface>()
            {
                Items = childrens
                    .Select(c => new RendererSurface()
                    {
                        Guid = c.Model.Guid,
                        Metadata = c.Metadata,
                        Location = new Point(
                            Canvas.GetLeft(c.Element),
                            Canvas.GetTop(c.Element))
                    }).ToList()
            };

            base.CreateStream();
        }
    }
}
