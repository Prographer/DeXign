using System.Windows;
using System.Windows.Input;

using DeXign.Models;
using DeXign.Controls;
using DeXign.Resources;
using DeXign.Core.Designer;
using System.Windows.Controls;
using System;
using DeXign.Windows.Pages;

namespace DeXign
{
    public partial class MainWindow : ChromeWindow, IViewModel<MainModel>
    {
        public MainModel Model { get; set; }
        
        public MainWindow()
        {
            InitializeComponent();
            InitializeCommands();
            InitializeLayouts();

            Model = new MainModel();
            this.DataContext = Model;
        }

        private void InitializeLayouts()
        {
            // ToolBox
            foreach (var element in DesignerManager.GetElementTypes())
            {
                if (!element.Attribute.Visible)
                    continue;

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
        }
        
        private void NewProject_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            CreateStoryboardPage();
        }

        private void OpenProject_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            MessageBox.Show("열기!");
        }

        public void CreateStoryboardPage()
        {
            var page = new StoryboardPage();

            tabControl.Items.Add(
                new ClosableTabItem()
                {
                    Header = "App1",
                    IsSelected = true,
                    Content = new Frame()
                    {
                        Content = page
                    },
                    Tag = page.Model
                });
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (ClosableTabItem)tabControl.SelectedItem;
            var itemModel = (StoryboardModel)item?.Tag;

            Model.StoryboardPage = (StoryboardPage)itemModel?.ViewModel;
        }
    }
}
