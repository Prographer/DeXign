using System;

namespace DeXign.Core
{
    public class DesignElementAttribute : Attribute
    {
        public bool Visible { get; set; } = true;

        public string DisplayName { get; set; }

        // �Ӽ� Ÿ���� Enum�ϰ�츸 �����
        public bool IsNotEnum { get; set; } = true;

        public string Category { get; set; }
    }
}
