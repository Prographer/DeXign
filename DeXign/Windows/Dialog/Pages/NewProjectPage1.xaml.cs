using System;
using System.Windows;
using System.Windows.Input;

namespace DeXign.Windows.Pages
{
    public partial class NewProjectPage1 : DialogPage
    {
        public string AppName { get; set; }
        public string PackageName { get; set; }

        public NewProjectPage1()
        {
            InitializeComponent();
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            var o = e.OriginalSource;

            base.OnKeyDown(e);
        }
        private void TextCell_TextChanged(object sender, EventArgs e)
        {
            AppName = tcAppName.Text;
            PackageName = tcPackage.Text;

            CommandManager.InvalidateRequerySuggested();
        }

        public override bool CanNext()
        {
            return !string.IsNullOrWhiteSpace(AppName) && !string.IsNullOrWhiteSpace(PackageName);
        }
    }
}
