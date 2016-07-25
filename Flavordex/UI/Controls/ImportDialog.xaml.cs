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
using Flavordex.Utilities;
using Flavordex.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.ApplicationModel.Resources;
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
        /// The format string for the default category name.
        /// </summary>
        private static readonly string _defaultCategory =
            ResourceLoader.GetForCurrentView("ImportExport").GetString("DefaultCategory");

        /// <summary>
        /// The format string for the date.
        /// </summary>
        private static readonly string _dateFormat =
            ResourceLoader.GetForCurrentView().GetString("DateFormat");

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="collection">The ImportCollection.</param>
        public ImportDialog(ImportCollection collection)
        {
            InitializeComponent();
            if (collection == null || collection.Entries.Count == 0)
            {
                EmptyList.Visibility = Visibility.Visible;
            }
            else
            {
                ListView.ItemsSource = collection.Entries;
            }

            if (collection.Entries.Any(e => string.IsNullOrWhiteSpace(e.Entry.Category)))
            {
                ShowCategories();
            }
        }

        /// <summary>
        /// Loads the list of categories to set the default.
        /// </summary>
        private async void ShowCategories()
        {
            var categories = new Collection<CategoryItemViewModel>();
            foreach (var item in await DatabaseHelper.GetCategoryListAsync())
            {
                categories.Add(new CategoryItemViewModel(item));
            }
            foreach (var item in categories.OrderBy(e => e.Name))
            {
                CategoryList.Items.Add(item);
            }
            if (Settings.ListCategory > 0)
            {
                CategoryList.SelectedItem =
                    categories.FirstOrDefault(e => e.Model.ID == Settings.ListCategory);
            }
            CategoryList.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Checks all items and checks for duplicates when the ListView is loaded.
        /// </summary>
        /// <param name="sender">The ListView.</param>
        /// <param name="e">The event arguments.</param>
        private void OnListLoaded(object sender, RoutedEventArgs e)
        {
            ListView.SelectAll();

            var duplicates = 0;
            foreach (ImportRecord item in ListView.Items)
            {
                if (item.IsDuplicate)
                {
                    duplicates++;
                }
            }
            if (duplicates > 0)
            {
                var format = ResourceLoader.GetForCurrentView("ImportExport")
                    .GetString("Button/UncheckDuplicates");
                DuplicateButton.Content =
                    string.Format(format, duplicates, Plurals.GetWord("Duplicates", duplicates));
                DuplicateButton.Visibility = Visibility.Visible;
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
        /// Unchecks duplicate entries when the Uncheck Duplicates button is clicked.
        /// </summary>
        /// <param name="sender">The Button.</param>
        /// <param name="e">The event arguments.</param>
        private void OnUncheckDuplicates(object sender, RoutedEventArgs e)
        {
            foreach (ImportRecord item in ListView.Items)
            {
                if (item.IsDuplicate)
                {
                    ListView.SelectedItems.Remove(item);
                }
            }
        }

        /// <summary>
        /// Imports the selected journal entries when the primary button is clicked.
        /// </summary>
        /// <param name="sender">The Button.</param>
        /// <param name="args">The event arguments.</param>
        private async void OnPrimaryButtonClicked(ContentDialog sender,
            ContentDialogButtonClickEventArgs args)
        {
            var deferral = args.GetDeferral();

            var defaultCategory =
                string.Format(_defaultCategory, DateTime.Now.ToString(_dateFormat));
            if (CategoryList.SelectedItem != null)
            {
                defaultCategory = (CategoryList.SelectedItem as CategoryItemViewModel).Model.Name;
            }

            PrimaryButtonText = SecondaryButtonText = "";
            ListPanel.Visibility = Visibility.Collapsed;
            ProgressBar.Maximum = ListView.SelectedItems.Count;
            ProgressPanel.Visibility = Visibility.Visible;
            foreach (ImportRecord item in ListView.SelectedItems)
            {
                if (string.IsNullOrWhiteSpace(item.Entry.Category))
                {
                    item.Entry.Category = defaultCategory;
                }

                if (await DatabaseHelper.UpdateEntryAsync(item.Entry, item.Extras, item.Flavors))
                {
                    var position = 0;
                    foreach (var photoItem in item.Photos)
                    {
                        var photo = await PhotoUtilities.AddPhotoAsync(photoItem.Path,
                            item.Entry.ID, position);
                        if (photo == null)
                        {
                            photo = new Models.Photo()
                            {
                                EntryID = item.Entry.ID,
                                Path = photoItem.Path
                            };
                            await DatabaseHelper.UpdatePhotoAsync(photo);
                        }
                    }
                    await PhotoUtilities.DeleteThumbnailAsync(item.Entry.ID);
                }
                ProgressBar.Value++;
            }

            deferral.Complete();
        }
    }
}
