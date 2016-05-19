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
