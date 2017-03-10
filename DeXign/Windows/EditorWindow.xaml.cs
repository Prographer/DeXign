using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

using DeXign.IO;
using DeXign.Controls;
using DeXign.Core.Designer;
using DeXign.Models;
using DeXign.Resources;
using DeXign.Windows.Pages;
using DeXign.Editor;
using DeXign.Database;
using DeXign.Extension;

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

            UpdateRecentMenu();

            Model = new MainModel();
            this.DataContext = Model;
            
            GroupSelector.SelectedItemChanged += GroupSelector_SelectedItemChanged;

            // Load
            OpenStoryboardPage(project);
        }

        private void UpdateRecentMenu()
        {
            int n = 1;

            menuItemRecent.Items.Clear();

            foreach (RecentItem item in RecentDB.GetFiles())
            {
                menuItemRecent.Items.Add(
                    new MenuItemEx()
                    {
                        Header = $"{n++} {item.FileName}",
                        Command = DXCommands.OpenProjectCommand,
                        CommandParameter = item
                    });
            }
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
            if (e.Parameter is RecentItem item)
            {
                var openedProject = Model.Projects
                    .FirstOrDefault(p => p.FileName.AnyEquals(item.FileName));

                // Open
                if (openedProject == null)
                {
                    DXProject project = item.OpenDXProject();

                    // 파일을 찾을 수 없음
                    if (project == null)
                    {
                        UpdateRecentMenu();
                        return;
                    }

                    OpenProject(project);
                    return;
                }

                // Select Tab
                SelectProject(openedProject);
            }
            else
            {
                OpenProject();
            }
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
            OpenProject(DXProject.OpenDialog());
        }

        private void OpenProject(DXProject project)
        {
            if (project == null)
                return;

            UpdateRecentMenu();

            OpenStoryboardPage(project);
        }

        public void CreateNewProject()
        {
            var projDialog = new ProjectDialog();
            
            if (projDialog.ShowDialog())
            {
                var project = DXProject.Create(
                    projDialog.FileName,
                    new DXProjectManifest()
                    {
                        ProjectName = projDialog.AppName,
                        PackageName = projDialog.PackageName
                    });
                
                OpenStoryboardPage(project);
            }
        }
        #endregion

        #region [ Storyboard Handling ]
        private void SelectProject(DXProject project)
        {
            foreach (ClosableTabItem item in tabControl.Items)
            {
                var model = item.Tag as StoryboardModel;

                if (model.Project.Equals(project))
                    item.IsSelected = true;
            }
        }

        public void OpenStoryboardPage(DXProject project)
        {
            // DeXign Project
            var page = new StoryboardPage();
            var tabItem = new ClosableTabItem()
            {
                Header = project.Manifest.ProjectName,
                IsSelected = true,
                Content = new Frame()
                {
                    Content = page
                },
                Tag = page.Model
            };

            tabItem.Closed += TabItem_Closed;

            // Add project on managed collection
            Model.Projects.SafeAdd(project);

            // Set Storyboard Project
            page.Model.Project = project;

            // Add TabPage
            tabControl.Items.Add(tabItem);
        }

        private void TabItem_Closed(object sender, EventArgs e)
        {
            var tabItem = sender as ClosableTabItem;
            var model = tabItem.Tag as StoryboardModel;

            // Remove project on managed collection
            Model.Projects.SafeRemove(model.Project);
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
