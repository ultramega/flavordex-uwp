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
using Flavordex.Models;
using System.Collections.ObjectModel;

namespace Flavordex.Utilities
{
    /// <summary>
    /// Contains data for a record parsed from a CSV file.
    /// </summary>
    public class ImportRecord
    {
        /// <summary>
        /// Gets or sets the journal entry data.
        /// </summary>
        public Entry Entry { get; internal set; }

        /// <summary>
        /// Gets the extra fields for the journal entry.
        /// </summary>
        public Collection<EntryExtra> Extras { get; } = new Collection<EntryExtra>();

        /// <summary>
        /// Gets the flavors for the journal entry.
        /// </summary>
        public Collection<EntryFlavor> Flavors { get; } = new Collection<EntryFlavor>();

        /// <summary>
        /// Gets the photos for the journal entry.
        /// </summary>
        public Collection<Photo> Photos { get; } = new Collection<Photo>();

        /// <summary>
        /// Whether this record has been detected as a duplicate.
        /// </summary>
        public bool IsDuplicate { get; internal set; }
    }
}
