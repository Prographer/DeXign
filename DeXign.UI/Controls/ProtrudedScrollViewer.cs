using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace DeXign.UI
{
    public class ProtrudedScrollViewer : ScrollViewer
    {
        ScrollBar verticalScrollBar;
        ScrollBar horizontalScrollBar;

        public ScrollBar VerticalScrollBar => verticalScrollBar;
        public ScrollBar HorizontalScrollBar => horizontalScrollBar;

        public ProtrudedScrollViewer()
        {
            this.Loaded += ProtrudedScrollViewer_Loaded;
        }

        private void ProtrudedScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            if (verticalScrollBar != null)
            {
                verticalScrollBar.Margin = new Thickness(
                    verticalScrollBar.RenderSize.Width, 0,
                    -verticalScrollBar.RenderSize.Width, 0);

                horizontalScrollBar.Margin = new Thickness(
                    0, horizontalScrollBar.RenderSize.Height,
                    0, -horizontalScrollBar.RenderSize.Height);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            verticalScrollBar = GetTemplateChild("PART_VerticalScrollBar") as ScrollBar;
            horizontalScrollBar = GetTemplateChild("PART_HorizontalScrollBar") as ScrollBar;
        }
    }
}
