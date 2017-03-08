using System;
using System.Windows.Input;

namespace DeXign.Windows.Pages
{
    public partial class NewProjectPage1 : DialogPage
    {
        public string AppName { get; set; }
        
        public NewProjectPage1()
        {
            InitializeComponent();
            string t = tcAppName.Text;
        }

        private void AppName_TextChanged(object sender, EventArgs e)
        {
            CommandManager.InvalidateRequerySuggested();

            AppName = tcAppName.Text;
        }

        public override bool CanNext()
        {
            return !string.IsNullOrWhiteSpace(tcAppName.Text);
        }
    }
}
