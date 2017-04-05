using System;
using System.Windows;

using DeXign.Core.Logic;
using DeXign.Core.Controls;

using WPFExtension;
using DeXign.Editor.Layer;
using DeXign.Editor.Renderer;
using System.Windows.Controls;
using DeXign.Extension;
using DeXign.Core;
using DeXign.Converter;
using System.Windows.Media;

namespace DeXign.Editor.Logic
{
    [TemplatePart(Name = "PART_nameBlock", Type = typeof(TextBlock))]
    public class ObjectSelector : ComponentElement
    {
        public static readonly DependencyProperty TargetRendererProperty =
            DependencyHelper.Register();

        public IRenderer TargetRenderer
        {
            get { return this.GetValue<IRenderer>(TargetRendererProperty); }
            set { SetValue(TargetRendererProperty, value); }
        }

        public new PSelector Model => (PSelector)base.Model;

        private TextBlock nameBlock;
        internal BindThumb ReturnThumb { get; private set; }

        public ObjectSelector()
        {
            TargetRendererProperty.AddValueChanged(this, TargetRenderer_Changed);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            nameBlock = GetTemplateChild<TextBlock>("PART_nameBlock");

            if (this.Model.TargetVisual != null)
            {
                string displayName = this.Model.TargetVisual.GetAttribute<DesignElementAttribute>().DisplayName;

                var b = BindingHelper.SetBinding(
                    this.Model.TargetVisual, PObject.NameProperty,
                    nameBlock, TextBlock.TextProperty,
                    converter: new FallbackStringConverter()
                    {
                        FallbackValue = $"<이름 없음> ({displayName})"
                    });
            }
        }

        protected override void OnAttachedComponentModel()
        {
            base.OnAttachedComponentModel();

            PSelector.TargetVisualProperty.AddValueChanged(this.Model, TargetVisualModel_Changed);

            ReturnThumb = ReturnThumbs[0];
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
    }
}
