using System;
using System.Windows;
using System.Windows.Controls;
using DeXign.Core.Controls;
using DeXign.Designer;
using DeXign.Designer.Interfaces;
using DeXign.Designer.Renderer;
using DeXign.Extension;

[assembly: ExportRenderer(typeof(PContentPage), typeof(ContentControl), typeof(ScreenRenderer))]

namespace DeXign.Designer.Renderer
{
    class ScreenRenderer : LayerRenderer<PContentPage, ContentControl>, IDropHost
    {
        public ScreenRenderer(UIElement adornedElement) : base(adornedElement)
        {
            BindingEx.SetBinding(
                Model, PContentPage.ContentProperty,
                this, ContentControl.ContentProperty);
        }

        protected override void OnElementAttached(ContentControl element)
        {
            element.AllowDrop = true;
            element.PreviewDrop += ElementOnPreviewDrop;
        }

        private void ElementOnPreviewDrop(object sender, DragEventArgs dragEventArgs)
        {
            throw new NotImplementedException();
        }

        public bool CanDrop(object item)
        {
            return true;
        }

        public void Drop(object item)
        {
            throw new NotImplementedException();
        }
    }
}