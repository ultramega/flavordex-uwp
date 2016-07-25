/*
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
            if (parameter is string && value != null
                && !string.IsNullOrWhiteSpace(value.ToString()))
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
