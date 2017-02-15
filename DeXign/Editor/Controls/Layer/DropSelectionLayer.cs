using DeXign.Core;
using DeXign.Core.Designer;
using DeXign.Editor.Interfaces;
using System;
using System.Windows;
using System.Windows.Media;

namespace DeXign.Editor.Layer
{
    class DropSelectionLayer : SelectionLayer, IDropHost
    {
        private bool dragCanceled = false;
        private Brush frameBrushBackup;

        public DropSelectionLayer(UIElement adornedElement) : base(adornedElement)
        {
            this.AllowDrop = true;
        }

        protected override void OnPreviewDragEnter(DragEventArgs e)
        {
            dragCanceled = !this.CanDrop(
                e.Data.GetData(
                    typeof(AttributeTuple<DesignElementAttribute, Type>)));

            ShowFrame(!dragCanceled);
        }

        private void ShowFrame(bool allowed)
        {
            frameBrushBackup = FrameBrush;

            FrameBrush = allowed ? Brushes.Green : Brushes.Red;
            AnimateFrameThickness(5, 0);
        }

        private void HideFrame()
        {
            AnimateFrameThickness(0, 0);
            FrameBrush = frameBrushBackup;
        }

        protected override void OnDragLeave(DragEventArgs e)
        {
            HideFrame();
        }

        protected override void OnDrop(DragEventArgs e)
        {
            HideFrame();

            if (!dragCanceled)
            {
                OnDrop(e.Data.GetData(typeof(AttributeTuple<DesignElementAttribute, Type>)));
            }
        }

        public virtual bool CanDrop(object item)
        {
            return false;
        }

        public virtual void OnDrop(object item)
        {
        }
    }
}
