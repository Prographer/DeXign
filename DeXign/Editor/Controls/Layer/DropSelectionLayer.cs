using DeXign.Core;
using DeXign.Core.Designer;
using DeXign.Editor.Interfaces;
using System;
using System.Windows;
using System.Windows.Media;

namespace DeXign.Editor.Layer
{
    public class DropSelectionLayer : SelectionLayer, IDropHost<AttributeTuple<DesignElementAttribute, Type>>
    {
        private bool dragCanceled = false;
        private Brush frameBrushBackup;

        public DropSelectionLayer(UIElement adornedElement) : base(adornedElement)
        {
            this.AllowDrop = true;
        }

        protected override void OnPreviewDragEnter(DragEventArgs e)
        {
            object data = e.Data.GetData(typeof(AttributeTuple<DesignElementAttribute, Type>));
            dragCanceled = !this.CanDrop((AttributeTuple<DesignElementAttribute, Type>)data);

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

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);
            e.Effects = DragDropEffects.All;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            HideFrame();

            if (!dragCanceled)
            {
                object data = e.Data.GetData(typeof(AttributeTuple<DesignElementAttribute, Type>));

                OnDrop((AttributeTuple<DesignElementAttribute, Type>)data);
            }
        }

        public virtual bool CanDrop(AttributeTuple<DesignElementAttribute, Type> item)
        {
            return false;
        }
        
        public virtual void OnDrop(AttributeTuple<DesignElementAttribute, Type> item)
        {
            OnCreatedChild(
                Parent.GenerateToElement(this.AdornedElement, item));
        }

        public virtual void OnCreatedChild(FrameworkElement child)
        {

        }
    }
}
