using Windows.UI.Xaml.Media;

namespace Bratched.Tools.RatingControl
{
    public sealed class RateItemDefinitionModel : IRateItemDefinition
    {
        public SolidColorBrush BackgroundColor { get; set; }
        public SolidColorBrush OutlineColor { get; set; }
        public object OutlineThickness { get; set; }
        public object PathData { get; set; }

    }
}
