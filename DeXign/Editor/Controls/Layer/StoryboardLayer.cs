using System.Linq;
using System.Windows;
using System.Windows.Media;
using DeXign.Editor.Controls;
using DeXign.Extension;
using System;

namespace DeXign.Editor.Layer
{
    public class StoryboardLayer : ControlLayer, IDisposable
    {
        bool IsDisposed = false;

        internal Storyboard RootParent;
        internal ScaleTransform RootScale;

        internal double ScaleX => RootScale.ScaleX;
        internal double ScaleY => RootScale.ScaleY;

        public StoryboardLayer(UIElement adornedElement) : base(adornedElement)
        {
            ((FrameworkElement)adornedElement).Loaded += Element_Loaded;
        }

        private void Element_Loaded(object sender, RoutedEventArgs e)
        {
            ((FrameworkElement)sender).Loaded -= Element_Loaded;

            this.RootParent = AdornedElement
                .FindLogicalParents<Storyboard>()
                .FirstOrDefault();

            this.RootScale = RootParent?.RenderTransform as ScaleTransform;

            OnLoaded((FrameworkElement)AdornedElement);
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
    }
}