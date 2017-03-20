namespace DeXign.Converter
{
    public class BoolToObjectConverter : BaseValueConverter<bool, object>
    {
        public object TrueValue { get; set; }
        public object FalseValue { get; set; }

        public bool IsNot { get; set; }

        public override object Convert(bool value, object parameter)
        {
            return value ^ IsNot ? TrueValue : FalseValue;
        }

        public override bool ConvertBack(object value, object parameter)
        {
            return value == TrueValue ^ IsNot;
        }
    }
}
