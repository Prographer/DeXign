using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows;

using DeXign.Extension;

using WPFExtension;

namespace DeXign.Behavior
{
    class ScrollBoundShadowEffect : Behavior<ScrollViewer>
    {
        public static readonly DependencyProperty OrientationProperty =
            DependencyHelper.Register(
                new PropertyMetadata(Orientation.Vertical));

        public static readonly DependencyProperty ShadowSizeProperty =
            DependencyHelper.Register(
                new PropertyMetadata(60d));

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public double ShadowSize
        {
            get { return (double)GetValue(ShadowSizeProperty); }
            set { SetValue(ShadowSizeProperty, value); }
        }

        ScrollBar verticalScrollBar;
        ScrollBar horizontalScrollBar;

        // mask
        LinearGradientBrush maskBrush;

        GradientStop topStop;
        GradientStop bottomStop;
        GradientStop topThumbStop;
        GradientStop bottomThumStop;

        public ScrollBoundShadowEffect()
        {
            maskBrush = new LinearGradientBrush()
            {
                StartPoint = new Point(0.5, 0),
                EndPoint = new Point(0.5, 1)
            };

            topStop = new GradientStop()
            {
                Color = Colors.Transparent,
                Offset = 0
            };

            topThumbStop = new GradientStop()
            {
                Color = Colors.Black,
                Offset = 0
            };

            bottomThumStop = new GradientStop()
            {
                Color = Colors.Black,
                Offset = 1
            };

            bottomStop = new GradientStop()
            {
                Color = Colors.Transparent,
                Offset = 1
            };

            maskBrush.GradientStops.Add(topStop);
            maskBrush.GradientStops.Add(topThumbStop);
            maskBrush.GradientStops.Add(bottomThumStop);
            maskBrush.GradientStops.Add(bottomStop);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property.Name == nameof(ShadowSize))
                InvalidateMasking();
            else if (e.Property.Name == nameof(Orientation))
                InvalidateOrientation();
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.OpacityMask = maskBrush;

            verticalScrollBar = AssociatedObject
                .FindVisualChildrens<ScrollBar>()
                .FirstOrDefault(sb => sb.Name == "PART_VerticalScrollBar");

            horizontalScrollBar = AssociatedObject
                .FindVisualChildrens<ScrollBar>()
                .FirstOrDefault(sb => sb.Name == "PART_HorizontalScrollBar");
            
            verticalScrollBar.ValueChanged += ScrollBar_ValueChanged;
            horizontalScrollBar.ValueChanged += ScrollBar_ValueChanged;

            if (AssociatedObject.IsLoaded)
                InvalidateMasking();
            else
                AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            InvalidateMasking();
        }

        protected override void OnDetaching()
        {
            if (verticalScrollBar != null)
            {
                verticalScrollBar.ValueChanged -= ScrollBar_ValueChanged;
                horizontalScrollBar.ValueChanged -= ScrollBar_ValueChanged;
            }

            verticalScrollBar = null;
            horizontalScrollBar = null;

            base.OnDetaching();
        }

        private void ScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            InvalidateMasking();
        }

        private void InvalidateMasking()
        {
            OpacityMasking(
                Orientation == Orientation.Vertical ? verticalScrollBar : horizontalScrollBar);
        }

        private void InvalidateOrientation()
        {
            if (Orientation == Orientation.Vertical)
            {
                maskBrush.StartPoint = new Point(0.5, 0);
                maskBrush.EndPoint = new Point(0.5, 1);
            }
            else
            {
                maskBrush.StartPoint = new Point(0, 0.5);
                maskBrush.EndPoint = new Point(1, 0.5);
            }
        }

        private void OpacityMasking(ScrollBar scrollBar)
        {
            if (scrollBar == null)
                return;

            double value = 
                (scrollBar.Value - scrollBar.Minimum) / (scrollBar.Maximum - scrollBar.Minimum);

            double size = 
                Orientation == Orientation.Vertical ? AssociatedObject.RenderSize.Height : AssociatedObject.RenderSize.Width;

            double maxOffset = Math.Min(size / 2, ShadowSize) / size;
            double topOffset = Math.Min(value, maxOffset);
            double bottomOffset = Math.Min(1 - value, maxOffset);
            
            topThumbStop.Offset = topOffset;
            bottomThumStop.Offset = 1 - bottomOffset;
        }
    }
}
