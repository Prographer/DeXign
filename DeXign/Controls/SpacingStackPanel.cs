using System;
using System.Windows;
using System.Windows.Controls;

using WPFExtension;

namespace DeXign.Controls
{
    class SpacingStackPanel : StackPanel
    {
        public static readonly DependencyProperty SpacingProperty =
            DependencyHelper.Register(
                new PropertyMetadata(10d));

        public double Spacing
        {
            get { return (double)GetValue(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }

        internal bool IsVertical
        {
            get { return Orientation == Orientation.Vertical; }
        }

        public SpacingStackPanel()
        {
            SpacingProperty.AddValueChanged(this, Spacing_Changed);
        }

        private void Spacing_Changed(object sender, EventArgs e)
        {
            this.InvalidateMeasure();
            this.InvalidateArrange();
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Size stackDesiredSize = new Size();
            Size layoutSlotSize = constraint;
                        
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

            return stackDesiredSize;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            var rcChild = new Rect(arrangeSize);
            double previousChildSize = 0.0;
            
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
                    rcChild.Width = Math.Max(arrangeSize.Width, child.DesiredSize.Width);
                }
                else
                {
                    rcChild.X += previousChildSize + space;
                    previousChildSize = child.DesiredSize.Width;

                    rcChild.Width = previousChildSize;
                    rcChild.Height = Math.Max(arrangeSize.Height, child.DesiredSize.Height);
                }

                child.Arrange(rcChild);
            }

            return arrangeSize;
        }
    }
}
