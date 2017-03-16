using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

using DeXign.OS;
using DeXign.Editor.Layer;
using DeXign.Editor.Renderer;
using DeXign.Core.Logic;

using WPFExtension;
using DeXign.Extension;
using System.Linq;
using System.Collections.Generic;

using t = System.Threading.Tasks;

namespace DeXign.Editor.Logic
{
    public class ThumbBindedEventArgs : EventArgs
    {
        public BindThumb OutputThumb { get; }
        public BindThumb InputThumb { get; }

        public ThumbBindedEventArgs(BindThumb output, BindThumb input)
        {
            this.OutputThumb = output;
            this.InputThumb = input;
        }
    }

    public class BindThumb : Control
    {
        public event EventHandler<ThumbBindedEventArgs> Binded;

        public static readonly DependencyProperty HasBindErrorProperty =
            DependencyHelper.Register(
                new PropertyMetadata(false));

        public static readonly DependencyProperty BindOptionProperty =
            DependencyHelper.Register();

#if DEBUG
        public static readonly DependencyProperty IsDebugProperty =
            DependencyHelper.Register();
#endif

        public bool HasBindError
        {
            get { return (bool)GetValue(HasBindErrorProperty); }
            set { SetValue(HasBindErrorProperty, value); }
        }

        public BindOptions BindOption
        {
            get { return (BindOptions)GetValue(BindOptionProperty); }
            set { SetValue(BindOptionProperty, value); }
        }
        
        public bool IsDebug
        {
            get { return (bool)GetValue(IsDebugProperty); }
            set { SetValue(IsDebugProperty, value); }
        }

        public IRenderer Renderer { get; protected internal set; }

        public PBinder Binder => (PBinder)this.DataContext;
        
        private LineConnectorBase dragLine;
        private bool dragCanceled = false;

        public BindThumb()
        {
            this.AllowDrop = true;
        }

        public BindThumb(PBinder binder) : this()
        {
            this.DataContext = binder;

            this.BindOption = binder.BindOption;
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);

            var componentElement = this.FindVisualParents<ComponentElement>(true).FirstOrDefault();

            if (componentElement != null)
                this.Renderer = componentElement.GetRenderer();
        }
        
        #region [ Drag Line ]
        public BindRequest CreateBindRequest()
        {
            return new BindRequest(this);
        }

        private void PushPendingDragLine()
        {
            if (dragLine != null)
                return;

            var layer = this.Renderer as StoryboardLayer;
            var storyboard = layer.Storyboard;

            dragLine = storyboard
                .CreatePendingConnectedLine(
                    GetDragLineStartPosition,
                    GetDragLineEndPosition);

            dragLine.Released += DragLine_Released;
        }

        private void PopPendingDragLine()
        {
            if (dragLine == null)
                return;

            // Release -> Released -> OnDragLineReleased -> PopPendingDragLine
            if (dragLine.IsConnected)
            {
                dragLine.Release();
                return;
            }
            
            dragLine.Released -= DragLine_Released;
            dragLine = null;

            var layer = this.Renderer as StoryboardLayer;
            var storyboard = layer.Storyboard;

            // Pop Pending Drag Line
            storyboard.PopPendingConnectedLine();
        }
        #endregion

        #region [ Drag ]
        protected override void OnPreviewDragEnter(DragEventArgs e)
        {
            object data = e.Data.GetData(typeof(BindRequest));

            if (data is BindRequest request)
                dragCanceled = !CanBind(request);

            this.HasBindError = dragCanceled;
        }

        protected override void OnDragLeave(DragEventArgs e)
        {
            dragCanceled = false;
            this.HasBindError = false;
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);
            e.Effects = DragDropEffects.All;
        }

        protected override void OnDrop(DragEventArgs e)
        {
            object data = e.Data.GetData(typeof(BindRequest));

            if (data is BindRequest request)
            {
                request.Handled = true;

                if (dragCanceled)
                {
                    this.HasBindError = false;
                    return;
                }

                OnBind(request);
            }
        }
        #endregion

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            var renderer = this.Renderer as StoryboardLayer;
            var storyboard = renderer.Storyboard;

            OnDragStarting();

            // Push Pending Drag Line
            PushPendingDragLine();
            
            BindRequest request = CreateBindRequest();

            DragDrop.DoDragDrop(this, request, DragDropEffects.None);
            
            if (!request.Handled)
            {
                OnDragEnd();
            }
            else
            {
                // Release Pending Drag Line
                PopPendingDragLine();
            }
        }

        private void DragLine_Released(object sender, EventArgs e)
        {
            OnDragLineReleased();
        }

        private void OnBind(BindRequest request)
        {
            // Release Pending Drag Line
            dragLine?.Release();

            // Bind To Data Model
            this.Binder.Bind(request.Source.Binder);

            // Notice
            this.OnBind(request.Source);

            // Propagate
            var expression = ResolveThumbExpression(request);

            expression.Output.PropagateBind(expression.Output, expression.Input);
        }

        // TODO: Ref
        // Core 라이브러리와 IDE 프로젝트의 의존성이 너무 강함.
        // 전파 수준을 Core단으로 변경 필요
        private void PropagateBind(BindThumb outputThumb, BindThumb inputThumb)
        {
            var nextHosts = new List<IBinderHost>();
            var hostQueue = new Queue<(IBinderHost Host, int Level)>(
                new[]
                {
                    (outputThumb.Binder.Host, 0)
                });
            
            while (hostQueue.Count > 0)
            {
                var data = hostQueue.Dequeue();

                nextHosts.Clear();

                // Output Binders
                foreach (IBinder binder in data.Host.Items.Find(BindOptions.Output | BindOptions.Return))
                {
                    if (data.Level == 0 && !binder.Equals(outputThumb.Binder as IBinder))
                        continue;

                    EnsurePropagateBindCore(binder);
                }

                // Output - Input Binders
                foreach (IBinder binder in data.Host.Items.Find(BindOptions.Output | BindOptions.Return))
                {
                    foreach (IBinder inputBinder in binder.Items)
                    {
                        if (data.Level == 0 && !inputBinder.Equals(inputThumb.Binder))
                            continue;

                        EnsurePropagateBindCore(inputBinder);

                        nextHosts.Add(inputBinder.Host);
                    }
                }
                
                foreach (IBinderHost inputHost in nextHosts.Distinct())
                {
                    hostQueue.Enqueue((inputHost, data.Level + 1));
                }
            }

            // Ensure Propagate
            void EnsurePropagateBindCore(IBinder binder)
            {
                if (binder is PBinder binderModel)
                {
                    var thumb = binderModel.GetView<BindThumb>();

                    // Model
                    thumb.Binder.PropagateBind(outputThumb.Binder, inputThumb.Binder);
                    
                    // Visual
                    thumb.OnPropagateBind(outputThumb, inputThumb);
                }
            }
        }

        protected virtual void OnPropagateBind(BindThumb outputThumb, BindThumb inputThumb)
        {
        }

        protected virtual void OnBind(BindThumb outputThumb)
        {
            // Raise Event
            Binded?.Invoke(this, new ThumbBindedEventArgs(this, outputThumb));
        }
        
        protected virtual bool CanBind(BindRequest request)
        {
            // 같은 렌더러 확인
            if (this.Renderer.Equals(request.Source.Renderer))
                return false;

            return this.Binder.CanBind(request.Source.Binder);
        }

        protected virtual void OnDragStarting()
        {
        }

        protected virtual void OnDragEnd()
        {
            // Release Pending Drag Line
            PopPendingDragLine();
        }

        protected virtual void OnDragLineReleased()
        {
            PopPendingDragLine();
        }

        protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            dragLine.Update();
            e.Handled = true;
        }

        protected virtual Point GetDragLineStartPosition(LineConnectorBase lineConnectorBase)
        {
            if (this.Binder.GetDirection() == BindDirection.Input)
                return MousePositionFromConnector(lineConnectorBase);

            return GetEdgePosition(lineConnectorBase);
        }

        protected virtual Point GetDragLineEndPosition(LineConnectorBase lineConnectorBase)
        {
            if (this.Binder.GetDirection() == BindDirection.Output)
                return MousePositionFromConnector(lineConnectorBase);

            return GetEdgePosition(lineConnectorBase);
        }

        private Point MousePositionFromConnector(LineConnectorBase lineConnectorBase)
        {
            return lineConnectorBase.Parent.PointFromScreen(SystemMouse.Position);
        }

        private Point GetEdgePosition(LineConnectorBase lineConnectorBase)
        {
            Point position = new Point(this.RenderSize.Width, this.RenderSize.Height / 2);

            if (this.Binder.GetDirection() == BindDirection.Input)
                position.X = 0;

            return this.TranslatePoint(
                position,
                lineConnectorBase.Parent);
        }

        private (BindThumb Output, BindThumb Input) ResolveThumbExpression(BindRequest request)
        {
            if (BindDirection.Output.HasFlag((BindDirection)request.Source.BindOption))
            {
                return (request.Source, this);
            }

            return (this, request.Source);
        }
    }
}
