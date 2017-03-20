namespace DeXign
{
    public enum Platform
    {
        /// <summary>
        /// 안드로이드
        /// </summary>
        Android = 1,

        /// <summary>
        /// iOS
        /// </summary>
        iOS = 2,

        /// <summary>
        /// 윈도우 응용 프로그램
        /// </summary>
        Window = 4,

        /// <summary>
        /// Xamarin Forms
        /// </summary>
        XForms = Android | iOS
    }
}
