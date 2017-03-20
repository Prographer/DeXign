using System;
using System.Windows;
using System.Windows.Media;

using DeXign.Core;
using DeXign.Core.Designer;
using DeXign.Editor;
using System.Windows.Input;
using DeXign.Extension;
using DeXign.Core.Logic;
using DeXign.Models;

namespace DeXign.Editor.Layer
{
    public class DropSelectionLayer : SelectionLayer, IDropHost<ItemDropRequest>
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
            var request = e.Data.GetData<ItemDropRequest>();

            dragCanceled = !this.CanDrop(request, e.GetPosition(Storyboard));
            
            if (request != null)
            {
                if (request.ItemType.CanCastingTo<PComponent>())
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
                var request = e.Data.GetData<ItemDropRequest>();
                
                OnDrop(request, e.GetPosition(Storyboard));
            }
        }

        public virtual bool CanDrop(ItemDropRequest request, Point mouse)
        {
            return false;
        }
        
        public virtual void OnDrop(ItemDropRequest request, Point mouse)
        {
            OnCreatedChild(
                Storyboard.GenerateToElement(this.AdornedElement, request.ItemType, mouse));

            Keyboard.Focus(Storyboard);
        }

        public virtual void OnCreatedChild(FrameworkElement child)
        {

        }
    }
}
