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
using Flavordex.Models.Data;
using Flavordex.Utilities.Databases;
using System;

namespace Flavordex.Models
{
    /// <summary>
    /// Model representing a category.
    /// </summary>
    public class Category : Model
    {
        /// <summary>
        /// The globally unique identifier string.
        /// </summary>
        public string UUID
        {
            get { return _data.GetString(Tables.Cats.UUID); }
            set { _data.SetString(Tables.Cats.UUID, value); }
        }

        /// <summary>
        /// The name of the category.
        /// </summary>
        public string Name
        {
            get { return _data.GetString(Tables.Cats.NAME); }
            set { _data.SetString(Tables.Cats.NAME, value); }
        }

        /// <summary>
        /// Whether this is a preset category.
        /// </summary>
        public bool IsPreset
        {
            get { return _data.GetBool(Tables.Cats.PRESET); }
            set { _data.SetBool(Tables.Cats.PRESET, value); }
        }

        /// <summary>
        /// The time the category was last updated.
        /// </summary>
        public DateTime Updated
        {
            get { return _data.GetDateTime(Tables.Cats.UPDATED); }
            set { _data.SetDateTime(Tables.Cats.UPDATED, value); }
        }

        /// <summary>
        /// Whether the category has been published to the backend.
        /// </summary>
        public bool IsPublished
        {
            get { return _data.GetBool(Tables.Cats.PUBLISHED); }
            set { _data.SetBool(Tables.Cats.PUBLISHED, value); }
        }

        /// <summary>
        /// Whether the category is synchronized with the backend.
        /// </summary>
        public bool IsSynced
        {
            get { return _data.GetBool(Tables.Cats.SYNCED); }
            set { _data.SetBool(Tables.Cats.SYNCED, value); }
        }

        /// <summary>
        /// The number of journal entries in the category.
        /// </summary>
        public long EntryCount
        {
            get { return _data.GetLong(Tables.Cats.NUM_ENTRIES); }
            set { _data.SetLong(Tables.Cats.NUM_ENTRIES, value); }
        }
    }
}
