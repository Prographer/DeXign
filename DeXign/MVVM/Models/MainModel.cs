using DeXign.IO;
using DeXign.Windows.Pages;
using System.Collections.Generic;

namespace DeXign.Models
{
    public class MainModel : BaseNotifyModel
    {
        private StoryboardPage _storyboardPage;
       
        public StoryboardPage StoryboardPage
        {
            get { return _storyboardPage; }
            set
            {
                _storyboardPage = value;
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
