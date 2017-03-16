﻿using System;
using System.Linq;

using DeXign.Core.Collections;

namespace DeXign.Core.Logic
{
    public class PBinder : PObject, IBinder
    {
        public event EventHandler<IBinder> Binded;
        public event EventHandler<IBinder> Released;

        public IBinderHost Host { get; }

        public BindOptions BindOption { get; }

        public BinderCollection Items { get; }

        public PBinder(IBinderHost host, BindOptions bindOption)
        {
            // Unique ID
            this.Guid = Guid.NewGuid();

            this.Host = host;
            this.BindOption = bindOption;

            this.Items = new BinderCollection(this);
        }

        public void PropagateBind(IBinder output, IBinder input)
        {
            OnPropagateBind(output, input);
        }

        protected virtual void OnPropagateBind(IBinder output, IBinder input)
        {
        }

        public BindDirection GetDirection()
        {
            if (BindDirection.Input.HasFlag((BindDirection)BindOption))
                return BindDirection.Input;

            return BindDirection.Output;
        }

        public virtual void Bind(IBinder targetBinder)
        {
            if (!CanBind(targetBinder))
                throw new Exception();

            this.Items.Add(targetBinder);

            if (targetBinder.CanBind(this))
                targetBinder.Bind(this);

            // Raise
            Binded?.Invoke(this, targetBinder);
        }

        public virtual bool CanBind(IBinder targetBinder)
        {
            int option = (int)(this.BindOption | targetBinder.BindOption);

            // 5: Input | Output
            // 10: Parameter | Return
            if (option != 5 && option != 10)
                return false;

            return !this.Items.Contains(targetBinder);
        }

        public virtual void Release(IBinder targetBinder)
        {
            if (this.Items.Contains(targetBinder))
            {
                this.Items.Remove(targetBinder);

                targetBinder.Release(this);

                // Raise
                Released?.Invoke(this, targetBinder);
            }
        }

        public virtual void ReleaseAll()
        {
            foreach (IBinder item in Items.ToArray())
            { 
                Release(item);
            }
        }

        public IBinderHost ProvideValue()
        {
            return this.Host;
        }
    }
}
