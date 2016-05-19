using System;
using Windows.UI.Xaml.Data;

namespace Flavordex.UI
{
    /// <summary>
    /// Converter to translate between long and double values.
    /// </summary>
    public class LongToDoubleConverter : IValueConverter
    {
        /// <summary>
        /// Converts a long value to a double value.
        /// </summary>
        /// <param name="value">The integer value.</param>
        /// <param name="targetType">The data type of the target property.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>The value as a double.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value is long ? double.Parse(value.ToString()) : 0d;
        }

        /// <summary>
        /// Converts a double value to a long value.
        /// </summary>
        /// <param name="value">The double value.</param>
        /// <param name="targetType">The data type of the target property.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>The value as an integer.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value is double ? long.Parse(value.ToString()) : 0;
        }
    }
}
