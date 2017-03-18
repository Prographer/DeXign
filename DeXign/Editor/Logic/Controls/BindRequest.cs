namespace DeXign.Editor.Logic
{
    public class BindRequest
    {
        public BindThumb Source { get; set; }

        public BindThumb Target { get; set; }

        public bool Handled { get; set; } = false;

        public BindRequest(BindThumb source)
        {
            this.Source = source;
        }
    }
}