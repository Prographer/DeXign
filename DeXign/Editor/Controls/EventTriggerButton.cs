using System.Windows.Controls;

using DeXign.Editor.Layer;

namespace DeXign.Editor.Controls
{
    class EventTriggerButton : Button
    {
        public SelectionLayer ParentLayer { get; set; }

        public EventTriggerButton(SelectionLayer parentLayer)
        {
            this.ParentLayer = parentLayer;
        }

        protected override void OnClick()
        {
            // TODO: Open Component Popup
        }
    }
}
