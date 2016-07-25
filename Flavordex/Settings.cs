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
using System.Runtime.CompilerServices;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace Flavordex
{
    /// <summary>
    /// Manages the settings for the application.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// Keys for the available settings.
        /// </summary>
        public struct Key
        {
            public const string ListSortField = "ListSortField";
            public const string ListSortDescending = "ListSortDescending";
            public const string ListCategory = "ListCategory";
            public const string DetectLocation = "DetectLocation";
        }

        /// <summary>
        /// The options available for the ListSortField setting.
        /// </summary>
        public enum SortField
        {
            Name,
            Date,
            Rating
        }

        /// <summary>
        /// Arguments for the SettingChanged event.
        /// </summary>
        public class SettingChangedEventArgs : EventArgs
        {
            /// <summary>
            /// Gets the key to the setting that changed.
            /// </summary>
            public string Key { get; }

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="key">The key to the setting that changed.</param>
            internal SettingChangedEventArgs(string key)
            {
                Key = key;
            }
        }

        /// <summary>
        /// The event handler for the SettingChanged event.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        public delegate void SettingChangedEventHandler(object sender, SettingChangedEventArgs e);

        /// <summary>
        /// Occurs when a setting is charged.
        /// </summary>
        public static event SettingChangedEventHandler SettingChanged = delegate { };

        /// <summary>
        /// Reference to the application settings.
        /// </summary>
        private static IPropertySet _settings = ApplicationData.Current.LocalSettings.Values;

        /// <summary>
        /// Gets or sets the field to use for sorting the list of Entries.
        /// </summary>
        public static SortField ListSortField
        {
            get
            {
                return GetValue(Key.ListSortField, SortField.Name);
            }
            set
            {
                _settings[Key.ListSortField] = (int)value;
                RaiseSettingChanged();
            }
        }

        /// <summary>
        /// Gets or sets whether to sort the list of Entries in descending order.
        /// </summary>
        public static bool ListSortDescending
        {
            get
            {
                return GetValue(Key.ListSortDescending, false);
            }
            set
            {
                _settings[Key.ListSortDescending] = value;
                RaiseSettingChanged();
            }
        }

        /// <summary>
        /// Gets or sets the Category to select when the application starts.
        /// </summary>
        public static long ListCategory
        {
            get
            {
                return GetValue(Key.ListCategory, -1L);
            }
            set
            {
                _settings[Key.ListCategory] = value;
                RaiseSettingChanged();
            }
        }

        /// <summary>
        /// Gets or sets whether to detect and remember the user's location.
        /// </summary>
        public static bool DetectLocation
        {
            get
            {
                return GetValue(Key.DetectLocation, false);
            }
            set
            {
                _settings[Key.DetectLocation] = value;
                RaiseSettingChanged();
            }
        }

        /// <summary>
        /// Raises the SettingChanged event.
        /// </summary>
        /// <param name="key">The key to the setting that changed.</param>
        private static void RaiseSettingChanged([CallerMemberName]string key = null)
        {
            SettingChanged(null, new SettingChangedEventArgs(key));
        }

        /// <summary>
        /// Gets the current value of a setting from the application settings.
        /// </summary>
        /// <typeparam name="T">The data type of the setting.</typeparam>
        /// <param name="key">The key to the setting.</param>
        /// <param name="defaultValue">The default value of the setting.</param>
        /// <returns>The current value of the setting.</returns>
        private static T GetValue<T>(string key, T defaultValue)
        {
            if (_settings.ContainsKey(key))
            {
                return (T)_settings[key];
            }
            return defaultValue;
        }
    }
}
