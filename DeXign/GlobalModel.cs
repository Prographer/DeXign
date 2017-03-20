using DeXign.Models;

namespace DeXign
{
    // Single tone
    public class GlobalModel : BaseNotifyModel
    {
        public static GlobalModel Instance { get; }

        private bool _isDebugging = false;
        public bool IsDebugging
        {
            get { return _isDebugging; }
            set { _isDebugging = value; RaisePropertyChanged(); }
        }

        static GlobalModel()
        {
            if (GlobalModel.Instance == null)
                GlobalModel.Instance = new GlobalModel();
        }
    }
}