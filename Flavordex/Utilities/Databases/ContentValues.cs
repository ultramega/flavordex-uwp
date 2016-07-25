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
using System.Collections.Generic;

namespace Flavordex.Utilities.Databases
{
    /// <summary>
    /// A Dictionary specialized for SQLite database records.
    /// </summary>
    public class ContentValues : Dictionary<string, object>
    {
        /// <summary>
        /// Creates a copy of this ContentValues.
        /// </summary>
        /// <returns>The copy of this ContentValues.</returns>
        public ContentValues Clone()
        {
            var copy = new ContentValues();
            foreach (var item in this)
            {
                copy[item.Key] = item.Value;
            }
            return copy;
        }

        /// <summary>
        /// Gets or sets an object in the Dictionary. Does not accept null values.
        /// </summary>
        /// <param name="key">The key for the object.</param>
        /// <returns>The object to set.</returns>
        public new object this[string key]
        {
            get
            {
                return base[key];
            }
            set
            {
                if (value != null)
                {
                    base[key] = value;
                }
            }
        }

        /// <summary>
        /// Gets a long value.
        /// </summary>
        /// <param name="key">The key to the value.</param>
        /// <returns>The long value.</returns>
        public long GetLong(string key)
        {
            return GetLong(key, 0);
        }

        /// <summary>
        /// Gets a long value.
        /// </summary>
        /// <param name="key">The key to the value.</param>
        /// <param name="defaultValue">The value to return if the key does not exist.</param>
        /// <returns>The long value.</returns>
        public long GetLong(string key, long defaultValue)
        {
            if (ContainsKey(key))
            {
                return (long)this[key];
            }
            return defaultValue;
        }

        /// <summary>
        /// Gets a double value.
        /// </summary>
        /// <param name="key">The key to the value.</param>
        /// <returns>The double value.</returns>
        public double GetDouble(string key)
        {
            return GetDouble(key, 0);
        }

        /// <summary>
        /// Gets a double value.
        /// </summary>
        /// <param name="key">The key to the value.</param>
        /// <param name="defaultValue">The value to return if the key does not exist.</param>
        /// <returns>The double value.</returns>
        public double GetDouble(string key, double defaultValue)
        {
            if (ContainsKey(key))
            {
                return (double)this[key];
            }
            return defaultValue;
        }

        /// <summary>
        /// Gets a string value.
        /// </summary>
        /// <param name="key">The key to the value.</param>
        /// <returns>The string value.</returns>
        public string GetString(string key)
        {
            return GetString(key, null);
        }

        /// <summary>
        /// Gets a string value.
        /// </summary>
        /// <param name="key">The key to the value.</param>
        /// <param name="defaultValue">The value to return if the key does not exist.</param>
        /// <returns>The string value.</returns>
        public string GetString(string key, string defaultValue)
        {
            if (ContainsKey(key))
            {
                return (string)this[key];
            }
            return defaultValue;
        }

        /// <summary>
        /// Gets a boolean value from a stored integer.
        /// </summary>
        /// <param name="key">The key to the value.</param>
        /// <returns>The boolean value.</returns>
        public bool GetBool(string key)
        {
            return GetBool(key, false);
        }

        /// <summary>
        /// Gets a boolean value from a stored integer.
        /// </summary>
        /// <param name="key">The key to the value.</param>
        /// <param name="defaultValue">The value to return if the key does not exist.</param>
        /// <returns>The boolean value.</returns>
        public bool GetBool(string key, bool defaultValue)
        {
            if (ContainsKey(key))
            {
                return (long)this[key] != 0L;
            }
            return defaultValue;
        }

        /// <summary>
        /// Gets a DateTime value from a stored Unix timestamp.
        /// </summary>
        /// <param name="key">The key to the value.</param>
        /// <returns>The DateTime value.</returns>
        public DateTime GetDateTime(string key)
        {
            return GetDateTime(key, new DateTime());
        }

        /// <summary>
        /// Gets a DateTime value from a stored Unix timestamp.
        /// </summary>
        /// <param name="key">The key to the value.</param>
        /// <param name="defaultValue">The value to return if the key does not exist.</param>
        /// <returns>The DateTime value.</returns>
        public DateTime GetDateTime(string key, DateTime defaultValue)
        {
            if (ContainsKey(key))
            {
                var ticks = (long)this[key] * TimeSpan.TicksPerMillisecond + 621355968000000000;
                return new DateTime(ticks);
            }
            return defaultValue;
        }

        /// <summary>
        /// Sets a long value.
        /// </summary>
        /// <param name="key">The key for the value.</param>
        /// <param name="value">The long value.</param>
        public void SetLong(string key, long value)
        {
            this[key] = value;
        }

        /// <summary>
        /// Sets a double value.
        /// </summary>
        /// <param name="key">The key for the value.</param>
        /// <param name="value">The double value.</param>
        public void SetDouble(string key, double value)
        {
            this[key] = value;
        }

        /// <summary>
        /// Sets a string value.
        /// </summary>
        /// <param name="key">The key for the value.</param>
        /// <param name="value">The string value.</param>
        public void SetString(string key, string value)
        {
            this[key] = value;
        }

        /// <summary>
        /// Sets a boolean value to be stored as an integer.
        /// </summary>
        /// <param name="key">The key for the value.</param>
        /// <param name="value">The boolean value.</param>
        public void SetBool(string key, bool value)
        {
            this[key] = value ? 1L : 0L;
        }

        /// <summary>
        /// Sets a DateTime value to be stored as a Unix timestamp.
        /// </summary>
        /// <param name="key">The key for the value.</param>
        /// <param name="value">The DateTime value.</param>
        public void SetDateTime(string key, DateTime value)
        {
            this[key] = (value.Ticks - 621355968000000000) / TimeSpan.TicksPerMillisecond;
        }
    }
}
