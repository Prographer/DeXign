using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace DeXign.Render
{
    public class GlyphRunFactory
    {
        private static Dictionary<(double, ushort), double> glyphWidthsCache;
        private static Dictionary<Typeface, GlyphRunFactory> glyphFactoryCache;

        public Typeface Typeface { get; }
        public GlyphTypeface GlyphTypeface { get; }

        static GlyphRunFactory()
        {
            glyphWidthsCache = new Dictionary<(double, ushort), double>();
            glyphFactoryCache = new Dictionary<Typeface, GlyphRunFactory>();
        }

        public static GlyphRunFactory Create(Typeface typeface)
        {
            GlyphRunFactory gFactory;

            if (!glyphFactoryCache.TryGetValue(typeface, out gFactory))
            {
                if (typeface.TryGetGlyphTypeface(out GlyphTypeface gTypeface))
                {
                    gFactory = new GlyphRunFactory(typeface, gTypeface);
                    glyphFactoryCache[typeface] = gFactory;
                }
            }

            return gFactory;
        }

        private GlyphRunFactory(Typeface typeface, GlyphTypeface gTypeface)
        {
            this.Typeface = typeface;
            this.GlyphTypeface = gTypeface;
        }

        public GlyphRun CreateGlyphRun(string text, double emSize)
        {
            return CreateGlyphRun(text, emSize, new Point());
        }

        public GlyphRun CreateGlyphRun(string text, double emSize, Point position)
        {
            return CreateGlyphRun(text, emSize, _ => position);
        }

        public GlyphRun CreateGlyphRun(string text, double emSize, 
            Rect bound,
            AlignmentX textAlignmentX,
            AlignmentY textAlignmentY)
        {
            return CreateGlyphRun(text, emSize,
                width =>
                {
                    var position = new Point(0, 0);

                    if (textAlignmentX == AlignmentX.Center)
                        position.X = (bound.Width - width) / 2;
                    else if (textAlignmentX == AlignmentX.Right)
                        position.X = bound.Width - width;

                    if (textAlignmentY == AlignmentY.Center)
                        position.Y = (bound.Height - emSize) / 2;
                    else if (textAlignmentY == AlignmentY.Bottom)
                        position.Y = bound.Height - emSize;
                    
                    return position;
                });
        }
        
        public GlyphRun CreateGlyphRun(string text, double emSize, Func<double, Point> positionCallback)
        {
            ushort[] glyphIndexes = new ushort[text.Length];
            double[] advanceWidths = new double[text.Length];

            var totalWidth = 0d;
            double glyphWidth;

            for (int n = 0; n < text.Length; n++)
            {
                glyphIndexes[n] = this.GlyphTypeface.CharacterToGlyphMap[text[n]];

                if (!glyphWidthsCache.TryGetValue((emSize, glyphIndexes[n]), out glyphWidth))
                {
                    glyphWidth = this.GlyphTypeface.AdvanceWidths[glyphIndexes[n]] * emSize;
                    glyphWidthsCache.Add((emSize, glyphIndexes[n]), glyphWidth);
                }

                advanceWidths[n] = glyphWidth;
                totalWidth += glyphWidth;
            }

            Point position = positionCallback(totalWidth);

            position.Y += emSize;

            return new GlyphRun(this.GlyphTypeface, 0, false, emSize, glyphIndexes, position, advanceWidths, null, null, null, null, null, null);
        }
    }
}
