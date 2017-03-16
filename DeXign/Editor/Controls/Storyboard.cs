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
using DeXign.Editor.Logic;

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
        private LineConnectorCollection lineCollection;
        #endregion

        #region [ Constructor ]
        public Storyboard()
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;
            
            InitializeLayer();
            InitializeComponents();
            InitializeBindings();

            this.AllowDrop = true;

            this.DataContextChanged += Storyboard_DataContextChanged;
            this.Loaded += Storyboard_Loaded;

            Application.Current.MainWindow.Deactivated += Storyboard_Deactivated;

            var z = ZoomPanel;
        }

        private void InitializeComponents()
        {
            pendingLines = new Stack<LineConnectorBase>();
            lineCollection = new LineConnectorCollection();

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

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);

            if (this.Parent is ZoomPanel zoomPanel)
            {
                this.ZoomPanel = zoomPanel;
            }
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
            foreach (var line in lineCollection)
                line.Update();

            LineLayer.InvalidateArrange();
        }
        #endregion

        #region [ Input ]
        private void DesignMode_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedLayer = GetSelectedLayer();

            if (selectedLayer is SelectionLayer selection)
            {
                selection.Select();
            }
        }

        private void Delete_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var items = GroupSelector.GetSelectedItems()
                .ToArray();

            if (items.Length > 0)
            {
                foreach (var item in items)
                {
                    IRenderer targetRenderer = null;

                    if (item is IRenderer renderer)
                        targetRenderer = renderer;

                    if (item is ComponentElement element)
                        targetRenderer = item.GetRenderer();

                    if (targetRenderer != null)
                        DeleteLayer(targetRenderer);
                }

                GroupSelector.UnselectAll();
            }
        }
        
        private void DeleteLayer(IRenderer renderer)
        {
            var element = renderer.Element;
            var parent = renderer.RendererParent.Element;

            // Check Selected Parent
            if (RendererTreeHelper
                .FindParents<IRenderer>(renderer)
                .Count(r => GroupSelector.IsSelected(r as FrameworkElement)) > 0)
            {
                return;
            }

            // * Task *
            if (renderer is IRendererLayout lRenderer)
            {
                TaskManager?.Push(
                    new LayoutTaskData(
                        RendererTaskType.Remove,
                        lRenderer,
                        () => RemoveElement(parent, element, true),
                        () => AddElement(parent, element, true),
                        () => RemoveElement(parent, element)));
            }
            else if (renderer is IRendererElement eRenderer)
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
            {
                GroupSelector.UnselectAll();
                return;
            }

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
        /// 선택된 레이어를 가져옵니다.
        /// </summary>
        /// <returns></returns>
        protected StoryboardLayer GetSelectedLayer()
        {
            var items = GroupSelector.GetSelectedItems();

            if (items?.Count() == 1)
            {
                object item = items.First();

                if (item is StoryboardLayer layer)
                {
                    return layer;
                }
            }

            return null;
        }
        #endregion

        #region [ Render Element ]
        /// <summary>
        /// 화면을 생성합니다.
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
        /// 어셈블리의 <see cref="DesignElementAttribute"/> 특성으로 등록된 데이터 모델 <see cref="Type"/>의 <see cref="IRenderer{TModel, TElement}"/>를 생성 후 Parent의 자식으로 설정합니다.
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
                IRenderer renderer = visual.GetRenderer();

                // * Task *
                if (renderer is IRendererLayout lRenderer)
                {
                    TaskManager?.Push(
                        new LayoutTaskData(
                            RendererTaskType.Add,
                            lRenderer,
                            () => AddElement(parent, visual, true),
                            () => RemoveElement(parent, visual, true),
                            () => DestroyElement(parent, visual)));
                }
                else if (renderer is IRendererElement eRenderer)
                {
                    TaskManager?.Push(
                        new ElementTaskData(
                            RendererTaskType.Add,
                            eRenderer,
                            () => AddElement(parent, visual, true),
                            () => RemoveElement(parent, visual, true),
                            () => DestroyElement(parent, visual)));
                }
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
                    pi => pi.SetValue(dataContext, element.DataContext), // Single Content
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
        /// Parent의 자식으로 등록된 Element를 삭제하고 <see cref="IRenderer"/> 및 레이어를 삭제합니다.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="element"></param>
        public void RemoveElement(FrameworkElement parent, FrameworkElement element, bool pushTask = false)
        {
            IRenderer parentRenderer = parent.GetRenderer();
            IRenderer childRenderer = element.GetRenderer();

            // Selection Check
            if (GroupSelector.IsSelected(childRenderer as FrameworkElement))
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

            if (parent.DataContext != null && parent.DataContext is DependencyObject dataContext)
            {
                // Remove On PObject Parent
                ObjectContentHelper.GetContent(
                    dataContext,
                    pi => pi.SetValue(dataContext, null),     // Single Content
                    list => list.SafeRemove(element.DataContext));   // List Content
            }

            // Remove On WPF Parent
            ObjectContentHelper.GetContent(
                parent,
                pi => pi.SetValue(parent, null),     // Single Content
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
            RendererManager.ResolveBinder(childRenderer).SetRenderer(null);
        }
        #endregion

        #region [ Bezier Line Connector ]
        private void Connector_Updated(object sender, EventArgs e)
        {
            LineLayer.InvalidateArrange();
        }
        
        /// <summary>
        /// 연결점을 시각적으로 동기화시켜주는 임시 <see cref="LineConnectorBase"/>를 생성합니다.
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="endPosition"></param>
        /// <returns></returns>
        public LineConnectorBase CreatePendingConnectedLine(
            Func<LineConnectorBase, Point> startPosition,
            Func<LineConnectorBase, Point> endPosition)
        {
            var connector = CreateConnectedLine(startPosition, endPosition);

            connector.Line.IsHitTestVisible = false;

            // add pending line
            pendingLines.Push(connector);
            lineCollection.Remove(connector);

            connector.Updated += Connector_Updated;

            return connector;
        }

        /// <summary>
        /// 두 <see cref="FrameworkElement"/>를 시각적으로 이어주는 임시 <see cref="LineConnector"/>를 생성합니다.
        /// </summary>
        /// <param name="output"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public LineConnector CreatePendingConnectedLine(
            BindThumb output,
            BindThumb target)
        {
            var connector = CreateConnectedLine(output, target);
            
            // add pending line
            pendingLines.Push(connector);
            lineCollection.Remove(connector);

            connector.Updated += Connector_Updated;

            return connector;
        }

        /// <summary>
        /// 연결점을 시각적으로 동기화시켜주는 <see cref="LineConnectorBase"/>를 생성합니다.
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
                connector.Line.LineBrush = Brushes.DimGray;

                BindingEx.SetBinding(
                    ZoomPanel, ZoomPanel.ScaleProperty,
                    connector.Line, BezierLine.StrokeThicknessProperty,
                    converter: new ReciprocalConverter()
                    {
                        Factor = 2
                    });
            }

            LineLayer.Add(connector.Line);
            lineCollection.Add(connector);

            return connector;
        }

        /// <summary>
        /// 두 쌍의 <see cref="BindThumb"/>를 시각적으로 이어주는 <see cref="LineConnector"/>를 생성합니다.
        /// </summary>
        /// <param name="output"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public LineConnector CreateConnectedLine(BindThumb output, BindThumb input)
        {
            if (lineCollection.HasThumbExpression(output, input))
                return null;

            var connector = new LineConnector(this, output, input);
            
            if (ZoomPanel != null)
            {
                connector.Line.LineBrush = Brushes.DimGray;

                BindingEx.SetBinding(
                    ZoomPanel, ZoomPanel.ScaleProperty,
                    connector.Line, BezierLine.StrokeThicknessProperty,
                    converter: new ReciprocalConverter()
                    {
                        Factor = 2
                    });
            }

            LineLayer.Add(connector.Line);
            lineCollection.Add(connector);

            return connector;
        }

        /// <summary>
        /// 연결된 <see cref="LineConnector"/>를 삭제합니다.
        /// </summary>
        /// <param name="lineConnector"></param>
        public void DeleteConnectedLine(LineConnector lineConnector)
        {
            if (!lineCollection.Contains(lineConnector))
                return;

            LineLayer.Remove(lineConnector.Line);
            lineCollection.Remove(lineConnector);
        }

        /// <summary>
        /// 임시로 연결된 <see cref="LineConnectorBase"/>를 삭제합니다.
        /// </summary>
        public void PopPendingConnectedLine()
        {
            if (!HasPendingConnectedLine())
                return;

            var connector = pendingLines.Pop();

            connector.Updated -= Connector_Updated;

            LineLayer.Remove(connector.Line);
            lineCollection.Remove(connector);

            connector.Release();
        }

        /// <summary>
        /// 임시로 생성된 <see cref="LineConnectorBase"/>의 존재 여부를 확인합니다.
        /// </summary>
        /// <returns></returns>
        public bool HasPendingConnectedLine()
        {
            return pendingLines.Count > 0;
        }
        #endregion

        #region [ Logic ]
        /// <summary>
        /// 로직 컴포넌트를 생성합니다.
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
                MessageBox.Show("Coming soon! (Logic)");
                return null;
            }

            var metadata = DesignerManager.GetElementType(pType);
            var control = this.GenerateToElement(this, metadata) as ComponentElement;

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

            Canvas.SetLeft(control, control.SnapToGrid(position.X));
            Canvas.SetTop(control, control.SnapToGrid(position.Y));
            Canvas.SetZIndex(control, 0);

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

        internal void ConnectComponent(BindThumb outputThumb, BindThumb inputThumb)
        {
            outputThumb.Binder.Bind(inputThumb.Binder);

            // Connect
            ConnectComponentLine(outputThumb, inputThumb);
        }

        internal void ConnectComponentLine(BindThumb outputThumb, BindThumb inputThumb)
        {
            LineConnector connector = CreateConnectedLine(outputThumb, inputThumb);
            connector?.Update();
        }

        internal void DisconnectComponentLine(BindThumb outputThumb, BindThumb inputThumb)
        {
            foreach (LineConnector line in lineCollection.FromThumb(outputThumb, inputThumb).ToArray())
            {
                DeleteConnectedLine(line);
            }
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
                var sourceBinder = sourceRenderer.ProvideValue()[BindOptions.Output].First() as PBinder;
                var targetBinder = targetRenderer.ProvideValue()[BindOptions.Input].First() as PBinder;

                ConnectComponent(
                    sourceBinder.GetView<BindThumb>(),
                    targetBinder.GetView<BindThumb>());
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
        public BindOptions BindType { get; set; }
    }
}