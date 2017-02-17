using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DeXign.Editor.Layer;
using DeXign.Extension;
using DeXign.Core.Designer;
using DeXign.Core;
using System;
using DeXign.Input;
using DeXign.Editor.Renderer;
using System.Windows.Markup;
using System.Collections;
using DeXign.Core.Controls;
using DeXign.Editor.Interfaces;
using System.Reflection;
using DeXign.Models;

namespace DeXign.Editor.Controls
{
    // TODO: 스토리 보드 구현해야함 할게 짱 많네
    class Storyboard : Canvas
    {
        public event EventHandler ElementChanged;

        public GuideLayer GuideLayer { get; }

        public Storyboard()
        {
            GuideLayer = new GuideLayer(this);

            AttachedAdorner.SetAdornerType(this, typeof(GuideLayer));
            AttachedAdorner.SetAdorner(this, GuideLayer);
            AttachedAdorner.SetAdornerIndex(this, 2);

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
                .Select(AttachedAdorner.GetAdorner)
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

        public FrameworkElement GenerateToElement(
            FrameworkElement parent,
            AttributeTuple<DesignElementAttribute, Type> data)
        {
            var position = parent.PointFromScreen(SystemMouse.Position);

            var rendererAttr = RendererManager.FromModelType(data.Element);
            var visual = RendererManager.CreateVisual(rendererAttr);

            if (visual == null)
                return null;

            // Add On PObject Parent
            ElementParentContentCore(
                (DependencyObject)parent.DataContext,
                pi => pi.SetValue(parent.DataContext, visual.DataContext), // Single Content
                list => list.Add(visual.DataContext));                     // List Content

            // Add On WPF Parent
            ElementParentContentCore(
                parent,
                pi => pi.SetValue(parent, visual),  // Single Content
                list => list.Add(visual));          // List Content

            if (!(parent.DataContext is PPage))
            {
                visual.Margin = new Thickness(position.X, position.Y, 0, 0);
                visual.VerticalAlignment = VerticalAlignment.Top;
                visual.HorizontalAlignment = HorizontalAlignment.Left;
            }

            ElementChanged?.Invoke(this, null);

            return visual;
        }
        
        public void RemoveElement(FrameworkElement parent, FrameworkElement element)
        {
            //var childRenderer = (IRenderer)AttachedAdorner.GetAdorner(element);
            //var parentRenderer = AttachedAdorner.GetAdorner(parent);
            
            if (parent.DataContext == null ||
                (parent.DataContext != null && !(parent.DataContext is PObject)))
                return;

            // Remove On PObject Parent
            ElementParentContentCore(
                (DependencyObject)parent.DataContext,
                pi => pi.SetValue(parent.DataContext, null), // Single Content
                list => list.Remove(element.DataContext));   // List Content

            // Remove On WPF Parent
            ElementParentContentCore(
                parent, 
                pi => pi.SetValue(parent, null), // Single Content
                list => list.Remove(element));   // List Content

            ElementChanged?.Invoke(this, null);
        }

        private void ElementParentContentCore(
            DependencyObject parent,
            Action<PropertyInfo> singleContent,
            Action<IList> listContent,
            Action failed = null)
        {
            var attr = parent.GetAttribute<ContentPropertyAttribute>();

            if (attr != null)
            {
                var contentPropertyInfo = parent
                    .GetType()
                    .GetProperty(attr.Name);

                if (contentPropertyInfo.CanCastingTo<DependencyObject>() ||
                    contentPropertyInfo.PropertyType == typeof(object))
                {
                    singleContent?.Invoke(
                        contentPropertyInfo);

                    return;
                }
                else if (contentPropertyInfo.CanCastingTo<IList>())
                {
                    listContent?.Invoke(
                        (IList)contentPropertyInfo.GetValue(parent));

                    return;
                }
            }

            failed?.Invoke();
        }
    }
}
