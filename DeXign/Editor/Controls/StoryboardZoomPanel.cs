using System;
using System.Windows.Input;
using System.Windows;

using DeXign.Core;
using DeXign.Core.Logic;
using DeXign.Core.Designer;
using DeXign.Controls;
using DeXign.Extension;
using System.Windows.Media;

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

            if (e.Data.GetDataPresent(typeof(AttributeTuple<DesignElementAttribute, Type>)))
                e.Effects = DragDropEffects.All;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);

            object data = e.Data.GetData(typeof(AttributeTuple<DesignElementAttribute, Type>));

            if (data is AttributeTuple<DesignElementAttribute, Type> tuple)
            {
                if (!tuple.Element.CanCastingTo<PComponent>())
                    return;

                this.Storyboard.AddNewComponent(
                    new Models.ComponentBoxItemModel(tuple),
                    e.GetPosition(this.Storyboard));
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            GroupSelector.UnselectAll();

            if (this.Storyboard != null)
                Keyboard.Focus(this.Storyboard);
        }
    }
}
