using DeXign.Editor.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DeXign.Editor.Layer
{
    partial class SelectionLayer : StoryboardLayer, IGuideProvider
    {
        internal Stack<Func<IEnumerable<Guideline>>> GuidelineFilter =
            new Stack<Func<IEnumerable<Guideline>>>();

        public IEnumerable<Guideline> GetGuidableLines()
        {
            if (GuidelineFilter.Count > 0)
                foreach (Guideline g in GuidelineFilter.Peek().Invoke())
                    yield return g;
        }

        internal IEnumerable<Guideline> GetSizeGuidableLines()
        {
            Point relative = AdornedElement.TranslatePoint(new Point(), RootParent);

            // left
            yield return new Guideline(
                relative,
                new Point(relative.X, relative.Y + AdornedElement.RenderSize.Height))
            {
                Direction = GuidelineDirection.Left
            };

            // right
            yield return new Guideline(
                new Point(relative.X + AdornedElement.RenderSize.Width, relative.Y),
                Point.Add(relative, (Vector)AdornedElement.RenderSize))
            {
                Direction = GuidelineDirection.Right
            };

            // top
            yield return new Guideline(
                relative,
                new Point(relative.X + AdornedElement.RenderSize.Width, relative.Y))
            {
                Direction = GuidelineDirection.Top
            };

            // bottom
            yield return new Guideline(
                new Point(relative.X, relative.Y + AdornedElement.RenderSize.Height),
                Point.Add(relative, (Vector)AdornedElement.RenderSize))
            {
                Direction = GuidelineDirection.Bottom
            };
        }

        private void MarginSnap(ref Thickness margin)
        {
            Guideline[] items = RootParent.GuideLayer.InvalidateSnappedGuidelines(this).ToArray();
            Size parentSize = Parent.Element.RenderSize;
            Size size = AdornedElement.RenderSize;

            var bound = GetParentRenderBound();

            bool allowVertical = true;
            bool allowHorizontal = true;

            if (Parent is IStackLayout)
            {
                var stackPanel = Parent.Element as StackPanel;

                allowVertical =
                    (stackPanel.Orientation == Orientation.Horizontal);

                allowHorizontal =
                    (stackPanel.Orientation == Orientation.Vertical);
            }

            for (int i = 0; i < items.Length; i++)
            {
                Point pRelative = RootParent.TranslatePoint(items[i].Point1, Parent.Element);
                GuidelineDirection direction = items[i].SnappedGuideline.Direction;

                switch (direction)
                {
                    case GuidelineDirection.Left:
                    case GuidelineDirection.Right:
                        if (!allowHorizontal)
                            break;

                        switch (ClipData.HorizontalAlignment)
                        {
                            case HorizontalAlignment.Left:
                                margin.Left = pRelative.X;
                                if (direction == GuidelineDirection.Right)
                                    margin.Left -= size.Width;
                                break;

                            case HorizontalAlignment.Right:
                                margin.Right = parentSize.Width - pRelative.X - size.Width;
                                if (direction == GuidelineDirection.Right)
                                    margin.Left += size.Width;
                                break;

                            case HorizontalAlignment.Center:
                                margin.Left = pRelative.X - parentSize.Width / 2 + size.Width / 2;
                                if (direction == GuidelineDirection.Right)
                                    margin.Left -= size.Width;
                                margin.Right = -margin.Left;
                                break;

                            case HorizontalAlignment.Stretch:
                                if (direction == GuidelineDirection.Left)
                                {
                                    margin.Left = pRelative.X;
                                    margin.Right = parentSize.Width - margin.Left - size.Width;
                                }
                                else
                                {
                                    margin.Right = parentSize.Width - pRelative.X;
                                    margin.Left = parentSize.Width - margin.Right - size.Width;
                                }
                                break;
                        }
                        break;

                    case GuidelineDirection.Top:
                    case GuidelineDirection.Bottom:
                        if (!allowVertical)
                            break;

                        switch (ClipData.VerticalAlignment)
                        {
                            case VerticalAlignment.Top:
                                margin.Top = pRelative.Y;
                                if (direction == GuidelineDirection.Bottom)
                                    margin.Top -= size.Height;
                                break;

                            case VerticalAlignment.Bottom:
                                margin.Bottom = parentSize.Height - pRelative.Y - size.Height;
                                if (direction == GuidelineDirection.Bottom)
                                    margin.Top += size.Height;
                                break;

                            case VerticalAlignment.Center:
                                margin.Top = pRelative.Y - parentSize.Height / 2 + size.Height / 2;
                                if (direction == GuidelineDirection.Bottom)
                                    margin.Top -= size.Height;
                                margin.Bottom = -margin.Top;
                                break;

                            case VerticalAlignment.Stretch:
                                if (direction == GuidelineDirection.Top)
                                {
                                    margin.Top = pRelative.Y;
                                    margin.Bottom = parentSize.Height - margin.Top - size.Height;
                                }
                                else
                                {
                                    margin.Bottom = parentSize.Height - pRelative.Y;
                                    margin.Top = parentSize.Height - margin.Bottom - size.Height;
                                }
                                break;
                        }
                        break;
                }
            }
        }
    }
}
