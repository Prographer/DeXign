using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DeXign.Designer.Layer;
using DeXign.Extension;

namespace DeXign.Designer.Controls
{
    // TODO: 스토리 보드 구현해야함 할게 짱 많네
    class Storyboard : Canvas
    {
        public GuideLayer GuideLayer { get; }

        public Storyboard()
        {
            GuideLayer = new GuideLayer(this);

            AttachedAdorner.SetAdornerType(this, typeof(GuideLayer));
            AttachedAdorner.SetAdorner(this, GuideLayer);
            AttachedAdorner.SetAdornerIndex(this, 2);

            this.InputBindings.Add(
                new KeyBinding()
                {
                    Key = Key.Escape,
                    Command = DXCommands.ESCCommand
                });

            this.CommandBindings.Add(
                new CommandBinding(DXCommands.ESCCommand, ESC_Execute));
        }

        private void ESC_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var items = GroupSelector.GetSelectedItems();

            if (items.Count() == 1)
            {
                object item = items.First();

                if (item is SelectionLayer)
                {
                    var layer = item as SelectionLayer;
                    var prevLayer = layer.AdornedElement
                        .FindParents<FrameworkElement>()
                        .Select(AttachedAdorner.GetAdorner)
                        .Skip(1)
                        .FirstOrDefault(adorner=> adorner != null && adorner is SelectionLayer);

                    if (prevLayer != null)
                        GroupSelector.Select(prevLayer, true);
                    else
                        GroupSelector.UnselectAll();
                }
            }
            else if (items.Count() > 1)
            {
                GroupSelector.UnselectAll();
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            GroupSelector.UnselectAll();
        }
    }
}
