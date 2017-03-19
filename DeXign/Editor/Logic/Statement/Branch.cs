using System.Windows;
using System.Windows.Controls;

using DeXign.Core.Logic;
using DeXign.Extension;
using DeXign.Resources;

namespace DeXign.Editor.Logic
{
    [TemplatePart(Name = "PART_valueCheckBox", Type = typeof(CheckBox))]
    public class Branch : ComponentElement
    {
        public new PBranch Model => (PBranch)base.Model;

        private CheckBox valueCheckBox;

        protected override void OnAttachedComponentModel()
        {
            base.OnAttachedComponentModel();

            BindingEx.SetBinding(
                valueCheckBox, CheckBox.IsCheckedProperty,
                Model.Value2Binder.GetView<BindThumb>(), BindThumb.IsEnabledProperty,
                converter: ResourceManager.GetConverter("Not"));
        }

        public override void OnApplyContentTemplate()
        {
            base.OnApplyContentTemplate();
            
            valueCheckBox = GetContentTemplateChild<CheckBox>("PART_valueCheckBox");
        }
    }
}
