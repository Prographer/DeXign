using System;
using DeXign.Core.Collections;
using System.Collections.Generic;

namespace DeXign.Core.Logic
{
    public interface IBinderHost : IBinderHostProvider
    {
        event EventHandler<BinderBindedEventArgs> Binded;
        event EventHandler<BinderBindedEventArgs> Released;

        IEnumerable<IBinder> this[BindOptions option] { get; }

        BinderCollection Items { get; }

        void ReleaseAll();
    }
}
