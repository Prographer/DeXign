using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using DeXign.Converter;
using DeXign.Designer.Controls;
using DeXign.Extension;

using System.Windows.Shell;

using WPFExtension;

namespace DeXign.Designer.Layer
{
    class SelectionLayer : StoryboardLayer
    {
        #region [ Dependency Property ]
        public static readonly DependencyProperty DisplayWidthTopProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty DisplayWidthBottomProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty DisplayHeightLeftProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty DisplayHeightRightProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty SelectionBrushProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(Brushes.Blue, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty DesignModeProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(DesignModeChanged));

        private static void DesignModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var layer = d as SelectionLayer;

            layer?.UpdateFrame();
        }
        #endregion

        #region [ Property ]
        public bool DisplayWidthTop
        {
            get { return (bool) GetValue(DisplayWidthTopProperty); }
            set { SetValue(DisplayWidthTopProperty, value);}
        }

        public bool DisplayWidthBottom
        {
            get { return (bool)GetValue(DisplayWidthBottomProperty); }
            set { SetValue(DisplayWidthBottomProperty, value); }
        }

        public bool DisplayHeightLeft
        {
            get { return (bool)GetValue(DisplayHeightLeftProperty); }
            set { SetValue(DisplayHeightLeftProperty, value); }
        }

        public bool DisplayHeightRight
        {
            get { return (bool)GetValue(DisplayHeightRightProperty); }
            set { SetValue(DisplayHeightRightProperty, value); }
        }

        public Brush SelectionBrush
        {
            get { return (Brush)GetValue(SelectionBrushProperty); }
            set { SetValue(SelectionBrushProperty, value); }
        }

        public DesignMode DesignMode
        {
            get { return (DesignMode) GetValue(DesignModeProperty); }
            set { SetValue(DesignModeProperty, value); }
        }
        #endregion

        #region [ Local Variable ]
        Rectangle frame;
        Grid resizeGrid;
        #endregion

        #region [ Constructor ]
        public SelectionLayer(UIElement adornedElement) : base(adornedElement)
        {
            InitializeComponents();
            InitializeSelector();

            // 스냅라인 등록
            Parent.GuideLayer.Add(this);
            
            ScaleTransform.ScaleXProperty.AddValueChanged(ParentScale, ScaleChanged);
        }

        private void InitializeSelector()
        {
            this.AddSelectedHandler(OnSelected);
            this.AddUnselectedHandler(OnUnselected);

            AdornedElement.MouseLeftButtonDown += Target_MouseLeftButtonDown;
        }

        private void InitializeComponents()
        {
            var element = (FrameworkElement)this.AdornedElement;

            element.MinWidth = 5;
            element.MinHeight = 5;

            // Frame
            Add(frame = new Rectangle()
            {
                Visibility = Visibility.Collapsed,
                Stroke = Brushes.Red,
                StrokeThickness = 1,
                SnapsToDevicePixels = true
            });

            // Grips
            var scale = new ScaleTransform(
                ParentScale.ScaleX,
                ParentScale.ScaleY);

            Add(resizeGrid = new Grid()
            {
                Visibility = Visibility.Collapsed,
                Children =
                {
                    new ResizeThumb(element)
                    {
                        ResizeDirection = ResizeGripDirection.TopLeft,
                        Cursor = Cursors.SizeNWSE,
                        Margin = new Thickness(-5, -5, 5, 5),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        RenderTransform = scale,
                        RenderTransformOrigin = new Point(1, 1)
                    },
                    new ResizeThumb(element)
                    {
                        ResizeDirection = ResizeGripDirection.Top,
                        Cursor = Cursors.SizeNS,
                        Margin = new Thickness(0, -5, 0, 5),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Top,
                        RenderTransform = scale,
                        RenderTransformOrigin = new Point(0.5, 1)
                    },
                    new ResizeThumb(element)
                    {
                        ResizeDirection = ResizeGripDirection.TopRight,
                        Cursor = Cursors.SizeNESW,
                        Margin = new Thickness(5, -5, -5, 5),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Top,
                        RenderTransform = scale,
                        RenderTransformOrigin = new Point(0, 1)
                    },
                    new ResizeThumb(element)
                    {
                        ResizeDirection = ResizeGripDirection.Left,
                        Cursor = Cursors.SizeWE,
                        Margin = new Thickness(-5, 0, 5, 0),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                        RenderTransform = scale,
                        RenderTransformOrigin = new Point(1, 0.5)
                    },
                    new ResizeThumb(element)
                    {
                        ResizeDirection = ResizeGripDirection.Right,
                        Cursor = Cursors.SizeWE,
                        Margin = new Thickness(5, 0, -5, 0),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Center,
                        RenderTransform = scale,
                        RenderTransformOrigin = new Point(0, 0.5)
                    },
                    new ResizeThumb(element)
                    {
                        ResizeDirection = ResizeGripDirection.BottomLeft,
                        Cursor = Cursors.SizeNESW,
                        Margin = new Thickness(-5, 5, 5, -5),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        RenderTransform = scale,
                        RenderTransformOrigin = new Point(1, 0)
                    },
                    new ResizeThumb(element)
                    {
                        ResizeDirection = ResizeGripDirection.Bottom,
                        Cursor = Cursors.SizeNS,
                        Margin = new Thickness(0, 5, 0, -5),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        RenderTransform = scale,
                        RenderTransformOrigin = new Point(0.5, 0)
                    },
                    new ResizeThumb(element)
                    {
                        ResizeDirection = ResizeGripDirection.BottomRight,
                        Cursor = Cursors.SizeNWSE,
                        Margin = new Thickness(5, 5, -5, -5),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        RenderTransform = scale
                    }
                }
            });

            UpdateFrameAlignment();

            var reciprocalConverter = new ReciprocalConverter();

            BindingEx.SetBinding(
                ParentScale, ScaleTransform.ScaleXProperty,
                scale, ScaleTransform.ScaleXProperty,
                converter: reciprocalConverter);

            BindingEx.SetBinding(
                ParentScale, ScaleTransform.ScaleYProperty,
                scale, ScaleTransform.ScaleYProperty,
                converter: reciprocalConverter);

            BindingEx.SetBinding(
                ParentScale, ScaleTransform.ScaleXProperty,
                frame, Shape.StrokeThicknessProperty,
                converter: reciprocalConverter);

            BindingEx.SetBinding(
                this, SelectionBrushProperty,
                frame, Shape.StrokeProperty);

            foreach (ResizeThumb thumb in resizeGrid.Children)
            {
                BindingEx.SetBinding(
                    this, SelectionBrushProperty,
                    thumb, ResizeThumb.StrokeProperty);

                thumb.DragStarted += ThumbOnDragStarted;
                thumb.DragCompleted += ThumbOnDragCompleted;
            }
        }
        #endregion

        #region [ Selection ]

        private void InvertDesignMode()
        {
            switch (DesignMode)
            {
                case DesignMode.Trigger:
                case DesignMode.None:
                    this.DesignMode = DesignMode.Size;
                    break;

                case DesignMode.Size:
                    this.DesignMode = DesignMode.Trigger;
                    break;
            }
            Console.WriteLine(DesignMode.ToString());
        }

        private void Target_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Keyboard Focus
            Keyboard.Focus(Parent);

            // Design Mode Change
            if (GroupSelector.IsSelected(this))
                InvertDesignMode();

            // Select
            GroupSelector.Select(this, true,
                multiSelect: Keyboard.IsKeyDown(Key.LeftShift));
            
            e.Handled = true;
        }

        private void OnSelected(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            this.DesignMode = DesignMode.Size;
        }

        private void OnUnselected(object sender, SelectionChangedEventArgs e)
        {
            this.DesignMode = DesignMode.None;
        }
        #endregion

        #region [ Guide Line Status ]
        private void ThumbOnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            DisplayWidthTop = false;
            DisplayWidthBottom = false;
            DisplayHeightLeft = false;
            DisplayHeightRight = false;
        }

        private void ThumbOnDragStarted(object sender, DragStartedEventArgs dragStartedEventArgs)
        {
            var thumb = (ResizeThumb)sender;

            switch (thumb.ResizeDirection)
            {
                case ResizeGripDirection.TopLeft:
                    DisplayHeightLeft = true;
                    DisplayWidthTop = true;
                    break;

                case ResizeGripDirection.Top:
                    DisplayHeightRight = true;
                    break;

                case ResizeGripDirection.TopRight:
                    DisplayHeightRight = true;
                    DisplayWidthTop = true;
                    break;

                case ResizeGripDirection.Left:
                case ResizeGripDirection.Right:
                    DisplayWidthBottom = true;
                    break;

                case ResizeGripDirection.BottomLeft:
                    DisplayWidthBottom = true;
                    DisplayHeightLeft = true;
                    break;

                case ResizeGripDirection.Bottom:
                    DisplayHeightRight = true;
                    break;

                case ResizeGripDirection.BottomRight:
                    DisplayWidthBottom = true;
                    DisplayHeightRight = true;
                    break; ;
            }
        }
        #endregion

        #region [ Invalidated ]
        private void ScaleChanged(object sender, EventArgs e)
        {
            UpdateFrameAlignment();
        }

        private void UpdateFrameAlignment()
        {
            if (frame != null)
            {
                double stroke = 1d / ScaleX;

                frame.Margin = new Thickness(-stroke);
            }
        }

        private void UpdateFrame()
        {
            frame.Visibility = Visibility.Collapsed;
            resizeGrid.Visibility = Visibility.Collapsed;

            if (DesignMode != DesignMode.None)
            {
                frame.Visibility = Visibility.Visible;

                if (DesignMode == DesignMode.Size)
                    resizeGrid.Visibility = Visibility.Visible;
            }
        }
        #endregion

        #region [ Render ]
        protected override void OnRender(DrawingContext dc)
        {
            if (DesignMode == DesignMode.Size)
            {
                var guidelines = new GuidelineSet();

                guidelines.GuidelinesX.Add(1 / ScaleX / 2);
                guidelines.GuidelinesX.Add(1 / ScaleX / 2);
                guidelines.GuidelinesY.Add(1 / ScaleY / 2);
                guidelines.GuidelinesY.Add(1 / ScaleY / 2);

                dc.PushGuidelineSet(guidelines);

                if (DisplayWidthTop)
                    DrawGuideLineWidth(dc, false);

                if (DisplayWidthBottom)
                    DrawGuideLineWidth(dc, true);

                if (DisplayHeightLeft)
                    DrawGuideLineHeight(dc, false);

                if (DisplayHeightRight)
                    DrawGuideLineHeight(dc, true);

                dc.Pop();
            }
            else if (DesignMode == DesignMode.Trigger)
            {

            }
        }

        private void DrawGuideLineWidth(DrawingContext dc, bool isBottom)
        {
            var pen = new Pen(SelectionBrush, 1d / ScaleX);

            double top = -23 / ScaleX;
            double hTop = top + 5 / ScaleX;
            double lineHeight = 15d / ScaleX;
            double lineWidth = RenderSize.Width - 1d / ScaleX;

            if (isBottom)
            {
                top = RenderSize.Height + 7d / ScaleX;
                hTop = top + 10d / ScaleX;
            }

            // Left Vertical Line
            dc.DrawLine(pen,
                new Point(0, top),
                new Point(0, top + lineHeight));

            // Right Vertical Line
            dc.DrawLine(pen,
                new Point(lineWidth, top),
                new Point(lineWidth, top + lineHeight));

            // Horizontal Line
            dc.DrawLine(pen,
                new Point(0, hTop),
                new Point(lineWidth, hTop));

            // Value Box
            string value = RenderSize.Width.ToString("#.##");

            var formattedText = new FormattedText(
                value,
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                new Typeface("Verdana"),
                9 / ScaleX,
                SelectionBrush);

            var textPosition = new Point(
                lineWidth / 2 - formattedText.Width / 2,
                hTop - formattedText.Height / 2);

            var textBound = new Rect(
                textPosition,
                new Size(formattedText.Width, formattedText.Height));
            
            textBound.Inflate(2 / ScaleX, 2 / ScaleX);
            
            bool wOverflow = textBound.Width >= lineWidth;

            if (wOverflow)
                dc.PushTransform(isBottom
                    ? new TranslateTransform(0, 15d / ScaleX)
                    : new TranslateTransform(0, -15d / ScaleX));

            dc.DrawRectangle(Brushes.White, null, textBound);
            dc.DrawText(formattedText, textPosition);

            if (wOverflow)
                dc.Pop();
        }

        private void DrawGuideLineHeight(DrawingContext dc, bool isRight)
        {
            var pen = new Pen(SelectionBrush, 1d / ScaleX);

            double left = -23 / ScaleX;
            double vLeft = left + 5 / ScaleX;
            double lineWidth = 15d / ScaleX;
            double lineHeight = RenderSize.Height - 1d / ScaleX;

            if (isRight)
            {
                left = RenderSize.Width + 7d / ScaleX;
                vLeft = left + 10d / ScaleX;
            }

            // Top Horizontal Line
            dc.DrawLine(pen,
                new Point(left, 0),
                new Point(left + lineWidth, 0));

            // Bottom Horizontal Line
            dc.DrawLine(pen,
                new Point(left, lineHeight),
                new Point(left + lineWidth, lineHeight));

            // Vertical Line
            dc.DrawLine(pen,
                new Point(vLeft, 0),
                new Point(vLeft, lineHeight));

            // Value Box
            string value = this.RenderSize.Height.ToString("#.##");

            var formattedText = new FormattedText(
                value,
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                new Typeface("Verdana"),
                9 / ScaleX,
                SelectionBrush);

            var textPosition = new Point(
                vLeft - formattedText.Width / 2,
                lineHeight / 2 - formattedText.Height / 2);

            var textBound = new Rect(
                textPosition,
                new Size(formattedText.Width, formattedText.Height));

            textBound.Inflate(2 / ScaleX, 2 / ScaleX);
            
            bool hOverflow = textBound.Width >= lineHeight;

            if (hOverflow)
                dc.PushTransform(isRight
                    ? new TranslateTransform(15d / ScaleX, 0)
                    : new TranslateTransform(-15d / ScaleX, 0));

            dc.PushTransform(
                new RotateTransform(90,
                textBound.X + textBound.Width / 2,
                textBound.Y + textBound.Height / 2));

            dc.DrawRectangle(Brushes.White, null, textBound);
            dc.DrawText(formattedText, textPosition);

            dc.Pop();

            if (hOverflow)
                dc.Pop();
        }
        #endregion
    }
}