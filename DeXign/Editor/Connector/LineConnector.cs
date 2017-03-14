using System;
using System.Windows;

using DeXign.Editor.Controls;
using DeXign.Extension;
using DeXign.Editor.Logic;
using DeXign.Editor.Renderer;

namespace DeXign.Editor
{
    public class LineConnector : LineConnectorBase
    {
        public new BindThumb Output { get; }
        public new BindThumb Input { get; }
        
        internal LineConnector(
            Storyboard parent,
            Func<LineConnectorBase, Point> output,
            Func<LineConnectorBase, Point> input) : base(parent, output, input)
        {
        }

        internal LineConnector(
            Storyboard parent,
            BindThumb output,
            BindThumb input) : base(parent)
        {
            // Instance
            this.Output = output;
            this.Input = input;

            // Func
            base.Output = GetOutputPosition;
            base.Input = GetInputPosition;
        }

        private Point GetOutputPosition(LineConnectorBase connector)
        {
            if (Output is IUISupport support)
                return support.GetLocation();

            return Output.TranslatePoint(
                new Point(
                    Output.RenderSize.Width,
                    Output.RenderSize.Height / 2), Parent);
        }

        private Point GetInputPosition(LineConnectorBase connector)
        {
            if (Input is IUISupport support)
                return support.GetLocation();

            return Input.TranslatePoint(
                new Point(
                    0,
                    Input.RenderSize.Height / 2), Parent);
        }

        protected override void OnRelease()
        {
        }
    }
}
