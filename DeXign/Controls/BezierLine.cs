﻿using System.Windows;
using System.Windows.Media;

using WPFExtension;

namespace DeXign.Controls
{
    public class BezierLine : FrameworkElement
    {
        public static readonly DependencyProperty X1Property =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty Y1Property =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty X2Property =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty Y2Property =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender));
        
        public static readonly DependencyProperty BezierPoint1Property =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(new Point(0, 0.5), FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty BezierPoint2Property =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(new Point(1, 0.5), FrameworkPropertyMetadataOptions.AffectsRender));
        
        public static readonly DependencyProperty StrokeThicknessProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty LineBrushProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsArrange));

#if DEBUG
        public static readonly DependencyProperty IsDebugProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
#endif

        public double X1
        {
            get { return this.GetValue<double>(X1Property); }
            set { SetValue(X1Property, value); }
        }

        public double Y1
        {
            get { return this.GetValue<double>(Y1Property); }
            set { SetValue(Y1Property, value); }
        }

        public double X2
        {
            get { return this.GetValue<double>(X2Property); }
            set { SetValue(X2Property, value); }
        }

        public double Y2
        {
            get { return this.GetValue<double>(Y2Property); }
            set { SetValue(Y2Property, value); }
        }

        public Point BezierPoint1
        {
            get { return this.GetValue<Point>(BezierPoint1Property); }
            set { SetValue(BezierPoint1Property, value); }
        }

        public Point BezierPoint2
        {
            get { return this.GetValue<Point>(BezierPoint2Property); }
            set { SetValue(BezierPoint2Property, value); }
        }
        
        public double StrokeThickness
        {
            get { return this.GetValue<double>(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public Brush LineBrush
        {
            get { return this.GetValue<Brush>(LineBrushProperty); }
            set { SetValue(LineBrushProperty, value); }
        }

#if DEBUG
        public bool IsDebug
        {
            get { return this.GetValue<bool>(IsDebugProperty); }
            set { SetValue(IsDebugProperty, value); }
        }
#endif

        PathGeometry geometry;
        PathFigure figure;
        BezierSegment segment;
        
        // debug line
        PathGeometry debugGeometry;
        PathFigure debugFigure;
        LineSegment line1;
        LineSegment line2;
        LineSegment line3;

        public BezierLine()
        {
            geometry = new PathGeometry();
            figure = new PathFigure() { IsClosed = false };
            segment = new BezierSegment();

            geometry.Figures.Add(figure);
            figure.Segments.Add(segment);
            
            // debug line
            debugGeometry = new PathGeometry();
            debugFigure = new PathFigure()
            {
                IsClosed = false,
                Segments =
                {
                    (line1 = new LineSegment()),
                    (line2 = new LineSegment()),
                    (line3 = new LineSegment())
                }
            };
            debugGeometry.Figures.Add(debugFigure);
        }

        protected override void OnRender(DrawingContext dc)
        {
            var startPoint = Multiply(new Point(X1, Y1), (Point)RenderSize);
            var endPoint = Multiply(new Point(X2, Y2), (Point)RenderSize);

            figure.StartPoint = startPoint;
            segment.Point1 = BezierPoint1;
            segment.Point2 = BezierPoint2;
            segment.Point3 = endPoint;

            var p = new Pen(LineBrush, StrokeThickness);

            dc.DrawGeometry(null, p, geometry);

#if DEBUG
            if (IsDebug)
            {
                debugFigure.StartPoint = startPoint;
                line1.Point = BezierPoint1;
                line2.Point = BezierPoint2;
                line3.Point = endPoint;

                var rp = new Pen(Brushes.Red, 1);

                dc.PushOpacity(0.5);
                dc.DrawGeometry(null, rp, debugGeometry);
            }
#endif
        }

        private Point Multiply(Point p1, Point p2)
        {
            return new Point(p1.X * p2.X, p1.Y * p2.Y);
        }
    }
}
