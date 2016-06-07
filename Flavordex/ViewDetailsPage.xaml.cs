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
