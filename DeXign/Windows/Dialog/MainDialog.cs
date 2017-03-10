using System;
using DeXign.Windows.Pages;
using DeXign.IO;
using System.IO;

namespace DeXign.Windows
{
    class MainDialog : DialogWindow
    {
        MainPage mainPage;
        ProjectPage1 projectPage1;
        ProjectPage2 projectPage2;

        public DXProject Project { get; private set; }

        public MainDialog() : base(
            new DialogPage[]
            {
                new MainPage(),
                new ProjectPage1(),
                new ProjectPage2()
            })
        {
            this.mainPage = Pages[0] as MainPage;
            this.projectPage1 = Pages[1] as ProjectPage1;
            this.projectPage2 = Pages[2] as ProjectPage2;
        }

        protected override void OnClosed(EventArgs e)
        {
            this.Project = mainPage.Project;

            if (this.Project == null)
            {
                string fileName = Path.Combine(projectPage2.SelectedDirectory, $"{projectPage1.AppName}.dx");

                this.Project = DXProject.Create(
                    fileName,
                    new DXProjectManifest()
                    {
                        ProjectName = projectPage1.AppName,
                        PackageName = projectPage1.PackageName
                    });
            }

            base.OnClosed(e);
        }
    }
}