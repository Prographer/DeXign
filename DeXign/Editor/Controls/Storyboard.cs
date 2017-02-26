using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

using DeXign.Core;
using DeXign.Core.Controls;
using DeXign.Core.Designer;
using DeXign.Editor.Layer;
using DeXign.Editor.Renderer;
using DeXign.Extension;

namespace DeXign.Editor.Controls
{
    public partial class Storyboard : Canvas
    {
        public event EventHandler ElementChanged;

        #region [ Properties ]
        public GuideLayer GuideLayer { get; private set; }
        public StoryboardRenderer Renderer { get; private set; }

        public List<ContentControl> Screens { get; } = new List<ContentControl>();
        #endregion

        #region [ Constructor ]
        public Storyboard()
        {
            InitializeLayer();
            InitializeBindings();
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

            this.CommandBindings.Add(
                new CommandBinding(DXCommands.ESCCommand, ESC_Execute));

            this.CommandBindings.Add(
                new CommandBinding(DXCommands.DeleteCommand, Delete_Execute));
        }

        private void InitializeLayer()
        {
            GuideLayer = new GuideLayer(this);
            Renderer = new StoryboardRenderer(this);

            this.AddAdorner(GuideLayer);
            this.AddAdorner(Renderer);

            GuideLayer.SetAdornerIndex(2);
            Renderer.SetAdornerIndex(2);

            this.SetRenderer(Renderer);
        }
        #endregion

        #region [ Input ]
        private void Delete_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var layers = GroupSelector.GetSelectedItems()
                .Cast<SelectionLayer>()
                .ToArray();

            if (layers.Length > 0)
            {
                foreach (var layer in layers)
                {
                    RemoveElement(
                        (FrameworkElement)layer.AdornedElement.Parent,
                        layer.AdornedElement);
                }

                GroupSelector.UnselectAll();
            }
        }

        private void ESC_Execute(object sender, ExecutedRoutedEventArgs e)
        {
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

        protected SelectionLayer GetSelectedLayer()
        {
            var items = GroupSelector.GetSelectedItems();

            if (items?.Count() == 1)
            {
                object item = items.First();

                if (item is SelectionLayer)
                {
                    return item as SelectionLayer;
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
        public ContentControl AddNewScreen()
        {
            var metadata = DesignerManager
                .GetElementTypes()
                .FirstOrDefault(at => at.Element == typeof(PContentPage));

            var control = this.GenerateToElement(this, metadata) as ContentControl;

            control.Margin = new Thickness(0);
            control.VerticalAlignment = VerticalAlignment.Top;
            control.HorizontalAlignment = HorizontalAlignment.Left;

            control.Width = 360;
            control.Height = 615;

            Canvas.SetTop(control, 80);
            Canvas.SetLeft(control, 80);

            Screens.Add(control);

            return control;
        }

        public FrameworkElement GenerateToElement(
            FrameworkElement parent,
            AttributeTuple<DesignElementAttribute, Type> data)
        {
            var rendererAttr = RendererManager.FromModelType(data.Element);
            var visual = RendererManager.CreateVisual(rendererAttr);

            if (visual == null)
                return null;

            IRenderer parentRenderer = parent.GetRenderer();
            IRenderer childRenderer = visual.GetRenderer();

            if (parent.DataContext != null && parent.DataContext is DependencyObject)
            {
                // Add On PObject Parent
                VisualContentHelper.GetContent(
                    (DependencyObject)parent.DataContext,
                    pi => pi.SetValue(parent.DataContext, visual.DataContext), // Single Content
                    list => list.Add(visual.DataContext));                     // List Content
            }

            // Add On WPF Parent
            VisualContentHelper.GetContent(
                parent,
                pi => pi.SetValue(parent, visual),  // Single Content
                list => list.Add(visual));          // List Content

            // Notice child added
            parentRenderer?.OnAddedChild(childRenderer);

            ElementChanged?.Invoke(this, null);

            return visual;
        }

        public void RemoveElement(FrameworkElement parent, FrameworkElement element)
        {
            IRenderer parentRenderer = parent.GetRenderer();
            IRenderer childRenderer = element.GetRenderer();

            if (parent.DataContext == null ||
                (parent.DataContext != null && !(parent.DataContext is PObject)))
                return;

            // Dispose
            if (childRenderer is IDisposable)
                ((IDisposable)childRenderer).Dispose();

            // Remove On AdornerLayer
            element.RemoveAdorner((Adorner)childRenderer);
            element.SetRenderer(null);
            childRenderer.Model.SetRenderer(null);

            // Remove On PObject Parent
            VisualContentHelper.GetContent(
                (DependencyObject)parent.DataContext,
                pi => pi.SetValue(parent.DataContext, null), // Single Content
                list => list.Remove(element.DataContext));   // List Content

            // Remove On WPF Parent
            VisualContentHelper.GetContent(
                parent,
                pi => pi.SetValue(parent, null), // Single Content
                list => list.Remove(element));   // List Content

            // Notice child removed 
            parentRenderer?.OnRemovedChild(childRenderer);

            ElementChanged?.Invoke(this, null);
        }
        #endregion

        #region [ Bezier Line Connector ]
        private void Connector_Updated(object sender, EventArgs e)
        {
            Renderer.InvalidateArrange();
        }

        public LineConnectorBase CreateConnectedLine(
            Func<LineConnectorBase, Point> startPosition,
            Func<LineConnectorBase, Point> endPosition)
        {
            var connector = new LineConnectorBase(this, startPosition, endPosition);

            connector.Updated += Connector_Updated;

            Renderer.Add(connector.Line);

            return connector;
        }

        public LineConnector CreateConnectedLine(
            FrameworkElement source,
            FrameworkElement target)
        {
            var connector = new LineConnector(this, source, target);

            connector.Updated += Connector_Updated;

            Renderer.Add(connector.Line);

            return connector;
        }

        public void RemoveConnectedLine(LineConnectorBase connector)
        {
            connector.Updated -= Connector_Updated;

            Renderer.Remove(connector.Line);
            connector.Release();
        }
        #endregion
    }
}
