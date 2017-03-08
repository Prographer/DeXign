using System;
using System.IO;
using wf = System.Windows.Forms;

namespace DeXign.Windows.Pages
{
    public partial class NewProjectPage2 : DialogPage
    {
        public string SelectedDirectory { get; set; }

        public NewProjectPage2()
        {
            InitializeComponent();
        }

        // 찾아보기
        private void LinkTextBlock_Click(object sender, EventArgs e)
        {
            var dialog = new wf.FolderBrowserDialog();

            dialog.ShowNewFolderButton = true;

            if (dialog.ShowDialog() == wf.DialogResult.OK)
            {
                SelectedDirectory = dialog.SelectedPath;
                tbDirectory.Text = SelectedDirectory;
            }
        }

        public override bool CanOk()
        {
            return !string.IsNullOrWhiteSpace(SelectedDirectory);
        }
    }
}
