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
using DeXign.Resources;

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
        public List<PComponent> Components => Model?.Project.Components;

        public bool IsComponentBoxOpen { get { return componentBoxPopup.IsOpen; } }

        internal ZoomPanel ZoomPanel { get; private set; }
        #endregion

        #region [ Local Variable ]
        private ClosableTabItem mangedTabItem;

        private Popup componentBoxPopup;
        private Point componentBoxPosition;
        private ComponentBox componentBox;

        private Stack<LineConnectorBase> pendingLines;
        private LineConnectorCollection lineCollection;

        // for unscaling
        private ReciprocalConverter scaleConverter;
        private ScaleTransform scaleTransform;
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
                Placement = PlacementMode.Custom,
                StaysOpen = false,
                CustomPopupPlacementCallback = new CustomPopupPlacementCallback(ComponentBoxPlaceCallback)
            };

            componentBoxPopup.Closed += ComponentBoxPopup_Closed;
            componentBox.ItemSelected += ComponentBox_ItemSelected;

            // Line Update Timer
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            foreach (var line in lineCollection)
                line.Update();

            LineLayer.InvalidateArrange();
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

                scaleConverter = new ReciprocalConverter();
                scaleTransform = new ScaleTransform();

                // ParentScale X -> scale X
                BindingEx.SetBinding(
                    ZoomPanel, ZoomPanel.ScaleProperty,
                    scaleTransform, ScaleTransform.ScaleXProperty,
                    converter: scaleConverter);

                // ParentScale Y -> scale Y
                BindingEx.SetBinding(
                    ZoomPanel, ZoomPanel.ScaleProperty,
                    scaleTransform, ScaleTransform.ScaleYProperty,
                    converter: scaleConverter);
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
            {
                mangedTabItem = frame.Parent as ClosableTabItem;
                mangedTabItem.Closed += MangedTabItem_Closed;
            }
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
        /// ���õ� ���̾ �����ɴϴ�.
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
        /// ȭ���� �����մϴ�.
        /// </summary>
        /// <returns></returns>
        public ContentControl AddNewScreen()
        {
            var metadata = DesignerManager.GetElementType(typeof(PContentPage));
            var control = this.GenerateToElement(this, metadata.Element, pushTask: false) as ContentControl;
            var model = (PContentPage)control.GetRenderer().Model;

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
            Type type,
            Point position = default(Point),
            bool pushTask = true)
        {
            var rendererAttr = RendererManager.FromModelType(type);
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

            this.AddElementCore(childRenderer);

            // Notice child added
            parentRenderer?.AddChild(childRenderer, childRenderer.Metadata.CreatedPosition);
        }

        private void AddElementCore(IRenderer childRenderer)
        {
            if (childRenderer.Model is PContentPage screen)
            {
                this.Screens?.SafeAdd(screen);
            }
            else if (childRenderer.Model is PComponent component)
            {
                this.Components?.SafeAdd(component);
            }
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

            RemoveElementCore(childRenderer);

            // Notice child removed 
            parentRenderer?.RemoveChild(childRenderer);
        }

        private void RemoveElementCore(IRenderer childRenderer)
        {
            if (childRenderer.Model is PContentPage screen)
            {
                this.Screens?.SafeRemove(screen);
            }
            else if (childRenderer.Model is PComponent component)
            {
                this.Components?.SafeRemove(component);
            }
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

            connector.Line.IsHitTestVisible = false;

            // add pending line
            pendingLines.Push(connector);
            lineCollection.Remove(connector);

            connector.Updated += Connector_Updated;

            return connector;
        }

        /// <summary>
        /// �� <see cref="FrameworkElement"/>�� �ð������� �̾��ִ� �ӽ� <see cref="LineConnector"/>�� �����մϴ�.
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
        /// �������� �ð������� ����ȭ�����ִ� <see cref="LineConnectorBase"/>�� �����մϴ�.
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
        /// �� ���� <see cref="BindThumb"/>�� �ð������� �̾��ִ� <see cref="LineConnector"/>�� �����մϴ�.
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
        /// ����� <see cref="LineConnector"/>�� �����մϴ�.
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
        /// �ӽ÷� ����� <see cref="LineConnectorBase"/>�� �����մϴ�.
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
            Type pType = model.ItemModelType;

            if (pType == null)
            {
                MessageBox.Show("Coming soon! (Logic)");
                return null;
            }

            var metadata = DesignerManager.GetElementType(pType);
            var control = this.GenerateToElement(this, metadata.Element, position) as ComponentElement;

            // Trigger Setting
            if (model.ComponentType == ComponentType.Event)
            {
                var triggerRenderer = control.GetRenderer() as TriggerRenderer;

                triggerRenderer.Model.SetRuntimeEvent(model.Data as EventInfo);
            }

            // Selector Setting
            if (model.ComponentType == ComponentType.Instance)
            {
                var selectorRenderer = control.GetRenderer() as SelectorRenderer;

                selectorRenderer.Model.Title = model.Title;
                selectorRenderer.Model.TargetVisual = model.Data as PVisual;
            }

            // Function Setting
            if (model.ComponentType == ComponentType.Function)
            {
                var functionRenderer = control.GetRenderer() as FunctionRenderer;

                functionRenderer.Model.SetRuntimeFunction(model.Data as MethodInfo);
            }

            ZoomFocusTo(control.GetRenderer());

            return control;
        }

        private void Storyboard_Deactivated(object sender, EventArgs e)
        {
            CloseComponentBox();
        }

        private void ComponentBoxPopup_Closed(object sender, EventArgs e)
        {
            CloseComponentBox();
        }

        internal void OpenComponentBox(object componentTarget)
        {
            // Set Target
            componentBox.TargetObject = componentTarget;

            // Set Last Open Position
            componentBoxPosition = SystemMouse.Position;

            // Popup Child Force Measure
            componentBox.Measure(
                new Size(componentBox.MaxWidth, componentBox.MaxHeight));

            // Popup Open
            componentBoxPopup.IsOpen = true;
        }

        private CustomPopupPlacement[] ComponentBoxPlaceCallback(Size popupSize, Size targetSize, Point offset)
        {
            var position = new Point(componentBoxPosition.X - 7, componentBoxPosition.Y - componentBox.DesiredSize.Height / 2);

            return new CustomPopupPlacement[]
            {
                new CustomPopupPlacement(position, PopupPrimaryAxis.Horizontal | PopupPrimaryAxis.Vertical)
            };
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

            if (connector != null)
            {
                connector.Line.LineBrush = new SolidColorBrush(Color.FromArgb(80, 255, 255, 255));
                if (outputThumb.Binder.BindOption == BindOptions.Output && inputThumb.BindOption == BindOptions.Input)
                    connector.Line.LineBrush = ResourceManager.GetBrush("Flat.Accent.DeepDark");
            }



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
            if (!IsComponentBoxOpen)
                return;

            // Close Opened Box
            CloseComponentBox();

            // Create Component
            FrameworkElement visual = AddNewComponent(model, this.PointFromScreen(componentBoxPosition));
            IRenderer targetRenderer = visual.GetRenderer();

            // * Logic Binding *
            PBinder outputBinder = null;
            PBinder inputBinder = null;

            // Component -> Component
            if (componentBox.TargetObject is BindRequest request)
            {
                var host = request.Source.Binder.Host as PBinderHost;
                IRenderer renderer = host.GetRenderer();
                IBinderHost targetHost = targetRenderer.ProvideValue();

                PBinder sourceBinder = request.Source.Binder;
                PBinder targetBinder = targetHost.GetConnectableBinders(sourceBinder).FirstOrDefault() as PBinder;

                if (sourceBinder == null || targetBinder == null)
                {
                    // ���� ����
                    return;
                }

                BinderHelper.GetPairObject(
                    ref outputBinder, ref inputBinder,
                    (sourceBinder, sourceBinder.BindOption),
                    (targetBinder, targetBinder.BindOption));
            }

            // Layout -> Component
            if (componentBox.TargetObject is PObject pObj)
            {
                IRenderer renderer = pObj.GetRenderer();

                outputBinder = renderer.ProvideValue()[BindOptions.Output].FirstOrDefault() as PBinder;
                inputBinder = targetRenderer.ProvideValue()[BindOptions.Input].FirstOrDefault() as PBinder;
            }

            if (outputBinder == null || inputBinder == null)
            {
                // ���� ����
                return;
            }

            ConnectComponent(
                outputBinder.GetView<BindThumb>(),
                inputBinder.GetView<BindThumb>());
        }
        #endregion

        #region [ Zoom ]
        public void ZoomFocusTo(IRenderer renderer, bool maintainScale = true)
        {
            if (!renderer.Element.IsLoaded)
                DispatcherEx.WaitForRender();

            Point pt1 = renderer.Element.TranslatePoint(new Point(), this);
            Point pt2 = renderer.Element.TranslatePoint((Point)renderer.Element.RenderSize, this);

            Rect bound = new Rect(pt1, pt2);

            Vector blank = (Vector)this.ZoomPanel.RenderSize - (Vector)bound.Size;
            blank.X /= 2 * (maintainScale ? this.ZoomPanel.Scale : 1);
            blank.Y /= 2 * (maintainScale ? this.ZoomPanel.Scale : 1);

            bound.Inflate(blank.X, blank.Y);

            this.ZoomPanel.ZoomFit(bound, true);
        }
        #endregion

        public void SetUnscaledControl(FrameworkElement element)
        {
            element.LayoutTransform = scaleTransform;
        }

        public void SetUnscaledProperty(FrameworkElement element, DependencyProperty property)
        {
            element.LayoutTransform = scaleTransform;
            return;
            if (property.PropertyType != typeof(double))
                throw new ArgumentException("Support only double property");

            double factor = (double)element.GetValue(property);

            BindingEx.SetBinding(
                ZoomPanel, ZoomPanel.ScaleProperty,
                element, property,
                converter: new ReciprocalConverter()
                {
                    Factor = factor
                });
        }

        public void Close()
        {
            mangedTabItem?.Close();
        }

        private void MangedTabItem_Closed(object sender, EventArgs e)
        {
            Model.Project.Close();
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