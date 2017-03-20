using System;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;

using DeXign.Core;
using DeXign.Core.Logic;
using DeXign.Core.Designer;
using DeXign.Controls;
using DeXign.Extension;
using DeXign.Editor.Logic;

namespace DeXign.Editor.Controls
{
    public class StoryboardZoomPanel : ZoomPanel
    {
        public Storyboard Storyboard { get; private set; }

        public StoryboardZoomPanel() : base()
        {
            this.AllowDrop = true;
            this.Background = Brushes.White;
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            if (newContent is Storyboard storyboard)
            {
                this.Storyboard = storyboard;
            }
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);

            if (e.Data.HasData<ItemDropRequest>() ||
                e.Data.HasData<BindRequest>())
                e.Effects = DragDropEffects.All;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);

            var dropRequest = e.Data.GetData<ItemDropRequest>();
            var bindRequest = e.Data.GetData<BindRequest>();

            // 컴포넌트 생성 (From 툴박스)
            if (dropRequest != null)
            {
                if (!dropRequest.ItemType.CanCastingTo<PComponent>())
                    return;

                this.Storyboard.AddNewComponent(
                    new Models.ComponentBoxItemModel(dropRequest),
                    e.GetPosition(this.Storyboard));
            }

            // 컴포넌트 바인더에서 드래그
            if (bindRequest != null)
            {
                bindRequest.Handled = true;

                if (bindRequest.Source is LayerEventTriggerButton)
                    return;

                this.Storyboard.OpenComponentBox(bindRequest);
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            GroupSelector.UnselectAll();

            if (this.Storyboard != null)
                Keyboard.Focus(this.Storyboard);
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);

            e.Handled = true;

            this.Storyboard.OpenComponentBox(this.Storyboard);
        }
    }
}
