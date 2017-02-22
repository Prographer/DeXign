using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace DeXign.Editor.Layer
{
    class GuideLayer : StoryboardLayer, IGuideService
    {
        public List<IGuideProvider> Items { get; }

        public double SnapThreshold { get; set; } = 10;

        List<Guideline> SnapItems;

        public GuideLayer(UIElement element) : base(element)
        {
            Items = new List<IGuideProvider>();
            SnapItems = new List<Guideline>();
        }

        public void Add(IGuideProvider layer)
        {
            Items.Add(layer);
        }

        public void Remove(IGuideProvider layer)
        {
            Items.Remove(layer);
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            var guidelines = new GuidelineSet();

            guidelines.GuidelinesX.Add(1 / ScaleX / 2);
            guidelines.GuidelinesX.Add(1 / ScaleX / 2);
            guidelines.GuidelinesY.Add(1 / ScaleY / 2);
            guidelines.GuidelinesY.Add(1 / ScaleY / 2);

            dc.PushGuidelineSet(guidelines);

            // TODO
            var pen = new Pen(Brushes.Red, 1 / ScaleX);

            foreach (Guideline item in SnapItems)
                dc.DrawLine(pen, item.Point1, item.Point2);

            dc.Pop();
        }

        public void ClearSnappedGuidelines()
        {
            SnapItems.Clear();
            this.InvalidateVisual();
        }

        public IEnumerable<Guideline> InvalidateSnappedGuidelines(IGuideProvider target)
        {
            SnapItems.Clear();

            foreach (Guideline gl in GetSnappedGuidelines(target))
            {
                if (gl.SnappedGuideline == null)
                    continue;

                if (gl.IsVertical)
                {
                    SnapItems.Add(
                        new Guideline(
                            new Point(gl.Point1.X, Math.Min(gl.Point1.Y, gl.SnappedGuideline.Point1.Y)),
                            new Point(gl.Point1.X, Math.Max(gl.Point2.Y, gl.SnappedGuideline.Point2.Y))));
                }
                else
                {
                    SnapItems.Add(
                        new Guideline(
                            new Point(Math.Min(gl.Point1.X, gl.SnappedGuideline.Point1.X), gl.Point1.Y),
                            new Point(Math.Max(gl.Point2.X, gl.SnappedGuideline.Point2.X), gl.Point2.Y)));
                }

                yield return gl;
            }

            this.InvalidateVisual();
        }

        public IEnumerable<Guideline> GetSnappedGuidelines(IGuideProvider target)
        {
            foreach (IGuideProvider provider in Items.Except(new[] { target }))
            {
                foreach (Guideline gl in provider.GetGuidableLines()
                    .Where(gl =>
                    {
                        return target.GetGuidableLines()
                            .Count(tGl =>
                            {
                                if (tGl.Slope != gl.Slope)
                                    return false;
                                
                                double length;

                                if (tGl.IsVertical)
                                    length = Math.Abs(tGl.Point1.X - gl.Point1.X);
                                else
                                    length = Math.Abs(tGl.Point1.Y - gl.Point1.Y);

                                if (length <= SnapThreshold / ScaleX)
                                {
                                    gl.SnappedGuideline = tGl;
                                    return true;
                                }

                                return false;
                            }) > 0;
                    }))
                {
                    yield return gl;
                }
            }
        }
    }
}
