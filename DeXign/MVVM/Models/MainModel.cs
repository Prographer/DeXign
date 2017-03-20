using DeXign.IO;
using DeXign.Windows.Pages;
using System.Collections.Generic;

namespace DeXign.Models
{
    public class MainModel : BaseNotifyModel
    {
        private StoryboardPage storyboardPage;
       
        public StoryboardPage StoryboardPage
        {
            get { return storyboardPage; }
            set
            {
                storyboardPage = value;
                
                this.SelectedProject = value != null ? value.Model.Project : null;

                RaisePropertyChanged();
            }
        }

        private DXProject selectedProject;
        public DXProject SelectedProject
        {
            get
            {
                return selectedProject;
            }
            private set
            {
                selectedProject = value;

                RaisePropertyChanged();
            }
        }

        public List<DXProject> Projects { get; }

        public MainModel()
        {
            Projects = new List<DXProject>();
        }
    }
}
