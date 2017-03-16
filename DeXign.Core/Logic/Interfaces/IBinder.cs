using System;
using DeXign.Core.Collections;
using System.Collections;

namespace DeXign.Core.Logic
{
    public interface IBinder : IBinderHostProvider
    {
        event EventHandler<IBinder> Binded;
        event EventHandler<IBinder> Released;

        IBinderHost Host { get; }

        BindOptions BindOption { get; }

        BinderCollection Items { get; }

        bool CanBind(IBinder targetBinder);
        void Bind(IBinder targetBinder);
        void Release(IBinder targetBinder);
        void ReleaseAll();
    }
}
