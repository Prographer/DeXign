using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

using DeXign.Resources;
using DeXign.Controls;
using System.Windows.Input;
using DeXign.UI;
using System.Reflection;
using System.Windows.Controls;
using System.Diagnostics;

namespace DeXign.Editor.Layer
{
    partial class SelectionLayer : StoryboardLayer
    {
        #region [ Render ]
        protected override Size ArrangeOverride(Size finalSize)
        {
            var arrangeSize = base.ArrangeOverride(finalSize);

            // * Vritual Parent Bound Arrange *

            Rect parentRect = GetParentRenderBound();
            double virtualWidth = this.Fit(ClipData.LeftClip.RenderSize.Height);
            double virtualHeight = this.Fit(ClipData.LeftClip.RenderSize.Height);

            if (virtualWidth == 0)
                virtualWidth = this.Fit(ClipData.TopClip.RenderSize.Width);

            if (virtualHeight == 0)
                virtualHeight = this.Fit(ClipData.TopClip.RenderSize.Height);

            ClipData.LeftClip.Arrange(
                new Rect(
                    parentRect.X - virtualWidth / 2, 0,
                    virtualWidth, RenderSize.Height));

            ClipData.RightClip.Arrange(
                new Rect(
                    parentRect.Right - virtualWidth / 2, 0,
                    virtualWidth, RenderSize.Height));

            ClipData.TopClip.Arrange(
                new Rect(
                    0, parentRect.Y - virtualHeight / 2,
                    RenderSize.Width, virtualHeight));

            ClipData.BottomClip.Arrange(
                new Rect(
                    0, parentRect.Bottom - virtualHeight / 2,
                    RenderSize.Width, virtualHeight));

            return arrangeSize;
        }

        protected void BeginGuidelineSet(DrawingContext dc)
        {
            var guidelines = new GuidelineSet();

            guidelines.GuidelinesX.Add(this.Fit(1) / 2);
            guidelines.GuidelinesX.Add(this.Fit(1) / 2);
            guidelines.GuidelinesY.Add(this.Fit(1) / 2);
            guidelines.GuidelinesY.Add(this.Fit(1) / 2);

            dc.PushGuidelineSet(guidelines);
        }

        protected void EndGuidelineSet(DrawingContext dc)
        {
            dc.Pop();

        }

        protected override void OnRender(DrawingContext dc)
        {
            DrawClickArea(dc);
            DrawFrame(dc);

            BeginGuidelineSet(dc);

            OnDispatchRender(dc);

            if (DesignMode != DesignMode.None)
                DrawSelectedFrame(dc);

            if (DesignMode == DesignMode.Size)
            {
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
            }

            EndGuidelineSet(dc);
        }

        private void DrawSelectedFrame(DrawingContext dc)
        {
            var pen = CreatePen(SelectionBrush, 1);
            var rect = new Rect(new Point(), this.RenderSize);

            dc.DrawRectangle(null, pen, rect);
        }

        private void DrawClickArea(DrawingContext dc)
        {
            dc.DrawRectangle(
                Brushes.Transparent,
                null,
                new Rect(new Point(0, 0), this.RenderSize));
        }

        private void DrawFrame(DrawingContext dc)
        {
            dc.PushOpacity(0.75);

            var scaledThickness = new Vector(
                this.Fit(FrameThickness),
                this.Fit(FrameThickness));

            dc.DrawRectangle(
                null, CreatePen(FrameBrush, FrameThickness),
                new Rect(
                    (Point)Vector.Divide(scaledThickness, 2),
                    (Size)Vector.Subtract((Vector)RenderSize, scaledThickness)));
            dc.Pop();
        }

        protected virtual void OnDispatchRender(DrawingContext dc)
        {
            if (IsHighlight)
            {
                int dash = 4;
                var dashedPen = CreatePen(HighlightBrush, 2d);

                dashedPen.DashStyle = new DashStyle(new double[] { dash, dash }, 0);

                var bound = new Rect(0, 0, this.RenderSize.Width, this.RenderSize.Height);

                this.InflateFit(ref bound, 2, 2);

                dc.DrawRectangle(null, dashedPen, bound);
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

            var parentLayout = GetParentLayoutInfo();

            var framePen = CreatePen(ResourceManager.GetBrush("LemonGrass"), 1);
            var solidPen = CreatePen(SelectionBrush, 1d);
            var dashedPen = CreatePen(SelectionBrush, 1d);

            dashedPen.DashStyle = new DashStyle(new double[] { 4, 4 }, 0);

            double hCenter = RenderSize.Height / 2;
            double wCenter = RenderSize.Width / 2;

            // Draw Parent Frame
            dc.DrawRectangle(null, framePen, parentLayout.Bound);

            // Draw Margin Guide Line
            dc.PushOpacity(0.5);
            {
                if (ClipData.LeftVisible)
                    dc.DrawLine(marginLeft ? solidPen : dashedPen,
                        new Point(parentLayout.Bound.X, hCenter),
                        new Point(-1, hCenter));

                if (ClipData.RightVisible)
                    dc.DrawLine(marginRight ? solidPen : dashedPen,
                        new Point(RenderSize.Width, hCenter),
                        new Point(parentLayout.Bound.Right, hCenter));

                if (ClipData.TopVisible)
                    dc.DrawLine(marginTop ? solidPen : dashedPen,
                        new Point(wCenter, parentLayout.Bound.Y),
                        new Point(wCenter, -1));

                if (ClipData.BottomVisible)
                    dc.DrawLine(marginBottom ? solidPen : dashedPen,
                        new Point(wCenter, RenderSize.Height),
                        new Point(wCenter, parentLayout.Bound.Bottom));
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
                parentLayout.Margin.Left / 2 - formattedTextLeft.Width / 2,
                hCenter - formattedTextLeft.Height / 2);

            var textPositionRight = new Point(
                RenderSize.Width + parentLayout.Margin.Right / 2 - formattedTextRight.Width / 2,
                hCenter - formattedTextRight.Height / 2);

            var textPositionTop = new Point(
                wCenter - formattedTextTop.Width / 2,
                parentLayout.Margin.Top / 2 - formattedTextTop.Height / 2);

            var textPositionBottom = new Point(
                wCenter - formattedTextBottom.Width / 2,
                RenderSize.Height + parentLayout.Margin.Bottom / 2 - formattedTextBottom.Height / 2);

            // Value Box Bounds
            var textBoundLeft = new Rect(textPositionLeft, new Size(formattedTextLeft.Width, formattedTextLeft.Height));
            var textBoundRight = new Rect(textPositionRight, new Size(formattedTextRight.Width, formattedTextRight.Height));
            var textBoundTop = new Rect(textPositionTop, new Size(formattedTextTop.Width, formattedTextTop.Height));
            var textBoundBottom = new Rect(textPositionBottom, new Size(formattedTextBottom.Width, formattedTextBottom.Height));

            // Value Box Bounds Inflating
            this.InflateFit(ref textBoundLeft, ValueBoxBlank, ValueBoxBlank);
            this.InflateFit(ref textBoundRight, ValueBoxBlank, ValueBoxBlank);
            this.InflateFit(ref textBoundTop, ValueBoxBlank, ValueBoxBlank);
            this.InflateFit(ref textBoundBottom, ValueBoxBlank, ValueBoxBlank);

            // Value Box Wrapping
            if (textBoundLeft.Width + Blank * 2 >= Math.Abs(parentLayout.Margin.Left))
            {
                if (parentLayout.Bound.X < 0)
                    textBoundLeft.X = parentLayout.Margin.Left - textBoundLeft.Width - Blank * 2;
                else
                    textBoundLeft.X = parentLayout.Margin.Left + Blank * 2;

                textPositionLeft.X = textBoundLeft.X + this.Fit(ValueBoxBlank);
            }

            if (textBoundRight.Width + Blank * 2 >= Math.Abs(parentLayout.Margin.Right))
            {
                if (parentLayout.Bound.Right >= RenderSize.Width)
                    textBoundRight.X = parentLayout.Bound.Right + Blank * 2;
                else
                    textBoundRight.X = parentLayout.Bound.Right - textBoundRight.Width - Blank * 2;

                textPositionRight.X = textBoundRight.X + this.Fit(ValueBoxBlank);
            }

            if (textBoundTop.Width + Blank * 2 >= Math.Abs(parentLayout.Margin.Top))
            {
                if (parentLayout.Bound.Y < 0)
                    textBoundTop.Y = parentLayout.Bound.Top - Blank * 2 - textBoundTop.Width / 2 - textBoundTop.Height / 2;
                else
                    textBoundTop.Y = parentLayout.Bound.Top + Blank * 2 + textBoundTop.Width / 2 - textBoundTop.Height / 2;

                textPositionTop.Y = textBoundTop.Y + this.Fit(ValueBoxBlank);
            }

            if (textBoundBottom.Width + Blank * 2 >= Math.Abs(parentLayout.Margin.Bottom))
            {
                if (parentLayout.Bound.Bottom >= RenderSize.Height)
                    textBoundBottom.Y = parentLayout.Bound.Bottom + Blank * 2 + textBoundBottom.Width / 2 - textBoundBottom.Height / 2;
                else
                    textBoundBottom.Y = parentLayout.Margin.Bottom - Blank * 2 - textBoundBottom.Width / 2 - textBoundBottom.Height / 2;

                textPositionBottom.Y = textBoundBottom.Y + this.Fit(ValueBoxBlank);
            }

            // Value Box Render
            if (marginLeft && valueLeft != "0")
                DrawTextBound(dc, formattedTextLeft, textPositionLeft, textBoundLeft, Brushes.White);

            if (marginRight && valueRight != "0")
                DrawTextBound(dc, formattedTextRight, textPositionRight, textBoundRight, Brushes.White);

            if (marginTop && valueTop != "0")
                DrawTextBound(dc, formattedTextTop, textPositionTop, textBoundTop, Brushes.White, 90);

            if (marginBottom && valueBottom != "0")
                DrawTextBound(dc, formattedTextBottom, textPositionBottom, textBoundBottom, Brushes.White, 90);
        }

        private void DrawGuideLineWidth(DrawingContext dc, bool isBottom)
        {
            var pen = CreatePen(SelectionBrush, 1d);

            double top = this.Fit(-23);
            double hTop = top + this.Fit(5);
            double lineHeight = this.Fit(15d);
            double lineWidth = RenderSize.Width - this.Fit(1d);

            if (isBottom)
            {
                top = RenderSize.Height + this.Fit(7d);
                hTop = top + this.Fit(10d);
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

            this.InflateFit(ref textBound, ValueBoxBlank, ValueBoxBlank);

            // Value Box Wrapping
            if (textBound.Width >= lineWidth)
            {
                textBound.Y += this.Fit(15d) * (isBottom ? 1 : -1);
                textPosition.Y = textBound.Y + ValueBoxBlank;
            }

            DrawTextBound(dc, formattedText, textPosition, textBound, Brushes.White);
        }

        private void DrawGuideLineHeight(DrawingContext dc, bool isRight)
        {
            var pen = CreatePen(SelectionBrush, 1d);

            double left = this.Fit(-23);
            double vLeft = left + this.Fit(5);
            double lineWidth = this.Fit(15d);
            double lineHeight = RenderSize.Height - this.Fit(1d);

            if (isRight)
            {
                left = RenderSize.Width + this.Fit(7d);
                vLeft = left + this.Fit(10d);
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

            this.InflateFit(ref textBound, ValueBoxBlank, ValueBoxBlank);

            // Value Box Wrapping
            if (textBound.Width >= lineHeight)
            {
                textBound.X += this.Fit(isRight ? 15d : -15d);
                textPosition.X = textBound.X + ValueBoxBlank;
            }

            DrawTextBound(dc, formattedText, textPosition, textBound, Brushes.White, 90);
        }

        protected void DrawTextBound(
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

        internal Rect GetParentRenderBound()
        {
            var parentElement = AdornedElement.Parent as FrameworkElement;
            var position = parentElement.TranslatePoint(new Point(), AdornedElement);
            var size = parentElement.RenderSize;

            var padding = GetPadding(parentElement);

            position.X += padding.Left;
            position.Y += padding.Top;

            size.Width -= (padding.Left + padding.Right);
            size.Height -= (padding.Top + padding.Bottom);

            if (Parent is IStackLayout)
            {
                var stack = Parent.Element as SpacingStackPanel;
                var bound = stack.GetArrangedBound(AdornedElement);

                position.X += bound.Left - padding.Left;
                position.Y += bound.Top - padding.Top;

                size.Width = bound.Width;
                size.Height = bound.Height;
            }

            return new Rect(
                position,
                size);
        }

        internal (Rect Bound, Thickness Margin) GetParentLayoutInfo()
        {
            var rect = GetParentRenderBound();

            return (rect, new Thickness(
                rect.X,
                rect.Y,
                rect.Right - RenderSize.Width,
                rect.Bottom - RenderSize.Height));
        }

        private Thickness GetPadding(FrameworkElement element)
        {
            if (element is ContentControl cc)
                return cc.Padding;

            if (element is SpacingStackPanel ssp)
                return ssp.Padding;

            return new Thickness(0);
        }
        #endregion
    }
}