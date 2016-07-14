using Flavordex.Models;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;

namespace Flavordex.UI.Controls
{
    /// <summary>
    /// Dialog to request user confirmation to delete a journal entry.
    /// </summary>
    public sealed partial class DeleteEntryDialog : ContentDialog
    {
        /// <summary>
        /// The format string for the dialog message.
        /// </summary>
        private static readonly string _format =
            ResourceLoader.GetForCurrentView().GetString("Message/DeleteEntry");

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entry">The journal entry to be deleted.</param>
        public DeleteEntryDialog(Entry entry)
        {
            InitializeComponent();
            Content = string.Format(_format, entry.Title);
        }
    }
}
