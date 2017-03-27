namespace DeXign.Core
{
    public enum ComparisonPredicate
    {
        [WPF("==")]
        [XForms("==")]
        [DesignElement(DisplayName = "값1 == 값2")]
        Equal,

        [WPF("!=")]
        [XForms("!=")]
        [DesignElement(DisplayName = "값1 != 값2")]
        Unequal,

        [WPF("<")]
        [XForms("<")]
        [DesignElement(DisplayName = "값1 < 값2")]
        LessThan,

        [WPF("<=")]
        [XForms("<=")]
        [DesignElement(DisplayName = "값1 <= 값2")]
        LessThanOrEqualTo,

        [WPF(">")]
        [XForms(">")]
        [DesignElement(DisplayName = "값1 > 값2")]
        GreaterThan,

        [WPF(">=")]
        [XForms(">=")]
        [DesignElement(DisplayName = "값1 >= 값2")]
        GreaterThanOrEqualTo,

        [WPF(".Contains({Line:2})")]
        [XForms(".Contains({Line:2})")]
        [DesignElement(DisplayName = @"값1 ⊃ 값2")]
        Contains
    }
}
