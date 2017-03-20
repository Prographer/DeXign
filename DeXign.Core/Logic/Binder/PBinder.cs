using System;
using System.Linq;
using System.ComponentModel;

using DeXign.Core.Collections;

namespace DeXign.Core.Logic
{
    public class PBinder : PObject, IBinder
    {
        public event EventHandler<IBinder> Binded;
        public event EventHandler<IBinder> Released;

        public IBinderHost Host { get; set; }

        public BindOptions BindOption { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BinderCollection Items { get; }

        public bool IsSingle { get; set; }

        public PBinder()
        {
            this.Items = new BinderCollection(this);
        }

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
            BindOptions option = (this.BindOption | targetBinder.BindOption);
            
            if (IsValidCombinedOption(option))
                return false;

            if (IsCirculating(targetBinder))
                return false;

            return IsSingle ? 
                Items.Count == 0 : 
                !this.Items.Contains(targetBinder);
        }

        private bool IsValidCombinedOption(BindOptions option)
        {
            int value = (int)option;

            // 5: Input | Output
            // 10: Parameter | Return

            return value != 5 && value != 10;
        }

        private bool IsCirculating(IBinder targetBinder)
        {
            BindOptions option = BindOptions.Output | BindOptions.Input;

            if (targetBinder.BindOption == BindOptions.Parameter || targetBinder.BindOption == BindOptions.Return)
                return false;

            return BinderHelper.FindHostNodes(this.Host as PBinderHost, option)
                .Contains(targetBinder.Host);
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
