using DeXign.Controls;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using DeXign.Core.Controls;
using DeXign.Core.Designer;

namespace DeXign
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : ChromeWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeCommands();
            InitializeLayouts();
        }

        private void InitializeLayouts()
        {
            // ToolBox
            foreach (var element in DesignerManager.GetElementTypes())
            {
                var resource = (DesignerResource)TryFindResource(element.Element);
                
                toolBox.AddItem(new ToolBoxItem(element, resource));
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
        }

        private void NewProject_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("새로 만들기!");
        }

        private void OpenProject_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("열기!");
        }

        const double ScaleRate = 1.1;
        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
      
        }
    }
}
