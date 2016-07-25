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

namespace Flavordex.Models
{
    /// <summary>
    /// Model representing a photo.
    /// </summary>
    public class Photo : Model
    {
        /// <summary>
        /// The primary ID of the journal entry this photo is associated with.
        /// </summary>
        public long EntryID
        {
            get { return _data.GetLong(Tables.Photos.ENTRY); }
            set { _data.SetLong(Tables.Photos.ENTRY, value); }
        }

        /// <summary>
        /// The hash of the photo file.
        /// </summary>
        public string Hash
        {
            get { return _data.GetString(Tables.Photos.HASH); }
            set { _data.SetString(Tables.Photos.HASH, value); }
        }

        /// <summary>
        /// The path to the photo file.
        /// </summary>
        public string Path
        {
            get { return _data.GetString(Tables.Photos.PATH); }
            set { _data.SetString(Tables.Photos.PATH, value); }
        }

        /// <summary>
        /// The Google Drive ID.
        /// </summary>
        public string DriveID
        {
            get { return _data.GetString(Tables.Photos.DRIVE_ID); }
            set { _data.SetString(Tables.Photos.DRIVE_ID, value); }
        }

        /// <summary>
        /// The sorting position in the list of photos.
        /// </summary>
        public long Position
        {
            get { return _data.GetLong(Tables.Photos.POS); }
            set { _data.SetLong(Tables.Photos.POS, value); }
        }
    }
}
