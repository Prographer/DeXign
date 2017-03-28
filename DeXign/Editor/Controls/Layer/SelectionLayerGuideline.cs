using DeXign.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            Point relative = AdornedElement.TranslatePoint(new Point(), Storyboard);

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

        private bool IsAdjacent(Guideline gl1, Guideline gl2, Size size)
        {
            if (gl1 == null || gl2 == null)
                return false;

            double distnace = Guideline.Distance(gl1, gl2);
            double length = gl1.IsVertical ? size.Width : size.Height;

            return DoubleEx.EpsilonEqauls(distnace, length);
        }

        private void MarginSnap(ref Thickness margin)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                Storyboard.GuideLayer.ClearSnappedGuidelines();
                return;
            }
            
            var bound = GetParentRenderBound();
            var padding = GetPadding(Parent.Element);

            Size parentSize = bound.Size;
            Size size = AdornedElement.RenderSize;

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

            Guideline guidedLeft = null;
            Guideline guidedTop = null;
            Guideline guidedRight = null;
            Guideline guidedBottom = null;
            
            foreach (Guideline item in Storyboard.GuideLayer.InvalidateSnappedGuidelines(this))
            {
                bool awHozitonal = false;
                bool awVertical = false;

                Point pRelative = Storyboard.TranslatePoint(item.Point1, Parent.Element);
                GuidelineDirection direction = item.SnappedGuideline.Direction;

                // 대칭 가이드라인 탐색
                switch (direction)
                {
                    case GuidelineDirection.Left:
                        if (guidedRight == null || IsAdjacent(guidedRight, item, size))
                        {
                            guidedLeft = item;
                            awHozitonal = true;
                        }
                        break;

                    case GuidelineDirection.Top:
                        if (guidedBottom == null || IsAdjacent(guidedBottom, item, size))
                        {
                            guidedTop = item;
                            awVertical = true;
                        }
                        break;

                    case GuidelineDirection.Right:
                        if (guidedLeft == null || IsAdjacent(guidedLeft, item, size))
                        {
                            guidedRight = item;
                            awHozitonal = true;
                        }
                        break;

                    case GuidelineDirection.Bottom:
                        if (guidedTop == null || IsAdjacent(guidedTop, item, size))
                        {
                            guidedBottom = item;
                            awVertical = true;
                        }
                        break;
                }

                // 마진 처리
                switch (direction)
                {
                    case GuidelineDirection.Left:
                    case GuidelineDirection.Right:
                        if (!awHozitonal || !allowHorizontal)
                            continue;
                        
                        switch (ClipData.HorizontalAlignment)
                        {
                            case HorizontalAlignment.Left:
                                margin.Left = pRelative.X - padding.Left;
                                if (direction == GuidelineDirection.Right)
                                    margin.Left -= size.Width;

                                margin.Right = Math.Min(parentSize.Width - RenderSize.Width - margin.Left, 0);
                                break;

                            case HorizontalAlignment.Right:
                                margin.Right = parentSize.Width - pRelative.X - size.Width + padding.Right;
                                if (direction == GuidelineDirection.Right)
                                    margin.Right += size.Width;

                                margin.Left = Math.Min(parentSize.Width - RenderSize.Width - margin.Right, 0);
                                break;

                            case HorizontalAlignment.Center:
                                margin.Left = pRelative.X - parentSize.Width / 2 + size.Width / 2 - padding.Left;
                                if (direction == GuidelineDirection.Right)
                                    margin.Left -= size.Width;
                                margin.Right = -margin.Left;
                                break;

                            case HorizontalAlignment.Stretch:
                                if (direction == GuidelineDirection.Left)
                                {
                                    margin.Left = pRelative.X - padding.Left;
                                    margin.Right = parentSize.Width - margin.Left - size.Width;
                                }
                                else
                                {
                                    margin.Right = parentSize.Width - pRelative.X + padding.Right;
                                    margin.Left = parentSize.Width - margin.Right - size.Width;
                                }
                                break;
                        }
                        break;

                    case GuidelineDirection.Top:
                    case GuidelineDirection.Bottom:
                        if (!awVertical || !allowVertical)
                            continue;
                        
                        switch (ClipData.VerticalAlignment)
                        {
                            case VerticalAlignment.Top:
                                margin.Top = pRelative.Y;
                                if (direction == GuidelineDirection.Bottom)
                                    margin.Top -= size.Height;

                                margin.Bottom = Math.Min(parentSize.Height - RenderSize.Height - margin.Top, 0);
                                break;

                            case VerticalAlignment.Bottom:
                                margin.Bottom = parentSize.Height - pRelative.Y - size.Height;
                                if (direction == GuidelineDirection.Bottom)
                                    margin.Bottom += size.Height;

                                margin.Top = Math.Min(parentSize.Height - RenderSize.Height - margin.Bottom, 0);
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

                item.IsVisible = true;
            }
        }
    }
}