namespace DeXign.Converter
{
    public class NotConverter : BaseValueConverter<bool, bool>
    {
        public override bool Convert(bool value, object parameter)
        {
            return !value;
        }

        public override bool ConvertBack(bool value, object parameter)
        {
            return !value;
        }
    }
}
