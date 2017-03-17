using System;
using System.Windows;

using DeXign.Core.Logic;
using DeXign.Core.Controls;

using WPFExtension;
using DeXign.Editor.Layer;
using DeXign.Editor.Renderer;

namespace DeXign.Editor.Logic
{
    public class ObjectSelector : ComponentElement
    {
        public static readonly DependencyProperty TargetRendererProperty =
            DependencyHelper.Register();

        public IRenderer TargetRenderer
        {
            get { return (IRenderer)GetValue(TargetRendererProperty); }
            set { SetValue(TargetRendererProperty, value); }
        }

        public new PSelector Model => (PSelector)base.Model;

        public ObjectSelector()
        {
            TargetRendererProperty.AddValueChanged(this, TargetRenderer_Changed);
        }

        protected override void OnAttachedComponentModel()
        {
            base.OnAttachedComponentModel();

            PSelector.TargetVisualProperty.AddValueChanged(this.Model, TargetVisualModel_Changed);
        }

        private void TargetVisualModel_Changed(object sender, EventArgs e)
        {
            this.TargetRenderer = this.Model.TargetVisual.GetRenderer();
        }

        private void TargetRenderer_Changed(object sender, EventArgs e)
        {
            if (this.TargetRenderer.Model is PVisual visualModel)
            {
                this.Model.TargetVisual = visualModel;
            }

            OnTargetRendererChangd();
        }

        protected virtual void OnTargetRendererChangd()
        {
        }

        protected override void OnUnSelected()
        {
            base.OnUnSelected();

            if (this.TargetRenderer is SelectionLayer layer)
            {
                layer.IsHighlight = false;
            }
        }

        protected override void OnSelected()
        {
            base.OnSelected();

            if (this.TargetRenderer is SelectionLayer layer)
            {
                layer.IsHighlight = true;
            }
        }
    }
}
