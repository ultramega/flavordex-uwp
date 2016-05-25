using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Flavordex.UI
{
    /// <summary>
    /// Converter used to translate between boolean and Visibility values.
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts between a boolean value to a Visibility value.
        /// </summary>
        /// <param name="value">The input value.</param>
        /// <param name="targetType">The data type of the target property.</param>
        /// <param name="parameter">The parameter as a string.</param>
        /// <param name="language">The language.</param>
        /// <returns>Visible if true, Collapsed if false.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (targetType.Name == "Visibility")
            {
                if (value is bool)
                {
                    if (parameter != null)
                    {
                        value = !(bool)value;
                    }
                    return (bool)value ? Visibility.Visible : Visibility.Collapsed;
                }
                return Visibility.Collapsed;
            }
            else
            {
                var result = value is Visibility ? (Visibility)value == Visibility.Visible : false;
                return parameter == null ? result : !result;
            }
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
