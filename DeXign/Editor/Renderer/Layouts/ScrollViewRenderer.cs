using System;

using DeXign.Editor;
using DeXign.Editor.Renderer;
using DeXign.Core;
using DeXign.Core.Designer;
using DeXign.Core.Controls;
using DeXign.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using DeXign.Extension;
using System.Windows;

[assembly: ExportRenderer(typeof(PScrollView), typeof(ProtrudedScrollViewer), typeof(ScrollViewRenderer))]

namespace DeXign.Editor.Renderer
{
    class ScrollViewRenderer : LayerRenderer<PScrollView, ProtrudedScrollViewer>
    {
        ScrollBar horizontalScrollBar;
        ScrollBar verticalScrollBar;

        public ScrollViewRenderer(ProtrudedScrollViewer adornedElement, PScrollView model) : base(adornedElement, model)
        {
        }
       
        protected override void OnElementAttached(ProtrudedScrollViewer element)
        {
            base.OnElementAttached(element);

            element.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            element.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

            this.verticalScrollBar = element.VerticalScrollBar;
            this.horizontalScrollBar = element.HorizontalScrollBar;

            (this.verticalScrollBar.Parent as Grid).Children.Remove(this.verticalScrollBar);
            (this.horizontalScrollBar.Parent as Grid).Children.Remove(this.horizontalScrollBar);

            // setting
            this.verticalScrollBar.Margin = new Thickness(0);
            this.verticalScrollBar.HorizontalAlignment = HorizontalAlignment.Right;
            this.verticalScrollBar.VerticalAlignment = VerticalAlignment.Stretch;

            this.horizontalScrollBar.Margin = new Thickness(0);
            this.horizontalScrollBar.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.horizontalScrollBar.VerticalAlignment = VerticalAlignment.Bottom;
            
            Add(this.verticalScrollBar);
            Add(this.horizontalScrollBar);
        }
        
        protected override Size ArrangeOverride(Size finalSize)
        {
            var size = base.ArrangeOverride(finalSize);

            if (IsElementAttached)
            {
                double blank = 10;

                this.verticalScrollBar.Arrange(
                    new Rect(
                        new Point(size.Width + blank, 0),
                        new Size(this.verticalScrollBar.RenderSize.Width, size.Height)));

                this.horizontalScrollBar.Arrange(
                    new Rect(
                        new Point(0, size.Height + blank),
                        new Size(size.Width, this.horizontalScrollBar.RenderSize.Height)));
            }

            return size;
        }
        
        protected override void OnDesignModeChanged()
        {
            base.OnDesignModeChanged();

            if (IsElementAttached)
            {
                this.verticalScrollBar.Visibility = (DesignMode == DesignMode.Size).ToVisibility();
                this.horizontalScrollBar.Visibility = (DesignMode == DesignMode.Size).ToVisibility();
            }
        }

        public override bool CanDrop(ItemDropRequest request, Point mouse)
        {
            return true;
        }
    }
}
