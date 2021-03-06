using System.Windows;
using System.Windows.Controls;

using WPFExtension;

namespace DeXign.Controls
{
    public class HintTextBox : TextBox
    {
        public static readonly DependencyProperty TextHintProperty =
            DependencyHelper.Register();

        public string TextHint
        {
            get { return this.GetValue<string>(TextHintProperty); }
            set { SetValue(TextHintProperty, value); }
        }
    }
}
