using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Shell;

using DeXign.Converter;
using DeXign.Editor.Controls;
using DeXign.Editor.Renderer;
using DeXign.Extension;
using DeXign.Resources;

using WPFExtension;
using DeXign.Controls;

namespace DeXign.Editor.Layer
{
    public partial class SelectionLayer : StoryboardLayer
    {
        public event EventHandler DesignModeChanged;

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
                new FrameworkPropertyMetadata(DesignMode.None, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty IsHighlightProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty HighlightBrushProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(Brushes.Red, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region [ Property ]
        /// <summary>
        /// 렌더러 대상입니다.
        /// </summary>
        public new FrameworkElement AdornedElement
        {
            get { return (FrameworkElement)base.AdornedElement; }
        }

        /// <summary>
        /// Margin의 상태를 설정하거나 가져옵니다.
        /// </summary>
        public bool DisplayMargin
        {
            get { return (bool)GetValue(DisplayMarginProperty); }
            set { SetValue(DisplayMarginProperty, value); }
        }

        /// <summary>
        /// 윗부분 Width 가이드라인의 상태를 설정하거나 가져옵니다.
        /// </summary>
        public bool DisplayWidthTop
        {
            get { return (bool)GetValue(DisplayWidthTopProperty); }
            set { SetValue(DisplayWidthTopProperty, value); }
        }

        /// <summary>
        /// 바닥부분 Width 가이드라인의 상태를 설정하거나 가져옵니다.
        /// </summary>
        public bool DisplayWidthBottom
        {
            get { return (bool)GetValue(DisplayWidthBottomProperty); }
            set { SetValue(DisplayWidthBottomProperty, value); }
        }

        /// <summary>
        /// 왼쪽 Height 가이드라인의 상태를 설정하거나 가져옵니다.
        /// </summary>
        public bool DisplayHeightLeft
        {
            get { return (bool)GetValue(DisplayHeightLeftProperty); }
            set { SetValue(DisplayHeightLeftProperty, value); }
        }

        /// <summary>
        /// 오른쪽 Height 가이드라인의 상태를 설정하거나 가져옵니다.
        /// </summary>
        public bool DisplayHeightRight
        {
            get { return (bool)GetValue(DisplayHeightRightProperty); }
            set { SetValue(DisplayHeightRightProperty, value); }
        }

        /// <summary>
        /// 강조 색상을 설정하거나 가져옵니다.
        /// </summary>
        public Brush SelectionBrush
        {
            get { return (Brush)GetValue(SelectionBrushProperty); }
            set { SetValue(SelectionBrushProperty, value); }
        }

        /// <summary>
        /// 렌더러의 프레임 색상을 설정하거나 가져옵니다.
        /// </summary>
        public Brush FrameBrush
        {
            get { return (Brush)GetValue(FrameBrushProperty); }
            set { SetValue(FrameBrushProperty, value); }
        }

        /// <summary>
        /// 렌더러의 프레임 두께를 설정하거나 가져옵니다.
        /// </summary>
        public double FrameThickness
        {
            get { return (double)GetValue(FrameThicknessProperty); }
            set { SetValue(FrameThicknessProperty, value); }
        }

        /// <summary>
        /// 디자인 모드를 가져오거나 설정할 수 있습니다.
        /// </summary>
        public DesignMode DesignMode
        {
            get { return (DesignMode)GetValue(DesignModeProperty); }
            set { SetValue(DesignModeProperty, value); }
        }

        public bool IsHighlight
        {
            get { return (bool)GetValue(IsHighlightProperty); }
            set { SetValue(IsHighlightProperty, value); }
        }

        public Brush HighlightBrush
        {
            get { return (Brush)GetValue(HighlightBrushProperty); }
            set { SetValue(HighlightBrushProperty, value); }
        }

        /// <summary>
        /// 부모 레이어 렌더러를 가져옵니다.
        /// </summary>
        public new IRenderer Parent { get; private set;}
        #endregion

        #region [ Local Variable ]
        const double Blank = 6;
        const double ValueBoxBlank = 2;

        LayerMoveThumb moveThumb;
        Rectangle frame;
        Grid resizeGrid;
        Grid clipGrid;
        bool cancelNextInvert;

        internal LayerEventTriggerButton TriggerButton;
        internal MarginClipHolder ClipData;
        #endregion

        #region [ Constructor ]
        public SelectionLayer(UIElement adornedElement) : base(adornedElement)
        {
            GuidelineFilter.Push(GetSizeGuidableLines);
        }

        // Element.Loaded -> OnLoaded
        protected override void OnLoaded(FrameworkElement adornedElement)
        {
#if DEBUG
            if (DesignerProperties.GetIsInDesignMode(this))
                this.Visibility = Visibility.Collapsed;

#endif

            // 디자인 모드 변경 이벤트 등록
            DesignModeProperty.AddValueChanged(this, DesignMode_Changed);

            // Element - VerticalAlignment
            VerticalAlignmentProperty.AddValueChanged(AdornedElement, AlignmentChanged);

            // Element - HorizontalAlignment
            HorizontalAlignmentProperty.AddValueChanged(AdornedElement, AlignmentChanged);

            InitializeComponents();
            InitializeSelector();

            Parent = AdornedElement.Parent.GetRenderer();

            SelectionBrush = ResourceManager.GetBrush("Flat.Accent.Dark");
            FrameBrush = ResourceManager.GetBrush("Flat.Accent.Light");
            HighlightBrush = ResourceManager.GetBrush("Flat.Accent.DeepDark");

            // 스냅라인 등록
            Storyboard.GuideLayer.Add(this);

            UpdateParentState();
        }

        private void AlignmentChanged(object sender, EventArgs e)
        {
            UpdateMarginClips();
        }

        protected override void OnDisposed()
        {
            // 스냅라인 등록 해제
            Storyboard.GuideLayer.Remove(this);

            // GroupSelector
            this.RemoveSelectedHandler(OnSelected);
            this.RemoveUnselectedHandler(OnUnselected);

            // Design Mode
            DesignModeProperty.RemoveValueChanged(this, DesignMode_Changed);

            // clips
            foreach (LayerMarginClip clip in clipGrid.Children)
                ToggleButton.IsCheckedProperty.RemoveValueChanged(clip, ClipChanged);
        }

        private void InitializeSelector()
        {
            this.AddSelectedHandler(OnSelected);
            this.AddUnselectedHandler(OnUnselected);

            if (!DesignTime.IsLocked(this))
                GroupSelector.Select(this, true);
        }

        private void InitializeComponents()
        {
            AdornedElement.SetDesignMinWidth(5);
            AdornedElement.SetDesignMinHeight(5);

            var scale = new ScaleTransform(Scale, Scale);

            #region < Add Move Thumb >
            Add(moveThumb = new LayerMoveThumb(this));

            moveThumb.DragCompleted += ThumbOnDragCompleted;
            moveThumb.Moved += MoveThumb_Moved;

            // Selection
            moveThumb.PreviewMouseLeftButtonDown += MoveThumb_PreviewMouseLeftButtonDown;
            moveThumb.PreviewMouseLeftButtonUp += MoveThumb_PreviewMouseLeftButtonUp;
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
                    (ClipData.LeftClip = new LayerMarginClip
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
                    (ClipData.TopClip = new LayerMarginClip
                    {
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        LayoutTransform = scale
                    }),
                    (ClipData.RightClip = new LayerMarginClip
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
                    (ClipData.BottomClip = new LayerMarginClip
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
                    new LayerResizeThumb(this)
                    {
                        ResizeDirection = ResizeGripDirection.TopLeft,
                        Cursor = Cursors.SizeNWSE,
                        Margin = new Thickness(-5, -5, 5, 5),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,
                        RenderTransform = scale,
                        RenderTransformOrigin = new Point(1, 1)
                    },
                    new LayerResizeThumb(this)
                    {
                        ResizeDirection = ResizeGripDirection.Top,
                        Cursor = Cursors.SizeNS,
                        Margin = new Thickness(0, -5, 0, 5),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Top,
                        RenderTransform = scale,
                        RenderTransformOrigin = new Point(0.5, 1)
                    },
                    new LayerResizeThumb(this)
                    {
                        ResizeDirection = ResizeGripDirection.TopRight,
                        Cursor = Cursors.SizeNESW,
                        Margin = new Thickness(5, -5, -5, 5),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Top,
                        RenderTransform = scale,
                        RenderTransformOrigin = new Point(0, 1)
                    },
                    new LayerResizeThumb(this)
                    {
                        ResizeDirection = ResizeGripDirection.Left,
                        Cursor = Cursors.SizeWE,
                        Margin = new Thickness(-5, 0, 5, 0),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center,
                        RenderTransform = scale,
                        RenderTransformOrigin = new Point(1, 0.5)
                    },
                    new LayerResizeThumb(this)
                    {
                        ResizeDirection = ResizeGripDirection.Right,
                        Cursor = Cursors.SizeWE,
                        Margin = new Thickness(5, 0, -5, 0),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        VerticalAlignment = VerticalAlignment.Center,
                        RenderTransform = scale,
                        RenderTransformOrigin = new Point(0, 0.5)
                    },
                    new LayerResizeThumb(this)
                    {
                        ResizeDirection = ResizeGripDirection.BottomLeft,
                        Cursor = Cursors.SizeNESW,
                        Margin = new Thickness(-5, 5, 5, -5),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        RenderTransform = scale,
                        RenderTransformOrigin = new Point(1, 0)
                    },
                    new LayerResizeThumb(this)
                    {
                        ResizeDirection = ResizeGripDirection.Bottom,
                        Cursor = Cursors.SizeNS,
                        Margin = new Thickness(0, 5, 0, -5),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Bottom,
                        RenderTransform = scale,
                        RenderTransformOrigin = new Point(0.5, 0)
                    },
                    new LayerResizeThumb(this)
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
            Add(TriggerButton = new LayerEventTriggerButton(this)
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
                Zoom, ZoomPanel.ScaleProperty,
                scale, ScaleTransform.ScaleXProperty,
                converter: reciprocalConverter);

            // ParentScale Y -> scale Y
            BindingEx.SetBinding(
                Zoom, ZoomPanel.ScaleProperty,
                scale, ScaleTransform.ScaleYProperty,
                converter: reciprocalConverter);

            // ParentScale X -> frame StrokeThickness
            BindingEx.SetBinding(
                Zoom, ZoomPanel.ScaleProperty,
                frame, Shape.StrokeThicknessProperty,
                converter: reciprocalConverter);

            // SelectionBrush -> triggerButton Background
            BindingEx.SetBinding(
                this, SelectionBrushProperty,
                TriggerButton, Control.BackgroundProperty);

            // SelectionBrush -> frame Stroke
            BindingEx.SetBinding(
                this, SelectionBrushProperty,
                frame, Shape.StrokeProperty);
            
            foreach (LayerResizeThumb thumb in resizeGrid.Children)
            {
                // SelectionBrush -> thumb Stroke
                BindingEx.SetBinding(
                    this, SelectionBrushProperty,
                    thumb, LayerResizeThumb.StrokeProperty);

                thumb.DragStarted += ThumbOnDragStarted;
                thumb.DragCompleted += ThumbOnDragCompleted;
            }

            // caching
            UpdateMarginClips();

            foreach (LayerMarginClip clip in clipGrid.Children)
                ToggleButton.IsCheckedProperty.AddValueChanged(clip, ClipChanged);
            #endregion
        }
        #endregion

        #region [ Selection ]
        internal void CancelNextSelect()
        {
            cancelNextInvert = true;
        }

        private void MoveThumb_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.Focus(Storyboard);

            if (!GroupSelector.IsSelected(this))
            {
                cancelNextInvert = true;
                Select();
            }
        }
        
        private void MoveThumb_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (cancelNextInvert)
            {
                cancelNextInvert = false;
                return;
            }

            Select();
        }

        public void Select()
        {
            // Design Mode Change
            if (GroupSelector.IsSelected(this))
                InvertDesignMode();

            // Select
            GroupSelector.Select(this, true,
                multiSelect: Keyboard.IsKeyDown(Key.LeftShift));
        }

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
            // Clear Snapped Guidelines
            Storyboard.GuideLayer.ClearSnappedGuidelines();

            DisplayWidthTop = false;
            DisplayWidthBottom = false;
            DisplayHeightLeft = false;
            DisplayHeightRight = false;
        }

        private void ThumbOnDragStarted(object sender, DragStartedEventArgs dragStartedEventArgs)
        {
            var thumb = (LayerResizeThumb)sender;

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
        private void RaiseDesignModeChanged()
        {
            DesignModeChanged?.Invoke(this, null);
        }

        private void MoveThumb_Moved(object sender, EventArgs e)
        {
            RaiseInvalidatedLayout();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            RaiseInvalidatedLayout();
        }

        private void ClipChanged(object sender, EventArgs e)
        {
            ApplyClipData();
        }

        protected void ApplyClipData()
        {
            var margin = AdornedElement.Margin;
            var parentMargin = GetParentRenderMargin();
            var renderSize = AdornedElement.RenderSize;

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

            SetMargin(margin, false);
            SetSize(renderSize.Width, renderSize.Height);

            this.InvalidateVisual();
        }

        private void DesignMode_Changed(object sender, EventArgs e)
        {
            OnDesignModeChanged();

            RaiseDesignModeChanged();
            RaiseInvalidatedLayout();
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
            TriggerButton.Visibility = Visibility.Collapsed;

            if (DesignMode != DesignMode.None)
            {
                frame.Visibility = Visibility.Visible;

                if (DesignMode == DesignMode.Size)
                    resizeGrid.Visibility = Visibility.Visible;
                else
                    TriggerButton.Visibility = Visibility.Visible;
            }

            this.InvalidateVisual();
        }

        private void UpdateParentState()
        {
            DisplayMargin = !(this.Parent is IStoryboard);

            clipGrid.Visibility = (DisplayMargin && DesignMode == DesignMode.Size).ToVisibility();

            if (this.Parent is IStackLayout)
            {
                var stackPanel = (StackPanel)AdornedElement.Parent;

                // Stack Vertical
                ClipData.LeftVisible = (stackPanel.Orientation == Orientation.Vertical);
                ClipData.RightVisible = (stackPanel.Orientation == Orientation.Vertical);

                // Stack Horizontal
                ClipData.TopVisible = (stackPanel.Orientation == Orientation.Horizontal);
                ClipData.BottomVisible = (stackPanel.Orientation == Orientation.Horizontal);
            }
        }

        internal void UpdateMarginClips()
        {
            ClipData.HorizontalAlignment = AdornedElement.HorizontalAlignment;
            ClipData.VerticalAlignment = AdornedElement.VerticalAlignment;
        }
        #endregion

        #region [ Element Dependent ]
        public void SetSize(double width, double height)
        {
            SetWidth(width);
            SetHeight(height);
        }

        public void SetMargin(Thickness margin, bool snap = true)
        {
            // Update Snap Line
            if (snap)
                MarginSnap(ref margin);

            AdornedElement.Margin = margin.Clean();
        }
        
        public void SetWidth(double width)
        {
            if (double.IsNaN(width) || AdornedElement.HorizontalAlignment != HorizontalAlignment.Stretch)
                AdornedElement.Width = Math.Round(width, 2);
        }

        public void SetHeight(double height)
        {
            if (double.IsNaN(height) || AdornedElement.VerticalAlignment != VerticalAlignment.Stretch)
                AdornedElement.Height = Math.Round(height, 2);
        }
        #endregion

        public void DragMove()
        {
            moveThumb.CaptureMouse();
        }
    }

    internal struct MarginClipHolder
    {
        public LayerMarginClip LeftClip;
        public LayerMarginClip TopClip;
        public LayerMarginClip RightClip;
        public LayerMarginClip BottomClip;

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