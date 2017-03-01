using System;
using System.Windows;

using DeXign.Controls;
using DeXign.Editor.Controls;

namespace DeXign.Editor
{
    public class LineConnectorBase
    {
        const double Offset = 30;

        public event EventHandler Released;
        public event EventHandler Updated;
        
        public Storyboard Parent { get; }

        public Func<LineConnectorBase, Point> Source { get; protected set; }
        public Func<LineConnectorBase, Point> Target { get; protected set; }

        public BezierLine Line { get; }

        public bool IsConnected { get; private set; }

        protected LineConnectorBase(Storyboard parent)
        {
            this.Parent = parent;
            this.Line = new BezierLine()
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left
            };

            IsConnected = true;
        }

        internal LineConnectorBase(
            Storyboard parent,
            Func<LineConnectorBase, Point> source,
            Func<LineConnectorBase, Point> target) : this(parent)
        {
            this.Source = source;
            this.Target = target;

            Update();
        }

        public void Release()
        {
            if (IsConnected)
            {
                IsConnected = false;
                
                Source = null;
                Target = null;

                OnRelease();
                Released?.Invoke(this, null);
            }
        }

        protected virtual void OnRelease()
        {
        }

        public void Update()
        {
            if (!IsConnected)
                return;
            
            Point sourcePosition = Source(this);
            Point targetPosition = Target(this);

            // Line Size
            Vector size = targetPosition - sourcePosition;

            Line.Width = Math.Abs(size.X);
            Line.Height = Math.Abs(size.Y);

            Line.X1 = 0;
            Line.Y1 = 0;
            Line.X2 = 1;
            Line.Y2 = 1;

            var bzPt1 = new Point(Math.Abs(size.X) * 0.5, Math.Abs(size.Y) * 0);
            var bzPt2 = new Point(Math.Abs(size.X) * 0.5, Math.Abs(size.Y) * 1);
            
            if (size.X < 0)
            {
                sourcePosition.X -= Line.Width;

                Line.X1 = 1;
                Line.X2 = 0;

                bzPt1.X = 1.2 * Math.Abs(size.X);
                bzPt2.X = -0.2 * Math.Abs(size.X);

                bzPt1.Y = 0.25 * Math.Abs(size.Y);
                bzPt2.Y = 0.75 * Math.Abs(size.Y);
            }

            if (size.Y < 0)
            {
                sourcePosition.Y -= Line.Height;

                Line.Y1 = 1;
                Line.Y2 = 0;

                // Swap Point Y
                double y = bzPt1.Y;
                bzPt1.Y = bzPt2.Y;
                bzPt2.Y = y;
            }

            Line.BezierPoint1 = bzPt1;
            Line.BezierPoint2 = bzPt2;
            
            Storyboard.SetLeft(Line, sourcePosition.X);
            Storyboard.SetTop(Line, sourcePosition.Y);
            
            Updated?.Invoke(this, null);
        }
    }
}
