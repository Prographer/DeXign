using DeXign.Windows.Pages;

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
    }
}
