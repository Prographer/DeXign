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
        
        public double StarWidth
        {
            get { return (double)GetValue(StarWidthProperty); }
            set { SetValue(StarWidthProperty, value); }
        }
        
        public StarGridViewColumn()
        {
            StarWidthProperty.AddValueChanged(this, StarWidthChanged);
        }

        private void StarWidthChanged(object sender, EventArgs e)
        { 
        }
    }
}
