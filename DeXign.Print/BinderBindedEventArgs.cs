using System;

namespace DeXign.Logic
{
    public class BinderBindedEventArgs : EventArgs
    {
        public IBinder BindedItem { get; }
        public BinderOptions BinderOptions { get; }

        public BinderBindedEventArgs(IBinder item, BinderOptions options)
        {
            this.BindedItem = item;
            this.BinderOptions = options;
        }
    }
}