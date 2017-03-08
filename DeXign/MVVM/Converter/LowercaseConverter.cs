namespace DeXign.Converter
{
    class LowercaseConverter : BaseValueConverter<string, string>
    {
        public override string Convert(string value, object parameter)
        {
            return value?.ToLower();
        }

        public override string ConvertBack(string value, object parameter)
        {
            return value;
        }
    }
}
