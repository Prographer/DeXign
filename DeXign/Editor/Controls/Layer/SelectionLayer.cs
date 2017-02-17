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
using System.ComponentModel;
using DeXign.Windows.Pages;
using System.Linq;
using DragHelper;
using DeXign.Input;

namespace DeXign.Editor.Layer
{
    public partial class SelectionLayer : StoryboardLayer
    {
        #region [ Dependency Property ]
        public static readonly DependencyProperty IsDraggingProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

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
        public bool IsDragging
        {
            get { return (bool)GetValue(IsDraggingProperty); }
            set { SetValue(IsDraggingProperty, value); }
        }

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

        MoveThumb moveThumb;
        Rectangle frame;
        EventTriggerButton triggerButton;
        Grid resizeGrid;
        Grid clipGrid;

        internal MarginClipHolder ClipData;
        internal bool CancelNextInvert;
        #endregion

        #region [ Constructor ]
        public SelectionLayer(UIElement adornedElement) : base(adornedElement)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                this.Visibility = Visibility.Collapsed;

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

            GroupSelector.Select(this, true);
        }

        private void InitializeComponents()
        {
            AdornedElement.SetDesignMinWidth(5);
            AdornedElement.SetDesignMinHeight(5);

            var scale = new ScaleTransform(
                ParentScale.ScaleX,
                ParentScale.ScaleY);

            #region < Add Move Thumb >
            Add(moveThumb = new MoveThumb(this));
            #endregion

            #region < Add Frame >
            Add(frame = new Rectangle()
            {
                Visibility = Visibility.Collapsed,
                Stroke = Brushes.Red,
                StrokeThickness = 1,
                SnapsToDevicePixels = true
            });
            #endregion

            #region < Add Margin Clips >
            ClipData = new MarginClipHolder();

            Add(clipGrid = new Grid()
            {
                Visibility = Visibility.Collapsed,
                Children =
                {
                    (ClipData.LeftClip = new MarginClip
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
                    (ClipData.TopClip = new MarginClip
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        LayoutTransform = scale
                    }),
                    (ClipData.RightClip = new MarginClip
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
                    (ClipData.BottomClip = new MarginClip
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        LayoutTransform = scale
                    }),
                }
            });
            #endregion

            #region < Add Grips >
            Add(resizeGrid = new Grid()
            {
                Visibility = Visibility.Collapsed,
                Children =
                {
                    new ResizeThumb(this)
                    {
                        ResizeDirection = ResizeGripDirection.TopLeft,
                        Cursor = Cursors.SizeNWSE,
                        Margin = new Thickness(-5, -5, 5, 5),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        RenderTransform = scale,
                        RenderTransformOrigin = new Point(1, 1)
                    },
                    new ResizeThumb(this)
                    {
                        ResizeDirection = ResizeGripDirection.Top,
                        Cursor = Cursors.SizeNS,
                        Margin = new Thickness(0, -5, 0, 5),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Top,
                        RenderTransform = scale,
                        RenderTransformOrigin = new Point(0.5, 1)
                    },
                    new ResizeThumb(this)
                    {
                        ResizeDirection = ResizeGripDirection.TopRight,
                        Cursor = Cursors.SizeNESW,
                        Margin = new Thickness(5, -5, -5, 5),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Top,
                        RenderTransform = scale,
                        RenderTransformOrigin = new Point(0, 1)
                    },
                    new ResizeThumb(this)
                    {
                        ResizeDirection = ResizeGripDirection.Left,
                        Cursor = Cursors.SizeWE,
                        Margin = new Thickness(-5, 0, 5, 0),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                        RenderTransform = scale,
                        RenderTransformOrigin = new Point(1, 0.5)
                    },
                    new ResizeThumb(this)
                    {
                        ResizeDirection = ResizeGripDirection.Right,
                        Cursor = Cursors.SizeWE,
                        Margin = new Thickness(5, 0, -5, 0),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Center,
                        RenderTransform = scale,
                        RenderTransformOrigin = new Point(0, 0.5)
                    },
                    new ResizeThumb(this)
                    {
                        ResizeDirection = ResizeGripDirection.BottomLeft,
                        Cursor = Cursors.SizeNESW,
                        Margin = new Thickness(-5, 5, 5, -5),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        RenderTransform = scale,
                        RenderTransformOrigin = new Point(1, 0)
                    },
                    new ResizeThumb(this)
                    {
                        ResizeDirection = ResizeGripDirection.Bottom,
                        Cursor = Cursors.SizeNS,
                        Margin = new Thickness(0, 5, 0, -5),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        RenderTransform = scale,
                        RenderTransformOrigin = new Point(0.5, 0)
                    },
                    new ResizeThumb(this)
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

            #region < Add Event Trigger Button >
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
                thumb.DragDelta += (s, e) => DragEvent();

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

        private void DragEvent()
        {
            Parent
                .FindLogicalParents<StoryboardPage>()
                .FirstOrDefault()
                .PresentXamlCode();
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

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            // 선택되지 않은 경우 드래그 방지
            if (DesignMode == DesignMode.None)
                e.Handled = true;

            base.OnPreviewMouseLeftButtonDown(e);
        }
        
        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);

            if (CancelNextInvert)
            {
                CancelNextInvert = false;
                return;
            }
            
            if (MoveThumbHitTest(e.GetPosition(this)))
                return;

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

        private bool MoveThumbHitTest(Point position)
        {
            HitTestResult hitResults = VisualTreeHelper.HitTest(this, position);

            return (
                hitResults != null && 
                hitResults.VisualHit != null &&
                (hitResults.VisualHit as FrameworkElement).TemplatedParent != moveThumb);
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

        #region [ Movement ]

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
            var margin = AdornedElement.Margin;
            var parentMargin = GetParentRenderMargin();
            var renderSize = new Size(ActualWidth, ActualHeight);

            parentMargin.Left *= -1;
            parentMargin.Top *= -1;

            AdornedElement.VerticalAlignment = ClipData.VerticalAlignment;
            AdornedElement.HorizontalAlignment = ClipData.HorizontalAlignment;

            #region < Horizontal >
            switch (AdornedElement.HorizontalAlignment)
            {
                case HorizontalAlignment.Stretch:
                    renderSize.Width = double.NaN;
                    margin.Left = parentMargin.Left;
                    margin.Right = parentMargin.Right;
                    break;

                case HorizontalAlignment.Left:
                    margin.Left = parentMargin.Left;
                    margin.Right = 0;
                    break;

                case HorizontalAlignment.Right:
                    margin.Left = 0;
                    margin.Right = parentMargin.Right;
                    break;

                case HorizontalAlignment.Center:
                    margin.Left = 0;
                    margin.Right = 0;
                    break;
            }
            #endregion

            #region < Vertical >
            switch (AdornedElement.VerticalAlignment)
            {
                case VerticalAlignment.Stretch:
                    renderSize.Height = double.NaN;
                    margin.Top = parentMargin.Top;
                    margin.Bottom = parentMargin.Bottom;
                    break;

                case VerticalAlignment.Top:
                    margin.Top = parentMargin.Top;
                    margin.Bottom = 0;
                    break;

                case VerticalAlignment.Bottom:
                    margin.Top = 0;
                    margin.Bottom = parentMargin.Bottom;
                    break;

                case VerticalAlignment.Center:
                    margin.Top = 0;
                    margin.Bottom = 0;
                    break;
            }
            #endregion

            AdornedElement.Margin = margin;
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

            clipGrid.Visibility = (DisplayMargin && DesignMode == DesignMode.Size).ToVisibility();

            if (AdornedElement.Parent is StackPanel)
            {
                var stackPanel = (StackPanel)AdornedElement.Parent;

                // Horizontal
                ClipData.TopVisible = (stackPanel.Orientation == Orientation.Horizontal);
                ClipData.BottomVisible = (stackPanel.Orientation == Orientation.Horizontal);

                // Vertical
                ClipData.LeftVisible = (stackPanel.Orientation == Orientation.Vertical);
                ClipData.RightVisible = (stackPanel.Orientation == Orientation.Vertical);
            }
        }

        private void UpdateMarginClips()
        {
            ClipData.HorizontalAlignment = AdornedElement.HorizontalAlignment;
            ClipData.VerticalAlignment = AdornedElement.VerticalAlignment;
        }
        #endregion
    }

    internal struct MarginClipHolder
    {
        public MarginClip LeftClip;
        public MarginClip TopClip;
        public MarginClip RightClip;
        public MarginClip BottomClip;

        public HorizontalAlignment HorizontalAlignment
        {
            get
            {
                if (Left && Right)
                    return HorizontalAlignment.Stretch;

                if (!Left && Right)
                    return HorizontalAlignment.Right;

                if (Left && !Right)
                    return HorizontalAlignment.Left;

                return HorizontalAlignment.Center;
            }
            set
            {
                switch (value)
                {
                    case HorizontalAlignment.Stretch:
                        Left = true;
                        Right = true;
                        break;

                    case HorizontalAlignment.Right:
                        Left = false;
                        Right = true;
                        break;

                    case HorizontalAlignment.Left:
                        Left = true;
                        Right = false;
                        break;

                    case HorizontalAlignment.Center:
                        Left = false;
                        Right = false;
                        break;
                }
            }
        }

        public VerticalAlignment VerticalAlignment
        {
            get
            {
                if (Top && Bottom)
                    return VerticalAlignment.Stretch;

                if (!Top && Bottom)
                    return VerticalAlignment.Bottom;

                if (Top && !Bottom)
                    return VerticalAlignment.Top;

                return VerticalAlignment.Center;
            }
            set
            {
                switch (value)
                {
                    case VerticalAlignment.Stretch:
                        Top = true;
                        Bottom = true;
                        break;

                    case VerticalAlignment.Bottom:
                        Top = false;
                        Bottom = true;
                        break;

                    case VerticalAlignment.Top:
                        Top = true;
                        Bottom = false;
                        break;

                    case VerticalAlignment.Center:
                        Top = false;
                        Bottom = false;
                        break;
                }
            }
        }

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
        
        public bool LeftVisible
        {
            get { return LeftClip.Visibility == Visibility.Visible; }
            set { LeftClip.Visibility = value.ToVisibility(); }
        }

        public bool RightVisible
        {
            get { return RightClip.Visibility == Visibility.Visible; }
            set { RightClip.Visibility = value.ToVisibility(); }
        }

        public bool TopVisible
        {
            get { return TopClip.Visibility == Visibility.Visible; }
            set { TopClip.Visibility = value.ToVisibility(); }
        }

        public bool BottomVisible
        {
            get { return BottomClip.Visibility == Visibility.Visible; }
            set { BottomClip.Visibility = value.ToVisibility(); }
        }
    }
}