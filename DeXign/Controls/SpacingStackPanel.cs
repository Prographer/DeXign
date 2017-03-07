using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using WPFExtension;

namespace DeXign.Controls
{
    public class SpacingStackPanel : StackPanel
    {
        public static readonly DependencyProperty SpacingProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(
                    10d,
                    FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty PaddingProperty =
            DependencyHelper.Register(
                new FrameworkPropertyMetadata(
                    new Thickness(),
                     FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender));

        public double Spacing
        {
            get { return (double)GetValue(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }

        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        internal bool IsVertical
        {
            get { return Orientation == Orientation.Vertical; }
        }

        internal Dictionary<object, Rect> lastArrangedBounds;

        public SpacingStackPanel()
        {
            lastArrangedBounds = new Dictionary<object, Rect>();
        }

        internal Rect GetArrangedBound(FrameworkElement element)
        {
            if (!lastArrangedBounds.ContainsKey(element))
                return new Rect(-1, -1, -1, -1);

            return lastArrangedBounds[element];
        }

        private Size CollapseThickness(Thickness thickness)
        {
            return new Size(
                thickness.Left + thickness.Right, 
                thickness.Top + thickness.Bottom);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Size stackDesiredSize = new Size();
            Size layoutSlotSize = constraint;

            Size padding = CollapseThickness(this.Padding);

            constraint.Width = Math.Max(0, constraint.Width - padding.Width);
            constraint.Height = Math.Max(0, constraint.Height - padding.Height);

            if (IsVertical)
            {
                layoutSlotSize.Height = double.PositiveInfinity;

                if (this.CanHorizontallyScroll)
                    layoutSlotSize.Width = double.PositiveInfinity;
            }
            else
            {
                layoutSlotSize.Width = double.PositiveInfinity;

                if (this.CanVerticallyScroll)
                    layoutSlotSize.Height = double.PositiveInfinity;
            }
            
            for (int i = 0, count = this.Children.Count; i < count; ++i)
            {
                UIElement child = this.Children[i];

                if (child == null)
                    continue;
                
                child.Measure(layoutSlotSize);
                Size childDesiredSize = child.DesiredSize;

                if (IsVertical)
                {
                    stackDesiredSize.Width = Math.Max(stackDesiredSize.Width, childDesiredSize.Width);
                    stackDesiredSize.Height += childDesiredSize.Height + Spacing;
                    
                    if (i == this.Children.Count - 1)
                        stackDesiredSize.Height -= Spacing;
                }
                else
                {
                    stackDesiredSize.Width += childDesiredSize.Width + Spacing;
                    stackDesiredSize.Height = Math.Max(stackDesiredSize.Height, childDesiredSize.Height);
                    
                    if (i == this.Children.Count - 1)
                        stackDesiredSize.Width -= Spacing;
                }
            }

            stackDesiredSize.Width += padding.Width;
            stackDesiredSize.Height += padding.Height;

            return stackDesiredSize;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var rcChild = new Rect(
                new Point(this.Padding.Left, this.Padding.Top),
                arrangeSize);

            double previousChildSize = 0.0;

            Size padding = CollapseThickness(this.Padding);

            for (int i = 0, count = this.Children.Count; i < count; ++i)
            {
                UIElement child = this.Children[i];

                if (child == null)
                    continue;

                double space = i > 0 ? Spacing : 0;

                if (IsVertical)
                {
                    rcChild.Y += previousChildSize + space;
                    previousChildSize = child.DesiredSize.Height;

                    rcChild.Height = previousChildSize;
                    rcChild.Width = Math.Max(Math.Max(arrangeSize.Width, child.DesiredSize.Width) - padding.Width, 0);
                }
                else
                {
                    rcChild.X += previousChildSize + space;
                    previousChildSize = child.DesiredSize.Width;

                    rcChild.Width = previousChildSize;
                    rcChild.Height = Math.Max(Math.Max(arrangeSize.Height, child.DesiredSize.Height) - padding.Height, 0);
                }

                // chaching
                lastArrangedBounds[child] = rcChild;

                child.Arrange(rcChild);
            }

            return arrangeSize;
        }
    }
}
