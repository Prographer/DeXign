using System;
using System.Windows;
using System.Windows.Shell;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using DeXign.Converter;
using DeXign.Extension;
using DeXign.Resources;
using DeXign.Editor.Controls;

using WPFExtension;

namespace DeXign.Editor.Layer
{
    partial class SelectionLayer : StoryboardLayer
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

        public static readonly DependencyProperty FrameBrushProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(Brushes.Blue, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty FrameThicknessProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

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

        public Brush FrameBrush
        {
            get { return (Brush)GetValue(FrameBrushProperty); }
            set { SetValue(FrameBrushProperty, value); }
        }

        public double FrameThickness
        {
            get { return (double)GetValue(FrameThicknessProperty); }
            set { SetValue(FrameThicknessProperty, value); }
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
            FrameBrush = ResourceManager.GetBrush("Accent");

            // 스냅라인 등록
            Parent.GuideLayer.Add(this);
            
            UpdateParentState();
        }

        private void InitializeSelector()
        {
            this.AddSelectedHandler(OnSelected);
            this.AddUnselectedHandler(OnUnselected);
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

            // ParentScale X -> scale X
            BindingEx.SetBinding(
                ParentScale, ScaleTransform.ScaleXProperty,
                scale, ScaleTransform.ScaleXProperty,
                converter: reciprocalConverter);

            // ParentScale Y -> scale Y
            BindingEx.SetBinding(
                ParentScale, ScaleTransform.ScaleYProperty,
                scale, ScaleTransform.ScaleYProperty,
                converter: reciprocalConverter);

            // ParentScale X -> frame StrokeThickness
            BindingEx.SetBinding(
                ParentScale, ScaleTransform.ScaleXProperty,
                frame, Shape.StrokeThicknessProperty,
                converter: reciprocalConverter);

            // SelectionBrush -> triggerButton Background
            BindingEx.SetBinding(
                this, SelectionBrushProperty,
                triggerButton, Control.BackgroundProperty);

            // SelectionBrush -> frame Stroke
            BindingEx.SetBinding(
                this, SelectionBrushProperty,
                frame, Shape.StrokeProperty);
            
            foreach (ResizeThumb thumb in resizeGrid.Children)
            {
                // SelectionBrush -> thumb Stroke
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
        
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
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

        protected virtual void OnDesignModeChanged()
        {
            if (DesignMode != DesignMode.None)
                AnimateFrameThickness(0, 150);

            UpdateParentState();
            UpdateMarginClips();
            UpdateFrame();
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