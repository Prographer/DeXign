namespace DeXign
{
    public enum Platform
    {
        /// <summary>
        /// �ȵ���̵�
        /// </summary>
        Android = 1,

        /// <summary>
        /// iOS
        /// </summary>
        iOS = 2,

        /// <summary>
        /// ������ ���� ���α׷�
        /// </summary>
        Window = 4,

        /// <summary>
        /// Xamarin Forms
        /// </summary>
        XForms = Android | iOS
    }
}
