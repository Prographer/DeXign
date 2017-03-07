using System;
using System.Windows;
using System.Windows.Input;

using DeXign.IO;
using DeXign.Controls;

namespace DeXign.Windows
{
    public partial class MainWindow : ChromeWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeCommands();
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
            // TODO: Create New Project
            this.Hide();
            ShowEditorWindow(new EditorWindow());
        }

        private void OpenProject_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            DXProject project;
            string fileName = e.Parameter?.ToString();

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                project = DXProject.Open(fileName);

                if (!project.CanOpen)
                {
                    // 메박 커스텀하고 내용 바꿀..
                    MessageBox.Show("어디 나사하나 빠진 파일 같습니다.");
                    return;
                }
            }
            else
            {
                project = DXProject.OpenDialog();
            }

            if (project == null)
                return;

            // TODO: Open Project
        }

        private void ShowEditorWindow(EditorWindow window)
        {
            window.Closed += Window_Closed;
            window.Show();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
