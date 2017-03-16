using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using DeXign.Animation;
using DeXign.Extension;

using WPFExtension;

namespace DeXign.Controls
{
    public class ZoomPanel : ContentControl
    {
        #region [ Dependency Property ]
        public static readonly DependencyProperty MinScaleProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(0.1d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty MaxScaleProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(8d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ScaleProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty OffsetXProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty OffsetYProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty IsPanningProperty =
            DependencyHelper.Register();
        #endregion
        
        #region [ Property ] 
        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        public double MinScale
        {
            get { return (double)GetValue(MinScaleProperty); }
            set { SetValue(MinScaleProperty, value); }
        }

        public double MaxScale
        {
            get { return (double)GetValue(MaxScaleProperty); }
            set { SetValue(MaxScaleProperty, value); }
        }

        public double OffsetX
        {
            get { return (double)GetValue(OffsetXProperty); }
            set { SetValue(OffsetXProperty, value); }
        }

        public double OffsetY
        {
            get { return (double)GetValue(OffsetYProperty); }
            set { SetValue(OffsetYProperty, value); }
        }

        public bool IsPanning
        {
            get { return (bool)GetValue(IsPanningProperty); }
            set { SetValue(IsPanningProperty, value); }
        }
        #endregion

        #region [ Local Variable ]
        private FrameworkElement contentElement;

        private TransformGroup contentTransformGroup;
        private ScaleTransform contentScaleTransform;
        private TranslateTransform contentOffsetTransform;

        // panning
        private Point beginPosition;
        #endregion

        public ZoomPanel()
        {
            EventManager.RegisterClassHandler(typeof(Window), Window.PreviewMouseWheelEvent, new MouseWheelEventHandler(Global_PreviewMouseWheel));
            EventManager.RegisterClassHandler(typeof(Window), Window.PreviewMouseDownEvent, new MouseButtonEventHandler(Global_PreviewMouseDown));
            EventManager.RegisterClassHandler(typeof(Window), Window.PreviewMouseUpEvent, new MouseButtonEventHandler(Global_PreviewMouseUp));

            this.Background = Brushes.Transparent;

            contentTransformGroup = new TransformGroup();

            contentOffsetTransform = new TranslateTransform();
            contentScaleTransform = new ScaleTransform();

            contentTransformGroup.Children.Add(contentOffsetTransform);
            contentTransformGroup.Children.Add(contentScaleTransform);

            // Scale X
            BindingEx.SetBinding(
                this, ScaleProperty,
                contentScaleTransform, ScaleTransform.ScaleXProperty);

            // Scale Y
            BindingEx.SetBinding(
                this, ScaleProperty,
                contentScaleTransform, ScaleTransform.ScaleYProperty);
            
            // Offset X
            BindingEx.SetBinding(
                this, OffsetXProperty,
                contentOffsetTransform, TranslateTransform.XProperty);

            // Offset Y
            BindingEx.SetBinding(
                this, OffsetYProperty,
                contentOffsetTransform, TranslateTransform.YProperty);
        }

        #region [ Mouse Handling ]
        private void Global_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!this.IsBoundHitted(e as MouseEventArgs))
                return;

            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                Point position = e.GetPosition(contentElement);

                double delta = (this.Scale >= 1 ? Math.Floor(this.Scale) / 10f : 0.1);

                if (e.Delta > 0)
                {
                    Zoom(this.Scale + delta, position);
                }
                else
                {
                    Zoom(this.Scale - delta, position);
                }

                e.Handled = true;
            }
        }
        
        private void Global_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (this.IsPanning)
            {
                this.IsPanning = false;

                this.ReleaseMouseCapture();
            }
        }

        private void Global_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.IsBoundHitted(e))
                return;

            if (!this.IsPanning &&
                e.LeftButton == MouseButtonState.Pressed &&
                Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                e.Handled = true;

                beginPosition = e.GetPosition(contentElement);
                this.IsPanning = true;

                this.CaptureMouse();
            }
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (this.IsPanning && e.LeftButton == MouseButtonState.Pressed)
            {
                Vector delta = e.GetPosition(contentElement) - beginPosition;
                
                this.OffsetX += delta.X;
                this.OffsetY += delta.Y;
            }

            base.OnPreviewMouseMove(e);
        }
        #endregion

        public void ZoomFit(bool animate = false)
        {
            ZoomFit(
                new Rect(
                    new Point(0, 0),
                    contentElement.RenderSize),
                animate);
        }

        public void ZoomFit(Rect bound, bool animate = false)
        {
            double xScale = RenderSize.Width / bound.Width;
            double yScale = RenderSize.Height / bound.Height;

            double scale = Math.Min(xScale, yScale);

            ScaleNormalize(ref scale);

            double offsetX = RenderSize.Width / scale / 2 - bound.Width / 2 - bound.X;
            double offsetY = RenderSize.Height / scale / 2 - bound.Height / 2 - bound.Y;

            Zoom(scale, offsetX, offsetY, animate);
        }

        public void Zoom(double scale, bool animate = false)
        {
            var contentCenter = 
                new Point(
                    this.OffsetX - this.RenderSize.Width / 2,
                    this.OffsetY - this.RenderSize.Height / 2);

            Zoom(scale, contentCenter, animate);
        }

        public void Zoom(double scale, Point position, bool animate = false)
        {
            ScaleNormalize(ref scale);

            double scaledScreenOffsetX = (position.X + this.OffsetX) * this.Scale;
            double scaledScreenOffsetY = (position.Y + this.OffsetY) * this.Scale;

            double offsetX = scaledScreenOffsetX / scale - position.X;
            double offsetY = scaledScreenOffsetY / scale - position.Y;

            Zoom(scale, offsetX, offsetY, animate);
        }

        public void Zoom(double scale, double offsetX, double offsetY, bool animate = false)
        {
            ScaleNormalize(ref scale);

            Animator.StopAnimation(this, OffsetXProperty);
            Animator.StopAnimation(this, OffsetYProperty);
            Animator.StopAnimation(this, ScaleProperty);

            if (animate)
            {
                this.BeginDoubleAnimation(OffsetXProperty, offsetX, 300, EasingFactory.CircleOut);
                this.BeginDoubleAnimation(OffsetYProperty, offsetY, 300, EasingFactory.CircleOut);
                this.BeginDoubleAnimation(ScaleProperty, scale, 600, EasingFactory.CircleOut);
            }
            else
            {
                this.Scale = scale;
                this.OffsetX = offsetX;
                this.OffsetY = offsetY;
            }
        }

        private void ScaleNormalize(ref double scale)
        {
            scale = Math.Min(Math.Max(scale, MinScale), MaxScale);
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            
            if (newContent is FrameworkElement content)
            {
                this.contentElement = content;

                content.RenderTransform = contentTransformGroup;
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Size childSize = base.MeasureOverride(
                new Size(double.PositiveInfinity, double.PositiveInfinity));
            
            double width = constraint.Width;
            double height = constraint.Height;

            if (double.IsInfinity(width))
                width = childSize.Width;

            if (double.IsInfinity(height))
                height = childSize.Height;

            return new Size(width, height);
        }

        private Size GetScaledContentSize()
        {
            return new Size(
                contentElement.RenderSize.Width * this.Scale,
                contentElement.RenderSize.Height * this.Scale);
        }
    }
}
