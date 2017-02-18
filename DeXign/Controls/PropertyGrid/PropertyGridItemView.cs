using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeXign.Core;
using DeXign.Core.Designer;
using System.Windows.Controls;

namespace DeXign.Controls
{
    internal class PropertyGridItemView : ListViewItem
    {
    //    public ToolBoxItemModel Model => (ToolBoxItemModel)DataContext;

    //    public string Category => Model.Category;

    //    private Point beginPosition;

    //    public ToolBoxItemView(ToolBoxItemModel item)
    //    {
    //        this.DataContext = item;
    //    }

    //    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    //    {
    //        base.OnMouseLeftButtonDown(e);

    //        beginPosition = e.GetPosition(this);
    //    }

    //    protected override void OnMouseMove(MouseEventArgs e)
    //    {
    //        base.OnMouseMove(e);

    //        if (e.LeftButton == MouseButtonState.Pressed)
    //        {
    //            Point position = e.GetPosition(this);
    //            Size dragSize = new Size(
    //                SystemParameters.MinimumHorizontalDragDistance,
    //                SystemParameters.MinimumVerticalDragDistance);

    //            if (Math.Abs(position.X - this.beginPosition.X) > dragSize.Width / 2 ||
    //                Math.Abs(position.Y - this.beginPosition.Y) > dragSize.Height / 2)
    //            {
    //                DragDrop.DoDragDrop(this, Model.Metadata, DragDropEffects.None);
    //                Selector.SetIsSelected(this, false);
    //            }
    //        }
    //    }

    //    protected override void OnMouseUp(MouseButtonEventArgs e)
    //    {
    //        base.OnMouseUp(e);
    //        Selector.SetIsSelected(this, false);
    //    }

    //    protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
    //    {
    //        e.Handled = true;
    //    }
    }
}
