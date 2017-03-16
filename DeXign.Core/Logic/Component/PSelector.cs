using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeXign.Core.Logic
{
    [DesignElement(Category = Constants.Logic.Default, DisplayName = "", Visible = false)]
    public class PSelector : PComponent
    {
        public PSelector()
        {
            this.AddNewBinder(BindOptions.Output);
        }
    }
}
