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
using System.Windows;
using System.IO;
using System.Windows.Media;
using DeXign.Core.Logic;
using DeXign.Editor.Renderer;

namespace DeXign.Windows
{
    public partial class EditorWindow : ChromeWindow, IViewModel<MainModel>
    {
        #region [ Property ]
        public MainModel Model { get; set; }
        #endregion

        #region [ Constructor ]
        public EditorWindow()
        {
            InitializeComponent();
            InitializeCommands();
            InitializeLayouts();

            UpdateRecentMenu();

            Model = new MainModel();
            this.DataContext = Model;

            GroupSelector.SelectedItemChanged += GroupSelector_SelectedItemChanged;
        }

        public EditorWindow(DXProject project) : this()
        {
            // Load
            OpenStoryboardPage(project);
        }

        private void UpdateRecentMenu()
        {
            menuItemRecent.Items.Clear();

            foreach (RecentItem item in RecentDB.GetFiles())
            {
                menuItemRecent.Items.Add(
                    new MenuItemEx()
                    {
                        Header = $"{menuItemRecent.Items.Count + 1} {item.FileName}",
                        Command = DXCommands.OpenProjectCommand,
                        CommandParameter = item
                    });
            }
        }

        private void GroupSelector_SelectedItemChanged(object sender, EventArgs e)
        {
            propertyGrid.SelectedObjects = GroupSelector.GetSelectedItems()
                .Where(obj => obj is IRenderer)
                .Select(r => (object)(r as IRenderer).Model)
                .ToArray();
        }

        private void InitializeLayouts()
        {
            // ToolBox
            foreach (var element in DesignerManager.GetElementTypes()
                .OrderBy(data => data.Attribute.Category)
                .OrderBy(data => data.Element.CanCastingTo<PComponent>() ? 1 : 0))
            {
                if (!element.Attribute.Visible)
                    continue;

                var resource = ResourceManager.GetDesignerResource(element.Element);

                toolBox.AddItem(
                    new ToolBoxItemView(
                        new ToolBoxItemModel(element, resource)));
            }

            foreach (var function in SDKManager.GetFunctions())
            {
                var resource = new DesignerResource()
                {
                    Content = new Image()
                    {
                        Source = ResourceManager.GetImageSource("Icons/IconFunction.png")
                    }
                };
                
                toolBox.AddItem(
                    new ToolBoxItemView(
                        new ToolBoxItemFunctionModel(
                            function.Element,
                            new AttributeTuple<Core.DesignElementAttribute, Type>(function.Attribute, typeof(PFunction)),
                            resource)));
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

            this.CommandBindings.Add(
                new CommandBinding(
                    DXCommands.SearchCommand, Search_Execute));
        }
        #endregion

        #region [ Commands ]
        private void Search_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            toolBoxSearchBar.Focus();
        }

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
        private bool SelectProject(string projectFileName)
        {
            foreach (ClosableTabItem item in tabControl.Items)
            {
                if (item.Tag is StoryboardModel model)
                {
                    if (model.Project.FileName.AnyEquals(projectFileName))
                    {
                        item.IsSelected = true;
                        return true;
                    }
                }
            }

            return false;
        }

        private bool SelectProject(DXProject project)
        {
            return SelectProject(project.FileName);
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

            // Update Recent
            UpdateRecentMenu();
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

        private void Rectangle_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {

        }
    }
}