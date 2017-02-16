using DeXign.Editor.Layer;
using DeXign.Core.Controls;

using System;
using System.Linq;
using System.Windows;
using System.Reflection;
using System.Collections.Generic;

namespace DeXign.Editor.Renderer
{
    public static class RendererManager
    {
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

            view.DataContext = model;
            AttachedAdorner.SetAdornerType(view, rendererAttr.RendererType);

            return view;
        }
    }
}
