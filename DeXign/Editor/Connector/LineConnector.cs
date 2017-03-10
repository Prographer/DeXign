using System;
using System.Windows;

using DeXign.Editor.Controls;
using DeXign.Extension;

namespace DeXign.Editor
{
    public class LineConnector : LineConnectorBase
    {
        public new FrameworkElement Source { get; }
        public new FrameworkElement Target { get; }

        internal LineConnector(
            Storyboard parent,
            Func<LineConnectorBase, Point> source,
            Func<LineConnectorBase, Point> target) : base(parent, source, target)
        {
        }

        internal LineConnector(
            Storyboard parent,
            FrameworkElement source,
            FrameworkElement target) : base(parent)
        {
            this.Source = source;
            this.Target = target;

            base.Source = GetSourcePosition;
            base.Target = GetTargetPosition;
        }

        private Point GetSourcePosition(LineConnectorBase connector)
        {
            if (Source is IUISupport support)
                return support.GetLocation();

            return Source.TranslatePoint(
                new Point(
                    Source.RenderSize.Width,
                    Source.RenderSize.Height / 2), Parent);
        }

        private Point GetTargetPosition(LineConnectorBase connector)
        {
            if (Target is IUISupport support)
                return support.GetLocation();

            return Target.TranslatePoint(
                new Point(
                    0,
                    Target.RenderSize.Height / 2), Parent);
        }

        protected override void OnRelease()
        {
        }
    }
}
