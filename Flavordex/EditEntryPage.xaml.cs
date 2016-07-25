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
using Flavordex.ViewModels;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Flavordex
{
    /// <summary>
    /// Page for editing a journal entry.
    /// </summary>
    public sealed partial class EditEntryPage : Page
    {
        /// <summary>
        /// Gets or sets the Entry being edited.
        /// </summary>
        public EntryViewModel Entry
        {
            get { return (EntryViewModel)GetValue(EntryProperty); }
            set { SetValue(EntryProperty, value); }
        }
        public static readonly DependencyProperty EntryProperty =
            DependencyProperty.Register("Entry", typeof(EntryViewModel), typeof(EditEntryPage),
                null);

        /// <summary>
        /// Constructor.
        /// </summary>
        public EditEntryPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Activates the back button and loads the Entry when the Page is navigated to.
        /// </summary>
        /// <param name="e">
        /// The event arguments containing the EntryViewModel or Entry ID as the Parameter.
        /// </param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var systemNavigationManager = SystemNavigationManager.GetForCurrentView();
            systemNavigationManager.BackRequested += OnBackRequested;
            systemNavigationManager.AppViewBackButtonVisibility =
                AppViewBackButtonVisibility.Visible;

            if (e.Parameter is long)
            {
                var id = (long)e.Parameter;
                var entry = EntryViewModel.GetInstance(await DatabaseHelper.GetEntryAsync(id));

                foreach (var extra in await DatabaseHelper.GetEntryExtrasAsync(id))
                {
                    entry.Extras.Add(new EntryExtraItemViewModel(extra));
                }

                var entryExtras = new Collection<EntryExtra>();
                var categoryExtras =
                    await DatabaseHelper.GetCategoryExtrasAsync(entry.Model.CategoryID);
                foreach (var item in categoryExtras)
                {
                    var extra = new EntryExtra();
                    extra.ExtraID = item.ID;
                    extra.IsPreset = item.IsPreset;
                    extra.Name = item.Name;

                    var entryExtra = entry.Extras.FirstOrDefault(k => k.Model.ExtraID == item.ID);
                    if (entryExtra != null)
                    {
                        extra.ID = entryExtra.Model.ID;
                        extra.Value = entryExtra.Value;
                        entryExtras.Add(extra);
                    }
                    else if (!item.IsDeleted)
                    {
                        entryExtras.Add(extra);
                    }
                }

                entry.Extras.Clear();
                foreach (var item in entryExtras)
                {
                    entry.Extras.Add(new EntryExtraItemViewModel(item));
                }

                Entry = entry;
            }
        }

        /// <summary>
        /// Deactivates the back button when the Page is navigated away from.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            var systemNavigationManager = SystemNavigationManager.GetForCurrentView();
            systemNavigationManager.BackRequested -= OnBackRequested;
            systemNavigationManager.AppViewBackButtonVisibility =
                AppViewBackButtonVisibility.Collapsed;
        }

        /// <summary>
        /// Navigates back when the back button is pressed.
        /// </summary>
        /// <param name="sender">The SystemNavigationManager.</param>
        /// <param name="e">The event arguments.</param>
        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame.GoBack();
            e.Handled = true;
        }

        /// <summary>
        /// Saves the Entry when the Save button is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnSaveEntry(object sender, RoutedEventArgs e)
        {
            var extras = new Collection<EntryExtra>();
            foreach (var extra in Entry.Extras)
            {
                extras.Add(extra.Model);
            }

            await DatabaseHelper.UpdateEntryAsync(Entry.Model, extras);
            Frame.GoBack();
        }
    }
}
