using System;
using System.Windows;
using System.Windows.Media;

using DeXign.Core;
using DeXign.Core.Designer;
using DeXign.Editor;
using System.Windows.Input;
using DeXign.Extension;
using DeXign.Core.Logic;

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

        protected override void OnPreviewDragEnter(DragEventArgs e)
        {
            object data = e.Data.GetData(typeof(AttributeTuple<DesignElementAttribute, Type>));
            dragCanceled = !this.CanDrop((AttributeTuple<DesignElementAttribute, Type>)data, e.GetPosition(Storyboard));
            
            if (data is AttributeTuple<DesignElementAttribute, Type> tuple)
            {
                if (tuple.Element.CanCastingTo<PComponent>())
                {
                    dragCanceled = true;
                    return;
                }
            }
            else
            {
                return;
            }

            ShowFrame(!dragCanceled);
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
                
                OnDrop((AttributeTuple<DesignElementAttribute, Type>)data, e.GetPosition(Storyboard));
            }
        }

        public virtual bool CanDrop(AttributeTuple<DesignElementAttribute, Type> item, Point mouse)
        {
            return false;
        }
        
        public virtual void OnDrop(AttributeTuple<DesignElementAttribute, Type> item, Point mouse)
        {
            OnCreatedChild(
                Storyboard.GenerateToElement(this.AdornedElement, item, mouse));

            Keyboard.Focus(Storyboard);
        }

        public virtual void OnCreatedChild(FrameworkElement child)
        {

        }
    }
}
