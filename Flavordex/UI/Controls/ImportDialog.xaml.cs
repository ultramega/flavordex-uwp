using Flavordex.Models.Data;
using Flavordex.Utilities;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Flavordex.UI.Controls
{
    /// <summary>
    /// Dialog for selecting journal entries to import from a CSV file.
    /// </summary>
    public sealed partial class ImportDialog : ContentDialog
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="records">The list of records from a CSV file.</param>
        public ImportDialog(Collection<ImportRecord> records)
        {
            InitializeComponent();
            if (records == null || records.Count == 0)
            {
                EmptyList.Visibility = Visibility.Visible;
            }
            else
            {
                ListView.ItemsSource = records;
            }
        }

        /// <summary>
        /// Enables or disables the Import button when the list selection changes.
        /// </summary>
        /// <param name="sender">The ListView.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IsPrimaryButtonEnabled = ListView.SelectedItems.Count > 0;
        }

        /// <summary>
        /// Imports the selected journal entries when the primary button is clicked.
        /// </summary>
        /// <param name="sender">The Button.</param>
        /// <param name="args">The event arguments.</param>
        private async void OnPrimaryButtonClicked(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var deferral = args.GetDeferral();

            PrimaryButtonText = SecondaryButtonText = "";
            ListView.Visibility = Visibility.Collapsed;
            ProgressBar.Maximum = ListView.SelectedItems.Count;
            ProgressPanel.Visibility = Visibility.Visible;
            foreach (ImportRecord item in ListView.SelectedItems)
            {
                if (await DatabaseHelper.UpdateEntryAsync(item.Entry, item.Extras))
                {
                    await DatabaseHelper.UpdateEntryFlavorsAsync(item.Entry.ID, item.Flavors);
                    foreach (var photo in item.Photos)
                    {
                        await DatabaseHelper.InsertPhotoAsync(photo);
                    }
                }
                ProgressBar.Value++;
            }

            deferral.Complete();
        }
    }
}
