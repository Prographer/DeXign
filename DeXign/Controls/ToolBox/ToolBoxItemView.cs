using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

using DeXign.Models;
using DeXign.Windows;
using DeXign.OS;
using System.Windows.Markup;
using DeXign.Extension;
using DeXign.Core.Logic;
using DeXign.Editor.Renderer;
using DeXign.Editor;
using DeXign.Editor.Logic;
using System.Collections.Generic;

namespace DeXign.Controls
{
    [TemplatePart(Name = "PART_contentPresenter", Type = typeof(ContentPresenter))]
    class ToolBoxItemView : ListViewItem
    {
        public ToolBoxItemModel Model => (ToolBoxItemModel)DataContext;

        public string Category => Model.Category;

        public bool IsComponent => Model.Metadata.Element.CanCastingTo<PComponent>();

        private Point beginPosition;
        private Point beginContentOffset;

        private ContentPresenter contentPresenter;
        private FloatingWindow previewWindow;

        private static Dictionary<Type, ComponentElement> componentCache;

        public ToolBoxItemView(ToolBoxItemModel model)
        {
            this.DataContext = model;

            componentCache = new Dictionary<Type, ComponentElement>();
        }
        
        private void ShowPreviewWindow()
        {
            if (previewWindow != null)
                HidePreviewWindow();

            FrameworkElement element;
            Size size;

            if (this.IsComponent)
            {
                element = CreateComponentElement();

                size = new Size(double.NaN, double.NaN);
            }
            else
            {
                // Content Copy
                element = XamlReader.Parse(XamlWriter.Save(contentPresenter.Content)) as FrameworkElement;

                size = contentPresenter.RenderSize;
            }

            previewWindow = new FloatingWindow(element, size.Width, size.Height);

            UpdatePreviewWindow();

            previewWindow.Show();
        }

        private ComponentElement CreateComponentElement()
        {
            if (!componentCache.ContainsKey(Model.Metadata.Element))
            {
                ExportRendererAttribute rAttr = RendererManager.FromModelType(Model.Metadata.Element);

                var model = Activator.CreateInstance(rAttr.ModelType) as PComponent;
                var view = Activator.CreateInstance(rAttr.ViewType) as ComponentElement;

                view.SetIsFloating(true);
                view.SetComponentModel(model);

                view.HorizontalAlignment = HorizontalAlignment.Left;
                view.VerticalAlignment = VerticalAlignment.Top;

                view.Margin = new Thickness(10);

                componentCache[Model.Metadata.Element] = view;
            }

            return componentCache[Model.Metadata.Element];
        }

        private void HidePreviewWindow()
        {
            previewWindow?.Close();

            previewWindow = null;
        }

        private void UpdatePreviewWindow()
        {
            if (previewWindow == null)
                return;

            Point position = SystemMouse.Position - (Vector)beginContentOffset;

            // 컴포넌트인경우 센터 정렬
            if (this.IsComponent)
                position = SystemMouse.Position - new Vector(previewWindow.Width / 2, previewWindow.Height / 2);

            previewWindow.SetPosition(position);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            
            beginPosition = e.GetPosition(this);
            beginContentOffset = e.GetPosition(contentPresenter);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point position = e.GetPosition(this);
                Size dragSize = new Size(
                    SystemParameters.MinimumHorizontalDragDistance,
                    SystemParameters.MinimumVerticalDragDistance);

                if (Math.Abs(position.X - this.beginPosition.X) > dragSize.Width / 2 ||
                    Math.Abs(position.Y - this.beginPosition.Y) > dragSize.Height / 2)
                {
                    ShowPreviewWindow();

                    DragDrop.DoDragDrop(this, Model.Metadata, DragDropEffects.None);

                    HidePreviewWindow();

                    Selector.SetIsSelected(this, false);
                }
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            Selector.SetIsSelected(this, false);
        }

        protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            e.Handled = true;

            UpdatePreviewWindow();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            contentPresenter = GetTemplateChild("PART_contentPresenter") as ContentPresenter;
        }
    }
}
