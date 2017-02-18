using System.Windows;
using System.Windows.Controls;

namespace DeXign.Controls
{
    // 왠지 모르게 InputGesutreText 바인딩이 이상하다.

    [TemplatePart(Name = "PART_ShortCut", Type = typeof(ContentPresenter))]
    public class MenuItemEx : MenuItem
    {
        ContentPresenter shortcut;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            shortcut = GetTemplateChild("PART_ShortCut") as ContentPresenter;
            UpdateShortcut();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property.Name == nameof(InputGestureText))
            {
                UpdateShortcut();
            }
        }

        private void UpdateShortcut()
        {
            if (shortcut == null)
                return;

            if (string.IsNullOrEmpty(InputGestureText))
            {
                shortcut.Visibility = Visibility.Collapsed;
            }
            else
            {
                shortcut.Visibility = Visibility.Visible;
            }
        }
    }
}
