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
using Flavordex.Models.Data;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources;

namespace Flavordex.Utilities
{
    /// <summary>
    /// Common methods for managing journal entries.
    /// </summary>
    public class EntryUtilities
    {
        /// <summary>
        /// The format string for the sharing subject.
        /// </summary>
        private static readonly string _shareSubjectFormat =
            ResourceLoader.GetForCurrentView().GetString("Share/Subject");

        /// <summary>
        /// The format string for the sharing body.
        /// </summary>
        private static readonly string _shareBodyFormat =
            ResourceLoader.GetForCurrentView().GetString("Share/Body");

        /// <summary>
        /// Opens the sharing UI for a journal entry.
        /// </summary>
        /// <param name="entry">The Entry to share.</param>
        public static void ShareEntry(Entry entry)
        {
            DataTransferManager.GetForCurrentView().DataRequested +=
                (DataTransferManager sender, DataRequestedEventArgs args) =>
            {
                args.Request.Data.Properties.Title =
                    string.Format(_shareSubjectFormat, entry.Title);
                args.Request.Data.SetText(string.Format(_shareBodyFormat, entry.Title,
                    entry.Rating));
            };
            DataTransferManager.ShowShareUI();
        }

        /// <summary>
        /// Deletes a journal entry.
        /// </summary>
        /// <param name="entry">The journal entry.</param>
        public static async void DeleteEntry(Entry entry)
        {
            await DatabaseHelper.DeleteEntryAsync(entry);
            await PhotoUtilities.DeleteThumbnailAsync(entry.ID);
        }
    }
}
