using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Documents;

using DeXign.Core.Controls;
using DeXign.Editor;
using DeXign.Editor.Layer;

using WPFExtension;
using DeXign.Core;

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

        public static ExportRendererAttribute FromModelType<T>(T model)
        {
            return FromModelType(typeof(T));
        }

        public static ExportRendererAttribute FromViewType<T>(T model)
        {
            return FromViewType(typeof(T));
        }

        public static FrameworkElement CreateVisualRenderer(ExportRendererAttribute rendererAttr, Point position)
        {
            if (rendererAttr == null)
            {
                MessageBox.Show("Coming soon!");
                return null;
            }

            var model = (PObject)Activator.CreateInstance(rendererAttr.ModelType);

            return CreateVisualRendererCore(rendererAttr, model, position);
        }

        public static FrameworkElement CreateVisualRendererFromModel(PObject model)
        {
            var rendererAttr = FromModelType(model.GetType());

            if (rendererAttr == null)
                return null;

            return CreateVisualRendererCore(rendererAttr, model, default(Point));
        }

        private static FrameworkElement CreateVisualRendererCore(ExportRendererAttribute rendererAttr, PObject model, Point position)
        {
            var view = (FrameworkElement)Activator.CreateInstance(rendererAttr.ViewType);
            var renderer = (IRenderer)Activator.CreateInstance(rendererAttr.RendererType, view, model);

            // metadata setting
            renderer.Metadata.CreatedTime = DateTime.Now;
            renderer.Metadata.CreatedPosition = position;

            view.DataContext = model;
            view.AddAdorner((Adorner)renderer);

            view.SetRenderer(renderer);
            model.SetRenderer(renderer);

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
