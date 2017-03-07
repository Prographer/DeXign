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
using DeXign.IO;
using DeXign.Core.Controls;
using DeXign.Core;
using Microsoft.Win32;

namespace DeXign.Windows
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

            this.CommandBindings.Add(
                new CommandBinding(
                    DXCommands.SaveProjectCommand, SaveProject_Execute));

            this.CommandBindings.Add(
                new CommandBinding(
                    DXCommands.UndoCommand, Undo_Execute));

            this.CommandBindings.Add(
                new CommandBinding(
                    DXCommands.RedoCommand, Redo_Execute));
        }

        #region [ Commands ]
        private void Redo_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            TaskNavigator.TaskManager.Redo();
        }

        private void Undo_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            TaskNavigator.TaskManager.Undo();
        }

        private void NewProject_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            CreateNewProject();
        }

        private void OpenProject_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            OpenProject();
        }

        private void SaveProject_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            SaveProject();
        }
        #endregion

        #region [ Project Handling ]
        private void SaveProject()
        {
            if (Model.StoryboardPage != null)
            {
                // WindowModel -> StoryboardPage Model -> DXProject . Save

                Model.StoryboardPage
                    .Model.Project.Save();
            }
        }

        private void OpenProject()
        {
            var fileDialog = new OpenFileDialog()
            {
                InitialDirectory = Environment.CurrentDirectory,
                Filter = "DeXign 프로젝트 파일(*.dx)|*.dx"
            };

            bool? result = fileDialog.ShowDialog();

            if (result != null && result.Value)
            {
                var project = DXProject.Open(fileDialog.FileName);

                if (!project.CanOpen)
                {
                    // 메박 커스텀하고 내용 바꿀..
                    MessageBox.Show("어디 나사하나 빠진 파일 같습니다.");
                    return;
                }

                OpenStoryboardPage(project);
            }
        }

        public void CreateNewProject()
        {
            var projDialog = new ProjectDialog();

            if (projDialog.ShowDialog())
            {
                var project = DXProject.Create(
                    $"{projDialog.AppName}.dx",
                    new DXProjectManifest()
                    {
                        ProjectName = projDialog.AppName
                    });

                OpenStoryboardPage(project);
            }
        }
        #endregion

        #region [ Storyboard Handling ]
        public void OpenStoryboardPage(DXProject project)
        {
            var page = new StoryboardPage();

            // DeXign Project
            page.Model.Project = project;

            tabControl.Items.Add(
                new ClosableTabItem()
                {
                    Header = project.Manifest.ProjectName,
                    IsSelected = true,
                    Content = new Frame()
                    {
                        Content = page
                    },
                    Tag = page.Model
                });
        }
        #endregion

        #region [ Layout ]
        private void tabControl_SelectionChanged(object sender, global::System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var item = (ClosableTabItem)tabControl.SelectedItem;
            var itemModel = (StoryboardModel)item?.Tag;

            Model.StoryboardPage = itemModel?.ViewModel;
        }
        #endregion
    }
}
