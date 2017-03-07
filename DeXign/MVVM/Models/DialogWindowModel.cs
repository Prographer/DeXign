using System.Windows.Controls;

namespace DeXign.Models
{
    public class DialogWindowModel : BaseNotifyModel
    {
        private Page page;
        public Page Page
        {
            get { return page; }
            set
            {
                page = value;
                RaisePropertyChanged();
            }
        }
    }
}
