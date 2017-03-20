using System;
using System.Windows;

using WPFExtension;

namespace DeXign.Core
{
    [Serializable]
    public class PObject : DependencyObject
    {
        public static readonly DependencyProperty NameProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty GuidProperty =
            DependencyHelper.Register();

        // for resources
        public Guid Guid
        {
            get { return GetValue<Guid>(GuidProperty); }
            set { SetValue(GuidProperty, value); }
        }

        [DesignElement(DisplayName = "이름", Visible = false)]
        public string Name
        {
            get { return GetValue<string>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public object Tag { get; set; }

        public PObject()
        {
            GuidProperty.AddValueChanged(this, Guid_Changed);
        }

        private void Guid_Changed(object sender, EventArgs e)
        {
            OnGuidChanged();   
        }

        protected virtual void OnGuidChanged()
        {
        }

        public T GetValue<T>(DependencyProperty dp)
        {
            return (T)GetValue(dp);
        }
    }
}