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
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public Brush MouseOverBrush
        {
            get { return (Brush)GetValue(MouseOverBrushProperty); }
            set { SetValue(MouseOverBrushProperty, value); }
        }

        public Brush MouseDownBrush
        {
            get { return (Brush)GetValue(MouseDownBrushProperty); }
            set { SetValue(MouseDownBrushProperty, value); }
        }

        public Brush HighlightBrush
        {
            get { return (Brush)GetValue(HighlightBrushProperty); }
            set { SetValue(HighlightBrushProperty, value); }
        }

        public bool IsHighlight
        {
            get { return (bool)GetValue(IsHighlightProperty); }
            set { SetValue(IsHighlightProperty, value); }
        }
    }
}
