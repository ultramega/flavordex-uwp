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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Flavordex.UI.Controls
{
    /// <summary>
    /// Control to display the input form for editing a journal entry.
    /// </summary>
    public sealed partial class EditEntryForm : UserControl
    {
        /// <summary>
        /// Gets or sets the Entry to edit.
        /// </summary>
        public EntryViewModel Entry
        {
            get { return (EntryViewModel)GetValue(EntryProperty); }
            set { SetValue(EntryProperty, value); }
        }
        public static readonly DependencyProperty EntryProperty =
            DependencyProperty.Register("Entry", typeof(EntryViewModel), typeof(EditEntryForm),
                null);

        /// <summary>
        /// Constructor.
        /// </summary>
        public EditEntryForm()
        {
            InitializeComponent();
            RegisterPropertyChangedCallback(EntryProperty, OnEntryPropertyChanged);
        }

        /// <summary>
        /// Loads the maker suggestions when the Entries property changes.
        /// </summary>
        /// <param name="sender">This UserControl.</param>
        /// <param name="dp">The EntryProperty.</param>
        private async void OnEntryPropertyChanged(DependencyObject sender, DependencyProperty dp)
        {
            Entry.SetMakers(await DatabaseHelper.GetMakersAsync());
        }

        /// <summary>
        /// Sets focus on the title field when it is loaded.
        /// </summary>
        /// <param name="sender">The title TextBox.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTitleLoaded(object sender, RoutedEventArgs e)
        {
            var field = sender as TextBox;
            field.Focus(FocusState.Programmatic);
            field.SelectionStart = field.Text.Length;
        }

        /// <summary>
        /// Updates the title when the field text is changed.
        /// </summary>
        /// <param name="sender">The TextBox.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTitleKeyUp(object sender, KeyRoutedEventArgs e)
        {
            Entry.Title = (sender as TextBox).Text;
        }

        /// <summary>
        /// Sets the origin field when a maker suggestion is selected.
        /// </summary>
        /// <param name="sender">The maker AutoSuggestBox.</param>
        /// <param name="args">The event arguments.</param>
        private void OnMakerSelected(AutoSuggestBox sender,
            AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            var maker = args.ChosenSuggestion as Maker;
            Entry.Origin = maker.Location;

            var children = (sender.Parent as Panel).Children;
            var index = children.IndexOf(sender);
            (children[index + 2] as Control).Focus(FocusState.Programmatic);
        }
    }
}
