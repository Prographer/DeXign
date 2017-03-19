namespace DeXign.Converter
{
    public class FallbackStringConverter : BaseValueConverter<string, string>
    {
        public string FallbackValue { get; set; }

        public override string Convert(string value, object parameter)
        {
            return string.IsNullOrEmpty(value) ? FallbackValue : value;
        }

        public override string ConvertBack(string value, object parameter)
        {
            return value;
        }
    }
}
