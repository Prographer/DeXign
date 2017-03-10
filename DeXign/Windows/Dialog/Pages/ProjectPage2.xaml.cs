using System;

using wf = System.Windows.Forms;

namespace DeXign.Windows.Pages
{
    public partial class ProjectPage2 : DialogPage
    {
        public string SelectedDirectory { get; set; }

        public ProjectPage2()
        {
            InitializeComponent();

            SetSelectedDirectory(Environment.CurrentDirectory);
        }

        // 찾아보기
        private void LinkTextBlock_Click(object sender, EventArgs e)
        {
            var dialog = new wf.FolderBrowserDialog();

            dialog.ShowNewFolderButton = true;
            dialog.SelectedPath = SelectedDirectory;

            if (dialog.ShowDialog() == wf.DialogResult.OK)
                SetSelectedDirectory(dialog.SelectedPath);
        }

        private void SetSelectedDirectory(string selectedPath)
        {
            SelectedDirectory = selectedPath;
            tbDirectory.Text = SelectedDirectory;
        }

        public override bool CanOk()
        {
            return !string.IsNullOrWhiteSpace(SelectedDirectory);
        }
    }
}
