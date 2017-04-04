using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using WPFExtension;

namespace DeXign.Controls
{
    class RoundButton : Button
    {
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyHelper.Register(new PropertyMetadata(new CornerRadius(10)));

        public static readonly DependencyProperty MouseOverBrushProperty =
            DependencyHelper.Register(new PropertyMetadata(Brushes.DimGray));

        public static readonly DependencyProperty MouseDownBrushProperty =
            DependencyHelper.Register(new PropertyMetadata(Brushes.Gray));

        public static readonly DependencyProperty HighlightBrushProperty =
            DependencyHelper.Register(new PropertyMetadata(Brushes.Blue));

        public static readonly DependencyProperty IsHighlightProperty =
            DependencyHelper.Register(new PropertyMetadata(false));

        public CornerRadius CornerRadius
        {
            get { return this.GetValue<CornerRadius>(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public Brush MouseOverBrush
        {
            get { return this.GetValue<Brush>(MouseOverBrushProperty); }
            set { SetValue(MouseOverBrushProperty, value); }
        }

        public Brush MouseDownBrush
        {
            get { return this.GetValue<Brush>(MouseDownBrushProperty); }
            set { SetValue(MouseDownBrushProperty, value); }
        }

        public Brush HighlightBrush
        {
            get { return this.GetValue<Brush>(HighlightBrushProperty); }
            set { SetValue(HighlightBrushProperty, value); }
        }

        public bool IsHighlight
        {
            get { return this.GetValue<bool>(IsHighlightProperty); }
            set { SetValue(IsHighlightProperty, value); }
        }
    }
}
