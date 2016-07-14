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
