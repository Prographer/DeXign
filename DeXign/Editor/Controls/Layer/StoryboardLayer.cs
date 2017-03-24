using System;
using System.Linq;
using System.Windows;

using DeXign.Editor.Controls;
using DeXign.Extension;
using DeXign.Controls;
using DeXign.Editor.Renderer;
using System.Windows.Media;
using System.Globalization;

namespace DeXign.Editor.Layer
{
    public class StoryboardLayer : ControlLayer, IDisposable
    {
        public event EventHandler RendererLoaded;
        public event EventHandler InvalidatedLayout;

        bool IsDisposed = false;

        internal Storyboard Storyboard;
        internal ZoomPanel Zoom => Storyboard.ZoomPanel;

        internal double Scale => Zoom.Scale;

        public StoryboardLayer(UIElement adornedElement) : base(adornedElement)
        {
            ((FrameworkElement)adornedElement).Loaded += Element_Loaded;
        }

        private void Element_Loaded(object sender, RoutedEventArgs e)
        {
            ((FrameworkElement)sender).Loaded -= Element_Loaded;
            
            this.Storyboard = AdornedElement
                .FindLogicalParents<Storyboard>()
                .FirstOrDefault();

            OnLoaded((FrameworkElement)AdornedElement);

            RendererLoaded?.Invoke(this, e);
        }

        protected virtual void OnLoaded(FrameworkElement adornedElement)
        {

        }

        public void Dispose()
        {
            if (!IsDisposed)
                OnDisposed();

            IsDisposed = true;
        }

        protected virtual void OnDisposed()
        {
        }

        protected void RaiseInvalidatedLayout()
        {
            InvalidatedLayout?.Invoke(this, null);
        }

        #region [ Scale Fit Methods ]
        protected void InflateFit(ref Rect rect, double x, double y)
        {
            rect.Inflate(x / Scale, y / Scale);
        }

        protected void Fit(ref double value)
        {
            value = Fit(value);
        }

        protected double Fit(double value)
        {
            return value / Scale;
        }
        #endregion

        #region [ Resource Methods ]
        protected FormattedText CreateFormattedText(string text, double size, string fontName, Brush brush)
        {
            return CreateFormattedText(text, size, new Typeface(fontName), brush);
        }

        protected FormattedText CreateFormattedText(string text, double size, Typeface typeface, Brush brush)
        {
            return new FormattedText(
                text, CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                typeface,
                this.Fit(size),
                brush);
        }

        protected Pen CreatePen(Brush brush, double width)
        {
            return new Pen(brush, this.Fit(width));
        }
        #endregion
    }
}