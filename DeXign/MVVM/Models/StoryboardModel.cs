using DeXign.Commands;
using DeXign.Windows.Pages;
using DeXign.Task;

namespace DeXign.Models
{
    public class StoryboardModel : IModel
    {
        public StoryboardPage ViewModel { get; }

        public TaskManager TaskManager { get; }

        public ActionCommand PlatformCommand { get; set; }
        
        public StoryboardModel()
        {
            PlatformCommand = new ActionCommand();
            TaskManager = new TaskManager();
        }

        public StoryboardModel(StoryboardPage viewModel) : this()
        {
            this.ViewModel = viewModel;
        }
    }
}
