using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace DeXign.Editor.Layer
{
    class GuideLayer : StoryboardLayer, IEnumerable<StoryboardLayer>
    {
        public List<StoryboardLayer> Items { get; }

        public GuideLayer(UIElement element) : base(element)
        {
            Items = new List<StoryboardLayer>();
        }

        public void Add(StoryboardLayer layer)
        {
            Items.Add(layer);
        }

        public void Remove(StoryboardLayer layer)
        {
            Items.Remove(layer);
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            
            // TODO
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        public IEnumerator<StoryboardLayer> GetEnumerator()
        {
            return Items.GetEnumerator();
        }
    }
}
