using DeXign.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace DeXign.Converter
{
    class GridViewColumnStarConverter : BaseValueConverter<PropertyGrid, double>
    {
        public override double Convert(PropertyGrid value, object parameter)
        {
            double width = value.Width;
            GridView gv = value.View as GridView;

            for (int i = 0; i < gv.Columns.Count; i++)
            {
                if (!Double.IsNaN(gv.Columns[i].Width))
                    width -= gv.Columns[i].Width;
            }

            return width - 5;// this is to take care of margin/padding
        }

        public override PropertyGrid ConvertBack(double value, object parameter)
        {
            throw new NotImplementedException();
        }
    }
}
