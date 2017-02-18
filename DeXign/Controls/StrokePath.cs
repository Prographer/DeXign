using System;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

using WPFExtension;

namespace DeXign.Controls
{
    [ContentProperty("Data")]
    class StrokePath : FrameworkElement
    {
        public static readonly DependencyProperty DataProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty FillProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(
                    Brushes.Black,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty StrokeProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(
                    0d,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty StretchProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(
                    Stretch.None,
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public Geometry Data
        {
            get { return (StreamGeometry)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public double StrokeThickness
        {
            get { return (double)GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public Stretch Stretch
        {
            get { return (Stretch)GetValue(StretchProperty); }
            set { SetValue(StretchProperty, value); }
        }

        ScaleTransform scale;
        TranslateTransform transform;

        public StrokePath()
        {
            scale = new ScaleTransform();
            transform = new TranslateTransform();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (Data != null)
            {
                double scaleX = 1;
                double scaleY = 1;

                switch (Stretch)
                {
                    case Stretch.Uniform:
                        scaleX = Math.Min(
                            RenderSize.Width / Data.Bounds.Width,
                            RenderSize.Height / Data.Bounds.Height);
                        scaleY = scaleX;
                        break;

                    case Stretch.UniformToFill:
                        scaleX = Math.Max(
                            RenderSize.Width / Data.Bounds.Width,
                            RenderSize.Height / Data.Bounds.Height);
                        scaleY = scaleX;
                        break;

                    case Stretch.Fill:
                        scaleX = RenderSize.Width / Data.Bounds.Width;
                        scaleY = RenderSize.Height / Data.Bounds.Height;
                        break;
                }

                scale.ScaleX = scaleX;
                scale.ScaleY = scaleY;
                scale.CenterX = RenderSize.Width / 2;
                scale.CenterY = RenderSize.Height / 2;

                transform.X = RenderSize.Width / 2 - Data.Bounds.Width / 2;
                transform.Y = RenderSize.Height / 2 - Data.Bounds.Height / 2;

                dc.PushTransform(scale);
                dc.PushTransform(transform);

                if (StrokeThickness != 0)
                    dc.DrawGeometry(null, new Pen(Stroke, StrokeThickness / scaleX), Data);

                dc.DrawGeometry(Fill, null, Data);

                dc.Pop();
                dc.Pop();
            }
        }
    }
}
