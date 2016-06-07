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
            DependencyProperty.Register("Entry", typeof(EntryViewModel), typeof(EditEntryPage), null);

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
            systemNavigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;

            EntryViewModel entry = null;
            if (e.Parameter is long)
            {
                entry = EntryViewModel.GetInstance(await DatabaseHelper.GetEntryAsync((long)e.Parameter));

                foreach (var extra in await DatabaseHelper.GetEntryExtrasAsync((long)e.Parameter))
                {
                    entry.Extras.Add(new EntryExtraItemViewModel(extra));
                }
            }
            else if (e.Parameter is EntryViewModel)
            {
                entry = (EntryViewModel)e.Parameter;
            }

            var entryExtras = new Collection<EntryExtra>();
            var categoryExtras = await DatabaseHelper.GetCategoryExtrasAsync(entry.Model.CategoryID, true);
            foreach (var item in categoryExtras)
            {
                var extra = new EntryExtra();
                extra.ExtraID = item.ID;
                extra.IsPreset = item.IsPreset;
                extra.Name = item.Name;

                var entryExtra = entry.Extras.FirstOrDefault(k => k.Model.ExtraID == item.ID);
                if (entryExtra != null)
                {
                    extra.Value = entryExtra.Value;
                }

                entryExtras.Add(extra);
            }

            entry.Extras.Clear();
            foreach (var item in entryExtras)
            {
                entry.Extras.Add(new EntryExtraItemViewModel(item));
            }

            Entry = entry;
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
            systemNavigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
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
