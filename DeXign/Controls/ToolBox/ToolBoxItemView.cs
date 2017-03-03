using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

using DeXign.Models;

namespace DeXign.Controls
{
    class ToolBoxItemView : ListViewItem
    {
        public ToolBoxItemModel Model => (ToolBoxItemModel)DataContext;

        public string Category => Model.Category;

        private Point beginPosition;

        public ToolBoxItemView(ToolBoxItemModel model)
        {
            this.DataContext = model;
        }
        
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            
            beginPosition = e.GetPosition(this);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point position = e.GetPosition(this);
                Size dragSize = new Size(
                    SystemParameters.MinimumHorizontalDragDistance,
                    SystemParameters.MinimumVerticalDragDistance);

                if (Math.Abs(position.X - this.beginPosition.X) > dragSize.Width / 2 ||
                    Math.Abs(position.Y - this.beginPosition.Y) > dragSize.Height / 2)
                {
                    try
                    {
                        DragDrop.DoDragDrop(this, Model.Metadata, DragDropEffects.None);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\r\n\r\n" + ex.StackTrace);
                    }
                    Selector.SetIsSelected(this, false);
                }
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            Selector.SetIsSelected(this, false);
        }

        protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            e.Handled = true;
        }
    }
}
