using System;
using System.Windows;
using System.Windows.Media;
using System.Globalization;

using DeXign.Resources;

namespace DeXign.Editor.Layer
{
    partial class SelectionLayer : StoryboardLayer
    {
        #region [ Render ]
        protected override Size ArrangeOverride(Size finalSize)
        {
            var arrangeSize = base.ArrangeOverride(finalSize);

            // * Virtual Bound Arrange *

            var rect = new Rect(new Point(0, 0), RenderSize);
            Inflate(ref rect, 1, 1);
             
            frame.Arrange(rect);

            // * Vritual Parent Bound Arrange *

            Rect parentRect = GetParentRenderBound();
            double virtualWidth = clipData.LeftClip.RenderSize.Height / ScaleX;
            double virtualHeight = clipData.LeftClip.RenderSize.Height / ScaleX;

            clipData.LeftClip.Arrange(
                new Rect(
                    parentRect.X - virtualWidth / 2, 0,
                    virtualWidth, RenderSize.Height));

            clipData.RightClip.Arrange(
                new Rect(
                    parentRect.Right - virtualWidth / 2, 0,
                    virtualWidth, RenderSize.Height));

            clipData.TopClip.Arrange(
                new Rect(
                    0, parentRect.Y - virtualHeight / 2,
                    RenderSize.Width, virtualHeight));

            clipData.BottomClip.Arrange(
                new Rect(
                    0, parentRect.Bottom - virtualHeight / 2,
                    RenderSize.Width, virtualHeight));

            return arrangeSize;
        }

        protected void BeginGuidelineSet(DrawingContext dc)
        {
            var guidelines = new GuidelineSet();
            
            guidelines.GuidelinesX.Add(1 / ScaleX / 2);
            guidelines.GuidelinesX.Add(1 / ScaleX / 2);
            guidelines.GuidelinesY.Add(1 / ScaleY / 2);
            guidelines.GuidelinesY.Add(1 / ScaleY / 2);

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

        private void DrawClickArea(DrawingContext dc)
        {
            dc.DrawRectangle(
                Brushes.Transparent,
                null,
                new Rect(new Point(0, 0), RenderSize));
        }

        private void DrawFrame(DrawingContext dc)
        {
            dc.PushOpacity(0.75);

            var scaledThickness = new Vector(
                FrameThickness / ScaleX,
                FrameThickness / ScaleX);

            dc.DrawRectangle(
                null, CreatePen(FrameBrush, FrameThickness),
                new Rect(
                    (Point)Vector.Divide(scaledThickness, 2),
                    (Size)Vector.Subtract((Vector)RenderSize, scaledThickness)));
            dc.Pop();
        }

        protected virtual void OnDispatchRender(DrawingContext dc)
        {
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

            var framePen = CreatePen(ResourceManager.GetBrush("LemonGrass"), 1);
            var solidPen = CreatePen(SelectionBrush, 1d);
            var dashedPen = CreatePen(SelectionBrush, 1d);

            dashedPen.DashStyle = new DashStyle(new double[] { 4, 4 }, 0);

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
                    textBoundTop.Y = parentRect.Top - Blank * 2 - textBoundTop.Width / 2 - textBoundTop.Height / 2;
                else
                    textBoundTop.Y = parentRect.Top + Blank * 2 + textBoundTop.Width / 2 - textBoundTop.Height / 2; 

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
            var pen = CreatePen(SelectionBrush, 1d);

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
            var pen = CreatePen(SelectionBrush, 1d);

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

        protected Pen CreatePen(Brush brush, double width)
        {
            return new Pen(brush, width / ScaleX);
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
}