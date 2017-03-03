using DeXign.Commands;
using DeXign.Windows.Pages;
using DeXign.Task;
using System;

namespace DeXign.Models
{
    public class StoryboardModel : IModel
    {
        public StoryboardPage ViewModel { get; }

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
