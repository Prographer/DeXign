using System.Windows;
using System.Windows.Input;

using DeXign.Models;
using DeXign.Controls;
using DeXign.Resources;
using DeXign.Core.Designer;
using System;
using System.Windows.Controls;

namespace DeXign
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : ChromeWindow
    {
        // TODO: Just Test Code
        ResourceDictionary androidStyle;
        ResourceDictionary iosStyle;

        public MainWindow()
        {
            InitializeResources();
            InitializeComponent();
            InitializeCommands();
            InitializeLayouts();

            storyboard.Resources = androidStyle;
        }

        private void InitializeResources()
        {
            // TODO: Just Test Code
            androidStyle = new ResourceDictionary()
            {
                Source = new Uri("/DeXign;component/Themes/Platforms/AndroidStyle.xaml", UriKind.RelativeOrAbsolute)
            };

            iosStyle = new ResourceDictionary()
            {
                Source = new Uri("/DeXign;component/Themes/Platforms/iOSStyle.xaml", UriKind.RelativeOrAbsolute)
            };
        }

        private void InitializeLayouts()
        {
            // ToolBox
            foreach (var element in DesignerManager.GetElementTypes())
            {
                var resource = ResourceManager.GetDesignerResource(element.Element);
                
                toolBox.AddItem(
                    new ToolBoxItemView(
                        new ToolBoxItemModel(element, resource)));
            }
        }

        private void InitializeCommands()
        {
            this.CommandBindings.Add(
                new CommandBinding(
                    DXCommands.OpenProjectCommand, OpenProject_Execute));

            this.CommandBindings.Add(
                new CommandBinding(
                    DXCommands.NewProjectCommand, NewProject_Execute));

            this.CommandBindings.Add(
                new CommandBinding(
                    DXCommands.PlatformCommand, PlatformChanged));
        }

        private void PlatformChanged(object sender, ExecutedRoutedEventArgs e)
        {
            // TODO: Just Test Code
            switch ((string)e.Parameter)
            {
                case "Android":
                    storyboard.Resources = androidStyle;
                    break;

                case "iOS":
                    storyboard.Resources = iosStyle;
                    break;
            }
        }

        private void NewProject_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("새로 만들기!");
        }

        private void OpenProject_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("열기!");
        }
    }
}
