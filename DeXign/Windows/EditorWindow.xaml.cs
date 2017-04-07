using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.ComponentModel;
using System.CodeDom.Compiler;
using t = System.Threading.Tasks;

using DeXign.IO;
using DeXign.Controls;
using DeXign.Core.Designer;
using DeXign.Models;
using DeXign.Resources;
using DeXign.Windows.Pages;
using DeXign.Editor;
using DeXign.Database;
using DeXign.Extension;
using DeXign.Core.Logic;
using DeXign.Editor.Renderer;
using DeXign.Core.Compiler;
using DeXign.Core.Controls;

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
                .Select(r => (DependencyObject)(r as IRenderer).Model)
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

            this.CommandBindings.Add(
                new CommandBinding(
                    DXCommands.RunDebugCommand, RunDebug_Execute));

            this.CommandBindings.Add(
                new CommandBinding(
                    DXCommands.StopDebugCommand, StopDebug_Execute));
        }
        #endregion

        #region [ Commands ]
        private async void StopDebug_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            await DXDebugger.Stop();

            // TODO: Stop
            GlobalModel.Instance.IsDebugging = false;
        }

        private async void RunDebug_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            // TODO: Compile
            var proj = this.Model.SelectedProject;

            if (proj == null)
            {
                MessageBox.Show("대상 프로젝트를 찾을 수 없습니다.", "DeXign", MessageBoxButton.OK);
                return;
            }

            GlobalModel.Instance.IsDebugging = true;
            GlobalModel.Instance.CompileProgress = 0;

            messagePanel.Show();

            await t.Task.Delay(300);

            // 저장
            proj.Save();
            
            // 컴파일 옵션
            var dxCompileOption = new DXCompileOption()
            {
                ApplicationName = proj.Manifest.ProjectName,
                RootNamespace = proj.Manifest.PackageName,
                TargetPlatform = this.Model.StoryboardPage.Model.SelectedPlatform,
                Directory = $"{Path.GetDirectoryName(proj.FileName)}"
            };

            PContentPage[] screens = proj.Screens.ToArray();
            PBinderHost[] binderHosts = screens
                .Select(s => s.GetRenderer())                   // PContentPage -> IRenderer
                .SelectMany(r => r.FindChildrens<IRenderer>())  // 모든 렌더러 자식 (하위 포함)
                .Where(r => r.ProvideValue().Items.Sum(b => b.Items.Count) > 0)   // 연결된 아이템들
                .Select(r => r.ProvideValue() as PBinderHost)   // BinderHost 선택
                .ToArray();

            BaseCompilerService service =
                DXCompiler.GetCompilerService(dxCompileOption.TargetPlatform).FirstOrDefault();
            
            // 컴파일 프로그레스 등록
            service.ProgressChanged += Compile_ProgressChanged;
            
            DXCompileResult result = await DXCompiler.Compile(
                new DXCompileParameter(dxCompileOption, screens, binderHosts));

            // 컴파일 프로그레스 등록해제
            service.ProgressChanged -= Compile_ProgressChanged;

            messagePanel.Hide();
            
            if (result.IsSuccess)
            {
                await OnCompileSuccess(result);
            }
            else
            {
                OnCompileError(result);
            }
        }

        private void Compile_ProgressChanged(object sender, double e)
        {
            GlobalModel.Instance.CompileProgress = e;
        }

        private async t.Task OnCompileSuccess(DXCompileResult result)
        {
            switch (result.Option.TargetPlatform)
            {
                case Platform.Window:
                    await DXDebugger.RunWinApplication(result.Outputs[0]);
                    break;

                case Platform.XForms:
                    break;
            }

            DXCommands.StopDebugCommand.Execute(null, null);
        }

        private void OnCompileError(DXCompileResult result)
        {
            var sb = new StringBuilder();

            foreach (object error in result.Errors)
            {
                if (error is CompilerError cErr)
                {
                    sb.AppendLine(cErr.ErrorText);
                }
                else if (error is Exception ex)
                {
                    sb.AppendLine($"Exception: {ex.Message}");
                }
            }
            
            MessageBox.Show("컴파일 에러: \r\n" + sb.ToString(), "DeXign", MessageBoxButton.OK, MessageBoxImage.Exclamation);

            GlobalModel.Instance.IsDebugging = false;
        }

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
            this.Model.SelectedProject?.Save();
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

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (GlobalModel.Instance.IsDebugging)
            {
                var r = MessageBox.Show("디버깅을 중지하시겠습니까?", "DeXign", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

                e.Cancel = true;

                if (r == MessageBoxResult.Yes)
                {
                    DXCommands.StopDebugCommand.Execute(null, null);
                }
            }
        }
    }
}