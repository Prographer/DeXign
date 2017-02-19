using DeXign.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WPFExtension;

namespace DeXign.Controls
{
    class StarGridViewColumn : GridViewColumn
    {
        public static readonly DependencyProperty StarWidthProperty =
            DependencyHelper.Register();

        public static readonly DependencyProperty TargetProperty =
            DependencyHelper.Register();

        public double StarWidth
        {
            get { return (double)GetValue(StarWidthProperty); }
            set { SetValue(StarWidthProperty, value); }
        }

        public GridView Target
        {
            get { return (GridView)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        public StarGridViewColumn()
        {
            StarWidthProperty.AddValueChanged(this, StarWidthChanged);
            TargetProperty.AddValueChanged(this, TargetChanged);
        }

        private void TargetChanged(object sender, EventArgs e)
        {
            var t = Target;
        }

        private void StarWidthChanged(object sender, EventArgs e)
        {
            if (Target == null)
                return;

            
        }
    }
}
