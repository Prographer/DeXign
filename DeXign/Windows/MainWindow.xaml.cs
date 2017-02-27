using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using DeXign.Controls;
using DeXign.Core.Designer;
using DeXign.Models;
using DeXign.Resources;
using DeXign.Windows.Pages;
using DeXign.Editor;
using System.Linq;
using DeXign.Core.Controls;
using WPFExtension;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using DeXign.Animation;

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
            
            // test
            GroupSelector.SelectedItemChanged += GroupSelector_SelectedItemChanged;
        }
        
        private void GroupSelector_SelectedItemChanged(object sender, EventArgs e)
        {
            propertyGrid.SelectedObjects = GroupSelector.GetSelectedItems()
                .Cast<IRenderer>()
                .Select(r => (object)r.Model)
                .ToArray();
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

        private void tabControl_SelectionChanged(object sender, global::System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var item = (ClosableTabItem)tabControl.SelectedItem;
            var itemModel = (StoryboardModel)item?.Tag;

            Model.StoryboardPage = itemModel?.ViewModel;
        }
    }
}
