using System;
using Windows.UI.Xaml.Data;

namespace Flavordex.UI
{
    /// <summary>
    /// Converter to append an arbitrary unit string to a value.
    /// </summary>
    public class ValueUnitsConverter : IValueConverter
    {
        /// <summary>
        /// Appends an arbitrary string to the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">The data type of the target property.</param>
        /// <param name="parameter">The string to append.</param>
        /// <param name="language">The language.</param>
        /// <returns>The value with the parameter appended.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter is string && value != null)
            {
                return string.Format("{0}{1}", value, parameter);
            }
            return value;
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
