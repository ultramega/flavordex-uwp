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
            DependencyProperty.Register("Entry", typeof(EntryViewModel), typeof(EditEntryForm), null);

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
        private void OnMakerSelected(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            var maker = args.ChosenSuggestion as Maker;
            Entry.Origin = maker.Location;

            var children = (sender.Parent as Panel).Children;
            var index = children.IndexOf(sender);
            (children[index + 2] as Control).Focus(FocusState.Programmatic);
        }
    }
}
