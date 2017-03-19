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

#if DEBUG
        public bool IsDebug
        {
            get { return (bool)GetValue(IsDebugProperty); }
            set { SetValue(IsDebugProperty, value); }
        }
#endif

        public IRenderer Renderer { get; protected internal set; }

        public PBinder Binder => (PBinder)this.DataContext;

        private BindThumb dragSnapTarget;
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

            this.Binder.Binded += Binder_Binded;
        }
        
        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);

            var componentElement = this.FindVisualParents<ComponentElement>(false).FirstOrDefault();

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

            dragLine.Line.Opacity = 0.5;

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
        protected void SetSnapTarget(BindThumb thumb)
        {
            dragSnapTarget = thumb;
        }

        protected override void OnPreviewDragEnter(DragEventArgs e)
        {
            var request = e.Data.GetData<BindRequest>();

            if (request != null)
            {
                dragCanceled = !CanBind(request);

                if (!dragCanceled)
                    request.Source.SetSnapTarget(this);
            }

            this.HasBindError = dragCanceled;
        }

        protected override void OnPreviewDragLeave(DragEventArgs e)
        {
            base.OnPreviewDragLeave(e);

            var request = e.Data.GetData<BindRequest>();

            if (request != null)
                request.Source.SetSnapTarget(null);

            dragCanceled = false;
            this.HasBindError = false;

            e.Handled = true;
        }

        protected override void OnPreviewDragOver(DragEventArgs e)
        {
            base.OnPreviewDragOver(e);

            e.Effects = DragDropEffects.All;

            e.Handled = true;
        }

        protected override void OnPreviewDrop(DragEventArgs e)
        {
            base.OnPreviewDrop(e);

            var request = e.Data.GetData<BindRequest>();

            if (request != null)
            {
                e.Handled = true;
                request.Handled = true;
                request.Target = this;

                if (dragCanceled)
                {
                    this.HasBindError = false;
                    return;
                }

                OnBind(request);
            }
        }

        protected override void OnPreviewGiveFeedback(GiveFeedbackEventArgs e)
        {
            if (dragLine == null)
                return;

            dragLine.Line.Opacity = (dragSnapTarget != null ? 1 : 0.5);
            dragLine.Update();

            e.Handled = true;
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
            
            if (request.Handled)
            {
                OnDragEnd(request);
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

        private void Binder_Binded(object sender, IBinder e)
        {
            var expression = BinderHelper.GetPairBinder(this.Binder, e);

            var output = (expression.Output as PBinder).GetView<BindThumb>();
            var input = (expression.Input as PBinder).GetView<BindThumb>();

            // Propagate
            output.PropagateBind(output, input);
        }

        private void OnBind(BindRequest request)
        {
            // Release Pending Drag Line
            dragLine?.Release();

            // Bind To Data Model
            this.Binder.Bind(request.Source.Binder);

            // Notice
            this.OnBind(request.Source);
        }

        public void InvalidatePropagate()
        {
            if (this.Binder.Items.Count > 0)
            {
                var inputThumb = (this.Binder.Items[0] as PBinder).GetView<BindThumb>();

                this.PropagateBind(this, inputThumb);
            }
        }

        // TODO: Ref
        // Core 라이브러리와 IDE 프로젝트의 의존성이 너무 강함.
        // 전파 수준을 Core단으로 변경 필요
#if DEBUG
        private async void PropagateBind(BindThumb outputThumb, BindThumb inputThumb)
#else
        private void PropagateBind(BindThumb outputThumb, BindThumb inputThumb)
#endif
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

#if DEBUG
                await t.Task.Delay(500);
#endif
                
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

#if DEBUG
        protected virtual async void OnPropagateBind(BindThumb outputThumb, BindThumb inputThumb)
        {
            this.IsDebug = true;

            await t.Task.Delay(1000);

            this.IsDebug = false;
        }
#else
        protected virtual void OnPropagateBind(BindThumb outputThumb, BindThumb inputThumb)
        {
        }
#endif

        protected virtual void OnBind(BindThumb targetThumb)
        {
            BindThumb outputThumb = null;
            BindThumb inputThumb = null;

            BinderHelper.GetPairObject(
                ref outputThumb, ref inputThumb,
                (this, this.BindOption),
                (targetThumb, targetThumb.BindOption));

            targetThumb.PopPendingDragLine();

            // Raise Event
            Binded?.Invoke(this, new ThumbBindedEventArgs(outputThumb, inputThumb));
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

        protected virtual void OnDragEnd(BindRequest request)
        {
        }

        protected virtual void OnDragLineReleased()
        {
            PopPendingDragLine();
        }

        protected virtual Point GetDragLineStartPosition(LineConnectorBase lineConnectorBase)
        {
            if (this.Binder.GetDirection() == BindDirection.Input)
                return MousePositionFromConnector(lineConnectorBase);

            return GetEdgePosition(lineConnectorBase, this);
        }

        protected virtual Point GetDragLineEndPosition(LineConnectorBase lineConnectorBase)
        {
            if (this.Binder.GetDirection() == BindDirection.Output)
                return MousePositionFromConnector(lineConnectorBase);

            return GetEdgePosition(lineConnectorBase, this);
        }

        private Point MousePositionFromConnector(LineConnectorBase lineConnectorBase)
        {
            if (dragSnapTarget != null)
                return GetEdgePosition(lineConnectorBase, dragSnapTarget);

            return lineConnectorBase.Parent.PointFromScreen(SystemMouse.Position);
        }

        private Point GetEdgePosition(LineConnectorBase lineConnectorBase, BindThumb thumb)
        {
            Point position = new Point(thumb.RenderSize.Width, thumb.RenderSize.Height / 2);

            if (thumb.Binder.GetDirection() == BindDirection.Input)
                position.X = 0;

            return thumb.TranslatePoint(
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
