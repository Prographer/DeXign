using DeXign.Extension;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPFExtension;

namespace DeXign.Controls
{
    public class PathButton : Button
    {
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty PathDataProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty PathWidthProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty PathHeightProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty PathVerticalAlignmentProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty PathHorizontalAlignmentProperty =
            DependencyHelper.Register();

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public StreamGeometry PathData
        {
            get { return (StreamGeometry)GetValue(PathDataProperty); }
            set { SetValue(PathDataProperty, value); }
        }

        public double PathWidth
        {
            get { return (double)GetValue(PathWidthProperty); }
            set { SetValue(PathWidthProperty, value); }
        }

        public double PathHeight
        {
            get { return (double)GetValue(PathHeightProperty); }
            set { SetValue(PathHeightProperty, value); }
        }

        public HorizontalAlignment PathHorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(PathHorizontalAlignmentProperty); }
            set { SetValue(PathHorizontalAlignmentProperty, value); }
        }

        public VerticalAlignment PathVerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(PathVerticalAlignmentProperty); }
            set { SetValue(PathVerticalAlignmentProperty, value); }
        }
    }
}
