using DeXign.Extension;
using DeXign.UI;
using DeXign.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using WPFExtension;

namespace DeXign.Controls
{
    public class Ruler : Control
    {
        const int BorderWidth = 16;

        public static readonly DependencyProperty TargetProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty ScaleProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(1d));

        public FrameworkElement Target
        {
            get { return this.GetValue<FrameworkElement>(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        public double Scale
        {
            get { return this.GetValue<double>(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        private Brush backgroundBrush;
        private Brush gradationBrush;
        private Brush foregroundBrush;
        private Pen gradationPen;
        
        public Ruler()
        {
            this.ClipToBounds = true;

            backgroundBrush = "#474747".ToBrush();
            gradationBrush = "#666666".ToBrush();
            foregroundBrush = "#9E9D9B".ToBrush();

            gradationPen = new Pen(gradationBrush, 1);
        }
        
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            // Draw Border
            dc.DrawRectangle(backgroundBrush, null, new Rect(0, 0, this.RenderSize.Width, BorderWidth));
            dc.DrawRectangle(backgroundBrush, null, new Rect(0, 0, BorderWidth, this.RenderSize.Height));

            dc.DrawLine(gradationPen, new Point(0, BorderWidth), new Point(this.RenderSize.Width, BorderWidth));
            dc.DrawLine(gradationPen, new Point(BorderWidth, 0), new Point(BorderWidth, this.RenderSize.Height));

            // Draw Gradation
            var highlights = new List<Rect>();

            double beginOffsetX = 0;
            double beginOffsetY = 0;

            if (this.Target != null)
            {
                Point position = this.Target.TranslatePoint(new Point(), this);

                beginOffsetX = position.X;
                beginOffsetY = position.Y;

                highlights.Add(
                    new Rect(
                        position,
                        new Size(
                            this.Target.RenderSize.Width * this.Scale,
                            this.Target.RenderSize.Height * this.Scale)));
            }
            
            foreach (Rect rect in highlights)
            {
                dc.PushOpacity(0.3);

                dc.DrawRectangle(Brushes.Black, null, new Rect(rect.X, 0, rect.Width, BorderWidth));
                dc.DrawRectangle(Brushes.Black, null, new Rect(0, rect.Y, BorderWidth, rect.Height));

                dc.Pop();
            }

            int smallTick = 5;
            int largeTick = 25;

            while (smallTick * this.Scale < 5)
            {
                smallTick += 5;
            }

            while (largeTick * this.Scale < 25)
            {
                largeTick += 25;
            }

            //var guidelines = new GuidelineSet();

            //guidelines.GuidelinesX.Add(0.5);
            //guidelines.GuidelinesX.Add(0.5);
            //guidelines.GuidelinesY.Add(0.5);
            //guidelines.GuidelinesY.Add(0.5);

            //dc.PushGuidelineSet(guidelines);

            //dc.PushTransform(new TranslateTransform(beginOffsetX, 0));

            ////for (int x = 0; x >= BorderWidth; x -= smallTick)
            ////{
            ////    //DrawHorizontal(x);
            ////}

            //int beginX = 0;

            //if (beginOffsetX < 0)
            //    beginX = -(int)beginOffsetX;
            
            //Console.WriteLine(((this.RenderSize.Width - beginOffsetX) / this.Scale - beginX) / smallTick);

            //for (int x = beginX; x * this.Scale < (this.RenderSize.Width - beginOffsetX); x += smallTick)
            //{
            //    DrawHorizontal(x);
            //}

            //dc.Pop();

            //void DrawHorizontal(double offset)
            //{
            //    double top = BorderWidth - 5;
                
            //    if (offset % largeTick < 1)
            //    {
            //        top = 0;

            //        var text = new FormattedText(
            //            offset.ToString(),
            //            CultureInfo.CurrentCulture,
            //            FlowDirection.LeftToRight,
            //            new Typeface("Verdana"),
            //            10,
            //            foregroundBrush);

            //        dc.DrawText(text,
            //            new Point(
            //                offset * this.Scale + 2, 
            //                BorderWidth - text.Height - 2));
            //    }
                
            //    dc.DrawLine(
            //        gradationPen,
            //        new Point(Math.Floor(offset * this.Scale), top),
            //        new Point(Math.Floor(offset * this.Scale), BorderWidth));
            //}
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == TargetProperty)
            {
                OnTargetChanged(
                    e.OldValue as FrameworkElement, 
                    e.NewValue as FrameworkElement);

                Dispatcher.BeginInvoke(
                    (Action)this.InvalidateVisual,
                    DispatcherPriority.Render);
            }
        }

        protected virtual void OnTargetChanged(FrameworkElement oldTarget, FrameworkElement newTarget)
        {
            if (oldTarget != null)
            {
                oldTarget.SizeChanged -= Ruler_SizeChanged;

                if (oldTarget is IMovable movable)
                    movable.Moved -= Target_Moved;
            }

            if (newTarget != null)
            {
                newTarget.SizeChanged += Ruler_SizeChanged;

                if (newTarget is IMovable movable)
                    movable.Moved += Target_Moved;
            }
        }

        private void Target_Moved(object sender, EventArgs e)
        {
            this.InvalidateVisual();
        }

        private void Ruler_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.InvalidateVisual();
        }
    }
}
