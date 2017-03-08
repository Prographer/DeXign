using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using DeXign.Controls;
using DeXign.Core.Designer;
using DeXign.Models;
using DeXign.Resources;
using DeXign.Windows.Pages;
using DeXign.Editor;
using DeXign.IO;

using Microsoft.Win32;
using System.IO;

namespace DeXign.Windows
{
    public partial class EditorWindow : ChromeWindow, IViewModel<MainModel>
    {
        public MainModel Model { get; set; }

        public EditorWindow(DXProject project)
        {
            InitializeComponent();
            InitializeCommands();
            InitializeLayouts();
            
            Model = new MainModel();
            this.DataContext = Model;
            
            GroupSelector.SelectedItemChanged += GroupSelector_SelectedItemChanged;

            // Load
            OpenStoryboardPage(project);
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
            DXProject project = DXProject.OpenDialog();

            if (project != null)
                OpenStoryboardPage(project);
        }

        public void CreateNewProject()
        {
            var projDialog = new ProjectDialog();
            
            if (projDialog.ShowDialog())
            {
                string fileName = Path.Combine(projDialog.Directory, $"{projDialog.AppName}.dx");

                var project = DXProject.Create(
                    fileName,
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
