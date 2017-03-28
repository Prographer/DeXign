using System.Windows;
using System.Windows.Media;

namespace DeXign.Extension
{
    public static class DrawingContextEx
    {
        public static StreamGeometry CreateRoundedRectangleGeometry(Rect rect, CornerRadius corner, bool isStroked, bool isFilled, bool isSmoothJoin)
        {
            var geometry = new StreamGeometry();

            using (var context = geometry.Open())
            {
                context.BeginFigure(rect.TopLeft + new Vector(0, corner.TopLeft), isFilled, true);

                context.ArcTo(new Point(rect.TopLeft.X + corner.TopLeft, rect.TopLeft.Y),
                    new Size(corner.TopLeft, corner.TopLeft),
                    90, false, SweepDirection.Clockwise, isStroked, isSmoothJoin);

                context.LineTo(rect.TopRight - new Vector(corner.TopRight, 0), isStroked, isSmoothJoin);

                context.ArcTo(new Point(rect.TopRight.X, rect.TopRight.Y + corner.TopRight),
                    new Size(corner.TopRight, corner.TopRight),
                    90, false, SweepDirection.Clockwise, isStroked, isSmoothJoin);

                context.LineTo(rect.BottomRight - new Vector(0, corner.BottomRight), isStroked, isSmoothJoin);

                context.ArcTo(new Point(rect.BottomRight.X - corner.BottomRight, rect.BottomRight.Y),
                    new Size(corner.BottomRight, corner.BottomRight),
                    90, false, SweepDirection.Clockwise, isStroked, isSmoothJoin);

                context.LineTo(rect.BottomLeft + new Vector(corner.BottomLeft, 0), isStroked, isSmoothJoin);

                context.ArcTo(new Point(rect.BottomLeft.X, rect.BottomLeft.Y - corner.BottomLeft),
                    new Size(corner.BottomLeft, corner.BottomLeft),
                    90, false, SweepDirection.Clockwise, isStroked, isSmoothJoin);

                context.Close();
            }

            return geometry;
        }

        public static void DrawRoundedRectangle(this DrawingContext dc, Brush brush, Pen pen, Rect rect, CornerRadius corner)
        {
            StreamGeometry geometry = CreateRoundedRectangleGeometry(rect, corner, pen != null, brush != null, true);

            dc.DrawGeometry(brush, pen, geometry);
        }
    }
}
