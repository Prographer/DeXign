using DeXign.Editor.Layer;
using DeXign.Core.Controls;

using System;
using System.Linq;
using System.Windows;
using System.Reflection;
using System.Collections.Generic;
using DeXign.Editor;
using System.Windows.Documents;
using WPFExtension;

namespace DeXign.Editor.Renderer
{
    public static class RendererManager
    {
        public static readonly DependencyProperty RendererProperty =
            DependencyHelper.RegisterAttached<IRenderer>();

        public static Assembly Assembly { get; private set; }

        public static List<ExportRendererAttribute> Items { get; set; }

        static RendererManager()
        {
            Assembly = Assembly.GetExecutingAssembly();
         
            Items = Assembly
                .GetCustomAttributes(typeof(ExportRendererAttribute), false)
                .Cast<ExportRendererAttribute>()
                .ToList();
        }

        public static ExportRendererAttribute FromModelType(Type modelType)
        {
            return Items.FirstOrDefault(item => item.ModelType == modelType);
        }

        public static ExportRendererAttribute FromViewType(Type viewType)
        {
            return Items.FirstOrDefault(item => item.ViewType == viewType);
        }

        public static ExportRendererAttribute FromModelType<T>()
        {
            return FromModelType(typeof(T));
        }

        public static ExportRendererAttribute FromViewType<T>()
        {
            return FromViewType(typeof(T));
        }

        public static FrameworkElement CreateVisual(ExportRendererAttribute rendererAttr)
        {
            if (rendererAttr == null)
            {
                MessageBox.Show("Coming soon!");
                return null;
            }

            var model = (PObject)Activator.CreateInstance(rendererAttr.ModelType);
            var view = (FrameworkElement)Activator.CreateInstance(rendererAttr.ViewType);
            var renderer = (IRenderer)Activator.CreateInstance(rendererAttr.RendererType, view, model);
            
            view.DataContext = model;
            view.AddAdorner((Adorner)renderer);
            view.SetRenderer(renderer);

            return view;
        }

        #region [ Extension ]
        public static void SetRenderer(this DependencyObject element, IRenderer renderer)
        {
            element.SetValue(RendererProperty, renderer);
        }

        public static IRenderer GetRenderer(this DependencyObject element)
        {
            return (IRenderer)element.GetValue(RendererProperty);
        }
        #endregion
    }
}
