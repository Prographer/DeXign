using System.Windows;
using System.Windows.Controls;
using System.Reflection;

using Microsoft.Win32;

using WPFExtension;

namespace DeXign.Controls
{
    [TemplatePart(Name = "PART_sourceBox", Type = typeof(SubmitTextBox))]
    [TemplatePart(Name = "PART_sourceButton", Type = typeof(Button))]
    [Setter(Key = "ImageSource", Type = typeof(string))]
    class ImageSourceSetter : BaseSetter
    {
        SubmitTextBox sourceBox;
        Button sourceButton;

        public ImageSourceSetter(DependencyObject target, PropertyInfo pi) : base(target, pi)
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            sourceBox = GetTemplateChild<SubmitTextBox>("PART_sourceBox");
            sourceButton = GetTemplateChild<Button>("PART_sourceButton");

            BindingHelper.SetBinding(
                this, ValueProperty,
                sourceBox, SubmitTextBox.TextProperty);

            sourceButton.Click += SourceButton_Click;
        }

        private void SourceButton_Click(object sender, RoutedEventArgs e)
        {
            var op = new OpenFileDialog()
            {
                Title = "이미지를 선택해주세요",
                Filter = "모든 이미지|*.jpg;*.jpeg;*.png|" +
                    "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                    "Portable Network Graphic (*.png)|*.png"
            };

            if (op.ShowDialog() == true)
            {
                Value = op.FileName;
            }
        }
    }
}
