using Flavordex.Models;
using Flavordex.Models.Data;
using Flavordex.UI;
using Flavordex.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Flavordex
{
    /// <summary>
    /// Page to display the radar graph of flavors for a journal entry.
    /// </summary>
    public sealed partial class ViewFlavorsPage : Page
    {
        /// <summary>
        /// Gets or sets the items to display in the RadarControl.
        /// </summary>
        public Collection<RadarItem> Flavors
        {
            get { return (Collection<RadarItem>)GetValue(FlavorsProperty); }
            set { SetValue(FlavorsProperty, value); }
        }
        public static readonly DependencyProperty FlavorsProperty =
            DependencyProperty.Register("Flavors", typeof(Collection<RadarItem>), typeof(ViewFlavorsPage), null);

        /// <summary>
        /// The journal entry.
        /// </summary>
        private Entry _entry;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ViewFlavorsPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Loads the Flavors for the requested Entry when the Page is navigated to.
        /// </summary>
        /// <param name="e">
        /// The event arguments containing the requested Entry as the Parameter.
        /// </param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _entry = e.Parameter as Entry;
            LoadFlavors();
            _entry.RecordChanged += OnRecordChanged;
        }

        /// <summary>
        /// Removes the RecordChanged event handler when the Page is navigated away from.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            _entry.RecordChanged -= OnRecordChanged;
        }

        /// <summary>
        /// Reloads the flavors when the Entry changes.
        /// </summary>
        /// <param name="sender">The Entry.</param>
        /// <param name="e">The event arguments.</param>
        private void OnRecordChanged(object sender, EventArgs e)
        {
            if (!Radar.IsInteractive)
            {
                LoadFlavors();
            }
        }

        /// <summary>
        /// Opens the context menu when the pointer is held down on the RadarGraph.
        /// </summary>
        /// <param name="sender">The RadarGraph.</param>
        /// <param name="e">The event arguments.</param>
        private void OnHolding(object sender, HoldingRoutedEventArgs e)
        {
            RadarFlyout.ShowAt(Radar, e.GetPosition(Radar));
        }

        /// <summary>
        /// Opens the context menu when the RadarGraph is right-clicked.
        /// </summary>
        /// <param name="sender">The RadarGraph.</param>
        /// <param name="e">The event arguments.</param>
        private void OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            RadarFlyout.ShowAt(Radar, e.GetPosition(Radar));
        }

        /// <summary>
        /// Enables interactive mode on the RadarGraph.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        public void OnEdit(object sender, RoutedEventArgs e)
        {
            Radar.IsInteractive = true;
        }

        /// <summary>
        /// Resets the RadarGraph to the current Category defaults after requesting confirmation
        /// from the user.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        public async void OnReset(object sender, RoutedEventArgs e)
        {
            var result = await ResetDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                Radar.IsInteractive = true;
                Flavors = new ObservableCollection<RadarItem>(await DatabaseHelper.GetCategoryFlavorsAsync(_entry.CategoryID));
            }
        }

        /// <summary>
        /// Updates the Flavors in the database when the Save button is pressed.
        /// </summary>
        /// <param name="sender">The RadarGraphEditor.</param>
        /// <param name="e">The event arguments.</param>
        private async void OnSaveFlavors(object sender, RoutedEventArgs e)
        {
            var flavors = new Collection<EntryFlavorItemViewModel>(Flavors.Cast<EntryFlavorItemViewModel>().ToList());
            await DatabaseHelper.InsertFlavorsAsync(_entry.ID, flavors);
        }

        /// <summary>
        /// Loads the Flavors for the Entry.
        /// </summary>
        private async void LoadFlavors()
        {
            var flavors = await DatabaseHelper.GetEntryFlavorsAsync(_entry.ID);
            Flavors = new Collection<RadarItem>(flavors.Cast<RadarItem>().ToList());
        }
    }
}
