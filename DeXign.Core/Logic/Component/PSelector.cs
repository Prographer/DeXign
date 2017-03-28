using System;
using System.Windows;

using DeXign.Core.Controls;

using WPFExtension;

namespace DeXign.Core.Logic
{
    [CSharpCodeMap("{Target}")]
    [JavaCodeMap("{Target}")]
    [DesignElement(Category = Constants.Logic.Default, DisplayName = "객체 가져오기", Visible = false)]
    public class PSelector : PComponent
    {
        public static readonly DependencyProperty TargetVisualProperty =
            DependencyHelper.Register();

        public PVisual TargetVisual
        {
            get { return GetValue<PVisual>(TargetVisualProperty); }
            set { SetValue(TargetVisualProperty, value); }
        }

        public PReturnBinder ReturnBinder { get; set; }

        public PSelector()
        {
            this.ClearBinder(BindOptions.Input | BindOptions.Parameter | BindOptions.Return);
            
            this.ReturnBinder = this.AddReturnBinder("", typeof(PVisual));

            TargetVisualProperty.AddValueChanged(this, TargetVisual_Changed);
        }

        private void TargetVisual_Changed(object sender, EventArgs e)
        {
            this.ReturnBinder.ReturnType = this.TargetVisual?.GetType();

            OnTargetVisualChanged();
        }

        protected virtual void OnTargetVisualChanged()
        {
        }
    }
}
