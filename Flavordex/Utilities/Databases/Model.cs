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

namespace Flavordex.Utilities.Databases
{
    /// <summary>
    /// The base for all Models.
    /// </summary>
    public class Model
    {
        /// <summary>
        /// Occurs when the data in this Model changes.
        /// </summary>
        public event EventHandler RecordChanged = delegate { };

        /// <summary>
        /// Occurs when the record represented by this Model is deleted.
        /// </summary>
        public event EventHandler RecordDeleted = delegate { };

        /// <summary>
        /// The data backing the Model.
        /// </summary>
        protected ContentValues _data = new ContentValues();

        /// <summary>
        /// The primary ID of the table row.
        /// </summary>
        public long ID
        {
            get { return _data.GetLong(BaseColumns._ID); }
            set { _data.SetLong(BaseColumns._ID, value); }
        }

        /// <summary>
        /// Gets a copy of the data backing this Model.
        /// </summary>
        /// <returns>The data backing this Model.</returns>
        public ContentValues GetData()
        {
            return _data.Clone();
        }

        /// <summary>
        /// Sets the date backing this Model.
        /// </summary>
        /// <param name="data">The data.</param>
        public void SetData(ContentValues data)
        {
            _data = data.Clone();
        }

        /// <summary>
        /// Raises the RecordChanged event for this Model.
        /// </summary>
        public void Changed()
        {
            RecordChanged(this, new EventArgs());
        }

        /// <summary>
        /// Raises the RecordDeleted event for this Model.
        /// </summary>
        public void Deleted()
        {
            RecordDeleted(this, new EventArgs());
        }
    }
}
