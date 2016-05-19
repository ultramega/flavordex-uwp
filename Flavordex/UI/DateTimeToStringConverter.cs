using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace Flavordex.UI
{
    /// <summary>
    /// Converter to format DateTimes as strings.
    /// </summary>
    public class DateTimeToStringConverter : IValueConverter
    {
        /// <summary>
        /// The format string for dates.
        /// </summary>
        private static readonly string _dateFormat = ResourceLoader.GetForCurrentView().GetString("DateFormat");

        /// <summary>
        /// The format string for dates with time.
        /// </summary>
        private static readonly string _dateTimeFormat = ResourceLoader.GetForCurrentView().GetString("DateTimeFormat");

        /// <summary>
        /// Converts a DateTime to a formatted string.
        /// </summary>
        /// <param name="value">The DateTime.</param>
        /// <param name="targetType">The data type of the target property.</param>
        /// <param name="parameter">"Long" to include the time.</param>
        /// <param name="language">The language.</param>
        /// <returns>The date as a formatted string.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var format = parameter is string && (parameter as string).ToLower().Equals("long") ? _dateTimeFormat : _dateFormat;
            return ((DateTime)value).ToString(format);
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
