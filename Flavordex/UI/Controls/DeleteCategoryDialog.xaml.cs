using Flavordex.Models;
using Flavordex.Utilities;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;

namespace Flavordex.UI.Controls
{
    /// <summary>
    /// Dialog to request user confirmation to delete a Category.
    /// </summary>
    public sealed partial class DeleteCategoryDialog : ContentDialog
    {
        /// <summary>
        /// The format string for the dialog message.
        /// </summary>
        private static readonly string _messageFormat =
            ResourceLoader.GetForCurrentView().GetString("Message/ConfirmDeleteCategory");

        /// <summary>
        /// The format string for the confirmation check box message.
        /// </summary>
        private static readonly string _confirmMessageFormat =
            ResourceLoader.GetForCurrentView().GetString("Message/DeleteCategoryEntries");

        /// <summary>
        /// Gets the dialog message.
        /// </summary>
        private string Message { get; }

        /// <summary>
        /// Gets the confirmation check box message.
        /// </summary>
        private string ConfirmMessage { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="category">The Category to be deleted.</param>
        public DeleteCategoryDialog(Category category)
        {
            InitializeComponent();
            Message = string.Format(_messageFormat, category.Name);
            if (category.EntryCount > 0)
            {
                ConfirmMessage = string.Format(_confirmMessageFormat, category.EntryCount,
                    Plurals.GetWord("entries", category.EntryCount));
            }
            else
            {
                Confirm.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                Confirm.IsChecked = true;
            }
        }
    }
}
