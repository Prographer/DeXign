using System;
using System.Windows;
using System.Windows.Controls;
using DeXign.Core.Controls;
using DeXign.Extension;
using DeXign.Editor;
using DeXign.Editor.Interfaces;
using DeXign.Editor.Renderer;

[assembly: ExportRenderer(typeof(PContentPage), typeof(ContentControl), typeof(ScreenRenderer))]

namespace DeXign.Editor.Renderer
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