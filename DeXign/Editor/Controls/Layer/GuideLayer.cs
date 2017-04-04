using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace DeXign.Editor.Layer
{
    public class GuideLayer : StoryboardLayer, IGuideService
    {
        private List<IGuideProvider> Items { get; }

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

            this.BeginGuidelineSet(dc);

            var pen = this.CreatePen(Brushes.Red, 1);
            
            foreach (var g in SnapItems
                .Where(item => item.IsVertical)
                .GroupBy(item => Math.Round(item.Point1.X, 2)))
            {
                var minY = g.Min(item => Math.Min(item.Point1.Y, item.Point2.Y));
                var maxY = g.Max(item => Math.Max(item.Point1.Y, item.Point2.Y));

                dc.DrawLine(pen, new Point(g.Key, minY), new Point(g.Key, maxY));
            }

            foreach (var g in SnapItems
                .Where(item => !item.IsVertical)
                .GroupBy(item => Math.Round(item.Point1.Y, 2)))
            {
                var minX = g.Min(item => Math.Min(item.Point1.X, item.Point2.X));
                var maxX = g.Max(item => Math.Max(item.Point1.X, item.Point2.X));

                dc.DrawLine(pen, new Point(minX, g.Key), new Point(maxX, g.Key));
            }

            this.EndGuidelineSet(dc);
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

                yield return gl;

                if (!gl.IsVisible)
                    continue;

                if (gl.IsVertical)
                {
                    SnapItems.Add(
                        new Guideline(
                            new Point(
                                gl.Point1.X,
                                Math.Min(gl.Point1.Y, gl.SnappedGuideline.Point1.Y)),
                            new Point(
                                gl.Point1.X,
                                Math.Max(gl.Point2.Y, gl.SnappedGuideline.Point2.Y))));
                }
                else
                {
                    SnapItems.Add(
                        new Guideline(
                            new Point(
                                Math.Min(gl.Point1.X, gl.SnappedGuideline.Point1.X),
                                gl.Point1.Y),
                            new Point(
                                Math.Max(gl.Point2.X, gl.SnappedGuideline.Point2.X),
                                gl.Point2.Y)));
                }
            }

            this.InvalidateVisual();
        }

        public IEnumerable<Guideline> GetSnappedGuidelines(IGuideProvider target)
        {
            foreach (IGuideProvider provider in Items.Except(new[] { target }))
            {
                if (provider is FrameworkElement frameworkElement &&
                    !frameworkElement.IsVisible)
                {
                    continue;
                }

                foreach (Guideline gl in provider.GetGuidableLines()
                    .Where(gl =>
                    {
                        return target.GetGuidableLines()
                            .Count(tGl =>
                            {
                                double length = Guideline.Distance(gl, tGl);

                                if (length == -1)
                                    return false;

                                if (length <= this.Fit(SnapThreshold))
                                {
                                    gl.SnappedGuideline = tGl;
                                    return true;
                                }

                                return false;
                            }) > 0;
                    }).
                    OrderBy(gl =>
                    {
                        return Guideline.Distance(gl, gl.SnappedGuideline);
                    }))
                {
                    yield return gl;
                }
            }
        }
    }
}
