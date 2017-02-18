using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace DeXign.Extension
{
    class SmoothScrollBehavior : IDisposable
    {
        enum ScrollAction
        {
            Wheel,
            Mouse,
            Track
        }

        #region 속성
        public UIElement Parent { get; private set; }
        public ScrollViewer Viewer { get; private set; }

        internal ScrollContentPresenter ContentPresenter { get; set; }
        internal ScrollBar ScrollBar { get; set; }
        internal double StepSize { get; set; } = 48;
        #endregion

        #region 글로벌 데이터
        DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.SystemIdle)
        {
            Interval = TimeSpan.FromMilliseconds(1)
        };

        // Scroll
        ScrollAction lastScroll = ScrollAction.Wheel;
        double acceleration = 0.1;
        double virtualScrollableHeight = 0;
        double virtualOffset = 0;
        double toOffset = 0;

        int lastDelta = 0;

        // Mouse Wheel
        bool isBounding = false;

        // Mouse Grap
        List<RepeatButtonHolder> repeatHolders = new List<RepeatButtonHolder>();

        double beginValue;
        Point beginPosition;
        bool isHold = false;
        #endregion

        public SmoothScrollBehavior(UIElement parent, ScrollViewer target)
        {
            int c = target.FindVisualChildrens<ScrollBar>().Count();

            this.Parent = parent;
            this.Viewer = target;
            this.ScrollBar = Viewer.FindVisualChildrens<ScrollBar>().FirstOrDefault();
            this.ContentPresenter = Viewer.FindVisualChildrens<ScrollContentPresenter>().FirstOrDefault();
            
            this.Viewer.PreviewMouseWheel += Viewer_PreviewMouseWheel;
            this.ScrollBar.PreviewMouseDown += ScrollBar_PreviewMouseDown;
            this.ScrollBar.PreviewMouseUp += ScrollBar_PreviewMouseUp;
            this.ScrollBar.PreviewMouseMove += ScrollBar_PreviewMouseMove;
            
            timer.Tick += LazyScroll;
            timer.Start();
        }

        #region 스크롤바 관련
        private double GetScrollRatio()
        {
            return ScrollBar.Maximum / GetScrollTrackLength();
        }

        private double GetScrollTrackLength()
        {
            return ScrollBar.Track.ActualHeight - ScrollBar.Track.Thumb.ActualHeight;
        }

        private double OffsetLimit(double offset)
        {
            return Math.Min(Math.Max(offset, 0), ScrollBar.Maximum);
        }

        private double BoundingOffsetLimit(double offset)
        {
            double availableDistance = Viewer.ActualHeight * 0.7;

            offset = Math.Min(offset, virtualScrollableHeight + availableDistance);
            offset = Math.Max(offset, -availableDistance);

            return offset;
        }
        #endregion

        #region 스크롤바 스크롤       
        private void ScrollBar_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (isHold)
            {
                // Scroll Factor
                acceleration = 0.2;
                isBounding = false;

                // Scroll Target
                Point delta = Mouse.GetPosition(ScrollBar) - (Vector)beginPosition;
                
                toOffset = OffsetLimit(beginValue + delta.Y * GetScrollRatio());
                
                ScrollBar.Value = toOffset;
            }
        }

        private void ScrollBar_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            lock (repeatHolders)
            {
                repeatHolders.Clear();
            }

            isHold = false;

            if (Mouse.Captured == ScrollBar)
            {
                e.Handled = true;
                Mouse.Capture(null);
            }
        }

        private void ScrollBar_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Visual source = e.OriginalSource as Visual;
            
            if (source.FindVisualParents<Thumb>(false).Count() > 0)
            {
                if (isHold = Mouse.Capture(ScrollBar as IInputElement))
                {
                    e.Handled = true;

                    // Scroll Factor
                    lastScroll = ScrollAction.Mouse;
                    beginValue = ScrollBar.Value;
                    beginPosition = Mouse.GetPosition(ScrollBar);
                }
            }
            else
            {
                RepeatButtonHolder holder = null;
                RepeatButton repeatButton = source
                    .FindVisualParents<RepeatButton>(false)
                    .FirstOrDefault();

                var pos = Mouse.GetPosition(ScrollBar.Track);

                if (repeatButton == ScrollBar.Track.IncreaseRepeatButton || // Down Track
                    repeatButton == ScrollBar.Track.DecreaseRepeatButton)   // Up Track
                {
                    e.Handled = true;

                    double targetOffset = pos.Y * GetScrollRatio();
                    double thumbOffset = ScrollBar.Track.Thumb.ActualHeight * GetScrollRatio();

                    if (targetOffset > virtualOffset)
                        targetOffset -= thumbOffset;

                    targetOffset = OffsetLimit(targetOffset);
                    
                    holder = new RepeatButtonHolder(repeatButton);
                    holder.OnClick += delegate
                    {
                        // Scroll Factor
                        isBounding = false;
                        acceleration = 0.15;
                        lastScroll = ScrollAction.Track;

                        // Target Offset
                        double delta = targetOffset - toOffset;
                        int sDelta = Math.Sign(delta);

                        double offset = toOffset + thumbOffset * sDelta;
                        
                        if (Math.Abs(targetOffset - offset) < thumbOffset)
                        {
                            toOffset = targetOffset;
                        }
                        else
                        {
                            toOffset = offset;
                        }
                    };
                }
                else
                {
                    // Small RepeatButton
                    e.Handled = true;

                    if (pos.Y < ScrollBar.Track.ActualHeight / 2) // Up Button
                    {
                        holder = new RepeatButtonHolder(repeatButton);
                        holder.OnClick += delegate
                        {   
                            // Scroll Factor
                            isBounding = false;
                            acceleration = 0.15;
                            lastScroll = ScrollAction.Track;

                            toOffset -= StepSize / 3;
                            toOffset = OffsetLimit(toOffset);
                        };
                    }
                    else // Down Button
                    {
                        holder = new RepeatButtonHolder(repeatButton);
                        holder.OnClick += delegate
                        {
                            // Scroll Factor
                            isBounding = false;
                            acceleration = 0.15;
                            lastScroll = ScrollAction.Track;

                            toOffset += StepSize / 3;
                            toOffset = OffsetLimit(toOffset);
                        };
                    }
                }

                if (holder != null)
                {
                    holder.Start();
                    repeatHolders.Add(holder);
                }
            }
        }
        #endregion

        #region 마우스 휠 스크롤
        private void Viewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!isHold)
            {
                e.Handled = true;

                // Scroll Factor
                acceleration = 0.1;
                isBounding = false;
                lastScroll = ScrollAction.Wheel;

                // Scroll Direction
                int sDelta = -Math.Sign(e.Delta);
                if (sDelta != lastDelta)
                {
                    toOffset = virtualOffset;
                }

                lastDelta = sDelta;

                // Target Offset & Bounding
                toOffset = BoundingOffsetLimit(toOffset + sDelta * StepSize);
            }
        }
        #endregion

        #region Smooth Lazy Scroll
        private void LazyScroll(object sender, EventArgs e)
        {
            OnRepeatButton();
            OnLazyScroll();
        }

        protected virtual void OnLazyScroll()
        {
            virtualScrollableHeight = Viewer.ScrollableHeight - ContentPresenter.Margin.Bottom - ContentPresenter.Margin.Top;

            double delta = (toOffset - virtualOffset).Clean();
            double offset = virtualOffset + delta * acceleration;

            double remainBottomOffset = Math.Max(offset - virtualScrollableHeight, 0).Clean();
            double remainTopOffset = Math.Min(offset, 0);

            ContentPresenter.Margin = new Thickness(0, -remainTopOffset, 0, remainBottomOffset);

            virtualOffset = offset;

            if (lastScroll != ScrollAction.Mouse)
                ScrollBar.Value = virtualOffset;

            if (isBounding)
            {
                double tValue = (virtualScrollableHeight - toOffset);

                if (remainBottomOffset > 0)
                    Viewer.ScrollToVerticalOffset(Viewer.ScrollableHeight);

                if (remainTopOffset < 0)
                {
                    Viewer.ScrollToVerticalOffset(0);
                    tValue = -toOffset;
                }

                toOffset += tValue * acceleration;
            }
            else
            {
                Viewer.ScrollToVerticalOffset(offset);

                if (remainBottomOffset > 0 || remainTopOffset < 0)
                {
                    isBounding = true;
                }
            }
        }

        protected virtual void OnRepeatButton()
        {
            lock (repeatHolders)
            {
                foreach (var holder in repeatHolders)
                    holder.Update();
            }
        }
        #endregion
        
        #region IDisposable
        private bool isDisposed = false;

        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;

                timer.Stop();
                timer.Tick -= LazyScroll;
                timer = null;

                this.Viewer.PreviewMouseWheel -= Viewer_PreviewMouseWheel;
                this.Viewer = null;
                this.Parent = null;
            }
        }
        #endregion
    }
}