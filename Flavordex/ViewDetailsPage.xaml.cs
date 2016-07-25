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
using Flavordex.ViewModels;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Flavordex
{
    /// <summary>
    /// Page to show the details for a journal entry.
    /// </summary>
    public sealed partial class ViewDetailsPage : Page
    {
        /// <summary>
        /// The Entry for which to display the details.
        /// </summary>
        private EntryViewModel Entry { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ViewDetailsPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Loads the requested Entry and realizes the relevant layout when the Page is navigated
        /// to.
        /// </summary>
        /// <param name="e">
        /// The event arguments containing the ID of the requested Entry as the Parameter.
        /// </param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Entry = (EntryViewModel)e.Parameter;
            Entry.Model.RecordChanged += OnRecordChanged;
        }

        /// <summary>
        /// Removes event handlers when the Page is navigated away from.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            Entry.Model.RecordChanged -= OnRecordChanged;
        }

        /// <summary>
        /// Reloads the extra fields when the record is changed.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private async void OnRecordChanged(object sender, EventArgs e)
        {
            Entry.Extras.Clear();
            foreach (var extra in await DatabaseHelper.GetEntryExtrasAsync(Entry.Model.ID))
            {
                Entry.Extras.Add(new EntryExtraItemViewModel(extra));
            }
        }
    }
}
