﻿/*
  The MIT License (MIT)
  Copyright © 2016 Steve Guidetti

  Permission is hereby granted, free of charge, to any person obtaining a copy
  of this software and associated documentation files (the “Software”), to deal
  in the Software without restriction, including without limitation the rights
  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
  copies of the Software, and to permit persons to whom the Software is
  furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in
  all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
  THE SOFTWARE.
*/
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
        private static readonly string _dateFormat =
            ResourceLoader.GetForCurrentView().GetString("DateFormat");

        /// <summary>
        /// The format string for dates with time.
        /// </summary>
        private static readonly string _dateTimeFormat =
            ResourceLoader.GetForCurrentView().GetString("DateTimeFormat");

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
            var format = parameter is string && (parameter as string).ToLower().Equals("long")
                ? _dateTimeFormat : _dateFormat;
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
