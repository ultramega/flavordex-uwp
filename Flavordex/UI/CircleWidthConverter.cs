using System;
using Windows.UI.Xaml.Data;

namespace Flavordex.UI
{
    /// <summary>
    /// Converter used by the Ellipses to calculate their height and width.
    /// </summary>
    internal class CircleWidthConverter : IValueConverter
    {
        /// <summary>
        /// Converts a given circle radius to the width and height multiplied by the given
        /// parameter value.
        /// </summary>
        /// <param name="value">The radius as a double.</param>
        /// <param name="targetType">The data type of the target property.</param>
        /// <param name="parameter">The parameter as a string.</param>
        /// <param name="language">The language.</param>
        /// <returns>The double value to use as the width or height of the Ellipse.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (double)value * double.Parse(parameter as string) * 2;
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">The data type of the target property.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>The result of the conversion.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
