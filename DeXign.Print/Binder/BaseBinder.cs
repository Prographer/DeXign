using DeXign.Core.Controls;
using System;
using DeXign.Logic.Collections;

namespace DeXign.Logic.Binder
{
    class BaseBinder : PControl, IBinder
    {
        public BinderCollection Inputs
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public BinderCollection Outputs
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public BinderCollection Parameters
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public event EventHandler<BinderBindedEventArgs> Binded;
        public event EventHandler<BinderReleasedEventArgs> Released;

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
