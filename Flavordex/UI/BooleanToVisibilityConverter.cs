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
