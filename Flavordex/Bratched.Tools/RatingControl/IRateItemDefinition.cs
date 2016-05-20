using Windows.UI.Xaml.Media;

namespace Bratched.Tools.RatingControl
{
    public interface IRateItemDefinition
    {
        SolidColorBrush BackgroundColor { get; set; }
        SolidColorBrush OutlineColor { get; set; }
        object OutlineThickness { get; set; }
        object PathData { get; set; }
    }
}
