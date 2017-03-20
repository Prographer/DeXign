using System;

namespace DeXign.Editor
{
    public class ItemDropRequest
    {
        public Type ItemType { get; set; }

        public object Data { get; set; }

        public ItemDropRequest(Type itemType)
        {
            this.ItemType = itemType;
        }
    }
}
