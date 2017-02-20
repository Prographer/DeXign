using System;

namespace DeXign.Core
{
    public class DesignElementAttribute : Attribute
    {
        public string Key { get; set; }

        public bool Visible { get; set; } = true;

        public string DisplayName { get; set; }

        // 속성 타입이 Enum일경우만 적용됨
        public bool IsNotEnum { get; set; } = true;

        public string Category { get; set; }
    }
}
