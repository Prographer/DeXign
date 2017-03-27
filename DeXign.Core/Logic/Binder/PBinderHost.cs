using System;
using System.Linq;
using System.Collections.Generic;

using DeXign.Core.Collections;
using System.Collections.Specialized;

namespace DeXign.Core.Logic
{
    [CSharpCodeMap("")]
    [JavaCodeMap("")]
    public class PBinderHost : PObject, IBinderHost
    {
        public event EventHandler<BinderBindedEventArgs> Binded;
        public event EventHandler<BinderBindedEventArgs> Released;

        public BinderCollection Items { get; }

        public IEnumerable<IBinder> this[BindOptions option]
        {
            get
            {
                return this.Items.Find(option);
            }
        }

        public PBinderHost()
        {
            // Unique ID
            this.Guid = Guid.NewGuid();

            // Host Collection
            this.Items = new BinderCollection(null);
            this.Items.CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (PBinder binder in e.NewItems)
                {
                    binder.Binded += Binder_Binded;
                    binder.Released += Binder_Released;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (PBinder binder in e.OldItems)
                {
                    binder.Binded -= Binder_Binded;
                    binder.Released -= Binder_Released;
                }
            }
        }

        protected override void OnGuidChanged()
        {
            base.OnGuidChanged();
            
            this.Items?.Clear();
        }

        public void AddNewBinder(BindOptions option)
        {
            AddBinder(new PBinder(this, option));
        }

        public void AddBinder(IBinder binder)
        {
            // 트리거 인풋은 한개밖에 가질 수 없음
            if (binder.BindOption == BindOptions.Input && this[binder.BindOption].Count() > 0)
                throw new Exception();
            
            this.Items.Add(binder);
        }

        public void ClearBinder(BindOptions option)
        {
            foreach (IBinder binder in this.Items.Find(option).ToArray())
            {
                binder.ReleaseAll();
                
                this.Items.Remove(binder);
            }
        }

        private void Binder_Released(object sender, IBinder e)
        {
            var resolved = ResolveExpression(sender as IBinder, e);
            var expression = new BinderExpression(resolved.Output, resolved.Input);

            Released?.Invoke(this, new BinderBindedEventArgs(expression));
        }

        private void Binder_Binded(object sender, IBinder e)
        {
            var resolved = ResolveExpression(sender as IBinder, e);
            var expression = new BinderExpression(resolved.Output, resolved.Input);

            Binded?.Invoke(this, new BinderBindedEventArgs(expression));
        }

        private (IBinder Output, IBinder Input) ResolveExpression(IBinder target, IBinder source)
        {
            if (BindDirection.Output.HasFlag((BindDirection)target.BindOption))
                return (target, source);

            return (source, target);
        }

        public void ReleaseAll()
        {
            foreach (IBinder binder in this.Items.ToArray())
            {
                binder.ReleaseAll();
            }
        }

        public IBinderHost ProvideValue()
        {
            return this;
        }

        public IEnumerable<IBinder> GetConnectableBinders(IBinder binder)
        {
            BindOptions pairOption = binder.GetPairOption();

            foreach (IBinder item in this[pairOption])
            {
                if (!EnsureCanBind(item, binder))
                    continue;

                yield return item;
            }
        }

        private bool EnsureCanBind(IBinder sourceBinder, IBinder targetBinder)
        {
            var pairBinder = sourceBinder.GetPairBinder(targetBinder);

            return CanBind(pairBinder.Output, pairBinder.Input);
        }

        protected virtual bool CanBind(IBinder outputBinder, IBinder inputBinder)
        {
            return outputBinder.CanBind(inputBinder);
        }
    }
}
