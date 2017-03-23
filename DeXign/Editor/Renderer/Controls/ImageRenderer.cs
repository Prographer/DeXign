using DeXign.Controls;
using DeXign.Converter;
using DeXign.Core;
using DeXign.Core.Controls;
using DeXign.Editor;
using DeXign.Editor.Renderer.Controls;
using DeXign.Extension;
using DeXign.Resources;
using DeXign.UI;

using System.Windows.Media;

[assembly: ExportRenderer(typeof(PImage), typeof(ImageBox), typeof(ImageRenderer))]

namespace DeXign.Editor.Renderer.Controls
{
    public class ImageRenderer : LayerRenderer<PImage, ImageBox>
    {
        public ImageRenderer(ImageBox adornedElement, PImage model) : base(adornedElement, model)
        {
        }

        protected override void OnElementAttached(ImageBox element)
        {
            base.OnElementAttached(element);

            BindingEx.SetBinding(
                Model, PImage.SourceProperty,
                element, ImageBox.SourceProperty,
                converter: ResourceManager.GetConverter("PathToImage"));

            BindingEx.SetBinding(
                Model, PImage.StretchProperty,
                element, ImageBox.StretchProperty,
                converter: new EnumToEnumConverter<PStretch, Stretch>());

            this.SetSize(60, 60);
        }
    }
}
