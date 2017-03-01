using System.Windows;

namespace DeXign.Extension
{
    public interface IUISupport
    {
        Point GetLocation();

        Rect GetBound();
    }
}
