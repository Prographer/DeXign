using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using DeXign.Extension;

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
            get { return this.GetValue<CornerRadius>(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public StreamGeometry PathData
        {
            get { return this.GetValue<StreamGeometry>(PathDataProperty); }
            set { SetValue(PathDataProperty, value); }
        }

        public double PathWidth
        {
            get { return this.GetValue<double>(PathWidthProperty); }
            set { SetValue(PathWidthProperty, value); }
        }

        public double PathHeight
        {
            get { return this.GetValue<double>(PathHeightProperty); }
            set { SetValue(PathHeightProperty, value); }
        }

        public HorizontalAlignment PathHorizontalAlignment
        {
            get { return this.GetValue<HorizontalAlignment>(PathHorizontalAlignmentProperty); }
            set { SetValue(PathHorizontalAlignmentProperty, value); }
        }

        public VerticalAlignment PathVerticalAlignment
        {
            get { return this.GetValue<VerticalAlignment>(PathVerticalAlignmentProperty); }
            set { SetValue(PathVerticalAlignmentProperty, value); }
        }
    }
}
