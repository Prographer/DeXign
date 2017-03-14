using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

using DeXign.OS;
using DeXign.Editor.Layer;
using DeXign.Editor.Renderer;
using DeXign.Core.Logic;
using DeXign.Core.Controls;

using WPFExtension;

namespace DeXign.Editor.Logic
{
    public class BindEventArgs : EventArgs
    {
        public BindType BindType { get; set; }

        public BindThumb Target { get; set; }

        public BindEventArgs(BindThumb thumb, BindType type)
        {
            this.Target = thumb;
            this.BindType = type;
        }
    }

    public class BindThumb : Control
    {
        public event EventHandler<BindEventArgs> Binded;

        public static readonly DependencyProperty BindTypeProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty HasBindErrorProperty =
            DependencyHelper.Register(
                new PropertyMetadata(false));

        public BindType BindType
        {
            get { return (BindType)GetValue(BindTypeProperty); }
            set { SetValue(BindTypeProperty, value); }
        }

        public bool HasBindError
        {
            get { return (bool)GetValue(HasBindErrorProperty); }
            set { SetValue(HasBindErrorProperty, value); }
        }
        
        public IRenderer Renderer { get; protected set; }
        
        private LineConnectorBase dragLine;
        private bool dragCanceled = false;

        public BindThumb()
        {
            this.AllowDrop = true;
        }

        #region [ Drag ]
        public BindRequest CreateBindRequest()
        {
            return new BindRequest(this);
        }

        private void PushPendingDragLine()
        {
            if (dragLine != null)
                return;

            var renderer = this.Renderer as StoryboardLayer;
            var storyboard = renderer.RootParent;

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

            var renderer = Renderer as StoryboardLayer;
            var storyboard = renderer.RootParent;

            // Pop Pending Drag Line
            storyboard.PopPendingConnectedLine();
        }
        #endregion

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

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            
            if (this.TemplatedParent is ComponentElement element)
            {
                this.Renderer = element.GetRenderer();
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            var renderer = this.Renderer as StoryboardLayer;
            var storyboard = renderer.RootParent;

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
            var thumb = GetThumbExpression(request);
            var binder = GetBinderExpression(request);

            // Release Pending Drag Line
            dragLine?.Release();

            // Bind To Data Model
            binder.Input.Bind(binder.Output, BinderOptions.Trigger);

            // Notice
            thumb.Output.OnOutputBind(thumb.Input);
            thumb.Input.OnInputBind(thumb.Output);
        }

        protected virtual void OnInputBind(BindThumb outputThumb)
        {
            // Raise Event
            Binded?.Invoke(this, new BindEventArgs(outputThumb, BindType.Input));
        }

        protected virtual void OnOutputBind(BindThumb inputThumb)
        {
            // Raise Event
            Binded?.Invoke(this, new BindEventArgs(inputThumb, BindType.Output));
        }

        private (BaseBinder Output, BaseBinder Input) GetBinderExpression(BindRequest request)
        {
            var thumb = GetThumbExpression(request);

            return (ResolveBinder(thumb.Output.Renderer), 
                    ResolveBinder(thumb.Input.Renderer));
        }

        private (BindThumb Output, BindThumb Input) GetThumbExpression(BindRequest request)
        {
            if (this.BindType == BindType.Input)
            {
                return (request.Source, this);
            }
            else
            {
                return (this, request.Source);
            }
        }

        private BaseBinder ResolveBinder(IRenderer renderer)
        {
            if (renderer.Model is PVisual visual)
            {
                return visual.Binder;
            }

            if (renderer.Model is PComponent model)
            {
                return model;
            }

            return null;
        }

        protected virtual bool CanBind(BindRequest request)
        {
            var thumb = GetThumbExpression(request);
            var binder = GetBinderExpression(request);

            // 같은 렌더러 확인
            if (thumb.Output.Renderer.Equals(thumb.Input.Renderer))
                return false;

            // 바인드 방향 확인
            if (thumb.Output.BindType == thumb.Input.BindType)
                return false;

            // 연결 확인
            return binder.Input.CanBind(binder.Output, BinderOptions.Trigger);
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
            if (BindType == BindType.Input)
                return MousePositionFromConnector(lineConnectorBase);

            return GetEdgePosition(lineConnectorBase);
        }

        protected virtual Point GetDragLineEndPosition(LineConnectorBase lineConnectorBase)
        {
            if (BindType == BindType.Output)
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

            if (BindType == BindType.Input)
                position.X = 0;

            return this.TranslatePoint(
                position,
                lineConnectorBase.Parent);
        }
    }
}
