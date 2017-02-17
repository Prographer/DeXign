using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WPFExtension;

namespace DeXign.Controls
{
    public class RectangleEx : FrameworkElement
    {
        public static readonly DependencyProperty RadiusProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(
                    5d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty FillProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(
                    Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty StrokeProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(
                    Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(
                    0d, FrameworkPropertyMetadataOptions.AffectsRender));

        public double Radius
        {
            get { return (double)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
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
        
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            
            double radius = new[] 
            {
                Radius,
                this.RenderSize.Width / 2,
                this.RenderSize.Height / 2
            }.Min();

            dc.DrawRoundedRectangle(
                Fill, 
                new Pen(Stroke, StrokeThickness), 
                new Rect(
                    new Point(),
                    this.RenderSize), 
                radius, radius);
        }
    }
}