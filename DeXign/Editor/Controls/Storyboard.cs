using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Reflection;
using System.ComponentModel;
using System.Windows.Threading;

using DeXign.Core;
using DeXign.Core.Controls;
using DeXign.Core.Logic;
using DeXign.Core.Logic.Component;
using DeXign.Editor.Layer;
using DeXign.Editor.Renderer;
using DeXign.OS;
using DeXign.Models;
using DeXign.Extension;
using DeXign.Core.Designer;
using DeXign.Controls;
using DeXign.Converter;
using DeXign.Task;
using DeXign.Utilities;

namespace DeXign.Editor.Controls
{
    public partial class Storyboard : Canvas
    {
        #region [ Properties ]
        public StoryboardModel Model { get; private set; }

        public DispatcherTaskManager TaskManager { get; set; }

        public GuideLayer GuideLayer { get; private set; }
        public AbsoluteLayer LineLayer { get; private set; }
        public StoryboardRenderer Renderer { get; private set; }

        public List<PContentPage> Screens => Model?.Project.Screens;

        public bool IsComponentBoxOpen { get { return componentBoxPopup.IsOpen; } }

        internal ZoomPanel ZoomPanel { get; private set; }
        #endregion

        #region [ Local Variable ]
        private ClosableTabItem mangedTabItem;
        private DispatcherTimer updateTimer;

        private Popup componentBoxPopup;
        private Point componentBoxPosition;
        private ComponentBox componentBox;
        
        private Stack<LineConnectorBase> pendingLines;
        private List<LineConnectorBase> managedLines;
        #endregion

        #region [ Constructor ]
        public Storyboard()
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            InitializeLayer();
            InitializeComponents();
            InitializeBindings();

            this.DataContextChanged += Storyboard_DataContextChanged;
            this.Loaded += Storyboard_Loaded;

            Application.Current.MainWindow.Deactivated += Storyboard_Deactivated;
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);

            if (this.Parent is ZoomPanel zoomPanel)
                this.ZoomPanel = zoomPanel;
        }

        private void Storyboard_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is StoryboardModel sbModel)
            {
                Model = sbModel;
            }
        }

        private void Storyboard_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= Storyboard_Loaded;

            var frame = VisualTreeHelperEx.FindVisualParents<Frame>(this).FirstOrDefault();

            if (frame != null)
                mangedTabItem = frame.Parent as ClosableTabItem;
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            foreach (var line in managedLines)
                line.Update();

            LineLayer.InvalidateArrange();
        }

        private void InitializeComponents()
        {
            pendingLines = new Stack<LineConnectorBase>();
            managedLines = new List<LineConnectorBase>();

            componentBox = new ComponentBox();
            componentBoxPopup = new Popup()
            {
                AllowsTransparency = true,
                Child = componentBox,
                PopupAnimation = PopupAnimation.None,
                Placement = PlacementMode.Relative
            };
            
            componentBox.ItemSelected += ComponentBox_ItemSelected;

            // Line Update Timer
            updateTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(10)
            };

            updateTimer.Tick += UpdateTimer_Tick;
            updateTimer.Start();
        }
        
        private void InitializeBindings()
        {
            this.InputBindings.Add(
                new KeyBinding()
                {
                    Key = Key.Escape,
                    Command = DXCommands.ESCCommand
                });

            this.InputBindings.Add(
                new KeyBinding()
                {
                    Key = Key.Delete,
                    Command = DXCommands.DeleteCommand
                });

            this.InputBindings.Add(
                new KeyBinding()
                {
                    Key = Key.Tab,
                    Command = DXCommands.DesignModeCommand
                });

            this.CommandBindings.Add(
                new CommandBinding(DXCommands.ESCCommand, ESC_Execute));

            this.CommandBindings.Add(
                new CommandBinding(DXCommands.DeleteCommand, Delete_Execute));

            this.CommandBindings.Add(
                new CommandBinding(DXCommands.DesignModeCommand, DesignMode_Execute));
        }
        
        private void InitializeLayer()
        {
            GuideLayer = new GuideLayer(this);
            LineLayer = new AbsoluteLayer(this);
            Renderer = new StoryboardRenderer(this);

            this.AddAdorner(GuideLayer);
            this.AddAdorner(LineLayer);
            this.AddAdorner(Renderer);

            GuideLayer.SetAdornerIndex(2);
            LineLayer.SetAdornerIndex(0);
            Renderer.SetAdornerIndex(2);

            this.SetRenderer(Renderer);
        }
        #endregion

        #region [ Input ]
        private void DesignMode_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedLayer = GetSelectedLayer();

            selectedLayer?.Select();
        }

        private void Delete_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var layers = GroupSelector.GetSelectedItems()
                .Cast<SelectionLayer>()
                .ToArray();

            if (layers.Length > 0)
            {
                foreach (var layer in layers)
                {
                    var element = layer.AdornedElement;
                    var parent = (FrameworkElement)element.Parent;

                    IRenderer elementRenderer = element.GetRenderer();

                    // Check Selected Parent
                    if (RendererTreeHelper
                        .FindParents<IRenderer>(elementRenderer)
                        .Count(r => GroupSelector.IsSelected(r)) > 0)
                    {
                        continue;
                    }

                    // * Task *
                    if (elementRenderer is IRendererLayout lRenderer)
                    {
                        TaskManager?.Push(
                            new LayoutTaskData(
                                RendererTaskType.Remove,
                                lRenderer,
                                () => RemoveElement(parent, element, true),
                                () => AddElement(parent, element, true),
                                () => RemoveElement(parent, element)));
                    }
                    else if (elementRenderer is IRendererElement eRenderer)
                    {
                        TaskManager?.Push(
                            new ElementTaskData(
                                RendererTaskType.Remove,
                                eRenderer,
                                () => RemoveElement(parent, element, true),
                                () => AddElement(parent, element, true),
                                () => RemoveElement(parent, element)));
                    }
                }

                GroupSelector.UnselectAll();
            }
        }

        private void ESC_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            // Component Box
            if (IsComponentBoxOpen)
            {
                CloseComponentBox();
                return;
            }

            // Selected Group
            if (GroupSelector.GetSelectedItemCount() > 1)
            {
                GroupSelector.UnselectAll();
                return;
            }

            // Selected Item
            var layer = GetSelectedLayer();

            if (layer == null)
                return;

            var prevLayer = layer.AdornedElement
                .FindVisualParents<FrameworkElement>()
                .Select(element => (StoryboardLayer)element.GetRenderer())
                .Skip(1)
                .FirstOrDefault(adorner => adorner != null && adorner is SelectionLayer);

            if (prevLayer != null)
                GroupSelector.Select(prevLayer, true);
            else
                GroupSelector.UnselectAll();
        }

        /// <summary>
        /// ���õ� ���̾ �����ɴϴ�.
        /// </summary>
        /// <returns></returns>
        protected SelectionLayer GetSelectedLayer()
        {
            var items = GroupSelector.GetSelectedItems();

            if (items?.Count() == 1)
            {
                object item = items.First();

                if (item is SelectionLayer layer)
                {
                    return layer;
                }
            }

            return null;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            GroupSelector.UnselectAll();
        }
        #endregion

        #region [ Render Element ]
        /// <summary>
        /// ȭ���� �����մϴ�.
        /// </summary>
        /// <returns></returns>
        public ContentControl AddNewScreen()
        {
            var metadata = DesignerManager.GetElementType(typeof(PContentPage));
            var control = this.GenerateToElement(this, metadata, pushTask: false) as ContentControl;
            var model = (PContentPage)control.GetRenderer().Model;
            
            // Add Screen To Project
            Screens?.Add(model);

            LayoutExtension.SetPageName(
                model, 
                $"Screen{Screens?.Count}");

            Keyboard.Focus(this);

            return control;
        }
        
        /// <summary>
        /// ������� <see cref="DesignElementAttribute"/> Ư������ ��ϵ� ������ �� <see cref="Type"/>�� <see cref="IRenderer{TModel, TElement}"/>�� ���� �� Parent�� �ڽ����� �����մϴ�.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public FrameworkElement GenerateToElement(
            FrameworkElement parent,
            AttributeTuple<DesignElementAttribute, Type> data, 
            Point position = default(Point),
            bool pushTask = true)
        {
            var rendererAttr = RendererManager.FromModelType(data.Element);
            var visual = RendererManager.CreateVisualRenderer(rendererAttr, position);

            if (visual == null)
                return null;

            // * Task *
            if (pushTask)
            {
                TaskManager?.Push(
                    visual.GetRenderer(),                      // Source
                    () => AddElement(parent, visual, true),    // Do Action
                    () => RemoveElement(parent, visual, true), // Undo Action
                    () => DestroyElement(parent, visual));     // Dispose Action
            }
            else
            {
                AddElement(parent, visual);
            }
            
            return visual;
        }

        public void AddElement(FrameworkElement parent, FrameworkElement element, bool pushTask = false)
        {
            IRenderer parentRenderer = parent.GetRenderer();
            IRenderer childRenderer = element.GetRenderer();

            element.AddAdorner((Adorner)childRenderer);

            if (parent.DataContext != null && parent.DataContext is DependencyObject dataContext)
            {
                // Add On PObject Parent
                ObjectContentHelper.GetContent(
                    dataContext,
                    pi => pi.SetValue(parent.DataContext, element.DataContext), // Single Content
                    list => list.SafeAdd(element.DataContext));                     // List Content
            }

            // Add On WPF Parent
            ObjectContentHelper.GetContent(
                parent,
                pi => pi.SetValue(parent, element),  // Single Content
                list => list.SafeAdd(element));          // List Content
            
            // Notice child added
            parentRenderer?.AddChild(childRenderer, childRenderer.Metadata.CreatedPosition);
        }

        /// <summary>
        /// Parent�� �ڽ����� ��ϵ� Element�� �����ϰ� <see cref="IRenderer"/> �� ���̾ �����մϴ�.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="element"></param>
        public void RemoveElement(FrameworkElement parent, FrameworkElement element, bool pushTask = false)
        {
            IRenderer parentRenderer = parent.GetRenderer();
            IRenderer childRenderer = element.GetRenderer();

            if (parent.DataContext == null ||
                (parent.DataContext != null && !(parent.DataContext is PObject)))
                return;

            // Selection Check
            if (GroupSelector.IsSelected(childRenderer))
            {
                GroupSelector.Select(childRenderer as SelectionLayer, false);
            }

            // Remove On AdornerLayer
            element.RemoveAdorner((Adorner)childRenderer);

            if (!pushTask)
            {
                // Dispose
                DestroyElement(parent, element);
            }

            // Remove On PObject Parent
            ObjectContentHelper.GetContent(
                (DependencyObject)parent.DataContext,
                pi => pi.SetValue(parent.DataContext, null), // Single Content
                list => list.SafeRemove(element.DataContext));   // List Content

            // Remove On WPF Parent
            ObjectContentHelper.GetContent(
                parent,
                pi => pi.SetValue(parent, null), // Single Content
                list => list.SafeRemove(element));   // List Content
            
            // Notice child removed 
            parentRenderer?.RemoveChild(childRenderer);
        }

        private void DestroyElement(FrameworkElement parent, FrameworkElement element)
        {
            IRenderer childRenderer = element.GetRenderer();

            // Unregister On Shared model store
            GlobalModels.UnRegister(childRenderer.Model);

            // Dispose
            if (childRenderer is IDisposable disposable)
                disposable.Dispose();

            element.SetRenderer(null);
            childRenderer.Model.SetRenderer(null);
        }
        #endregion

        #region [ Bezier Line Connector ]
        private void Connector_Updated(object sender, EventArgs e)
        {
            LineLayer.InvalidateArrange();
        }
        
        /// <summary>
        /// �������� �ð������� ����ȭ�����ִ� �ӽ� <see cref="LineConnectorBase"/>�� �����մϴ�.
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="endPosition"></param>
        /// <returns></returns>
        public LineConnectorBase CreatePendingConnectedLine(
            Func<LineConnectorBase, Point> startPosition,
            Func<LineConnectorBase, Point> endPosition)
        {
            var connector = CreateConnectedLine(startPosition, endPosition);

            // add pending line
            pendingLines.Push(connector);
            managedLines.Remove(connector);

            connector.Updated += Connector_Updated;

            return connector;
        }

        /// <summary>
        /// �� <see cref="FrameworkElement"/>�� �ð������� �̾��ִ� �ӽ� <see cref="LineConnector"/>�� �����մϴ�.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public LineConnector CreatePendingConnectedLine(
            FrameworkElement source,
            FrameworkElement target)
        {
            var connector = CreateConnectedLine(source, target);
            
            // add pending line
            pendingLines.Push(connector);
            managedLines.Remove(connector);

            connector.Updated += Connector_Updated;

            return connector;
        }

        /// <summary>
        /// �������� �ð������� ����ȭ�����ִ� �ӽ� <see cref="LineConnectorBase"/>�� �����մϴ�.
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="endPosition"></param>
        /// <returns></returns>
        public LineConnectorBase CreateConnectedLine(
            Func<LineConnectorBase, Point> startPosition,
            Func<LineConnectorBase, Point> endPosition)
        {
            var connector = new LineConnectorBase(this, startPosition, endPosition);

            if (ZoomPanel != null)
            {
                BindingEx.SetBinding(
                    ZoomPanel, ZoomPanel.ScaleProperty,
                    connector.Line, BezierLine.StrokeThicknessProperty,
                    converter: new ReciprocalConverter());
            }

            LineLayer.Add(connector.Line);
            managedLines.Add(connector);

            return connector;
        }

        /// <summary>
        /// �� <see cref="FrameworkElement"/>�� �ð������� �̾��ִ� �ӽ� <see cref="LineConnector"/>�� �����մϴ�.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public LineConnector CreateConnectedLine(
            FrameworkElement source,
            FrameworkElement target)
        {
            var connector = new LineConnector(this, source, target);

            if (ZoomPanel != null)
            {
                BindingEx.SetBinding(
                    ZoomPanel, ZoomPanel.ScaleProperty,
                    connector.Line, BezierLine.StrokeThicknessProperty,
                    converter: new ReciprocalConverter());
            }

            LineLayer.Add(connector.Line);
            managedLines.Add(connector);

            return connector;
        }

        /// <summary>
        /// �ӽ÷� ������ <see cref="LineConnectorBase"/>�� �����մϴ�.
        /// </summary>
        public void PopPendingConnectedLine()
        {
            if (!HasPendingConnectedLine())
                return;

            var connector = pendingLines.Pop();

            connector.Updated -= Connector_Updated;

            LineLayer.Remove(connector.Line);
            managedLines.Remove(connector);

            connector.Release();
        }

        /// <summary>
        /// �ӽ÷� ������ <see cref="LineConnectorBase"/>�� ���� ���θ� Ȯ���մϴ�.
        /// </summary>
        /// <returns></returns>
        public bool HasPendingConnectedLine()
        {
            return pendingLines.Count > 0;
        }
        #endregion

        #region [ Logic ]
        /// <summary>
        /// ���� ������Ʈ�� �����մϴ�.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public FrameworkElement AddNewComponent(ComponentBoxItemModel model, Point position)
        {
            Type pType = null;

            switch (model.ComponentType)
            {
                case ComponentType.Event:
                    pType = typeof(PTrigger);
                    break;

                case ComponentType.Instance:
                    pType = model.Data as Type;
                    break;
            }

            if (pType == null)
            {
                MessageBox.Show("Coming soon!");
                return null;
            }

            var metadata = DesignerManager.GetElementType(pType);
            var control = this.GenerateToElement(this, metadata) as FrameworkElement;

            // Trigger Setting
            if (model.ComponentType == ComponentType.Event)
            {
                var triggerRenderer = control.GetRenderer() as TriggerRenderer;

                triggerRenderer.Model.EventInfo = model.Data as EventInfo;
            }

            // Add Visual
            control.Margin = new Thickness(0);
            control.VerticalAlignment = VerticalAlignment.Top;
            control.HorizontalAlignment = HorizontalAlignment.Left;

            Canvas.SetLeft(control, position.X);
            Canvas.SetTop(control, position.Y);

            return control;
        }

        private void Storyboard_Deactivated(object sender, EventArgs e)
        {
            CloseComponentBox();
        }

        internal void OpenComponentBox(PObject componentTarget)
        {
            // Set Target
            componentBox.TargetObject = componentTarget;

            // Set Last Open Position
            componentBoxPosition = SystemMouse.Position;

            // Popup Child Force Measure
            componentBox.Measure(
                new Size(componentBox.MaxWidth, componentBox.MaxHeight));

            // Popup Place Area
            componentBoxPopup.PlacementRectangle =
                new Rect(
                    new Point(componentBoxPosition.X - 6, componentBoxPosition.Y - componentBox.DesiredSize.Height / 2),
                    componentBox.DesiredSize);

            // Popup Open
            componentBoxPopup.IsOpen = true;
        }

        internal void CloseComponentBox()
        {
            PopPendingConnectedLine();
            componentBoxPopup.IsOpen = false;
        }

        internal void ConnectComponent(IRenderer outputRenderer, IRenderer inputRenderer, BinderOptions option)
        {
            var layer = (inputRenderer as StoryboardLayer);

            BinderOperation.SetBind(outputRenderer, inputRenderer, option);

#if DEBUG
            // output check
            System.Diagnostics.Debug.Assert(
                outputRenderer.ProvideValue()
                    .Outputs.Contains(inputRenderer.ProvideValue()));

            // input check
            System.Diagnostics.Debug.Assert(
                inputRenderer.ProvideValue()
                    .Inputs.Contains(outputRenderer.ProvideValue()));
#endif

            // Connect
            if (layer.IsLoaded)
            {
                ConnectRendererBezierLine(outputRenderer, inputRenderer);
                return;
            }

            layer.RendererLoaded += (s, e) =>
            {
                ConnectRendererBezierLine(outputRenderer, inputRenderer);
            };
        }

        private void ConnectRendererBezierLine(IRenderer outputRenderer, IRenderer inputRenderer)
        {
            var connector = CreateConnectedLine((FrameworkElement)outputRenderer, (FrameworkElement)inputRenderer);
            connector.Update();
        }
        
        private void ComponentBox_ItemSelected(object sender, ComponentBoxItemModel model)
        {
            if (IsComponentBoxOpen)
            {
                IRenderer sourceRenderer = componentBox.TargetObject.GetRenderer();
                
                // Close Opened Box
                CloseComponentBox();

                // Create Component
                FrameworkElement visual = AddNewComponent(model, this.PointFromScreen(componentBoxPosition));
                IRenderer targetRenderer = visual.GetRenderer();

                if (visual == null)
                    return;

                // * Logic Binding *
                ConnectComponent(sourceRenderer, targetRenderer, BinderOptions.Trigger);
            }
        }
        #endregion

        #region [ I/O ]
        public void Save()
        {
        }
        #endregion

        public void Close()
        {
            mangedTabItem?.Close();
        }
    }

    internal class ComponentRequest
    {
        public PObject Target { get; set; }

        public FrameworkElement VisualBinderSource { get; set; }

        public IRenderer BinderSource { get; set; }
        public BinderOptions BinderOption { get; set; }
    }
}