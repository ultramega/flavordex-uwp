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
        /// Converts a boolean value to a Visibility value.
        /// </summary>
        /// <param name="value">The boolean.</param>
        /// <param name="targetType">The data type of the target property.</param>
        /// <param name="parameter">The parameter as a string.</param>
        /// <param name="language">The language.</param>
        /// <returns>Visible if true, Collapsed if false.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
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

        /// <summary>
        /// Converts a Visibility value to a boolean value.
        /// </summary>
        /// <param name="value">The Visibility.</param>
        /// <param name="targetType">The data type of the target property.</param>
        /// <param name="parameter">The parameter as a string.</param>
        /// <param name="language">The language.</param>
        /// <returns>True if Visible, false if Collapsed.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value is Visibility ? (Visibility)value == Visibility.Visible : false;
        }
    }
}
