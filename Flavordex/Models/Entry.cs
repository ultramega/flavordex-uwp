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
    /// Model representing a journal entry.
    /// </summary>
    public class Entry : Model
    {
        /// <summary>
        /// The globally unique identifier string.
        /// </summary>
        public string UUID
        {
            get { return _data.GetString(Tables.Entries.UUID); }
            set { _data.SetString(Tables.Entries.UUID, value); }
        }

        /// <summary>
        /// The title of the journal entry.
        /// </summary>
        public string Title
        {
            get { return _data.GetString(Tables.Entries.TITLE); }
            set { _data.SetString(Tables.Entries.TITLE, value); }
        }

        /// <summary>
        /// The primary ID of the journal entry's category.
        /// </summary>
        public long CategoryID
        {
            get { return _data.GetLong(Tables.Entries.CAT_ID); }
            set { _data.SetLong(Tables.Entries.CAT_ID, value); }
        }

        /// <summary>
        /// The UUID of the journal entry's category.
        /// </summary>
        public string CategoryUUID
        {
            get { return _data.GetString(Tables.Entries.CAT_UUID); }
            set { _data.SetString(Tables.Entries.CAT_UUID, value); }
        }

        /// <summary>
        /// The name of the journal entry's category.
        /// </summary>
        public string Category
        {
            get { return _data.GetString(Tables.Entries.CAT); }
            set { _data.SetString(Tables.Entries.CAT, value); }
        }

        /// <summary>
        /// The primary ID of the product's maker.
        /// </summary>
        public long MakerID
        {
            get { return _data.GetLong(Tables.Entries.MAKER_ID); }
            set { _data.SetLong(Tables.Entries.MAKER_ID, value); }
        }

        /// <summary>
        /// The name of the product's maker.
        /// </summary>
        public string Maker
        {
            get { return _data.GetString(Tables.Entries.MAKER); }
            set { _data.SetString(Tables.Entries.MAKER, value); }
        }

        /// <summary>
        /// The location of the product's maker.
        /// </summary>
        public string Origin
        {
            get { return _data.GetString(Tables.Entries.ORIGIN); }
            set { _data.SetString(Tables.Entries.ORIGIN, value); }
        }

        /// <summary>
        /// The product's price.
        /// </summary>
        public string Price
        {
            get { return _data.GetString(Tables.Entries.PRICE); }
            set { _data.SetString(Tables.Entries.PRICE, value); }
        }

        /// <summary>
        /// The tasting location.
        /// </summary>
        public string Location
        {
            get { return _data.GetString(Tables.Entries.LOCATION); }
            set { _data.SetString(Tables.Entries.LOCATION, value); }
        }

        /// <summary>
        /// The tasting date and time.
        /// </summary>
        public DateTime Date
        {
            get { return _data.GetDateTime(Tables.Entries.DATE); }
            set { _data.SetDateTime(Tables.Entries.DATE, value); }
        }

        /// <summary>
        /// The journal entry's rating.
        /// </summary>
        public double Rating
        {
            get { return _data.GetDouble(Tables.Entries.RATING); }
            set { _data.SetDouble(Tables.Entries.RATING, value); }
        }

        /// <summary>
        /// The journal entry's notes.
        /// </summary>
        public string Notes
        {
            get { return _data.GetString(Tables.Entries.NOTES); }
            set { _data.SetString(Tables.Entries.NOTES, value); }
        }

        /// <summary>
        /// The time the journal entry was last updated.
        /// </summary>
        public DateTime Updated
        {
            get { return _data.GetDateTime(Tables.Entries.UPDATED); }
            set { _data.SetDateTime(Tables.Entries.UPDATED, value); }
        }

        /// <summary>
        /// Whether the journal entry has been published to the backend.
        /// </summary>
        public bool IsPublished
        {
            get { return _data.GetBool(Tables.Entries.PUBLISHED); }
            set { _data.SetBool(Tables.Entries.PUBLISHED, value); }
        }

        /// <summary>
        /// Whether the journal entry is synchronized with the backend.
        /// </summary>
        public bool IsSynced
        {
            get { return _data.GetBool(Tables.Entries.SYNCED); }
            set { _data.SetBool(Tables.Entries.SYNCED, value); }
        }
    }
}
