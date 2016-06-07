using Flavordex.Models.Data;
using Flavordex.UI.Controls;
using Flavordex.Utilities;
using Flavordex.ViewModels;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Flavordex
{
    /// <summary>
    /// Page to display the details, flavors, and photos for a journal entry in a tabbed layout.
    /// </summary>
    public sealed partial class ViewEntryPage : Page
    {
        /// <summary>
        /// The Entry to display.
        /// </summary>
        public EntryViewModel Entry
        {
            get { return (EntryViewModel)GetValue(EntryProperty); }
            set { SetValue(EntryProperty, value); }
        }
        public static readonly DependencyProperty EntryProperty =
            DependencyProperty.Register("Entry", typeof(EntryViewModel), typeof(ViewEntryPage), null);

        /// <summary>
        /// Gets or sets whether the details tab is visible.
        /// </summary>
        public bool IsDetailsVisible
        {
            get { return (bool)GetValue(IsDetailsVisibleProperty); }
            set { SetValue(IsDetailsVisibleProperty, value); }
        }
        public static readonly DependencyProperty IsDetailsVisibleProperty =
            DependencyProperty.Register("IsDetailsVisible", typeof(bool), typeof(ViewEntryPage), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets whether the flavors tab is visible.
        /// </summary>
        public bool IsFlavorsVisible
        {
            get { return (bool)GetValue(IsFlavorsVisibleProperty); }
            set { SetValue(IsFlavorsVisibleProperty, value); }
        }
        public static readonly DependencyProperty IsFlavorsVisibleProperty =
            DependencyProperty.Register("IsFlavorsVisible", typeof(bool), typeof(ViewEntryPage), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets whether the photos tab is visible.
        /// </summary>
        public bool IsPhotosVisible
        {
            get { return (bool)GetValue(IsPhotosVisibleProperty); }
            set { SetValue(IsPhotosVisibleProperty, value); }
        }
        public static readonly DependencyProperty IsPhotosVisibleProperty =
            DependencyProperty.Register("IsPhotosVisible", typeof(bool), typeof(ViewEntryPage), new PropertyMetadata(false));

        /// <summary>
        /// The primary ID of the journal entry.
        /// </summary>
        private long _entryId;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ViewEntryPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Loads the requested Entry when the Page is navigated to. Also enables the back button
        /// when the Page is in the narrow state.
        /// </summary>
        /// <param name="e">
        /// The event arguments containing the ID of the requested Entry as the Parameter.
        /// </param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _entryId = (long)e.Parameter;
            Entry = EntryViewModel.GetInstance(await DatabaseHelper.GetEntryAsync(_entryId));

            foreach (var extra in await DatabaseHelper.GetEntryExtrasAsync(_entryId))
            {
                Entry.Extras.Add(new EntryExtraItemViewModel(extra));
            }

            FindName("Pivot");
        }

        /// <summary>
        /// Load the Page for the selected tab as needed when the selected tab changes.
        /// </summary>
        /// <param name="sender">The Pivot.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTabChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = (sender as Pivot).SelectedIndex;

            switch (index)
            {
                case 0:
                    if (DetailsFrame.CurrentSourcePageType != typeof(ViewDetailsPage))
                    {
                        DetailsFrame.Navigate(typeof(ViewDetailsPage), Entry);
                    }
                    break;
                case 1:
                    if (FlavorsFrame.CurrentSourcePageType != typeof(ViewFlavorsPage))
                    {
                        FlavorsFrame.Navigate(typeof(ViewFlavorsPage), Entry.Model);
                    }
                    break;
                case 2:
                    if (PhotosFrame.CurrentSourcePageType != typeof(ViewPhotosPage))
                    {
                        PhotosFrame.Navigate(typeof(ViewPhotosPage), Entry.Model);
                    }
                    break;
            }

            IsDetailsVisible = index == 0;
            IsFlavorsVisible = index == 1;
            IsPhotosVisible = index == 2;
        }

        /// <summary>
        /// Called when the Share Entry button is clicked in the CommandBar.
        /// </summary>
        /// <param name="sender">The AppBarButton.</param>
        /// <param name="e">The event arguments.</param>
        private void OnShareEntry(object sender, RoutedEventArgs e)
        {
            EntryUtilities.ShareEntry(Entry.Model);
        }

        /// <summary>
        /// Called when the Edit Entry button is clicked in the CommandBar.
        /// </summary>
        /// <param name="sender">The AppBarButton.</param>
        /// <param name="e">The event arguments.</param>
        private void OnEditEntry(object sender, RoutedEventArgs e)
        {
            (Window.Current.Content as Frame).Navigate(typeof(EditEntryPage), Entry.Model.ID);
        }

        /// <summary>
        /// Called when the Delete Entry button is clicked in the CommandBar.
        /// </summary>
        /// <param name="sender">The AppBarButton.</param>
        /// <param name="e">The event arguments.</param>
        private async void OnDeleteEntry(object sender, RoutedEventArgs e)
        {
            var result = await new DeleteEntryDialog(Entry.Model).ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                EntryUtilities.DeleteEntry(Entry.Model);
            }
        }

        /// <summary>
        /// Called when the Edit Flavors button is clicked in the CommandBar.
        /// </summary>
        /// <param name="sender">The AppBarButton.</param>
        /// <param name="e">The event arguments.</param>
        private void OnEditFlavors(object sender, RoutedEventArgs e)
        {
            if (FlavorsFrame.CurrentSourcePageType == typeof(ViewFlavorsPage))
            {
                (FlavorsFrame.Content as ViewFlavorsPage).OnEdit(sender, e);
            }
        }

        /// <summary>
        /// Called when the Reset Flavors button is clicked in the CommandBar.
        /// </summary>
        /// <param name="sender">The AppBarButton.</param>
        /// <param name="e">The event arguments.</param>
        private void OnResetFlavors(object sender, RoutedEventArgs e)
        {
            if (FlavorsFrame.CurrentSourcePageType == typeof(ViewFlavorsPage))
            {
                (FlavorsFrame.Content as ViewFlavorsPage).OnReset(sender, e);
            }
        }

        /// <summary>
        /// Called when the Take Photo button is clicked in the CommandBar.
        /// </summary>
        /// <param name="sender">The FlyoutMenuItem.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTakePhoto(object sender, RoutedEventArgs e)
        {
            if (PhotosFrame.CurrentSourcePageType == typeof(ViewPhotosPage))
            {
                (PhotosFrame.Content as ViewPhotosPage).OnTakePhoto(sender, e);
            }
        }

        /// <summary>
        /// Called when the Select Photo button is clicked in the CommandBar.
        /// </summary>
        /// <param name="sender">The FlyoutMenuItem.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSelectPhoto(object sender, RoutedEventArgs e)
        {
            if (PhotosFrame.CurrentSourcePageType == typeof(ViewPhotosPage))
            {
                (PhotosFrame.Content as ViewPhotosPage).OnSelectPhoto(sender, e);
            }
        }
    }
}
