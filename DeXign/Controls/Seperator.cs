using DeXign.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace DeXign.Controls
{
    public class Seperator : FrameworkElement
    {
        public static readonly DependencyProperty LineBrush1Property =
            DependencyHelper.Register(new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty LineBrush2Property =
            DependencyHelper.Register(new FrameworkPropertyMetadata(Brushes.White, FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush LineBrush1
        {
            get { return (Brush)GetValue(LineBrush1Property); }
            set { SetValue(LineBrush1Property, value); }
        }

        public Brush LineBrush2
        {
            get { return (Brush)GetValue(LineBrush2Property); }
            set { SetValue(LineBrush2Property, value); }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Rect rect;
            Rect bound = new Rect(new Point(0, 0), new Size(this.ActualWidth, this.ActualHeight));

            if (this.ActualWidth > this.ActualHeight)
            {
                rect = new Rect(
                    new Point(0, 0), 
                    new Point(this.ActualWidth, this.ActualHeight / 2));
            }
            else
            {
                rect = new Rect(
                    new Point(0, 0),
                    new Point(this.ActualWidth / 2, this.ActualHeight));
            }

            drawingContext.DrawRectangle(LineBrush2, null, bound);
            drawingContext.DrawRectangle(LineBrush1, null, rect);
        }
    }
}
