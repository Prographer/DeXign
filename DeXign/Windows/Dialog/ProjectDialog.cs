using DeXign.Windows.Pages;

namespace DeXign.Windows
{
    class ProjectDialog : DialogWindow
    {
        NewProjectPage1 page1;
        NewProjectPage2 page2;

        public string AppName => page1?.AppName;
        public string Directory => page2?.SelectedDirectory;

        public ProjectDialog() : base(new DialogPage[]
        {
            new NewProjectPage1(),
            new NewProjectPage2()
        })
        {
            page1 = this.Pages[0] as NewProjectPage1;
            page2 = this.Pages[1] as NewProjectPage2;
        }
    }
}
