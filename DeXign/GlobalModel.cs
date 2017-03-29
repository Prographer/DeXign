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

        private double _compileProgress = 0;
        public double CompileProgress
        {
            get { return _compileProgress; }
            set { _compileProgress = value; RaisePropertyChanged(); }
        }

        static GlobalModel()
        {
            if (GlobalModel.Instance == null)
                GlobalModel.Instance = new GlobalModel();
        }
    }
}