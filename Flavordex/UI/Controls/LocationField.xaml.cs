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
using Flavordex.Utilities;
using Flavordex.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Flavordex.UI.Controls
{
    /// <summary>
    /// Input field for inputting the tasting location for a journal entry.
    /// </summary>
    public sealed partial class LocationField : UserControl
    {
        /// <summary>
        /// Gets or sets the journal entry to be edited.
        /// </summary>
        public EntryViewModel Entry
        {
            get { return (EntryViewModel)GetValue(EntryProperty); }
            set { SetValue(EntryProperty, value); }
        }
        public static readonly DependencyProperty EntryProperty =
            DependencyProperty.Register("Entry", typeof(EntryViewModel), typeof(LocationField),
                null);

        /// <summary>
        /// Constructor.
        /// </summary>
        public LocationField()
        {
            InitializeComponent();
            RegisterPropertyChangedCallback(EntryProperty, OnEntryPropertyChanged);
        }

        /// <summary>
        /// Updates the location field with the detected location if the Detect Location setting is
        /// enabled, the Entry is new, and the location value is empty when the Entry property is
        /// changed.
        /// </summary>
        /// <param name="sender">This Control.</param>
        /// <param name="dp">The EntryProperty.</param>
        private async void OnEntryPropertyChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (Settings.DetectLocation && Entry.Model.ID == 0
                && string.IsNullOrWhiteSpace(Entry.Location))
            {
                Progress.IsActive = true;

                await LocationUtilities.UpdateLocationAsync();
                if (string.IsNullOrWhiteSpace(Entry.Location))
                {
                    Entry.Location = LocationUtilities.LocationName;
                }

                Progress.IsActive = false;
            }
        }
    }
}
