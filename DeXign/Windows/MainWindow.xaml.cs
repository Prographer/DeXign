using DeXign.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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

        public enum SexType { Male, Female };

        public class User
        {
            public string Name { get; set; }

            public int Age { get; set; }

            public string Mail { get; set; }

            public string Sex { get; set; }
        }
    }
}
