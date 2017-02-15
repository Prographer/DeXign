using System;
using DeXign.Commands;
using DeXign.Windows.Pages;

namespace DeXign.Models
{
    public class StoryboardModel : IModel
    {
        public StoryboardPage ViewModel { get; }

        public ActionCommand PlatformCommand { get; set; }
        
        public StoryboardModel()
        {
            PlatformCommand = new ActionCommand();
        }

        public StoryboardModel(StoryboardPage viewModel) : this()
        {
            this.ViewModel = viewModel;
        }
    }
}
