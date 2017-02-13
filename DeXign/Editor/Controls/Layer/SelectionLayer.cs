using System;
using System.Globalization;
using System.Windows;
using System.Windows.Shell;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using DeXign.Converter;
using DeXign.Extension;
using DeXign.Editor.Controls;

using WPFExtension;
using DeXign.Resources;

namespace DeXign.Editor.Layer
{
    class SelectionLayer : StoryboardLayer
    {
        #region [ Dependency Property ]
        public static readonly DependencyProperty DisplayMarginProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

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
                new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty DesignModeProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(DesignModeChanged));

        private static void DesignModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var layer = d as SelectionLayer;

            layer.OnDesignModeChanged();
        }
        #endregion

        #region [ Property ]
        public new FrameworkElement AdornedElement
        {
            get { return (FrameworkElement)base.AdornedElement; }
        }

        public bool DisplayMargin
        {
            get { return (bool)GetValue(DisplayMarginProperty); }
            set { SetValue(DisplayMarginProperty, value); }
        }

        public bool DisplayWidthTop
        {
            get { return (bool)GetValue(DisplayWidthTopProperty); }
            set { SetValue(DisplayWidthTopProperty, value); }
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
            get { return (DesignMode)GetValue(DesignModeProperty); }
            set { SetValue(DesignModeProperty, value); }
        }
        #endregion

        #region [ Local Variable ]
        const double Blank = 6;
        const double ValueBoxBlank = 2;

        Rectangle frame;
        EventTriggerButton triggerButton;
        Grid resizeGrid;
        Grid clipGrid;
        MarginClipHolder clipData;
        #endregion

        #region [ Constructor ]
        public SelectionLayer(UIElement adornedElement) : base(adornedElement)
        {
            InitializeComponents();
            InitializeSelector();

            SelectionBrush = ResourceManager.GetBrush("Accent");

            // 스냅라인 등록
            Parent.GuideLayer.Add(this);

            ScaleTransform.ScaleXProperty.AddValueChanged(ParentScale, ScaleChanged);

            UpdateParentState();
        }

        private void InitializeSelector()
        {
            this.AddSelectedHandler(OnSelected);
            this.AddUnselectedHandler(OnUnselected);

            AdornedElement.MouseLeftButtonDown += Target_MouseLeftButtonDown;
        }

        private void InitializeComponents()
        {
            AdornedElement.MinWidth = 5;
            AdornedElement.MinHeight = 5;

            var scale = new ScaleTransform(
                ParentScale.ScaleX,
                ParentScale.ScaleY);

            #region < Frame >
            Add(frame = new Rectangle()
            {
                Visibility = Visibility.Collapsed,
                Stroke = Brushes.Red,
                StrokeThickness = 1,
                SnapsToDevicePixels = true
            });
            #endregion

            #region < Margin Clips >
            clipData = new MarginClipHolder();

            Add(clipGrid = new Grid()
            {
                Visibility = Visibility.Collapsed,
                Children =
                {
                    (clipData.LeftClip = new MarginClip
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        LayoutTransform = new TransformGroup()
                        {
                            Children =
                            {
                                scale,
                                new RotateTransform(90)
                            }
                        }
                    }),
                    (clipData.TopClip = new MarginClip
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        LayoutTransform = scale
                    }),
                    (clipData.RightClip = new MarginClip
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        LayoutTransform = new TransformGroup()
                        {
                            Children =
                            {
                                scale,
                                new RotateTransform(90)
                            }
                        }
                    }),
                    (clipData.BottomClip = new MarginClip
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        LayoutTransform = scale
                    }),
                }
            });
            #endregion

            #region < Grips >
            Add(resizeGrid = new Grid()
            {
                Visibility = Visibility.Collapsed,
                Children =
                {
                    new ResizeThumb(AdornedElement)
                    {
                        ResizeDirection = ResizeGripDirection.TopLeft,
                        Cursor = Cursors.SizeNWSE,
                        Margin = new Thickness(-5, -5, 5, 5),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        RenderTransform = scale,
                        RenderTransformOrigin = new Point(1, 1)
                    },
                    new ResizeThumb(AdornedElement)
                    {
                        ResizeDirection = ResizeGripDirection.Top,
                        Cursor = Cursors.SizeNS,
                        Margin = new Thickness(0, -5, 0, 5),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Top,
                        RenderTransform = scale,
                        RenderTransformOrigin = new Point(0.5, 1)
                    },
                    new ResizeThumb(AdornedElement)
                    {
                        ResizeDirection = ResizeGripDirection.TopRight,
                        Cursor = Cursors.SizeNESW,
                        Margin = new Thickness(5, -5, -5, 5),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Top,
                        RenderTransform = scale,
                        RenderTransformOrigin = new Point(0, 1)
                    },
                    new ResizeThumb(AdornedElement)
                    {
                        ResizeDirection = ResizeGripDirection.Left,
                        Cursor = Cursors.SizeWE,
                        Margin = new Thickness(-5, 0, 5, 0),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                        RenderTransform = scale,
                        RenderTransformOrigin = new Point(1, 0.5)
                    },
                    new ResizeThumb(AdornedElement)
                    {
                        ResizeDirection = ResizeGripDirection.Right,
                        Cursor = Cursors.SizeWE,
                        Margin = new Thickness(5, 0, -5, 0),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Center,
                        RenderTransform = scale,
                        RenderTransformOrigin = new Point(0, 0.5)
                    },
                    new ResizeThumb(AdornedElement)
                    {
                        ResizeDirection = ResizeGripDirection.BottomLeft,
                        Cursor = Cursors.SizeNESW,
                        Margin = new Thickness(-5, 5, 5, -5),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        RenderTransform = scale,
                        RenderTransformOrigin = new Point(1, 0)
                    },
                    new ResizeThumb(AdornedElement)
                    {
                        ResizeDirection = ResizeGripDirection.Bottom,
                        Cursor = Cursors.SizeNS,
                        Margin = new Thickness(0, 5, 0, -5),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        RenderTransform = scale,
                        RenderTransformOrigin = new Point(0.5, 0)
                    },
                    new ResizeThumb(AdornedElement)
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
            #endregion

            #region < Event Trigger Button >
            Add(triggerButton = new EventTriggerButton(this)
            {
                Visibility = Visibility.Collapsed,
                Margin = new Thickness(10, 0, -10, 0),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                RenderTransformOrigin = new Point(0, 0.5),
                RenderTransform = scale
            });
            #endregion

            #region < Binding >
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
                triggerButton, Control.BackgroundProperty);

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

            // caching
            UpdateMarginClips();

            foreach (MarginClip clip in clipGrid.Children)
                ToggleButton.IsCheckedProperty.AddValueChanged(clip, ClipChanged);
            #endregion

            UpdateFrameAlignment();
        }

        private void ClipChanged(object sender, EventArgs e)
        {
            var parentMargin = GetParentRenderMargin();
            var renderSize = new Size(ActualWidth, ActualHeight);

            parentMargin.Left *= -1;
            parentMargin.Top *= -1;

            #region < Horizontal >
            if (clipData.Left && clipData.Right)
            {
                AdornedElement.HorizontalAlignment = HorizontalAlignment.Stretch;
                renderSize.Width = double.NaN;
            }

            if (clipData.Left && !clipData.Right)
            {
                AdornedElement.HorizontalAlignment = HorizontalAlignment.Left;
                parentMargin.Right = 0;
            }

            if (!clipData.Left && clipData.Right)
            {
                AdornedElement.HorizontalAlignment = HorizontalAlignment.Right;
                parentMargin.Left = 0;
            }

            if (!clipData.Left && !clipData.Right)
            {
                AdornedElement.HorizontalAlignment = HorizontalAlignment.Center;
            }
            #endregion

            #region < Vertical >
            if (clipData.Top && clipData.Bottom)
            {
                AdornedElement.VerticalAlignment = VerticalAlignment.Stretch;
                renderSize.Height = double.NaN;
            }

            if (clipData.Top && !clipData.Bottom)
            {
                AdornedElement.VerticalAlignment = VerticalAlignment.Top;
                parentMargin.Bottom = 0;
            }

            if (!clipData.Top && clipData.Bottom)
            {
                AdornedElement.VerticalAlignment = VerticalAlignment.Bottom;
                parentMargin.Top = 0;
            }

            if (!clipData.Top && !clipData.Bottom)
            {
                AdornedElement.VerticalAlignment = VerticalAlignment.Center;
            }
            #endregion

            AdornedElement.Margin = parentMargin;
            AdornedElement.Width = renderSize.Width;
            AdornedElement.Height = renderSize.Height;

            this.InvalidateVisual();
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
        protected virtual void OnDesignModeChanged()
        {
            UpdateParentState();
            UpdateMarginClips();
            UpdateFrame();
        }

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
            triggerButton.Visibility = Visibility.Collapsed;

            if (DesignMode != DesignMode.None)
            {
                frame.Visibility = Visibility.Visible;

                if (DesignMode == DesignMode.Size)
                    resizeGrid.Visibility = Visibility.Visible;
                else
                    triggerButton.Visibility = Visibility.Visible;
            }

            this.InvalidateVisual();
        }

        private void UpdateParentState()
        {
            DisplayMargin = !(AdornedElement.Parent is Canvas);

            clipGrid.Visibility = BoolToVisibility(DisplayMargin && DesignMode == DesignMode.Size);
        }

        private void UpdateMarginClips()
        {
            clipData.Left =
                AdornedElement.HorizontalAlignment == HorizontalAlignment.Left ||
                AdornedElement.HorizontalAlignment == HorizontalAlignment.Stretch;

            clipData.Right =
                AdornedElement.HorizontalAlignment == HorizontalAlignment.Right ||
                AdornedElement.HorizontalAlignment == HorizontalAlignment.Stretch;

            clipData.Top =
                AdornedElement.VerticalAlignment == VerticalAlignment.Top ||
                AdornedElement.VerticalAlignment == VerticalAlignment.Stretch;

            clipData.Bottom =
                AdornedElement.VerticalAlignment == VerticalAlignment.Bottom ||
                AdornedElement.VerticalAlignment == VerticalAlignment.Stretch;
        }
        #endregion

        #region [ Render ]
        protected override Size ArrangeOverride(Size finalSize)
        {
            // * Vritual Parent Bound Arrange *

            var arrangeSize = base.ArrangeOverride(finalSize);

            Rect rect = GetParentRenderBound();
            double virtualWidth = clipData.LeftClip.RenderSize.Height / ScaleX;
            double virtualHeight = clipData.LeftClip.RenderSize.Height / ScaleX;

            clipData.LeftClip.Arrange(
                new Rect(
                    rect.X - virtualWidth / 2, 0, 
                    virtualWidth, RenderSize.Height));
            
            clipData.RightClip.Arrange(
                new Rect(
                    rect.Right - virtualWidth / 2, 0,
                    virtualWidth, RenderSize.Height));

            clipData.TopClip.Arrange(
                new Rect(
                    0, rect.Y - virtualHeight / 2,
                    RenderSize.Width, virtualHeight));

            clipData.BottomClip.Arrange(
                new Rect(
                    0, rect.Bottom - virtualHeight / 2, 
                    RenderSize.Width, virtualHeight));

            return arrangeSize;
        }

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

                if (DisplayMargin)
                    DrawGuidLineMargin(dc);

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

        private void DrawGuidLineMargin(DrawingContext dc)
        {
            bool marginLeft =
                AdornedElement.HorizontalAlignment == HorizontalAlignment.Left ||
                AdornedElement.HorizontalAlignment == HorizontalAlignment.Stretch;

            bool marginRight =
                AdornedElement.HorizontalAlignment == HorizontalAlignment.Right ||
                AdornedElement.HorizontalAlignment == HorizontalAlignment.Stretch;

            bool marginTop =
                AdornedElement.VerticalAlignment == VerticalAlignment.Top ||
                AdornedElement.VerticalAlignment == VerticalAlignment.Stretch;

            bool marginBottom =
                AdornedElement.VerticalAlignment == VerticalAlignment.Bottom ||
                AdornedElement.VerticalAlignment == VerticalAlignment.Stretch;

            var parentRect = GetParentRenderBound();
            var parentMargin = GetParentRenderMargin();

            var framePen = new Pen(ResourceManager.GetBrush("LemonGrass"), 1d / ScaleX);
            var solidPen = new Pen(SelectionBrush, 1d / ScaleX);
            var dashedPen = new Pen(SelectionBrush, 1d / ScaleX)
            {
                DashStyle = new DashStyle(new double[] { 4, 4 }, 0)
            };

            double hCenter = RenderSize.Height / 2;
            double wCenter = RenderSize.Width / 2;

            // Draw Parent Frame
            dc.DrawRectangle(null, framePen, parentRect);

            // Draw Margin Guide Line
            dc.PushOpacity(0.5);
            {
                dc.DrawLine(marginLeft ? solidPen : dashedPen,
                    new Point(parentRect.X, hCenter),
                    new Point(-1, hCenter));

                dc.DrawLine(marginRight ? solidPen : dashedPen,
                    new Point(RenderSize.Width, hCenter),
                    new Point(parentRect.Right, hCenter));

                dc.DrawLine(marginTop ? solidPen : dashedPen,
                    new Point(wCenter, parentRect.Y),
                    new Point(wCenter, -1));

                dc.DrawLine(marginBottom ? solidPen : dashedPen,
                    new Point(wCenter, RenderSize.Height),
                    new Point(wCenter, parentRect.Bottom));
            }
            dc.Pop();

            // Draw Margin Value
            string valueLeft = AdornedElement.Margin.Left.ToString("0.##");
            string valueRight = AdornedElement.Margin.Right.ToString("0.##");
            string valueTop = AdornedElement.Margin.Top.ToString("0.##");
            string valueBottom = AdornedElement.Margin.Bottom.ToString("0.##");

            // Value FormattedText
            var formattedTextLeft = CreateFormattedText(valueLeft, 9, "Verdana", SelectionBrush);
            var formattedTextRight = CreateFormattedText(valueRight, 9, "Verdana", SelectionBrush);
            var formattedTextTop = CreateFormattedText(valueTop, 9, "Verdana", SelectionBrush);
            var formattedTextBottom = CreateFormattedText(valueBottom, 9, "Verdana", SelectionBrush);

            // Value Positions
            var textPositionLeft = new Point(
                parentMargin.Left / 2 - formattedTextLeft.Width / 2,
                hCenter - formattedTextLeft.Height / 2);

            var textPositionRight = new Point(
                RenderSize.Width + parentMargin.Right / 2 - formattedTextRight.Width / 2,
                hCenter - formattedTextRight.Height / 2);

            var textPositionTop = new Point(
                wCenter - formattedTextTop.Width / 2,
                parentMargin.Top / 2 - formattedTextTop.Height / 2);

            var textPositionBottom = new Point(
                wCenter - formattedTextBottom.Width / 2,
                RenderSize.Height + parentMargin.Bottom / 2 - formattedTextBottom.Height / 2);

            // Value Box Bounds
            var textBoundLeft = new Rect(textPositionLeft, new Size(formattedTextLeft.Width, formattedTextLeft.Height));
            var textBoundRight = new Rect(textPositionRight, new Size(formattedTextRight.Width, formattedTextRight.Height));
            var textBoundTop = new Rect(textPositionTop, new Size(formattedTextTop.Width, formattedTextTop.Height));
            var textBoundBottom = new Rect(textPositionBottom, new Size(formattedTextBottom.Width, formattedTextBottom.Height));

            // Value Box Bounds Inflating
            Inflate(ref textBoundLeft, ValueBoxBlank, ValueBoxBlank);
            Inflate(ref textBoundRight, ValueBoxBlank, ValueBoxBlank);
            Inflate(ref textBoundTop, ValueBoxBlank, ValueBoxBlank);
            Inflate(ref textBoundBottom, ValueBoxBlank, ValueBoxBlank);

            // Value Box Wrapping
            if (textBoundLeft.Width + Blank * 2 >= Math.Abs(parentMargin.Left))
            {
                if (parentRect.X < 0)
                    textBoundLeft.X = parentMargin.Left - textBoundLeft.Width - Blank * 2;
                else
                    textBoundLeft.X = parentMargin.Left + Blank * 2;

                textPositionLeft.X = textBoundLeft.X + ValueBoxBlank / ScaleX;
            }

            if (textBoundRight.Width + Blank * 2 >= Math.Abs(parentMargin.Right))
            {
                if (parentRect.Right >= RenderSize.Width)
                    textBoundRight.X = parentRect.Right + Blank * 2;
                else
                    textBoundRight.X = parentRect.Right - textBoundRight.Width - Blank * 2;

                textPositionRight.X = textBoundRight.X + ValueBoxBlank / ScaleX;
            }

            if (textBoundTop.Width + Blank * 2 >= Math.Abs(parentMargin.Top))
            {
                if (parentRect.Y < 0)
                    textBoundTop.Y = parentMargin.Top - textBoundTop.Width - Blank * 2;
                else
                    textBoundTop.Y = parentMargin.Top + Blank * 2;

                textPositionTop.Y = textBoundTop.Y + ValueBoxBlank / ScaleX;
            }
            
            if (textBoundBottom.Width + Blank * 2 >= Math.Abs(parentMargin.Bottom))
            {
                if (parentRect.Bottom >= RenderSize.Height)
                    textBoundBottom.Y = parentRect.Bottom + Blank * 2 + textBoundBottom.Width / 2 - textBoundBottom.Height / 2;
                else
                    textBoundBottom.Y = parentRect.Bottom - Blank * 2 - textBoundBottom.Width / 2 - textBoundBottom.Height / 2;

                textPositionBottom.Y = textBoundBottom.Y + ValueBoxBlank / ScaleX;
            }

            // Value Box Render
            if (marginLeft && valueLeft != "0")
                DrawValueBox(dc, formattedTextLeft, textPositionLeft, textBoundLeft, Brushes.White);

            if (marginRight && valueRight != "0")
                DrawValueBox(dc, formattedTextRight, textPositionRight, textBoundRight, Brushes.White);

            if (marginTop && valueTop != "0")
                DrawValueBox(dc, formattedTextTop, textPositionTop, textBoundTop, Brushes.White, 90);

            if (marginBottom && valueBottom != "0")
                DrawValueBox(dc, formattedTextBottom, textPositionBottom, textBoundBottom, Brushes.White, 90);
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

            var formattedText = CreateFormattedText(value, 9, "Verdana", SelectionBrush);

            var textPosition = new Point(
                lineWidth / 2 - formattedText.Width / 2,
                hTop - formattedText.Height / 2);

            var textBound = new Rect(
                textPosition,
                new Size(formattedText.Width, formattedText.Height));

            Inflate(ref textBound, ValueBoxBlank, ValueBoxBlank);

            // Value Box Wrapping
            if (textBound.Width >= lineWidth)
            {
                textBound.Y += (isBottom ? 15d : -15d) / ScaleX;
                textPosition.Y = textBound.Y + ValueBoxBlank;
            }

            DrawValueBox(dc, formattedText, textPosition, textBound, Brushes.White);
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

            var formattedText = CreateFormattedText(value, 9, "Verdana", SelectionBrush);

            var textPosition = new Point(
                vLeft - formattedText.Width / 2,
                lineHeight / 2 - formattedText.Height / 2);

            var textBound = new Rect(
                textPosition,
                new Size(formattedText.Width, formattedText.Height));

            Inflate(ref textBound, ValueBoxBlank, ValueBoxBlank);

            // Value Box Wrapping
            if (textBound.Width >= lineHeight)
            {
                textBound.X += (isRight ? 15d : -15d) / ScaleX;
                textPosition.X = textBound.X + ValueBoxBlank;
            }

            DrawValueBox(dc, formattedText, textPosition, textBound, Brushes.White, 90);
        }

        protected void DrawValueBox(
            DrawingContext dc,
            FormattedText text, Point textPosition,
            Rect bound, Brush background,
            double rotate = 0)
        {
            bool isRotate = (rotate != 0);

            if (isRotate)
                dc.PushTransform(
                    new RotateTransform(rotate,
                    bound.X + bound.Width / 2,
                    bound.Y + bound.Height / 2));

            dc.DrawRectangle(background, null, bound);
            dc.DrawText(text, textPosition);

            if (isRotate)
                dc.Pop();
        }

        protected FormattedText CreateFormattedText(string text, double size, string fontName, Brush brush)
        {
            return new FormattedText(
                text, CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                new Typeface(fontName),
                size / ScaleX,
                brush);
        }

        protected Rect GetParentRenderBound()
        {
            var parentElement = AdornedElement.Parent as FrameworkElement;
            var position = parentElement.TranslatePoint(new Point(), AdornedElement);

            return new Rect(
                position,
                parentElement.RenderSize);
        }

        protected Thickness GetParentRenderMargin()
        {
            var rect = GetParentRenderBound();

            return new Thickness(
                rect.X,
                rect.Y,
                rect.Right - RenderSize.Width,
                rect.Bottom - RenderSize.Height);
        }

        protected void Inflate(ref Rect rect, double x, double y)
        {
            rect.Inflate(x / ScaleX, y / ScaleY);
        }

        private Visibility BoolToVisibility(bool value)
        {
            return value ? Visibility.Visible : Visibility.Collapsed;
        }
        #endregion
    }

    internal struct MarginClipHolder
    {
        public MarginClip LeftClip;
        public MarginClip TopClip;
        public MarginClip RightClip;
        public MarginClip BottomClip;

        public bool Left
        {
            get { return LeftClip.IsChecked.Value; }
            set { LeftClip.IsChecked = value; }
        }

        public bool Top
        {
            get { return TopClip.IsChecked.Value; }
            set { TopClip.IsChecked = value; }
        }

        public bool Right
        {
            get { return RightClip.IsChecked.Value; }
            set { RightClip.IsChecked = value; }
        }

        public bool Bottom
        {
            get { return BottomClip.IsChecked.Value; }
            set { BottomClip.IsChecked = value; }
        }
    }
}