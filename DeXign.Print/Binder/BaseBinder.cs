using DeXign.Core.Controls;
using System;
using DeXign.Logic.Collections;

namespace DeXign.Logic.Binder
{
    class BaseBinder : PControl, IBinder
    {
        public event EventHandler<BinderBindedEventArgs> Binded;
        public event EventHandler<BinderReleasedEventArgs> Released;

        public BinderCollection Inputs { get; }

        public BinderCollection Outputs { get; }

        public BinderCollection Parameters { get; }

        public BaseBinder()
        {
            Inputs = new BinderCollection();
            Outputs = new BinderCollection();
            Parameters = new BinderCollection();
        }

        public void Bind(IBinder outputBinder, BinderOptions options)
        {
            throw new NotImplementedException();
        }

        public bool CanBind(IBinder outputBinder, BinderOptions options)
        {
            throw new NotImplementedException();
        }

        public void ReleaseAll()
        {
            throw new NotImplementedException();
        }

        public void ReleaseInput(IBinder outputBinder)
        {
            throw new NotImplementedException();
        }

        public void ReleaseOutput(IBinder inputBinder)
        {
            throw new NotImplementedException();
        }

        public void SetProvider(IBinderProvider provider)
        {
            throw new NotImplementedException();
        }
    }
}
