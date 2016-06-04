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
            DependencyProperty.Register("Entry", typeof(EntryViewModel), typeof(LocationField), null);

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
            if (Settings.DetectLocation && Entry.Model.ID == 0 && string.IsNullOrWhiteSpace(Entry.Location))
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
