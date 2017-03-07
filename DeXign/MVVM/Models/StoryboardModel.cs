using DeXign.Commands;
using DeXign.Windows.Pages;
using DeXign.IO;
using DeXign.Task;

namespace DeXign.Models
{
    public class StoryboardModel : IModel
    {
        public StoryboardPage ViewModel { get; }

        public DXProject Project { get; set; }

        public DispatcherTaskManager TaskManager { get; }

        public ActionCommand PlatformCommand { get; set; }
        
        public StoryboardModel()
        {
            PlatformCommand = new ActionCommand();
            TaskManager = new DispatcherTaskManager();
        }

        public StoryboardModel(StoryboardPage viewModel) : this()
        {
            this.ViewModel = viewModel;
        }
    }
}
