using System;

namespace DeXign.Core.Logic
{
    public class BinderReleasedEventArgs : EventArgs
    {
        public IBinder ReleasedItem { get; }
        public BinderOptions BinderOptions { get; }

        public BinderReleasedEventArgs(IBinder item, BinderOptions options)
        {
            this.ReleasedItem = item;
            this.BinderOptions = options;
        }
    }
}
